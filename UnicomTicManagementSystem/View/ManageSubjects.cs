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


            RefreshDataGridView();
        }

        private void RefreshDataGridView()
        {
            SubjectController sub = new SubjectController();
            List<Subject> subjects = sub.GetAllSubjects();

            dataGridView1.DataSource = subjects;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["CourseName"].DisplayIndex = 0;
            dataGridView1.Columns["SubjectName"].DisplayIndex = 1;
            dataGridView1.Columns["CourseId"].Visible = false;
            dataGridView1.Columns["SubjectId"].Visible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ManageSubjects_Load(object sender, EventArgs e)
        {
            LoadCoursesIntoComboBox();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

        }
        private void LoadCoursesIntoComboBox()
        {
            CourseController courseController = new CourseController();
            List<Course> courses = courseController.GetCourses();

            comboBox1.DataSource = courses;
            comboBox1.DisplayMember = "courseName"; 
            comboBox1.ValueMember = "courseId";
            comboBox1.SelectedIndex = -1;
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
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select a course to add subject");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Type The subject to be added");
                return;
            }

            Course selectedCourse = (Course)comboBox1.SelectedItem;
            Subject subject = new Subject();
            subject.subjectName = textBox1.Text;

            SubjectController subjectController = new SubjectController();
            string message = subjectController.AddSubject(subject, selectedCourse);
            MessageBox.Show(message);
            comboBox1.SelectedIndex = -1;
            textBox1.Text = "";

            RefreshDataGridView();




        }
    }
}
