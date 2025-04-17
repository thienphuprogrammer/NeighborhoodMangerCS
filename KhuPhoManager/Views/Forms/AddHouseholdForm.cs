using System;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;

namespace KhuPhoManager.Views.Forms
{
    public class AddHouseholdForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private TextBox houseNumberTextBox;
        private Button addButton;
        private Button cancelButton;

        public AddHouseholdForm(GuiNeighborhoodController controller)
        {
            _controller = controller;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Add New Household";
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create instruction label
            Label instructionLabel = new Label
            {
                Text = "Enter the house number for the new household:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 20),
                AutoSize = true
            };

            // Create house number input
            houseNumberTextBox = new TextBox
            {
                Location = new Point(30, 50),
                Width = 320,
                Font = new Font("Segoe UI", 10)
            };
            
            // Only allow numeric input
            houseNumberTextBox.KeyPress += (sender, e) => {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            };

            // Create buttons
            addButton = new Button
            {
                Text = "Add Household",
                Location = new Point(30, 90),
                Width = 150,
                Height = 40
            };
            addButton.Click += AddButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(200, 90),
                Width = 150,
                Height = 40
            };
            cancelButton.Click += (sender, e) => this.Close();

            // Add controls to form
            this.Controls.Add(instructionLabel);
            this.Controls.Add(houseNumberTextBox);
            this.Controls.Add(addButton);
            this.Controls.Add(cancelButton);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(houseNumberTextBox.Text))
            {
                MessageBox.Show("Please enter a house number.", "Input Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(houseNumberTextBox.Text, out int houseNumber) || houseNumber <= 0)
            {
                MessageBox.Show("Please enter a valid positive house number.", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Household newHousehold = new Household(houseNumber);
                _controller.AddHousehold(newHousehold);
                
                MessageBox.Show($"Household #{houseNumber} added successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Ask if user wants to add members to this household
                if (MessageBox.Show("Do you want to add members to this household?", "Add Members", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Close();
                    new AddPersonToHouseholdForm(_controller, houseNumber).ShowDialog();
                }
                else
                {
                    this.Close();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
