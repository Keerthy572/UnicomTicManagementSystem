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
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.View
{
    public partial class AdminRegisterForm : Form
    {
        public AdminRegisterForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("please fill all the fields correctly");
                return;
            }

            if (textBox2.Text != textBox3.Text)
            {
                MessageBox.Show("Re-entered password doesn't match password");
                return;
            }

            User user = new User();
            user.userName = textBox1.Text;
            user.password = textBox2.Text;

            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                string insertUser = "INSERT INTO User (UserName, Password, UserType) VALUES (@uname, @pwd, 'admin');";
                SQLiteCommand cmd = new SQLiteCommand(insertUser, con);
                cmd.Parameters.AddWithValue("@uname", user.userName);
                cmd.Parameters.AddWithValue("@pwd", user.password);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Admin registered Successfully");
            this.Close();
        }

        private void AdminRegisterForm_Load(object sender, EventArgs e)
        {

        }
    }
}
