﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTicManagementSystem.Models
{
    internal class User
    {
        public int userId {  get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string userType { get; set; }
    }
}
