using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;
using KhuPhoManager.Views.Helpers;

namespace KhuPhoManager.Views.UserControls
{
    /// <summary>
    /// UserControl for the Households Management panel
    /// </summary>
    public partial class HouseholdsPanel : UserControl
    {
        private readonly GuiNeighborhoodController _controller;
        private ListView householdsListView;

        /// <summary>
        /// Constructor for the Households panel
        /// </summary>
        public HouseholdsPanel(GuiNeighborhoodController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeComponent();
            InitializeCustomComponents();
        }

        /// <summary>
        /// Initialize the households panel components
        /// </summary>
        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(15, 10, 15, 10);

            // Title
            Label householdsTitle = new Label
            {
                Text = "Households Management",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 40
            };

            // Create a panel for buttons
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(0, 5, 0, 5),
                Margin = new Padding(0, 0, 0, 10)
            };

            // Add Household button
            Button addButton = new Button
            {
                Text = "Add Household",
                BackColor = UIHelper.PrimaryColor,
                ForeColor = UIHelper.TextColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 35),
                Location = new Point(0, 5),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            addButton.FlatAppearance.BorderSize = 0;
            addButton.Click += AddButton_Click;

            // Edit Household button
            Button editButton = new Button
            {
                Text = "Edit Household",
                BackColor = UIHelper.HighlightColor,
                ForeColor = UIHelper.SecondaryColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 35),
                Location = new Point(130, 5),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            editButton.FlatAppearance.BorderSize = 0;
            editButton.Click += EditButton_Click;

            // Delete Household button
            Button deleteButton = new Button
            {
                Text = "Delete Household",
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 35),
                Location = new Point(260, 5),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            deleteButton.FlatAppearance.BorderSize = 0;
            deleteButton.Click += DeleteButton_Click;

            // Add buttons to panel
            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(editButton);
            buttonPanel.Controls.Add(deleteButton);

            // ListView for all households
            householdsListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9)
            };

            householdsListView.Columns.Add("House #", 80);
            householdsListView.Columns.Add("Address", 250);
            householdsListView.Columns.Add("Adults", 80);
            householdsListView.Columns.Add("Children", 80);
            householdsListView.Columns.Add("Total", 80);
            householdsListView.Columns.Add("Avg Age", 80);

            // Double-click to edit
            householdsListView.DoubleClick += EditButton_Click;

            // Add controls
            this.Controls.Add(householdsListView);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(householdsTitle);

            // Initial load
            LoadHouseholdList();
        }

        // Panel for inline editing
        private Panel editPanel;
        private NumericUpDown houseNumberInput;
        private TextBox addressInput;
        private Button saveButton;
        private Button cancelButton;
        private bool isEditing = false;
        private int editingHouseNumber = -1;

        /// <summary>
        /// Handles click on the add button
        /// </summary>
        private void AddButton_Click(object sender, EventArgs e)
        {
            if (isEditing) return; // Prevent multiple edit sessions
            isEditing = true;
            editingHouseNumber = -1; // -1 indicates adding new

            // Hide the button panel and show the edit panel
            ShowEditPanel("Add New Household", 1, ""); // Start with 1 as the minimum value
        }

        /// <summary>
        /// Shows the inline edit panel for adding or editing a household
        /// </summary>
        private void ShowEditPanel(string title, int houseNumber, string address)
        {
            // Create edit panel if it doesn't exist
            if (editPanel == null)
            {
                // Create the edit panel
                editPanel = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 180,
                    BackColor = Color.FromArgb(240, 240, 240),
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(10)
                };

                // Title for the edit panel
                Label editTitle = new Label
                {
                    Text = title,
                    Dock = DockStyle.Top,
                    Height = 30,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = UIHelper.PrimaryColor,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                // Create a table layout panel for the form fields
                TableLayoutPanel formLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 3,
                    Padding = new Padding(5),
                    ColumnStyles = { 
                        new ColumnStyle(SizeType.Percent, 30F),
                        new ColumnStyle(SizeType.Percent, 70F)
                    }
                };

                // House number input
                Label houseNumberLabel = new Label
                {
                    Text = "House Number:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 10, 3, 10)
                };

                houseNumberInput = new NumericUpDown
                {
                    Dock = DockStyle.Fill,
                    Minimum = 1,
                    Maximum = 9999,
                    Margin = new Padding(3, 10, 3, 10)
                };
                // Set the value after setting the min/max to ensure it's within range
                houseNumberInput.Value = Math.Max(houseNumberInput.Minimum, Math.Min(houseNumber, houseNumberInput.Maximum));
                
                // Address input
                Label addressLabel = new Label
                {
                    Text = "Address:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 10, 3, 10)
                };

                addressInput = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Text = address,
                    Margin = new Padding(3, 10, 3, 10)
                };

                // Buttons panel
                Panel buttonPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Height = 40
                };

                saveButton = new Button
                {
                    Text = "Save",
                    Width = 80,
                    Height = 30,
                    BackColor = UIHelper.PrimaryColor,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Location = new Point(formLayout.Width - 180, 5)
                };
                saveButton.FlatAppearance.BorderSize = 0;

                cancelButton = new Button
                {
                    Text = "Cancel",
                    Width = 80,
                    Height = 30,
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Location = new Point(formLayout.Width - 90, 5)
                };
                cancelButton.FlatAppearance.BorderSize = 0;

                // Add event handlers
                saveButton.Click += SaveHousehold_Click;
                cancelButton.Click += CancelEdit_Click;

                // Add controls to button panel
                buttonPanel.Controls.Add(saveButton);
                buttonPanel.Controls.Add(cancelButton);

                // Add controls to form layout
                formLayout.Controls.Add(houseNumberLabel, 0, 0);
                formLayout.Controls.Add(houseNumberInput, 1, 0);
                formLayout.Controls.Add(addressLabel, 0, 1);
                formLayout.Controls.Add(addressInput, 1, 1);
                formLayout.Controls.Add(buttonPanel, 1, 2);

                // Add controls to edit panel
                editPanel.Controls.Add(formLayout);
                editPanel.Controls.Add(editTitle);
            }
            else
            {
                // Update the existing panel
                ((Label)editPanel.Controls[1]).Text = title;
                houseNumberInput.Value = Math.Max(houseNumberInput.Minimum, Math.Min(houseNumber, houseNumberInput.Maximum));
                addressInput.Text = address;
            }

            // Disable house number input if editing
            houseNumberInput.Enabled = (editingHouseNumber == -1);

            // Add the edit panel to the main panel
            if (!Controls.Contains(editPanel))
            {
                Controls.Add(editPanel);
                editPanel.BringToFront();
            }
        }

        /// <summary>
        /// Handles saving a household (add or edit)
        /// </summary>
        private void SaveHousehold_Click(object sender, EventArgs e)
        {
            try
            {
                int houseNumber = (int)houseNumberInput.Value;
                string address = addressInput.Text.Trim();

                if (string.IsNullOrWhiteSpace(address))
                {
                    MessageBox.Show("Address cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (editingHouseNumber == -1) // Adding new
                {
                    // Check if house number already exists
                    if (_controller.GetHouseholdByNumber(houseNumber) != null)
                    {
                        MessageBox.Show($"A household with house number {houseNumber} already exists.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Create and add the household
                    Household newHousehold = new Household(houseNumber, address);
                    _controller.AddHousehold(newHousehold);
                }
                else // Editing existing
                {
                    // Get the household
                    Household household = _controller.GetHouseholdByNumber(editingHouseNumber);
                    if (household != null)
                    {
                        // Update the address
                        household.Address = address;
                    }
                }

                // Hide the edit panel and refresh the list
                HideEditPanel();
                LoadHouseholdList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving household: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cancels the edit operation
        /// </summary>
        private void CancelEdit_Click(object sender, EventArgs e)
        {
            HideEditPanel();
        }

        /// <summary>
        /// Hides the edit panel
        /// </summary>
        private void HideEditPanel()
        {
            if (editPanel != null && Controls.Contains(editPanel))
            {
                Controls.Remove(editPanel);
            }
            isEditing = false;
        }

        /// <summary>
        /// Handles click on the edit button
        /// </summary>
        private void EditButton_Click(object sender, EventArgs e)
        {
            if (isEditing) return; // Prevent multiple edit sessions

            if (householdsListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a household to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the selected household
            ListViewItem selectedItem = householdsListView.SelectedItems[0];
            int houseNumber = int.Parse(selectedItem.Text);
            Household household = _controller.GetHouseholdByNumber(houseNumber);

            if (household == null)
            {
                MessageBox.Show("The selected household could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Set editing mode and show edit panel
            isEditing = true;
            editingHouseNumber = houseNumber;
            ShowEditPanel("Edit Household", houseNumber, household.Address);
        }

        /// <summary>
        /// Handles click on the delete button
        /// </summary>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (isEditing) return; // Prevent actions during edit

            if (householdsListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a household to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the selected household
            ListViewItem selectedItem = householdsListView.SelectedItems[0];
            int houseNumber = int.Parse(selectedItem.Text);
            Household household = _controller.GetHouseholdByNumber(houseNumber);

            if (household == null)
            {
                MessageBox.Show("The selected household could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create an inline deletion confirmation panel
            Panel confirmPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.FromArgb(255, 235, 235), // Light red background for warning
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            // Warning label
            Label warningLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.DarkRed,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Set appropriate warning message
            if (household.Members.Count > 0)
            {
                warningLabel.Text = $"Warning: Household #{houseNumber} has {household.Members.Count} members.\nDeleting this household will remove all its members as well.";
            }
            else
            {
                warningLabel.Text = $"Are you sure you want to delete Household #{houseNumber}?";
            }

            // Buttons panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };

            // Confirm button
            Button confirmButton = new Button
            {
                Text = "Delete",
                Width = 80,
                Height = 30,
                BackColor = Color.Red,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(confirmPanel.Width / 2 - 90, 5)
            };
            confirmButton.FlatAppearance.BorderSize = 0;

            // Cancel button
            Button cancelDeleteButton = new Button
            {
                Text = "Cancel",
                Width = 80,
                Height = 30,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(confirmPanel.Width / 2 + 10, 5)
            };
            cancelDeleteButton.FlatAppearance.BorderSize = 0;

            // Add event handlers
            confirmButton.Click += (s, args) =>
            {
                try
                {
                    // Delete the household
                    bool success = _controller.RemoveHousehold(houseNumber);

                    if (success)
                    {
                        MessageBox.Show($"Household #{houseNumber} deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the household.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Remove the confirmation panel and refresh the list
                Controls.Remove(confirmPanel);
                LoadHouseholdList();
            };

            cancelDeleteButton.Click += (s, args) =>
            {
                // Just remove the confirmation panel
                Controls.Remove(confirmPanel);
            };

            // Add controls to panels
            buttonPanel.Controls.Add(confirmButton);
            buttonPanel.Controls.Add(cancelDeleteButton);
            confirmPanel.Controls.Add(buttonPanel);
            confirmPanel.Controls.Add(warningLabel);

            // Add the confirmation panel to the main panel
            Controls.Add(confirmPanel);
            confirmPanel.BringToFront();
        }

        /// <summary>
        /// Loads the household list with current data
        /// </summary>
        public void LoadHouseholdList()
        {
            // Clear existing items
            householdsListView.Items.Clear();

            // Get households from controller
            var households = _controller.GetHouseholds();

            // Add each household to the list
            foreach (var household in households)
            {
                AddHouseholdToListView(household);
            }
        }

        /// <summary>
        /// Adds a household to the ListView
        /// </summary>
        private void AddHouseholdToListView(Household household)
        {
            int adultCount = household.AdultCount;
            int childCount = household.ChildCount;
            int totalMembers = adultCount + childCount;
            double avgAge = household.GetAverageAge();

            ListViewItem item = new ListViewItem(household.HouseNumber.ToString());
            item.SubItems.Add(household.Address ?? "No Address");
            item.SubItems.Add(adultCount.ToString());
            item.SubItems.Add(childCount.ToString());
            item.SubItems.Add(totalMembers.ToString());
            item.SubItems.Add(avgAge > 0 ? avgAge.ToString("F1") : "N/A");

            // Alternate row colors for better readability
            item.BackColor = householdsListView.Items.Count % 2 == 0
                ? Color.White
                : Color.FromArgb(245, 245, 245);

            householdsListView.Items.Add(item);
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
