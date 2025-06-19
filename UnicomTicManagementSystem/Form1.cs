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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public int num;
        public string usertype;
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
                    usertype = item.userType;
                    Dashboard.userId = item.userId;
                }                                    
            }


            if (isAuthenticated)
            {
                MessageBox.Show("Login successful");
                if(usertype == "admin")
                {
                    this.Hide();
                    var form = new AdminDashboard();
                    form.ShowDialog();
                    this.Close();
                }
                else if (usertype == "student")
                {

                }
                else if (usertype == "lecturer")
                {

                }
                else if (usertype == "staff")
                {

                }

            }
            else
            {
                MessageBox.Show("Invalid Login");
            }
        }
    }
}
