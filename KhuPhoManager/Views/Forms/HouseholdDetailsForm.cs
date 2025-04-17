using System;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;

namespace KhuPhoManager.Views.Forms
{
    public class HouseholdDetailsForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private readonly Household _household;
        private ListView membersListView;
        private Button closeButton;
        private Button addMemberButton;
        private Button removeMemberButton;

        public HouseholdDetailsForm(GuiNeighborhoodController controller, Household household)
        {
            _controller = controller;
            _household = household;
            InitializeComponent();
            LoadMembers();
        }

        private void InitializeComponent()
        {
            this.Text = $"Household #{_household.HouseNumber} Details";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create info panel
            Panel infoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.AliceBlue
            };

            // Add household info labels
            Label houseNumberLabel = new Label
            {
                Text = $"House Number: {_household.HouseNumber}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            Label memberInfoLabel = new Label
            {
                Text = $"Total Members: {_household.Members.Count} (Adults: {_household.AdultCount}, Children: {_household.ChildCount})",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 45),
                AutoSize = true
            };

            infoPanel.Controls.Add(houseNumberLabel);
            infoPanel.Controls.Add(memberInfoLabel);

            // Create ListView for members
            membersListView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(10)
            };

            // Add columns to ListView
            membersListView.Columns.Add("Type", 100);
            membersListView.Columns.Add("Full Name", 200);
            membersListView.Columns.Add("Age", 60);
            membersListView.Columns.Add("Occupation/Class", 150);
            membersListView.Columns.Add("ID/Birth Certificate", 150);

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

            // Add member button
            addMemberButton = new Button
            {
                Text = "Add Member",
                Width = 120,
                Height = 40,
                Location = new Point(550, 10),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            addMemberButton.Click += AddMember_Click;

            // Remove member button
            removeMemberButton = new Button
            {
                Text = "Remove Member",
                Width = 120,
                Height = 40,
                Location = new Point(420, 10),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Enabled = false
            };
            removeMemberButton.Click += RemoveMember_Click;

            // Enable/disable remove button based on selection
            membersListView.SelectedIndexChanged += (sender, e) => {
                removeMemberButton.Enabled = membersListView.SelectedItems.Count > 0;
            };

            // Add controls to form
            buttonPanel.Controls.Add(closeButton);
            buttonPanel.Controls.Add(addMemberButton);
            buttonPanel.Controls.Add(removeMemberButton);
            this.Controls.Add(infoPanel);
            this.Controls.Add(membersListView);
            this.Controls.Add(buttonPanel);
        }

        private void LoadMembers()
        {
            membersListView.Items.Clear();
            
            foreach (var member in _household.Members)
            {
                ListViewItem item = new ListViewItem(member.PersonType);
                item.SubItems.Add(member.FullName);
                item.SubItems.Add(member.Age.ToString());
                
                if (member is Adult adult)
                {
                    item.SubItems.Add(adult.Occupation);
                    item.SubItems.Add(adult.IdNumber);
                }
                else if (member is Child child)
                {
                    item.SubItems.Add(child.SchoolClass);
                    item.SubItems.Add(child.BirthCertificateNumber);
                }
                
                item.Tag = member;
                membersListView.Items.Add(item);
            }
        }

        private void AddMember_Click(object sender, EventArgs e)
        {
            new AddPersonToHouseholdForm(_controller, _household.HouseNumber).ShowDialog();
            
            // Refresh members list
            LoadMembers();
            
            // Update household info
            this.Text = $"Household #{_household.HouseNumber} Details";
            foreach (Control control in this.Controls)
            {
                if (control is Panel panel && panel.Dock == DockStyle.Top)
                {
                    foreach (Control panelControl in panel.Controls)
                    {
                        if (panelControl is Label label && label.Location.Y > 40)
                        {
                            label.Text = $"Total Members: {_household.Members.Count} (Adults: {_household.AdultCount}, Children: {_household.ChildCount})";
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void RemoveMember_Click(object sender, EventArgs e)
        {
            if (membersListView.SelectedItems.Count > 0)
            {
                var person = (IPerson)membersListView.SelectedItems[0].Tag;
                
                if (MessageBox.Show($"Are you sure you want to remove {person.FullName} from this household?", 
                    "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_controller.RemovePersonFromHousehold(_household.HouseNumber, person.IdNumber))
                    {
                        MessageBox.Show("Person removed successfully.", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Refresh members list
                        LoadMembers();
                        
                        // Update household info
                        foreach (Control control in this.Controls)
                        {
                            if (control is Panel panel && panel.Dock == DockStyle.Top)
                            {
                                foreach (Control panelControl in panel.Controls)
                                {
                                    if (panelControl is Label label && label.Location.Y > 40)
                                    {
                                        label.Text = $"Total Members: {_household.Members.Count} (Adults: {_household.AdultCount}, Children: {_household.ChildCount})";
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to remove person.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
