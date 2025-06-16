using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Main;

namespace UnicomTicManagementSystem.View
{
    public partial class ManageGroups : Form
    {
        private int selectedGroupId = 0;
        private string selectedGroupName;

        public ManageGroups()
        {
            InitializeComponent();
            Load += ManageGroups_Load;
            dataGridView1.CellClick += dataGridView1_CellClick;
            button1.Click += ButtonAdd_Click;
            button2.Click += ButtonUpdate_Click;
            button3.Click += ButtonDelete_Click;
        }

        private void ManageGroups_Load(object sender, EventArgs e)
        {
            RefreshGroupGrid();
        }

        private void RefreshGroupGrid()
        {
            GroupController controller = new GroupController();
            List<Group> groups = controller.GetAllGroups();

            dataGridView1.DataSource = groups;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Enter a group name.");
                return;
            }

            GroupController controller = new GroupController();
            Group group = new Group { groupName = textBox1.Text };
            string result = controller.AddGroup(group);
            MessageBox.Show(result);
            textBox1.Clear();
            RefreshGroupGrid();
        }

        private void ButtonUpdate_Click(object sender, EventArgs e)
        {
            if (selectedGroupId == 0 || string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Select a group from the table and type a new name.");
                return;
            }

            GroupController controller = new GroupController();
            Group group = new Group { groupId = selectedGroupId, groupName = textBox1.Text };
            string result = controller.UpdateGroup(group);
            MessageBox.Show(result);
            textBox1.Clear();
            selectedGroupId = 0;
            RefreshGroupGrid();
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (selectedGroupId == 0)
            {
                MessageBox.Show("Select a group from the table to delete.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this group?", "Confirm", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                GroupController controller = new GroupController();
                string result = controller.DeleteGroup(selectedGroupId);
                MessageBox.Show(result);
                textBox1.Clear();
                selectedGroupId = 0;
                RefreshGroupGrid();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var group = (Group)dataGridView1.CurrentRow.DataBoundItem;
                if (group != null)
                {
                    selectedGroupId = group.groupId;
                    selectedGroupName = group.groupName;
                    textBox1.Text = group.groupName;
                }
            }
        }
    }
}
