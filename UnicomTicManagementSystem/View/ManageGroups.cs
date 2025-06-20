using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.View
{
    public partial class ManageGroups : Form
    {
        private int selectedGroupId = 0;
        private string selectedGroupName;

        public ManageGroups()
        {
            InitializeComponent();
        }

        // Load event - populate the group list on form load.
        private void ManageGroups_Load(object sender, EventArgs e)
        {
            RefreshGroupGrid();
        }

        // Loads all groups from the database and displays in the DataGridView.
        private void RefreshGroupGrid()
        {
            try
            {
                GroupController controller = new GroupController();
                List<Group> groups = controller.GetAllGroups();

                dataGridView1.DataSource = groups;
                dataGridView1.ReadOnly = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load groups.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handles the Add button click event to add a new group.
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Enter a group name.");
                return;
            }

            try
            {
                GroupController controller = new GroupController();
                Group group = new Group { groupName = textBox1.Text.Trim() };
                string result = controller.AddGroup(group);

                MessageBox.Show(result);
                textBox1.Clear();
                RefreshGroupGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add group.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handles the Update button click event to update a selected group.
        private void ButtonUpdate_Click(object sender, EventArgs e)
        {
            if (selectedGroupId == 0 || string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Select a group from the table and type a new name.");
                return;
            }

            try
            {
                GroupController controller = new GroupController();
                Group group = new Group { groupId = selectedGroupId, groupName = textBox1.Text.Trim() };
                string result = controller.UpdateGroup(group);

                MessageBox.Show(result);
                textBox1.Clear();
                selectedGroupId = 0;
                RefreshGroupGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update group.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handles the Delete button click event to delete a selected group.
        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (selectedGroupId == 0)
            {
                MessageBox.Show("Select a group from the table to delete.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this group?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    GroupController controller = new GroupController();
                    string result = controller.DeleteGroup(selectedGroupId);

                    MessageBox.Show(result);
                    textBox1.Clear();
                    selectedGroupId = 0;
                    RefreshGroupGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to delete group.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Handles DataGridView cell click to load selected group data into the input field.
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    var group = (Group)dataGridView1.CurrentRow.DataBoundItem;
                    if (group != null)
                    {
                        selectedGroupId = group.groupId;
                        selectedGroupName = group.groupName;
                        textBox1.Text = group.groupName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to select group.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Close the form
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
