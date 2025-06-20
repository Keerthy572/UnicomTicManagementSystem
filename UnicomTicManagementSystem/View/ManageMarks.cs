using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.View
{
    public partial class ManageMarks : Form
    {
        private MarkController markController = new MarkController();
        private List<Exam> exams = new List<Exam>();

        public ManageMarks()
        {
            InitializeComponent();

            try
            {
                LoadExams();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load exams: " + ex.Message);
            }
        }

        // Load exams into the ComboBox for selection
        private void LoadExams()
        {
            exams = markController.GetExams();

            comboBox1.DisplayMember = "examName";
            comboBox1.ValueMember = "examId";
            comboBox1.DataSource = exams;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            // Optionally, select first exam if exists
            if (exams.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        // When exam selection changes, load student marks for the selected exam
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue is int examId)
                {
                    var studentMarks = markController.GetStudentsWithMarks(examId);

                    dataGridView1.DataSource = studentMarks;
                    dataGridView1.AllowUserToAddRows = false;

                    // Set readonly for student info columns to prevent accidental edits
                    if (dataGridView1.Columns.Contains("StudentId"))
                        dataGridView1.Columns["StudentId"].ReadOnly = true;

                    if (dataGridView1.Columns.Contains("StudentName"))
                        dataGridView1.Columns["StudentName"].ReadOnly = true;

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load marks: " + ex.Message);
            }
        }

        // Add or update marks from DataGridView for selected exam
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue is int examId)
                {
                    markController.AddOrUpdateMarks(examId, dataGridView1);
                    MessageBox.Show("Marks updated successfully.");
                }
                else
                {
                    MessageBox.Show("Please select an exam before updating marks.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update marks: " + ex.Message);
            }
        }

        // Delete all marks for selected exam after confirmation
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue is int examId)
                {
                    var confirmResult = MessageBox.Show(
                        "Are you sure you want to delete all marks for this exam?",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        markController.DeleteMarks(examId);
                        dataGridView1.DataSource = markController.GetStudentsWithMarks(examId);
                        MessageBox.Show("Marks deleted successfully.");
                    }
                }
                else
                {
                    MessageBox.Show("Please select an exam before deleting marks.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete marks: " + ex.Message);
            }
        }
      

        // Close the form when button1 is clicked
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
