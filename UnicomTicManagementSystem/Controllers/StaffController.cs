using System;
using System.Collections.Generic;
using System.Data.SQLite;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class StaffController
    {
        // Add a new staff member with associated user account
        public void AddStaff(Staff staff, string userName, string password)
        {
            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    // Check if password already exists in any user account
                    string checkQuery = "SELECT COUNT(*) FROM User WHERE Password = @password";
                    SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@password", password);
                    long count = (long)checkCmd.ExecuteScalar();

                    if (count > 0)
                        throw new Exception("Password already exists. Please choose a different one.");

                    // Insert into User table
                    string insertUser = "INSERT INTO User (UserName, Password, UserType) VALUES (@uname, @pwd, 'staff');";
                    SQLiteCommand cmd = new SQLiteCommand(insertUser, con);
                    cmd.Parameters.AddWithValue("@uname", userName);
                    cmd.Parameters.AddWithValue("@pwd", password);
                    cmd.ExecuteNonQuery();

                    // Retrieve generated UserId
                    long userId = con.LastInsertRowId;

                    // Insert into Staff table
                    string insertStaff = "INSERT INTO Staff (StaffName, UserId) VALUES (@sname, @uid);";
                    SQLiteCommand staffCmd = new SQLiteCommand(insertStaff, con);
                    staffCmd.Parameters.AddWithValue("@sname", staff.staffName);
                    staffCmd.Parameters.AddWithValue("@uid", userId);
                    staffCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add staff: " + ex.Message);
            }
        }

        // Update staff details and user account information
        public void UpdateStaff(Staff staff, string userName, string password)
        {
            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    // Ensure no other user is using the same password
                    string checkQuery = "SELECT COUNT(*) FROM User WHERE Password = @password AND UserId != @uid";
                    SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@password", password);
                    checkCmd.Parameters.AddWithValue("@uid", staff.userId);
                    long count = (long)checkCmd.ExecuteScalar();

                    if (count > 0)
                        throw new Exception("Password already exists. Please choose a different one.");

                    // Update User table
                    string updateUser = "UPDATE User SET UserName = @uname, Password = @pwd WHERE UserId = @uid;";
                    SQLiteCommand userCmd = new SQLiteCommand(updateUser, con);
                    userCmd.Parameters.AddWithValue("@uname", userName);
                    userCmd.Parameters.AddWithValue("@pwd", password);
                    userCmd.Parameters.AddWithValue("@uid", staff.userId);
                    userCmd.ExecuteNonQuery();

                    // Update Staff table
                    string updateStaff = "UPDATE Staff SET StaffName = @sname WHERE StaffId = @sid;";
                    SQLiteCommand staffCmd = new SQLiteCommand(updateStaff, con);
                    staffCmd.Parameters.AddWithValue("@sname", staff.staffName);
                    staffCmd.Parameters.AddWithValue("@sid", staff.staffId);
                    staffCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update staff: " + ex.Message);
            }
        }

        // Delete a staff record and associated user account
        public void DeleteStaff(int staffId, int userId)
        {
            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    // Delete staff record
                    string deleteStaff = "DELETE FROM Staff WHERE StaffId = @sid;";
                    SQLiteCommand cmd = new SQLiteCommand(deleteStaff, con);
                    cmd.Parameters.AddWithValue("@sid", staffId);
                    cmd.ExecuteNonQuery();

                    // Delete related user record
                    string deleteUser = "DELETE FROM User WHERE UserId = @uid;";
                    SQLiteCommand userCmd = new SQLiteCommand(deleteUser, con);
                    userCmd.Parameters.AddWithValue("@uid", userId);
                    userCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete staff: " + ex.Message);
            }
        }

        // Retrieve all staff members along with user account info
        public List<Staff> GetAllStaff()
        {
            List<Staff> staffList = new List<Staff>();
            string query = @"
                SELECT s.StaffId, s.StaffName, u.UserId, u.UserName, u.Password
                FROM Staff s
                JOIN User u ON s.UserId = u.UserId
                WHERE u.UserType = 'staff';";

            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    SQLiteCommand cmd = new SQLiteCommand(query, con);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        staffList.Add(new Staff
                        {
                            staffId = reader.GetInt32(0),
                            staffName = reader.GetString(1),
                            userId = reader.GetInt32(2),
                            userName = reader.GetString(3),
                            password = reader.GetString(4)
                        });
                    }
                }

                return staffList;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve staff data: " + ex.Message);
            }
        }
    }
}
