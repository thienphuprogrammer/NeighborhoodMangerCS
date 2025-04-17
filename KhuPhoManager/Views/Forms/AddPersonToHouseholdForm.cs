using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;
using FontAwesome.Sharp;

namespace KhuPhoManager.Views.Forms
{
    public class AddPersonToHouseholdForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private readonly int _houseNumber;
        private TabControl personTypeTabControl;
        
        // Adult input controls
        private TextBox adultNameTextBox;
        private NumericUpDown adultAgeNumeric;
        private TextBox adultOccupationTextBox;
        private TextBox adultIdTextBox;
        
        // Child input controls
        private TextBox childNameTextBox;
        private NumericUpDown childAgeNumeric;
        private TextBox childSchoolClassTextBox;
        private TextBox childBirthCertTextBox;
        
        // Buttons
        private IconButton addButton;
        private IconButton cancelButton;

        public AddPersonToHouseholdForm(GuiNeighborhoodController controller, int houseNumber)
        {
            _controller = controller;
            _houseNumber = houseNumber;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Theme colors
            Color primaryColor = Color.FromArgb(24, 90, 157);       // Dark blue
            Color secondaryColor = Color.FromArgb(17, 53, 71);      // Even darker blue
            Color highlightColor = Color.FromArgb(86, 204, 242);    // Light blue
            Color accentColor = Color.FromArgb(255, 74, 130);       // Pink accent
            Color panelColor = Color.FromArgb(247, 250, 252);       // Very light blue-gray
            
            // Form settings
            this.Text = $"Add Person to Household #{_houseNumber}";
            this.Size = new Size(580, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = panelColor;
            this.Font = new Font("Segoe UI", 9F);

            // Create header panel
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(20, 0, 20, 0)
            };
            
            // Add shadow to header
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

            // Add header icon
            IconPictureBox headerIcon = new IconPictureBox
            {
                IconChar = IconChar.UserPlus,
                IconSize = 36,
                IconColor = primaryColor,
                BackColor = Color.Transparent,
                Size = new Size(36, 36),
                Location = new Point(20, 22)
            };

            // Add header title
            Label headerTitle = new Label
            {
                Text = $"Add Person to Household #{_houseNumber}",
                Font = new Font("Segoe UI Semibold", 14, FontStyle.Bold),
                ForeColor = primaryColor,
                Location = new Point(70, 25),
                AutoSize = true
            };

            headerPanel.Controls.Add(headerTitle);
            headerPanel.Controls.Add(headerIcon);

            // Create content panel
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = panelColor
            };
            
            // Create tab container with rounded corners
            Panel tabContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            // Add rounded corners to tab container
            tabContainer.Paint += (sender, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = new GraphicsPath())
                {
                    int radius = 10;
                    Rectangle rect = new Rectangle(0, 0, tabContainer.Width - 1, tabContainer.Height - 1);
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

            // Create tab control with a modern look
            personTypeTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.FlatButtons,
                ItemSize = new Size(100, 40),
                Font = new Font("Segoe UI", 10F),
                Padding = new Point(15, 8)
            };

            // Create Adult tab
            TabPage adultTab = new TabPage("Adult");
            adultTab.BackColor = Color.White;
            
            // Create adult icon
            IconPictureBox adultIcon = new IconPictureBox
            {
                IconChar = IconChar.UserTie,
                IconSize = 32,
                IconColor = primaryColor,
                BackColor = Color.Transparent,
                Size = new Size(32, 32),
                Location = new Point(15, 15)
            };
            
            // Add header to the adult tab
            Label adultHeader = new Label
            {
                Text = "Add Adult",
                Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold),
                ForeColor = primaryColor,
                Location = new Point(55, 18),
                AutoSize = true
            };

            // Panel for adult form fields
            Panel adultFormPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 60, 15, 15),
                BackColor = Color.White
            };

            // Adult controls with modern styling
            Label adultNameLabel = new Label { Text = "Full Name:", Location = new Point(10, 20), AutoSize = true, ForeColor = Color.FromArgb(80, 80, 80) };
            adultNameTextBox = new TextBox { 
                Location = new Point(150, 20), 
                Width = 300,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(5)
            };
            
            Label adultAgeLabel = new Label { Text = "Age:", Location = new Point(10, 60), AutoSize = true, ForeColor = Color.FromArgb(80, 80, 80) };
            adultAgeNumeric = new NumericUpDown 
            { 
                Location = new Point(150, 60), 
                Width = 100,
                Font = new Font("Segoe UI", 9.5F),
                Minimum = 18,
                Maximum = 120,
                Value = 30,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            Label adultOccupationLabel = new Label { Text = "Occupation:", Location = new Point(10, 100), AutoSize = true, ForeColor = Color.FromArgb(80, 80, 80) };
            adultOccupationTextBox = new TextBox { 
                Location = new Point(150, 100), 
                Width = 300,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(5)
            };
            
            Label adultIdLabel = new Label { Text = "ID Number:", Location = new Point(10, 140), AutoSize = true, ForeColor = Color.FromArgb(80, 80, 80) };
            adultIdTextBox = new TextBox { 
                Location = new Point(150, 140), 
                Width = 300,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(5)
            };

            // Add controls to adult panel
            adultFormPanel.Controls.Add(adultNameLabel);
            adultFormPanel.Controls.Add(adultNameTextBox);
            adultFormPanel.Controls.Add(adultAgeLabel);
            adultFormPanel.Controls.Add(adultAgeNumeric);
            adultFormPanel.Controls.Add(adultOccupationLabel);
            adultFormPanel.Controls.Add(adultOccupationTextBox);
            adultFormPanel.Controls.Add(adultIdLabel);
            adultFormPanel.Controls.Add(adultIdTextBox);
            
            adultTab.Controls.Add(adultFormPanel);
            adultTab.Controls.Add(adultHeader);
            adultTab.Controls.Add(adultIcon);

            // Create Child tab
            TabPage childTab = new TabPage("Child");
            childTab.BackColor = Color.White;
            
            // Create child icon
            IconPictureBox childIcon = new IconPictureBox
            {
                IconChar = IconChar.Child,
                IconSize = 32,
                IconColor = highlightColor,
                BackColor = Color.Transparent,
                Size = new Size(32, 32),
                Location = new Point(15, 15)
            };
            
            // Add header to the child tab
            Label childHeader = new Label
            {
                Text = "Add Child",
                Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold),
                ForeColor = highlightColor,
                Location = new Point(55, 18),
                AutoSize = true
            };

            // Panel for child form fields
            Panel childFormPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 60, 15, 15),
                BackColor = Color.White
            };

            // Child controls with modern styling
            Label childNameLabel = new Label { Text = "Full Name:", Location = new Point(10, 20), AutoSize = true, ForeColor = Color.FromArgb(80, 80, 80) };
            childNameTextBox = new TextBox { 
                Location = new Point(150, 20), 
                Width = 300,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(5)
            };
            
            Label childAgeLabel = new Label { Text = "Age:", Location = new Point(10, 60), AutoSize = true, ForeColor = Color.FromArgb(80, 80, 80) };
            childAgeNumeric = new NumericUpDown 
            { 
                Location = new Point(150, 60), 
                Width = 100,
                Font = new Font("Segoe UI", 9.5F),
                Minimum = 0,
                Maximum = 17,
                Value = 10,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            Label childSchoolClassLabel = new Label { Text = "School Class:", Location = new Point(10, 100), AutoSize = true, ForeColor = Color.FromArgb(80, 80, 80) };
            childSchoolClassTextBox = new TextBox { 
                Location = new Point(150, 100), 
                Width = 300,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(5)
            };
            
            Label childBirthCertLabel = new Label { Text = "Birth Certificate:", Location = new Point(10, 140), AutoSize = true, ForeColor = Color.FromArgb(80, 80, 80) };
            childBirthCertTextBox = new TextBox { 
                Location = new Point(150, 140), 
                Width = 300,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(5)
            };

            // Add controls to child panel
            childFormPanel.Controls.Add(childNameLabel);
            childFormPanel.Controls.Add(childNameTextBox);
            childFormPanel.Controls.Add(childAgeLabel);
            childFormPanel.Controls.Add(childAgeNumeric);
            childFormPanel.Controls.Add(childSchoolClassLabel);
            childFormPanel.Controls.Add(childSchoolClassTextBox);
            childFormPanel.Controls.Add(childBirthCertLabel);
            childFormPanel.Controls.Add(childBirthCertTextBox);
            
            childTab.Controls.Add(childFormPanel);
            childTab.Controls.Add(childHeader);
            childTab.Controls.Add(childIcon);

            // Add tabs to tab control
            personTypeTabControl.TabPages.Add(adultTab);
            personTypeTabControl.TabPages.Add(childTab);

            // Create button panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                Padding = new Padding(15, 15, 15, 15),
                BackColor = Color.White
            };
            
            // Add rounded corners to button panel
            buttonPanel.Paint += (sender, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
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
                    
                    e.Graphics.FillPath(new SolidBrush(Color.White), path);
                }
            };

            // Create modern buttons with icons
            addButton = new IconButton
            {
                Text = "Add Person",
                IconChar = IconChar.UserPlus,
                IconColor = Color.White,
                ForeColor = Color.White,
                BackColor = primaryColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 40),
                Location = new Point(buttonPanel.Width - 280, 15),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0),
                Font = new Font("Segoe UI", 9.5F)
            };
            addButton.FlatAppearance.BorderSize = 0;
            addButton.Click += AddButton_Click;

            cancelButton = new IconButton
            {
                Text = "Cancel",
                IconChar = IconChar.TimesCircle,
                IconColor = Color.White,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(108, 117, 125),  // Gray
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 40),
                Location = new Point(buttonPanel.Width - 140, 15),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0),
                Font = new Font("Segoe UI", 9.5F)
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (sender, e) => this.Close();

            // Add tab control to container
            tabContainer.Controls.Add(personTypeTabControl);
            
            // Add container to content panel
            contentPanel.Controls.Add(tabContainer);
            
            // Add buttons to button panel
            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(cancelButton);
            
            // Add panels to form
            this.Controls.Add(contentPanel);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(headerPanel);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                IPerson newPerson;
                
                // Check which tab is active
                if (personTypeTabControl.SelectedTab.Text == "Adult")
                {
                    // Validate adult input
                    if (string.IsNullOrWhiteSpace(adultNameTextBox.Text))
                    {
                        MessageBox.Show("Please enter a name for the adult.", "Input Required", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(adultIdTextBox.Text))
                    {
                        MessageBox.Show("Please enter an ID number for the adult.", "Input Required", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    newPerson = new Adult
                    {
                        FullName = adultNameTextBox.Text,
                        Age = (int)adultAgeNumeric.Value,
                        Occupation = adultOccupationTextBox.Text,
                        IdNumber = adultIdTextBox.Text
                    };
                }
                else
                {
                    // Validate child input
                    if (string.IsNullOrWhiteSpace(childNameTextBox.Text))
                    {
                        MessageBox.Show("Please enter a name for the child.", "Input Required", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(childBirthCertTextBox.Text))
                    {
                        MessageBox.Show("Please enter a birth certificate number for the child.", "Input Required", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    newPerson = new Child
                    {
                        FullName = childNameTextBox.Text,
                        Age = (int)childAgeNumeric.Value,
                        SchoolClass = childSchoolClassTextBox.Text,
                        BirthCertificateNumber = childBirthCertTextBox.Text
                    };
                }

                // Add person to household
                bool success = _controller.AddPersonToHousehold(_houseNumber, newPerson);
                
                if (success)
                {
                    MessageBox.Show($"{newPerson.PersonType} {newPerson.FullName} was added to household #{_houseNumber} successfully!", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Ask if user wants to add another person
                    if (MessageBox.Show("Do you want to add another person to this household?", "Add Another", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // Clear form for new entry
                        adultNameTextBox.Clear();
                        adultOccupationTextBox.Clear();
                        adultIdTextBox.Clear();
                        childNameTextBox.Clear();
                        childSchoolClassTextBox.Clear();
                        childBirthCertTextBox.Clear();
                    }
                    else
                    {
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show($"Failed to add person to household #{_houseNumber}.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
