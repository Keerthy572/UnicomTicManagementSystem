using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class TimetableController
    {
        public List<Timetable> GetAllTimetables()
        {
            List<Timetable> list = new List<Timetable>();
            using (var con = DataBaseCon.Connection())
            {
                string query = @"
                    SELECT t.TimetableId, t.Date, 
                           ts.StartTime || ' - ' || ts.EndTime AS TimeSlot,
                           g.GroupName, l.LecturerName, s.SubjectName, r.RoomName
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
                            TimeSlot = reader["TimeSlot"].ToString(),
                            GroupName = reader["GroupName"].ToString(),
                            LecturerName = reader["LecturerName"].ToString(),
                            SubjectId = 0, // Not shown in this query
                            RoomId = 0, // Not shown in this query
                        });
                    }
                }
            }
            return list;
        }

        public List<Room> GetRooms()
        {
            var rooms = new List<Room>();
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
            return rooms;
        }

        public List<KeyValuePair<int, string>> GetLecturersByGroup(int groupId)
        {
            var list = new List<KeyValuePair<int, string>>();
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
            return list;
        }

        public List<KeyValuePair<int, string>> GetSubjectsByLecturer(int lecturerId)
        {
            var list = new List<KeyValuePair<int, string>>();
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
            return list;
        }

        

        public List<TimeSlot> GetTimeSlots()
        {
            var list = new List<TimeSlot>();
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
            return list;
        }


        public bool IsConflict(int timeSlotId, string date, int roomId, int lecturerId, int? timetableId = null)
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

        public void AddTimetable(Timetable t)
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

        public void UpdateTimetable(Timetable t)
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

        public void DeleteTimetable(int timetableId)
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

        public bool IsTimeSlotAvailable(int timeSlotId, string date, int roomId, int lecturerId, int? timetableIdToExclude = null)
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

    }
}
