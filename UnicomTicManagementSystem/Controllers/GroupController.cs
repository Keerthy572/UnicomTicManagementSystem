using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class GroupController
    {
        public List<Group> GetAllGroups()
        {
            List<Group> groups = new List<Group>();
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
            return groups;
        }

        public string AddGroup(Group group)
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

        public string UpdateGroup(Group group)
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

        public string DeleteGroup(int groupId)
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
    }
}
