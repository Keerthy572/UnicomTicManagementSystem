using System;
using System.Collections.Generic;
using System.Data.SQLite;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class StaffController
    {
        public void AddStaff(Staff staff, string userName, string password)
        {
            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                string checkQuery = "SELECT COUNT(*) FROM User WHERE Password = @password";
                SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@password", password);
                long count = (long)checkCmd.ExecuteScalar();

                if (count > 0)
                    throw new Exception("Password already exists.");

                string insertUser = "INSERT INTO User (UserName, Password, UserType) VALUES (@uname, @pwd, 'staff');";
                SQLiteCommand cmd = new SQLiteCommand(insertUser, con);
                cmd.Parameters.AddWithValue("@uname", userName);
                cmd.Parameters.AddWithValue("@pwd", password);
                cmd.ExecuteNonQuery();

                long userId = con.LastInsertRowId;

                string insertStaff = "INSERT INTO Staff (StaffName, UserId) VALUES (@sname, @uid);";
                SQLiteCommand staffCmd = new SQLiteCommand(insertStaff, con);
                staffCmd.Parameters.AddWithValue("@sname", staff.staffName);
                staffCmd.Parameters.AddWithValue("@uid", userId);
                staffCmd.ExecuteNonQuery();
            }
        }

        public void UpdateStaff(Staff staff, string userName, string password)
        {
            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                string checkQuery = "SELECT COUNT(*) FROM User WHERE Password = @password AND UserId != @uid";
                SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@password", password);
                checkCmd.Parameters.AddWithValue("@uid", staff.userId);
                long count = (long)checkCmd.ExecuteScalar();

                if (count > 0)
                    throw new Exception("Password already exists.");

                string updateUser = "UPDATE User SET UserName = @uname, Password = @pwd WHERE UserId = @uid;";
                SQLiteCommand userCmd = new SQLiteCommand(updateUser, con);
                userCmd.Parameters.AddWithValue("@uname", userName);
                userCmd.Parameters.AddWithValue("@pwd", password);
                userCmd.Parameters.AddWithValue("@uid", staff.userId);
                userCmd.ExecuteNonQuery();

                string updateStaff = "UPDATE Staff SET StaffName = @sname WHERE StaffId = @sid;";
                SQLiteCommand staffCmd = new SQLiteCommand(updateStaff, con);
                staffCmd.Parameters.AddWithValue("@sname", staff.staffName);
                staffCmd.Parameters.AddWithValue("@sid", staff.staffId);
                staffCmd.ExecuteNonQuery();
            }
        }

        public void DeleteStaff(int staffId, int userId)
        {
            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                string deleteStaff = "DELETE FROM Staff WHERE StaffId = @sid;";
                SQLiteCommand cmd = new SQLiteCommand(deleteStaff, con);
                cmd.Parameters.AddWithValue("@sid", staffId);
                cmd.ExecuteNonQuery();

                string deleteUser = "DELETE FROM User WHERE UserId = @uid;";
                SQLiteCommand userCmd = new SQLiteCommand(deleteUser, con);
                userCmd.Parameters.AddWithValue("@uid", userId);
                userCmd.ExecuteNonQuery();
            }
        }

        public List<Staff> GetAllStaff()
        {
            List<Staff> staffList = new List<Staff>();
            string query = @"SELECT s.StaffId, s.StaffName, u.UserId, u.UserName, u.Password
                             FROM Staff s
                             JOIN User u ON s.UserId = u.UserId
                             WHERE u.UserType = 'staff';";

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
    }
}
