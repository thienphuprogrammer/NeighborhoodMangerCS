using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;
using KhuPhoManager.Views.Forms;
using KhuPhoManager.Views.Helpers;

namespace KhuPhoManager.Views.UserControls
{
    /// <summary>
    /// UserControl for the Dashboard panel
    /// </summary>
    public partial class DashboardPanel : UserControl
    {
        private readonly GuiNeighborhoodController _controller;
        private Panel statisticsPanel;
        private ListView householdListView;

        /// <summary>
        /// Constructor for the Dashboard panel
        /// </summary>
        public DashboardPanel(GuiNeighborhoodController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeComponent();
            InitializeCustomComponents();
        }

        /// <summary>
        /// Initialize the dashboard components
        /// </summary>
        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(15, 10, 15, 10);

            // Dashboard title
            Label dashboardTitle = new Label
            {
                Text = "Neighborhood Dashboard",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 5, 0, 0),
                Margin = new Padding(0, 0, 0, 10)
            };

            // Statistics panel with margin
            statisticsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                Padding = new Padding(0, 10, 0, 10),
                Margin = new Padding(0, 0, 0, 15)
            };

            // Recent Activity panel with modern card-based design
            Panel recentActivityPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 220,
                Padding = new Padding(0, 0, 0, 10),
                Margin = new Padding(0, 0, 0, 15)
            };

            // Recent Activity title
            Label recentActivityTitle = new Label
            {
                Text = "Recent Activity",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 30
            };

            // Create a flow layout panel for activity cards
            FlowLayoutPanel activityCardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                Padding = new Padding(5)
            };

            // Create activity cards
            Panel[] activityCards = new Panel[4];
            
            // Activity 1: Household Added
            activityCards[0] = CreateActivityCard(
                DateTime.Now,
                "Household Added",
                "New household #101 at 123 Main St",
                "🏠",
                UIHelper.PrimaryColor);

            // Activity 2: Person Added
            activityCards[1] = CreateActivityCard(
                DateTime.Now.AddHours(-2),
                "Person Added",
                "John Doe added to household #102",
                "👤",
                Color.FromArgb(75, 192, 192));

            // Activity 3: Person Edited
            activityCards[2] = CreateActivityCard(
                DateTime.Now.AddHours(-5),
                "Person Edited",
                "Updated information for Jane Smith in household #103",
                "✏️",
                Color.FromArgb(255, 159, 64));

            // Activity 4: Household Removed
            activityCards[3] = CreateActivityCard(
                DateTime.Now.AddDays(-1),
                "Household Removed",
                "Removed household #104 from the system",
                "🗑️",
                Color.FromArgb(255, 99, 132));

            // Add activity cards to the flow panel
            foreach (var card in activityCards)
            {
                activityCardsPanel.Controls.Add(card);
            }

            // Add controls to recent activity panel
            recentActivityPanel.Controls.Add(activityCardsPanel);
            recentActivityPanel.Controls.Add(recentActivityTitle);

            // Quick Stats panel
            Panel quickStatsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(0, 0, 0, 10),
                Margin = new Padding(0, 0, 0, 15)
            };

            // Quick Stats title
            Label quickStatsTitle = new Label
            {
                Text = "Quick Statistics",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 30
            };

            // Quick Stats content panel
            Panel quickStatsContent = new Panel
            {
                Dock = DockStyle.Fill
            };

            // Create quick stat labels
            Label totalHouseholdsLabel = new Label
            {
                Text = $"Total Households: {_controller.GetHouseholdCount()}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                AutoSize = true
            };

            Label totalPopulationLabel = new Label
            {
                Text = $"Total Population: {_controller.GetTotalPopulation()}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 35),
                AutoSize = true
            };

            Label adultsChildrenLabel = new Label
            {
                Text = $"Adults: {_controller.GetTotalAdults()} | Children: {_controller.GetTotalChildren()}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 60),
                AutoSize = true
            };

            // Add labels to quick stats content panel
            quickStatsContent.Controls.Add(totalHouseholdsLabel);
            quickStatsContent.Controls.Add(totalPopulationLabel);
            quickStatsContent.Controls.Add(adultsChildrenLabel);

            // Add controls to quick stats panel
            quickStatsPanel.Controls.Add(quickStatsContent);
            quickStatsPanel.Controls.Add(quickStatsTitle);

            // Recent Households panel
            Panel recentHouseholdsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 0, 0)
            };

            // Recent Households title
            Label recentHouseholdsTitle = new Label
            {
                Text = "Recent Households",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
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

            // View details button
            Button viewDetailsButton = new Button
            {
                Text = "View Details",
                BackColor = UIHelper.HighlightColor,
                ForeColor = UIHelper.SecondaryColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Location = new Point(10, 10),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 10, 10, 0)
            };
            viewDetailsButton.FlatAppearance.BorderSize = 0;

            // Manage households button
            Button manageHouseholdsButton = new Button
            {
                Text = "Manage Households",
                BackColor = UIHelper.PrimaryColor,
                ForeColor = UIHelper.TextColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(140, 30),
                Location = new Point(140, 10),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 10, 0, 0)
            };
            manageHouseholdsButton.FlatAppearance.BorderSize = 0;

            // Action buttons panel
            Panel actionButtonsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(0, 10, 0, 0)
            };

            // Add buttons to action panel
            actionButtonsPanel.Controls.Add(viewDetailsButton);
            actionButtonsPanel.Controls.Add(manageHouseholdsButton);

            // Add controls to recent households panel
            recentHouseholdsPanel.Controls.Add(householdListView);
            recentHouseholdsPanel.Controls.Add(actionButtonsPanel);
            recentHouseholdsPanel.Controls.Add(recentHouseholdsTitle);

            // Add controls to dashboard panel
            this.Controls.Add(recentHouseholdsPanel);
            this.Controls.Add(quickStatsPanel);
            this.Controls.Add(recentActivityPanel);
            this.Controls.Add(statisticsPanel);
            this.Controls.Add(dashboardTitle);

            // Set up event handlers
            viewDetailsButton.Click += ViewDetailsButton_Click;
            householdListView.DoubleClick += HouseholdListView_DoubleClick;

            // Initial data load
            RefreshDashboard();
        }

        /// <summary>
        /// Handles double-click on the household list view
        /// </summary>
        private void HouseholdListView_DoubleClick(object sender, EventArgs e)
        {
            ViewHouseholdDetails();
        }

        /// <summary>
        /// Handles click on the view details button
        /// </summary>
        private void ViewDetailsButton_Click(object sender, EventArgs e)
        {
            ViewHouseholdDetails();
        }

        /// <summary>
        /// Views the details of the selected household
        /// </summary>
        private void ViewHouseholdDetails()
        {
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
        }

        /// <summary>
        /// Refreshes the dashboard with current data
        /// </summary>
        public void RefreshDashboard()
        {
            RefreshStatistics();
            LoadHouseholdList();
            RefreshRecentActivity();
        }

        /// <summary>
        /// Refreshes the recent activity section with latest data
        /// </summary>
        private void RefreshRecentActivity()
        {
            // In a real application, this would fetch recent activity data from a log or database
            // For now, we'll just update the timestamps to be relative to the current time
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
            UIHelper.CreateStatCard(statsFlow, "Total Households", totalHouseholds.ToString(), UIHelper.PrimaryColor);
            UIHelper.CreateStatCard(statsFlow, "Total Adults", totalAdults.ToString(), UIHelper.SecondaryColor);
            UIHelper.CreateStatCard(statsFlow, "Total Children", totalChildren.ToString(), UIHelper.AccentColor);
            UIHelper.CreateStatCard(statsFlow, "Total Population", totalPopulation.ToString(), UIHelper.HighlightColor);
            UIHelper.CreateStatCard(statsFlow, "Avg. Household Size", avgHouseholdSize.ToString(), Color.FromArgb(75, 192, 192));

            // Add flow panel to statistics panel
            statisticsPanel.Controls.Add(statsFlow);
        }

        /// <summary>
        /// Loads the household list with current data
        /// </summary>
        private void LoadHouseholdList()
        {
            // Clear existing items
            householdListView.Items.Clear();

            // Get households from controller
            var households = _controller.GetHouseholds().OrderByDescending(h => h.HouseNumber).Take(5).ToList();

            // Add each household to the list
            foreach (var household in households)
            {
                AddHouseholdToListView(household);
            }
        }

        /// <summary>
        /// Adds a household to the ListView
        /// </summary>
        private void AddHouseholdToListView(Household household)
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

        /// <summary>
        /// Creates an activity card for the recent activity section
        /// </summary>
        private Panel CreateActivityCard(DateTime timestamp, string action, string details, string icon, Color accentColor)
        {
            // Create card panel
            Panel card = new Panel
            {
                Width = 220,
                Height = 150,
                BackColor = Color.White,
                Margin = new Padding(10),
                Padding = new Padding(15),
                BorderStyle = BorderStyle.None
            };

            // Add shadow and rounded corners
            card.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Draw rounded rectangle
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                int radius = 10;
                Rectangle rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                path.AddArc(rect.X + rect.Width - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
                path.AddArc(rect.X + rect.Width - radius * 2, rect.Y + rect.Height - radius * 2, radius * 2, radius * 2, 0, 90);
                path.AddArc(rect.X, rect.Y + rect.Height - radius * 2, radius * 2, radius * 2, 90, 90);
                path.CloseAllFigures();

                // Fill with white
                using (SolidBrush cardBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(cardBrush, path);
                }

                // Draw accent color on top
                using (SolidBrush accentBrush = new SolidBrush(accentColor))
                {
                    g.FillRectangle(accentBrush, new Rectangle(0, 0, card.Width, 5));
                }

                // Draw subtle border
                using (Pen borderPen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    g.DrawPath(borderPen, path);
                }
            };

            // Icon label
            Label iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24),
                AutoSize = true,
                Location = new Point(15, 15)
            };

            // Action label
            Label actionLabel = new Label
            {
                Text = action,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = accentColor,
                AutoSize = true,
                Location = new Point(15, 55)
            };

            // Details label
            Label detailsLabel = new Label
            {
                Text = details,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Size = new Size(190, 40),
                Location = new Point(15, 80)
            };

            // Timestamp label
            Label timestampLabel = new Label
            {
                Text = FormatTimeAgo(timestamp),
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(15, 120)
            };

            // Add controls to card
            card.Controls.Add(timestampLabel);
            card.Controls.Add(detailsLabel);
            card.Controls.Add(actionLabel);
            card.Controls.Add(iconLabel);

            return card;
        }

        /// <summary>
        /// Formats a timestamp as a human-readable time ago string
        /// </summary>
        private string FormatTimeAgo(DateTime timestamp)
        {
            TimeSpan timeDiff = DateTime.Now - timestamp;

            if (timeDiff.TotalMinutes < 1)
                return "Just now";
            if (timeDiff.TotalMinutes < 60)
                return $"{(int)timeDiff.TotalMinutes} minutes ago";
            if (timeDiff.TotalHours < 24)
                return $"{(int)timeDiff.TotalHours} hours ago";
            if (timeDiff.TotalDays < 7)
                return $"{(int)timeDiff.TotalDays} days ago";

            return timestamp.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
