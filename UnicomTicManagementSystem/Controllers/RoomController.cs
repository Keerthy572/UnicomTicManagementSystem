﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class RoomController
    {
        // Adds a new room to the database after checking for duplicates
        public void AddRoom(Room room)
        {
            if (string.IsNullOrWhiteSpace(room.roomName) || string.IsNullOrWhiteSpace(room.roomType))
            {
                MessageBox.Show("Room Name and Room Type are required.");
                return;
            }
                

            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                try
                {
                    // Check for duplicate room
                    string checkQuery = "SELECT COUNT(*) FROM Room WHERE RoomName = @roomName AND RoomType = @roomType";
                    using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@roomName", room.roomName);
                        checkCmd.Parameters.AddWithValue("@roomType", room.roomType);
                        long count = (long)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Room already exists.");
                            return;
                        }
                    }

                    // Insert new room
                    string query = "INSERT INTO Room (RoomName, RoomType) VALUES (@name, @type)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", room.roomName);
                        cmd.Parameters.AddWithValue("@type", room.roomType);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while adding room: " + ex.Message);
                }
            }
        }

        // Updates an existing room's details
        public void UpdateRoom(Room room)
        {
            if (room.roomId <= 0)
            {
                MessageBox.Show("Invalid room selected.");
                return;
            }
                

            if (string.IsNullOrWhiteSpace(room.roomName) || string.IsNullOrWhiteSpace(room.roomType))
            {
                MessageBox.Show("Room Name and Room Type are required.");
                return;
            }
                

            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                try
                {
                    string query = "UPDATE Room SET RoomName = @name, RoomType = @type WHERE RoomId = @id";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", room.roomName);
                        cmd.Parameters.AddWithValue("@type", room.roomType);
                        cmd.Parameters.AddWithValue("@id", room.roomId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                            throw new Exception("No room found to update.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while updating room: " + ex.Message);
                }
            }
        }

        // Deletes a room by RoomId
        public void DeleteRoom(int roomId)
        {
            if (roomId <= 0)
            {
                MessageBox.Show("Invalid room selected.");
                return;
            }
                

            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                try
                {
                    string query = "DELETE FROM Room WHERE RoomId = @id";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", roomId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            MessageBox.Show("No room found to delete.");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while deleting room: " + ex.Message);
                }
            }
        }

        // Retrieves all rooms from the database as a DataTable
        public DataTable GetAllRooms()
        {
            using (SQLiteConnection con = DataBaseCon.Connection())
            {
                try
                {
                    string query = "SELECT * FROM Room";
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, con))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving rooms: " + ex.Message);
                    return null;
                }
            }
        }
    }
}
