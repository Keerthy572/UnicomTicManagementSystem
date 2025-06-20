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
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DataBaseInitializer dataBaseInitializer = new DataBaseInitializer();
            dataBaseInitializer.InitializeTable();           
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DataBaseInitializer.AdminRegistration();
            Application.Run(new Form1());
            //Application.Run(new AdminDashboard());
            //Application.Run(new LecturerDashboard());

        }
    }
}
