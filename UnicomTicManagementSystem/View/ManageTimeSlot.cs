using System;
using System.Data;
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

            // Set time-only format for start and end pickers
            dtStart.Format = DateTimePickerFormat.Time;
            dtStart.ShowUpDown = true;
            dtEnd.Format = DateTimePickerFormat.Time;
            dtEnd.ShowUpDown = true;

            LoadTimeSlots();
        }

        // Loads all existing time slots into the grid
        private void LoadTimeSlots()
        {
            try
            {
                dgvTimeSlots.DataSource = controller.GetTimeSlots();
                dgvTimeSlots.ReadOnly = true;
                dgvTimeSlots.AllowUserToAddRows = false;
                dgvTimeSlots.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading time slots: " + ex.Message);
            }
        }

        // Add new time slot
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
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
                MessageBox.Show("Time slot added.");
                LoadTimeSlots();
                selectedTimeSlotId = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding time slot: " + ex.Message);
            }
        }

        // Load selected time slot into form for editing
        private void dgvTimeSlots_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvTimeSlots.Rows[e.RowIndex];

                    selectedTimeSlotId = Convert.ToInt32(row.Cells["TimeSlotId"].Value);
                    dtStart.Value = DateTime.ParseExact(row.Cells["StartTime"].Value.ToString(), "HH:mm", null);
                    dtEnd.Value = DateTime.ParseExact(row.Cells["EndTime"].Value.ToString(), "HH:mm", null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error selecting time slot: " + ex.Message);
            }
        }

        // Update selected time slot
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedTimeSlotId == -1)
                {
                    MessageBox.Show("Please select a time slot to update.");
                    return;
                }

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
                MessageBox.Show("Time slot updated.");
                LoadTimeSlots();
                selectedTimeSlotId = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating time slot: " + ex.Message);
            }
        }

        // Delete selected time slot
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedTimeSlotId == -1)
                {
                    MessageBox.Show("Please select a time slot to delete.");
                    return;
                }

                DialogResult confirm = MessageBox.Show("Are you sure you want to delete this time slot?", "Confirm Delete", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    controller.DeleteTimeSlot(selectedTimeSlotId);
                    MessageBox.Show("Time slot deleted.");
                    LoadTimeSlots();
                    selectedTimeSlotId = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting time slot: " + ex.Message);
            }
        }

        // Close the form
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
