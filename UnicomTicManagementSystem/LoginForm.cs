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
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;
using UnicomTicManagementSystem.View;

namespace UnicomTicManagementSystem
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public int num;

        // Handles the Login button click event
        // Validates entered credentials against the user list from database
        // Navigates to the appropriate dashboard if authenticated
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtusername.Text.Trim();
                string password = txtpassword.Text.Trim();

                // Input validation
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Please enter both username and password.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                User user1 = new User();
                user1.userName = username;
                user1.password = password;

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

                foreach (var item in userList)
                {
                    if (item.userName == username && item.password == password)
                    {
                        isAuthenticated = true;
                        Dashboard.userId = item.userId;
                        Dashboard.userType = item.userType;
                        break;
                    }
                }

                if (isAuthenticated)
                {
                    MessageBox.Show("Login successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();

                    // Open appropriate dashboard based on user type
                    Form dashboardForm = null;

                    if (Dashboard.userType == "admin")
                        dashboardForm = new AdminDashboard();
                    else if (Dashboard.userType == "student")
                        dashboardForm = new StudentDashboard();
                    else if (Dashboard.userType == "lecturer")
                        dashboardForm = new LecturerDashboard();
                    else if (Dashboard.userType == "staff")
                        dashboardForm = new StaffDashboard();

                    if (dashboardForm != null)
                    {
                        dashboardForm.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("User type is not recognized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Show(); // Stay on login form
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Login. Please check your credentials.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handles the Exit button click event
        // Closes the application
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while exiting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
                
    }
}
