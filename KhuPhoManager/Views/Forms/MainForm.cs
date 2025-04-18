using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Views.Helpers;
using KhuPhoManager.Views.UserControls;

namespace KhuPhoManager.Views.Forms
{
    /// <summary>
    /// Main form for the KhuPho Manager application with a modern, professional UI
    /// </summary>
    public partial class MainForm : Form
    {
        // Controller
        private readonly GuiNeighborhoodController _controller;

        // UI Elements
        private Panel sidePanel;
        private Panel headerPanel;
        private Panel mainPanel;
        private Panel navIndicator;
        
        // User controls for panels
        private DashboardPanel dashboardPanel;
        private HouseholdsPanel householdsPanel;
        private PeoplePanel peoplePanel;
        private ReportsPanel reportsPanel;
        private SettingsPanel settingsPanel;
        private UserControl activePanel;

        // Navigation buttons
        private List<Button> navigationButtons;
        private Button activeButton;
        private Button dashboardButton;
        private Button householdsButton;
        private Button peopleButton;
        private Button reportsButton;
        private Button settingsButton;

        // Status label
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
                dashboardPanel.RefreshDashboard();
            };

            householdsButton.Click += (sender, e) => {
                SetActiveButton(householdsButton);
                ShowPanel(householdsPanel);
                householdsPanel.LoadHouseholdList();
            };

            peopleButton.Click += (sender, e) => {
                SetActiveButton(peopleButton);
                ShowPanel(peoplePanel);
                peoplePanel.LoadPeopleList();
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
            refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 30000, // 30 seconds
                Enabled = true
            };
            
            refreshTimer.Tick += (sender, e) => {
                if (activePanel == dashboardPanel)
                {
                    dashboardPanel.RefreshDashboard();
                }
                else if (activePanel == householdsPanel)
                {
                    householdsPanel.LoadHouseholdList();
                }
                
                UpdateStatusLabel();
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
                activeButton.BackColor = UIHelper.SecondaryColor;
                activeButton.ForeColor = UIHelper.TextColor;
            }

            // Set new active button
            activeButton = button;
            activeButton.BackColor = UIHelper.PrimaryColor;
            activeButton.ForeColor = UIHelper.HighlightColor;

            // Position navigation indicator
            navIndicator.Height = button.Height;
            navIndicator.Top = button.Top;
            navIndicator.Left = 0;
            navIndicator.Visible = true;
        }

        /// <summary>
        /// Shows the specified panel and hides all others
        /// </summary>
        private void ShowPanel(UserControl panel)
        {
            // Hide all panels
            dashboardPanel.Visible = false;
            householdsPanel.Visible = false;
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

            if (activePanel == dashboardPanel)
                panelName = "Dashboard";
            else if (activePanel == householdsPanel)
                panelName = "Households Management";
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
            this.BackColor = UIHelper.PanelColor;

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

            // Create user controls for panels
            dashboardPanel = new DashboardPanel(_controller);
            householdsPanel = new HouseholdsPanel(_controller);
            peoplePanel = new PeoplePanel(_controller);
            reportsPanel = new ReportsPanel(_controller);
            settingsPanel = new SettingsPanel(_controller);

            // Add user controls to main panel
            mainPanel.Controls.Add(dashboardPanel);
            mainPanel.Controls.Add(householdsPanel);
            mainPanel.Controls.Add(peoplePanel);
            mainPanel.Controls.Add(reportsPanel);
            mainPanel.Controls.Add(settingsPanel);

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
                BackColor = UIHelper.PrimaryColor
            };

            // Add app title to header
            Label titleLabel = new Label
            {
                Text = "Neighborhood Manager",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UIHelper.TextColor,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            // Status label in header
            statusLabel = new Label
            {
                Text = "Ready",
                Font = new Font("Segoe UI", 9),
                ForeColor = UIHelper.TextColor,
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
                BackColor = UIHelper.SecondaryColor
            };

            // Main content panel
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIHelper.PanelColor,
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
                BackColor = UIHelper.SecondaryColor
            };

            // App logo label
            Label logoLabel = new Label
            {
                Text = "KhuPho",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = UIHelper.HighlightColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            logoPanel.Controls.Add(logoLabel);
            sidePanel.Controls.Add(logoPanel);

            // Navigation indicator panel
            navIndicator = new Panel
            {
                BackColor = UIHelper.HighlightColor,
                Size = new Size(7, 45),
                Location = new Point(0, 0),
                Visible = false
            };

            sidePanel.Controls.Add(navIndicator);

            // Navigation buttons
            navigationButtons = new List<Button>();

            dashboardButton = UIHelper.CreateNavButton("Dashboard", "🏠", 0, sidePanel);
            householdsButton = UIHelper.CreateNavButton("Households", "🏘️", 1, sidePanel);
            peopleButton = UIHelper.CreateNavButton("People", "👪", 2, sidePanel);
            reportsButton = UIHelper.CreateNavButton("Reports", "📊", 3, sidePanel);
            settingsButton = UIHelper.CreateNavButton("Settings", "⚙️", 4, sidePanel);

            // Add buttons to navigation list
            navigationButtons.Add(dashboardButton);
            navigationButtons.Add(householdsButton);
            navigationButtons.Add(peopleButton);
            navigationButtons.Add(reportsButton);
            navigationButtons.Add(settingsButton);
            
            // Add buttons to side panel
            sidePanel.Controls.Add(dashboardButton);
            sidePanel.Controls.Add(householdsButton);
            sidePanel.Controls.Add(peopleButton);
            sidePanel.Controls.Add(reportsButton);
            sidePanel.Controls.Add(settingsButton);
        }

        /// <summary>
        /// Refreshes the dashboard with current data
        /// </summary>
        private void RefreshDashboard()
        {
            if (dashboardPanel != null)
            {
                dashboardPanel.RefreshDashboard();
            }
            
            // Update status label
            UpdateStatusLabel();
        }
    }
}