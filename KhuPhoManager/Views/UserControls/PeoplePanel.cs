using KhuPhoManager.Controllers;
using KhuPhoManager.Models;
using KhuPhoManager.Views.Helpers;

namespace KhuPhoManager.Views.UserControls
{
    /// <summary>
    /// UserControl for the People Management panel
    /// </summary>
    public partial class PeoplePanel : UserControl
    {
        private System.Collections.Generic.List<(IPerson person, int houseNumber, string address)> _allPeople = new System.Collections.Generic.List<(IPerson person, int houseNumber, string address)>();
        private readonly GuiNeighborhoodController _controller;
        private ListView peopleListView;

        // Panel for inline editing
        private Panel editPanel;
        private TextBox nameTextBox;
        private NumericUpDown ageInput;
        private ComboBox typeComboBox;
        private ComboBox householdComboBox;
        private TextBox occupationTextBox;
        private TextBox idNumberTextBox;
        private TextBox schoolTextBox;
        private TextBox birthCertTextBox;
        private NumericUpDown gradeInput;
        private DateTimePicker dateOfBirthPicker;
        private Button saveButton;
        private Button cancelButton;
        private bool isEditing = false;
        private string editingPersonId = null;
        private int editingHouseNumber = -1;

        private Panel personDetailPanel;

        /// <summary>
        /// Constructor for the People panel
        /// </summary>
        public PeoplePanel(GuiNeighborhoodController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeComponent();
            InitializeCustomComponents();
        }

        /// <summary>
        /// Initialize the people panel components
        /// </summary>
        private int _sortColumn = -1;
        private SortOrder _sortOrder = SortOrder.None;
        private string _personNameFilter = string.Empty;

        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(15, 10, 15, 10);

            // People title
            Label peopleTitle = new Label
            {
                Text = "People Management",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 5, 0, 0),
                Margin = new Padding(0, 0, 0, 10)
            };

            // Create a panel for buttons
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(0, 5, 0, 5),
                Margin = new Padding(0, 0, 0, 10)
            };

            // Add Person button
            Button addButton = new Button
            {
                Text = "Add Person",
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

            // Edit Person button
            Button editButton = new Button
            {
                Text = "Edit Person",
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

            // Delete Person button
            Button deleteButton = new Button
            {
                Text = "Delete Person",
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

            // Create ListView for people
            // --- Search box for filtering by name ---
            TextBox searchTextBox = new TextBox
            {
                PlaceholderText = "Find person by name...",
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 8),
                Font = new Font("Segoe UI", 10)
            };
            searchTextBox.TextChanged += (s, e) => ApplyPeopleFilterAndSort();

            // --- ListView for people ---
            peopleListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9),
                Sorting = SortOrder.None
            };
            // Add columns to ListView
            peopleListView.Columns.Add("Name", 200);
            peopleListView.Columns.Add("Birthday", 100);
            peopleListView.Columns.Add("Age", 50);
            peopleListView.Columns.Add("Type", 80);
            peopleListView.Columns.Add("House #", 70);
            peopleListView.Columns.Add("Address", 250);
            // Sorting on column click
            peopleListView.ColumnClick += PeopleListView_ColumnClick;
            // Show details when a person is selected
            peopleListView.ItemSelectionChanged += PeopleListView_ItemSelectionChanged;

            // Add controls to panel
            this.Controls.Add(peopleListView);
            this.Controls.Add(searchTextBox);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(peopleTitle);

            // Initial load
            ApplyPeopleFilterAndSort();
        }

        /// <summary>
        /// Handles sorting when a column header is clicked
        /// </summary>
        private void PeopleListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (_sortColumn == e.Column)
            {
                // Toggle sort order
                _sortOrder = _sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                _sortColumn = e.Column;
                _sortOrder = SortOrder.Ascending;
            }
            ApplyPeopleFilterAndSort();
        }

        /// <summary>
        /// Filter and sort people, then update the ListView
        /// </summary>
        public void ApplyPeopleFilterAndSort()
        {
            // Gather all people with household info
            _allPeople = new System.Collections.Generic.List<(IPerson person, int houseNumber, string address)>();
            var households = _controller.GetHouseholds();
            foreach (var hh in households)
            {
                foreach (var p in hh.Members)
                {
                    _allPeople.Add((p, hh.HouseNumber, hh.Address));
                }
            }

            // Filter
            string filter = _personNameFilter = (this.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Dock == DockStyle.Top)?.Text ?? "").Trim().ToLower();
            var filtered = string.IsNullOrEmpty(filter)
                ? _allPeople
                : _allPeople.Where(t => t.person.FullName.ToLower().Contains(filter)).ToList();

            // Sort
            if (_sortColumn >= 0)
            {
                switch (_sortColumn)
                {
                    case 0: // Name
                        filtered = _sortOrder == SortOrder.Ascending ?
                            filtered.OrderBy(t => t.person.FullName).ToList() :
                            filtered.OrderByDescending(t => t.person.FullName).ToList();
                        break;
                    case 1: // Birthday
                        filtered = _sortOrder == SortOrder.Ascending ?
                            filtered.OrderBy(t => t.person.DateOfBirth).ToList() :
                            filtered.OrderByDescending(t => t.person.DateOfBirth).ToList();
                        break;
                    case 2: // Age
                        filtered = _sortOrder == SortOrder.Ascending ?
                            filtered.OrderBy(t => t.person.Age).ToList() :
                            filtered.OrderByDescending(t => t.person.Age).ToList();
                        break;
                    case 3: // Type
                        filtered = _sortOrder == SortOrder.Ascending ?
                            filtered.OrderBy(t => t.person.PersonType).ToList() :
                            filtered.OrderByDescending(t => t.person.PersonType).ToList();
                        break;
                    case 4: // House #
                        filtered = _sortOrder == SortOrder.Ascending ?
                            filtered.OrderBy(t => t.houseNumber).ToList() :
                            filtered.OrderByDescending(t => t.houseNumber).ToList();
                        break;
                    case 5: // Address
                        filtered = _sortOrder == SortOrder.Ascending ?
                            filtered.OrderBy(t => t.address).ToList() :
                            filtered.OrderByDescending(t => t.address).ToList();
                        break;
                }
            }

            // Update ListView
            peopleListView.BeginUpdate();
            peopleListView.Items.Clear();
            foreach (var tup in filtered)
            {
                var item = new ListViewItem(new[]
                {
                    tup.person.FullName,
                    tup.person.DateOfBirth.ToString("M/d/yyyy"),
                    tup.person.Age.ToString(),
                    tup.person.PersonType,
                    tup.houseNumber.ToString(),
                    tup.address ?? ""
                });
                item.Tag = tup.person;
                peopleListView.Items.Add(item);
            }
            peopleListView.EndUpdate();
        }

        /// <summary>
        /// Handles click on the add button
        /// </summary>
        private void AddButton_Click(object sender, EventArgs e)
        {
            if (isEditing) return; // Prevent multiple edit sessions
            isEditing = true;
            editingPersonId = null; // null indicates adding new
            editingHouseNumber = -1;

            // Show the edit panel
            ShowEditPanel("Add New Person", "", 0, "Adult", -1, "", "", 0);
        }

        /// <summary>
        /// Shows the inline edit panel for adding or editing a person
        /// </summary>
        private void ShowEditPanel(string title, string name, int age, string personType, int householdNumber, string occupation, string school, int grade)
        {
            // Create edit panel if it doesn't exist
            if (editPanel == null)
            {
                // Create the edit panel
                editPanel = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 440,
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
                    RowCount = 8,
                    Padding = new Padding(5),
                    ColumnStyles = { 
                        new ColumnStyle(SizeType.Percent, 30F),
                        new ColumnStyle(SizeType.Percent, 70F)
                    }
                };

                // Name input
                Label nameLabel = new Label
                {
                    Text = "Full Name:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 10, 3, 10)
                };

                nameTextBox = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Text = name,
                    Margin = new Padding(3, 10, 3, 10)
                };

                // Date of Birth input
                Label dobLabel = new Label
                {
                    Text = "Date of Birth:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 10, 3, 10)
                };

                dateOfBirthPicker = new DateTimePicker
                {
                    Dock = DockStyle.Fill,
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Now,
                    Margin = new Padding(3, 10, 3, 10)
                };

                // Age input
                Label ageLabel = new Label
                {
                    Text = "Age:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 10, 3, 10)
                };

                ageInput = new NumericUpDown
                {
                    Dock = DockStyle.Fill,
                    Minimum = 0,
                    Maximum = 120,
                    Value = age,
                    Margin = new Padding(3, 10, 3, 10)
                };

                // Person type input
                Label typeLabel = new Label
                {
                    Text = "Person Type:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 10, 3, 10)
                };

                typeComboBox = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Margin = new Padding(3, 10, 3, 10)
                };
                typeComboBox.Items.AddRange(new string[] { "Adult", "Child" });
                typeComboBox.SelectedItem = personType;

                // Household input
                Label householdLabel = new Label
                {
                    Text = "Household:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 10, 3, 10)
                };

                householdComboBox = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Margin = new Padding(3, 10, 3, 10)
                };

                // Populate households dropdown
                var households = _controller.GetHouseholds();
                foreach (var household in households)
                {
                    householdComboBox.Items.Add(new HouseholdItem(household));
                    if (household.HouseNumber == householdNumber)
                    {
                        householdComboBox.SelectedIndex = householdComboBox.Items.Count - 1;
                    }
                }

                if (householdComboBox.SelectedIndex == -1 && householdComboBox.Items.Count > 0)
                    householdComboBox.SelectedIndex = 0;

                // Additional fields for Adult
                Label occupationLabel = new Label
                {
                    Text = "Occupation:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 10, 3, 10)
                };

                occupationTextBox = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Text = occupation,
                    Margin = new Padding(3, 10, 3, 10)
                };

                Label idNumberLabel = new Label
                {
                    Text = "ID Number:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 10, 3, 10)
                };

                idNumberTextBox = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Text = "",
                    Margin = new Padding(3, 10, 3, 10)
                };

                // Additional fields for Child
                Label schoolLabel = new Label
                {
                    Text = "School:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 10, 3, 10)
                };

                schoolTextBox = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Text = school,
                    Margin = new Padding(3, 10, 3, 10)
                };

                Label birthCertLabel = new Label
                {
                    Text = "Birth Certificate Number:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 10, 3, 10)
                };

                birthCertTextBox = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Text = "",
                    Margin = new Padding(3, 10, 3, 10)
                };

                Label gradeLabel = new Label
                {
                    Text = "Grade:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Margin = new Padding(3, 10, 3, 10)
                };

                gradeInput = new NumericUpDown
                {
                    Dock = DockStyle.Fill,
                    Minimum = 0,
                    Maximum = 12,
                    Margin = new Padding(3, 10, 3, 10)
                };
                // Set the value after setting the min/max to ensure it's within range
                gradeInput.Value = Math.Max(gradeInput.Minimum, Math.Min(grade, gradeInput.Maximum));

                // Show/hide fields based on person type
                typeComboBox.SelectedIndexChanged += (s, args) =>
                {
                    bool isAdult = typeComboBox.SelectedItem.ToString() == "Adult";
                    UpdatePersonTypeFields(isAdult);
                };

                editPanel.Controls.Add(formLayout);

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
                saveButton.Click += SavePerson_Click;
                cancelButton.Click += CancelEdit_Click;

                // Add controls to button panel
                buttonPanel.Controls.Add(saveButton);
                buttonPanel.Controls.Add(cancelButton);

                // Add controls to form layout
                formLayout.Controls.Add(nameLabel, 0, 0);
                formLayout.Controls.Add(nameTextBox, 1, 0);
                formLayout.Controls.Add(dobLabel, 0, 1);
                formLayout.Controls.Add(dateOfBirthPicker, 1, 1);
                formLayout.Controls.Add(ageLabel, 0, 2);
                formLayout.Controls.Add(ageInput, 1, 2);
                formLayout.Controls.Add(typeLabel, 0, 3);
                formLayout.Controls.Add(typeComboBox, 1, 3);
                formLayout.Controls.Add(householdLabel, 0, 4);
                formLayout.Controls.Add(householdComboBox, 1, 4);
                formLayout.Controls.Add(occupationLabel, 0, 5);
                formLayout.Controls.Add(occupationTextBox, 1, 5);
                formLayout.Controls.Add(idNumberLabel, 0, 6);
                formLayout.Controls.Add(idNumberTextBox, 1, 6);
                formLayout.Controls.Add(schoolLabel, 0, 7);
                formLayout.Controls.Add(schoolTextBox, 1, 7);
                formLayout.Controls.Add(birthCertLabel, 0, 8);
                formLayout.Controls.Add(birthCertTextBox, 1, 8);
                formLayout.Controls.Add(gradeLabel, 0, 9);
                formLayout.Controls.Add(gradeInput, 1, 9);
                formLayout.Controls.Add(buttonPanel, 1, 10);

                // Add controls to edit panel
                editPanel.Controls.Add(formLayout);
                editPanel.Controls.Add(editTitle);
            }
            else
            {
                // Update the existing panel
                ((Label)editPanel.Controls[1]).Text = title;
                nameTextBox.Text = name;
                // Set DateOfBirth to today by default; will be updated below if editing
                if (dateOfBirthPicker != null)
                    dateOfBirthPicker.Value = DateTime.Now;
                ageInput.Value = Math.Max(ageInput.Minimum, Math.Min(age, ageInput.Maximum));
                typeComboBox.SelectedItem = personType;
                occupationTextBox.Text = occupation;
                schoolTextBox.Text = school;
                gradeInput.Value = Math.Max(gradeInput.Minimum, Math.Min(grade, gradeInput.Maximum));

                // Update household selection
                householdComboBox.Items.Clear();
                var households = _controller.GetHouseholds();
                foreach (var household in households)
                {
                    householdComboBox.Items.Add(new HouseholdItem(household));
                    if (household.HouseNumber == householdNumber)
                    {
                        householdComboBox.SelectedIndex = householdComboBox.Items.Count - 1;
                    }
                }

                if (householdComboBox.SelectedIndex == -1 && householdComboBox.Items.Count > 0)
                    householdComboBox.SelectedIndex = 0;
            }

            // Set initial visibility of fields based on person type
            bool isAdult = typeComboBox.SelectedItem.ToString() == "Adult";
            UpdatePersonTypeFields(isAdult);

            // Add the edit panel to the main panel
            if (!Controls.Contains(editPanel))
            {
                Controls.Add(editPanel);
                editPanel.BringToFront();
            }
        }

        /// <summary>
        /// Updates the visibility of fields based on person type
        /// </summary>
        private void UpdatePersonTypeFields(bool isAdult)
        {
            TableLayoutPanel formLayout = (TableLayoutPanel)editPanel.Controls[0];
            // Adult fields
            Control occupationLabel = formLayout.GetControlFromPosition(0, 5);
            Control occupationInput = formLayout.GetControlFromPosition(1, 5);
            Control idNumberLabel = formLayout.GetControlFromPosition(0, 6);
            Control idNumberInput = formLayout.GetControlFromPosition(1, 6);
            // Child fields
            Control schoolLabel = formLayout.GetControlFromPosition(0, 7);
            Control schoolInput = formLayout.GetControlFromPosition(1, 7);
            Control birthCertLabel = formLayout.GetControlFromPosition(0, 8);
            Control birthCertInput = formLayout.GetControlFromPosition(1, 8);
            Control gradeLabel = formLayout.GetControlFromPosition(0, 9);
            Control gradeInput = formLayout.GetControlFromPosition(1, 9);

            occupationLabel.Visible = isAdult;
            occupationInput.Visible = isAdult;
            idNumberLabel.Visible = isAdult;
            idNumberInput.Visible = isAdult;
            schoolLabel.Visible = !isAdult;
            schoolInput.Visible = !isAdult;
            birthCertLabel.Visible = !isAdult;
            birthCertInput.Visible = !isAdult;
            gradeLabel.Visible = !isAdult;
            gradeInput.Visible = !isAdult;
        }

        /// <summary>
        /// Handles saving a person (add or edit)
        /// </summary>
        private void SavePerson_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                string name = nameTextBox.Text.Trim();
                int age = (int)ageInput.Value;
                string personType = typeComboBox.SelectedItem.ToString();
                DateTime dateOfBirth = dateOfBirthPicker.Value;
                string idNumber = idNumberTextBox.Text.Trim();
                string birthCertificateNumber = birthCertTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Please enter a name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (householdComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a household.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get selected household
                var selectedHousehold = ((HouseholdItem)householdComboBox.SelectedItem).Household;

                if (editingPersonId == null) // Adding new
                {
                    // Create the appropriate person type
                    IPerson newPerson;
                    if (personType == "Adult")
                    {
                        string occupation = occupationTextBox.Text.Trim();
                        // Generate a random ID for new person
                        newPerson = new Adult(
                            fullName: name, 
                            age: age, 
                            occupation: occupation, 
                            idNumber: idNumber, 
                            dateOfBirth: dateOfBirth
                        );
                    }
                    else // Child
                    {
                        string school = schoolTextBox.Text.Trim();
                        int grade = (int)gradeInput.Value;
                        // Generate a random ID for new person
                        newPerson = new Child(
                            fullName: name, 
                            age: age, 
                            school: school, 
                            grade: grade, 
                            birthCertificateNumber: birthCertificateNumber,
                            dateOfBirth: dateOfBirth
                        );
                    }

                    // Add person to household
                    selectedHousehold.AddMember(newPerson);
                    MessageBox.Show($"{name} added successfully to Household #{selectedHousehold.HouseNumber}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else // Editing existing
                {
                    // Find the person in the original household
                    IPerson existingPerson = null;
                    Household originalHousehold = null;

                    foreach (var household in _controller.GetHouseholds())
                    {
                        foreach (var person in household.Members)
                        {
                            if (person.Id == editingPersonId)
                            {
                                existingPerson = person;
                                originalHousehold = household;
                                break;
                            }
                        }
                        if (existingPerson != null) break;
                    }

                    if (existingPerson == null || originalHousehold == null)
                    {
                        MessageBox.Show("The person you are trying to edit could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        HideEditPanel();
                        return;
                    }

                    // Create updated person
                    IPerson updatedPerson;
                    if (personType == "Adult")
                    {
                        string occupation = occupationTextBox.Text.Trim();
                        // Use the existing person's ID number
                        updatedPerson = new Adult(
                            fullName: name, 
                            age: age, 
                            occupation: occupation, 
                            idNumber: existingPerson.Id, 
                            dateOfBirth: dateOfBirth);
                        // Preserve the original Id property
                        updatedPerson.Id = existingPerson.Id;
                    }
                    else // Child
                    {
                        string school = schoolTextBox.Text.Trim();
                        int grade = (int)gradeInput.Value;
                        // Use the existing person's ID number
                        updatedPerson = new Child(
                            fullName: name, 
                            age: age, 
                            school: school, 
                            grade: grade, 
                            dateOfBirth: dateOfBirth,
                            birthCertificateNumber: birthCertificateNumber
                        );
                        // Preserve the original Id property
                        updatedPerson.Id = existingPerson.Id;
                    }

                    // If household changed, remove from old and add to new
                    if (originalHousehold.HouseNumber != selectedHousehold.HouseNumber)
                    {
                        originalHousehold.RemoveMemberById(editingPersonId);
                        selectedHousehold.AddMember(updatedPerson);
                    }
                    else
                    {
                        // Update in same household
                        _controller.EditPersonInformation(originalHousehold.HouseNumber, editingPersonId, updatedPerson);
                    }

                    MessageBox.Show($"{name} updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Hide the edit panel and refresh the list
                HideEditPanel();
                ApplyPeopleFilterAndSort();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving person: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// Helper class for household items in the combobox
        /// </summary>
        private class HouseholdItem
        {
            public Household Household { get; }

            public HouseholdItem(Household household)
            {
                Household = household;
            }

            public override string ToString()
            {
                return $"#{Household.HouseNumber} - {Household.Address}";
            }
        }

        /// <summary>
        /// Handles click on the edit button
        /// </summary>
        private void EditButton_Click(object sender, EventArgs e)
        {
            if (isEditing) return; // Prevent multiple edit sessions

            if (peopleListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a person to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the selected person
            ListViewItem selectedItem = peopleListView.SelectedItems[0];
            string personName = selectedItem.Text;
            int houseNumber = int.Parse(selectedItem.SubItems[4].Text);

            // Find the person in the household
            var households = _controller.GetHouseholds();
            var household = households.FirstOrDefault(h => h.HouseNumber == houseNumber);
            if (household == null)
            {
                MessageBox.Show("The household for this person could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IPerson person = null;
            foreach (var member in household.Members)
            {
                if (member.FullName == personName)
                {
                    person = member;
                    break;
                }
            }

            if (person == null)
            {
                MessageBox.Show("The selected person could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Set editing mode
            isEditing = true;
            editingPersonId = person.Id; // Use IdNumber as the identifier for finding the person
            editingHouseNumber = houseNumber;

            // Determine person type and specific fields
            string personType = person.PersonType;
            string occupation = "";
            string school = "";
            int grade = 0;

            if (personType == "Adult" && person is Adult adult)
            {
                occupation = adult.Occupation;
            }
            else if (personType == "Child" && person is Child child)
            {
                // Now that we've added School and Grade properties to the Child class, we can use them
                school = child.School;
                grade = child.Grade;
            }

            // Show the edit panel
            ShowEditPanel("Edit Person", person.FullName, person.Age, personType, houseNumber, occupation, school, grade);
        }

        /// <summary>
        /// Handles click on the delete button
        /// </summary>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (isEditing) return; // Prevent actions during edit

            if (peopleListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a person to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the selected person
            ListViewItem selectedItem = peopleListView.SelectedItems[0];
            string personName = selectedItem.Text;
            int houseNumber = int.Parse(selectedItem.SubItems[4].Text);

            // Find the person in the household
            var households = _controller.GetHouseholds();
            var household = households.FirstOrDefault(h => h.HouseNumber == houseNumber);
            if (household == null)
            {
                MessageBox.Show("The household for this person could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IPerson person = null;
            foreach (var member in household.Members)
            {
                if (member.FullName == personName)
                {
                    person = member;
                    break;
                }
            }

            if (person == null)
            {
                MessageBox.Show("The selected person could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Text = $"Are you sure you want to delete {personName} from Household #{houseNumber}?",
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.DarkRed,
                TextAlign = ContentAlignment.MiddleCenter,
            };

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
                    // Delete the person
                    bool success = household.RemoveMemberById(person.Id);

                    if (success)
                    {
                        MessageBox.Show($"{personName} deleted successfully from Household #{houseNumber}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Failed to delete {personName}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Remove the confirmation panel and refresh the list
                Controls.Remove(confirmPanel);
                ApplyPeopleFilterAndSort();
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
        /// Handler for selection change to show detail panel
        /// </summary>
        private void PeopleListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (isEditing) return; // Don't show detail panel while editing
            if (e.IsSelected)
            {
                // Get selected person info
                var selectedItem = e.Item;
                string personName = selectedItem.Text;
                int houseNumber = int.Parse(selectedItem.SubItems[4].Text);
                var household = _controller.GetHouseholds().FirstOrDefault(h => h.HouseNumber == houseNumber);
                if (household == null) return;
                var person = household.Members.FirstOrDefault(p => p.FullName == personName);
                if (person != null)
                {
                    ShowPersonDetailPanel(person, houseNumber);
                }
            }
            else
            {
                HidePersonDetailPanel();
            }
        }

        /// <summary>
        /// Show the inline detail panel for a person
        /// </summary>
        private void ShowPersonDetailPanel(IPerson person, int houseNumber)
        {
            if (personDetailPanel == null)
            {
                personDetailPanel = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 290,
                    BackColor = Color.FromArgb(250, 250, 250),
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(10)
                };
            }
            else
            {
                personDetailPanel.Controls.Clear();
            }

            // Title
            Label detailTitle = new Label
            {
                Text = "Person Details",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor
            };

            // Table for details
            TableLayoutPanel detailLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(5),
                AutoSize = true
            };
            detailLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            detailLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            // Add details
            detailLayout.Controls.Add(new Label { Text = "ID:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 0);
            detailLayout.Controls.Add(new Label { Text = person.Id, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 0);
            detailLayout.Controls.Add(new Label { Text = "Name:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 1);
            detailLayout.Controls.Add(new Label { Text = person.FullName, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 1);
            detailLayout.Controls.Add(new Label { Text = "Date of Birth:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 2);
            detailLayout.Controls.Add(new Label { Text = person.DateOfBirth.ToString("M/d/yyyy"), Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 2);
            detailLayout.Controls.Add(new Label { Text = "Age:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 3);
            detailLayout.Controls.Add(new Label { Text = person.Age.ToString(), Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 3);
            detailLayout.Controls.Add(new Label { Text = "Type:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 4);
            detailLayout.Controls.Add(new Label { Text = person.PersonType, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 4);
            detailLayout.Controls.Add(new Label { Text = "Household:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 5);
            detailLayout.Controls.Add(new Label { Text = houseNumber.ToString(), Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 5);

            var household = _controller.GetHouseholds().FirstOrDefault(h => h.HouseNumber == houseNumber);
            string address = household?.Address ?? "";
            detailLayout.Controls.Add(new Label { Text = "Address:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 6);
            detailLayout.Controls.Add(new Label { Text = address, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 6);

            if (person.PersonType == "Adult" && person is Adult adult)
            {
                detailLayout.Controls.Add(new Label { Text = "Occupation:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 7);
                detailLayout.Controls.Add(new Label { Text = adult.Occupation, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 7);
                detailLayout.Controls.Add(new Label { Text = "Number ID:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 8);
                detailLayout.Controls.Add(new Label { Text = adult.IdNumber, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 8);
                // empty row
                detailLayout.Controls.Add(new Label { Text = "", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 9);
                detailLayout.Controls.Add(new Label { Text = "", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 9);
            }
            else if (person.PersonType == "Child" && person is Child child)
            {
                detailLayout.Controls.Add(new Label { Text = "Birth Certificate Number:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 7);
                detailLayout.Controls.Add(new Label { Text = child.BirthCertificateNumber, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 7);
                detailLayout.Controls.Add(new Label { Text = "School:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 8);
                detailLayout.Controls.Add(new Label { Text = child.School, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 8);
                detailLayout.Controls.Add(new Label { Text = "Grade:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 9);
                detailLayout.Controls.Add(new Label { Text = child.Grade.ToString(), Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) }, 1, 9);
            }

            personDetailPanel.Controls.Add(detailLayout);
            personDetailPanel.Controls.Add(detailTitle);

            if (!Controls.Contains(personDetailPanel))
            {
                Controls.Add(personDetailPanel);
                personDetailPanel.BringToFront();
            }
            else
            {
                personDetailPanel.BringToFront();
            }
        }

        /// <summary>
        /// Hide the person detail panel
        /// </summary>
        private void HidePersonDetailPanel()
        {
            if (personDetailPanel != null && Controls.Contains(personDetailPanel))
            {
                Controls.Remove(personDetailPanel);
            }
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