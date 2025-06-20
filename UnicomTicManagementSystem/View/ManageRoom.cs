using System;
using System.Data;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Main;

namespace UnicomTicManagementSystem.Forms
{
    public partial class ManageRoom : Form
    {
        // Controller instance for room operations
        RoomController controller = new RoomController();

        // To keep track of selected room for update/delete
        int selectedRoomId = 0;

        public ManageRoom()
        {
            InitializeComponent();

            // Populate ComboBox with room types
            comboBox1.Items.Add("Lecture Hall");
            comboBox1.Items.Add("Lab");
            comboBox1.SelectedIndex = -1;
        }

        // Load rooms when the form loads
        private void ManageRoom_Load(object sender, EventArgs e)
        {
            LoadRooms();
        }

        // Loads all rooms into the DataGridView
        private void LoadRooms()
        {
            try
            {
                dataGridView1.DataSource = controller.GetAllRooms();
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.Columns["roomId"].Visible = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rooms: " + ex.Message);
            }
        }

        // When a row is selected, populate the form fields
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                selectedRoomId = row.Cells["roomId"].Value != DBNull.Value
                    ? Convert.ToInt32(row.Cells["roomId"].Value)
                    : 0;

                textBox1.Text = row.Cells["roomName"].Value?.ToString() ?? "";
                comboBox1.Text = row.Cells["roomType"].Value?.ToString() ?? "";
            }
        }

        // Add room button
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) || comboBox1.SelectedIndex == -1)
                    throw new Exception("Please enter a room name and select a room type.");

                Room room = new Room
                {
                    roomName = textBox1.Text.Trim(),
                    roomType = comboBox1.Text
                };

                controller.AddRoom(room);
                MessageBox.Show("Room added successfully.");
                LoadRooms();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding room: " + ex.Message);
            }
        }

        // Update room button
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedRoomId == 0)
                    throw new Exception("Please select a room to update.");

                if (string.IsNullOrWhiteSpace(textBox1.Text) || comboBox1.SelectedIndex == -1)
                    throw new Exception("Please enter a room name and select a room type.");

                Room room = new Room
                {
                    roomId = selectedRoomId,
                    roomName = textBox1.Text.Trim(),
                    roomType = comboBox1.Text
                };

                controller.UpdateRoom(room);
                MessageBox.Show("Room updated successfully.");
                LoadRooms();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating room: " + ex.Message);
            }
        }

        // Delete room button
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedRoomId == 0)
                    throw new Exception("Please select a room to delete.");

                DialogResult confirm = MessageBox.Show("Are you sure you want to delete this room?", "Confirm", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    controller.DeleteRoom(selectedRoomId);
                    MessageBox.Show("Room deleted successfully.");
                    LoadRooms();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting room: " + ex.Message);
            }
        }

        // Clear input fields
        private void ClearFields()
        {
            textBox1.Clear();
            comboBox1.SelectedIndex = -1;
            selectedRoomId = 0;
        }

        // Close button
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
