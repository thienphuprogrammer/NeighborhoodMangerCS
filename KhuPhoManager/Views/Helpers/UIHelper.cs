using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KhuPhoManager.Views.Helpers
{
    /// <summary>
    /// Helper class for creating UI elements with consistent styling
    /// </summary>
    public static class UIHelper
    {
        // Common UI Colors
        public static readonly Color PrimaryColor = Color.FromArgb(24, 90, 157);       // Dark blue
        public static readonly Color SecondaryColor = Color.FromArgb(17, 53, 71);      // Even darker blue
        public static readonly Color HighlightColor = Color.FromArgb(86, 204, 242);    // Light blue
        public static readonly Color TextColor = Color.FromArgb(238, 238, 238);        // Off-white
        public static readonly Color PanelColor = Color.FromArgb(247, 250, 252);       // Very light blue-gray
        public static readonly Color AccentColor = Color.FromArgb(255, 74, 130);       // Pink accent

        /// <summary>
        /// Creates an action card for the UI
        /// </summary>
        public static Panel CreateActionCard(string title, string description, string icon, EventHandler clickHandler)
        {
            // Create card panel
            Panel card = new Panel
            {
                Width = 350,
                Height = 250,
                BackColor = Color.White,
                Margin = new Padding(20),
                Padding = new Padding(15),
                Cursor = Cursors.Hand
            };

            // Add shadow and rounded corners
            card.Paint += (sender, e) => {
                Graphics g = e.Graphics;
                Rectangle rect = new Rectangle(0, 0, card.Width, card.Height);

                // Draw shadow
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                {
                    g.FillRectangle(shadowBrush, new Rectangle(5, 5, rect.Width, rect.Height));
                }

                // Draw rounded rectangle
                using (GraphicsPath path = new GraphicsPath())
                {
                    int radius = 10;
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(rect.Width - radius, rect.Y, radius, radius, 270, 90);
                    path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
                    path.AddArc(rect.X, rect.Height - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();

                    using (SolidBrush cardBrush = new SolidBrush(Color.White))
                    {
                        g.FillPath(cardBrush, path);
                    }

                    using (Pen borderPen = new Pen(HighlightColor, 1))
                    {
                        g.DrawPath(borderPen, path);
                    }
                }
            };

            // Icon label
            Label iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24),
                ForeColor = PrimaryColor,
                AutoSize = true,
                Location = new Point(15, 15),
            };

            // Title label
            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                AutoSize = true,
                Location = new Point(15, 70),
            };

            // Description label
            Label descriptionLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Size = new Size(200, 50),
                Location = new Point(15, 90)
            };

            // Add controls to card
            card.Controls.Add(iconLabel);
            card.Controls.Add(titleLabel);
            card.Controls.Add(descriptionLabel);

            // Add click event
            card.Click += clickHandler;

            return card;
        }

        /// <summary>
        /// Creates a navigation button for the sidebar
        /// </summary>
        public static Button CreateNavButton(string text, string icon, int position, Panel sidePanel)
        {
            Button button = new Button
            {
                Text = "  " + text,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(sidePanel.Width, 50),
                Location = new Point(0, 100 + (position * 55)),
                BackColor = SecondaryColor,
                ForeColor = TextColor,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular),
                Cursor = Cursors.Hand,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding = new Padding(15, 0, 0, 0)
            };

            // Remove border
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 100, 167);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(20, 80, 140);

            // Add icon if available
            if (!string.IsNullOrEmpty(icon))
            {
                button.Text = icon + "   " + text;
            }

            // Add hover effect with transition
            button.MouseEnter += (sender, e) => {
                if (button.BackColor != PrimaryColor)
                    button.BackColor = Color.FromArgb(30, 100, 167);
            };

            button.MouseLeave += (sender, e) => {
                if (button.BackColor != PrimaryColor)
                    button.BackColor = SecondaryColor;
            };

            return button;
        }

        /// <summary>
        /// Creates a statistic card for the dashboard
        /// </summary>
        public static void CreateStatCard(Panel parent, string title, string value, Color color)
        {
            Panel statCard = new Panel
            {
                Width = 150,
                Height = 100,
                Margin = new Padding(10),
                BackColor = Color.White
            };

            // Add shadow and rounded corners
            statCard.Paint += (sender, e) => {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Create rounded rectangle path
                GraphicsPath path = new GraphicsPath();
                int radius = 12; // Slightly larger corner radius
                Rectangle rect = new Rectangle(0, 0, statCard.Width - 1, statCard.Height - 1);

                // Add arcs for rounded corners
                path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);                  // Top-left
                path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);  // Top-right
                path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);  // Bottom-right
                path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);  // Bottom-left
                path.CloseAllFigures();

                // Draw shadow with slightly larger offset for more depth
                using (GraphicsPath shadowPath = (GraphicsPath)path.Clone())
                {
                    using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                    {
                        g.TranslateTransform(4, 4);
                        g.FillPath(shadowBrush, shadowPath);
                        g.ResetTransform();
                    }
                }

                // Fill card background
                g.FillPath(new SolidBrush(Color.White), path);

                // Draw accent line on left side with gradient effect
                using (LinearGradientBrush accentBrush = new LinearGradientBrush(
                    new Rectangle(0, 0, 6, statCard.Height),
                    PrimaryColor,
                    Color.FromArgb(86, 204, 242), // Highlight color
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(accentBrush, new Rectangle(0, 0, 6, statCard.Height));
                }

                // Add subtle border
                using (Pen borderPen = new Pen(Color.FromArgb(15, 0, 0, 0), 1))
                {
                    g.DrawPath(borderPen, path);
                }
            };

            // Title label
            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(statCard.Width, 20),
                Location = new Point(0, 70)
            };

            // Value label
            Label valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = color,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(statCard.Width, 50),
                Location = new Point(0, 20)
            };

            // Add controls to stat card
            statCard.Controls.Add(valueLabel);
            statCard.Controls.Add(titleLabel);

            // Add stat card to parent
            parent.Controls.Add(statCard);
        }
    }
}
