using System;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;

namespace KhuPhoManager.Views.Forms
{
    public class HouseholdListForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private ListView householdListView;
        private Button closeButton;

        public HouseholdListForm(GuiNeighborhoodController controller)
        {
            _controller = controller;
            InitializeComponent();
            LoadHouseholds();
        }

        private void InitializeComponent()
        {
            this.Text = "Households";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create ListView for households
            householdListView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(10)
            };

            // Add columns to ListView
            householdListView.Columns.Add("House Number", 120);
            householdListView.Columns.Add("Total Members", 120);
            householdListView.Columns.Add("Adults", 120);
            householdListView.Columns.Add("Children", 120);

            // Create panel for buttons
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60
            };

            // Add close button
            closeButton = new Button
            {
                Text = "Close",
                Width = 100,
                Height = 40,
                Location = new Point(690, 10),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            closeButton.Click += (sender, e) => this.Close();

            // Add view details button
            Button viewDetailsButton = new Button
            {
                Text = "View Details",
                Width = 120,
                Height = 40,
                Location = new Point(560, 10),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            viewDetailsButton.Click += ViewDetails_Click;

            // Add controls to form
            buttonPanel.Controls.Add(closeButton);
            buttonPanel.Controls.Add(viewDetailsButton);
            this.Controls.Add(householdListView);
            this.Controls.Add(buttonPanel);
        }

        private void LoadHouseholds()
        {
            householdListView.Items.Clear();
            
            foreach (var household in _controller.GetHouseholds())
            {
                ListViewItem item = new ListViewItem(household.HouseNumber.ToString());
                item.SubItems.Add(household.Members.Count.ToString());
                item.SubItems.Add(household.AdultCount.ToString());
                item.SubItems.Add(household.ChildCount.ToString());
                item.Tag = household;
                
                householdListView.Items.Add(item);
            }
        }

        private void ViewDetails_Click(object sender, EventArgs e)
        {
            if (householdListView.SelectedItems.Count > 0)
            {
                var household = (Household)householdListView.SelectedItems[0].Tag;
                new HouseholdDetailsForm(_controller, household).ShowDialog();
                
                // Refresh list after viewing details
                LoadHouseholds();
            }
            else
            {
                MessageBox.Show("Please select a household to view", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
