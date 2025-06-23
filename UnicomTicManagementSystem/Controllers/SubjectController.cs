using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class SubjectController
    {
        // Adds a new subject to a specific course
        public string AddSubject(Subject sub, Course course)
        {
            if (string.IsNullOrWhiteSpace(sub.subjectName))
                return "Subject name cannot be empty.";

            if (course == null || course.courseId <= 0)
                return "Invalid course selected.";

            try
            {
                using (var Dbcon = DataBaseCon.Connection())
                {
                    string addSubjectQuery = "INSERT INTO Subjects(SubjectName, CourseId) VALUES (@subjectname, @CourseId);";
                    SQLiteCommand addSubjectCommand = new SQLiteCommand(addSubjectQuery, Dbcon);
                    addSubjectCommand.Parameters.AddWithValue("@subjectname", sub.subjectName.Trim());
                    addSubjectCommand.Parameters.AddWithValue("@CourseId", course.courseId);
                    addSubjectCommand.ExecuteNonQuery();

                    return "Subject added successfully into course.";
                }
            }
            catch (Exception ex)
            {
                return "Error adding subject: " + ex.Message;
            }
        }

        // Retrieves all subjects with their course names
        public List<Subject> GetAllSubjects()
        {
            List<Subject> subjectList = new List<Subject>();

            try
            {
                using (var Dbcon = DataBaseCon.Connection())
                {
                    string getSubjectsQuery = @"
                        SELECT s.SubjectId, c.CourseName, s.CourseId, s.SubjectName
                        FROM Subjects s
                        LEFT JOIN Course c ON s.CourseId = c.CourseId
                        ORDER BY c.CourseName, s.SubjectName;";

                    SQLiteCommand getSubjectsCommand = new SQLiteCommand(getSubjectsQuery, Dbcon);
                    var reader = getSubjectsCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        Subject subject = new Subject
                        {
                            subjectId = reader.GetInt32(0),
                            courseName = reader.GetString(1),
                            courseId = reader.GetInt32(2),
                            subjectName = reader.GetString(3)
                        };

                        subjectList.Add(subject);
                    }

                    return subjectList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving subjects: " + ex.Message);
                return new List<Subject>();
            }
        }

        // Updates subject details
        public string UpdateSubject(Subject subject)
        {
            if (subject == null || subject.subjectId <= 0)
                throw new Exception("Invalid subject selected.");
            if (string.IsNullOrWhiteSpace(subject.subjectName))
                throw new Exception("Subject name cannot be empty.");

            try
            {
                using (var Dbcon = DataBaseCon.Connection())
                {
                    string updateQuery = "UPDATE Subjects SET SubjectName = @name, CourseId = @courseId WHERE SubjectId = @id;";
                    SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, Dbcon);
                    updateCommand.Parameters.AddWithValue("@name", subject.subjectName.Trim());
                    updateCommand.Parameters.AddWithValue("@courseId", subject.courseId);
                    updateCommand.Parameters.AddWithValue("@id", subject.subjectId);
                    updateCommand.ExecuteNonQuery();

                    return "Subject updated successfully.";
                }
            }
            catch (Exception ex)
            {
                return "Error updating subject: " + ex.Message;
            }
        }

        // Deletes a subject by ID
        public string DeleteSubject(int subjectId)
        {
            if (subjectId <= 0)
                throw new Exception("Invalid subject selected.");

            try
            {
                using (var Dbcon = DataBaseCon.Connection())
                {
                    string deleteQuery = "DELETE FROM Subjects WHERE SubjectId = @id;";
                    SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, Dbcon);
                    deleteCommand.Parameters.AddWithValue("@id", subjectId);
                    deleteCommand.ExecuteNonQuery();

                    return "Subject deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                return "Error deleting subject: " + ex.Message;
            }
        }
    }
}
