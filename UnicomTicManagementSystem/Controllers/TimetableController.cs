using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class TimetableController
    {
        // Get all timetable entries with joined details from related tables
        public List<Timetable> GetAllTimetables()
        {
            var list = new List<Timetable>();

            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = @"
                        SELECT t.TimetableId, t.Date,
                               t.TimeSlotId, ts.StartTime, ts.EndTime,
                               t.GroupId, g.GroupName,
                               t.LecturerId, l.LecturerName,
                               t.SubjectId, s.SubjectName,
                               t.RoomId, r.RoomName
                        FROM Timetable t
                        JOIN TimeSlot ts ON t.TimeSlotId = ts.TimeSlotId
                        JOIN Groups g ON t.GroupId = g.GroupId
                        JOIN Lecturer l ON t.LecturerId = l.LecturerId
                        JOIN Subjects s ON t.SubjectId = s.SubjectId
                        JOIN Room r ON t.RoomId = r.RoomId";

                    using (var cmd = new SQLiteCommand(query, con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Timetable
                            {
                                TimetableId = Convert.ToInt32(reader["TimetableId"]),
                                Date = reader["Date"].ToString(),

                                TimeSlotId = Convert.ToInt32(reader["TimeSlotId"]),
                                StartTime = reader["StartTime"].ToString(),
                                EndTime = reader["EndTime"].ToString(),
                                TimeSlot = reader["StartTime"] + " - " + reader["EndTime"],

                                GroupId = Convert.ToInt32(reader["GroupId"]),
                                GroupName = reader["GroupName"].ToString(),

                                LecturerId = Convert.ToInt32(reader["LecturerId"]),
                                LecturerName = reader["LecturerName"].ToString(),

                                SubjectId = Convert.ToInt32(reader["SubjectId"]),
                                SubjectName = reader["SubjectName"].ToString(),

                                RoomId = Convert.ToInt32(reader["RoomId"]),
                                RoomName = reader["RoomName"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to load timetables.\n" + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            return list;
        }

        // Get all rooms
        public List<Room> GetRooms()
        {
            var rooms = new List<Room>();
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = "SELECT RoomId, RoomName, RoomType FROM Room";
                    using (var cmd = new SQLiteCommand(query, con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rooms.Add(new Room
                            {
                                roomId = Convert.ToInt32(reader["RoomId"]),
                                roomName = reader["RoomName"].ToString(),
                                roomType = reader["RoomType"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to load rooms.\n" + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            return rooms;
        }

        // Get lecturers assigned to a specific group
        public List<KeyValuePair<int, string>> GetLecturersByGroup(int groupId)
        {
            var list = new List<KeyValuePair<int, string>>();
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = "SELECT LecturerId, LecturerName FROM Lecturer WHERE GroupId = @groupId";
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@groupId", groupId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new KeyValuePair<int, string>(
                                    Convert.ToInt32(reader["LecturerId"]),
                                    reader["LecturerName"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to load lecturers for the group.\n" + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            return list;
        }

        // Get subjects assigned to a lecturer via LecturerCourse table
        public List<KeyValuePair<int, string>> GetSubjectsByLecturer(int lecturerId)
        {
            var list = new List<KeyValuePair<int, string>>();
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = @"
                        SELECT s.SubjectId, s.SubjectName 
                        FROM LecturerCourse lc
                        JOIN Course c ON lc.CourseId = c.CourseId
                        JOIN Subjects s ON s.CourseId = c.CourseId
                        WHERE lc.LecturerId = @lecturerId";

                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new KeyValuePair<int, string>(
                                    Convert.ToInt32(reader["SubjectId"]),
                                    reader["SubjectName"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to load subjects for the lecturer.\n" + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            return list;
        }

        // Get all time slots
        public List<TimeSlot> GetTimeSlots()
        {
            var list = new List<TimeSlot>();
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = "SELECT * FROM TimeSlot";
                    using (var cmd = new SQLiteCommand(query, con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TimeSlot
                            {
                                TimeSlotId = Convert.ToInt32(reader["TimeSlotId"]),
                                StartTime = reader["StartTime"].ToString(),
                                EndTime = reader["EndTime"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to load time slots.\n" + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            return list;
        }

        // Check if there is any conflict for given time, date, room or lecturer excluding an optional timetable entry
        public bool IsConflict(int timeSlotId, string date, int roomId, int lecturerId, int? timetableId = null)
        {
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = @"
                        SELECT COUNT(*) FROM Timetable
                        WHERE Date = @date AND TimeSlotId = @timeSlotId AND (
                            RoomId = @roomId OR LecturerId = @lecturerId
                        )";

                    if (timetableId.HasValue)
                    {
                        query += " AND TimetableId != @timetableId";
                    }

                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.Parameters.AddWithValue("@timeSlotId", timeSlotId);
                        cmd.Parameters.AddWithValue("@roomId", roomId);
                        cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                        if (timetableId.HasValue)
                            cmd.Parameters.AddWithValue("@timetableId", timetableId.Value);

                        long count = (long)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to check timetable conflicts.\n" + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                // On error, return true to prevent adding conflicting entry
                return true;
            }
        }

        // Add a new timetable entry
        public void AddTimetable(Timetable t)
        {
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = @"
                        INSERT INTO Timetable (SubjectId, RoomId, TimeSlotId, LecturerId, Date, GroupId)
                        VALUES (@subjectId, @roomId, @timeSlotId, @lecturerId, @date, @groupId)";

                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@subjectId", t.SubjectId);
                        cmd.Parameters.AddWithValue("@roomId", t.RoomId);
                        cmd.Parameters.AddWithValue("@timeSlotId", t.TimeSlotId);
                        cmd.Parameters.AddWithValue("@lecturerId", t.LecturerId);
                        cmd.Parameters.AddWithValue("@date", t.Date);
                        cmd.Parameters.AddWithValue("@groupId", t.GroupId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to add timetable entry.\n" + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        // Update an existing timetable entry
        public void UpdateTimetable(Timetable t)
        {
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = @"
                        UPDATE Timetable
                        SET SubjectId = @subjectId, RoomId = @roomId, TimeSlotId = @timeSlotId,
                            LecturerId = @lecturerId, Date = @date, GroupId = @groupId
                        WHERE TimetableId = @timetableId";

                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@subjectId", t.SubjectId);
                        cmd.Parameters.AddWithValue("@roomId", t.RoomId);
                        cmd.Parameters.AddWithValue("@timeSlotId", t.TimeSlotId);
                        cmd.Parameters.AddWithValue("@lecturerId", t.LecturerId);
                        cmd.Parameters.AddWithValue("@date", t.Date);
                        cmd.Parameters.AddWithValue("@groupId", t.GroupId);
                        cmd.Parameters.AddWithValue("@timetableId", t.TimetableId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to update timetable entry.\n" + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        // Delete a timetable entry by its ID
        public void DeleteTimetable(int timetableId)
        {
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = "DELETE FROM Timetable WHERE TimetableId = @timetableId";
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@timetableId", timetableId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to delete timetable entry.\n" + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        // Alternative method to check if a timeslot is available for given date, room and lecturer excluding optional timetable entry
        public bool IsTimeSlotAvailable(int timeSlotId, string date, int roomId, int lecturerId, int? timetableIdToExclude = null)
        {
            try
            {
                using (var con = DataBaseCon.Connection())
                {
                    string query = @"
                        SELECT COUNT(*) 
                        FROM Timetable 
                        WHERE TimeSlotId = @timeSlotId AND Date = @date
                          AND (RoomId = @roomId OR LecturerId = @lecturerId)";

                    if (timetableIdToExclude.HasValue)
                    {
                        query += " AND TimetableId != @timetableId";
                    }

                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@timeSlotId", timeSlotId);
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.Parameters.AddWithValue("@roomId", roomId);
                        cmd.Parameters.AddWithValue("@lecturerId", lecturerId);
                        if (timetableIdToExclude.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@timetableId", timetableIdToExclude.Value);
                        }

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to check time slot availability.\n" + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                // Return false on error to be safe
                return false;
            }
        }

        // Get timetables for a certain Lecturer
        public List<Timetable> GetTimetablesByLecturerUserId(int userId)
        {
            var list = new List<Timetable>();
            try
            {
                using (var conn = DataBaseCon.Connection())
                {
                    // First, get LecturerId from UserId
                    string getLecturerIdQuery = "SELECT LecturerId FROM Lecturer WHERE UserId = @UserId";
                    int lecturerId = -1;

                    using (var cmd = new SQLiteCommand(getLecturerIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            lecturerId = Convert.ToInt32(result);
                        else
                            return list; // No matching lecturer found
                    }

                    // Now fetch timetables for this LecturerId
                    string query = @"
                        SELECT t.TimetableId, t.SubjectId, s.SubjectName, 
                               t.RoomId, r.RoomName,
                               t.TimeSlotId, ts.StartTime, ts.EndTime,
                               t.LecturerId, l.LecturerName,
                               t.GroupId, g.GroupName,
                               t.Date
                        FROM Timetable t
                        JOIN Subjects s ON t.SubjectId = s.SubjectId
                        JOIN Room r ON t.RoomId = r.RoomId
                        JOIN TimeSlot ts ON t.TimeSlotId = ts.TimeSlotId
                        JOIN Lecturer l ON t.LecturerId = l.LecturerId
                        JOIN Groups g ON t.GroupId = g.GroupId
                        WHERE t.LecturerId = @LecturerId";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LecturerId", lecturerId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new Timetable
                                {
                                    TimetableId = Convert.ToInt32(reader["TimetableId"]),
                                    SubjectId = Convert.ToInt32(reader["SubjectId"]),
                                    SubjectName = reader["SubjectName"].ToString(),
                                    RoomId = Convert.ToInt32(reader["RoomId"]),
                                    RoomName = reader["RoomName"].ToString(),
                                    TimeSlotId = Convert.ToInt32(reader["TimeSlotId"]),
                                    StartTime = reader["StartTime"].ToString(),
                                    EndTime = reader["EndTime"].ToString(),
                                    TimeSlot = reader["StartTime"] + " - " + reader["EndTime"],
                                    LecturerId = Convert.ToInt32(reader["LecturerId"]),
                                    LecturerName = reader["LecturerName"].ToString(),
                                    GroupId = Convert.ToInt32(reader["GroupId"]),
                                    GroupName = reader["GroupName"].ToString(),
                                    Date = reader["Date"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something went wrong : " + ex.Message);
            }
            

            return list;
        }

    }
}
