using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UnicomTicManagementSystem.Forms
{
    public partial class ManageExams : Form
    {
        private ExamController controller = new ExamController();
        private int selectedExamId = 0;

        public ManageExams()
        {
            InitializeComponent();

            try
            {
                LoadCourses();
                LoadExams();

                comboBoxCourse.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBoxSubject.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        // Loads all courses into comboBoxCourse.       
        private void LoadCourses()
        {
            try
            {
                var courses = controller.GetAllCourses();
                comboBoxCourse.DisplayMember = "Value";
                comboBoxCourse.ValueMember = "Key";
                comboBoxCourse.DataSource = new BindingSource(courses.ToDictionary(x => x.courseId, x => x.courseName), null);
                comboBoxCourse.SelectedIndex = -1;
                comboBoxSubject.DataSource = null; // Clear subjects initially
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load courses: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        /// Loads subjects of the selected course into comboBoxSubject.                
        private void LoadSubjects(int courseId)
        {
            try
            {
                var subjects = controller.GetSubjectsByCourse(courseId);
                comboBoxSubject.DisplayMember = "Value";
                comboBoxSubject.ValueMember = "Key";
                comboBoxSubject.DataSource = new BindingSource(subjects.ToDictionary(x => x.subjectId, x => x.subjectName), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load subjects: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxSubject.DataSource = null;
            }
        }

        
        // Loads all exams into the DataGridView.       
        private void LoadExams()
        {
            try
            {
                var exams = controller.GetAllExams();
                dataGridView1.DataSource = exams;

                // Hide IDs for cleaner UI
                if (dataGridView1.Columns["examId"] != null) dataGridView1.Columns["examId"].Visible = false;
                if (dataGridView1.Columns["subjectId"] != null) dataGridView1.Columns["subjectId"].Visible = false;
                if (dataGridView1.Columns["courseId"] != null) dataGridView1.Columns["courseId"].Visible = false;

                dataGridView1.ReadOnly = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load exams: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBoxCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When course changes, load related subjects
            try
            {
                if (comboBoxCourse.SelectedValue is int courseId)
                {
                    LoadSubjects(courseId);
                }
                else
                {
                    comboBoxSubject.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading subjects: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Populate form inputs with selected exam details for update/delete
            try
            {
                if (e.RowIndex < 0) return;

                var row = dataGridView1.Rows[e.RowIndex];
                selectedExamId = Convert.ToInt32(row.Cells["examId"].Value);
                textBoxExamName.Text = row.Cells["examName"].Value.ToString();

                int courseId = Convert.ToInt32(row.Cells["courseId"].Value);
                int subjectId = Convert.ToInt32(row.Cells["subjectId"].Value);

                comboBoxCourse.SelectedValue = courseId;
                LoadSubjects(courseId);
                comboBoxSubject.SelectedValue = subjectId;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error selecting exam: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxCourse.Items.Count == 0 || comboBoxSubject.Items.Count == 0)
                {
                    MessageBox.Show("Courses or Subjects aren't created yet. Courses and Subjects have to be created  to add exam ");
                    return;
                }                
                if (string.IsNullOrWhiteSpace(textBoxExamName.Text))
                {
                    MessageBox.Show("Please enter exam name.");
                    return;
                }
                    

                if (comboBoxCourse.SelectedValue == null || comboBoxSubject.SelectedValue == null)
                {
                    MessageBox.Show("Please select course and subject.");
                    return;
                }
                    

                string examName = textBoxExamName.Text.Trim();
                int subjectId = (int)comboBoxSubject.SelectedValue;

                if (controller.ExamExists(examName, subjectId))
                {
                    MessageBox.Show("An exam with the same name already exists for the selected course and subject.");

                }

                controller.AddExam(examName, subjectId);

                MessageBox.Show("Exam added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadExams();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add exam: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedExamId == 0)
                {
                    MessageBox.Show("Please select an exam to update.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBoxExamName.Text))
                {
                    MessageBox.Show("Please enter exam name.");
                    return;
                }

                if (comboBoxCourse.SelectedValue == null || comboBoxSubject.SelectedValue == null)
                {
                    MessageBox.Show("Please select course and subject.");
                    return;
                }


                string examName = textBoxExamName.Text.Trim();
                int subjectId = (int)comboBoxSubject.SelectedValue;

                if (controller.ExamExists(examName, subjectId, selectedExamId))
                {
                    MessageBox.Show("Another exam with the same name already exists for the selected course and subject.");
                    return;
                }

                controller.UpdateExam(selectedExamId, examName, subjectId);

                MessageBox.Show("Exam updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadExams();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update exam: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedExamId == 0)
                {
                    MessageBox.Show("Please select an exam to delete.");
                    return;
                }

                var confirm = MessageBox.Show("Are you sure you want to delete this exam?", "Confirm Delete", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    controller.DeleteExam(selectedExamId);
                    MessageBox.Show("Exam deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadExams();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete exam: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        // Clears form inputs and resets selection state.      
        private void ClearForm()
        {
            textBoxExamName.Clear();
            comboBoxCourse.SelectedIndex = -1;
            comboBoxSubject.DataSource = null;
            selectedExamId = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
}
