using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class StudentController
    {
        public List<Student> GetStudents()
        {
            List<Student> studentList = new List<Student>();
            string query = @"SELECT s.StudentId, s.StudentName, s.CourseId, s.GroupId,
                            u.UserId, u.UserName, u.Password, u.UserType,
                            c.CourseName, g.GroupName
                     FROM Student s
                     JOIN User u ON s.UserId = u.UserId
                     JOIN Course c ON s.CourseId = c.CourseId
                     JOIN Groups g ON s.GroupId = g.GroupId
                     WHERE u.UserType = 'student';";

            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    studentList.Add(new Student
                    {
                        studentId = reader.GetInt32(0),
                        studentName = reader.GetString(1),
                        courseId = reader.GetInt32(2),
                        GroupId = reader.GetInt32(3),
                        userId = reader.GetInt32(4),
                        userName = reader.GetString(5),
                        password = reader.GetString(6),
                        userType = reader.GetString(7),
                        courseName = reader.GetString(8),
                        groupName = reader.GetString(9)
                    });
                }
            }

            return studentList;
        }

        

        public bool IsPasswordUnique(string password)
        {
            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                string query = "SELECT COUNT(*) FROM User WHERE Password = @Password";
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@Password", password);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count == 0;
            }
        }

        public void AddStudent(Student student)
        {
            if (!IsPasswordUnique(student.password))
                throw new Exception("Password already exists!");

            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                SQLiteTransaction transaction = con.BeginTransaction();

                string insertUser = "INSERT INTO User (UserName, Password, UserType) VALUES (@UserName, @Password, 'student');";
                SQLiteCommand cmdUser = new SQLiteCommand(insertUser, con);
                cmdUser.Parameters.AddWithValue("@UserName", student.userName);
                cmdUser.Parameters.AddWithValue("@Password", student.password);
                cmdUser.ExecuteNonQuery();

                long userId = con.LastInsertRowId;

                string insertStudent = "INSERT INTO Student (StudentName, UserId, CourseId, GroupId) VALUES (@StudentName, @UserId, @CourseId, @GroupId);";
                SQLiteCommand cmdStudent = new SQLiteCommand(insertStudent, con);
                cmdStudent.Parameters.AddWithValue("@StudentName", student.studentName);
                cmdStudent.Parameters.AddWithValue("@UserId", userId);
                cmdStudent.Parameters.AddWithValue("@CourseId", student.courseId);
                cmdStudent.Parameters.AddWithValue("@GroupId", student.GroupId);
                cmdStudent.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        public void UpdateStudent(Student student)
        {
            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                SQLiteTransaction transaction = con.BeginTransaction();

                string updateUser = "UPDATE User SET UserName = @UserName, Password = @Password WHERE UserId = @UserId;";
                SQLiteCommand cmdUser = new SQLiteCommand(updateUser, con);
                cmdUser.Parameters.AddWithValue("@UserName", student.userName);
                cmdUser.Parameters.AddWithValue("@Password", student.password);
                cmdUser.Parameters.AddWithValue("@UserId", student.userId);
                cmdUser.ExecuteNonQuery();

                string updateStudent = "UPDATE Student SET StudentName = @StudentName, CourseId = @CourseId, GroupId = @GroupId WHERE StudentId = @StudentId;";
                SQLiteCommand cmdStudent = new SQLiteCommand(updateStudent, con);
                cmdStudent.Parameters.AddWithValue("@StudentName", student.studentName);
                cmdStudent.Parameters.AddWithValue("@CourseId", student.courseId);
                cmdStudent.Parameters.AddWithValue("@GroupId", student.GroupId);
                cmdStudent.Parameters.AddWithValue("@StudentId", student.studentId);
                cmdStudent.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        public void DeleteStudent(int studentId, int userId)
        {
            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                SQLiteTransaction transaction = con.BeginTransaction();

                SQLiteCommand cmdStudent = new SQLiteCommand("DELETE FROM Student WHERE StudentId = @StudentId", con);
                cmdStudent.Parameters.AddWithValue("@StudentId", studentId);
                cmdStudent.ExecuteNonQuery();

                SQLiteCommand cmdUser = new SQLiteCommand("DELETE FROM User WHERE UserId = @UserId", con);
                cmdUser.Parameters.AddWithValue("@UserId", userId);
                cmdUser.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        public List<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();
            string query = "SELECT CourseId, CourseName FROM Course";

            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    courses.Add(new Course
                    {
                        courseId = reader.GetInt32(0),
                        courseName = reader.GetString(1)
                    });
                }
            }
            return courses;
        }

        public List<Group> GetGroups()
        {
            List<Group> groups = new List<Group>();
            string query = "SELECT GroupId, GroupName FROM Groups";

            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    groups.Add(new Group
                    {
                        groupId = reader.GetInt32(0),
                        groupName = reader.GetString(1)
                    });
                }
            }
            return groups;
        }

    }
}
