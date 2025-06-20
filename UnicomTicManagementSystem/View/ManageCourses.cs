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
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.View;

namespace UnicomTicManagementSystem.View
{
    public partial class ManageCourses : Form
    {
        private AdminDashboard adminDashboard;
        private int updateCourseId;        // Holds the selected course ID 
        private string selectedCourse;     // Holds selected course name 

        public ManageCourses(AdminDashboard dashboard)
        {
            InitializeComponent();
            adminDashboard = dashboard;

            try
            {
                LoadCoursesToGrid();
                textBox1.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading courses: " + ex.Message);
            }
        }

        // Loads courses from the database to the DataGridView
        private void LoadCoursesToGrid()
        {
            try
            {
                CourseController courseController = new CourseController();
                List<Course> courses = courseController.GetCourses();
                dataGridView1.DataSource = courses;
                dataGridView1.ReadOnly = true;
                dataGridView1.Columns["CourseId"].Visible = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load courses: " + ex.Message);
            }
        }

        // Add Button Click: Add new course
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Please enter a course name");
                    return;
                }

                Course course = new Course { courseName = textBox1.Text };
                CourseController courseController = new CourseController();
                string message = courseController.AddCourse(course);
                MessageBox.Show(message);

                LoadCoursesToGrid();
                textBox1.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding course: " + ex.Message);
            }
        }

        // Update Button Click: Update selected course
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (updateCourseId == 0)
                {
                    MessageBox.Show("Select a course to update.");
                    return;
                }

                Course course = new Course
                {
                    courseId = updateCourseId,
                    courseName = textBox1.Text
                };

                CourseController courseController = new CourseController();
                string message = courseController.UpdateCourse(course);
                MessageBox.Show(message);

                LoadCoursesToGrid();
                textBox1.Text = "";
                updateCourseId = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating course: " + ex.Message);
            }
        }

        // Delete Button Click: Delete selected course
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (updateCourseId == 0)
                {
                    MessageBox.Show("Select a course to delete");
                    return;
                }

                if (selectedCourse != textBox1.Text)
                {
                    MessageBox.Show("Select a course correctly from table to delete ");
                    return;
                }

                Course course = new Course { courseId = updateCourseId };
                CourseController courseController = new CourseController();
                string message = courseController.DeleteCourse(course);
                MessageBox.Show(message);

                LoadCoursesToGrid();
                textBox1.Text = "";
                updateCourseId = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting course: " + ex.Message);
            }
        }

        // DataGridView Cell Click: Load selected course into textbox
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var course = (Course)dataGridView1.CurrentRow?.DataBoundItem;
                if (course != null)
                {
                    updateCourseId = course.courseId;
                    textBox1.Text = course.courseName;
                    selectedCourse = course.courseName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error selecting course: " + ex.Message);
            }
        }

        // Button to go to Manage Subjects page
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                adminDashboard.LoadFormInAdminPanel(new ManageSubjects(adminDashboard));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load subjects form: " + ex.Message);
            }
        }

        // Close the form
        private void button5_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}