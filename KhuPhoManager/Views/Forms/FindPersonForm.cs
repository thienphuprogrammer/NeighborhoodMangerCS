using System;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;

namespace KhuPhoManager.Views.Forms
{
    public class FindPersonForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private TextBox idTextBox;
        private Button searchButton;
        private Button closeButton;
        private Panel resultsPanel;
        private Label personInfoLabel;

        public FindPersonForm(GuiNeighborhoodController controller)
        {
            _controller = controller;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Find Person by ID";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create search panel
            Panel searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80
            };

            Label searchLabel = new Label
            {
                Text = "Enter ID or Birth Certificate Number:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            idTextBox = new TextBox
            {
                Location = new Point(20, 45),
                Width = 300,
                Font = new Font("Segoe UI", 10)
            };

            searchButton = new Button
            {
                Text = "Search",
                Location = new Point(330, 43),
                Width = 100,
                Height = 30
            };
            searchButton.Click += SearchButton_Click;

            searchPanel.Controls.Add(searchLabel);
            searchPanel.Controls.Add(idTextBox);
            searchPanel.Controls.Add(searchButton);

            // Create results panel
            resultsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                Visible = false
            };

            Label resultsHeaderLabel = new Label
            {
                Text = "Person Information:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 10),
                AutoSize = true
            };

            personInfoLabel = new Label
            {
                Location = new Point(20, 40),
                Size = new Size(440, 150),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            Button editButton = new Button
            {
                Text = "Edit Person",
                Location = new Point(20, 200),
                Width = 120,
                Height = 30
            };
            editButton.Click += EditButton_Click;

            resultsPanel.Controls.Add(resultsHeaderLabel);
            resultsPanel.Controls.Add(personInfoLabel);
            resultsPanel.Controls.Add(editButton);

            // Create button panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            closeButton = new Button
            {
                Text = "Close",
                Location = new Point(390, 10),
                Width = 80,
                Height = 30,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            closeButton.Click += (sender, e) => this.Close();

            buttonPanel.Controls.Add(closeButton);

            // Add panels to form
            this.Controls.Add(buttonPanel);
            this.Controls.Add(resultsPanel);
            this.Controls.Add(searchPanel);
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(idTextBox.Text))
            {
                MessageBox.Show("Please enter an ID or birth certificate number.", "Input Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = _controller.FindPersonById(idTextBox.Text);
            if (result.HasValue)
            {
                var (person, household) = result.Value;
                
                // Set tag to use for editing
                personInfoLabel.Tag = new { Person = person, HouseholdNumber = household.HouseNumber };
                
                // Display person info
                string personInfo = $"Name: {person.FullName}\r\n" +
                    $"Type: {person.PersonType}\r\n" +
                    $"Age: {person.Age}\r\n";
                
                if (person is Adult adult)
                {
                    personInfo += $"Occupation: {adult.Occupation}\r\n" +
                        $"ID Number: {adult.IdNumber}";
                }
                else if (person is Child child)
                {
                    personInfo += $"School Class: {child.SchoolClass}\r\n" +
                        $"Birth Certificate: {child.BirthCertificateNumber}";
                }
                
                personInfo += $"\r\n\r\nFound in Household #{household.HouseNumber}";
                
                personInfoLabel.Text = personInfo;
                resultsPanel.Visible = true;
            }
            else
            {
                MessageBox.Show($"No person found with ID: {idTextBox.Text}", "Not Found", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                resultsPanel.Visible = false;
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (personInfoLabel.Tag != null)
            {
                dynamic tagInfo = personInfoLabel.Tag;
                IPerson person = tagInfo.Person;
                int householdNumber = tagInfo.HouseholdNumber;
                
                var editForm = new EditPersonForm(_controller, person, householdNumber);
                editForm.ShowDialog();
                
                // Refresh the search results after editing
                SearchButton_Click(sender, e);
            }
        }
    }
}
