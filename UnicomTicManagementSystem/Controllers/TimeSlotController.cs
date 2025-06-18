using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using UnicomTicManagementSystem.Repositories;

public class TimeSlotController
{
    public DataTable GetTimeSlots()
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

    public void AddTimeSlot(string startTime, string endTime)
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

    public void UpdateTimeSlot(int id, string startTime, string endTime)
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

    public void DeleteTimeSlot(int id)
    {
        using (var con = DataBaseCon.Connection())
        {
            string query = "DELETE FROM TimeSlot WHERE TimeSlotId = @id";
            SQLiteCommand cmd = new SQLiteCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }

    public bool TimeSlotOverlaps(string startTime, string endTime, int? excludeId = null)
    {
        using (var con = DataBaseCon.Connection())
        {
            string query = @"
            SELECT COUNT(*) 
            FROM TimeSlot 
            WHERE 
                (StartTime < @end AND EndTime > @start)";

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


}
