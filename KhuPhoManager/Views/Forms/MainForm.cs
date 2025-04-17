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
            
            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Name = "MainForm";
            this.Text = "Neighborhood Manager - Dashboard";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.MinimumSize = new Size(1200, 800);
            
            // Create menu
            MenuStrip mainMenu = new MenuStrip();
            mainMenu.BackColor = Color.FromArgb(30, 30, 65);
            mainMenu.ForeColor = Color.White;
            this.MainMenuStrip = mainMenu;
            
            // File menu
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            fileMenu.ForeColor = Color.White;
            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("Open", null, OpenFile_Click);
            ToolStripMenuItem saveMenuItem = new ToolStripMenuItem("Save", null, SaveFile_Click);
            ToolStripMenuItem refreshMenuItem = new ToolStripMenuItem("Refresh Dashboard", null, RefreshDashboard);
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Exit", null, (s, e) => this.Close());
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { openMenuItem, saveMenuItem, refreshMenuItem, new ToolStripSeparator(), exitMenuItem });
            
            // Household menu
            ToolStripMenuItem householdMenu = new ToolStripMenuItem("Households");
            householdMenu.ForeColor = Color.White;
            ToolStripMenuItem listHouseholdsMenuItem = new ToolStripMenuItem("List All Households", null, ListHouseholds_Click);
            ToolStripMenuItem addHouseholdMenuItem = new ToolStripMenuItem("Add New Household", null, AddHousehold_Click);
            ToolStripMenuItem removeHouseholdMenuItem = new ToolStripMenuItem("Remove Household", null, RemoveHousehold_Click);
            householdMenu.DropDownItems.AddRange(new ToolStripItem[] { listHouseholdsMenuItem, addHouseholdMenuItem, removeHouseholdMenuItem });
            
            // People menu
            ToolStripMenuItem peopleMenu = new ToolStripMenuItem("People");
            peopleMenu.ForeColor = Color.White;
            ToolStripMenuItem findPersonMenuItem = new ToolStripMenuItem("Find Person", null, FindPerson_Click);
            ToolStripMenuItem addPersonMenuItem = new ToolStripMenuItem("Add Person", null, AddPerson_Click);
            ToolStripMenuItem sortByAgeMenuItem = new ToolStripMenuItem("Sort by Age", null, SortByAge_Click);
            peopleMenu.DropDownItems.AddRange(new ToolStripItem[] { findPersonMenuItem, addPersonMenuItem, sortByAgeMenuItem });
            
            // Reports menu
            ToolStripMenuItem reportsMenu = new ToolStripMenuItem("Reports");
            reportsMenu.ForeColor = Color.White;
            ToolStripMenuItem statisticsMenuItem = new ToolStripMenuItem("Neighborhood Statistics", null, Statistics_Click);
            ToolStripMenuItem mostFewestMembersMenuItem = new ToolStripMenuItem("Households with Most/Fewest Members", null, MostFewestMembers_Click);
            reportsMenu.DropDownItems.AddRange(new ToolStripItem[] { statisticsMenuItem, mostFewestMembersMenuItem });
            
            // Help menu
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Help");
            helpMenu.ForeColor = Color.White;
            ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About", null, About_Click);
            helpMenu.DropDownItems.AddRange(new ToolStripItem[] { aboutMenuItem });
            
            // Add all menus to the menu strip
            mainMenu.Items.AddRange(new ToolStripItem[] { fileMenu, householdMenu, peopleMenu, reportsMenu, helpMenu });
            
            // Add menu to form
            this.Controls.Add(mainMenu);
            
            // Create status strip
            statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.FromArgb(30, 30, 65);
            statusStrip.ForeColor = Color.White;
            statusLabel = new ToolStripStatusLabel("Ready");
            statusLabel.ForeColor = Color.White;
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);
            
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
            // Initialize the dashboard on form load
            RefreshDashboard(sender, e);
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
        /// Creates a statistics box on the statistics panel
        /// </summary>
        private void CreateStatBox(string title, string value, Point location, Color color)
        {
            Panel statBox = new Panel
            {
                Width = 160,
                Height = 80,
                Location = location,
                BackColor = Color.White
            };
            
            // Add rounded corners to the stat box
            statBox.Paint += (sender, e) => {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 5;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(statBox.Width - radius - 1, 0, radius, radius, 270, 90);
                    path.AddArc(statBox.Width - radius - 1, statBox.Height - radius - 1, radius, radius, 0, 90);
                    path.AddArc(0, statBox.Height - radius - 1, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    
                    // Draw left border with color
                    e.Graphics.FillRectangle(new SolidBrush(color), new Rectangle(0, 5, 5, statBox.Height - 10));
                    e.Graphics.FillPath(new SolidBrush(Color.White), path);
                }
            };
            
            // Value label
            Label valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = color,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 5),
                Size = new Size(140, 40),
                AutoSize = false
            };
            
            // Title label
            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 50),
                Size = new Size(140, 20),
                AutoSize = false
            };
            
            statBox.Controls.Add(valueLabel);
            statBox.Controls.Add(titleLabel);
            statisticsPanel.Controls.Add(statBox);
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
    }
}
