using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.View
{
    public partial class MarkAttendance : Form
    {
        TimetableController timetableController = new TimetableController();
        AttendanceController attendanceController = new AttendanceController();

        private int selectedTimetableId;
        private int selectedGroupId;
        

        public MarkAttendance()
        {
            InitializeComponent();
            LoadGrid();
        }

        // Loads timetables into dataGridView1 depending on the user type (admin or lecturer)
        private void LoadGrid()
        {
            try
            {
                List<Timetable> timetables = new List<Timetable>();
                if (Dashboard.userType == "lecturer")
                {
                    timetables = timetableController.GetTimetablesByLecturerUserId(Dashboard.userId);
                }
                else
                {
                    timetables = timetableController.GetAllTimetables();
                }
                    
                dataGridView1.DataSource = timetables;
                if (timetables.Count == 0)
                {
                    MessageBox.Show("Not any timetables created yet. Have to create timetables to mark attendance");
                    return;
                }


                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ReadOnly = true;

                dataGridView1.Columns["TimetableId"].HeaderText = "Timetable Id";
                dataGridView1.Columns["Date"].HeaderText = "Date";
                dataGridView1.Columns["TimeSlot"].HeaderText = "Time Slot";
                dataGridView1.Columns["GroupName"].HeaderText = "Group";
                dataGridView1.Columns["LecturerName"].HeaderText = "Lecturer";
                dataGridView1.Columns["SubjectName"].HeaderText = "Subject";
                dataGridView1.Columns["RoomName"].HeaderText = "Room";

                dataGridView1.Columns["SubjectId"].Visible = false;
                dataGridView1.Columns["RoomId"].Visible = false;
                dataGridView1.Columns["TimeSlotId"].Visible = false;
                dataGridView1.Columns["StartTime"].Visible = false;
                dataGridView1.Columns["EndTime"].Visible = false;
                dataGridView1.Columns["GroupId"].Visible = false;
                dataGridView1.Columns["LecturerId"].Visible = false;

                dataGridView1.CellClick += DataGridView1_CellClick;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load timetable grid: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // When a timetable is selected, load the students related, into dataGridView2.
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    selectedTimetableId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["TimetableId"].Value);
                    selectedGroupId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["GroupId"].Value);

                    LoadStudentsWithStatus();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("failed to select timetable : " + ex.Message, "Error", MessageBoxButtons.OK);
                }                
            }
        }

        // Loads the students of the selected group and their attendance status
        private void LoadStudentsWithStatus()
        {
            try
            {
                dataGridView2.Columns.Clear();
                var data = attendanceController.GetStudentsWithAttendance(selectedGroupId, selectedTimetableId);

                DataTable dt = new DataTable();
                dt.Columns.Add("StudentId", typeof(int));
                dt.Columns.Add("StudentName", typeof(string));
                dt.Columns.Add("Status", typeof(string));

                foreach (var item in data)
                {
                    dt.Rows.Add(item.StudentId, item.StudentName, item.Status);
                }

                dataGridView2.DataSource = dt;
                dataGridView2.AllowUserToAddRows = false;
                dataGridView2.Columns["StudentName"].ReadOnly = true;
                dataGridView2.Columns["StudentId"].Visible = false;

                DataGridViewComboBoxColumn statusCol = new DataGridViewComboBoxColumn();
                statusCol.DataPropertyName = "Status";
                statusCol.HeaderText = "Status";
                statusCol.Items.AddRange("Present", "Absent", "Late", "Excused");
                statusCol.FlatStyle = FlatStyle.Flat;

                int statusColIndex = dataGridView2.Columns["Status"].Index;
                dataGridView2.Columns.Remove("Status");
                statusCol.Name = "Status"; // <- This line fixes the error
                dataGridView2.Columns.Insert(statusColIndex, statusCol);


                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load student list.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Saves or updates attendance for all listed students in the selected timetable
        private void btnAddUpdate_Click(object sender, EventArgs e)
        {
            if (selectedTimetableId == 0)
            {
                MessageBox.Show("Select a timetable first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var attendanceList = new List<Attendance>();

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.IsNewRow) continue;
                    attendanceList.Add(new Attendance
                    {
                        TimetableId = selectedTimetableId,
                        StudentId = Convert.ToInt32(row.Cells["StudentId"].Value),
                        Status = row.Cells["Status"].Value?.ToString()
                    });
                }
                attendanceController.AddOrUpdateAttendance(attendanceList);
                MessageBox.Show("Attendance saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save attendance." + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Deletes attendance records for the selected timetable.
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedTimetableId == 0)
            {
                MessageBox.Show("Select a timetable first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                attendanceController.DeleteAttendance(selectedTimetableId);
                MessageBox.Show("Attendance deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadStudentsWithStatus();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete attendance." + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Closes the attendance form.
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
