using System;
using System.Windows.Forms;
using KhuPhoManager.Controllers;
using KhuPhoManager.Views.Forms;

namespace KhuPhoManager
{
    /// <summary>
    /// Main program class for the KhuPho Manager application
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Configure application exception handling
            Application.ThreadException += (sender, e) =>
            {
                MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}", 
                    "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            
            // Set up global exception handling
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                MessageBox.Show($"A fatal error occurred: {(e.ExceptionObject as Exception)?.Message ?? "Unknown error"}", 
                    "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            
            try
            {
                // Create GUI controller and launch the main form
                GuiNeighborhoodController controller = new GuiNeighborhoodController();
                Application.Run(new MainForm(controller));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting application: {ex.Message}", 
                    "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
