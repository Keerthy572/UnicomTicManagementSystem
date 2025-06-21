using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.View
{
    public partial class LecturerDashboard : Form
    {
        private List<Subject> subjects;
        private List<Timetable> timetables;

        // Constructor: Initializes components and loads subjects and timetables
        public LecturerDashboard()
        {
            InitializeComponent();

            try
            {
                SubjectController sub = new SubjectController();
                subjects = sub.GetAllSubjects();

                TimetableController controller = new TimetableController();
                timetables = controller.GetAllTimetables();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load data: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        // Button1 click: Displays subjects in the DataGridView
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                panel3.Controls.Clear();
                panel3.Controls.Add(dataGridView1);

                dataGridView1.DataSource = subjects;
                dataGridView1.ReadOnly = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dataGridView1.Columns["CourseName"].DisplayIndex = 0;
                dataGridView1.Columns["SubjectName"].DisplayIndex = 1;
                dataGridView1.Columns["CourseId"].Visible = false;
                dataGridView1.Columns["SubjectId"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying subjects: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Button2 click: Displays timetables in the DataGridView
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                panel3.Controls.Clear();
                panel3.Controls.Add(dataGridView1);

                dataGridView1.DataSource = timetables;
                dataGridView1.ReadOnly = true;

                dataGridView1.Columns["TimetableId"].HeaderText = "Timetable Id";
                dataGridView1.Columns["Date"].HeaderText = "Date";
                dataGridView1.Columns["TimeSlot"].HeaderText = "Time Slot";
                dataGridView1.Columns["GroupName"].HeaderText = "Group";
                dataGridView1.Columns["LecturerName"].HeaderText = "Lecturer";
                dataGridView1.Columns["SubjectName"].HeaderText = "Subject";
                dataGridView1.Columns["RoomName"].HeaderText = "Room";

                dataGridView1.Columns["TimetableId"].DisplayIndex = 0;
                dataGridView1.Columns["Date"].DisplayIndex = 1;
                dataGridView1.Columns["TimeSlot"].DisplayIndex = 2;
                dataGridView1.Columns["GroupName"].DisplayIndex = 3;
                dataGridView1.Columns["LecturerName"].DisplayIndex = 4;
                dataGridView1.Columns["SubjectName"].DisplayIndex = 5;
                dataGridView1.Columns["RoomName"].DisplayIndex = 6;

                dataGridView1.Columns["SubjectId"].Visible = false;
                dataGridView1.Columns["RoomId"].Visible = false;
                dataGridView1.Columns["TimeSlotId"].Visible = false;
                dataGridView1.Columns["StartTime"].Visible = false;
                dataGridView1.Columns["EndTime"].Visible = false;
                dataGridView1.Columns["GroupId"].Visible = false;
                dataGridView1.Columns["LecturerId"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying timetable: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Button3 click: Opens ManageMarks form in panel3
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                ManageMarks manageMarks = new ManageMarks();
                panel3.Controls.Clear();
                manageMarks.TopLevel = false;
                manageMarks.FormBorderStyle = FormBorderStyle.None;
                manageMarks.Dock = DockStyle.Fill;
                panel3.Controls.Add(manageMarks);
                manageMarks.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading marks module: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Button4 click: Logs out and returns to login screen
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                var form = new LoginForm();
                form.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Logout failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
