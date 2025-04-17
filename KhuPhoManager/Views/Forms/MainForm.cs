using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;

namespace KhuPhoManager.Views.Forms
{
    /// <summary>
    /// Main form for the KhuPho Manager application with a modern, professional UI
    /// </summary>
    public partial class MainForm : Form
    {
        // Controller
        private readonly GuiNeighborhoodController _controller;
        
        // UI Colors
        private readonly Color _primaryColor = Color.FromArgb(24, 90, 157);       // Dark blue
        private readonly Color _secondaryColor = Color.FromArgb(17, 53, 71);      // Even darker blue
        private readonly Color _highlightColor = Color.FromArgb(86, 204, 242);    // Light blue
        private readonly Color _textColor = Color.FromArgb(238, 238, 238);        // Off-white
        private readonly Color _panelColor = Color.FromArgb(247, 250, 252);       // Very light blue-gray
        private readonly Color _accentColor = Color.FromArgb(255, 74, 130);       // Pink accent
        
        // UI Elements
        private Panel sidePanel;
        private Panel headerPanel;
        private Panel mainPanel;
        private Panel dashboardPanel;
        private Panel householdsPanel;
        private Panel peoplePanel;
        private Panel reportsPanel;
        private Panel settingsPanel;
        private Panel activePanel;
        private Panel navIndicator;
        
        // Navigation buttons
        private List<Button> navigationButtons;
        private Button activeButton;
        private Button dashboardButton;
        private Button householdsButton;
        private Button peopleButton;
        private Button reportsButton;
        private Button settingsButton;
        
        // Dashboard controls
        private Panel statisticsPanel;
        private ListView householdListView;
        private Label statusLabel;
        private System.Windows.Forms.Timer refreshTimer;
        
        /// <summary>
        /// Constructor for the MainForm
        /// </summary>
        /// <param name="controller">The neighborhood controller</param>
        public MainForm(GuiNeighborhoodController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            
            InitializeComponent();
            InitializeCustomComponents();
            SetupEventHandlers();
            
            // Initial data load
            RefreshDashboard();
        }
        
        /// <summary>
        /// Set up event handlers for form controls
        /// </summary>
        private void SetupEventHandlers()
        {
            // Navigation button click events
            dashboardButton.Click += (sender, e) => {
                SetActiveButton(dashboardButton);
                ShowPanel(dashboardPanel);
                RefreshDashboard();
            };
            
            householdsButton.Click += (sender, e) => {
                SetActiveButton(householdsButton);
                ShowPanel(householdsPanel);
                RefreshDashboard();
            };
            
            peopleButton.Click += (sender, e) => {
                SetActiveButton(peopleButton);
                ShowPanel(peoplePanel);
            };
            
            reportsButton.Click += (sender, e) => {
                SetActiveButton(reportsButton);
                ShowPanel(reportsPanel);
            };
            
            settingsButton.Click += (sender, e) => {
                SetActiveButton(settingsButton);
                ShowPanel(settingsPanel);
            };
            
            // Set up refresh timer
            refreshTimer.Tick += (sender, e) => {
                if (activePanel == dashboardPanel || activePanel == householdsPanel)
                {
                    RefreshDashboard();
                }
            };
            
            // Form resize event
            this.Resize += (sender, e) => {
                RefreshStatistics();
            };
        }
        
        /// <summary>
        /// Sets the active navigation button with visual indicator
        /// </summary>
        private void SetActiveButton(Button button)
        {
            if (activeButton != null)
            {
                // Reset previous active button
                activeButton.BackColor = _secondaryColor;
                activeButton.ForeColor = _textColor;
            }
            
            // Set new active button
            activeButton = button;
            activeButton.BackColor = _primaryColor;
            activeButton.ForeColor = _highlightColor;
            
            // Position navigation indicator
            navIndicator.Height = button.Height;
            navIndicator.Top = button.Top;
            navIndicator.Left = 0;
            navIndicator.Visible = true;
        }
        
        /// <summary>
        /// Shows the specified panel and hides all others
        /// </summary>
        private void ShowPanel(Panel panel)
        {
            // Hide all panels
            dashboardPanel.Visible = false;
            peoplePanel.Visible = false;
            reportsPanel.Visible = false;
            settingsPanel.Visible = false;
            
            // Show the selected panel
            panel.Visible = true;
            activePanel = panel;
            
            // Update status label
            UpdateStatusLabel();
        }
        
        /// <summary>
        /// Updates the status label with current information
        /// </summary>
        private void UpdateStatusLabel()
        {
            string panelName = "Unknown";
            
            if (activePanel == dashboardPanel || activePanel == householdsPanel)
                panelName = "Dashboard";
            else if (activePanel == peoplePanel)
                panelName = "People Management";
            else if (activePanel == reportsPanel)
                panelName = "Reports";
            else if (activePanel == settingsPanel)
                panelName = "Settings";
                
            statusLabel.Text = $"{panelName} | Households: {_controller.GetHouseholdCount()} | Population: {_controller.GetTotalPopulation()}";
        }
        
        /// <summary>
        /// Initialize the form components
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form settings
            this.Text = "Neighborhood Manager";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 700);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.BackColor = _panelColor;
            
            // Try to load icon if available
            try
            {
                this.Icon = new Icon(GetType(), "house.ico");
            }
            catch
            {
                // Icon not found, continue without it
            }
            
            this.ResumeLayout(false);
        }
        
        /// <summary>
        /// Initialize custom UI components
        /// </summary>
        private void InitializeCustomComponents()
        {
            // Initialize main panels
            InitializeMainLayout();
            
            // Initialize navigation
            InitializeNavigation();
            
            // Initialize content panels
            InitializeDashboardPanel();
            InitializeHouseholdsPanel();
            InitializePeoplePanel();
            InitializeReportsPanel();
            InitializeSettingsPanel();
            
            // Set up refresh timer
            refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 30000, // 30 seconds
                Enabled = true
            };
            
            // Set initial active panel
            SetActiveButton(dashboardButton);
            ShowPanel(dashboardPanel);
        }
        
        /// <summary>
        /// Initialize the main layout panels
        /// </summary>
        private void InitializeMainLayout()
        {
            // Header panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = _primaryColor
            };
            
            // Add app title to header
            Label titleLabel = new Label
            {
                Text = "Neighborhood Manager",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = _textColor,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            
            // Status label in header
            statusLabel = new Label
            {
                Text = "Ready",
                Font = new Font("Segoe UI", 9),
                ForeColor = _textColor,
                AutoSize = true,
                Location = new Point(this.Width - 150, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(statusLabel);
            
            // Side panel for navigation
            sidePanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = _secondaryColor
            };
            
            // Main content panel
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _panelColor,
                Padding = new Padding(15)
            };
            
            // Add panels to form
            this.Controls.Add(mainPanel);
            this.Controls.Add(sidePanel);
            this.Controls.Add(headerPanel);
        }
        
        /// <summary>
        /// Initialize the navigation sidebar
        /// </summary>
        private void InitializeNavigation()
        {
            // Logo panel at the top of sidebar
            Panel logoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = _secondaryColor
            };
            
            // App logo label
            Label logoLabel = new Label
            {
                Text = "KhuPho",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = _highlightColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            logoPanel.Controls.Add(logoLabel);
            sidePanel.Controls.Add(logoPanel);
            
            // Navigation indicator panel
            navIndicator = new Panel
            {
                BackColor = _highlightColor,
                Size = new Size(7, 45),
                Location = new Point(0, 0),
                Visible = false
            };
            
            sidePanel.Controls.Add(navIndicator);
            
            // Navigation buttons
            navigationButtons = new List<Button>();
            
            dashboardButton = CreateNavButton("Dashboard", "", 0);
            householdsButton = CreateNavButton("Households", "", 1);
            peopleButton = CreateNavButton("People", "", 2);
            reportsButton = CreateNavButton("Reports", "", 3);
            settingsButton = CreateNavButton("Settings", "", 4);
            
            // Add buttons to side panel
            sidePanel.Controls.Add(dashboardButton);
            sidePanel.Controls.Add(householdsButton);
            sidePanel.Controls.Add(peopleButton);
            sidePanel.Controls.Add(reportsButton);
            sidePanel.Controls.Add(settingsButton);
        }
        
        /// <summary>
        /// Initialize the dashboard panel
        /// </summary>
        private void InitializeDashboardPanel()
        {
            dashboardPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _panelColor,
                Visible = false
            };
            
            // Dashboard title
            Label dashboardTitle = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = _primaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 5, 0, 0)
            };
            
            // Statistics panel
            statisticsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                Padding = new Padding(0, 10, 0, 10)
            };
            
            // Household list panel
            Panel householdListPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };
            
            // Household list title
            Label householdListTitle = new Label
            {
                Text = "Households",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = _primaryColor,
                Dock = DockStyle.Top,
                Height = 30
            };
            
            // Household ListView
            householdListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9)
            };
            
            // Add columns to the ListView
            householdListView.Columns.Add("House #", 80);
            householdListView.Columns.Add("Address", 250);
            householdListView.Columns.Add("Adults", 80);
            householdListView.Columns.Add("Children", 80);
            householdListView.Columns.Add("Total", 80);
            
            // Action buttons panel
            Panel actionButtonsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(0, 10, 0, 0)
            };
            
            // Add household button
            Button addHouseholdButton = new Button
            {
                Text = "Add Household",
                BackColor = _primaryColor,
                ForeColor = _textColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Location = new Point(0, 10),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            addHouseholdButton.FlatAppearance.BorderSize = 0;
            
            // Remove household button
            Button removeHouseholdButton = new Button
            {
                Text = "Remove Household",
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = _textColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Location = new Point(130, 10),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            removeHouseholdButton.FlatAppearance.BorderSize = 0;
            
            // View details button
            Button viewDetailsButton = new Button
            {
                Text = "View Details",
                BackColor = _highlightColor,
                ForeColor = _secondaryColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Location = new Point(260, 10),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            viewDetailsButton.FlatAppearance.BorderSize = 0;
            
            // Add buttons to action panel
            actionButtonsPanel.Controls.Add(addHouseholdButton);
            actionButtonsPanel.Controls.Add(removeHouseholdButton);
            actionButtonsPanel.Controls.Add(viewDetailsButton);
            
            // Add controls to household list panel
            householdListPanel.Controls.Add(householdListView);
            householdListPanel.Controls.Add(actionButtonsPanel);
            householdListPanel.Controls.Add(householdListTitle);
            
            // Add controls to dashboard panel
            dashboardPanel.Controls.Add(householdListPanel);
            dashboardPanel.Controls.Add(statisticsPanel);
            dashboardPanel.Controls.Add(dashboardTitle);
            
            // Add dashboard panel to main panel
            mainPanel.Controls.Add(dashboardPanel);
            
            // Set up event handlers
            addHouseholdButton.Click += (sender, e) => {
                AddHouseholdForm addHouseholdForm = new AddHouseholdForm(_controller);
                addHouseholdForm.ShowDialog();
                RefreshDashboard();
            };
            
            removeHouseholdButton.Click += (sender, e) => {
                RemoveHouseholdForm removeHouseholdForm = new RemoveHouseholdForm(_controller);
                removeHouseholdForm.ShowDialog();
                RefreshDashboard();
            };
            
            viewDetailsButton.Click += (sender, e) => {
                if (householdListView.SelectedItems.Count > 0)
                {
                    int houseNumber = int.Parse(householdListView.SelectedItems[0].Text);
                    var household = _controller.GetHouseholds().FirstOrDefault(h => h.HouseNumber == houseNumber);
                    
                    if (household != null)
                    {
                        HouseholdDetailsForm detailsForm = new HouseholdDetailsForm(_controller, household);
                        detailsForm.ShowDialog();
                        RefreshDashboard();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a household first.", "No Selection", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            
            householdListView.DoubleClick += (sender, e) => {
                viewDetailsButton.PerformClick();
            };
        }
        
        /// <summary>
        /// Initialize the households panel
        /// </summary>
        private void InitializeHouseholdsPanel()
        {
            // We're using the dashboard panel for households as well
            // This method is here for consistency and future expansion
            householdsPanel = dashboardPanel;
        }
        
        /// <summary>
        /// Initialize the people management panel
        /// </summary>
        private void InitializePeoplePanel()
        {
            peoplePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _panelColor,
                Visible = false
            };
            
            // People panel title
            Label peopleTitle = new Label
            {
                Text = "People Management",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = _primaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 5, 0, 0)
            };
            
            // Create a flow layout panel for action cards
            FlowLayoutPanel actionCardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                Padding = new Padding(0, 20, 0, 0)
            };
            
            // Add action cards
            Panel addPersonCard = CreateActionCard(
                "Add Person", 
                "Add a new person to a household", 
                "",
                (sender, e) => {
                    // Show dialog to select a household first
                    using (Form promptForm = new Form())
                    {
                        promptForm.Width = 350;
                        promptForm.Height = 180;
                        promptForm.Text = "Select Household";
                        promptForm.StartPosition = FormStartPosition.CenterParent;
                        promptForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                        promptForm.MaximizeBox = false;
                        promptForm.MinimizeBox = false;
                        
                        Label promptLabel = new Label() { Left = 20, Top = 20, Width = 310, Text = "Enter the house number to add a person to:" };
                        NumericUpDown houseNumberInput = new NumericUpDown() { Left = 20, Top = 50, Width = 310, Minimum = 1, Maximum = 9999 };
                        Button okButton = new Button() { Text = "OK", Left = 155, Width = 80, Top = 90, DialogResult = DialogResult.OK };
                        Button cancelButton = new Button() { Text = "Cancel", Left = 250, Width = 80, Top = 90, DialogResult = DialogResult.Cancel };
                        
                        promptForm.Controls.Add(promptLabel);
                        promptForm.Controls.Add(houseNumberInput);
                        promptForm.Controls.Add(okButton);
                        promptForm.Controls.Add(cancelButton);
                        promptForm.AcceptButton = okButton;
                        promptForm.CancelButton = cancelButton;
                        
                        if (promptForm.ShowDialog() == DialogResult.OK)
                        {
                            int houseNumber = (int)houseNumberInput.Value;
                            

                            // Check if the household exists
                            var household = _controller.GetHouseholds().FirstOrDefault(h => h.HouseNumber == houseNumber);
                            if (household != null)
                            {
                                AddPersonToHouseholdForm addPersonForm = new AddPersonToHouseholdForm(_controller, houseNumber);
                                addPersonForm.ShowDialog();
                                RefreshDashboard();
                            }
                            else
                            {
                                MessageBox.Show($"Household #{houseNumber} does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                });
                
            Panel findPersonCard = CreateActionCard(
                "Find Person", 
                "Search for a person by ID or name", 
                "",
                (sender, e) => {
                    FindPersonForm findPersonForm = new FindPersonForm(_controller);
                    findPersonForm.ShowDialog();
                });
                
            Panel sortByAgeCard = CreateActionCard(
                "Sort By Age", 
                "View people sorted by their age", 
                "",
                (sender, e) => {
                    SortByAgeForm sortByAgeForm = new SortByAgeForm(_controller);
                    sortByAgeForm.ShowDialog();
                });
            
            // Add cards to the flow panel
            actionCardsPanel.Controls.Add(addPersonCard);
            actionCardsPanel.Controls.Add(findPersonCard);
            actionCardsPanel.Controls.Add(sortByAgeCard);
            
            // Add controls to the people panel
            peoplePanel.Controls.Add(actionCardsPanel);
            peoplePanel.Controls.Add(peopleTitle);
            
            // Add people panel to main panel
            mainPanel.Controls.Add(peoplePanel);
        }
        
        /// <summary>
        /// Initialize the reports panel
        /// </summary>
        private void InitializeReportsPanel()
        {
            reportsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _panelColor,
                Visible = false
            };
            
            // Reports panel title
            Label reportsTitle = new Label
            {
                Text = "Reports & Statistics",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = _primaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 5, 0, 0)
            };
            
            // Create a flow layout panel for report cards
            FlowLayoutPanel reportCardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                Padding = new Padding(0, 20, 0, 0)
            };
            
            // Add report cards
            Panel statsCard = CreateActionCard(
                "Neighborhood Statistics", 
                "View detailed statistics about the neighborhood", 
                "",
                (sender, e) => {
                    NeighborhoodStatsForm statsForm = new NeighborhoodStatsForm(_controller);
                    statsForm.ShowDialog();
                });
                
            Panel memberAnalysisCard = CreateActionCard(
                "Household Size Analysis", 
                "Find households with most and fewest members", 
                "",
                (sender, e) => {
                    MostFewestMembersForm membersForm = new MostFewestMembersForm(_controller);
                    membersForm.ShowDialog();
                });
            
            // Add cards to the flow panel
            reportCardsPanel.Controls.Add(statsCard);
            reportCardsPanel.Controls.Add(memberAnalysisCard);
            
            // Add controls to the reports panel
            reportsPanel.Controls.Add(reportCardsPanel);
            reportsPanel.Controls.Add(reportsTitle);
            
            // Add reports panel to main panel
            mainPanel.Controls.Add(reportsPanel);
        }
        
        /// <summary>
        /// Initialize the settings panel
        /// </summary>
        private void InitializeSettingsPanel()
        {
            settingsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _panelColor,
                Visible = false
            };
            
            // Settings panel title
            Label settingsTitle = new Label
            {
                Text = "Settings & File Operations",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = _primaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 5, 0, 0)
            };
            
            // Create a flow layout panel for setting cards
            FlowLayoutPanel settingCardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                Padding = new Padding(0, 20, 0, 0)
            };
            
            // Add setting cards
            Panel openFileCard = CreateActionCard(
                "Open File", 
                "Load neighborhood data from a file", 
                "",
                (sender, e) => {
                    // Create an open file dialog
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                        openFileDialog.Title = "Open Neighborhood Data File";
                        
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string filename = openFileDialog.FileName;
                            ReadFileForm readFileForm = new ReadFileForm(_controller, filename);
                            readFileForm.ShowDialog();
                            RefreshDashboard();
                        }
                    }
                });
                
            Panel saveFileCard = CreateActionCard(
                "Save File", 
                "Save neighborhood data to a file", 
                "",
                (sender, e) => {
                    // Create a save file dialog
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                        saveFileDialog.Title = "Save Neighborhood Data File";
                        saveFileDialog.DefaultExt = "csv";
                        
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string filename = saveFileDialog.FileName;
                            SaveFileForm saveFileForm = new SaveFileForm(_controller, filename);
                            saveFileForm.ShowDialog();
                        }
                    }
                });
                
            Panel aboutCard = CreateActionCard(
                "About", 
                "View information about this application", 
                "",
                (sender, e) => {
                    MessageBox.Show(
                        "Neighborhood Manager Application\nVersion 1.0\n\n" +
                        "A professional application for managing neighborhood data\n" +
                        "Developed for KhuPhoManager", 
                        "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
            
            // Add cards to the flow panel
            settingCardsPanel.Controls.Add(openFileCard);
            settingCardsPanel.Controls.Add(saveFileCard);
            settingCardsPanel.Controls.Add(aboutCard);
            
            // Add controls to the settings panel
            settingsPanel.Controls.Add(settingCardsPanel);
            settingsPanel.Controls.Add(settingsTitle);
            
            // Add settings panel to main panel
            mainPanel.Controls.Add(settingsPanel);
        }
        
        /// <summary>
        /// Creates a navigation button for the sidebar
        /// </summary>
        private Button CreateNavButton(string text, string icon, int position)
        {
            Button button = new Button
            {
                Text = "  " + text,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(sidePanel.Width, 45),
                Location = new Point(0, 100 + (position * 45)),
                BackColor = _secondaryColor,
                ForeColor = _textColor,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Cursor = Cursors.Hand,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding = new Padding(10, 0, 0, 0)
            };
            
            // Remove border
            button.FlatAppearance.BorderSize = 0;
            
            // Add icon if available
            if (!string.IsNullOrEmpty(icon))
            {
                button.Text = icon + " " + text;
            }
            
            // Add hover effect
            button.MouseEnter += (sender, e) => {
                if (button != activeButton)
                    button.BackColor = Color.FromArgb(30, 100, 167);
            };
            
            button.MouseLeave += (sender, e) => {
                if (button != activeButton)
                    button.BackColor = _secondaryColor;
            };
            
            // Add to navigation buttons list
            navigationButtons.Add(button);
            
            return button;
        }
        
        /// <summary>
        /// Creates an action card for the panels
        /// </summary>
        private Panel CreateActionCard(string title, string description, string icon, EventHandler clickHandler)
        {
            Panel card = new Panel
            {
                Width = 300,
                Height = 150,
                Margin = new Padding(15),
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };
            
            // Add shadow and rounded corners
            card.Paint += (sender, e) => {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Create rounded rectangle path
                GraphicsPath path = new GraphicsPath();
                int radius = 10;
                Rectangle rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                
                // Add arcs for rounded corners
                path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);                  // Top-left
                path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);  // Top-right
                path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);  // Bottom-right
                path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);  // Bottom-left
                path.CloseAllFigures();
                
                // Draw shadow
                using (GraphicsPath shadowPath = (GraphicsPath)path.Clone())
                {
                    using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                    {
                        g.TranslateTransform(3, 3);
                        g.FillPath(shadowBrush, shadowPath);
                        g.ResetTransform();
                    }
                }
                
                // Fill card background
                g.FillPath(new SolidBrush(Color.White), path);
                
                // Draw accent line on left side
                using (SolidBrush accentBrush = new SolidBrush(_primaryColor))
                {
                    g.FillRectangle(accentBrush, new Rectangle(0, 0, 5, card.Height));
                }
            };
            
            // Icon label
            Label iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24, FontStyle.Regular),
                Location = new Point(20, 20),
                Size = new Size(50, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            // Title label
            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold),
                ForeColor = _primaryColor,
                Location = new Point(80, 30),
                AutoSize = true
            };
            
            // Description label
            Label descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(20, 90),
                Size = new Size(260, 40),
                TextAlign = ContentAlignment.TopLeft
            };
            
            // Add controls to card
            card.Controls.Add(iconLabel);
            card.Controls.Add(titleLabel);
            card.Controls.Add(descLabel);
            
            // Add click handler
            card.Click += clickHandler;
            
            // Make sure child controls pass clicks to parent
            foreach (Control control in card.Controls)
            {
                control.Click += clickHandler;
            }
            
            return card;
        }
        
        /// <summary>
        /// Refreshes the dashboard with current data
        /// </summary>
        private void RefreshDashboard()
        {
            // Refresh statistics
            RefreshStatistics();
            
            // Refresh household list
            LoadHouseholdList();
            
            // Update status label
            UpdateStatusLabel();
        }
        
        /// <summary>
        /// Refreshes the statistics panel with current data
        /// </summary>
        private void RefreshStatistics()
        {
            // Clear existing statistics
            statisticsPanel.Controls.Clear();
            
            // Create a flow layout for statistics cards
            FlowLayoutPanel statsFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true
            };
            
            // Get statistics from controller
            int totalHouseholds = _controller.GetHouseholdCount();
            int totalAdults = _controller.GetTotalAdults();
            int totalChildren = _controller.GetTotalChildren();
            int totalPopulation = _controller.GetTotalPopulation();
            double avgHouseholdSize = totalHouseholds > 0 ? Math.Round((double)totalPopulation / totalHouseholds, 2) : 0;
            
            // Create statistics cards
            CreateStatCard(statsFlow, "Total Households", totalHouseholds.ToString(), _primaryColor);
            CreateStatCard(statsFlow, "Total Adults", totalAdults.ToString(), _secondaryColor);
            CreateStatCard(statsFlow, "Total Children", totalChildren.ToString(), _accentColor);
            CreateStatCard(statsFlow, "Total Population", totalPopulation.ToString(), _highlightColor);
            CreateStatCard(statsFlow, "Avg. Household Size", avgHouseholdSize.ToString(), Color.FromArgb(75, 192, 192));
            
            // Add flow panel to statistics panel
            statisticsPanel.Controls.Add(statsFlow);
        }
        
        /// <summary>
        /// Creates a statistic card for the statistics panel
        /// </summary>
        private void CreateStatCard(FlowLayoutPanel parent, string title, string value, Color color)
        {
            Panel statCard = new Panel
            {
                Width = 180,
                Height = parent.Height - 20,
                Margin = new Padding(10, 10, 10, 10),
                BackColor = Color.White
            };
            
            // Add shadow and rounded corners
            statCard.Paint += (sender, e) => {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Create rounded rectangle path
                GraphicsPath path = new GraphicsPath();
                int radius = 10;
                Rectangle rect = new Rectangle(0, 0, statCard.Width - 1, statCard.Height - 1);
                
                // Add arcs for rounded corners
                path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);                  // Top-left
                path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);  // Top-right
                path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);  // Bottom-right
                path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);  // Bottom-left
                path.CloseAllFigures();
                
                // Draw shadow
                using (GraphicsPath shadowPath = (GraphicsPath)path.Clone())
                {
                    using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                    {
                        g.TranslateTransform(2, 2);
                        g.FillPath(shadowBrush, shadowPath);
                        g.ResetTransform();
                    }
                }
                
                // Fill card background
                g.FillPath(new SolidBrush(Color.White), path);
                
                // Draw top colored accent
                using (SolidBrush accentBrush = new SolidBrush(color))
                {
                    g.FillRectangle(accentBrush, new Rectangle(0, 0, statCard.Width, 5));
                }
            };
            
            // Value label
            Label valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = color,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(statCard.Width, 50),
                Location = new Point(0, 20)
            };
            
            // Title label
            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(statCard.Width, 20),
                Location = new Point(0, 70)
            };
            
            // Add controls to stat card
            statCard.Controls.Add(valueLabel);
            statCard.Controls.Add(titleLabel);
            
            // Add stat card to parent
            parent.Controls.Add(statCard);
        }
        
        /// <summary>
        /// Loads the household list with current data
        /// </summary>
        private void LoadHouseholdList()
        {
            // Clear existing items
            householdListView.Items.Clear();
            
            // Get households from controller
            var households = _controller.GetHouseholds();
            
            // Add each household to the list
            foreach (var household in households)
            {
                int adultCount = household.AdultCount;
                int childCount = household.ChildCount;
                int totalMembers = adultCount + childCount;
                
                ListViewItem item = new ListViewItem(household.HouseNumber.ToString());
                item.SubItems.Add(household.Address ?? "No Address");
                item.SubItems.Add(adultCount.ToString());
                item.SubItems.Add(childCount.ToString());
                item.SubItems.Add(totalMembers.ToString());
                
                // Alternate row colors for better readability
                item.BackColor = householdListView.Items.Count % 2 == 0 
                    ? Color.White 
                    : Color.FromArgb(245, 245, 245);
                
                householdListView.Items.Add(item);
            }
        }
    }
}
