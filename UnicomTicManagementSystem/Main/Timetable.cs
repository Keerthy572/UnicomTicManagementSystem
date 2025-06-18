using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTicManagementSystem.Main
{
    internal class Timetable
    {
        public int TimetableId { get; set; }

        public int SubjectId { get; set; }
        public string SubjectName { get; set; }

        public int RoomId { get; set; }
        public string RoomName { get; set; }

        public int TimeSlotId { get; set; }
        public string TimeSlot { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public int LecturerId { get; set; }
        public string LecturerName { get; set; }

        public int GroupId { get; set; }
        public string GroupName { get; set; }

        public string Date { get; set; } // Stored as TEXT ("yyyy-MM-dd")
    }
}
