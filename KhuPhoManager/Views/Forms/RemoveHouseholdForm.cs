using System;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;

namespace KhuPhoManager.Views.Forms
{
    public class RemoveHouseholdForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private ComboBox householdComboBox;
        private Button removeButton;
        private Button cancelButton;

        public RemoveHouseholdForm(GuiNeighborhoodController controller)
        {
            _controller = controller;
            InitializeComponent();
            LoadHouseholds();
        }

        private void InitializeComponent()
        {
            this.Text = "Remove Household";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create warning label
            Label warningLabel = new Label
            {
                Text = "⚠️ Warning: Removing a household will delete all its members!",
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(350, 40)
            };

            // Create instruction label
            Label instructionLabel = new Label
            {
                Text = "Select the household to remove:",
                Location = new Point(20, 70),
                AutoSize = true
            };

            // Create household dropdown
            householdComboBox = new ComboBox
            {
                Location = new Point(20, 100),
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Create buttons
            removeButton = new Button
            {
                Text = "Remove Household",
                Location = new Point(20, 150),
                Width = 150,
                Height = 40
            };
            removeButton.Click += RemoveButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(200, 150),
                Width = 150,
                Height = 40
            };
            cancelButton.Click += (sender, e) => this.Close();

            // Add controls to form
            this.Controls.Add(warningLabel);
            this.Controls.Add(instructionLabel);
            this.Controls.Add(householdComboBox);
            this.Controls.Add(removeButton);
            this.Controls.Add(cancelButton);
        }

        private void LoadHouseholds()
        {
            householdComboBox.Items.Clear();
            
            var households = _controller.GetHouseholds();
            if (households.Count == 0)
            {
                householdComboBox.Items.Add("No households available");
                householdComboBox.SelectedIndex = 0;
                removeButton.Enabled = false;
                return;
            }
            
            foreach (var household in households)
            {
                string itemText = $"Household #{household.HouseNumber} ({household.Members.Count} members)";
                int index = householdComboBox.Items.Add(itemText);
                householdComboBox.Items[index] = new HouseholdItem(household.HouseNumber, itemText);
            }
            
            if (householdComboBox.Items.Count > 0)
            {
                householdComboBox.SelectedIndex = 0;
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (householdComboBox.SelectedIndex < 0 || !(householdComboBox.SelectedItem is HouseholdItem))
            {
                MessageBox.Show("Please select a household to remove.", "Selection Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            HouseholdItem selectedItem = (HouseholdItem)householdComboBox.SelectedItem;
            int houseNumber = selectedItem.HouseNumber;
            
            // Extract member count from string for confirmation message
            int memberCount = 0;
            string displayText = selectedItem.ToString();
            if (displayText.Contains("(") && displayText.Contains(")"))
            {
                string countStr = displayText.Substring(
                    displayText.IndexOf("(") + 1, 
                    displayText.IndexOf(" members)") - displayText.IndexOf("(") - 1);
                
                int.TryParse(countStr, out memberCount);
            }
            
            string confirmMessage = $"Are you sure you want to remove Household #{houseNumber}?";
            if (memberCount > 0)
            {
                confirmMessage += $"\n\nThis will also remove {memberCount} household members!";
            }
            
            if (MessageBox.Show(confirmMessage, "Confirm Removal", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    bool success = _controller.RemoveHousehold(houseNumber);
                    
                    if (success)
                    {
                        MessageBox.Show($"Household #{houseNumber} was removed successfully.", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadHouseholds(); // Refresh list
                    }
                    else
                    {
                        MessageBox.Show($"Failed to remove Household #{houseNumber}.", "Error", 
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
    
    /// <summary>
    /// Helper class to store household information in combobox items
    /// </summary>
    internal class HouseholdItem
    {
        public int HouseNumber { get; }
        private string DisplayText { get; }
        
        public HouseholdItem(int houseNumber, string displayText)
        {
            HouseNumber = houseNumber;
            DisplayText = displayText;
        }
        
        public override string ToString()
        {
            return DisplayText;
        }
    }
}
