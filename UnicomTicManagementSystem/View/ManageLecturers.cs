using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

// ... [Namespaces unchanged]
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
            try
            {
                LoadGroups();
                LoadLecturers();
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during form load: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load groups in Combobox
        private void LoadGroups()
        {
            try
            {
                comboBox1.DataSource = groupController.GetAllGroups();
                comboBox1.DisplayMember = "groupName";
                comboBox1.ValueMember = "groupId";
                comboBox1.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load groups: " + ex.Message);
            }
        }

        //Load lecturers in dataGridView
        private void LoadLecturers()
        {
            try
            {
                var lecturers = lecturerController.GetLecturers();
                dataGridView1.DataSource = lecturers;

                dataGridView1.Columns["password"].Visible = false;
                dataGridView1.Columns["courseId"].Visible = false;
                dataGridView1.Columns["courseName"].Visible = false;

                dataGridView1.ReadOnly = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading lecturers: " + ex.Message);
            }
        }

        //Clear Textboxes and comboboxes after add, update, delete
        private void ClearInputs()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            comboBox1.SelectedIndex = -1;
            selectedLecturer = null;
            originalLecturerSnapshot = null;
        }

        // Adding new lecturers
        private void button1_Click(object sender, EventArgs e) 
        {
            // Input validation
            if (comboBox1.Items.Count == 0)
            {
                MessageBox.Show("First create groups to add Lecturers");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("please fill all the fields correctly.");
                return;
            }
            if (!textBox1.Text.Trim().All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                MessageBox.Show("Lecturer name must contain only letters and spaces.Spaces before and after text you enter will be removed");
                return;
            }
            if (textBox2.Text.Trim().Length < 6)
            {
                MessageBox.Show("Username must be at least 6 characters long.Spaces before and after text you enter will be removed");
                return;
            }
            if (textBox3.Text.Trim().Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.Spaces before and after text you enter will be removed");
                return;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select a group."); return;
            }

            try
            {
                // Create and add lecturer
                var lecturer = new Lecturer
                {
                    lecturerName = textBox1.Text.Trim(),
                    userName = textBox2.Text.Trim(),
                    password = textBox3.Text.Trim(),
                    courseId = 0,
                    GroupId = Convert.ToInt32(comboBox1.SelectedValue)
                };

                lecturerController.AddLecturer(lecturer);
                LoadLecturers();
                MessageBox.Show("Lecturer added successfully.");
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding lecturer: " + ex.Message);
            }
        }

        // Load selected row data into form
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    selectedLecturer = new Lecturer
                    {
                        lecturerId = Convert.ToInt32(row.Cells["lecturerId"].Value),
                        lecturerName = row.Cells["lecturerName"].Value?.ToString(),
                        userName = row.Cells["userName"].Value?.ToString(),
                        password = row.Cells["password"].Value?.ToString(),
                        userId = Convert.ToInt32(row.Cells["userId"].Value),
                        GroupId = Convert.ToInt32(row.Cells["GroupId"].Value)
                    };

                    // Snapshot for delete validation
                    originalLecturerSnapshot = new Lecturer
                    {
                        lecturerId = selectedLecturer.lecturerId,
                        lecturerName = selectedLecturer.lecturerName,
                        userName = selectedLecturer.userName,
                        password = selectedLecturer.password,
                        userId = selectedLecturer.userId,
                        GroupId = selectedLecturer.GroupId
                    };

                    // Populate form fields
                    textBox1.Text = selectedLecturer.lecturerName;
                    textBox2.Text = selectedLecturer.userName;
                    textBox3.Text = selectedLecturer.password;
                    comboBox1.SelectedValue = selectedLecturer.GroupId;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading selection: " + ex.Message);
                }
            }
        }


        // Update an existing lecturer details
        private void button2_Click(object sender, EventArgs e) 
        {
            if (selectedLecturer == null)
            {
                MessageBox.Show("Select a lecturer first."); return;
            }

            // Validation
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("please fill all the fields correctly.");
                return;
            }
            if (!textBox1.Text.Trim().All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                MessageBox.Show("Lecturer name must contain only letters and spaces.Spaces before and after text you enter will be removed");
                return;
            }
            if (textBox2.Text.Trim().Length < 6)
            {
                MessageBox.Show("Username must be at least 6 characters long.Spaces before and after text you enter will be removed");
                return;
            }
            if (textBox3.Text.Trim().Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.Spaces before and after text you enter will be removed");
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
                MessageBox.Show("Lecturer updated.");
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message);
            }
        }

        // Delete an existing lecturer details
        private void button3_Click(object sender, EventArgs e) 
        {
            if (selectedLecturer == null || originalLecturerSnapshot == null)
            {
                MessageBox.Show("Please select a lecturer to delete."); return;
            }

            // Ensure user hasn't modified values before delete
            if (textBox1.Text.Trim() != originalLecturerSnapshot.lecturerName ||
                textBox2.Text.Trim() != originalLecturerSnapshot.userName ||
                textBox3.Text.Trim() != originalLecturerSnapshot.password ||
                Convert.ToInt32(comboBox1.SelectedValue) != originalLecturerSnapshot.GroupId)
            {
                MessageBox.Show("Do not change details before deletion."); return;
            }

            if (MessageBox.Show($"Delete lecturer '{selectedLecturer.lecturerName}'?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    lecturerController.DeleteLecturer(selectedLecturer.lecturerId, selectedLecturer.userId);
                    LoadLecturers();
                    MessageBox.Show("Deleted successfully.");
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Delete failed: " + ex.Message);
                }
            }
        }

        // Load LecturersCourses form in adminpanal to add courses to lecturer
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                adminDashboard.LoadFormInAdminPanel(new LecturersCourses(adminDashboard));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Navigation error: " + ex.Message);
            }
        }

        // Load ManageUsers form in adminpanal
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                adminDashboard.LoadFormInAdminPanel(new ManageUsers(adminDashboard));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Navigation error: " + ex.Message);
            }
        }
                
    }
}

