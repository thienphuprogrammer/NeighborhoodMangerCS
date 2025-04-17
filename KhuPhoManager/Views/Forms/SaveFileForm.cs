using System;
using System.Drawing;
using System.Windows.Forms;
using KhuPhoManager.Controllers;

namespace KhuPhoManager.Views.Forms
{
    public class SaveFileForm : Form
    {
        private readonly GuiNeighborhoodController _controller;
        private readonly string _filename;
        private Label statusLabel;
        private ProgressBar progressBar;
        private Button closeButton;

        public SaveFileForm(GuiNeighborhoodController controller, string filename)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _filename = filename ?? throw new ArgumentNullException(nameof(filename));
            InitializeComponent();
            SaveFile();
        }

        private void InitializeComponent()
        {
            this.Text = "Saving Data File";
            this.Size = new Size(450, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;

            // Create file info label
            Label fileInfoLabel = new Label
            {
                Text = $"File: {_filename}",
                Location = new Point(20, 20),
                Size = new Size(410, 20),
                AutoEllipsis = true
            };

            // Create status label
            statusLabel = new Label
            {
                Text = "Saving file...",
                Location = new Point(20, 50),
                Size = new Size(410, 20)
            };

            // Create progress bar
            progressBar = new ProgressBar
            {
                Location = new Point(20, 80),
                Size = new Size(410, 25),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30
            };

            // Create close button
            closeButton = new Button
            {
                Text = "Close",
                Location = new Point(175, 120),
                Width = 100,
                Height = 30,
                Enabled = false
            };
            closeButton.Click += (sender, e) => this.Close();

            // Add controls to form
            this.Controls.Add(fileInfoLabel);
            this.Controls.Add(statusLabel);
            this.Controls.Add(progressBar);
            this.Controls.Add(closeButton);
        }

        private async void SaveFile()
        {
            try
            {
                // Use Task.Run to execute the file saving operation asynchronously
                bool success = await System.Threading.Tasks.Task.Run(() => _controller.WriteToFile(_filename));
                
                if (success)
                {
                    progressBar.Style = ProgressBarStyle.Continuous;
                    progressBar.Value = 100;
                    statusLabel.Text = "File saved successfully!";
                    statusLabel.ForeColor = Color.Green;
                }
                else
                {
                    progressBar.Style = ProgressBarStyle.Continuous;
                    progressBar.Value = 0;
                    statusLabel.Text = "Failed to save file. Check permissions and try again.";
                    statusLabel.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = 0;
                statusLabel.Text = $"Error: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
            }
            finally
            {
                closeButton.Enabled = true;
            }
        }
    }
}
