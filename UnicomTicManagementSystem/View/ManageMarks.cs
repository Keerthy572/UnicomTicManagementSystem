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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UnicomTicManagementSystem.View
{
    public partial class ManageMarks : Form
    {
        private MarkController markController = new MarkController();
        private List<Exam> exams = new List<Exam>();

        public ManageMarks()
        {
            InitializeComponent();
            LoadExams();
        }

        private void LoadExams()
        {
            exams = markController.GetExams();
            comboBox1.DisplayMember = "examName";
            comboBox1.ValueMember = "examId";
            comboBox1.DataSource = exams;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue is int examId)
            {
                dataGridView1.DataSource = markController.GetStudentsWithMarks(examId);
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.Columns["StudentId"].ReadOnly = true;
                dataGridView1.Columns["StudentName"].ReadOnly = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                int examId = Convert.ToInt32(comboBox1.SelectedValue);
                markController.AddOrUpdateMarks(examId, dataGridView1);
               
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
                int examId = Convert.ToInt32(comboBox1.SelectedValue);
                if (MessageBox.Show("Are you sure to delete all marks for this exam?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    markController.DeleteMarks(examId);
                    dataGridView1.DataSource = markController.GetStudentsWithMarks(examId);
                    MessageBox.Show("Marks deleted.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ManageMarks_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
