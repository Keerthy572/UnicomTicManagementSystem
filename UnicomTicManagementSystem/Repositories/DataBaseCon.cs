using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTicManagementSystem.Repositories
{
    internal static class DataBaseCon
    {
        public static SQLiteConnection Connection()
        {
            string connectionString = "Data Source = UnicomTic.db; Version = 3;";
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            return con;
        }
    }
}
