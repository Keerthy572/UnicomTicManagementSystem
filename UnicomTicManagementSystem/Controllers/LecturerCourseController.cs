using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class LecturerCourseController
    {
        // Add a new lecturer-course assignment if it doesn't already exist
        public void AddLecturerCourse(LecturerCourse lc)
        {
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    // Check if the lecturer is already assigned to the course
                    string checkQuery = "SELECT COUNT(*) FROM LecturerCourse WHERE LecturerId = @lid AND CourseId = @cid;";
                    SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@lid", lc.LecturerId);
                    checkCmd.Parameters.AddWithValue("@cid", lc.CourseId);

                    long exists = (long)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        MessageBox.Show("This Lecturer is already assigned to this Course.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Insert the lecturer-course assignment
                    string insertQuery = "INSERT INTO LecturerCourse (LecturerId, CourseId) VALUES (@lid, @cid);";
                    SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, con);
                    insertCmd.Parameters.AddWithValue("@lid", lc.LecturerId);
                    insertCmd.Parameters.AddWithValue("@cid", lc.CourseId);
                    insertCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add lecturer-course assignment.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Update an existing lecturer-course assignment
        public void UpdateLecturerCourse(int oldLecturerId, int oldCourseId, LecturerCourse updated)
        {
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = @"UPDATE LecturerCourse 
                                     SET LecturerId = @newLid, CourseId = @newCid 
                                     WHERE LecturerId = @oldLid AND CourseId = @oldCid;";
                    SQLiteCommand cmd = new SQLiteCommand(query, con);
                    cmd.Parameters.AddWithValue("@newLid", updated.LecturerId);
                    cmd.Parameters.AddWithValue("@newCid", updated.CourseId);
                    cmd.Parameters.AddWithValue("@oldLid", oldLecturerId);
                    cmd.Parameters.AddWithValue("@oldCid", oldCourseId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update lecturer-course assignment.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Delete a lecturer-course assignment
        public void DeleteLecturerCourse(int lecturerId, int courseId)
        {
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = "DELETE FROM LecturerCourse WHERE LecturerId = @lid AND CourseId = @cid;";
                    SQLiteCommand cmd = new SQLiteCommand(query, con);
                    cmd.Parameters.AddWithValue("@lid", lecturerId);
                    cmd.Parameters.AddWithValue("@cid", courseId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete lecturer-course assignment.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Get all lecturer-course assignments with names
        public List<LecturerCourse> GetLecturerCourses()
        {
            List<LecturerCourse> list = new List<LecturerCourse>();

            try
            {
                string query = @"SELECT lc.LecturerId, l.LecturerName, lc.CourseId, c.CourseName
                                 FROM LecturerCourse lc
                                 JOIN Lecturer l ON lc.LecturerId = l.LecturerId
                                 JOIN Course c ON lc.CourseId = c.CourseId
                                 ORDER BY l.LecturerName, c.CourseName;";

                using (var con = DataBaseCon.Connection())
                {
                    SQLiteCommand cmd = new SQLiteCommand(query, con);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        list.Add(new LecturerCourse
                        {
                            LecturerId = reader.GetInt32(0),
                            LecturerName = reader.GetString(1),
                            CourseId = reader.GetInt32(2),
                            CourseName = reader.GetString(3)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load lecturer-course assignments.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return list;
        }
    }
}
