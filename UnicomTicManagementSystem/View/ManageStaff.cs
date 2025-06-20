using System;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.View;

namespace UnicomTicManagementSystem.Views
{
    public partial class ManageStaff : Form
    {
        private AdminDashboard adminDashboard;
        private StaffController controller = new StaffController();
        private int selectedStaffId = 0;
        private int selectedUserId = 0;

        public ManageStaff(AdminDashboard dashboard)
        {
            InitializeComponent();
            adminDashboard = dashboard;
        }

        // Load staff records on form load
        private void ManageStaff_Load(object sender, EventArgs e)
        {
            LoadStaff();
        }

        // Fetch and bind all staff data to the DataGridView
        private void LoadStaff()
        {
            try
            {
                dataGridView1.DataSource = controller.GetAllStaff();
                dataGridView1.ReadOnly = true;
                dataGridView1.Columns["userId"].Visible = false;
                dataGridView1.Columns["staffId"].Visible = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load staff data: " + ex.Message);
            }
        }

        // Handle selection of a staff row for editing/deletion
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    selectedStaffId = Convert.ToInt32(row.Cells["staffId"].Value);
                    selectedUserId = Convert.ToInt32(row.Cells["userId"].Value);

                    textBox1.Text = row.Cells["staffName"].Value.ToString();
                    textBox2.Text = row.Cells["userName"].Value.ToString();
                    textBox3.Text = row.Cells["password"].Value.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load selected staff data: " + ex.Message);
                }
            }
        }

        // Add new staff
        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            Staff s = new Staff
            {
                staffName = textBox1.Text.Trim()
            };

            try
            {
                controller.AddStaff(s, textBox2.Text.Trim(), textBox3.Text.Trim());
                MessageBox.Show("Staff added successfully.");
                ClearInputs();
                LoadStaff();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add staff: " + ex.Message);
            }
        }

        // Update existing staff
        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedStaffId == 0 || selectedUserId == 0)
            {
                MessageBox.Show("Please select a staff record to update.");
                return;
            }

            if (!ValidateInput()) return;

            Staff s = new Staff
            {
                staffId = selectedStaffId,
                userId = selectedUserId,
                staffName = textBox1.Text.Trim()
            };

            try
            {
                controller.UpdateStaff(s, textBox2.Text.Trim(), textBox3.Text.Trim());
                MessageBox.Show("Staff updated successfully.");
                ClearInputs();
                LoadStaff();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update staff: " + ex.Message);
            }
        }

        // Delete selected staff
        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedStaffId == 0 || selectedUserId == 0)
            {
                MessageBox.Show("Please select a staff record to delete.");
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this staff?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.No) return;

            try
            {
                controller.DeleteStaff(selectedStaffId, selectedUserId);
                MessageBox.Show("Staff deleted successfully.");
                ClearInputs();
                LoadStaff();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete staff: " + ex.Message);
            }
        }

        // Validate input fields before database operations
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter staff name.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter username.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Please enter password.");
                return false;
            }

            return true;
        }

        // Clear form inputs and reset selected IDs
        private void ClearInputs()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            selectedStaffId = 0;
            selectedUserId = 0;
        }

        // Navigate back to ManageUsers form
        private void button4_Click(object sender, EventArgs e)
        {
            adminDashboard.LoadFormInAdminPanel(new ManageUsers(adminDashboard));
        }
    }
}
