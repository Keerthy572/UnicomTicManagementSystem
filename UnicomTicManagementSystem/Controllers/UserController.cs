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
    internal class UserController
    {
        public List<User> LoginCheck()
        {
            List<User> userlist = new List<User>();
            string loginCheck = "SELECT UserName, Password FROM User;";

            using (SQLiteConnection dbcon = DataBaseCon.Connection())
            {
                SQLiteCommand getdetails = new SQLiteCommand(loginCheck, dbcon);
                using(SQLiteDataReader reader = getdetails.ExecuteReader())
                {
                    while (reader.Read())
                    {
                       User user = new User();
                        user.userName = reader.GetString(0);
                        user.password = reader.GetString(1);
                        
                        userlist.Add(user);
                    }
                }return userlist;
            }
        }
         
    }
}
