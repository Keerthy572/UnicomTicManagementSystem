using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTicManagementSystem.Main
{
    public class ExamMark
    {
        public string ExamName { get; set; }
        public string SubjectName { get; set; }
        public string Score { get; set; } // Use string so you can show "Absent"
    }

}
