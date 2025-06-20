using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Main;
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
            LoadCourses();
            LoadExams();
            comboBoxCourse.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxSubject.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LoadCourses()
        {
            comboBoxCourse.DisplayMember = "Value";
            comboBoxCourse.ValueMember = "Key";
            comboBoxCourse.DataSource = new BindingSource(controller.GetAllCourses().ToDictionary(x => x.courseId, x => x.courseName), null);
            comboBoxCourse.SelectedIndex = -1;
            comboBoxSubject.DataSource = null; // Clear subjects initially
        }

        private void LoadSubjects(int courseId)
        {
            var subjects = controller.GetSubjectsByCourse(courseId);
            comboBoxSubject.DisplayMember = "Value";
            comboBoxSubject.ValueMember = "Key";
            comboBoxSubject.DataSource = new BindingSource(subjects.ToDictionary(x => x.subjectId, x => x.subjectName), null);
        }

        private void LoadExams()
        {
            var exams = controller.GetAllExams();
            dataGridView1.DataSource = exams;
            dataGridView1.Columns["examId"].Visible = false;
            dataGridView1.Columns["subjectId"].Visible = false;
            dataGridView1.Columns["courseId"].Visible = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void comboBoxCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCourse.SelectedValue is int courseId)
            {
                LoadSubjects(courseId);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
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

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBoxExamName.Text))
                    throw new Exception("Please enter exam name.");

                if (comboBoxCourse.SelectedValue == null || comboBoxSubject.SelectedValue == null)
                    throw new Exception("Please select course and subject.");

                string examName = textBoxExamName.Text.Trim();
                int subjectId = (int)comboBoxSubject.SelectedValue;

                if (controller.ExamExists(examName, subjectId))
                    throw new Exception("An exam with the same name already exists for the selected course and subject.");

                controller.AddExam(examName, subjectId);

                MessageBox.Show("Exam added successfully.");
                LoadExams();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedExamId == 0)
                    throw new Exception("Please select an exam to update.");

                if (string.IsNullOrWhiteSpace(textBoxExamName.Text))
                    throw new Exception("Please enter exam name.");

                if (comboBoxCourse.SelectedValue == null || comboBoxSubject.SelectedValue == null)
                    throw new Exception("Please select course and subject.");

                string examName = textBoxExamName.Text.Trim();
                int subjectId = (int)comboBoxSubject.SelectedValue;

                if (controller.ExamExists(examName, subjectId, selectedExamId))
                    throw new Exception("Another exam with the same name already exists for the selected course and subject.");

                controller.UpdateExam(selectedExamId, examName, subjectId);

                MessageBox.Show("Exam updated successfully.");
                LoadExams();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedExamId == 0)
                    throw new Exception("Please select an exam to delete.");

                var confirm = MessageBox.Show("Are you sure you want to delete this exam?", "Confirm Delete", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    controller.DeleteExam(selectedExamId);
                    MessageBox.Show("Exam deleted successfully.");
                    LoadExams();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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
