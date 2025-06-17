using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class RoomController
    {
        public void AddRoom(Room room)
        {
            if (string.IsNullOrWhiteSpace(room.roomName) || string.IsNullOrWhiteSpace(room.roomType))
                throw new Exception("Room Name and Room Type are required.");

            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                string checkQuery = "SELECT COUNT(*) FROM Room WHERE RoomName = @roomName AND RoomType = @roomType";
                SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@roomName", room.roomName);
                checkCmd.Parameters.AddWithValue("@roomType", room.roomType);

                long count = (long)checkCmd.ExecuteScalar();
                if (count > 0)
                    throw new Exception("Room already exists.");

                string query = "INSERT INTO Room (RoomName, RoomType) VALUES (@name, @type)";
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@name", room.roomName);
                cmd.Parameters.AddWithValue("@type", room.roomType);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateRoom(Room room)
        {
            if (string.IsNullOrWhiteSpace(room.roomName) || string.IsNullOrWhiteSpace(room.roomType))
                throw new Exception("Room Name and Room Type are required.");

            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                string query = "UPDATE Room SET RoomName = @name, RoomType = @type WHERE RoomId = @id";
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@name", room.roomName);
                cmd.Parameters.AddWithValue("@type", room.roomType);
                cmd.Parameters.AddWithValue("@id", room.roomId);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteRoom(int roomId)
        {
            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                string query = "DELETE FROM Room WHERE RoomId = @id";
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@id", roomId);
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetAllRooms()
        {
            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                string query = "SELECT * FROM Room";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, con);
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }
    }
}
