using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class GroupController
    {
        // Retrieves all groups from the database
        public List<Group> GetAllGroups()
        {
            List<Group> groups = new List<Group>();

            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    string query = "SELECT * FROM Groups";
                    SQLiteCommand cmd = new SQLiteCommand(query, con);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Group group = new Group()
                        {
                            groupId = Convert.ToInt32(reader["GroupId"]),
                            groupName = reader["GroupName"].ToString()
                        };
                        groups.Add(group);
                    }
                }
            }
            catch (Exception ex)
            {
                // In production you might log the error instead
                MessageBox.Show("Error loading groups: " + ex.Message);
            }

            return groups;
        }

        // Adds a new group to the database
        public string AddGroup(Group group)
        {
            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    string query = "INSERT INTO Groups (GroupName) VALUES (@name)";
                    SQLiteCommand cmd = new SQLiteCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", group.groupName);
                    int result = cmd.ExecuteNonQuery();
                    return result > 0 ? "Group added successfully." : "Failed to add group.";
                }
            }
            catch (Exception ex)
            {
                return "Error adding group: " + ex.Message;
            }
        }

        // Updates an existing group's name
        public string UpdateGroup(Group group)
        {
            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    string query = "UPDATE Groups SET GroupName = @name WHERE GroupId = @id";
                    SQLiteCommand cmd = new SQLiteCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", group.groupName);
                    cmd.Parameters.AddWithValue("@id", group.groupId);
                    int result = cmd.ExecuteNonQuery();
                    return result > 0 ? "Group updated successfully." : "Failed to update group.";
                }
            }
            catch (Exception ex)
            {
                return "Error updating group: " + ex.Message;
            }
        }

        // Deletes a group from the database
        public string DeleteGroup(int groupId)
        {
            try
            {
                using (SQLiteConnection con = DataBaseCon.Connection())
                {
                    string query = "DELETE FROM Groups WHERE GroupId = @id";
                    SQLiteCommand cmd = new SQLiteCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", groupId);
                    int result = cmd.ExecuteNonQuery();
                    return result > 0 ? "Group deleted successfully." : "Failed to delete group.";
                }
            }
            catch (Exception ex)
            {
                return "Error deleting group: " + ex.Message;
            }
        }
    }
}
