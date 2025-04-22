using System;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;

namespace KhuPhoManager.Views.Forms
{
    public class EditPersonForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private readonly IPerson _originalPerson;
        private readonly int _householdNumber;
        
        // Input controls
        private TextBox nameTextBox;
        private NumericUpDown ageNumeric;
        private TextBox occupationTextBox;
        private TextBox idTextBox;
        private TextBox schoolClassTextBox;
        
        // Buttons
        private Button saveButton;
        private Button cancelButton;

        public EditPersonForm(GuiNeighborhoodController controller, IPerson person, int householdNumber)
        {
            _controller = controller;
            _originalPerson = person;
            _householdNumber = householdNumber;
            InitializeComponent();
            LoadPersonData();
        }

        private void InitializeComponent()
        {
            bool isAdult = _originalPerson is Adult;
            
            this.Text = $"Edit {(isAdult ? "Adult" : "Child")} Information";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create header label
            Label headerLabel = new Label
            {
                Text = $"Edit {_originalPerson.PersonType} Information:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Create form fields
            int currentY = 60;
            
            // Name field
            Label nameLabel = new Label
            {
                Text = "Full Name:",
                Location = new Point(20, currentY),
                AutoSize = true
            };
            
            nameTextBox = new TextBox
            {
                Location = new Point(150, currentY),
                Width = 300
            };
            
            currentY += 40;
            
            // Age field
            Label ageLabel = new Label
            {
                Text = "Age:",
                Location = new Point(20, currentY),
                AutoSize = true
            };
            
            ageNumeric = new NumericUpDown
            {
                Location = new Point(150, currentY),
                Width = 100,
                Minimum = isAdult ? 18 : 0,
                Maximum = isAdult ? 120 : 17
            };
            
            currentY += 40;
            
            // Additional fields based on person type
            if (isAdult)
            {
                // Occupation field
                Label occupationLabel = new Label
                {
                    Text = "Occupation:",
                    Location = new Point(20, currentY),
                    AutoSize = true
                };
                
                occupationTextBox = new TextBox
                {
                    Location = new Point(150, currentY),
                    Width = 300
                };
                
                currentY += 40;
                
                // ID field
                Label idLabel = new Label
                {
                    Text = "ID Number:",
                    Location = new Point(20, currentY),
                    AutoSize = true
                };
                
                idTextBox = new TextBox
                {
                    Location = new Point(150, currentY),
                    Width = 300
                };
                
                // Add adult fields
                this.Controls.Add(occupationLabel);
                this.Controls.Add(occupationTextBox);
                this.Controls.Add(idLabel);
                this.Controls.Add(idTextBox);
            }
            else
            {
                // School Class field
                Label schoolClassLabel = new Label
                {
                    Text = "School Class:",
                    Location = new Point(20, currentY),
                    AutoSize = true
                };
                
                schoolClassTextBox = new TextBox
                {
                    Location = new Point(150, currentY),
                    Width = 300
                };
                
                currentY += 40;
                
                // Birth Certificate field
                Label birthCertLabel = new Label
                {
                    Text = "Birth Certificate:",
                    Location = new Point(20, currentY),
                    AutoSize = true
                };
                
                idTextBox = new TextBox
                {
                    Location = new Point(150, currentY),
                    Width = 300
                };
                
                // Add child fields
                this.Controls.Add(schoolClassLabel);
                this.Controls.Add(schoolClassTextBox);
                this.Controls.Add(birthCertLabel);
                this.Controls.Add(idTextBox);
            }
            
            // Add buttons
            currentY += 60;
            
            saveButton = new Button
            {
                Text = "Save Changes",
                Location = new Point(150, currentY),
                Width = 120,
                Height = 40
            };
            saveButton.Click += SaveButton_Click;
            
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(290, currentY),
                Width = 120,
                Height = 40
            };
            cancelButton.Click += (sender, e) => this.Close();

            // Add common controls to form
            this.Controls.Add(headerLabel);
            this.Controls.Add(nameLabel);
            this.Controls.Add(nameTextBox);
            this.Controls.Add(ageLabel);
            this.Controls.Add(ageNumeric);
            this.Controls.Add(saveButton);
            this.Controls.Add(cancelButton);
        }

        private void LoadPersonData()
        {
            nameTextBox.Text = _originalPerson.FullName;
            ageNumeric.Value = _originalPerson.Age;
            
            if (_originalPerson is Adult adult)
            {
                occupationTextBox.Text = adult.Occupation;
                idTextBox.Text = adult.IdNumber;
            }
            else if (_originalPerson is Child child)
            {
                schoolClassTextBox.Text = child.SchoolClass;
                idTextBox.Text = child.BirthCertificateNumber;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Please enter a name.", "Input Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(idTextBox.Text))
            {
                MessageBox.Show("Please enter an ID/Birth Certificate number.", "Input Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Create updated person object
                IPerson updatedPerson;
                
                if (_originalPerson is Adult)
                {
                    if (string.IsNullOrWhiteSpace(occupationTextBox.Text))
                    {
                        MessageBox.Show("Please enter an occupation.", "Input Required", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                    updatedPerson = new Adult
                    {
                        FullName = nameTextBox.Text,
                        Age = (int)ageNumeric.Value,
                        Occupation = occupationTextBox.Text,
                        IdNumber = idTextBox.Text
                    };
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(schoolClassTextBox.Text))
                    {
                        MessageBox.Show("Please enter a school class.", "Input Required", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                    updatedPerson = new Child
                    {
                        FullName = nameTextBox.Text,
                        Age = (int)ageNumeric.Value,
                        SchoolClass = schoolClassTextBox.Text,
                        BirthCertificateNumber = idTextBox.Text
                    };
                }

                // Update person in the household
                bool success = _controller.EditPersonInformation(_householdNumber, _originalPerson.Id, updatedPerson);
                
                if (success)
                {
                    MessageBox.Show("Person information updated successfully.", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to update person information.", "Error", 
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
