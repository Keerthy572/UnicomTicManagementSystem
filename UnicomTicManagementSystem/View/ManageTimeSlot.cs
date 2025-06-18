using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnicomTicManagementSystem.View
{
    public partial class ManageTimeSlot : Form
    {
        TimeSlotController controller = new TimeSlotController();
        int selectedTimeSlotId = -1;

        public ManageTimeSlot()
        {
            InitializeComponent();
            dtStart.Format = DateTimePickerFormat.Time;
            dtStart.ShowUpDown = true;
            dtEnd.Format = DateTimePickerFormat.Time;
            dtEnd.ShowUpDown = true;

            LoadTimeSlots();
        }

        private void LoadTimeSlots()
        {
            dgvTimeSlots.DataSource = controller.GetTimeSlots();
            dgvTimeSlots.ReadOnly = true;
            dgvTimeSlots.AllowUserToAddRows = false;
            dgvTimeSlots.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string start = dtStart.Value.ToString("HH:mm");
            string end = dtEnd.Value.ToString("HH:mm");

            if (start == end)
            {
                MessageBox.Show("Start and end times cannot be the same.");
                return;
            }

            if (controller.TimeSlotOverlaps(start, end))
            {
                MessageBox.Show("This time range overlaps with an existing slot.", "Overlap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            controller.AddTimeSlot(start, end);
            LoadTimeSlots();
        }


        private void dgvTimeSlots_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvTimeSlots.Rows[e.RowIndex];

                selectedTimeSlotId = Convert.ToInt32(row.Cells["TimeSlotId"].Value);
                dtStart.Value = DateTime.ParseExact(row.Cells["StartTime"].Value.ToString(), "HH:mm", null);
                dtEnd.Value = DateTime.ParseExact(row.Cells["EndTime"].Value.ToString(), "HH:mm", null);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedTimeSlotId == -1) return;

            string start = dtStart.Value.ToString("HH:mm");
            string end = dtEnd.Value.ToString("HH:mm");

            if (start == end)
            {
                MessageBox.Show("Start and end times cannot be the same.");
                return;
            }

            if (controller.TimeSlotOverlaps(start, end, selectedTimeSlotId))
            {
                MessageBox.Show("This time range overlaps with another existing slot.", "Overlap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            controller.UpdateTimeSlot(selectedTimeSlotId, start, end);
            LoadTimeSlots();
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedTimeSlotId == -1) return;

            controller.DeleteTimeSlot(selectedTimeSlotId);
            LoadTimeSlots();
        }

        private void ManageTimeSlot_Load(object sender, EventArgs e)
        {

        }
    }

}
