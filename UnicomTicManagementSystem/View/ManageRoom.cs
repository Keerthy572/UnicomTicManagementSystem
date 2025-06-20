using System;
using System.Data;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Main;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UnicomTicManagementSystem.Forms
{
    public partial class ManageRoom : Form
    {
        RoomController controller = new RoomController();
        int selectedRoomId = 0;

        public ManageRoom()
        {
            InitializeComponent();
            comboBox1.Items.Add("Lecture Hall");
            comboBox1.Items.Add("Lab");
            comboBox1.SelectedIndex = -1;
        }

        private void ManageRoom_Load(object sender, EventArgs e)
        {
            LoadRooms();
        }

        private void LoadRooms()
        {
            dataGridView1.DataSource = controller.GetAllRooms();
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns["roomId"].Visible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

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


        private void button1_Click(object sender, EventArgs e) // Add
        {
            try
            {
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
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e) // Update
        {
            try
            {
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
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e) // Delete
        {
            try
            {
                controller.DeleteRoom(selectedRoomId);
                MessageBox.Show("Room deleted successfully.");
                LoadRooms();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClearFields()
        {
            textBox1.Clear();
            comboBox1.SelectedIndex = -1;
            selectedRoomId = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
