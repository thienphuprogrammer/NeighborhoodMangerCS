using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;
using FontAwesome.Sharp;

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
            // Main form settings
            this.Text = $"Household #{_household.HouseNumber} Details";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(247, 250, 252); // Light blue-gray background
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            // Theme colors
            Color primaryColor = Color.FromArgb(24, 90, 157);       // Dark blue
            Color secondaryColor = Color.FromArgb(17, 53, 71);      // Even darker blue
            Color highlightColor = Color.FromArgb(86, 204, 242);    // Light blue
            Color accentColor = Color.FromArgb(255, 74, 130);       // Pink accent
            Color panelColor = Color.FromArgb(247, 250, 252);       // Very light blue-gray

            // Create header panel
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.White,
                Padding = new Padding(20, 0, 20, 0)
            };

            // Add shadow to header panel
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

            // House icon
            IconPictureBox houseIcon = new IconPictureBox
            {
                IconChar = IconChar.HouseChimney,
                IconSize = 48,
                IconColor = primaryColor,
                BackColor = Color.Transparent,
                Size = new Size(48, 48),
                Location = new Point(20, 25)
            };

            // Add household info labels
            Label houseNumberLabel = new Label
            {
                Text = $"Household #{_household.HouseNumber}",
                Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold),
                ForeColor = primaryColor,
                Location = new Point(80, 20),
                AutoSize = true
            };

            Label memberInfoLabel = new Label
            {
                Text = $"Total Members: {_household.Members.Count} (Adults: {_household.AdultCount}, Children: {_household.ChildCount})",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                Location = new Point(80, 50),
                AutoSize = true
            };

            headerPanel.Controls.Add(houseIcon);
            headerPanel.Controls.Add(houseNumberLabel);
            headerPanel.Controls.Add(memberInfoLabel);

            // Create main content panel
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = panelColor
            };

            // Create list view container with rounded corners
            Panel listViewContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(1)
            };

            // Add rounded corners to list view container
            listViewContainer.Paint += (sender, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = new GraphicsPath())
                {
                    int radius = 10;
                    Rectangle rect = new Rectangle(0, 0, listViewContainer.Width - 1, listViewContainer.Height - 1);
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

            // Create ListView for members
            membersListView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F)
            };

            // Add columns to ListView
            membersListView.Columns.Add("Type", 100);
            membersListView.Columns.Add("Full Name", 200);
            membersListView.Columns.Add("Age", 60);
            membersListView.Columns.Add("Occupation/Class", 150);
            membersListView.Columns.Add("ID/Birth Certificate", 150);

            // Add title for the members list
            Label membersTitle = new Label
            {
                Text = "Household Members",
                Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold),
                ForeColor = primaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.White,
                Padding = new Padding(15, 10, 0, 0)
            };

            // Create panel for buttons
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.White,
                Padding = new Padding(15, 10, 15, 10)
            };

            // Add rounded corners to bottom panel
            buttonPanel.Paint += (sender, e) => {
                if (buttonPanel.ClientRectangle.Width > 0 && buttonPanel.ClientRectangle.Height > 0)
                {
                    using (var path = new GraphicsPath())
                    {
                        int radius = 10;
                        Rectangle rect = new Rectangle(0, 0, buttonPanel.Width, buttonPanel.Height);
                        path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                        path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                        path.AddLine(rect.Right, rect.Bottom - radius, rect.Right, 0);
                        path.AddLine(rect.Right, 0, rect.Left, 0);
                        path.AddLine(rect.Left, 0, rect.Left, rect.Bottom - radius);
                        path.CloseAllFigures();

                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.FillPath(new SolidBrush(Color.White), path);
                    }
                }
            };

            // Add close button with icon
            closeButton = new IconButton
            {
                Text = "Close",
                IconChar = IconChar.TimesCircle,
                IconColor = Color.White,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(108, 117, 125), // Gray
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 40),
                Location = new Point(this.Width - 120, 15),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (sender, e) => this.Close();

            // Add member button with icon
            addMemberButton = new IconButton
            {
                Text = "Add Member",
                IconChar = IconChar.UserPlus,
                IconColor = Color.White,
                ForeColor = Color.White,
                BackColor = primaryColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 40),
                Location = new Point(this.Width - 260, 15),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            addMemberButton.FlatAppearance.BorderSize = 0;
            addMemberButton.Click += AddMember_Click;

            // Remove member button with icon
            removeMemberButton = new IconButton
            {
                Text = "Remove",
                IconChar = IconChar.UserMinus,
                IconColor = Color.White,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(220, 53, 69), // Red
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 40),
                Location = new Point(this.Width - 370, 15),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Enabled = false,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            removeMemberButton.FlatAppearance.BorderSize = 0;
            removeMemberButton.Click += RemoveMember_Click;

            // Enable/disable remove button based on selection
            membersListView.SelectedIndexChanged += (sender, e) => {
                removeMemberButton.Enabled = membersListView.SelectedItems.Count > 0;
            };

            // Add list view to container
            listViewContainer.Controls.Add(membersListView);
            listViewContainer.Controls.Add(membersTitle);

            // Add controls to panels
            buttonPanel.Controls.Add(closeButton);
            buttonPanel.Controls.Add(addMemberButton);
            buttonPanel.Controls.Add(removeMemberButton);

            contentPanel.Controls.Add(listViewContainer);
            contentPanel.Controls.Add(buttonPanel);

            // Add panels to form
            this.Controls.Add(contentPanel);
            this.Controls.Add(headerPanel);
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
