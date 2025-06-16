using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void button1_Click(object sender, EventArgs e)
        {
            adminDashboard.LoadFormInAdminPanel(new ManageStudents(adminDashboard));
        }

        private void ManageUsers_Load(object sender, EventArgs e)
        {

        }

        
    }
}
