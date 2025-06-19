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
            string loginCheck = "SELECT UserId, UserName, Password , UserType FROM User;";

            using (SQLiteConnection dbcon = DataBaseCon.Connection())
            {
                SQLiteCommand getdetails = new SQLiteCommand(loginCheck, dbcon);
                using(SQLiteDataReader reader = getdetails.ExecuteReader())
                {
                    while (reader.Read())
                    {
                       User user = new User();
                        user.userId = reader.GetInt32(0);
                        user.userName = reader.GetString(1);
                        user.password = reader.GetString(2);
                        user.userType = reader.GetString(3);


                        userlist.Add(user);
                    }
                }return userlist;
            }
        }


         
    }
}
