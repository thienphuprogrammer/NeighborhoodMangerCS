using System;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Views.Helpers;

namespace KhuPhoManager.Views.UserControls
{
    /// <summary>
    /// UserControl for the Reports panel
    /// </summary>
    public partial class ReportsPanel : UserControl
    {
        private readonly GuiNeighborhoodController _controller;

        /// <summary>
        /// Constructor for the Reports panel
        /// </summary>
        public ReportsPanel(GuiNeighborhoodController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeComponent();
            InitializeCustomComponents();
        }

        /// <summary>
        /// Initialize the reports panel components
        /// </summary>
        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(15, 20, 15, 20);

            // Reports title
            Label reportsTitle = new Label
            {
                Text = "Reports",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
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
                Padding = new Padding(10)
            };

            // Create report cards
            Panel statisticsCard = UIHelper.CreateActionCard(
                "Neighborhood Statistics", 
                "View statistics about the neighborhood", 
                "📊", 
                (sender, e) => {
                    NeighborhoodStatsForm statsForm = new NeighborhoodStatsForm(_controller);
                    statsForm.ShowDialog();
                }
            );
            
            Panel ageAnalysisCard = UIHelper.CreateActionCard(
                "Age Analysis", 
                "Analyze age distribution in the neighborhood", 
                "📈", 
                (sender, e) => {
                    AgeAnalysisForm ageForm = new AgeAnalysisForm(_controller);
                    ageForm.ShowDialog();
                }
            );

            Panel householdSizeCard = UIHelper.CreateActionCard(
                "Household Size Analysis", 
                "Analyze household sizes in the neighborhood", 
                "🏠", 
                (sender, e) => {
                    HouseholdSizeAnalysisForm sizeForm = new HouseholdSizeAnalysisForm(_controller);
                    sizeForm.ShowDialog();
                }
            );

            Panel populationTrendsCard = UIHelper.CreateActionCard(
                "Population Trends", 
                "View population trends over time", 
                "📊", 
                (sender, e) => {
                    PopulationTrendsForm trendsForm = new PopulationTrendsForm(_controller);
                    trendsForm.ShowDialog();
                }
            );

            // Add cards to the panel
            reportCardsPanel.Controls.Add(statisticsCard);
            reportCardsPanel.Controls.Add(ageAnalysisCard);
            reportCardsPanel.Controls.Add(householdSizeCard);
            reportCardsPanel.Controls.Add(populationTrendsCard);

            // Add controls to reports panel
            this.Controls.Add(reportCardsPanel);
            this.Controls.Add(reportsTitle);
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

    /// <summary>
    /// Form for displaying neighborhood statistics
    /// </summary>
    public class NeighborhoodStatsForm : Form
    {
        private readonly GuiNeighborhoodController _controller;

        public NeighborhoodStatsForm(GuiNeighborhoodController controller)
        {
            _controller = controller;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Neighborhood Statistics";
            this.Size = new Size(650, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label titleLabel = new Label
            {
                Text = "Neighborhood Statistics",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create a panel for statistics
            Panel statsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Get statistics from controller
            int householdCount = _controller.GetHouseholdCount();
            int totalPopulation = _controller.GetTotalPopulation();
            int adultCount = _controller.GetTotalAdults();
            int childCount = _controller.GetTotalChildren();
            double avgHouseholdSize = householdCount > 0 ? (double)totalPopulation / householdCount : 0;
            double adultPercentage = totalPopulation > 0 ? (double)adultCount / totalPopulation * 100 : 0;
            double childPercentage = totalPopulation > 0 ? (double)childCount / totalPopulation * 100 : 0;

            // Create a table layout for statistics
            TableLayoutPanel tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                BackColor = Color.White
            };

            // Set column widths
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Add headers
            AddTableCell(tableLayout, "Statistic", 0, 0, true);
            AddTableCell(tableLayout, "Value", 0, 1, true);

            // Add statistics rows
            AddTableCell(tableLayout, "Total Households", 1, 0);
            AddTableCell(tableLayout, householdCount.ToString(), 1, 1);

            AddTableCell(tableLayout, "Total Population", 2, 0);
            AddTableCell(tableLayout, totalPopulation.ToString(), 2, 1);

            AddTableCell(tableLayout, "Adult Count", 3, 0);
            AddTableCell(tableLayout, $"{adultCount} ({adultPercentage:F1}%)", 3, 1);

            AddTableCell(tableLayout, "Child Count", 4, 0);
            AddTableCell(tableLayout, $"{childCount} ({childPercentage:F1}%)", 4, 1);

            AddTableCell(tableLayout, "Average Household Size", 5, 0);
            AddTableCell(tableLayout, avgHouseholdSize.ToString("F1"), 5, 1);

            // Add households with most and fewest members
            var householdsWithMost = _controller.GetHouseholdsWithMostMembers();
            var householdsWithFewest = _controller.GetHouseholdsWithFewestMembers();

            string mostMembersText = householdsWithMost.Count > 0 
                ? $"#{householdsWithMost[0].HouseNumber} ({householdsWithMost[0].Members.Count} members)" 
                : "N/A";

            string fewestMembersText = householdsWithFewest.Count > 0 
                ? $"#{householdsWithFewest[0].HouseNumber} ({householdsWithFewest[0].Members.Count} members)" 
                : "N/A";

            AddTableCell(tableLayout, "Largest Household", 6, 0);
            AddTableCell(tableLayout, mostMembersText, 6, 1);

            AddTableCell(tableLayout, "Smallest Household", 7, 0);
            AddTableCell(tableLayout, fewestMembersText, 7, 1);

            // Add the table to the stats panel
            statsPanel.Controls.Add(tableLayout);

            // Add a button to export statistics
            Button exportButton = new Button
            {
                Text = "Export Statistics",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = UIHelper.PrimaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 10, 0, 0)
            };
            exportButton.FlatAppearance.BorderSize = 0;

            exportButton.Click += (sender, e) => {
                using (SaveFileDialog saveDialog = new SaveFileDialog()
                {
                    Filter = "Text Files (*.txt)|*.txt",
                    Title = "Export Statistics",
                    FileName = "Neighborhood_Statistics.txt"
                })
                {
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveDialog.FileName))
                            {
                                writer.WriteLine("NEIGHBORHOOD STATISTICS");
                                writer.WriteLine("=======================");
                                writer.WriteLine($"Total Households: {householdCount}");
                                writer.WriteLine($"Total Population: {totalPopulation}");
                                writer.WriteLine($"Adult Count: {adultCount} ({adultPercentage:F1}%)");
                                writer.WriteLine($"Child Count: {childCount} ({childPercentage:F1}%)");
                                writer.WriteLine($"Average Household Size: {avgHouseholdSize:F1}");
                                writer.WriteLine($"Largest Household: {mostMembersText}");
                                writer.WriteLine($"Smallest Household: {fewestMembersText}");
                                writer.WriteLine("\nGenerated on: " + DateTime.Now.ToString());
                            }

                            MessageBox.Show("Statistics exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error exporting statistics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };

            // Add controls to form
            this.Controls.Add(statsPanel);
            this.Controls.Add(exportButton);
            this.Controls.Add(titleLabel);
        }

        private void AddTableCell(TableLayoutPanel table, string text, int row, int column, bool isHeader = false)
        {
            Label label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", isHeader ? 10 : 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? UIHelper.PrimaryColor : Color.Black,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(5),
                AutoSize = false
            };

            table.Controls.Add(label, column, row);
        }
    }

    /// <summary>
    /// Form for displaying age analysis
    /// </summary>
    public class AgeAnalysisForm : Form
    {
        private readonly GuiNeighborhoodController _controller;

        public AgeAnalysisForm(GuiNeighborhoodController controller)
        {
            _controller = controller;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Age Analysis";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label titleLabel = new Label
            {
                Text = "Age Distribution Analysis",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create main panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Create a split container for age distribution and sorted list
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 250,
                Panel1MinSize = 200,
                Panel2MinSize = 100,
                BackColor = UIHelper.PanelColor
            };

            // Get people sorted by age
            var peopleSortedByAge = _controller.GetPeopleSortedByAge();

            // Create age distribution panel
            Panel distributionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create age groups
            Dictionary<string, int> ageGroups = new Dictionary<string, int>
            {
                { "0-10", 0 },
                { "11-20", 0 },
                { "21-30", 0 },
                { "31-40", 0 },
                { "41-50", 0 },
                { "51-60", 0 },
                { "61+", 0 }
            };

            // Count people in each age group
            foreach (var (person, _) in peopleSortedByAge)
            {
                int age = person.Age;
                if (age <= 10) ageGroups["0-10"]++;
                else if (age <= 20) ageGroups["11-20"]++;
                else if (age <= 30) ageGroups["21-30"]++;
                else if (age <= 40) ageGroups["31-40"]++;
                else if (age <= 50) ageGroups["41-50"]++;
                else if (age <= 60) ageGroups["51-60"]++;
                else ageGroups["61+"]++;
            }

            // Find the maximum count for scaling
            int maxCount = ageGroups.Values.Max();

            // Create a panel for the chart
            Panel chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40, 20, 20, 40)
            };

            // Add chart title
            Label chartTitle = new Label
            {
                Text = "Age Distribution Chart",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.TopCenter
            };

            // Create bars for the chart
            int barWidth = (chartPanel.Width - 60) / ageGroups.Count;
            int barSpacing = 10;
            int startX = 50;
            int startY = chartPanel.Height - 50;

            // Draw the chart
            chartPanel.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Draw X and Y axis
                g.DrawLine(Pens.Black, 40, startY, chartPanel.Width - 20, startY); // X axis
                g.DrawLine(Pens.Black, 40, 20, 40, startY); // Y axis

                // Draw bars
                int x = startX;
                foreach (var group in ageGroups)
                {
                    // Calculate bar height and ensure it's at least 1 pixel to avoid zero-height rectangle error
                    int barHeight = maxCount > 0 ? (int)((float)group.Value / maxCount * (startY - 60)) : 0;
                    barHeight = Math.Max(barHeight, group.Value > 0 ? 1 : 0); // Ensure visible bars have at least 1px height
                    
                    // Skip drawing if the height is zero (no data for this group)
                    if (barHeight > 0)
                    {
                        Rectangle barRect = new Rectangle(x, startY - barHeight, barWidth - barSpacing, barHeight);

                    // Fill the bar with a gradient
                    using (System.Drawing.Drawing2D.LinearGradientBrush brush = 
                        new System.Drawing.Drawing2D.LinearGradientBrush(
                            barRect, UIHelper.PrimaryColor, UIHelper.HighlightColor, 90f))
                    {
                        g.FillRectangle(brush, barRect);
                    }

                        // Draw bar outline
                        g.DrawRectangle(Pens.DarkGray, barRect);

                        // Draw value on top of the bar
                        string value = group.Value.ToString();
                        SizeF valueSize = g.MeasureString(value, new Font("Segoe UI", 8));
                        g.DrawString(value, new Font("Segoe UI", 8), Brushes.Black, 
                            x + (barWidth - barSpacing) / 2 - valueSize.Width / 2, 
                            startY - barHeight - 15);
                    }

                    // Draw X-axis label
                    g.DrawString(group.Key, new Font("Segoe UI", 8), Brushes.Black, 
                        x + (barWidth - barSpacing) / 2 - g.MeasureString(group.Key, new Font("Segoe UI", 8)).Width / 2, 
                        startY + 5);

                    x += barWidth;
                }

                // Draw Y-axis labels
                for (int i = 0; i <= 5; i++)
                {
                    int value = maxCount * i / 5;
                    int y = startY - (startY - 60) * i / 5;
                    g.DrawString(value.ToString(), new Font("Segoe UI", 8), Brushes.Black, 10, y - 6);
                    g.DrawLine(Pens.LightGray, 40, y, chartPanel.Width - 20, y); // Grid line
                }
            };

            // Add chart to distribution panel
            distributionPanel.Controls.Add(chartPanel);
            distributionPanel.Controls.Add(chartTitle);

            // Create a ListView for people sorted by age
            ListView peopleListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9)
            };

            // Add columns to the ListView
            peopleListView.Columns.Add("Name", 200);
            peopleListView.Columns.Add("Age", 60);
            peopleListView.Columns.Add("Type", 80);
            peopleListView.Columns.Add("Household", 100);

            // Add people to the ListView
            foreach (var (person, houseNumber) in peopleSortedByAge)
            {
                ListViewItem item = new ListViewItem(person.FullName);
                item.SubItems.Add(person.Age.ToString());
                item.SubItems.Add(person.PersonType);
                item.SubItems.Add(houseNumber.ToString());

                // Alternate row colors for better readability
                item.BackColor = peopleListView.Items.Count % 2 == 0
                    ? Color.White
                    : Color.FromArgb(245, 245, 245);

                peopleListView.Items.Add(item);
            }

            // Add a label for the list
            Label listLabel = new Label
            {
                Text = "People Sorted by Age",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Panel listPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 5, 0, 0)
            };

            listPanel.Controls.Add(peopleListView);
            listPanel.Controls.Add(listLabel);

            // Add panels to split container
            splitContainer.Panel1.Controls.Add(distributionPanel);
            splitContainer.Panel2.Controls.Add(listPanel);

            // Add split container to main panel
            mainPanel.Controls.Add(splitContainer);

            // Add a button to export age analysis
            Button exportButton = new Button
            {
                Text = "Export Analysis",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = UIHelper.PrimaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 10, 0, 0)
            };
            exportButton.FlatAppearance.BorderSize = 0;

            exportButton.Click += (sender, e) => {
                using (SaveFileDialog saveDialog = new SaveFileDialog()
                {
                    Filter = "Text Files (*.txt)|*.txt",
                    Title = "Export Age Analysis",
                    FileName = "Age_Analysis.txt"
                })
                {
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveDialog.FileName))
                            {
                                writer.WriteLine("AGE DISTRIBUTION ANALYSIS");
                                writer.WriteLine("=========================");
                                writer.WriteLine("Age Group Distribution:");
                                foreach (var group in ageGroups)
                                {
                                    writer.WriteLine($"{group.Key}: {group.Value} people");
                                }

                                writer.WriteLine("\nPeople Sorted by Age:");
                                writer.WriteLine("Name\tAge\tType\tHousehold");
                                foreach (var (person, houseNumber) in peopleSortedByAge)
                                {
                                    writer.WriteLine($"{person.FullName}\t{person.Age}\t{person.PersonType}\t{houseNumber}");
                                }

                                writer.WriteLine("\nGenerated on: " + DateTime.Now.ToString());
                            }

                            MessageBox.Show("Age analysis exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error exporting analysis: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };

            // Add controls to form
            this.Controls.Add(mainPanel);
            this.Controls.Add(exportButton);
            this.Controls.Add(titleLabel);
        }
    }

    /// <summary>
    /// Form for displaying household size analysis
    /// </summary>
    public class HouseholdSizeAnalysisForm : Form
    {
        private readonly GuiNeighborhoodController _controller;

        public HouseholdSizeAnalysisForm(GuiNeighborhoodController controller)
        {
            _controller = controller;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Household Size Analysis";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label titleLabel = new Label
            {
                Text = "Household Size Analysis",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create main panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Create a split container for household size chart and list
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 250,
                Panel1MinSize = 200,
                Panel2MinSize = 100,
                BackColor = UIHelper.PanelColor
            };

            // Get households
            var households = _controller.GetHouseholds();

            // Create household size distribution panel
            Panel distributionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create household size groups
            Dictionary<string, int> sizeGroups = new Dictionary<string, int>
            {
                { "1-2", 0 },
                { "3-4", 0 },
                { "5-6", 0 },
                { "7-8", 0 },
                { "9+", 0 }
            };

            // Count households in each size group
            foreach (var household in households)
            {
                int size = household.Members.Count;
                if (size <= 2) sizeGroups["1-2"]++;
                else if (size <= 4) sizeGroups["3-4"]++;
                else if (size <= 6) sizeGroups["5-6"]++;
                else if (size <= 8) sizeGroups["7-8"]++;
                else sizeGroups["9+"]++;
            }

            // Find the maximum count for scaling
            int maxCount = sizeGroups.Values.Max();

            // Create a panel for the chart
            Panel chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40, 20, 20, 40)
            };

            // Add chart title
            Label chartTitle = new Label
            {
                Text = "Household Size Distribution",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.TopCenter
            };

            // Create pie chart
            chartPanel.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Calculate total for percentages
                int total = sizeGroups.Values.Sum();
                if (total == 0) return;

                // Define pie chart dimensions
                int centerX = chartPanel.Width / 2;
                int centerY = chartPanel.Height / 2 - 10;
                int radius = Math.Min(centerX, centerY) - 60;

                // Colors for pie slices
                Color[] colors = new Color[] {
                    UIHelper.PrimaryColor,
                    UIHelper.HighlightColor,
                    Color.FromArgb(255, 74, 130),  // Pink
                    Color.FromArgb(75, 192, 192),  // Teal
                    Color.FromArgb(255, 159, 64)   // Orange
                };

                float startAngle = 0;
                int colorIndex = 0;
                int legendY = 40;

                // Draw each slice and legend item
                foreach (var group in sizeGroups)
                {
                    if (group.Value == 0) continue;

                    float sweepAngle = (float)group.Value / total * 360f;
                    
                    // Draw pie slice - ensure sweep angle is at least 0.1 to avoid rendering issues
                    float safeAngle = Math.Max(sweepAngle, 0.1f);
                    using (SolidBrush brush = new SolidBrush(colors[colorIndex % colors.Length]))
                    {
                        g.FillPie(brush, centerX - radius, centerY - radius, radius * 2, radius * 2, startAngle, safeAngle);
                    }
                    g.DrawPie(Pens.White, centerX - radius, centerY - radius, radius * 2, radius * 2, startAngle, safeAngle);

                    // Calculate percentage
                    float percentage = (float)group.Value / total * 100f;

                    // Draw legend item
                    using (SolidBrush brush = new SolidBrush(colors[colorIndex % colors.Length]))
                    {
                        g.FillRectangle(brush, chartPanel.Width - 180, legendY, 15, 15);
                    }
                    g.DrawRectangle(Pens.Gray, chartPanel.Width - 180, legendY, 15, 15);
                    g.DrawString($"{group.Key} members: {group.Value} ({percentage:F1}%)", 
                        new Font("Segoe UI", 9), Brushes.Black, chartPanel.Width - 160, legendY);

                    // Update for next slice
                    startAngle += sweepAngle;
                    colorIndex++;
                    legendY += 25;
                }

                // Draw title in center
                string centerText = $"{total} Households";
                SizeF textSize = g.MeasureString(centerText, new Font("Segoe UI", 10, FontStyle.Bold));
                g.DrawString(centerText, new Font("Segoe UI", 10, FontStyle.Bold), 
                    Brushes.Black, centerX - textSize.Width / 2, centerY - textSize.Height / 2);
            };

            // Add chart to distribution panel
            distributionPanel.Controls.Add(chartPanel);
            distributionPanel.Controls.Add(chartTitle);

            // Create a ListView for households sorted by size
            ListView householdListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9)
            };

            // Add columns to the ListView
            householdListView.Columns.Add("House #", 70);
            householdListView.Columns.Add("Address", 200);
            householdListView.Columns.Add("Size", 50);
            householdListView.Columns.Add("Adults", 60);
            householdListView.Columns.Add("Children", 60);

            // Add households to the ListView sorted by size
            foreach (var household in households.OrderByDescending(h => h.Members.Count))
            {
                ListViewItem item = new ListViewItem(household.HouseNumber.ToString());
                item.SubItems.Add(household.Address ?? "No Address");
                item.SubItems.Add(household.Members.Count.ToString());
                item.SubItems.Add(household.AdultCount.ToString());
                item.SubItems.Add(household.ChildCount.ToString());

                // Alternate row colors for better readability
                item.BackColor = householdListView.Items.Count % 2 == 0
                    ? Color.White
                    : Color.FromArgb(245, 245, 245);

                householdListView.Items.Add(item);
            }

            // Add a label for the list
            Label listLabel = new Label
            {
                Text = "Households Sorted by Size (Largest to Smallest)",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Panel listPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 5, 0, 0)
            };

            listPanel.Controls.Add(householdListView);
            listPanel.Controls.Add(listLabel);

            // Add panels to split container
            splitContainer.Panel1.Controls.Add(distributionPanel);
            splitContainer.Panel2.Controls.Add(listPanel);

            // Add split container to main panel
            mainPanel.Controls.Add(splitContainer);

            // Add a button to export household size analysis
            Button exportButton = new Button
            {
                Text = "Export Analysis",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = UIHelper.PrimaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 10, 0, 0)
            };
            exportButton.FlatAppearance.BorderSize = 0;

            exportButton.Click += (sender, e) => {
                using (SaveFileDialog saveDialog = new SaveFileDialog()
                {
                    Filter = "Text Files (*.txt)|*.txt",
                    Title = "Export Household Size Analysis",
                    FileName = "Household_Size_Analysis.txt"
                })
                {
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveDialog.FileName))
                            {
                                writer.WriteLine("HOUSEHOLD SIZE ANALYSIS");
                                writer.WriteLine("=======================\n");
                                
                                writer.WriteLine("Household Size Distribution:");
                                int total = sizeGroups.Values.Sum();
                                foreach (var group in sizeGroups)
                                {
                                    float percentage = total > 0 ? (float)group.Value / total * 100f : 0;
                                    writer.WriteLine($"{group.Key} members: {group.Value} households ({percentage:F1}%)");
                                }

                                writer.WriteLine("\nHouseholds Sorted by Size (Largest to Smallest):");
                                writer.WriteLine("House #\tAddress\tSize\tAdults\tChildren");
                                foreach (var household in households.OrderByDescending(h => h.Members.Count))
                                {
                                    writer.WriteLine($"{household.HouseNumber}\t{household.Address ?? "No Address"}\t{household.Members.Count}\t{household.AdultCount}\t{household.ChildCount}");
                                }

                                writer.WriteLine("\nGenerated on: " + DateTime.Now.ToString());
                            }

                            MessageBox.Show("Household size analysis exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error exporting analysis: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };

            // Add controls to form
            this.Controls.Add(mainPanel);
            this.Controls.Add(exportButton);
            this.Controls.Add(titleLabel);
        }
    }

    /// <summary>
    /// Form for displaying population trends
    /// </summary>
    public class PopulationTrendsForm : Form
    {
        private readonly GuiNeighborhoodController _controller;

        public PopulationTrendsForm(GuiNeighborhoodController controller)
        {
            _controller = controller;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Population Trends";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label titleLabel = new Label
            {
                Text = "Population Trends Analysis",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 25, 0, 0)
            };

            // Create main panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Create a tab control for different trend views
            TabControl trendTabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9)
            };

            // Create tabs for different trend views
            TabPage populationTab = new TabPage("Population Distribution");
            TabPage ageTab = new TabPage("Age Trends");
            TabPage householdTab = new TabPage("Household Trends");

            // Get data from controller
            var households = _controller.GetHouseholds();
            int totalPopulation = _controller.GetTotalPopulation();
            int adultCount = _controller.GetTotalAdults();
            int childCount = _controller.GetTotalChildren();

            // Population Distribution Tab
            populationTab.BackColor = UIHelper.PanelColor;
            populationTab.Padding = new Padding(10);

            Panel populationPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create a panel for the population chart
            Panel populationChartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Draw population distribution chart
            populationChartPanel.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Define chart dimensions
                int chartWidth = populationChartPanel.Width - 100;
                int chartHeight = populationChartPanel.Height - 100;
                int startX = 50;
                int startY = populationChartPanel.Height - 50;

                // Draw axes
                g.DrawLine(Pens.Black, startX, startY, startX + chartWidth, startY); // X-axis
                g.DrawLine(Pens.Black, startX, startY, startX, startY - chartHeight); // Y-axis

                // Draw title
                g.DrawString("Population Distribution", new Font("Segoe UI", 12, FontStyle.Bold),
                    Brushes.Black, startX + chartWidth / 2 - 80, 10);

                // Draw bars for adults and children
                int barWidth = 80;
                int spacing = 40;
                int maxValue = Math.Max(adultCount, childCount);
                float scale = maxValue > 0 ? (float)chartHeight / maxValue : 0;

                // Draw adult bar - ensure height is at least 1 pixel if there are adults
                int adultHeight = (int)(adultCount * scale);
                adultHeight = Math.Max(adultHeight, adultCount > 0 ? 1 : 0);
                
                if (adultHeight > 0) {
                    Rectangle adultBar = new Rectangle(startX + spacing, startY - adultHeight, barWidth, adultHeight);
                    using (System.Drawing.Drawing2D.LinearGradientBrush brush =
                        new System.Drawing.Drawing2D.LinearGradientBrush(
                            adultBar, UIHelper.PrimaryColor, Color.FromArgb(100, UIHelper.PrimaryColor), 90f))
                    {
                        g.FillRectangle(brush, adultBar);
                    }
                    g.DrawRectangle(Pens.DarkGray, adultBar);
                }

                // Draw child bar - ensure height is at least 1 pixel if there are children
                int childHeight = (int)(childCount * scale);
                childHeight = Math.Max(childHeight, childCount > 0 ? 1 : 0);
                
                if (childHeight > 0) {
                    Rectangle childBar = new Rectangle(startX + spacing * 2 + barWidth, startY - childHeight, barWidth, childHeight);
                    using (System.Drawing.Drawing2D.LinearGradientBrush brush =
                        new System.Drawing.Drawing2D.LinearGradientBrush(
                            childBar, UIHelper.HighlightColor, Color.FromArgb(100, UIHelper.HighlightColor), 90f))
                    {
                        g.FillRectangle(brush, childBar);
                    }
                    g.DrawRectangle(Pens.DarkGray, childBar);
                }

                // Draw values on top of bars
                g.DrawString(adultCount.ToString(), new Font("Segoe UI", 9, FontStyle.Bold),
                    Brushes.Black, startX + spacing + barWidth / 2 - 10, startY - adultHeight - 20);
                g.DrawString(childCount.ToString(), new Font("Segoe UI", 9, FontStyle.Bold),
                    Brushes.Black, startX + spacing * 2 + barWidth + barWidth / 2 - 10, startY - childHeight - 20);

                // Draw labels below bars
                g.DrawString("Adults", new Font("Segoe UI", 9),
                    Brushes.Black, startX + spacing + barWidth / 2 - 20, startY + 5);
                g.DrawString("Children", new Font("Segoe UI", 9),
                    Brushes.Black, startX + spacing * 2 + barWidth + barWidth / 2 - 25, startY + 5);

                // Draw percentage labels
                float adultPercentage = totalPopulation > 0 ? (float)adultCount / totalPopulation * 100 : 0;
                float childPercentage = totalPopulation > 0 ? (float)childCount / totalPopulation * 100 : 0;

                g.DrawString($"{adultPercentage:F1}%", new Font("Segoe UI", 9),
                    Brushes.Black, startX + spacing + barWidth / 2 - 15, startY - adultHeight / 2 - 10);
                g.DrawString($"{childPercentage:F1}%", new Font("Segoe UI", 9),
                    Brushes.Black, startX + spacing * 2 + barWidth + barWidth / 2 - 15, startY - childHeight / 2 - 10);

                // Draw total population
                g.DrawString($"Total Population: {totalPopulation}", new Font("Segoe UI", 10, FontStyle.Bold),
                    Brushes.Black, startX + chartWidth - 180, startY - chartHeight + 20);

                // Draw legend
                int legendX = startX + chartWidth - 180;
                int legendY = startY - chartHeight + 50;

                g.FillRectangle(new SolidBrush(UIHelper.PrimaryColor), legendX, legendY, 15, 15);
                g.DrawRectangle(Pens.DarkGray, legendX, legendY, 15, 15);
                g.DrawString("Adults", new Font("Segoe UI", 9),
                    Brushes.Black, legendX + 20, legendY);

                g.FillRectangle(new SolidBrush(UIHelper.HighlightColor), legendX, legendY + 25, 15, 15);
                g.DrawRectangle(Pens.DarkGray, legendX, legendY + 25, 15, 15);
                g.DrawString("Children", new Font("Segoe UI", 9),
                    Brushes.Black, legendX + 20, legendY + 25);
            };

            populationPanel.Controls.Add(populationChartPanel);
            populationTab.Controls.Add(populationPanel);

            // Age Trends Tab
            ageTab.BackColor = UIHelper.PanelColor;
            ageTab.Padding = new Padding(10);

            Panel agePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create age groups for a line chart
            Dictionary<string, int> ageGroups = new Dictionary<string, int>
            {
                { "0-10", 0 },
                { "11-20", 0 },
                { "21-30", 0 },
                { "31-40", 0 },
                { "41-50", 0 },
                { "51-60", 0 },
                { "61+", 0 }
            };

            // Count people in each age group
            foreach (var household in households)
            {
                foreach (var person in household.Members)
                {
                    int age = person.Age;
                    if (age <= 10) ageGroups["0-10"]++;
                    else if (age <= 20) ageGroups["11-20"]++;
                    else if (age <= 30) ageGroups["21-30"]++;
                    else if (age <= 40) ageGroups["31-40"]++;
                    else if (age <= 50) ageGroups["41-50"]++;
                    else if (age <= 60) ageGroups["51-60"]++;
                    else ageGroups["61+"]++;
                }
            }

            // Create a panel for the age trend chart
            Panel ageTrendPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Draw age trend line chart
            ageTrendPanel.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Define chart dimensions
                int chartWidth = ageTrendPanel.Width - 100;
                int chartHeight = ageTrendPanel.Height - 100;
                int startX = 50;
                int startY = ageTrendPanel.Height - 50;

                // Draw axes
                g.DrawLine(Pens.Black, startX, startY, startX + chartWidth, startY); // X-axis
                g.DrawLine(Pens.Black, startX, startY, startX, startY - chartHeight); // Y-axis

                // Draw title
                g.DrawString("Age Distribution Trend", new Font("Segoe UI", 12, FontStyle.Bold),
                    Brushes.Black, startX + chartWidth / 2 - 80, 10);

                // Find max value for scaling
                int maxValue = ageGroups.Values.Max();
                float scale = maxValue > 0 ? (float)chartHeight / maxValue : 0;

                // Calculate point spacing
                int pointCount = ageGroups.Count;
                float pointSpacing = (float)chartWidth / (pointCount - 1);

                // Create points for the line
                Point[] points = new Point[pointCount];
                string[] labels = new string[pointCount];
                int index = 0;

                foreach (var group in ageGroups)
                {
                    int x = startX + (int)(index * pointSpacing);
                    int y = startY - (int)(group.Value * scale);
                    points[index] = new Point(x, y);
                    labels[index] = group.Key;
                    index++;
                }

                // Draw the line connecting points
                using (Pen linePen = new Pen(UIHelper.PrimaryColor, 2))
                {
                    g.DrawLines(linePen, points);
                }

                // Draw points and values
                foreach (var point in points)
                {
                    g.FillEllipse(new SolidBrush(UIHelper.HighlightColor), point.X - 5, point.Y - 5, 10, 10);
                    g.DrawEllipse(Pens.DarkGray, point.X - 5, point.Y - 5, 10, 10);
                }

                // Draw X-axis labels
                for (int i = 0; i < labels.Length; i++)
                {
                    int x = startX + (int)(i * pointSpacing);
                    g.DrawString(labels[i], new Font("Segoe UI", 8),
                        Brushes.Black, x - 15, startY + 5);

                    // Draw value above point
                    int value = ageGroups[labels[i]];
                    g.DrawString(value.ToString(), new Font("Segoe UI", 8),
                        Brushes.Black, x - 5, points[i].Y - 20);
                }

                // Draw Y-axis labels
                for (int i = 0; i <= 5; i++)
                {
                    int value = maxValue * i / 5;
                    int y = startY - (int)(value * scale);
                    g.DrawString(value.ToString(), new Font("Segoe UI", 8),
                        Brushes.Black, startX - 30, y - 5);
                    g.DrawLine(Pens.LightGray, startX, y, startX + chartWidth, y); // Grid line
                }
            };

            agePanel.Controls.Add(ageTrendPanel);
            ageTab.Controls.Add(agePanel);

            // Household Trends Tab
            householdTab.BackColor = UIHelper.PanelColor;
            householdTab.Padding = new Padding(10);

            Panel householdPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create a panel for household statistics
            Panel householdStatsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Create table layout for household statistics
            TableLayoutPanel statsTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                BackColor = Color.White
            };

            // Set column widths
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Calculate household statistics
            int householdCount = households.Count;
            double avgHouseholdSize = householdCount > 0 ? (double)totalPopulation / householdCount : 0;
            double avgAdultsPerHousehold = householdCount > 0 ? (double)adultCount / householdCount : 0;
            double avgChildrenPerHousehold = householdCount > 0 ? (double)childCount / householdCount : 0;

            // Add headers
            AddTableCell(statsTable, "Statistic", 0, 0, true);
            AddTableCell(statsTable, "Value", 0, 1, true);

            // Add statistics rows
            AddTableCell(statsTable, "Total Households", 1, 0);
            AddTableCell(statsTable, householdCount.ToString(), 1, 1);

            AddTableCell(statsTable, "Average Household Size", 2, 0);
            AddTableCell(statsTable, avgHouseholdSize.ToString("F1"), 2, 1);

            AddTableCell(statsTable, "Average Adults per Household", 3, 0);
            AddTableCell(statsTable, avgAdultsPerHousehold.ToString("F1"), 3, 1);

            AddTableCell(statsTable, "Average Children per Household", 4, 0);
            AddTableCell(statsTable, avgChildrenPerHousehold.ToString("F1"), 4, 1);

            // Add households with most and fewest members
            var householdsWithMost = _controller.GetHouseholdsWithMostMembers();
            var householdsWithFewest = _controller.GetHouseholdsWithFewestMembers();

            string mostMembersText = householdsWithMost.Count > 0
                ? $"#{householdsWithMost[0].HouseNumber} ({householdsWithMost[0].Members.Count} members)"
                : "N/A";

            string fewestMembersText = householdsWithFewest.Count > 0
                ? $"#{householdsWithFewest[0].HouseNumber} ({householdsWithFewest[0].Members.Count} members)"
                : "N/A";

            AddTableCell(statsTable, "Largest Household", 5, 0);
            AddTableCell(statsTable, mostMembersText, 5, 1);

            AddTableCell(statsTable, "Smallest Household", 6, 0);
            AddTableCell(statsTable, fewestMembersText, 6, 1);

            householdStatsPanel.Controls.Add(statsTable);
            householdPanel.Controls.Add(householdStatsPanel);
            householdTab.Controls.Add(householdPanel);

            // Add tabs to tab control
            trendTabs.Controls.Add(populationTab);
            trendTabs.Controls.Add(ageTab);
            trendTabs.Controls.Add(householdTab);

            // Add tab control to main panel
            mainPanel.Controls.Add(trendTabs);

            // Add a button to export population trends
            Button exportButton = new Button
            {
                Text = "Export Trends",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = UIHelper.PrimaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 10, 0, 0)
            };
            exportButton.FlatAppearance.BorderSize = 0;

            exportButton.Click += (sender, e) => {
                using (SaveFileDialog saveDialog = new SaveFileDialog()
                {
                    Filter = "Text Files (*.txt)|*.txt",
                    Title = "Export Population Trends",
                    FileName = "Population_Trends.txt"
                })
                {
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveDialog.FileName))
                            {
                                writer.WriteLine("POPULATION TRENDS ANALYSIS");
                                writer.WriteLine("==========================\n");

                                writer.WriteLine("Population Distribution:");
                                writer.WriteLine($"Total Population: {totalPopulation}");
                                writer.WriteLine($"Adults: {adultCount} ({(totalPopulation > 0 ? (float)adultCount / totalPopulation * 100 : 0):F1}%)");
                                writer.WriteLine($"Children: {childCount} ({(totalPopulation > 0 ? (float)childCount / totalPopulation * 100 : 0):F1}%)\n");

                                writer.WriteLine("Age Distribution:");
                                foreach (var group in ageGroups)
                                {
                                    writer.WriteLine($"{group.Key}: {group.Value} people");
                                }
                                writer.WriteLine();

                                writer.WriteLine("Household Statistics:");
                                writer.WriteLine($"Total Households: {householdCount}");
                                writer.WriteLine($"Average Household Size: {avgHouseholdSize:F1}");
                                writer.WriteLine($"Average Adults per Household: {avgAdultsPerHousehold:F1}");
                                writer.WriteLine($"Average Children per Household: {avgChildrenPerHousehold:F1}");
                                writer.WriteLine($"Largest Household: {mostMembersText}");
                                writer.WriteLine($"Smallest Household: {fewestMembersText}");

                                writer.WriteLine("\nGenerated on: " + DateTime.Now.ToString());
                            }

                            MessageBox.Show("Population trends exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error exporting trends: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };

            // Add controls to form
            this.Controls.Add(mainPanel);
            this.Controls.Add(exportButton);
            this.Controls.Add(titleLabel);
        }

        private void AddTableCell(TableLayoutPanel table, string text, int row, int column, bool isHeader = false)
        {
            Label label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", isHeader ? 10 : 9, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? UIHelper.PrimaryColor : Color.Black,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(5),
                AutoSize = false
            };

            table.Controls.Add(label, column, row);
        }
    }
}
