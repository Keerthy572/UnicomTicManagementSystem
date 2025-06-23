using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class AttendanceController
    {
        // Fetch students of the group and their attendance (if already marked) for the given timetable
        public List<(int StudentId, string StudentName, string Status)> GetStudentsWithAttendance(int groupId, int timetableId)
        {
            var list = new List<(int, string, string)>();

            try
            {
                using (var conn = DataBaseCon.Connection())
                {
                    string query = @"
                    SELECT s.StudentId, s.StudentName,
                           IFNULL(a.Status, '') as Status
                    FROM Student s
                    LEFT JOIN Attendance a ON s.StudentId = a.StudentId AND a.TimetableId = @TimetableId
                    WHERE s.GroupId = @GroupId;";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupId", groupId);
                        cmd.Parameters.AddWithValue("@TimetableId", timetableId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add((
                                    Convert.ToInt32(reader["StudentId"]),
                                    reader["StudentName"].ToString(),
                                    reader["Status"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show("Something went wrong: " + ex.Message, "Error");
            }
            

            return list;
        }

        // Add or update attendance for each student
        public void AddOrUpdateAttendance(List<Attendance> records)
        {
            try
            {
                using (var conn = DataBaseCon.Connection())
                {
                    foreach (var att in records)
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Attendance WHERE TimetableId=@Tid AND StudentId=@Sid";
                        using (var checkCmd = new SQLiteCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@Tid", att.TimetableId);
                            checkCmd.Parameters.AddWithValue("@Sid", att.StudentId);
                            long exists = (long)checkCmd.ExecuteScalar();

                            string query = exists > 0 ?
                                "UPDATE Attendance SET Status=@Status WHERE TimetableId=@Tid AND StudentId=@Sid" :
                                "INSERT INTO Attendance (TimetableId, StudentId, Status) VALUES (@Tid, @Sid, @Status)";

                            using (var cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Tid", att.TimetableId);
                                cmd.Parameters.AddWithValue("@Sid", att.StudentId);
                                cmd.Parameters.AddWithValue("@Status", att.Status);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: " + ex.Message, "Error");
            }

        }

        // Delete all attendance records for a given timetable
        public void DeleteAttendance(int timetableId)
        {
            try
            {
                using (var conn = DataBaseCon.Connection())
                {
                    string query = "DELETE FROM Attendance WHERE TimetableId=@Tid";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Tid", timetableId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something went wrong: " + ex.Message, "Error");
            }

        }

        // Retrieves the attendance records for the currently logged-in student
        public List<dynamic> GetStudentMarkedAttendance(int userId)
        {
            var list = new List<dynamic>();

            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    // First, get the StudentId linked to this userId
                    int studentId = -1;
                    using (var cmd = new SQLiteCommand("SELECT StudentId FROM Student WHERE UserId = @UserId", con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        var result = cmd.ExecuteScalar();
                        if (result == null) return list;
                        studentId = Convert.ToInt32(result);
                    }

                    // Now get attendance and related timetable info for this student
                    string query = @"
                SELECT 
                    t.Date,
                    ts.StartTime || ' - ' || ts.EndTime AS TimeSlot,
                    s.SubjectName,
                    a.Status
                FROM Attendance a
                JOIN Timetable t ON a.TimetableId = t.TimetableId
                JOIN TimeSlot ts ON t.TimeSlotId = ts.TimeSlotId
                JOIN Subjects s ON t.SubjectId = s.SubjectId
                WHERE a.StudentId = @StudentId";

                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new
                                {
                                    Date = reader["Date"].ToString(),
                                    TimeSlot = reader["TimeSlot"].ToString(),
                                    SubjectName = reader["SubjectName"].ToString(),
                                    Status = reader["Status"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: " + ex.Message, "Error");
            }

            return list; 
        }


    }
}
