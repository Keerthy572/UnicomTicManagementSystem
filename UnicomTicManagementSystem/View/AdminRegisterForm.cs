using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.View
{
    public partial class AdminRegisterForm : Form
    {
        public AdminRegisterForm()
        {
            InitializeComponent();
        }

        // Handles the Register button click to validate input and insert admin user into database
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if any field is empty
                if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                    string.IsNullOrWhiteSpace(textBox2.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Please fill all the fields correctly");
                    return;
                }

                if (textBox1.Text.Trim().Length < 6)
                {
                    MessageBox.Show("Username must be at least 6 characters long.Spaces before and after text you enter will be removed");
                    return;
                }
                if (textBox2.Text.Trim().Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 characters long.Spaces before and after text you enter will be removed");
                    return;
                }


                // Check if password and confirm password match
                if (textBox2.Text != textBox3.Text)
                {
                    MessageBox.Show("Re-entered password doesn't match the password");
                    return;
                }

                User user = new User();
                user.userName = textBox1.Text.Trim();
                user.password = textBox2.Text.Trim();

                // Insert admin user into the User table
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    string insertUser = "INSERT INTO User (UserName, Password, UserType) VALUES (@uname, @pwd, 'admin');";
                    using (SQLiteCommand cmd = new SQLiteCommand(insertUser, con))
                    {
                        cmd.Parameters.AddWithValue("@uname", user.userName);
                        cmd.Parameters.AddWithValue("@pwd", user.password);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Admin registered successfully");
                this.Hide();
                var form = new LoginForm();
                form.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while registering admin: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        // Handles the Cancel/Exit button to close the application
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
