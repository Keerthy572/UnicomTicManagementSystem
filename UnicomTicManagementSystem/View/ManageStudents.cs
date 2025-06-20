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
            LoadCourses();
            LoadGroups();
            LoadStudents();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;

        }

        private void LoadCourses()
        {
            StudentController controller = new StudentController();
            List<Course> courses = controller.GetCourses();

            comboBox1.DataSource = courses;
            comboBox1.DisplayMember = "courseName";
            comboBox1.ValueMember = "courseId";
        }

        private void LoadGroups()
        {
            StudentController controller = new StudentController();
            List<Group> groups = controller.GetGroups();

            comboBox2.DataSource = groups;
            comboBox2.DisplayMember = "groupName";
            comboBox2.ValueMember = "groupId";
        }

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


        private void LoadStudents()
        {
            dataGridView1.DataSource = studentController.GetStudents();
            dataGridView1.ReadOnly = true;
            
            dataGridView1.Columns["userId"].Visible = false;
            dataGridView1.Columns["courseId"].Visible = false;
            dataGridView1.Columns["GroupId"].Visible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void button1_Click(object sender, EventArgs e) // Add
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text) )
            {
                MessageBox.Show("please fill all the fields correctly");
                return;
            }
            if (comboBox1.SelectedIndex == -1 ||  comboBox2.SelectedIndex == -1)
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

        private void button2_Click(object sender, EventArgs e) // Update
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

        private void button3_Click(object sender, EventArgs e) // Delete
        {
            if (selectedStudentName != textBox1.Text)
            {
                MessageBox.Show("Select a student from table correctly to delete");
                return;
            }
            if (selectedUserName != textBox2.Text)
            {
                MessageBox.Show("Select a username from table correctly to delete");
            }
            if (selectedPassword != textBox3.Text)
            {
                MessageBox.Show("Select a password from table correctly to delete");
                return;
            }
            //if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
            //{
            //    MessageBox.Show("Select course and group to add student");
            //    return;
            //}
            if (selectedStudentId > 0 && selectedUserId > 0)
            {
                studentController.DeleteStudent(selectedStudentId, selectedUserId);
                LoadStudents();
                MessageBox.Show("Student deleted.");
                ClearInputs();
            }
            //else
            //{
            //    MessageBox.Show("Select a student from table correctly to delete");
            //    return;
            //}
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
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

        private void ManageUsers_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            adminDashboard.LoadFormInAdminPanel(new ManageUsers(adminDashboard));
        }
    }
}
