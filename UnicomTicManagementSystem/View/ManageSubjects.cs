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

namespace UnicomTicManagementSystem.View
{
    public partial class ManageSubjects : Form
    {
        private AdminDashboard adminDashboard;
        public ManageSubjects(AdminDashboard dashboard)
        {
            InitializeComponent();
            adminDashboard = dashboard;
            this.Load += ManageSubjects_Load;


            RefreshSubjectGrid();
        }

        private int selectedSubjectId = 0;
        private string selectedSubjectName;

        private void dataGridViewSubjects_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }


        private void RefreshSubjectGrid()
        {
            SubjectController sub = new SubjectController();
            List<Subject> subjects = sub.GetAllSubjects();

            dataGridViewSubjects.DataSource = subjects;
            dataGridViewSubjects.ReadOnly = true;
            dataGridViewSubjects.Columns["CourseName"].DisplayIndex = 0;
            dataGridViewSubjects.Columns["SubjectName"].DisplayIndex = 1;
            dataGridViewSubjects.Columns["CourseId"].Visible = false;
            dataGridViewSubjects.Columns["SubjectId"].Visible = false;
            dataGridViewSubjects.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ManageSubjects_Load(object sender, EventArgs e)
        {
            LoadCoursesIntoComboBox();
            comboBoxCourses.DropDownStyle = ComboBoxStyle.DropDownList;

        }
        private void LoadCoursesIntoComboBox()
        {
            CourseController courseController = new CourseController();
            List<Course> courses = courseController.GetCourses();

            comboBoxCourses.DataSource = courses;
            comboBoxCourses.DisplayMember = "courseName"; 
            comboBoxCourses.ValueMember = "courseId";
            comboBoxCourses.SelectedIndex = -1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            adminDashboard.LoadFormInAdminPanel(new ManageCourses(adminDashboard));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBoxCourses.SelectedIndex == -1)
            {
                MessageBox.Show("Select a course to add subject");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxSubjectName.Text))
            {
                MessageBox.Show("Type The subject to be added");
                return;
            }

            Course selectedCourse = (Course)comboBoxCourses.SelectedItem;
            Subject subject = new Subject();
            subject.subjectName = textBoxSubjectName.Text;

            SubjectController subjectController = new SubjectController();
            string message = subjectController.AddSubject(subject, selectedCourse);
            MessageBox.Show(message);
            comboBoxCourses.SelectedIndex = -1;
            textBoxSubjectName.Text = "";

            RefreshSubjectGrid();




        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedSubjectId == 0)
            {
                MessageBox.Show("Please select a subject to update.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxSubjectName.Text) || comboBoxCourses.SelectedValue == null)
            {
                MessageBox.Show("Please enter subject name and select a course.");
                return;
            }

            Subject subject = new Subject
            {
                subjectId = selectedSubjectId,
                subjectName = textBoxSubjectName.Text,
                courseId = Convert.ToInt32(comboBoxCourses.SelectedValue)
            };

            SubjectController subjectController = new SubjectController();
            string message = subjectController.UpdateSubject(subject);
            MessageBox.Show(message);

            RefreshSubjectGrid();
            ClearFormInputs();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (selectedSubjectId == 0)
            {
                MessageBox.Show("Please select a subject from table to delete.");
                return;
            }

            if (selectedSubjectName != textBoxSubjectName.Text)
            {
                MessageBox.Show("Please select a subject correctly from the table to delete.");
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this subject?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                SubjectController subjectController = new SubjectController();
                string message = subjectController.DeleteSubject(selectedSubjectId);
                MessageBox.Show(message);

                RefreshSubjectGrid();
                ClearFormInputs();
            }
        }

        
        private void ClearFormInputs()
        {
            textBoxSubjectName.Clear();
            comboBoxCourses.SelectedIndex = -1;
            selectedSubjectId = 0;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridViewSubjects_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewSubjects_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var subject = (Subject)dataGridViewSubjects.CurrentRow.DataBoundItem;
                if (subject != null)
                {
                    selectedSubjectId = subject.subjectId;
                    selectedSubjectName = subject.subjectName;
                    textBoxSubjectName.Text = subject.subjectName;
                    comboBoxCourses.SelectedValue = subject.courseId;
                }
            }
        }
    }
}
