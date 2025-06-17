using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.View
{
    public partial class ManageLecturers : Form
    {
        LecturerController lecturerController = new LecturerController();
        GroupController groupController = new GroupController();
        Lecturer selectedLecturer = null;
        private Lecturer originalLecturerSnapshot = null;

        private AdminDashboard adminDashboard;

        public ManageLecturers(AdminDashboard dashboard)
        {
            InitializeComponent();
            adminDashboard = dashboard;
        }

        private void ManageLecturers_Load(object sender, EventArgs e)
        {
            LoadGroups();
            LoadLecturers();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LoadGroups()
        {
            GroupController groupController = new GroupController();
            comboBox1.DataSource = groupController.GetAllGroups();
            comboBox1.DisplayMember = "groupName";
            comboBox1.ValueMember = "groupId";
            comboBox1.SelectedIndex = -1;
        }

        private void LoadLecturers()
        {
            dataGridView1.DataSource = lecturerController.GetLecturers();
            dataGridView1.Columns["password"].Visible = false;
            dataGridView1.Columns["courseId"].Visible = false;            
            dataGridView1.Columns["courseName"].Visible = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void ClearInputs()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            comboBox1.SelectedIndex = -1;
            selectedLecturer = null;
        }

        private void button1_Click(object sender, EventArgs e) // Add Lecturer
        {           
            // Basic input validation
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Enter Lecturer Name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Enter Username.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Enter Password.");
                return;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select a group to add lecturer.");
                return;
            }

            try
            {
                // Check if password already exists in User table
                string passwordToCheck = textBox3.Text.Trim();
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    string checkQuery = "SELECT COUNT(*) FROM User WHERE Password = @password";
                    SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@password", passwordToCheck);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("Password already exists. Please choose a different one.");
                        return;
                    }
                }

                Lecturer lecturer = new Lecturer
                {
                    lecturerName = textBox1.Text.Trim(),
                    userName = textBox2.Text.Trim(),
                    password = textBox3.Text.Trim(),
                    courseId = 0, // No course is selected in this form
                    GroupId = Convert.ToInt32(comboBox1.SelectedValue)
                };

                lecturerController.AddLecturer(lecturer);
                LoadLecturers(); // Refresh DataGridView
                MessageBox.Show("Lecturer added successfully.");
                ClearInputs(); // Reset inputs
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedLecturer = new Lecturer
                {
                    lecturerId = Convert.ToInt32(row.Cells["lecturerId"].Value),
                    lecturerName = row.Cells["lecturerName"].Value.ToString(),
                    userName = row.Cells["userName"].Value.ToString(),
                    password = row.Cells["password"].Value.ToString(),
                    userId = Convert.ToInt32(row.Cells["userId"].Value),
                    GroupId = Convert.ToInt32(row.Cells["GroupId"].Value)
                };

                // Also save a snapshot for comparison on delete
                originalLecturerSnapshot = new Lecturer
                {
                    lecturerId = selectedLecturer.lecturerId,
                    lecturerName = selectedLecturer.lecturerName,
                    userName = selectedLecturer.userName,
                    password = selectedLecturer.password,
                    userId = selectedLecturer.userId,
                    GroupId = selectedLecturer.GroupId
                };

                textBox1.Text = selectedLecturer.lecturerName;
                textBox2.Text = selectedLecturer.userName;
                textBox3.Text = selectedLecturer.password;
                comboBox1.SelectedValue = selectedLecturer.GroupId;
            }
        }


        private void button2_Click(object sender, EventArgs e) // Update
        {
            if (selectedLecturer == null)
            {
                MessageBox.Show("Select a lecturer to update.");
                return;
            }

            // Input validation
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Enter Lecturer Name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Enter Username.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Enter Password.");
                return;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select a group to update lecturer.");
                return;
            }

            try
            {
                selectedLecturer.lecturerName = textBox1.Text.Trim();
                selectedLecturer.userName = textBox2.Text.Trim();
                selectedLecturer.password = textBox3.Text.Trim();
                selectedLecturer.GroupId = Convert.ToInt32(comboBox1.SelectedValue);

                lecturerController.UpdateLecturer(selectedLecturer);
                LoadLecturers();
                MessageBox.Show("Lecturer updated successfully.");
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void button3_Click(object sender, EventArgs e) // Delete
        {
            if (selectedLecturer == null || originalLecturerSnapshot == null)
            {
                MessageBox.Show("Please select a lecturer to delete.");
                return;
            }

            // Check if inputs are unchanged from the original selection
            if (textBox1.Text.Trim() != originalLecturerSnapshot.lecturerName
                || textBox2.Text.Trim() != originalLecturerSnapshot.userName
                || textBox3.Text.Trim() != originalLecturerSnapshot.password
                || (comboBox1.SelectedValue == null || Convert.ToInt32(comboBox1.SelectedValue) != originalLecturerSnapshot.GroupId))
            {
                MessageBox.Show("Input fields have been changed since selection. Please do not modify details before deleting.");
                return;
            }

            DialogResult confirmResult = MessageBox.Show(
                $"Are you sure you want to delete lecturer '{selectedLecturer.lecturerName}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.No)
            {
                return;
            }

            try
            {
                lecturerController.DeleteLecturer(selectedLecturer.lecturerId, selectedLecturer.userId);
                LoadLecturers();
                MessageBox.Show("Lecturer deleted successfully.");
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            adminDashboard.LoadFormInAdminPanel(new LecturersCourses(adminDashboard));
        }
    }
}
