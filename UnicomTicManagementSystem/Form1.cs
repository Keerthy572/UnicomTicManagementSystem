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
        private void button1_Click(object sender, EventArgs e)
        {
            User user1 = new User();
            user1.userName = txtusername.Text;
            user1.password = txtpassword.Text;

            UserController userController = new UserController();
            var userList = userController.LoginCheck();

            foreach ( var item in userList )
            {
                if (item.userName == txtusername.Text &&  item.password == txtpassword.Text)
                {
                    MessageBox.Show("Login Success");
                }
                else
                {
                    num = 1;
                }
               
               
            }
            if (num ==1)
            {
                MessageBox.Show("Invalid Login");
            }
        }
    }
}
