using System;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Views.Helpers;

namespace KhuPhoManager.Views.UserControls
{
    /// <summary>
    /// UserControl for the Settings panel
    /// </summary>
    public partial class SettingsPanel : UserControl
    {
        private readonly GuiNeighborhoodController _controller;

        /// <summary>
        /// Constructor for the Settings panel
        /// </summary>
        public SettingsPanel(GuiNeighborhoodController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeComponent();
            InitializeCustomComponents();
        }

        /// <summary>
        /// Initialize the settings panel components
        /// </summary>
        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(15, 10, 15, 10);

            // Settings panel title
            Label settingsTitle = new Label
            {
                Text = "Settings",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 5, 0, 0)
            };

            // Create a flow layout panel for settings cards
            FlowLayoutPanel settingsCardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                Padding = new Padding(0, 20, 0, 0)
            };

            // Add setting cards
            Panel openFileCard = UIHelper.CreateActionCard(
                "Open File",
                "Load neighborhood data from a file",
                "📂",
                OpenFile_Click);

            Panel saveFileCard = UIHelper.CreateActionCard(
                "Save File",
                "Save neighborhood data to a file",
                "💾",
                SaveFile_Click);

            Panel aboutCard = UIHelper.CreateActionCard(
                "About",
                "View information about this application",
                "ℹ️",
                About_Click);

            // Add cards to the flow panel
            settingsCardsPanel.Controls.Add(openFileCard);
            settingsCardsPanel.Controls.Add(saveFileCard);
            settingsCardsPanel.Controls.Add(aboutCard);

            // Add controls to the settings panel
            this.Controls.Add(settingsCardsPanel);
            this.Controls.Add(settingsTitle);
        }

        /// <summary>
        /// Handles click on the Open File card
        /// </summary>
        private void OpenFile_Click(object sender, EventArgs e)
        {
            // Create an open file dialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.Title = "Open Neighborhood Data File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filename = openFileDialog.FileName;
                    ReadFileForm readFileForm = new ReadFileForm(_controller, filename);
                    readFileForm.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Handles click on the Save File card
        /// </summary>
        private void SaveFile_Click(object sender, EventArgs e)
        {
            // Create a save file dialog
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.Title = "Save Neighborhood Data File";
                saveFileDialog.DefaultExt = "csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filename = saveFileDialog.FileName;
                    SaveFileForm saveFileForm = new SaveFileForm(_controller, filename);
                    saveFileForm.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Handles click on the About card
        /// </summary>
        private void About_Click(object sender, EventArgs e)
        {
            // Create and show an about dialog with application information
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        /// <summary>
        /// Handles click on the Theme Settings card
        /// </summary>
        private void ThemeSettings_Click(object sender, EventArgs e)
        {
            // Create and show a theme settings dialog
            ThemeSettingsForm themeSettingsForm = new ThemeSettingsForm();
            themeSettingsForm.ShowDialog();
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

    /// <summary>
    /// Form for reading data from a file
    /// </summary>
    public class ReadFileForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private readonly string _filename;

        public ReadFileForm(GuiNeighborhoodController controller, string filename)
        {
            _controller = controller;
            _filename = filename;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Reading File";
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(15);

            // TODO: Implement file reading UI
            Label fileLabel = new Label
            {
                Text = $"Reading data from: {_filename}",
                Font = new Font("Segoe UI", 10),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 30
            };

            Button readButton = new Button
            {
                Text = "Read File",
                BackColor = UIHelper.PrimaryColor,
                ForeColor = UIHelper.TextColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 35),
                Location = new Point(130, 100),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            readButton.FlatAppearance.BorderSize = 0;
            readButton.Click += (sender, e) => {
                try {
                    Cursor.Current = Cursors.WaitCursor;
                    bool success = _controller.ReadFromFile(_filename);
                    Cursor.Current = Cursors.Default;
                    
                    if (success) {
                        MessageBox.Show($"Successfully loaded data from {_filename}.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    } else {
                        MessageBox.Show($"Failed to load data from {_filename}. The file may be corrupted or in an incorrect format.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch (Exception ex) {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show($"An error occurred: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } finally {
                    this.Close();
                }
            };

            this.Controls.Add(readButton);
            this.Controls.Add(fileLabel);
        }
    }

    /// <summary>
    /// Form for saving data to a file
    /// </summary>
    public class SaveFileForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private readonly string _filename;

        public SaveFileForm(GuiNeighborhoodController controller, string filename)
        {
            _controller = controller;
            _filename = filename;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Saving File";
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(15);

            // TODO: Implement file saving UI
            Label fileLabel = new Label
            {
                Text = $"Saving data to: {_filename}",
                Font = new Font("Segoe UI", 10),
                ForeColor = UIHelper.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 30
            };

            Button saveButton = new Button
            {
                Text = "Save File",
                BackColor = UIHelper.PrimaryColor,
                ForeColor = UIHelper.TextColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 35),
                Location = new Point(130, 100),
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += (sender, e) => {
                try {
                    Cursor.Current = Cursors.WaitCursor;
                    bool success = _controller.WriteToFile(_filename);
                    Cursor.Current = Cursors.Default;
                    
                    if (success) {
                        MessageBox.Show($"Successfully saved data to {_filename}.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    } else {
                        MessageBox.Show($"Failed to save data to {_filename}.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch (Exception ex) {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show($"An error occurred: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } finally {
                    this.Close();
                }
            };

            this.Controls.Add(saveButton);
            this.Controls.Add(fileLabel);
        }
    }

    /// <summary>
    /// Form for displaying application information
    /// </summary>
    public class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "About KhuPho Manager";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // App logo/icon
            PictureBox logoBox = new PictureBox
            {
                Size = new Size(64, 64),
                Location = new Point((this.ClientSize.Width - 64) / 2, 20),
                BackColor = Color.Transparent
            };
            // You would set the Image property if you have an app icon
            // logoBox.Image = Properties.Resources.AppIcon;
            logoBox.BackColor = UIHelper.PrimaryColor;

            // App title
            Label titleLabel = new Label
            {
                Text = "KhuPho Manager",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(360, 30),
                Location = new Point(20, 94)
            };

            // Version
            Label versionLabel = new Label
            {
                Text = "Version 1.0",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(360, 20),
                Location = new Point(20, 124)
            };

            // Description
            Label descriptionLabel = new Label
            {
                Text = "A neighborhood management application for managing households and residents.",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.TopCenter,
                Size = new Size(360, 40),
                Location = new Point(20, 154)
            };

            // Copyright
            Label copyrightLabel = new Label
            {
                Text = "© 2025 KhuPho Manager Team",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(360, 20),
                Location = new Point(20, 200)
            };

            // OK button
            Button okButton = new Button
            {
                Text = "OK",
                BackColor = UIHelper.PrimaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point((this.ClientSize.Width - 100) / 2, 230)
            };
            okButton.FlatAppearance.BorderSize = 0;
            okButton.Click += (sender, e) => this.Close();

            // Add controls
            this.Controls.Add(okButton);
            this.Controls.Add(copyrightLabel);
            this.Controls.Add(descriptionLabel);
            this.Controls.Add(versionLabel);
            this.Controls.Add(titleLabel);
            this.Controls.Add(logoBox);
        }
    }

    /// <summary>
    /// Form for theme settings
    /// </summary>
    public class ThemeSettingsForm : Form
    {
        private ComboBox themeComboBox;
        
        public ThemeSettingsForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Theme Settings";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UIHelper.PanelColor;
            this.Padding = new Padding(20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label titleLabel = new Label
            {
                Text = "Theme Settings",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UIHelper.PrimaryColor,
                Size = new Size(360, 30),
                Location = new Point(20, 20)
            };

            // Theme selection
            Label themeLabel = new Label
            {
                Text = "Select Theme:",
                Font = new Font("Segoe UI", 10),
                Size = new Size(100, 25),
                Location = new Point(20, 70)
            };

            themeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(240, 25),
                Location = new Point(120, 70)
            };
            themeComboBox.Items.AddRange(new string[] { "Default Blue", "Dark Mode", "Light Mode", "High Contrast" });
            themeComboBox.SelectedIndex = 0;

            // Preview panel
            Panel previewPanel = new Panel
            {
                Size = new Size(360, 100),
                Location = new Point(20, 110),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label previewLabel = new Label
            {
                Text = "Theme Preview",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(100, 20),
                Location = new Point(10, 10)
            };

            Button previewButton = new Button
            {
                Text = "Sample Button",
                Size = new Size(120, 30),
                Location = new Point(120, 40),
                FlatStyle = FlatStyle.Flat
            };
            previewButton.FlatAppearance.BorderSize = 0;

            previewPanel.Controls.Add(previewButton);
            previewPanel.Controls.Add(previewLabel);

            // Update preview when theme changes
            themeComboBox.SelectedIndexChanged += (sender, e) => {
                switch (themeComboBox.SelectedIndex)
                {
                    case 0: // Default Blue
                        previewPanel.BackColor = UIHelper.PanelColor;
                        previewLabel.ForeColor = UIHelper.PrimaryColor;
                        previewButton.BackColor = UIHelper.PrimaryColor;
                        previewButton.ForeColor = Color.White;
                        break;
                    case 1: // Dark Mode
                        previewPanel.BackColor = Color.FromArgb(50, 50, 50);
                        previewLabel.ForeColor = Color.White;
                        previewButton.BackColor = Color.FromArgb(0, 120, 215);
                        previewButton.ForeColor = Color.White;
                        break;
                    case 2: // Light Mode
                        previewPanel.BackColor = Color.White;
                        previewLabel.ForeColor = Color.Black;
                        previewButton.BackColor = Color.FromArgb(0, 120, 215);
                        previewButton.ForeColor = Color.White;
                        break;
                    case 3: // High Contrast
                        previewPanel.BackColor = Color.Black;
                        previewLabel.ForeColor = Color.Yellow;
                        previewButton.BackColor = Color.Yellow;
                        previewButton.ForeColor = Color.Black;
                        break;
                }
            };

            // Apply button
            Button applyButton = new Button
            {
                Text = "Apply Theme",
                BackColor = UIHelper.PrimaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Location = new Point(140, 220)
            };
            applyButton.FlatAppearance.BorderSize = 0;
            applyButton.Click += (sender, e) => {
                MessageBox.Show("Theme settings will be applied when you restart the application.", 
                    "Theme Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            };

            // Cancel button
            Button cancelButton = new Button
            {
                Text = "Cancel",
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 30),
                Location = new Point(280, 220)
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (sender, e) => this.Close();

            // Add controls
            this.Controls.Add(cancelButton);
            this.Controls.Add(applyButton);
            this.Controls.Add(previewPanel);
            this.Controls.Add(themeComboBox);
            this.Controls.Add(themeLabel);
            this.Controls.Add(titleLabel);
        }
    }
}
