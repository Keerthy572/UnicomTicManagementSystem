using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicomTicManagementSystem.Repositories
{
    internal class DataBaseInitializer
    {
        public void InitializeTable()
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




                        CREATE TABLE IF NOT EXISTS LecturerStudent (
                            LecturerId INTEGER,
                            StudentId INTEGER,
                            PRIMARY KEY (StudentId, LecturerId),
                            FOREIGN KEY (StudentId) REFERENCES Student(StudentId),
                            FOREIGN KEY (LecturerId) REFERENCES Lecturer(LecturerId)
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
                        
                        CREATE TABLE IF NOT EXISTS CourseSubject (
                            CourseId INTEGER,
                            SubjectId INTEGER,
                            PRIMARY KEY (CourseId, SubjectId),
                            FOREIGN KEY (CourseId) REFERENCES Course(CourseId),
                            FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId)
                        
                        );

                        CREATE TABLE IF NOT EXISTS StudentSubject (
                            StudentId INTEGER,
                            SubjectId INTEGER,
                            PRIMARY KEY (StudentId, SubjectId),
                            FOREIGN KEY (StudentId) REFERENCES Student(StudentId),
                            FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId)
                        
                        );

                        CREATE TABLE IF NOT EXISTS LecturerCourse (
                            LecturerId INTEGER,
                            CourseId INTEGER,
                            PRIMARY KEY (LecturerId, CourseId),
                            FOREIGN KEY (LecturerId) REFERENCES Lecturer(LecturerId),
                            FOREIGN KEY (CourseId) REFERENCES Course(CourseId)
                        
                        );

                        


                        CREATE TABLE IF NOT EXISTS StudentTimetable (
                            StudentId INTEGER,
                            TimetableId INTEGER,
                            PRIMARY KEY (StudentId, TimetableId),
                            FOREIGN KEY (StudentId) REFERENCES Student(StudentId),
                            FOREIGN KEY (TimetableId) REFERENCES Timetable(TimetableId)
                        );";

                SQLiteCommand CreateTable = new SQLiteCommand(initializeTableQuery, dbcon);
                CreateTable.ExecuteNonQuery();


            }
        }
        
    }
}
