using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnicomTicManagementSystem.Main;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    internal class CourseController
    {
        public string AddCourse(Course course)
        {
            using(var Dbcon = DataBaseCon.Connection())
            {
                string addCourseQuery= "INSERT INTO Course(CourseName) VALUES(@course); ";

                SQLiteCommand insert = new SQLiteCommand(addCourseQuery, Dbcon);
                insert.Parameters.AddWithValue("@course", course.courseName);
                insert.ExecuteNonQuery();
            }
            return "Course added successfully";
        }

        public List<Course> GetCourses()
        {
            List < Course > coursel = new List<Course>();
            using ( var Dbcon = DataBaseCon.Connection())
            {
                string getCoursesQuery = "SELECT * FROM Course";
                SQLiteCommand getCoursesCommand = new SQLiteCommand(getCoursesQuery, Dbcon);

                var reader = getCoursesCommand.ExecuteReader();
                while (reader.Read())
                {
                    Course course = new Course();
                    course.courseId = reader.GetInt32(0);
                    course.courseName = reader.GetString(1);
                    
                    coursel.Add(course);

                }

                return coursel;
            }
        }

        public string UpdateCourse(Course course)
        {
            using (var Dbcon = DataBaseCon.Connection())
            {
                string updateQuery = "UPDATE Course SET CourseName = @name  WHERE CourseId = @id;";
                SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, Dbcon);
                updateCommand.Parameters.AddWithValue("@name", course.courseName);
                updateCommand.Parameters.AddWithValue("@id", course.courseId);

                updateCommand.ExecuteNonQuery();

                return "Course updated successfully";
            }
        }


        public string DeleteCourse(Course course)
        {
            using (var Dbcon = DataBaseCon.Connection())
            {
                string deleteQuery = "DELETE From Course WHERE CourseId = @id;";
                SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, Dbcon);
                deleteCommand.Parameters.AddWithValue("@id", course.courseId);
                deleteCommand.ExecuteNonQuery();

                return "Course deleted successfully";
            }
        }



    }   
}
