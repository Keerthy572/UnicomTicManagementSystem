using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Main;

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

        private void LecturersCourses_Load(object sender, EventArgs e)
        {
            LoadComboBoxes();
            LoadLecturerCourses();
        }

        private void LoadComboBoxes()
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

        private void LoadLecturerCourses()
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

        private void button1_Click(object sender, EventArgs e) // Add
        {
            if(comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1 )
            {
                MessageBox.Show("Select a lecturer and course to add ");
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
        }


        private void button2_Click(object sender, EventArgs e) // Update
        {
            if (selectedLecturerId == -1 || selectedCourseId == -1)
            {
                MessageBox.Show("Please select a row to update.");
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
        }

        private void button3_Click(object sender, EventArgs e) // Delete
        {
            if (selectedLecturerId == -1 || selectedCourseId == -1)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }

            lcController.DeleteLecturerCourse(selectedLecturerId, selectedCourseId);
            LoadLecturerCourses();
            selectedLecturerId = selectedCourseId = -1;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
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

        private void button4_Click(object sender, EventArgs e)
        {
            adminDashboard.LoadFormInAdminPanel(new ManageLecturers(adminDashboard));

        }
    }
}
