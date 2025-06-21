using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTicManagementSystem.Forms;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.View
{
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }

        // Loads a child form into the main panel of the admin dashboard
        internal void LoadFormInAdminPanel(Form childForm)
        {
            try
            {
                mainPanel.Controls.Clear();
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;
                mainPanel.Controls.Add(childForm);
                childForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Opens ManageCourses form in the admin panel
        private void button1_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageCourses(this));
        }

        // Opens ManageGroups form in the admin panel
        private void button2_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageGroups());
        }

        // Opens ManageUsers form in the admin panel
        private void button3_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageUsers(this));
        }

        // Opens ManageRoom form in the admin panel
        private void button4_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageRoom());
        }

        // Opens ManageExams form in the admin panel
        private void button5_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageExams());
        }

        // Opens ManageMarks form in the admin panel
        private void button6_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageMarks());
        }

        // Opens ManageTimeSlot form in the admin panel
        private void button7_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageTimeSlot());
        }

        // Opens CreateTimetable form in the admin panel
        private void button8_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new CreateTimetable());
        }

        // Logs out the current user and returns to login form
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                var form = new LoginForm();
                form.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while logging out: " + ex.Message, "Logout Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
