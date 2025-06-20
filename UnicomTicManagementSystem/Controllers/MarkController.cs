using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class MarkController
    {
        
        // Retrieves the list of exams.
        // Lecturers see only their related exams, others see all exams.     
        public List<Exam> GetExams()
        {
            List<Exam> exams = new List<Exam>();

            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = "";

                    if (Dashboard.userType == "lecturer")
                    {
                        // Restrict to exams related to lecturer's courses
                        query = @"
                            SELECT e.ExamId, e.ExamName, s.SubjectName, s.CourseId, c.CourseName
                            FROM Exam e
                            JOIN Subjects s ON e.SubjectId = s.SubjectId
                            JOIN Course c ON s.CourseId = c.CourseId
                            JOIN LecturerCourse lc ON c.CourseId = lc.CourseId
                            JOIN Lecturer l ON lc.LecturerId = l.LecturerId
                            WHERE l.UserId = @userId";
                    }
                    else
                    {
                        // Admins or other users get all exams
                        query = @"
                            SELECT e.ExamId, e.ExamName, s.SubjectName, s.CourseId, c.CourseName
                            FROM Exam e
                            JOIN Subjects s ON e.SubjectId = s.SubjectId
                            JOIN Course c ON s.CourseId = c.CourseId";
                    }

                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        if (Dashboard.userType == "lecturer")
                            cmd.Parameters.AddWithValue("@userId", Dashboard.userId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                exams.Add(new Exam
                                {
                                    examId = Convert.ToInt32(reader["ExamId"]),
                                    examName = reader["ExamName"].ToString(),
                                    subjectName = reader["SubjectName"].ToString(),
                                    courseId = Convert.ToInt32(reader["CourseId"]),
                                    courseName = reader["CourseName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to retrieve exams: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return exams;
        }

        
        // Retrieves a DataTable of students with their marks for a given exam.
        // Scores are cast to TEXT to handle null (Absent) values gracefully.       
        public DataTable GetStudentsWithMarks(int examId)
        {
            DataTable table = new DataTable();

            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = @"
                        SELECT s.StudentId, s.StudentName,
                               CAST((SELECT Score FROM Marks m WHERE m.StudentId = s.StudentId AND m.ExamId = @examId) AS TEXT) AS Score
                        FROM Student s
                        WHERE s.CourseId = (
                            SELECT c.CourseId
                            FROM Exam e
                            JOIN Subjects sub ON e.SubjectId = sub.SubjectId
                            JOIN Course c ON sub.CourseId = c.CourseId
                            WHERE e.ExamId = @examId
                        )";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@examId", examId);
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to retrieve student marks: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return table;
        }

        
        // Adds or updates marks for the specified exam based on the data in the DataGridView.
        // Validates all input before making any database changes.        
        public void AddOrUpdateMarks(int examId, DataGridView dgv)
        {
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    // Validate input first
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;

                        var scoreCell = row.Cells["Score"].Value;

                        if (scoreCell == null || string.IsNullOrWhiteSpace(scoreCell.ToString()))
                        {
                            MessageBox.Show("All score fields must be filled.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string scoreText = scoreCell.ToString().Trim();

                        if (!string.Equals(scoreText, "Absent", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!int.TryParse(scoreText, out int parsedScore) || parsedScore < 1 || parsedScore > 100)
                            {
                                MessageBox.Show($"Invalid score '{scoreText}' for a student.\nOnly 1–100 or 'Absent' allowed.", "Invalid Score", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }

                    // After validation, proceed to insert/update marks
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;

                        int studentId = Convert.ToInt32(row.Cells["StudentId"].Value);
                        var scoreCell = row.Cells["Score"].Value;
                        string scoreText = scoreCell.ToString().Trim();

                        int? score = null;
                        if (!string.Equals(scoreText, "Absent", StringComparison.OrdinalIgnoreCase))
                        {
                            score = int.Parse(scoreText);
                        }

                        string checkQuery = "SELECT COUNT(*) FROM Marks WHERE ExamId = @examId AND StudentId = @studentId";
                        using (var checkCmd = new SQLiteCommand(checkQuery, con))
                        {
                            checkCmd.Parameters.AddWithValue("@examId", examId);
                            checkCmd.Parameters.AddWithValue("@studentId", studentId);
                            long exists = (long)checkCmd.ExecuteScalar();

                            if (exists == 0)
                            {
                                string insertQuery = "INSERT INTO Marks (StudentId, ExamId, Score) VALUES (@studentId, @examId, @score)";
                                using (var insertCmd = new SQLiteCommand(insertQuery, con))
                                {
                                    insertCmd.Parameters.AddWithValue("@studentId", studentId);
                                    insertCmd.Parameters.AddWithValue("@examId", examId);
                                    insertCmd.Parameters.AddWithValue("@score", score.HasValue ? (object)score.Value : DBNull.Value);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                string updateQuery = "UPDATE Marks SET Score = @score WHERE StudentId = @studentId AND ExamId = @examId";
                                using (var updateCmd = new SQLiteCommand(updateQuery, con))
                                {
                                    updateCmd.Parameters.AddWithValue("@score", score.HasValue ? (object)score.Value : DBNull.Value);
                                    updateCmd.Parameters.AddWithValue("@studentId", studentId);
                                    updateCmd.Parameters.AddWithValue("@examId", examId);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("Marks saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save marks: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
        // Deletes all marks for the given exam.       
        public void DeleteMarks(int examId)
        {
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string deleteQuery = "DELETE FROM Marks WHERE ExamId = @examId";
                    using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@examId", examId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete marks: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
