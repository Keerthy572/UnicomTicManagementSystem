using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class SubjectController
    {
        public string AddSubject(Subject sub,Course course)
        {
            using(var Dbcon = DataBaseCon.Connection())
            {
                string addSubjectQuery = "INSERT INTO Subjects(SubjectName,CourseId) VALUES (@subjectname,@CourseId);";
                SQLiteCommand addSubjectCommand = new SQLiteCommand(addSubjectQuery,Dbcon);
                addSubjectCommand.Parameters.AddWithValue("@subjectname", sub.subjectName);
                addSubjectCommand.Parameters.AddWithValue("@CourseId",course.courseId);
                addSubjectCommand.ExecuteNonQuery();

                return "Subject added successfully into course";
            }
        }

        public List<Subject> GetAllSubjects()
        {
            List<Subject> subjectl = new List<Subject>();
            using (var Dbcon = DataBaseCon.Connection())
            {
                string getSubjectsQuery = @"SELECT s.subjectId,c.courseName, s.courseId,  s.subjectName
                                            FROM Subjects s
                                            LEFT JOIN Course c ON s.courseId = c.courseId
                                            ORDER BY c.courseName, s.subjectName;";

                SQLiteCommand getSubjectsCommand = new SQLiteCommand(getSubjectsQuery, Dbcon);

                var reader = getSubjectsCommand.ExecuteReader();
                while (reader.Read())
                {
                    Subject subject = new Subject();
                    subject.subjectId = reader.GetInt32(0);
                    subject.courseName = reader.GetString(1);
                    subject.courseId = reader.GetInt32(2);                    
                    subject.subjectName = reader.GetString(3);

                    subjectl.Add(subject);

                }

                return subjectl;
            }
        }

        public string UpdateSubject(Subject subject)
        {
            using (var Dbcon = DataBaseCon.Connection())
            {
                string updateQuery = "UPDATE Subjects SET SubjectName = @name, CourseId = @courseId WHERE SubjectId = @id;";
                SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, Dbcon);
                updateCommand.Parameters.AddWithValue("@name", subject.subjectName);
                updateCommand.Parameters.AddWithValue("@courseId", subject.courseId);
                updateCommand.Parameters.AddWithValue("@id", subject.subjectId);

                updateCommand.ExecuteNonQuery();
                return "Subject updated successfully";
            }
        }

        public string DeleteSubject(int subjectId)
        {
            using (var Dbcon = DataBaseCon.Connection())
            {
                string deleteQuery = "DELETE FROM Subjects WHERE SubjectId = @id;";
                SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, Dbcon);
                deleteCommand.Parameters.AddWithValue("@id", subjectId);

                deleteCommand.ExecuteNonQuery();
                return "Subject deleted successfully";
            }
        }

    }
}
