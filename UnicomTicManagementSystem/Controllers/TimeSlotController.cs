using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using UnicomTicManagementSystem.Repositories;

public class TimeSlotController
{
    // Get all time slots from the database
    public DataTable GetTimeSlots()
    {
        try
        {
            using (var con = DataBaseCon.Connection())
            {
                string query = "SELECT * FROM TimeSlot";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, con);
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to load time slots: " + ex.Message);
        }
    }

    // Add a new time slot
    public void AddTimeSlot(string startTime, string endTime)
    {
        try
        {
            using (var con = DataBaseCon.Connection())
            {
                string query = "INSERT INTO TimeSlot (StartTime, EndTime) VALUES (@start, @end)";
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@start", startTime);
                cmd.Parameters.AddWithValue("@end", endTime);
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to add time slot: " + ex.Message);
        }
    }

    // Update an existing time slot
    public void UpdateTimeSlot(int id, string startTime, string endTime)
    {
        try
        {
            using (var con = DataBaseCon.Connection())
            {
                string query = "UPDATE TimeSlot SET StartTime = @start, EndTime = @end WHERE TimeSlotId = @id";
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@start", startTime);
                cmd.Parameters.AddWithValue("@end", endTime);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to update time slot: " + ex.Message);
        }
    }

    // Delete a time slot by its ID
    public void DeleteTimeSlot(int id)
    {
        try
        {
            using (var con = DataBaseCon.Connection())
            {
                string query = "DELETE FROM TimeSlot WHERE TimeSlotId = @id";
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to delete time slot: " + ex.Message);
        }
    }

    // Check if a new time range overlaps with existing time slots
    public bool TimeSlotOverlaps(string startTime, string endTime, int? excludeId = null)
    {
        try
        {
            using (var con = DataBaseCon.Connection())
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM TimeSlot 
                    WHERE (StartTime < @end AND EndTime > @start)";

                if (excludeId.HasValue)
                {
                    query += " AND TimeSlotId != @id";
                }

                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@start", startTime);
                    cmd.Parameters.AddWithValue("@end", endTime);
                    if (excludeId.HasValue)
                        cmd.Parameters.AddWithValue("@id", excludeId.Value);

                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to check overlap: " + ex.Message);
        }
    }
}
