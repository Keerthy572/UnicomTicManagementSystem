using System;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.View;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

        private void ManageStaff_Load(object sender, EventArgs e)
        {
            LoadStaff();
        }

        private void LoadStaff()
        {
            dataGridView1.DataSource = controller.GetAllStaff();
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["userId"].Visible = false;
            dataGridView1.Columns["staffId"].Visible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedStaffId = Convert.ToInt32(row.Cells["staffId"].Value);
                selectedUserId = Convert.ToInt32(row.Cells["userId"].Value);

                textBox1.Text = row.Cells["staffName"].Value.ToString();
                textBox2.Text = row.Cells["userName"].Value.ToString();
                textBox3.Text = row.Cells["password"].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e) // Add
        {
            if (!ValidateInput()) return;

            Staff s = new Staff
            {
                staffName = textBox1.Text
            };

            try
            {
                controller.AddStaff(s, textBox2.Text, textBox3.Text);
                MessageBox.Show("Staff added successfully.");
                ClearInputs();
                LoadStaff();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e) // Update
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
                staffName = textBox1.Text
            };

            try
            {
                controller.UpdateStaff(s, textBox2.Text, textBox3.Text);
                MessageBox.Show("Staff updated.");
                ClearInputs();
                LoadStaff();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e) // Delete
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
                MessageBox.Show("Staff deleted.");
                ClearInputs();
                LoadStaff();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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

        private void ClearInputs()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            selectedStaffId = 0;
            selectedUserId = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            adminDashboard.LoadFormInAdminPanel(new ManageUsers(adminDashboard));
        }
    }
}
