using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class StudentController
    {
        // collect all students from the database
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

            try
            {
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
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching students: " + ex.Message);
            }

            return studentList;
        }

        // Check if the password is unique (not already in use)
        public bool IsPasswordUnique(string password)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Error while checking password uniqueness: " + ex.Message);
            }
        }

        // Add a new student to the database
        public void AddStudent(Student student)
        {
            if (!IsPasswordUnique(student.password))
                throw new Exception("Password already exists!"); // Validate password uniqueness

            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    SQLiteTransaction transaction = con.BeginTransaction();

                    // Insert User record
                    string insertUser = "INSERT INTO User (UserName, Password, UserType) VALUES (@UserName, @Password, 'student');";
                    SQLiteCommand cmdUser = new SQLiteCommand(insertUser, con);
                    cmdUser.Parameters.AddWithValue("@UserName", student.userName);
                    cmdUser.Parameters.AddWithValue("@Password", student.password);
                    cmdUser.ExecuteNonQuery();

                    // Get the last inserted UserId
                    long userId = con.LastInsertRowId;

                    // Insert Student record
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
            catch (Exception ex)
            {
                throw new Exception("Error while adding student: " + ex.Message);
            }
        }

        // Update an existing student record
        public void UpdateStudent(Student student)
        {
            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    SQLiteTransaction transaction = con.BeginTransaction();

                    // Update User record
                    string updateUser = "UPDATE User SET UserName = @UserName, Password = @Password WHERE UserId = @UserId;";
                    SQLiteCommand cmdUser = new SQLiteCommand(updateUser, con);
                    cmdUser.Parameters.AddWithValue("@UserName", student.userName);
                    cmdUser.Parameters.AddWithValue("@Password", student.password);
                    cmdUser.Parameters.AddWithValue("@UserId", student.userId);
                    cmdUser.ExecuteNonQuery();

                    // Update Student record
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
            catch (Exception ex)
            {
                throw new Exception("Error while updating student: " + ex.Message);
            }
        }

        // Delete a student record from the database
        public void DeleteStudent(int studentId, int userId)
        {
            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    SQLiteTransaction transaction = con.BeginTransaction();

                    // Delete Student record
                    SQLiteCommand cmdStudent = new SQLiteCommand("DELETE FROM Student WHERE StudentId = @StudentId", con);
                    cmdStudent.Parameters.AddWithValue("@StudentId", studentId);
                    cmdStudent.ExecuteNonQuery();

                    // Delete User record
                    SQLiteCommand cmdUser = new SQLiteCommand("DELETE FROM User WHERE UserId = @UserId", con);
                    cmdUser.Parameters.AddWithValue("@UserId", userId);
                    cmdUser.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while deleting student: " + ex.Message);
            }
        }

        // Get the list of all available courses
        public List<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();
            string query = "SELECT CourseId, CourseName FROM Course";

            try
            {
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
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching courses: " + ex.Message);
            }
            return courses;
        }

        // Get the list all available groups
        public List<Group> GetGroups()
        {
            List<Group> groups = new List<Group>();
            string query = "SELECT GroupId, GroupName FROM Groups";

            try
            {
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
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching groups: " + ex.Message);
            }

            return groups;
        }

        // Retrieves student profile info for current user
        public List<Student> GetStudentsForGrid()
        {
            List<Student> st = new List<Student>();

            try
            {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load student info: " + ex.Message);
            }

            return st;
        }

        // Retrieves exam marks for current student
        public List<ExamMark> GetExamMarks()
        {
            List<ExamMark> examMarks = new List<ExamMark>();

            try
            {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to retrieve exam marks: " + ex.Message);
            }

            return examMarks;
        }
    }
}