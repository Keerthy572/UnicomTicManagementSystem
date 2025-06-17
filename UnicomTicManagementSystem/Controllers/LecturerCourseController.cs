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
    internal class LecturerCourseController
    {
        public void AddLecturerCourse(LecturerCourse lc)
        {
            using (var con = DataBaseCon.Connection())
            {
                // First check if the entry already exists
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

                // Insert if not exists
                string insertQuery = "INSERT INTO LecturerCourse (LecturerId, CourseId) VALUES (@lid, @cid);";
                SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, con);
                insertCmd.Parameters.AddWithValue("@lid", lc.LecturerId);
                insertCmd.Parameters.AddWithValue("@cid", lc.CourseId);
                insertCmd.ExecuteNonQuery();
            }
        }


        public void UpdateLecturerCourse(int oldLecturerId, int oldCourseId, LecturerCourse updated)
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

        public void DeleteLecturerCourse(int lecturerId, int courseId)
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

        public List<LecturerCourse> GetLecturerCourses()
        {
            List<LecturerCourse> list = new List<LecturerCourse>();

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

            return list;
        }
    }
}
