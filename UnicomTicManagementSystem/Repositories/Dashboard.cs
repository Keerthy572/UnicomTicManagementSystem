using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTicManagementSystem.Repositories
{
    public static class Dashboard  // This class acts as a global session store for the logged-in user.
    {
        public static int userId;
        public static string userType;
    }
}
