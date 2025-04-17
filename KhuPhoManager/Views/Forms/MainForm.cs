using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;
using FontAwesome.Sharp;

namespace KhuPhoManager.Views.Forms
{
    public partial class MainForm : Form
    {
        private GuiNeighborhoodController _controller;
        
        // Dashboard panels and controls
        private Panel dashboardPanel;
        private Panel sidebarPanel;
        private Panel mainContentPanel;
        private Panel headerPanel;
        private Panel statisticsPanel;
        private Panel householdListPanel;
        private Panel actionPanel;
        private ListView householdListView;
        private System.Windows.Forms.Timer refreshTimer;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        
        // Modern UI elements
        private FlowLayoutPanel menuPanel;
        private Label applicationTitle;
        private IconButton dashboardButton;
        private IconButton householdsButton;
        private IconButton peopleButton;
        private IconButton reportsButton;
        private IconButton settingsButton;
        private IconButton activeButton;
        private Panel navIndicator;
        private Panel activePanel;
        
        // Theme colors
        private readonly Color primaryColor = Color.FromArgb(24, 90, 157);       // Dark blue
        private readonly Color secondaryColor = Color.FromArgb(17, 53, 71);      // Even darker blue
        private readonly Color highlightColor = Color.FromArgb(86, 204, 242);    // Light blue
        private readonly Color textColor = Color.FromArgb(238, 238, 238);        // Off-white
        private readonly Color panelColor = Color.FromArgb(247, 250, 252);       // Very light blue-gray
        private readonly Color accentColor = Color.FromArgb(255, 74, 130);       // Pink accent

        public MainForm(GuiNeighborhoodController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // MainForm - Set up the main form
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Name = "MainForm";
            this.Text = "Neighborhood Manager";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.MinimumSize = new Size(1280, 800);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            
            // Create the main layout structure
            // 1. Sidebar panel for navigation
            sidebarPanel = new Panel
            {
                Width = 250,
                Dock = DockStyle.Left,
                BackColor = secondaryColor,
                Padding = new Padding(0, 20, 0, 0)
            };
            
            // 2. Main content area
            mainContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = panelColor,
                Padding = new Padding(20)
            };
            
            // App title in sidebar
            applicationTitle = new Label
            {
                Text = "Neighborhood Manager",
                ForeColor = textColor,
                Font = new Font("Segoe UI Semibold", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(0, 20, 0, 20)
            };
            
            // Navigation indicator panel
            navIndicator = new Panel
            {
                Width = 4,
                Height = 45,
                BackColor = highlightColor,
                Location = new Point(0, 0),
                Visible = false
            };
            
            // Create menu panel for sidebar navigation
            menuPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10, 30, 10, 0),
                AutoScroll = true
            };
            
            // Create navigation buttons
            dashboardButton = CreateMenuButton("Dashboard", IconChar.ChartPie);
            householdsButton = CreateMenuButton("Households", IconChar.HouseChimney);
            peopleButton = CreateMenuButton("People", IconChar.UserGroup);
            reportsButton = CreateMenuButton("Reports", IconChar.FileAlt);
            settingsButton = CreateMenuButton("Settings", IconChar.Gear);
            
            // Add click handlers to menu buttons
            dashboardButton.Click += (s, e) => ActivateButton(s as IconButton, "Dashboard");
            householdsButton.Click += (s, e) => ActivateButton(s as IconButton, "Households");
            peopleButton.Click += (s, e) => ActivateButton(s as IconButton, "People");
            reportsButton.Click += (s, e) => ActivateButton(s as IconButton, "Reports");
            settingsButton.Click += (s, e) => ActivateButton(s as IconButton, "Settings");
            
            // Add buttons to the menu panel
            menuPanel.Controls.Add(dashboardButton);
            menuPanel.Controls.Add(householdsButton);
            menuPanel.Controls.Add(peopleButton);
            menuPanel.Controls.Add(reportsButton);
            menuPanel.Controls.Add(settingsButton);
            
            // Add components to the sidebar
            sidebarPanel.Controls.Add(menuPanel);
            sidebarPanel.Controls.Add(applicationTitle);
            sidebarPanel.Controls.Add(navIndicator);
            
            // Create the header panel for the main content area
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 0, 0, 0)
            };
            
            // Add a shadow to the header
            headerPanel.Paint += (sender, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, headerPanel.Height - 2, headerPanel.Width, 2);
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    new Point(0, rect.Y), new Point(0, rect.Bottom),
                    Color.FromArgb(40, 0, 0, 0), Color.Transparent))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            };
            
            // Create a title for the current section in the header
            Label headerTitle = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold),
                ForeColor = primaryColor,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            
            // Add a refresh button to the header
            IconButton refreshButton = new IconButton
            {
                IconChar = IconChar.SyncAlt,
                IconColor = primaryColor,
                IconSize = 18,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 40),
                Location = new Point(headerPanel.Width - 60, 10),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Text = ""
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += RefreshDashboard;
            
            headerPanel.Controls.Add(headerTitle);
            headerPanel.Controls.Add(refreshButton);
            
            // Initialize main content panels for each section
            InitializeDashboardPanel();
            InitializeHouseholdsPanel();
            
            // Add the main structural panels to the form
            mainContentPanel.Controls.Add(dashboardPanel);
            mainContentPanel.Controls.Add(householdListPanel);
            mainContentPanel.Controls.Add(headerPanel);
            
            this.Controls.Add(mainContentPanel);
            this.Controls.Add(sidebarPanel);
            
            // Create status strip
            statusStrip = new StatusStrip();
            statusStrip.BackColor = secondaryColor;
            statusStrip.ForeColor = textColor;
            statusLabel = new ToolStripStatusLabel("Ready");
            statusLabel.ForeColor = textColor;
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);
            
            // Activate Dashboard by default
            ActivateButton(dashboardButton, "Dashboard");
            
            // Create main dashboard panel
            dashboardPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(240, 240, 250)
            };
            
            // Create top statistics panel
            statisticsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(10),
                BorderStyle = BorderStyle.None
            };
            
            // Add round corners and shadow effect to statistics panel
            statisticsPanel.Paint += (sender, e) => {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.Clear(statisticsPanel.Parent.BackColor);
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 10;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(statisticsPanel.Width - radius - 1, 0, radius, radius, 270, 90);
                    path.AddArc(statisticsPanel.Width - radius - 1, statisticsPanel.Height - radius - 1, radius, radius, 0, 90);
                    path.AddArc(0, statisticsPanel.Height - radius - 1, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    e.Graphics.FillPath(new SolidBrush(Color.White), path);
                    // Add a slight drop shadow effect
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(40, 0, 0, 0), 2), path);
                }
            };
            
            // Create household list panel (occupies the left side of the main area)
            householdListPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 500,
                BackColor = Color.White,
                Margin = new Padding(0, 10, 10, 0),
                Padding = new Padding(10),
                BorderStyle = BorderStyle.None
            };
            
            // Add round corners and shadow effect to household list panel
            householdListPanel.Paint += (sender, e) => {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.Clear(householdListPanel.Parent.BackColor);
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 10;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(householdListPanel.Width - radius - 1, 0, radius, radius, 270, 90);
                    path.AddArc(householdListPanel.Width - radius - 1, householdListPanel.Height - radius - 1, radius, radius, 0, 90);
                    path.AddArc(0, householdListPanel.Height - radius - 1, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    e.Graphics.FillPath(new SolidBrush(Color.White), path);
                    // Add a slight drop shadow effect
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(40, 0, 0, 0), 2), path);
                }
            };
            
            // Create action panel (right side of the main area)
            actionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(10, 10, 0, 0),
                Padding = new Padding(10),
                BorderStyle = BorderStyle.None
            };
            
            // Add round corners and shadow effect to action panel
            actionPanel.Paint += (sender, e) => {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.Clear(actionPanel.Parent.BackColor);
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 10;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(actionPanel.Width - radius - 1, 0, radius, radius, 270, 90);
                    path.AddArc(actionPanel.Width - radius - 1, actionPanel.Height - radius - 1, radius, radius, 0, 90);
                    path.AddArc(0, actionPanel.Height - radius - 1, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    e.Graphics.FillPath(new SolidBrush(Color.White), path);
                    // Add a slight drop shadow effect
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(40, 0, 0, 0), 2), path);
                }
            };
            
            // Create household list view
            householdListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White
            };
            
            // Configure household list view columns
            householdListView.Columns.Add("House #", 80);
            householdListView.Columns.Add("Members", 80);
            householdListView.Columns.Add("Adults", 80);
            householdListView.Columns.Add("Children", 80);
            
            // Add household list view to household list panel
            Label householdsHeader = new Label
            {
                Text = "Households",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(40, 40, 100)
            };
            
            Panel householdActionsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };
            
            Button addHouseholdButton = new Button
            {
                Text = "Add Household",
                BackColor = Color.FromArgb(30, 30, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 150,
                Height = 40,
                Location = new Point(0, 5)
            };
            addHouseholdButton.FlatAppearance.BorderSize = 0;
            addHouseholdButton.Click += AddHousehold_Click;
            
            Button viewDetailsButton = new Button
            {
                Text = "View Details",
                BackColor = Color.FromArgb(60, 60, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 150,
                Height = 40,
                Location = new Point(160, 5)
            };
            viewDetailsButton.FlatAppearance.BorderSize = 0;
            viewDetailsButton.Click += ViewHouseholdDetails_Click;
            
            householdActionsPanel.Controls.Add(addHouseholdButton);
            householdActionsPanel.Controls.Add(viewDetailsButton);
            
            // Configure action panel
            Label actionHeader = new Label
            {
                Text = "Quick Actions",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(40, 40, 100)
            };
            
            FlowLayoutPanel actionButtonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 10, 0, 0)
            };
            
            // Create action buttons
            string[] actionLabels = new[] {
                "Add Person", "Find Person", 
                "View Statistics", "View Household Rankings", 
                "Sort by Age", "Save Data", "Load Data"
            };
            
            EventHandler[] actionHandlers = new EventHandler[] {
                AddPerson_Click, FindPerson_Click,
                Statistics_Click, MostFewestMembers_Click,
                SortByAge_Click, SaveFile_Click, OpenFile_Click
            };
            
            for (int i = 0; i < actionLabels.Length; i++)
            {
                Button actionButton = new Button
                {
                    Text = actionLabels[i],
                    BackColor = Color.FromArgb(230, 230, 250),
                    ForeColor = Color.FromArgb(30, 30, 65),
                    FlatStyle = FlatStyle.Flat,
                    Width = 300,
                    Height = 50,
                    Margin = new Padding(0, 0, 0, 10),
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    TextAlign = ContentAlignment.MiddleLeft,
                    ImageAlign = ContentAlignment.MiddleRight
                };
                actionButton.FlatAppearance.BorderSize = 0;
                actionButton.Click += actionHandlers[i];
                actionButtonsPanel.Controls.Add(actionButton);
            }
            
            // Assemble the panels
            householdListPanel.Controls.Add(householdListView);
            householdListPanel.Controls.Add(householdActionsPanel);
            householdListPanel.Controls.Add(householdsHeader);
            
            actionPanel.Controls.Add(actionButtonsPanel);
            actionPanel.Controls.Add(actionHeader);
            
            dashboardPanel.Controls.Add(actionPanel);
            dashboardPanel.Controls.Add(householdListPanel);
            dashboardPanel.Controls.Add(statisticsPanel);
            
            this.Controls.Add(dashboardPanel);
            
            // Create refresh timer
            refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 30000 // Refresh every 30 seconds
            };
            refreshTimer.Tick += (s, e) => RefreshDashboard(s, e);
            refreshTimer.Start();
            
            this.ResumeLayout(false);
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            statusLabel.Text = "Loading neighborhood data...";
            
            try
            {
                // Initialize all panels if needed
                if (dashboardPanel == null) InitializeDashboardPanel();
                if (householdListPanel == null) InitializeHouseholdsPanel();
                if (actionPanel == null) InitializePeoplePanel();
                
                // Make sure all panels are added to the main content area
                if (!mainContentPanel.Controls.Contains(dashboardPanel))
                    mainContentPanel.Controls.Add(dashboardPanel);
                if (!mainContentPanel.Controls.Contains(householdListPanel))
                    mainContentPanel.Controls.Add(householdListPanel);
                if (!mainContentPanel.Controls.Contains(actionPanel))
                    mainContentPanel.Controls.Add(actionPanel);
                    
                // Create refresh timer if not already created
                if (refreshTimer == null)
                {
                    refreshTimer = new System.Windows.Forms.Timer();
                    refreshTimer.Interval = 60000; // Refresh every minute
                    refreshTimer.Tick += RefreshDashboard;
                    refreshTimer.Start();
                }
                
                // Set the dashboard as the active panel by default
                ActivateButton(dashboardButton, "Dashboard");
                
                // Update status
                statusLabel.Text = "Ready";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = $"Error: {ex.Message}";
            }
        }
        
        private void OpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                openFileDialog.Title = "Open Neighborhood Data File";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Show the read file form
                        ReadFileForm readFileForm = new ReadFileForm(_controller, openFileDialog.FileName);
                        readFileForm.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void SaveFile_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                saveFileDialog.Title = "Save Neighborhood Data File";
                saveFileDialog.DefaultExt = "csv";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Show the save file form
                        SaveFileForm saveFileForm = new SaveFileForm(_controller, saveFileDialog.FileName);
                        saveFileForm.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void ListHouseholds_Click(object sender, EventArgs e)
        {
            HouseholdListForm householdListForm = new HouseholdListForm(_controller);
            householdListForm.ShowDialog();
        }
        
        private void AddHousehold_Click(object sender, EventArgs e)
        {
            AddHouseholdForm addHouseholdForm = new AddHouseholdForm(_controller);
            addHouseholdForm.ShowDialog();
        }

        private void RemoveHousehold_Click(object sender, EventArgs e)
        {
            RemoveHouseholdForm removeHouseholdForm = new RemoveHouseholdForm(_controller);
            removeHouseholdForm.ShowDialog();
        }
        
        private void FindPerson_Click(object sender, EventArgs e)
        {
            FindPersonForm findPersonForm = new FindPersonForm(_controller);
            findPersonForm.ShowDialog();
        }
        
        private void AddPerson_Click(object sender, EventArgs e)
        {
            // Get list of households
            var households = _controller.GetHouseholds();
            if (households.Count == 0)
            {
                MessageBox.Show("There are no households to add a person to. Please create a household first.", 
                               "No Households", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Create a form to select a household
            using (var selectForm = new Form())
            {
                selectForm.Text = "Select Household";
                selectForm.Size = new Size(400, 200);
                selectForm.StartPosition = FormStartPosition.CenterParent;
                selectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                selectForm.MaximizeBox = false;
                selectForm.MinimizeBox = false;

                var label = new Label
                {
                    Text = "Select a household to add a person to:",
                    Location = new Point(20, 20),
                    AutoSize = true
                };
                
                var combo = new ComboBox
                {
                    Location = new Point(20, 50),
                    Width = 350,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                // Add households to the dropdown
                foreach (var household in households)
                {
                    combo.Items.Add(new HouseholdItem(household.HouseNumber, 
                        $"Household #{household.HouseNumber} ({household.Members.Count} members)"));
                };
                combo.SelectedIndex = 0;

                var okButton = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(100, 100),
                    Width = 100
                };

                var cancelButton = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(210, 100),
                    Width = 100
                };

                selectForm.Controls.Add(label);
                selectForm.Controls.Add(combo);
                selectForm.Controls.Add(okButton);
                selectForm.Controls.Add(cancelButton);
                selectForm.AcceptButton = okButton;
                selectForm.CancelButton = cancelButton;

                if (selectForm.ShowDialog() == DialogResult.OK)
                {
                    var selectedItem = combo.SelectedItem as HouseholdItem;
                    if (selectedItem != null)
                    {
                        // Open the add person form with the selected household number
                        AddPersonToHouseholdForm addPersonForm = new AddPersonToHouseholdForm(_controller, selectedItem.HouseNumber);
                        addPersonForm.ShowDialog();
                    }
                }
            }
        }

        /// <summary>
        /// Helper class to store household information in combobox items
        /// </summary>
        internal class HouseholdItem
        {
            public int HouseNumber { get; }
            private string DisplayText { get; }
            
            public HouseholdItem(int houseNumber, string displayText)
            {
                HouseNumber = houseNumber;
                DisplayText = displayText;
            }
            
            public override string ToString()
            {
                return DisplayText;
            }
        }
        
        /// <summary>
        /// Creates a navigation menu button for the sidebar
        /// </summary>
        private IconButton CreateMenuButton(string text, IconChar icon)
        {
            var button = new IconButton
            {
                Text = text,
                IconChar = icon,
                FlatStyle = FlatStyle.Flat,
                ForeColor = textColor,
                IconColor = textColor,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                IconSize = 24,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding = new Padding(10, 0, 0, 0),
                Margin = new Padding(5, 5, 5, 5),
                Size = new Size(230, 45),
                Cursor = Cursors.Hand
            };
            
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = primaryColor;
            button.FlatAppearance.MouseDownBackColor = primaryColor;
            
            return button;
        }
        
        /// <summary>
        /// Activates a sidebar button and shows its corresponding panel
        /// </summary>
        private void ActivateButton(IconButton button, string panelName)
        {
            if (button == null) return;
            
            // Deactivate current button if exists
            if (activeButton != null)
            {
                activeButton.BackColor = secondaryColor;
                activeButton.ForeColor = textColor;
                activeButton.IconColor = textColor;
                activeButton.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            }
            
            // Activate new button
            button.BackColor = primaryColor;
            button.ForeColor = highlightColor;
            button.IconColor = highlightColor;
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            activeButton = button;
            
            // Show active indicator
            navIndicator.Height = button.Height;
            navIndicator.Top = button.Top;
            navIndicator.Left = 0;
            navIndicator.Visible = true;
            
            // Show the corresponding panel
            ShowPanel(panelName);
        }
        
        /// <summary>
        /// Shows the panel corresponding to the active button
        /// </summary>
        private void ShowPanel(string panelName)
        {
            // Hide all panels first
            if (dashboardPanel != null) dashboardPanel.Visible = false;
            if (householdListPanel != null) householdListPanel.Visible = false;
            if (actionPanel != null) actionPanel.Visible = false;
            
            // Show the selected panel
            switch (panelName)
            {
                case "Dashboard":
                    if (dashboardPanel == null)
                    {
                        InitializeDashboardPanel();
                        mainContentPanel.Controls.Add(dashboardPanel);
                    }
                    dashboardPanel.Visible = true;
                    activePanel = dashboardPanel;
                    RefreshDashboard(null, null);
                    break;
                    
                case "Households":
                    if (householdListPanel == null)
                    {
                        InitializeHouseholdsPanel();
                        mainContentPanel.Controls.Add(householdListPanel);
                    }
                    householdListPanel.Visible = true;
                    activePanel = householdListPanel;
                    RefreshHouseholdList();
                    break;
                    
                case "People":
                    if (actionPanel == null)
                    {
                        InitializePeoplePanel();
                        mainContentPanel.Controls.Add(actionPanel);
                    }
                    actionPanel.Visible = true;
                    activePanel = actionPanel;
                    break;
                
                // Add more cases for other panels
            }
        }
        
        private void SortByAge_Click(object sender, EventArgs e)
        {
            SortByAgeForm sortByAgeForm = new SortByAgeForm(_controller);
            sortByAgeForm.ShowDialog();
        }
        
        private void Statistics_Click(object sender, EventArgs e)
        {
            NeighborhoodStatsForm statsForm = new NeighborhoodStatsForm(_controller);
            statsForm.ShowDialog();
        }
        
        private void MostFewestMembers_Click(object sender, EventArgs e)
        {
            MostFewestMembersForm mostFewestMembersForm = new MostFewestMembersForm(_controller);
            mostFewestMembersForm.ShowDialog();
        }
        
        private void About_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Neighborhood Manager\nVersion 1.0\n\nA Windows Forms application for managing neighborhoods, households, and residents.",
                "About Neighborhood Manager",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        
        /// <summary>
        /// Refreshes all dashboard elements with current data
        /// </summary>
        private void RefreshDashboard(object sender, EventArgs e)
        {
            try
            {
                // Update status
                statusLabel.Text = $"Last updated: {DateTime.Now.ToString("g")}";
                
                // Refresh household list
                RefreshHouseholdList();
                
                // Refresh statistics panel
                RefreshStatisticsPanel();
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error refreshing dashboard: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Refreshes the household list view with current data
        /// </summary>
        private void RefreshHouseholdList()
        {
            householdListView.Items.Clear();
            
            var households = _controller.GetHouseholds();
            foreach (var household in households)
            {
                var item = new ListViewItem(household.HouseNumber.ToString());
                item.SubItems.Add(household.Members.Count.ToString());
                item.SubItems.Add(household.AdultCount.ToString());
                item.SubItems.Add(household.ChildCount.ToString());
                item.Tag = household.HouseNumber;
                householdListView.Items.Add(item);
            }
            
            // Resize columns to fit content
            foreach (ColumnHeader column in householdListView.Columns)
            {
                column.Width = -2; // Auto-size to column header and content
            }
        }
        
        /// <summary>
        /// Refreshes the statistics panel with current data
        /// </summary>
        private void RefreshStatisticsPanel()
        {
            // Clear existing controls except for design elements
            foreach (Control control in statisticsPanel.Controls)
            {
                if (!(control is Label headerLabel) || headerLabel.Name != "statsHeader")
                {
                    control.Dispose();
                }
            }
            statisticsPanel.Controls.Clear();
            
            // Add statistics header
            Label header = new Label
            {
                Text = "Neighborhood Statistics",
                Name = "statsHeader",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 100),
                Location = new Point(10, 10),
                AutoSize = true
            };
            statisticsPanel.Controls.Add(header);
            
            // Create stat boxes for key metrics
            CreateStatBox("Total Households", _controller.GetHouseholdCount().ToString(), new Point(20, 50), Color.FromArgb(30, 30, 65));
            CreateStatBox("Total Population", _controller.GetTotalPopulation().ToString(), new Point(200, 50), Color.FromArgb(0, 120, 215));
            CreateStatBox("Adults", _controller.GetTotalAdults().ToString(), new Point(380, 50), Color.FromArgb(0, 130, 130));
            CreateStatBox("Children", _controller.GetTotalChildren().ToString(), new Point(560, 50), Color.FromArgb(200, 30, 90));
            
            // Add some additional info if available
            if (_controller.GetHouseholdCount() > 0)
            {
                var avgMembers = (double)_controller.GetTotalPopulation() / _controller.GetHouseholdCount();
                CreateStatBox("Avg Members/Household", avgMembers.ToString("F1"), new Point(740, 50), Color.FromArgb(140, 100, 190));
                
                // Could add more derived statistics here
            }
        }
        
        /// <summary>
        /// Creates a beautiful statistics box on the statistics panel
        /// </summary>
        private void CreateStatBox(string title, string value, Point location, Color color)
        {
            Panel statBox = new Panel
            {
                Width = 200,
                Height = 100,
                Location = location,
                BackColor = Color.White
            };
            
            // Add shadow and rounded corners to the stat box
            statBox.Paint += (sender, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Create rounded rectangle path
                using (var path = new GraphicsPath())
                {
                    int radius = 10;
                    Rectangle rect = new Rectangle(0, 0, statBox.Width - 1, statBox.Height - 1);
                    path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                    path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
                    path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                    path.CloseAllFigures();
                    
                    // Create gradient background
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        new Point(0, 0), new Point(statBox.Width, statBox.Height),
                        Color.White, Color.FromArgb(10, color)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    
                    // Draw a thin border
                    using (Pen pen = new Pen(Color.FromArgb(30, color), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                    
                    // Add a colored indicator on top
                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        e.Graphics.FillRectangle(brush, new Rectangle(0, 0, statBox.Width, 4));
                    }
                }
            };
            
            // Icon
            IconPictureBox iconBox = new IconPictureBox
            {
                IconChar = GetIconForStat(title),
                IconSize = 32,
                IconColor = color,
                BackColor = Color.Transparent,
                Size = new Size(40, 40),
                Location = new Point(15, 15)
            };
            
            // Value label
            Label valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI Semibold", 20, FontStyle.Bold),
                ForeColor = color,
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(60, 15),
                Size = new Size(125, 40),
                AutoSize = false
            };
            
            // Title label
            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(15, 65),
                Size = new Size(170, 20),
                AutoSize = false
            };
            
            statBox.Controls.Add(iconBox);
            statBox.Controls.Add(valueLabel);
            statBox.Controls.Add(titleLabel);
            statisticsPanel.Controls.Add(statBox);
        }
        
        /// <summary>
        /// Returns an appropriate icon for the stat box based on its title
        /// </summary>
        private IconChar GetIconForStat(string title)
        {
            if (title.Contains("Household")) return IconChar.Home;
            if (title.Contains("Population")) return IconChar.Users;
            if (title.Contains("Adult")) return IconChar.UserTie;
            if (title.Contains("Children")) return IconChar.Child;
            if (title.Contains("Avg")) return IconChar.ChartLine;
            
            return IconChar.InfoCircle;
        }
        
        /// <summary>
        /// Handles the View Details button click for households
        /// </summary>
        private void ViewHouseholdDetails_Click(object sender, EventArgs e)
        {
            if (householdListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a household to view details.", "Selection Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            int houseNumber = (int)householdListView.SelectedItems[0].Tag;
            var household = _controller.GetHouseholds().FirstOrDefault(h => h.HouseNumber == houseNumber);
            
            if (household != null)
            {
                HouseholdDetailsForm detailsForm = new HouseholdDetailsForm(_controller, household);
                detailsForm.ShowDialog();
                
                // Refresh the dashboard after viewing details in case changes were made
                RefreshDashboard(sender, e);
            }
        }
        
        /// <summary>
        /// Initialize the People panel with filtering and search options
        /// </summary>
        private void InitializePeoplePanel()
        {
            // Create people panel if it doesn't exist or recreate it
            if (actionPanel == null)
            {
                actionPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Visible = false,
                    Padding = new Padding(15, 15, 15, 15),
                    BackColor = panelColor
                };
                
                // Add components and functionality as needed similar to the other panels
                Panel contentPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White
                };
                
                // Add rounded corners to content panel
                contentPanel.Paint += (sender, e) => {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var path = new GraphicsPath())
                    {
                        int radius = 10;
                        Rectangle rect = new Rectangle(0, 0, contentPanel.Width - 1, contentPanel.Height - 1);
                        path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                        path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
                        path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                        path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                        path.CloseAllFigures();
                        
                        e.Graphics.FillPath(new SolidBrush(Color.White), path);
                        
                        // Add a subtle shadow
                        using (Pen pen = new Pen(Color.FromArgb(20, 0, 0, 0), 1))
                        {
                            e.Graphics.DrawPath(pen, path);
                        }
                    }
                };
                
                Label peopleTitle = new Label
                {
                    Text = "People Management",
                    Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold),
                    ForeColor = primaryColor,
                    Dock = DockStyle.Top,
                    Height = 40,
                    Padding = new Padding(15, 15, 0, 0)
                };
                
                // Create a FlowLayoutPanel for the people management functions
                FlowLayoutPanel peopleActionsPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    Padding = new Padding(20, 20, 20, 20),
                    AutoScroll = true
                };
                
                // Create action cards for people management
                AddActionCard(peopleActionsPanel, "Find Person", "Search for a person by ID or name", IconChar.Search, FindPerson_Click);
                AddActionCard(peopleActionsPanel, "Add Person", "Add a new person to a household", IconChar.UserPlus, AddPerson_Click);
                AddActionCard(peopleActionsPanel, "Sort by Age", "View all people sorted by age", IconChar.SortAmountDown, SortByAge_Click);
                
                contentPanel.Controls.Add(peopleActionsPanel);
                contentPanel.Controls.Add(peopleTitle);
                
                actionPanel.Controls.Add(contentPanel);
            }
        }
        
        /// <summary>
        /// Helper method to create action cards for the people panel
        /// </summary>
        private void AddActionCard(FlowLayoutPanel parent, string title, string description, IconChar icon, EventHandler clickHandler)
        {
            Panel card = new Panel
            {
                Width = parent.Width - 50,
                Height = 100,
                Margin = new Padding(0, 0, 0, 15),
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };
            
            // Add shadow and hover effect
            card.Paint += (sender, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = new GraphicsPath())
                {
                    int radius = 10;
                    Rectangle rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                    path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                    path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
                    path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                    path.CloseAllFigures();
                    
                    // Draw shadow
                    using (var shadowPath = (GraphicsPath)path.Clone())
                    {
                        using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                        {
                            e.Graphics.TranslateTransform(2, 2);
                            e.Graphics.FillPath(shadowBrush, shadowPath);
                            e.Graphics.ResetTransform();
                        }
                    }
                    
                    e.Graphics.FillPath(new SolidBrush(Color.White), path);
                    
                    // Add a color indicator on the left side
                    using (var leftBar = new SolidBrush(primaryColor))
                    {
                        e.Graphics.FillRectangle(leftBar, new Rectangle(0, 0, 5, card.Height));
                    }
                }
            };
            
            // Add icon
            IconPictureBox iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconSize = 40,
                IconColor = primaryColor,
                BackColor = Color.Transparent,
                Size = new Size(50, 50),
                Location = new Point(20, 25)
            };
            
            // Add title
            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold),
                ForeColor = primaryColor,
                Location = new Point(80, 20),
                AutoSize = true
            };
            
            // Add description
            Label descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(80, 50),
                Size = new Size(300, 30),
                AutoSize = false
            };
            
            // Add controls to card
            card.Controls.Add(iconBox);
            card.Controls.Add(titleLabel);
            card.Controls.Add(descLabel);
            
            // Add click handler
            card.Click += clickHandler;
            // Make sure all child controls pass clicks to parent
            foreach (Control control in card.Controls)
            {
                control.Click += clickHandler;
            }
            
            parent.Controls.Add(card);
        }
    }
}
