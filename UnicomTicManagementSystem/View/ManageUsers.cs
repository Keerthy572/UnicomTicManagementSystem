using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTicManagementSystem.Views;

namespace UnicomTicManagementSystem.View
{
    public partial class ManageUsers : Form
    {
        private AdminDashboard adminDashboard;

        public ManageUsers(AdminDashboard dashboard)
        {
            InitializeComponent();
            adminDashboard = dashboard;
        }

        

        // Load ManageStudents form inside the Admin Dashboard panel
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {               
                adminDashboard.LoadFormInAdminPanel(new ManageStudents(adminDashboard));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while opening Manage Students: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load ManageLecturers form inside the Admin Dashboard panel
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {               
                adminDashboard.LoadFormInAdminPanel(new ManageLecturers(adminDashboard));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while opening Manage Lecturers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load ManageStaff form inside the Admin Dashboard panel
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {                
                adminDashboard.LoadFormInAdminPanel(new ManageStaff(adminDashboard));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while opening Manage Staff: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Safely close the ManageUsers form
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {                
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while closing the form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
