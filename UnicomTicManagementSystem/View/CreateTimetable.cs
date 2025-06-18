using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Controllers;
using static System.Windows.Forms.AxHost;

namespace UnicomTicManagementSystem.View
{
    public partial class CreateTimetable : Form
    {
        TimetableController controller = new TimetableController();
        int selectedTimetableId = -1;

        public CreateTimetable()
        {
            InitializeComponent();
            LoadTimeSlots();
            LoadGroups();
            LoadRooms();
            dtDate.Format = DateTimePickerFormat.Short;
        }

        private void LoadTimeSlots()
        {
            var timeSlots = controller.GetTimeSlots(); // This should return List<TimeSlot>
            comboBox1.DataSource = timeSlots;
            comboBox1.DisplayMember = "DisplayTime";  // Or use "StartTime" if DisplayTime doesn't exist
            comboBox1.ValueMember = "TimeSlotId";
        }


        private void LoadGroups()
        {
            GroupController groupController = new GroupController();
            var groups = groupController.GetAllGroups();
            comboBox2.DataSource = groups;
            comboBox2.DisplayMember = "GroupName";
            comboBox2.ValueMember = "GroupId";
        }

        private void LoadRooms()
        {
            comboBox5.DataSource = controller.GetRooms();
            comboBox5.DisplayMember = "RoomName";
            comboBox5.ValueMember = "RoomId";
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue is int groupId)
            {
                comboBox3.DataSource = controller.GetLecturersByGroup(groupId);
                comboBox3.DisplayMember = "Value";  // the string (LecturerName)
                comboBox3.ValueMember = "Key";      // the int (LecturerId)

            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedValue is int lecturerId)
            {
                comboBox4.DataSource = controller.GetSubjectsByLecturer(lecturerId);
                comboBox4.DisplayMember = "Value";  // SubjectName
                comboBox4.ValueMember = "Key";      // SubjectId

            }
        }

        private void LoadGrid()
        {
            dataGridView1.DataSource = controller.GetAllTimetables();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string date = dtDate.Value.ToString("yyyy-MM-dd");
            int timeSlotId = (int)comboBox1.SelectedValue;
            int groupId = (int)comboBox2.SelectedValue;
            int lecturerId = (int)comboBox3.SelectedValue;
            int subjectId = (int)comboBox4.SelectedValue;
            int roomId = (int)comboBox5.SelectedValue;

            if (!controller.IsTimeSlotAvailable(timeSlotId, date, roomId, lecturerId))
            {
                MessageBox.Show("Conflict: Room or Lecturer already booked for the selected time slot.", "Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Timetable t = new Timetable
            {
                TimeSlotId = timeSlotId,
                Date = date,
                GroupId = groupId,
                LecturerId = lecturerId,
                SubjectId = subjectId,
                RoomId = roomId
            };


            controller.AddTimetable(t);
            LoadGrid();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridView1.Rows[e.RowIndex];
                selectedTimetableId = Convert.ToInt32(row.Cells["TimetableId"].Value);

                comboBox1.SelectedValue = Convert.ToInt32(row.Cells["TimeSlotId"].Value);
                comboBox2.SelectedValue = Convert.ToInt32(row.Cells["GroupId"].Value);
                comboBox3.SelectedValue = Convert.ToInt32(row.Cells["LecturerId"].Value);
                comboBox4.SelectedValue = Convert.ToInt32(row.Cells["SubjectId"].Value);
                comboBox5.SelectedValue = Convert.ToInt32(row.Cells["RoomId"].Value);
                dtDate.Value = DateTime.Parse(row.Cells["Date"].Value.ToString());
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedTimetableId == -1) return;

            string date = dtDate.Value.ToString("yyyy-MM-dd");
            int timeSlotId = (int)comboBox1.SelectedValue;
            int groupId = (int)comboBox2.SelectedValue;
            int lecturerId = (int)comboBox3.SelectedValue;
            int subjectId = (int)comboBox4.SelectedValue;
            int roomId = (int)comboBox5.SelectedValue;

            if (!controller.IsTimeSlotAvailable(timeSlotId, date, roomId, lecturerId, selectedTimetableId))
            {
                MessageBox.Show("Conflict: Room or Lecturer already booked for the selected time slot.", "Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Timetable t = new Timetable
            {
                TimeSlotId = timeSlotId,
                Date = date,
                GroupId = groupId,
                LecturerId = lecturerId,
                SubjectId = subjectId,
                RoomId = roomId
            };

            controller.UpdateTimetable(t);
            LoadGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedTimetableId == -1) return;
            controller.DeleteTimetable(selectedTimetableId);
            LoadGrid();
        }
    }
}
