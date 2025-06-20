using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.View
{
    public partial class LecturersCourses : Form
    {
        private LecturerCourseController lcController = new LecturerCourseController();
        private LecturerController lecturerController = new LecturerController();
        private CourseController courseController = new CourseController();

        private int selectedLecturerId = -1;
        private int selectedCourseId = -1;
        private AdminDashboard adminDashboard;

        public LecturersCourses(AdminDashboard dashboard)
        {
            InitializeComponent();
            adminDashboard = dashboard;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // Form load event: populate combo boxes and grid
        private void LecturersCourses_Load(object sender, EventArgs e)
        {
            try
            {
                LoadComboBoxes();
                LoadLecturerCourses();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Loads lecturers and courses into combo boxes
        private void LoadComboBoxes()
        {
            try
            {
                var lecturers = lecturerController.GetLecturers();
                comboBox1.DataSource = lecturers;
                comboBox1.DisplayMember = "LecturerName";
                comboBox1.ValueMember = "LecturerId";
                comboBox1.SelectedIndex = -1;

                var courses = courseController.GetCourses();
                comboBox2.DataSource = courses;
                comboBox2.DisplayMember = "CourseName";
                comboBox2.ValueMember = "CourseId";
                comboBox2.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading combo boxes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Loads the lecturer-course mappings into the DataGridView
        private void LoadLecturerCourses()
        {
            try
            {
                var lcList = lcController.GetLecturerCourses();
                dataGridView1.DataSource = lcList;
                dataGridView1.Columns["LecturerId"].Visible = false;
                dataGridView1.Columns["CourseId"].Visible = false;
                dataGridView1.ReadOnly = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading lecturer-course data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Adds a new lecturer-course mapping
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
                {
                    MessageBox.Show("Select a lecturer and course to add.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int selectedLecturerId = (int)comboBox1.SelectedValue;
                int selectedCourseId = (int)comboBox2.SelectedValue;

                var lc = new LecturerCourse
                {
                    LecturerId = selectedLecturerId,
                    CourseId = selectedCourseId
                };

                lcController.AddLecturerCourse(lc);
                LoadLecturerCourses();
                MessageBox.Show("Lecturer-course assigned successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding lecturer-course: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Updates an existing lecturer-course mapping
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedLecturerId == -1 || selectedCourseId == -1)
                {
                    MessageBox.Show("Please select a row to update.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var updated = new LecturerCourse
                {
                    LecturerId = (int)comboBox1.SelectedValue,
                    CourseId = (int)comboBox2.SelectedValue
                };

                lcController.UpdateLecturerCourse(selectedLecturerId, selectedCourseId, updated);
                LoadLecturerCourses();
                selectedLecturerId = selectedCourseId = -1;

                MessageBox.Show("Lecturer-course updated successfully.", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating lecturer-course: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Deletes a selected lecturer-course mapping
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedLecturerId == -1 || selectedCourseId == -1)
                {
                    MessageBox.Show("Please select a row to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lcController.DeleteLecturerCourse(selectedLecturerId, selectedCourseId);
                LoadLecturerCourses();
                selectedLecturerId = selectedCourseId = -1;

                MessageBox.Show("Lecturer-course removed successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting lecturer-course: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handles grid row selection to enable update/delete
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    var row = dataGridView1.Rows[e.RowIndex];
                    selectedLecturerId = Convert.ToInt32(row.Cells["LecturerId"].Value);
                    selectedCourseId = Convert.ToInt32(row.Cells["CourseId"].Value);

                    comboBox1.SelectedValue = selectedLecturerId;
                    comboBox2.SelectedValue = selectedCourseId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting row: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Navigates back to ManageLecturers form
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                adminDashboard.LoadFormInAdminPanel(new ManageLecturers(adminDashboard));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
