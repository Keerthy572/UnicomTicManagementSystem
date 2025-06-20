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

        // Constructor: Initializes the form and loads data into ComboBoxes and DataGridView.
        // Sets date format and ComboBox styles.
        public CreateTimetable()
        {
            InitializeComponent();
            try
            {
                LoadTimeSlots();
                LoadGroups();
                LoadRooms();
                LoadGrid();

                dtDate.Format = DateTimePickerFormat.Short;

                // Set ComboBoxes to DropDownList to prevent invalid text input
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox5.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Loads available TimeSlots into comboBox1.
        // TimeSlots must have a DisplayTime property for display.
        private void LoadTimeSlots()
        {
            try
            {
                var timeSlots = controller.GetTimeSlots(); // Returns List<TimeSlot>
                comboBox1.DataSource = timeSlots;
                comboBox1.DisplayMember = "DisplayTime";  // Or use StartTime property if DisplayTime is unavailable
                comboBox1.ValueMember = "TimeSlotId";
                comboBox1.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load time slots: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Loads Groups into comboBox2 from GroupController.
        private void LoadGroups()
        {
            try
            {
                GroupController groupController = new GroupController();
                var groups = groupController.GetAllGroups();
                comboBox2.DataSource = groups;
                comboBox2.DisplayMember = "GroupName";
                comboBox2.ValueMember = "GroupId";
                comboBox2.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load groups: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Loads available Rooms into comboBox5.
        private void LoadRooms()
        {
            try
            {
                comboBox5.DataSource = controller.GetRooms();
                comboBox5.DisplayMember = "RoomName";
                comboBox5.ValueMember = "RoomId";
                comboBox5.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load rooms: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event handler: When a Group is selected in comboBox2,
        // loads the lecturers of that group into comboBox3.
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox2.SelectedValue is int groupId)
                {
                    comboBox3.DataSource = controller.GetLecturersByGroup(groupId);
                    comboBox3.DisplayMember = "Value";  // LecturerName string
                    comboBox3.ValueMember = "Key";      // LecturerId int
                    comboBox3.SelectedIndex = -1;
                }
                else
                {
                    comboBox3.DataSource = null;
                    comboBox4.DataSource = null; // Clear subjects as well
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load lecturers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event handler: When a Lecturer is selected in comboBox3,
        // loads the subjects taught by that lecturer into comboBox4.
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox3.SelectedValue is int lecturerId)
                {
                    comboBox4.DataSource = controller.GetSubjectsByLecturer(lecturerId);
                    comboBox4.DisplayMember = "Value";  // SubjectName string
                    comboBox4.ValueMember = "Key";      // SubjectId int
                    comboBox4.SelectedIndex = -1;
                }
                else
                {
                    comboBox4.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load subjects: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Loads all timetable entries into dataGridView1.
        // Configures column headers, visibility and order.
        private void LoadGrid()
        {
            try
            {
                var timetables = controller.GetAllTimetables();
                dataGridView1.DataSource = timetables;

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ReadOnly = true;

                // Configure column headers for user friendliness
                dataGridView1.Columns["TimetableId"].HeaderText = "Timetable Id";
                dataGridView1.Columns["Date"].HeaderText = "Date";
                dataGridView1.Columns["TimeSlot"].HeaderText = "Time Slot";
                dataGridView1.Columns["GroupName"].HeaderText = "Group";
                dataGridView1.Columns["LecturerName"].HeaderText = "Lecturer";
                dataGridView1.Columns["SubjectName"].HeaderText = "Subject";
                dataGridView1.Columns["RoomName"].HeaderText = "Room";

                // Set column display order
                dataGridView1.Columns["TimetableId"].DisplayIndex = 0;
                dataGridView1.Columns["Date"].DisplayIndex = 1;
                dataGridView1.Columns["TimeSlot"].DisplayIndex = 2;
                dataGridView1.Columns["GroupName"].DisplayIndex = 3;
                dataGridView1.Columns["LecturerName"].DisplayIndex = 4;
                dataGridView1.Columns["SubjectName"].DisplayIndex = 5;
                dataGridView1.Columns["RoomName"].DisplayIndex = 6;

                // Hide internal ID columns and unused fields
                dataGridView1.Columns["SubjectId"].Visible = false;
                dataGridView1.Columns["RoomId"].Visible = false;
                dataGridView1.Columns["TimeSlotId"].Visible = false;
                dataGridView1.Columns["StartTime"].Visible = false;
                dataGridView1.Columns["EndTime"].Visible = false;
                dataGridView1.Columns["GroupId"].Visible = false;
                dataGridView1.Columns["LecturerId"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load timetable grid: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Adds a new timetable entry after validating selections and checking for conflicts.
        // Resets form controls on success.
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate ComboBox selections
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

                // Check for scheduling conflicts
                if (!controller.IsTimeSlotAvailable(timeSlotId, date, roomId, lecturerId))
                {
                    MessageBox.Show("Conflict: Either the room or lecturer is already booked for the selected date and time slot.", "Schedule Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                // Reset selections for next entry
                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;
                comboBox3.DataSource = null;
                comboBox4.DataSource = null;
                comboBox5.SelectedIndex = -1;
                dtDate.Value = DateTime.Today;

                MessageBox.Show("Timetable added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add timetable: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event handler for grid cell click: Loads selected timetable entry data into the form controls for editing.
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load selected timetable entry: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Updates the selected timetable entry after validation and conflict checks.
        // Resets form controls on success.
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedTimetableId == -1)
                {
                    MessageBox.Show("Please select a timetable entry to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate selections
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

                // Conflict check ignoring current timetable entry
                if (!controller.IsTimeSlotAvailable(timeSlotId, date, roomId, lecturerId, selectedTimetableId))
                {
                    MessageBox.Show("Conflict: Either the room or lecturer is already booked for the selected time slot.", "Schedule Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Timetable t = new Timetable
                {
                    TimetableId = selectedTimetableId, // Important for update
                    TimeSlotId = timeSlotId,
                    Date = date,
                    GroupId = groupId,
                    LecturerId = lecturerId,
                    SubjectId = subjectId,
                    RoomId = roomId
                };

                controller.UpdateTimetable(t);
                LoadGrid();

                // Reset form controls after update
                selectedTimetableId = -1;
                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;
                comboBox3.DataSource = null;
                comboBox4.DataSource = null;
                comboBox5.SelectedIndex = -1;
                dtDate.Value = DateTime.Today;

                MessageBox.Show("Timetable updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update timetable: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Deletes the selected timetable entry after confirmation.
        // Refreshes the grid and resets form controls.
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedTimetableId == -1)
                {
                    MessageBox.Show("Please select a timetable entry to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show("Are you sure you want to delete the selected timetable entry?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    controller.DeleteTimetable(selectedTimetableId);
                    LoadGrid();

                    // Reset form after delete
                    selectedTimetableId = -1;
                    comboBox1.SelectedIndex = -1;
                    comboBox2.SelectedIndex = -1;
                    comboBox3.DataSource = null;
                    comboBox4.DataSource = null;
                    comboBox5.SelectedIndex = -1;
                    dtDate.Value = DateTime.Today;

                    MessageBox.Show("Timetable deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete timetable: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Form Load event handler (currently unused).
        private void CreateTimetable_Load(object sender, EventArgs e)
        {
            // No code needed here currently
        }

        // Closes the CreateTimetable form.
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
