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
            LoadGrid();
            dtDate.Format = DateTimePickerFormat.Short;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox5.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LoadTimeSlots()
        {
            var timeSlots = controller.GetTimeSlots(); // This should return List<TimeSlot>
            comboBox1.DataSource = timeSlots;
            comboBox1.DisplayMember = "DisplayTime";  // Or use "StartTime" if DisplayTime doesn't exist
            comboBox1.ValueMember = "TimeSlotId";
            comboBox1.SelectedIndex = -1;
        }


        private void LoadGroups()
        {
            GroupController groupController = new GroupController();
            var groups = groupController.GetAllGroups();
            comboBox2.DataSource = groups;
            comboBox2.DisplayMember = "GroupName";
            comboBox2.ValueMember = "GroupId";
            comboBox2.SelectedIndex = -1;

        }

        private void LoadRooms()
        {
            comboBox5.DataSource = controller.GetRooms();
            comboBox5.DisplayMember = "RoomName";
            comboBox5.ValueMember = "RoomId";
            comboBox5.SelectedIndex = -1;

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue is int groupId)
            {
                comboBox3.DataSource = controller.GetLecturersByGroup(groupId);
                comboBox3.DisplayMember = "Value";  // the string (LecturerName)
                comboBox3.ValueMember = "Key";      // the int (LecturerId)
                comboBox3.SelectedIndex = -1;


            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedValue is int lecturerId)
            {
                comboBox4.DataSource = controller.GetSubjectsByLecturer(lecturerId);
                comboBox4.DisplayMember = "Value";  // SubjectName
                comboBox4.ValueMember = "Key";      // SubjectId
                comboBox4.SelectedIndex = -1;


            }
        }

        private void LoadGrid()
        {
            var timetables = controller.GetAllTimetables();
            dataGridView1.DataSource = timetables;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;

            // Optional: manually set column headers or order
            dataGridView1.Columns["TimetableId"].HeaderText ="Timetable Id"; // hide internal ID
            dataGridView1.Columns["Date"].HeaderText = "Date";
            dataGridView1.Columns["TimeSlot"].HeaderText = "Time Slot";
            dataGridView1.Columns["GroupName"].HeaderText = "Group";
            dataGridView1.Columns["LecturerName"].HeaderText = "Lecturer";
            dataGridView1.Columns["SubjectName"].HeaderText = "Subject";
            dataGridView1.Columns["RoomName"].HeaderText = "Room";

            dataGridView1.Columns["TimetableId"].DisplayIndex = 0;
            dataGridView1.Columns["Date"].DisplayIndex = 1;
            dataGridView1.Columns["TimeSlot"].DisplayIndex = 2;
            dataGridView1.Columns["GroupName"].DisplayIndex = 3;
            dataGridView1.Columns["LecturerName"].DisplayIndex = 4;
            dataGridView1.Columns["SubjectName"].DisplayIndex = 5;
            dataGridView1.Columns["RoomName"].DisplayIndex = 6;

            dataGridView1.Columns["SubjectId"].Visible = false;
            dataGridView1.Columns["RoomId"].Visible = false;
            dataGridView1.Columns["TimeSlotId"].Visible = false;
            dataGridView1.Columns["StartTime"].Visible = false;
            dataGridView1.Columns["EndTime"].Visible = false;
            dataGridView1.Columns["GroupId"].Visible = false;
            dataGridView1.Columns["LecturerId"].Visible = false;


        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Ensure all ComboBoxes have selections
            if (comboBox1.SelectedIndex == -1 ||
                comboBox2.SelectedIndex == -1 ||
                comboBox3.SelectedIndex == -1 ||
                comboBox4.SelectedIndex == -1 ||
                comboBox5.SelectedIndex == -1)
            {
                MessageBox.Show("Please select all required fields (Time Slot, Group, Lecturer, Subject, and Room).", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string date = dtDate.Value.ToString("yyyy-MM-dd");
            int timeSlotId = (int)comboBox1.SelectedValue;
            int groupId = (int)comboBox2.SelectedValue;
            int lecturerId = (int)comboBox3.SelectedValue;
            int subjectId = (int)comboBox4.SelectedValue;
            int roomId = (int)comboBox5.SelectedValue;

            // Conflict validation: Room or Lecturer already booked
            if (!controller.IsTimeSlotAvailable(timeSlotId, date, roomId, lecturerId))
            {
                MessageBox.Show("Conflict: Either the room or lecturer is already booked for the selected date and time slot.", "Schedule Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create and add the timetable entry
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

            // Optional: Reset form for next entry
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.DataSource = null;
            comboBox4.DataSource = null;
            comboBox5.SelectedIndex = -1;
            dtDate.Value = DateTime.Today;

            MessageBox.Show("Timetable added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (selectedTimetableId == -1)
            {
                MessageBox.Show("Please select a timetable entry to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if all required fields are selected
            if (comboBox1.SelectedIndex == -1 ||
                comboBox2.SelectedIndex == -1 ||
                comboBox3.SelectedIndex == -1 ||
                comboBox4.SelectedIndex == -1 ||
                comboBox5.SelectedIndex == -1)
            {
                MessageBox.Show("Please make sure all fields (Time Slot, Group, Lecturer, Subject, Room) are selected.", "Missing Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string date = dtDate.Value.ToString("yyyy-MM-dd");
            int timeSlotId = (int)comboBox1.SelectedValue;
            int groupId = (int)comboBox2.SelectedValue;
            int lecturerId = (int)comboBox3.SelectedValue;
            int subjectId = (int)comboBox4.SelectedValue;
            int roomId = (int)comboBox5.SelectedValue;

            // Conflict check: same timeslot, date, and (room or lecturer)
            if (!controller.IsTimeSlotAvailable(timeSlotId, date, roomId, lecturerId, selectedTimetableId))
            {
                MessageBox.Show("Conflict: Either the room or lecturer is already booked for the selected time slot.", "Schedule Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Perform update
            Timetable t = new Timetable
            {
                TimetableId = selectedTimetableId,  // ✅ this was missing!
                TimeSlotId = timeSlotId,
                Date = date,
                GroupId = groupId,
                LecturerId = lecturerId,
                SubjectId = subjectId,
                RoomId = roomId
            };

            controller.UpdateTimetable(t);
            LoadGrid();

            // Optional reset
            selectedTimetableId = -1;
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.DataSource = null;
            comboBox4.DataSource = null;
            comboBox5.SelectedIndex = -1;
            dtDate.Value = DateTime.Today;

            MessageBox.Show("Timetable updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedTimetableId == -1) return;
            controller.DeleteTimetable(selectedTimetableId);
            LoadGrid();
        }

        private void CreateTimetable_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
