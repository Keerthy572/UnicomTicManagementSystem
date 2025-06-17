using System;
using System.Collections.Generic;
using System.Data.SQLite;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class ExamController
    {
        public List<Exam> GetAllExams()
        {
            List<Exam> exams = new List<Exam>();

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
            return exams;
        }

        public List<(int courseId, string courseName)> GetAllCourses()
        {
            var courses = new List<(int, string)>();
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
            return courses;
        }

        public List<(int subjectId, string subjectName)> GetSubjectsByCourse(int courseId)
        {
            var subjects = new List<(int, string)>();
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
            return subjects;
        }

        public void AddExam(string examName, int subjectId)
        {
            if (string.IsNullOrWhiteSpace(examName)) throw new Exception("Exam name cannot be empty.");

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

        public void UpdateExam(int examId, string examName, int subjectId)
        {
            if (examId <= 0) throw new Exception("Invalid exam selected.");
            if (string.IsNullOrWhiteSpace(examName)) throw new Exception("Exam name cannot be empty.");

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

        public void DeleteExam(int examId)
        {
            if (examId <= 0) throw new Exception("Invalid exam selected.");

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

        public bool ExamExists(string examName, int subjectId, int? excludeExamId = null)
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


    }
}
