using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicomTicManagementSystem.View;
using System.Windows.Forms;


namespace UnicomTicManagementSystem.Repositories
{
    // Responsible for initializing all database tables and triggering admin registration if needed.
    internal class DataBaseInitializer
    {
        // Creates all necessary tables in the database if they don't already exist.
        public void InitializeTable()
        {
            try
            {
                using (SQLiteConnection dbcon = DataBaseCon.Connection())
                {
                    string initializeTableQuery = @"
                        CREATE TABLE IF NOT EXISTS User (
                            UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                            UserName TEXT NOT NULL, 
                            Password TEXT NOT NULL,
                            UserType TEXT NOT NULL
                        );

                        CREATE TABLE IF NOT EXISTS Course (
                            CourseId INTEGER PRIMARY KEY AUTOINCREMENT,
                            CourseName TEXT NOT NULL
                        );

                        CREATE TABLE IF NOT EXISTS Groups (
                            GroupId INTEGER PRIMARY KEY AUTOINCREMENT,
                            GroupName TEXT NOT NULL
                        );

                        CREATE TABLE IF NOT EXISTS Staff (
                            StaffId INTEGER PRIMARY KEY AUTOINCREMENT,
                            StaffName TEXT NOT NULL,
                            UserId INTEGER,
                            FOREIGN KEY (UserId) REFERENCES User(UserId)
                            
                        );

                        CREATE TABLE IF NOT EXISTS Subjects (
                            SubjectId INTEGER PRIMARY KEY AUTOINCREMENT,
                            SubjectName TEXT NOT NULL,
                            CourseId INTEGER,
                            FOREIGN KEY (CourseId) REFERENCES Course(CourseId)
                        );

                        CREATE TABLE IF NOT EXISTS Lecturer (
                            LecturerId INTEGER PRIMARY KEY AUTOINCREMENT,
                            LecturerName TEXT NOT NULL,
                            UserID INTEGER,                            
                            GroupId INTEGER,
                            FOREIGN KEY (UserID) REFERENCES User(UserId),                            
                            FOREIGN KEY (GroupId) REFERENCES Groups(GroupId)
                        );

                        CREATE TABLE IF NOT EXISTS Student (
                            StudentId INTEGER PRIMARY KEY AUTOINCREMENT,
                            StudentName TEXT NOT NULL,
                            UserId INTEGER,
                            CourseId INTEGER,
                            GroupId INTEGER,
                            FOREIGN KEY (UserId) REFERENCES User(UserId),
                            FOREIGN KEY (CourseId) REFERENCES Course(CourseId),
                            FOREIGN KEY (GroupId) REFERENCES Groups(GroupId)
                        );

                        CREATE TABLE IF NOT EXISTS Exam (
                            ExamId INTEGER PRIMARY KEY AUTOINCREMENT,
                            ExamName TEXT NOT NULL,
                            SubjectId INTEGER,
                            FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId)
                        );

                       CREATE TABLE IF NOT EXISTS Marks (
                            MarkId INTEGER PRIMARY KEY AUTOINCREMENT,
                            StudentId INTEGER,
                            ExamId INTEGER,
                            Score INTEGER NULL,
                            FOREIGN KEY (StudentId) REFERENCES Student(StudentId),
                            FOREIGN KEY (ExamId) REFERENCES Exam(ExamId)
                        );


                        CREATE TABLE IF NOT EXISTS Room (
                            RoomId INTEGER PRIMARY KEY AUTOINCREMENT,
                            RoomName TEXT NOT NULL, 
                            RoomType TEXT NOT NULL 
                        );

                        CREATE TABLE IF NOT EXISTS TimeSlot (
                            TimeSlotId INTEGER PRIMARY KEY AUTOINCREMENT,
                            StartTime TEXT NOT NULL,
                            EndTime TEXT NOT NULL
                        );

                        CREATE TABLE IF NOT EXISTS Timetable (
                            TimetableId INTEGER PRIMARY KEY AUTOINCREMENT,
                            SubjectId INTEGER,
                            RoomId INTEGER,
                            TimeSlotId INTEGER,
                            LecturerId INTEGER,
                            Date TEXT,
                            GroupId INTEGER,
                            FOREIGN KEY (GroupId) REFERENCES Groups(GroupId),
                            FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId),
                            FOREIGN KEY (TimeSlotId) REFERENCES TimeSlot(TimeSlotId),
                            FOREIGN KEY (RoomId) REFERENCES Room(RoomId),
                            FOREIGN KEY (LecturerId) REFERENCES Lecturer(LecturerId) 
                        );

                        CREATE TABLE IF NOT EXISTS Attendance (
                            TimetableId INTEGER,
                            StudentId INTEGER,
                            Status TEXT NOT NULL,
                            FOREIGN KEY (TimetableId) REFERENCES Timetable(TimetableId),
                            FOREIGN KEY (StudentId) REFERENCES Student(StudentId)

                        ); 
                        

                        CREATE TABLE IF NOT EXISTS LecturerCourse (
                            LecturerId INTEGER,
                            CourseId INTEGER,
                            PRIMARY KEY (LecturerId, CourseId),
                            FOREIGN KEY (LecturerId) REFERENCES Lecturer(LecturerId),
                            FOREIGN KEY (CourseId) REFERENCES Course(CourseId)
                        
                        );";


                    SQLiteCommand CreateTable = new SQLiteCommand(initializeTableQuery, dbcon);
                    CreateTable.ExecuteNonQuery();


                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error creating database tables:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        // Launches admin registration form if no user records exist and run login form
        public static void AdminRegistration()
        {
            try
            {
                using (SQLiteConnection conn = DataBaseCon.Connection())
                {

                    string checkQuery = "SELECT COUNT(*) FROM User";
                    using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        long count = (long)checkCmd.ExecuteScalar();
                        if (count == 0)
                        {
                            //When registered successfully it also runs login form
                            Application.Run(new AdminRegisterForm());
                        }
                        else
                        {
                            // Start the application with the login form
                            Application.Run(new LoginForm());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to verify admin account:" + ex.Message, "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }


    }
}
