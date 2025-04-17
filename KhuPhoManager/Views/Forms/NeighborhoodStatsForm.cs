using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using KhuPhoManager.Controllers;

namespace KhuPhoManager.Views.Forms
{
    public class NeighborhoodStatsForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private Panel statsPanel;
        private Chart populationChart;

        public NeighborhoodStatsForm(GuiNeighborhoodController controller)
        {
            _controller = controller;
            InitializeComponent();
            LoadStatistics();
        }

        private void InitializeComponent()
        {
            this.Text = "Neighborhood Statistics";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create title label
            Label titleLabel = new Label
            {
                Text = "Neighborhood Statistics",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Create stats panel
            statsPanel = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(350, 300),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create chart panel
            Panel chartPanel = new Panel
            {
                Location = new Point(380, 60),
                Size = new Size(380, 300),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create population chart
            populationChart = new Chart
            {
                Dock = DockStyle.Fill
            };

            populationChart.ChartAreas.Add(new ChartArea("MainChartArea"));
            populationChart.Titles.Add(new Title("Population Distribution", Docking.Top, new Font("Segoe UI", 12, FontStyle.Bold), Color.Black));

            chartPanel.Controls.Add(populationChart);

            // Create close button
            Button closeButton = new Button
            {
                Text = "Close",
                Location = new Point(350, 400),
                Width = 100,
                Height = 40
            };
            closeButton.Click += (sender, e) => this.Close();

            // Add controls to form
            this.Controls.Add(titleLabel);
            this.Controls.Add(statsPanel);
            this.Controls.Add(chartPanel);
            this.Controls.Add(closeButton);
        }

        private void LoadStatistics()
        {
            try
            {
                // Get statistics from controller
                var householdCount = _controller.GetHouseholdCount();
                var totalPopulation = _controller.GetTotalPopulation();
                var adultCount = _controller.GetTotalAdults();
                var childCount = _controller.GetTotalChildren();
                
                // Create statistics labels
                int yPosition = 20;
                int spacing = 40;
                
                AddStatLabel("Total Households:", householdCount.ToString(), yPosition);
                yPosition += spacing;
                
                AddStatLabel("Total Population:", totalPopulation.ToString(), yPosition);
                yPosition += spacing;
                
                AddStatLabel("Adults:", adultCount.ToString(), yPosition);
                yPosition += spacing;
                
                AddStatLabel("Children:", childCount.ToString(), yPosition);
                yPosition += spacing;
                
                var adultPercentage = totalPopulation > 0 ? (adultCount * 100.0 / totalPopulation).ToString("F1") + "%" : "0%";
                var childPercentage = totalPopulation > 0 ? (childCount * 100.0 / totalPopulation).ToString("F1") + "%" : "0%";
                
                AddStatLabel("Adult Percentage:", adultPercentage, yPosition);
                yPosition += spacing;
                
                AddStatLabel("Child Percentage:", childPercentage, yPosition);
                yPosition += spacing;
                
                AddStatLabel("Average People per Household:", householdCount > 0 ? (totalPopulation * 1.0 / householdCount).ToString("F1") : "0", yPosition);
                
                // Create pie chart for population distribution
                populationChart.Series.Add(new Series
                {
                    Name = "Population",
                    ChartType = SeriesChartType.Pie,
                    IsValueShownAsLabel = true,
                    LabelFormat = "{0} ({1:P1})"
                });
                
                populationChart.Series["Population"].Points.AddXY("Adults", adultCount);
                populationChart.Series["Population"].Points.AddXY("Children", childCount);
                
                populationChart.Series["Population"].Points[0].Color = Color.SteelBlue;
                populationChart.Series["Population"].Points[1].Color = Color.LightGreen;
                
                // Set legend
                populationChart.Legends.Add(new Legend("Legend"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void AddStatLabel(string labelText, string valueText, int yPosition)
        {
            Label nameLabel = new Label
            {
                Text = labelText,
                Location = new Point(20, yPosition),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            
            Label valueLabel = new Label
            {
                Text = valueText,
                Location = new Point(220, yPosition),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };
            
            statsPanel.Controls.Add(nameLabel);
            statsPanel.Controls.Add(valueLabel);
        }
    }
}
