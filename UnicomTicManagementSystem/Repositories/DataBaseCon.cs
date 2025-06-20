using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTicManagementSystem.Repositories
{
    //This class provide SQLite database connections for the application
    internal static class DataBaseCon
    {
        // Returns an open SQLiteConnection to the UnicomTic.db database.
        public static SQLiteConnection Connection()
        {
            string connectionString = "Data Source = UnicomTic.db; Version = 3;";
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            return con;
        }
    }
}
