using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.View
{
    public partial class StudentDashboard : Form
    {
        private List<Student> st;
        private List<Subject> subjects;
        private List<ExamMark> examMark;
        private List<Timetable> timetables;
        public StudentDashboard()
        {
            InitializeComponent();
            st = GetStudentsForGrid();

            SubjectController sub = new SubjectController();
            subjects = sub.GetAllSubjects();

            examMark = GetExamMarks();

            TimetableController controller = new TimetableController();
            timetables = controller.GetAllTimetables();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = st;
            dataGridView1.Columns["userId"].Visible = false;
            dataGridView1.Columns["courseId"].Visible = false;
            dataGridView1.Columns["GroupId"].Visible = false;
            dataGridView1.Columns["usertype"].Visible = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }
       

        private void button2_Click(object sender, EventArgs e)
        {
            
            dataGridView1.DataSource = subjects;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["CourseName"].DisplayIndex = 0;
            dataGridView1.Columns["SubjectName"].DisplayIndex = 1;
            dataGridView1.Columns["CourseId"].Visible = false;
            dataGridView1.Columns["SubjectId"].Visible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void button3_Click(object sender, EventArgs e)
        {         
                dataGridView1.DataSource = examMark;
                dataGridView1.ReadOnly = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
        }


        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = timetables;
            dataGridView1.Columns["TimetableId"].HeaderText = "Timetable Id"; // hide internal ID
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


        private List<Student> GetStudentsForGrid()
        {
            List<Student> st = new List<Student>();
            StudentController studentController = new StudentController();
            List<Student> studentList = studentController.GetStudents();

            foreach (Student student in studentList)
            {
                if (student.userId == Dashboard.userId)
                {
                    st.Add(new Student
                    {
                        studentId = student.studentId,
                        studentName = student.studentName,
                        userName = student.userName,
                        password = student.password,
                        courseName = student.courseName,
                        groupName = student.groupName,
                    });
                }

            }

            return st;
        }


        private List<ExamMark> GetExamMarks()
        {
            List<ExamMark> examMarks = new List<ExamMark>();

            using (var con = DataBaseCon.Connection())
            {
                string query = @"
            SELECT e.ExamName, s.SubjectName, m.Score
            FROM Marks m
            JOIN Exam e ON m.ExamId = e.ExamId
            JOIN Subjects s ON e.SubjectId = s.SubjectId
            JOIN Student st ON m.StudentId = st.StudentId
            WHERE st.UserId = @userId;
        ";

                using (var cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@userId", Dashboard.userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string score = reader["Score"] == DBNull.Value ? "Absent" : reader["Score"].ToString();

                            examMarks.Add(new ExamMark
                            {
                                ExamName = reader["ExamName"].ToString(),
                                SubjectName = reader["SubjectName"].ToString(),
                                Score = score
                            });
                        }
                    }
                }
            }

            return examMarks;
        }

        
    }
}
