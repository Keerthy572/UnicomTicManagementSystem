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
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.View
{
    public partial class ManageStudents : Form
    {
        private AdminDashboard adminDashboard;
        StudentController studentController = new StudentController();
        int selectedStudentId = -1;
        int selectedUserId = -1;
        string selectedStudentName;
        string selectedUserName;
        string selectedPassword;

        public ManageStudents(AdminDashboard dashboard)
        {
            InitializeComponent();
            adminDashboard = dashboard;

            try
            {
                LoadCourses();
                LoadGroups();
                LoadStudents();

                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Initialization error: " + ex.Message);
            }
        }

        // Load list of courses into comboBox1
        private void LoadCourses()
        {
            try
            {
                StudentController controller = new StudentController();
                List<Course> courses = controller.GetCourses();

                comboBox1.DataSource = courses;
                comboBox1.DisplayMember = "courseName";
                comboBox1.ValueMember = "courseId";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load courses: " + ex.Message);
            }
        }

        // Load list of groups into comboBox2
        private void LoadGroups()
        {
            try
            {
                StudentController controller = new StudentController();
                List<Group> groups = controller.GetGroups();

                comboBox2.DataSource = groups;
                comboBox2.DisplayMember = "groupName";
                comboBox2.ValueMember = "groupId";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load groups: " + ex.Message);
            }
        }

        // Clear input fields and selection tracking variables
        private void ClearInputs()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            selectedStudentId = -1;
            selectedUserId = -1;
        }

        // Load students into DataGridView
        private void LoadStudents()
        {
            try
            {
                dataGridView1.DataSource = studentController.GetStudents();
                dataGridView1.ReadOnly = true;
                dataGridView1.Columns["userId"].Visible = false;
                dataGridView1.Columns["courseId"].Visible = false;
                dataGridView1.Columns["GroupId"].Visible = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load students: " + ex.Message);
            }
        }

        // Add new student
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("please fill all the fields correctly");
                return;
            }
            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Select course and group to add student");
                return;
            }
            try
            {
                Student student = new Student
                {
                    studentName = textBox1.Text,
                    userName = textBox2.Text,
                    password = textBox3.Text,
                    courseId = Convert.ToInt32(comboBox1.SelectedValue),
                    GroupId = Convert.ToInt32(comboBox2.SelectedValue)
                };

                studentController.AddStudent(student);
                LoadStudents();
                MessageBox.Show("Student added.");
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Update selected student
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("please fill all the fields correctly");
                return;
            }

            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Select course and group to update a student");
                return;
            }
            try
            {
                Student student = new Student
                {
                    studentId = selectedStudentId,
                    userId = selectedUserId,
                    studentName = textBox1.Text,
                    userName = textBox2.Text,
                    password = textBox3.Text,
                    courseId = Convert.ToInt32(comboBox1.SelectedValue),
                    GroupId = Convert.ToInt32(comboBox2.SelectedValue)
                };

                studentController.UpdateStudent(student);
                LoadStudents();
                MessageBox.Show("Student updated.");
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Delete selected student
        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedStudentName != textBox1.Text || selectedUserName != textBox2.Text || selectedPassword != textBox3.Text)
            {
                MessageBox.Show("Select the correct student from the table to delete");
                return;
            }

            try
            {
                if (selectedStudentId > 0 && selectedUserId > 0)
                {
                    studentController.DeleteStudent(selectedStudentId, selectedUserId);
                    LoadStudents();
                    MessageBox.Show("Student deleted.");
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting student: " + ex.Message);
            }
        }

        // Handle student row click in DataGridView
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    selectedStudentId = Convert.ToInt32(row.Cells["studentId"].Value);
                    selectedUserId = Convert.ToInt32(row.Cells["userId"].Value);

                    textBox1.Text = row.Cells["studentName"].Value.ToString();
                    selectedStudentName = row.Cells["studentName"].Value.ToString();
                    textBox2.Text = row.Cells["userName"].Value.ToString();
                    selectedUserName = row.Cells["userName"].Value.ToString();
                    textBox3.Text = row.Cells["password"].Value.ToString();
                    selectedPassword = row.Cells["password"].Value.ToString();
                    comboBox1.SelectedValue = Convert.ToInt32(row.Cells["courseId"].Value);
                    comboBox2.SelectedValue = Convert.ToInt32(row.Cells["GroupId"].Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading selected student: " + ex.Message);
            }
        }

        // Navigation to ManageUsers
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                adminDashboard.LoadFormInAdminPanel(new ManageUsers(adminDashboard));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open ManageUsers form: " + ex.Message);
            }
        }
    }
}
