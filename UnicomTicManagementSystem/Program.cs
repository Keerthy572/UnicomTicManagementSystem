using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTicManagementSystem.Repositories;
using UnicomTicManagementSystem.View;

namespace UnicomTicManagementSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            DataBaseInitializer dataBaseInitializer = new DataBaseInitializer();
            dataBaseInitializer.InitializeTable();           
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Automatically register a default admin user if none exists
            DataBaseInitializer.AdminRegistration();

            // Start the application with the login form
            Application.Run(new LoginForm());
        }
    }
}
