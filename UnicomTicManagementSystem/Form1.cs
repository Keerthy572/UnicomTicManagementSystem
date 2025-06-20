using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;
using UnicomTicManagementSystem.View;

namespace UnicomTicManagementSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
                
        public int num;
        private void button1_Click(object sender, EventArgs e)
        {
            User user1 = new User();
            user1.userName = txtusername.Text;
            user1.password = txtpassword.Text;

            UserController userController = new UserController();
            var userList = userController.LoginCheck();

            if (userList.Count == 0)
            {
                MessageBox.Show("No users found in the system.");
                txtusername.Text = "";
                txtpassword.Text = "";
                return;
            }

            bool isAuthenticated = false;

            foreach ( var item in userList )
            {
                if (item.userName == txtusername.Text &&  item.password == txtpassword.Text)
                {
                    isAuthenticated = true;
                    Dashboard.userId = item.userId;
                    Dashboard.userType = item.userType;
                }                                    
            }


            if (isAuthenticated)
            {
                MessageBox.Show("Login successful");
                if(Dashboard.userType == "admin")
                {
                    this.Hide();
                    var form = new AdminDashboard();
                    form.ShowDialog();
                    this.Close();
                }
                else if (Dashboard.userType == "student")
                {
                    this.Hide();
                    var form = new StudentDashboard();
                    form.ShowDialog();
                    this.Close();
                }
                else if (Dashboard.userType == "lecturer")
                {
                    this.Hide();
                    var form = new LecturerDashboard();
                    form.ShowDialog();
                    this.Close();
                }
                else if (Dashboard.userType == "staff")
                {
                    this.Hide();
                    var form = new StaffDashboard();
                    form.ShowDialog();
                    this.Close();
                }

            }
            else
            {
                MessageBox.Show("Invalid Login");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
