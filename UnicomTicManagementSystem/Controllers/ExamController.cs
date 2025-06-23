using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class ExamController
    {

        // Retrieves all exams with subject and course information.
        public List<Exam> GetAllExams()
        {
            List<Exam> exams = new List<Exam>();

            try
            {
                using (var conn = DataBaseCon.Connection())
                {
                    string query = @"
                        SELECT e.ExamId, e.ExamName, 
                               s.SubjectId, s.SubjectName, 
                               c.CourseId, c.CourseName
                        FROM Exam e
                        INNER JOIN Subjects s ON e.SubjectId = s.SubjectId
                        INNER JOIN Course c ON s.CourseId = c.CourseId;";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                exams.Add(new Exam
                                {
                                    examId = Convert.ToInt32(reader["ExamId"]),
                                    examName = Convert.ToString(reader["ExamName"]),
                                    subjectId = Convert.ToInt32(reader["SubjectId"]),
                                    subjectName = Convert.ToString(reader["SubjectName"]),
                                    courseId = Convert.ToInt32(reader["CourseId"]),
                                    courseName = Convert.ToString(reader["CourseName"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to retrieve exams. " + ex.Message);
            }

            return exams;
        }

        // Retrieves all courses as a list of tuples.
        public List<(int courseId, string courseName)> GetAllCourses()
        {
            var courses = new List<(int, string)>();

            try
            {
                using (var conn = DataBaseCon.Connection())
                {
                    string query = "SELECT CourseId, CourseName FROM Course;";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                courses.Add((Convert.ToInt32(reader["CourseId"]), Convert.ToString(reader["CourseName"])));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load courses. " + ex.Message);
            }

            return courses;
        }

        // Retrieves all subjects for a given course.
        public List<(int subjectId, string subjectName)> GetSubjectsByCourse(int courseId)
        {
            var subjects = new List<(int, string)>();

            try
            {
                using (var conn = DataBaseCon.Connection())
                {
                    string query = "SELECT SubjectId, SubjectName FROM Subjects WHERE CourseId = @courseId;";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@courseId", courseId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                subjects.Add((Convert.ToInt32(reader["SubjectId"]), Convert.ToString(reader["SubjectName"])));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load subjects for the course. " + ex.Message);
            }

            return subjects;
        }

        // Adds a new exam.
        public void AddExam(string examName, int subjectId)
        {
            if (string.IsNullOrWhiteSpace(examName))
            {
                MessageBox.Show("Exam name cannot be empty.");
                return;
            }
                

            try
            {
                using (var conn = DataBaseCon.Connection())
                {
                    string query = "INSERT INTO Exam (ExamName, SubjectId) VALUES (@examName, @subjectId);";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@examName", examName);
                        cmd.Parameters.AddWithValue("@subjectId", subjectId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add exam. " + ex.Message);
            }
        }

        // Updates an existing exam.
        public void UpdateExam(int examId, string examName, int subjectId)
        {
            if (examId <= 0)
            {
                MessageBox.Show("Invalid exam selected.");
                return;
            }
                
            if (string.IsNullOrWhiteSpace(examName))
            {
                MessageBox.Show("Exam name cannot be empty.");
                return;
            }
                

            try
            {
                using (var conn = DataBaseCon.Connection())
                {
                    string query = "UPDATE Exam SET ExamName = @examName, SubjectId = @subjectId WHERE ExamId = @examId;";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@examName", examName);
                        cmd.Parameters.AddWithValue("@subjectId", subjectId);
                        cmd.Parameters.AddWithValue("@examId", examId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update exam. " + ex.Message);
            }
        }

        // Deletes an exam.
        public void DeleteExam(int examId)
        {
            if (examId <= 0)
            {
                MessageBox.Show("Invalid exam selected.");
                return;
            }
                

            try
            {
                using (var conn = DataBaseCon.Connection())
                {
                    string query = "DELETE FROM Exam WHERE ExamId = @examId;";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@examId", examId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete exam. " + ex.Message);
            }
        }

        // Checks whether an exam with the same name already exists for a given subject.
        public bool ExamExists(string examName, int subjectId, int? excludeExamId = null)
        {
            try
            {
                using (var conn = DataBaseCon.Connection())
                {
                    string query = "SELECT COUNT(*) FROM Exam WHERE ExamName = @examName AND SubjectId = @subjectId";
                    if (excludeExamId.HasValue)
                        query += " AND ExamId != @excludeExamId";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@examName", examName);
                        cmd.Parameters.AddWithValue("@subjectId", subjectId);
                        if (excludeExamId.HasValue)
                            cmd.Parameters.AddWithValue("@excludeExamId", excludeExamId.Value);

                        long count = (long)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to check if exam exists. " + ex.Message);
                return false;
            }
        }
    }
}
