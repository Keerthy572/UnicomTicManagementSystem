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

namespace UnicomTicManagementSystem.View
{
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }

        internal void LoadFormInAdminPanel(Form childForm)
        {
            mainPanel.Controls.Clear();
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None; 
            childForm.Dock = DockStyle.Fill; 
            mainPanel.Controls.Add(childForm);
            childForm.Show(); 
        }

        private void mainPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageCourses(this));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageGroups());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageUsers(this));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageRoom());

        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageExams());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LoadFormInAdminPanel(new ManageMarks());

        }
    }
}
