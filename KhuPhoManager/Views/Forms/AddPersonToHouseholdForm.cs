using System;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;

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
        private Button addButton;
        private Button cancelButton;

        public AddPersonToHouseholdForm(GuiNeighborhoodController controller, int houseNumber)
        {
            _controller = controller;
            _houseNumber = houseNumber;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"Add Person to Household #{_houseNumber}";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create instruction label
            Label instructionLabel = new Label
            {
                Text = $"Add a new person to Household #{_houseNumber}:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Create tab control for person types
            personTypeTabControl = new TabControl
            {
                Location = new Point(20, 50),
                Size = new Size(450, 250)
            };

            // Create Adult tab
            TabPage adultTab = new TabPage("Adult");
            Panel adultPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Adult controls
            Label adultNameLabel = new Label { Text = "Full Name:", Location = new Point(10, 20), AutoSize = true };
            adultNameTextBox = new TextBox { Location = new Point(150, 20), Width = 250 };
            
            Label adultAgeLabel = new Label { Text = "Age:", Location = new Point(10, 50), AutoSize = true };
            adultAgeNumeric = new NumericUpDown 
            { 
                Location = new Point(150, 50), 
                Width = 100,
                Minimum = 18,
                Maximum = 120,
                Value = 30
            };
            
            Label adultOccupationLabel = new Label { Text = "Occupation:", Location = new Point(10, 80), AutoSize = true };
            adultOccupationTextBox = new TextBox { Location = new Point(150, 80), Width = 250 };
            
            Label adultIdLabel = new Label { Text = "ID Number:", Location = new Point(10, 110), AutoSize = true };
            adultIdTextBox = new TextBox { Location = new Point(150, 110), Width = 250 };

            // Add controls to adult panel
            adultPanel.Controls.Add(adultNameLabel);
            adultPanel.Controls.Add(adultNameTextBox);
            adultPanel.Controls.Add(adultAgeLabel);
            adultPanel.Controls.Add(adultAgeNumeric);
            adultPanel.Controls.Add(adultOccupationLabel);
            adultPanel.Controls.Add(adultOccupationTextBox);
            adultPanel.Controls.Add(adultIdLabel);
            adultPanel.Controls.Add(adultIdTextBox);
            adultTab.Controls.Add(adultPanel);

            // Create Child tab
            TabPage childTab = new TabPage("Child");
            Panel childPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Child controls
            Label childNameLabel = new Label { Text = "Full Name:", Location = new Point(10, 20), AutoSize = true };
            childNameTextBox = new TextBox { Location = new Point(150, 20), Width = 250 };
            
            Label childAgeLabel = new Label { Text = "Age:", Location = new Point(10, 50), AutoSize = true };
            childAgeNumeric = new NumericUpDown 
            { 
                Location = new Point(150, 50), 
                Width = 100,
                Minimum = 0,
                Maximum = 17,
                Value = 10
            };
            
            Label childSchoolClassLabel = new Label { Text = "School Class:", Location = new Point(10, 80), AutoSize = true };
            childSchoolClassTextBox = new TextBox { Location = new Point(150, 80), Width = 250 };
            
            Label childBirthCertLabel = new Label { Text = "Birth Certificate:", Location = new Point(10, 110), AutoSize = true };
            childBirthCertTextBox = new TextBox { Location = new Point(150, 110), Width = 250 };

            // Add controls to child panel
            childPanel.Controls.Add(childNameLabel);
            childPanel.Controls.Add(childNameTextBox);
            childPanel.Controls.Add(childAgeLabel);
            childPanel.Controls.Add(childAgeNumeric);
            childPanel.Controls.Add(childSchoolClassLabel);
            childPanel.Controls.Add(childSchoolClassTextBox);
            childPanel.Controls.Add(childBirthCertLabel);
            childPanel.Controls.Add(childBirthCertTextBox);
            childTab.Controls.Add(childPanel);

            // Add tabs to tab control
            personTypeTabControl.TabPages.Add(adultTab);
            personTypeTabControl.TabPages.Add(childTab);

            // Create buttons
            addButton = new Button
            {
                Text = "Add Person",
                Location = new Point(120, 320),
                Width = 120,
                Height = 30
            };
            addButton.Click += AddButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(250, 320),
                Width = 120,
                Height = 30
            };
            cancelButton.Click += (sender, e) => this.Close();

            // Add controls to form
            this.Controls.Add(instructionLabel);
            this.Controls.Add(personTypeTabControl);
            this.Controls.Add(addButton);
            this.Controls.Add(cancelButton);
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
