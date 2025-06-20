using System;
using System.Collections.Generic;
using System.Data.SQLite;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class LecturerController
    {
        // Add a new lecturer to the User and Lecturer tables
        public void AddLecturer(Lecturer lecturer)
        {
            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    // Check if password already exists in User table
                    string checkQuery = "SELECT COUNT(*) FROM User WHERE Password = @password";
                    SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@password", lecturer.password);
                    long count = (long)checkCmd.ExecuteScalar();

                    if (count > 0)
                        throw new Exception("Password already exists. Please choose a different one.");

                    // Insert into User table
                    string insertUser = "INSERT INTO User (UserName, Password, UserType) VALUES (@uname, @pwd, 'lecturer');";
                    SQLiteCommand cmd = new SQLiteCommand(insertUser, con);
                    cmd.Parameters.AddWithValue("@uname", lecturer.userName);
                    cmd.Parameters.AddWithValue("@pwd", lecturer.password);
                    cmd.ExecuteNonQuery();

                    long userId = con.LastInsertRowId; // Retrieve the newly created user ID

                    // Insert into Lecturer table
                    string insertLecturer = "INSERT INTO Lecturer (LecturerName, UserId, GroupId) VALUES (@lname, @uid, @gid);";
                    SQLiteCommand lecCmd = new SQLiteCommand(insertLecturer, con);
                    lecCmd.Parameters.AddWithValue("@lname", lecturer.lecturerName);
                    lecCmd.Parameters.AddWithValue("@uid", userId);
                    lecCmd.Parameters.AddWithValue("@gid", lecturer.GroupId);
                    lecCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add lecturer: " + ex.Message);
            }
        }

        // Update an existing lecturer's information
        public void UpdateLecturer(Lecturer lecturer)
        {
            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    // Check if new password already exists for another user
                    string checkQuery = "SELECT COUNT(*) FROM User WHERE Password = @password AND UserId != @uid";
                    SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@password", lecturer.password);
                    checkCmd.Parameters.AddWithValue("@uid", lecturer.userId);
                    long count = (long)checkCmd.ExecuteScalar();

                    if (count > 0)
                        throw new Exception("Password already exists. Please choose a different one.");

                    // Update User table
                    string updateUser = "UPDATE User SET UserName = @uname, Password = @pwd WHERE UserId = @uid;";
                    SQLiteCommand userCmd = new SQLiteCommand(updateUser, con);
                    userCmd.Parameters.AddWithValue("@uname", lecturer.userName);
                    userCmd.Parameters.AddWithValue("@pwd", lecturer.password);
                    userCmd.Parameters.AddWithValue("@uid", lecturer.userId);
                    userCmd.ExecuteNonQuery();

                    // Update Lecturer table
                    string updateLecturer = "UPDATE Lecturer SET LecturerName = @lname, GroupId = @gid WHERE LecturerId = @lid;";
                    SQLiteCommand lecCmd = new SQLiteCommand(updateLecturer, con);
                    lecCmd.Parameters.AddWithValue("@lname", lecturer.lecturerName);
                    lecCmd.Parameters.AddWithValue("@gid", lecturer.GroupId);
                    lecCmd.Parameters.AddWithValue("@lid", lecturer.lecturerId);
                    lecCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update lecturer: " + ex.Message);
            }
        }

        // Delete a lecturer by lecturer ID and user ID
        public void DeleteLecturer(int lecturerId, int userId)
        {
            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    // Delete from Lecturer table
                    string delLecturer = "DELETE FROM Lecturer WHERE LecturerId = @lid;";
                    SQLiteCommand cmd = new SQLiteCommand(delLecturer, con);
                    cmd.Parameters.AddWithValue("@lid", lecturerId);
                    cmd.ExecuteNonQuery();

                    // Delete from User table
                    string delUser = "DELETE FROM User WHERE UserId = @uid;";
                    SQLiteCommand userCmd = new SQLiteCommand(delUser, con);
                    userCmd.Parameters.AddWithValue("@uid", userId);
                    userCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete lecturer: " + ex.Message);
            }
        }

        // Retrieve all lecturers with their group and user info
        public List<Lecturer> GetLecturers()
        {
            List<Lecturer> list = new List<Lecturer>();
            string query = @"
                SELECT l.LecturerId, l.LecturerName, l.GroupId,
                       u.UserId, u.UserName, u.Password,
                       g.GroupName 
                FROM Lecturer l
                JOIN User u ON l.UserId = u.UserId
                JOIN Groups g ON l.GroupId = g.GroupId
                WHERE u.UserType = 'lecturer';";

            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    SQLiteCommand cmd = new SQLiteCommand(query, con);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        list.Add(new Lecturer
                        {
                            lecturerId = reader.GetInt32(0),
                            lecturerName = reader.GetString(1),
                            GroupId = reader.GetInt32(2),
                            userId = reader.GetInt32(3),
                            userName = reader.GetString(4),
                            password = reader.GetString(5),
                            groupName = reader.GetString(6)
                        });
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve lecturers: " + ex.Message);
            }
        }
    }
}
