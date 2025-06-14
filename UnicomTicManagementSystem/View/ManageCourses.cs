using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.View;

namespace UnicomTicManagementSystem.View
{
    public partial class ManageCourses : Form
    {
        private AdminDashboard adminDashboard;

        public ManageCourses(AdminDashboard dashboard)
        {
            InitializeComponent();
            adminDashboard = dashboard;
            CourseController courseController = new CourseController();
            List<Course> courses = courseController.GetCourses();
            dataGridView1.DataSource = courses;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["CourseId"].Visible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            textBox1.Text = "";
        }

        private int updateCourseId;
        private string selectedCourse;


        //Delete Button Event
        private void button3_Click(object sender, EventArgs e)
        {
            if (updateCourseId == 0)
            {
                MessageBox.Show("Select a course to delete");
                return;
            }

            if (selectedCourse != textBox1.Text )
            {
                MessageBox.Show("Select a course correctly from table to delete ");
                return;
            }
            Course course = new Course();
            course.courseId = updateCourseId;

            CourseController courseController = new CourseController();
            string message = courseController.DeleteCourse(course);
            MessageBox.Show(message);

            List<Course> courses = courseController.GetCourses();
            dataGridView1.DataSource = courses;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["CourseId"].Visible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            textBox1.Text = "";
            updateCourseId = 0;
        }


        //Add Button Event
        private void button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter a course name");
                return;
            }
            Course course = new Course();
            course.courseName = textBox1.Text;

            CourseController courseController = new CourseController();
            string message = courseController.AddCourse(course);
            MessageBox.Show(message);

            List<Course> courses = courseController.GetCourses();
            dataGridView1.DataSource = courses;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["CourseId"].Visible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            textBox1.Text = "";

        }
        
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }
        

        //Update Button Event
        private void button2_Click(object sender, EventArgs e)
        {
            if (updateCourseId == 0)
            {
                MessageBox.Show("Select a course to update.");
                return;
            }

            Course course = new Course();
            course.courseId = updateCourseId;
            course.courseName = textBox1.Text;

            CourseController courseController = new CourseController();
            string message = courseController.UpdateCourse(course);
            MessageBox.Show(message);

            List<Course> courses = courseController.GetCourses();
            dataGridView1.DataSource = courses;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["CourseId"].Visible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            textBox1.Text = "";
            updateCourseId = 0;
        }


        
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var course = (Course)dataGridView1.CurrentRow.DataBoundItem;
            if (course != null)
            {
                updateCourseId = course.courseId;
                textBox1.Text = course.courseName;
                selectedCourse = course.courseName;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            adminDashboard.LoadFormInAdminPanel(new ManageSubjects(adminDashboard));
        }

        private void ManageCourses_Load(object sender, EventArgs e)
        {

        }
    }
}
