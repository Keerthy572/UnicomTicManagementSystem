using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTicManagementSystem.Models
{
    internal class Attendance
    {
        public int TimetableId { get; set; }
        public int StudentId { get; set; }
        public string Status { get; set; }
        public int GroupId { get; set; }

    }
}
