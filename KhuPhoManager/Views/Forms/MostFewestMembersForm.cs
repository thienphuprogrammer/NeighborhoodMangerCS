using System;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;
using System.Collections.Generic;

namespace KhuPhoManager.Views.Forms
{
    public class MostFewestMembersForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private ListView mostMembersListView;
        private ListView fewestMembersListView;
        
        public MostFewestMembersForm(GuiNeighborhoodController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeComponent();
            LoadHouseholds();
        }
        
        private void InitializeComponent()
        {
            this.Text = "Households with Most/Fewest Members";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            // Create the most members section
            Panel mostMembersPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 250
            };
            
            Label mostMembersLabel = new Label
            {
                Text = "Households with Most Members",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            
            mostMembersListView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(20, 50),
                Size = new Size(750, 180)
            };
            
            // Add columns to the most members list view
            mostMembersListView.Columns.Add("House Number", 120);
            mostMembersListView.Columns.Add("Total Members", 120);
            mostMembersListView.Columns.Add("Adults", 100);
            mostMembersListView.Columns.Add("Children", 100);
            
            mostMembersPanel.Controls.Add(mostMembersLabel);
            mostMembersPanel.Controls.Add(mostMembersListView);
            
            // Create the fewest members section
            Panel fewestMembersPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 250,
                Location = new Point(0, 250)
            };
            
            Label fewestMembersLabel = new Label
            {
                Text = "Households with Fewest Members",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            
            fewestMembersListView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(20, 50),
                Size = new Size(750, 180)
            };
            
            // Add columns to the fewest members list view
            fewestMembersListView.Columns.Add("House Number", 120);
            fewestMembersListView.Columns.Add("Total Members", 120);
            fewestMembersListView.Columns.Add("Adults", 100);
            fewestMembersListView.Columns.Add("Children", 100);
            
            fewestMembersPanel.Controls.Add(fewestMembersLabel);
            fewestMembersPanel.Controls.Add(fewestMembersListView);
            
            // Create button panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60
            };
            
            Button closeButton = new Button
            {
                Text = "Close",
                Location = new Point(350, 15),
                Width = 100,
                Height = 30
            };
            closeButton.Click += (sender, e) => this.Close();
            
            Button viewDetailsButton = new Button
            {
                Text = "View Details",
                Location = new Point(220, 15),
                Width = 120,
                Height = 30
            };
            viewDetailsButton.Click += ViewDetails_Click;
            
            buttonPanel.Controls.Add(closeButton);
            buttonPanel.Controls.Add(viewDetailsButton);
            
            // Add all the panels to the form
            this.Controls.Add(buttonPanel);
            this.Controls.Add(fewestMembersPanel);
            this.Controls.Add(mostMembersPanel);
        }
        
        private void LoadHouseholds()
        {
            // Load households with most members
            var householdsWithMostMembers = _controller.GetHouseholdsWithMostMembers();
            PopulateListView(mostMembersListView, householdsWithMostMembers);
            
            // Load households with fewest members
            var householdsWithFewestMembers = _controller.GetHouseholdsWithFewestMembers();
            PopulateListView(fewestMembersListView, householdsWithFewestMembers);
        }
        
        private void PopulateListView(ListView listView, List<Household> households)
        {
            listView.Items.Clear();
            
            if (households.Count == 0)
            {
                ListViewItem emptyItem = new ListViewItem("No households found");
                listView.Items.Add(emptyItem);
                return;
            }
            
            foreach (var household in households)
            {
                ListViewItem item = new ListViewItem(household.HouseNumber.ToString());
                item.SubItems.Add(household.Members.Count.ToString());
                item.SubItems.Add(household.AdultCount.ToString());
                item.SubItems.Add(household.ChildCount.ToString());
                item.Tag = household;
                
                listView.Items.Add(item);
            }
            
            // Add banding colors for better readability
            for (int i = 0; i < listView.Items.Count; i++)
            {
                if (i % 2 == 0)
                {
                    listView.Items[i].BackColor = Color.AliceBlue;
                }
            }
        }
        
        private void ViewDetails_Click(object sender, EventArgs e)
        {
            ListView activeListView = null;
            
            // Determine which ListView has the focus
            if (mostMembersListView.Focused || mostMembersListView.SelectedItems.Count > 0)
            {
                activeListView = mostMembersListView;
            }
            else if (fewestMembersListView.Focused || fewestMembersListView.SelectedItems.Count > 0)
            {
                activeListView = fewestMembersListView;
            }
            
            if (activeListView?.SelectedItems.Count > 0 && activeListView.SelectedItems[0].Tag is Household household)
            {
                // Open the household details form
                HouseholdDetailsForm detailsForm = new HouseholdDetailsForm(_controller, household);
                detailsForm.ShowDialog();
                
                // Refresh data after viewing details
                LoadHouseholds();
            }
            else
            {
                MessageBox.Show("Please select a household to view the details.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
