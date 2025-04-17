using System;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Models;

namespace KhuPhoManager.Views.Forms
{
    public class SortByAgeForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private RadioButton ascendingRadioButton;
        private RadioButton descendingRadioButton;
        private Button sortButton;
        private Button closeButton;
        private ListView peopleListView;

        public SortByAgeForm(GuiNeighborhoodController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Sort People by Age";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create sorting panel
            Panel sortingPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80
            };

            Label instructionLabel = new Label
            {
                Text = "Sort people by age:",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            ascendingRadioButton = new RadioButton
            {
                Text = "Ascending (Youngest to Oldest)",
                Location = new Point(150, 18),
                AutoSize = true,
                Checked = true
            };

            descendingRadioButton = new RadioButton
            {
                Text = "Descending (Oldest to Youngest)",
                Location = new Point(370, 18),
                AutoSize = true
            };

            sortButton = new Button
            {
                Text = "Sort",
                Location = new Point(620, 15),
                Width = 100,
                Height = 30
            };
            sortButton.Click += SortButton_Click;

            sortingPanel.Controls.Add(instructionLabel);
            sortingPanel.Controls.Add(ascendingRadioButton);
            sortingPanel.Controls.Add(descendingRadioButton);
            sortingPanel.Controls.Add(sortButton);

            // Create ListView for people
            peopleListView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(10)
            };

            // Add columns to ListView
            peopleListView.Columns.Add("House Number", 100);
            peopleListView.Columns.Add("Type", 80);
            peopleListView.Columns.Add("Full Name", 200);
            peopleListView.Columns.Add("Age", 60);
            peopleListView.Columns.Add("Occupation/Class", 150);
            peopleListView.Columns.Add("ID/Birth Certificate", 150);

            // Create button panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            closeButton = new Button
            {
                Text = "Close",
                Location = new Point(680, 10),
                Width = 100,
                Height = 30,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            closeButton.Click += (sender, e) => this.Close();

            buttonPanel.Controls.Add(closeButton);

            // Add controls to form
            this.Controls.Add(buttonPanel);
            this.Controls.Add(peopleListView);
            this.Controls.Add(sortingPanel);

            // Initial sort
            SortPeople();
        }

        private void SortButton_Click(object sender, EventArgs e)
        {
            SortPeople();
        }

        private void SortPeople()
        {
            peopleListView.Items.Clear();
            
            bool ascending = ascendingRadioButton.Checked;
            var sortedPeople = _controller.GetPeopleSortedByAge(ascending);
            
            foreach (var (person, houseNumber) in sortedPeople)
            {
                ListViewItem item = new ListViewItem(houseNumber.ToString());
                item.SubItems.Add(person.PersonType);
                item.SubItems.Add(person.FullName);
                item.SubItems.Add(person.Age.ToString());
                
                if (person is Adult adult)
                {
                    item.SubItems.Add(adult.Occupation);
                    item.SubItems.Add(adult.IdNumber);
                }
                else if (person is Child child)
                {
                    item.SubItems.Add(child.SchoolClass);
                    item.SubItems.Add(child.BirthCertificateNumber);
                }
                
                peopleListView.Items.Add(item);
            }
            
            // Add color banding to rows for better readability
            for (int i = 0; i < peopleListView.Items.Count; i++)
            {
                if (i % 2 == 0)
                {
                    peopleListView.Items[i].BackColor = Color.AliceBlue;
                }
            }
        }
    }
}
