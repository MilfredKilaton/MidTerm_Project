using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;

namespace WorkSpace
{

    public class Student
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleInitial { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public string? Course { get; set; }
        public string? Year { get; set; }
        public List<Subject>? StudentSubjects { get; set; }
        public string? Status { get; set; }
    }

    public class Subject
    {
        public string? Name { get; set; }
        public int Id { get; set; }
        public decimal? Grade { get; set; }
    }

    public class DataStore
    {
        public static List<Student> StudentList = new List<Student>();
        public static List<Subject> SubjectList = new List<Subject>();
    }

    class Program
    {
        static void Main()
        {
            int choice = 1;

            while (true)
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);

                MainMenu();
                OptionHighlighter(choice);

                ConsoleKeyInfo input = Console.ReadKey(true);

                if (input.Key == ConsoleKey.UpArrow && choice > 1) choice--;
                else if (input.Key == ConsoleKey.DownArrow && choice < 5) choice++;
                else if (input.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    Console.CursorVisible = true;

                    switch (choice)
                    {
                        case 1:
                            Register.RegisterStudentProcess();
                        break;

                        case 5:
                            Console.WriteLine("Thank you for using this program. - The Dreamer");
                            return;

                        case var _ when DataStore.StudentList.Count == 0:
                            Console.WriteLine($"Error: {DataStore.StudentList.Count} students are registered. Please register first.");

                        break;

                        case 2:
                            Enroll.SubjectEnrollProcess();
                        break;

                        case 3:

                        break;

                        case 4:

                        break;
                    }

                    Console.WriteLine("\nPress any key to return to menu...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("╔═════════════════════════════╗");
            Console.WriteLine("║          MAIN MENU          ║");
            Console.WriteLine("╠═════════════════════════════╣");
            Console.WriteLine("║ 1. Register Student         ║");
            Console.WriteLine("║ 2. Enroll Student Subjects  ║");
            Console.WriteLine("║ 3. Enter Grades             ║");
            Console.WriteLine("║ 4. Show Grades by Student   ║");
            Console.WriteLine("║ 5. Exit                     ║");
            Console.WriteLine("╚═════════════════════════════╝");
        }

        static void OptionHighlighter(int choice)
        {
            string[] options = {
                "║ 1. Register Student         ║",
                "║ 2. Enroll Student Subjects  ║",
                "║ 3. Enter Grades             ║",
                "║ 4. Show Grades by Student   ║",
                "║ 5. Exit                     ║"
            };

            string[] description = {
                "Register students with their information",
                "Enroll student subjects depending on their course",
                "Enter student grades for enrolled subjects",
                "Show all grades for a specific student",
                "Terminate the program"
            };

            Console.SetCursorPosition(0, 2 + choice);

            Color(ConsoleColor.Black, ConsoleColor.White);
            Console.Write($"{options[choice - 1]} - {description[choice - 1]}".PadRight(Console.WindowWidth - 1));
            Console.ResetColor();
        }

        static void Color(ConsoleColor textColor, ConsoleColor backgroundColor)
        {
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;
        }
    }

    class Grades
    {
        public static void StudentGradeProcess()
        {
            Console.Clear();
            Console.WriteLine("---------- GRADING SYSTEM ----------");
            foreach (var student in DataStore.StudentList)
            {
                var gradedStudents = DataStore.StudentList.Where(student => student.Status == "Graded");
                var enrolledStudents = DataStore.StudentList.Where(student => student.Status == "Enrolled");

                string properName = $"{student.LastName}, {student.FirstName}";

                Console.WriteLine("Graded Students: ");
                foreach(var gradedStudent in gradedStudents)
                {
                    Console.WriteLine($"| ID: {student.Id, -8} | Name: {properName, -30} | Course: {student.Course, -4} |");
                }
                Console.WriteLine("-------------------------------------------------------");

                Console.WriteLine("To be graded Students: ");
                foreach (var enrolledStudent in enrolledStudents)
                {
                    Console.WriteLine($"| ID: {student.Id, -8} | Name: {properName, -30} | Course: {student.Course, -4} |");
                }


            }
        }
    }

    class Enroll
    {
        public static void SubjectEnrollProcess()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("---------- ENROLLING SYSTEM ----------");

                foreach (var student in DataStore.StudentList.Where(student => student.Status == "Registered"))
                {
                    string properName = $"{student.LastName}, {student.FirstName}";

                    Console.WriteLine($"| {properName, -36} |");
                    Console.WriteLine($"| {student.Id, -10} | {student.Course, -4} | {student.Year,-11} | {student.Age,-2} |");
                }

                Console.WriteLine("------------ Welcome to enrolling student subjects ------------");
                Console.Write("Enter student ID: ");
                string idToFind = Console.ReadLine() ?? string.Empty;

                var selectedStudent = DataStore.StudentList.FirstOrDefault(student => student.Id == idToFind && student.Status == "Registered");

                

                foreach (var student in selectedStudent?.Status!)
                {
                    Console.WriteLine($"{selectedStudent.FirstName} - {selectedStudent.Status}");
                }

                if (selectedStudent == null)
                {
                    Console.WriteLine($"No student with an ID of {idToFind} was found. . .");
                    return;
                }

                if (selectedStudent.Status == "Enrolled")
                {
                    Console.WriteLine($"This student has already been enrolled with the following subjects: ");
                    foreach (var subject in DataStore.SubjectList)
                    {
                        Console.WriteLine($"Subject ID: {subject.Id} | Subject Name: {subject.Name}");
                        Console.ReadKey();
                        return;
                    }
                }

                int index = DataStore.StudentList.FindIndex(student => student.Id == idToFind && student.Status == "Registered");

                Console.Clear();
                Console.WriteLine($"Student Found: {selectedStudent?.FirstName} {selectedStudent?.LastName}");
                Console.WriteLine($"Course: {selectedStudent?.Course} | Year: {selectedStudent?.Year}");
                Console.WriteLine($"---------------------------------------------------------");

                List<Subject> subjects = SubjectChecker(selectedStudent?.StudentSubjects!, selectedStudent?.Course!);

                DataStore.StudentList.Add(new Student { StudentSubjects = subjects, Status = "Enrolled" });

                selectedStudent?.Status!.Insert(index, "Enrolled");

                

                Console.WriteLine($"Student {selectedStudent?.LastName} has been enrolled with the following subjects:");
                foreach (var subject in subjects)
                {
                    Console.WriteLine($"Subject ID: {subject.Id} | Subject Name: {subject.Name}");
                }


                Console.Write("Try again? (Enter 'exit' to return): ");
                if (Console.ReadLine()?.ToLower() == "exit") break;
            }
            
        }

        static List<Subject> SubjectChecker(List<Subject> subject, string course)
        {
            subject = new List<Subject>();

            string[] bsitSubjects = { "PathFit 1", "Introduction to Computer", "The Works of Rizal", "HCI" };
            int[] bsitSubjectsId = { 101, 102, 103, 104 };

            string[] bsmeSubjects = { "PathFit 1", "Fluid Mechanics", "The Works of Rizal", "Thermodynamics" };
            int[] bsmeSubjectsId = { 201, 202, 203, 204 };

            switch (course.ToUpper()) // System.NullReferenceException: 'Object reference not set to an instance of an object.' course was null.
            {
                case "BSIT":
                    for (int i = 0; i < bsitSubjects.Length; i++)
                    {
                        subject.Add(new Subject { Name = bsitSubjects[i], Id = bsitSubjectsId[i] });
                    }
                    break;

                case "BSME":
                    for (int i = 0; i < bsmeSubjects.Length; i++)
                    {
                        subject.Add(new Subject { Name = bsmeSubjects[i], Id = bsmeSubjectsId[i] });
                    }
                    break;
            }

            return subject;
        }
    }


    class Register
    {
        public static void RegisterStudentProcess()
        {
            const string dateFormat = "MM/dd/yyyy";

            string newStudentId = string.Empty;
            string firstName = string.Empty;
            string lastName = string.Empty;
            string contactNumber = string.Empty;
            string address = string.Empty;
            string course = string.Empty;
            string year = string.Empty;
            string middleInitial = string.Empty;

            DateTime birthDate = DateTime.Now;

            int age = 0;

            while (true)
            {
                try
                {
                    Console.Clear();

                    Console.WriteLine("Enter student informations: ");

                    Console.Write("Unique ID: ");
                    while (true)
                    {
                        newStudentId = Console.ReadLine()?.ToLower() ?? string.Empty;

                        if (DataStore.StudentList.Any(student => student.Id == newStudentId))
                        {
                            Console.Write("You've inputted an existing ID.\n > ");
                        }
                        else if (string.IsNullOrWhiteSpace(newStudentId))
                        {
                            Console.Write("Fill out the information.\n > ");
                        }
                        else if (newStudentId.Length > 8)
                        {
                            Console.Write("The ID must not be 8 or more characters.\n > ");
                        }
                        else if (newStudentId.All(char.IsLetter) || Convert.ToInt32(newStudentId) < 1)
                        {
                            Console.Write("The ID must be integer only.\n > ");
                        }
                        else
                        {
                            break;
                        }
                    }

                    Console.Write("First Name: ");
                    firstName = Console.ReadLine() ?? string.Empty;

                    string input = "Place Holder";
                    do
                    {
                        Console.Write("Middle Initial (N/A if none): ");
                        input = Console.ReadLine()?.ToUpper() ?? string.Empty;

                    } while (input != "N/A" && !(input.Length == 1 && char.IsLetter(input[0])));

                    middleInitial = input;

                    Console.Write("Last Name: ");
                    lastName = Console.ReadLine() ?? string.Empty;

                    Console.Write("Address: ");
                    address = Console.ReadLine() ?? string.Empty;

                    do
                    {
                        Console.Write("11 Digits required.\nContact Number: ");
                        input = Console.ReadLine() ?? string.Empty;

                        contactNumber = input.Replace(" ", String.Empty);
                    } while (contactNumber.Length != 11);

                    while (true)
                    {
                        Console.Write("Date of Birth (MM/dd/yyyy): ");
                        while (!DateTime.TryParseExact(Console.ReadLine(), dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
                        {
                            Console.Write("Invalid Input. (MM/dd/yyyy): ");
                        }

                        age = DateTime.Today.Year - birthDate.Year;

                        if (birthDate.Date > DateTime.Today.AddYears(-age)) age--;

                        Console.Write($"Calculated age is {age}. Is this correct? (Y | N): ");
                        if (Console.ReadLine()?.ToLower() == "y") break;
                    }

                    Console.Write("Course (BSIT | BSME): ");
                    course = CourseChecker();

                    year = YearChecker();
                    while (true)
                    {
                        Console.Write($"You've inputted {year}. Is this correct? (Y/N): ");
                        string confirmation = Console.ReadLine()?.ToLower() ?? string.Empty;

                        if (confirmation == "y") break;
                        if (confirmation == "n") continue;

                        Console.WriteLine("Invalid input. Please type Y or N");
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected Error: {ex.Message}");
                }

                string jsonString = JsonSerializer.Serialize(new Student
                {
                    Id = newStudentId,
                    FirstName = firstName,
                    MiddleInitial = middleInitial,
                    LastName = lastName,
                    Age = age,
                    Address = address,
                    ContactNumber = contactNumber,
                    BirthDate = birthDate,
                    Course = course,
                    Year = year,
                    Status = "Registered"

                }, new JsonSerializerOptions { WriteIndented = true });

                TextFile.TextFileAppender(jsonString);



                char choice = '\0';

                Console.Write("\n\nInsert Another Student? (Y | N): ");
                while (!char.TryParse(Console.ReadLine()?.ToLower(), out choice) && (choice != 'y' || choice != 'n'))
                {
                    Console.Write("\nInvalid input. (Y | N) : ");
                }

                if (choice == 'n') break;
            }
        }

        static string CourseChecker()
        {
            string course = Console.ReadLine()?.ToUpper() ?? string.Empty;

            while (course != "BSIT" && course != "BSME")
            {
                Console.Write("None of the above. Choose between 'BSIT' or 'BSME'\n > ");
                course = Console.ReadLine()?.ToUpper() ?? string.Empty;
            }

            return course;
        }

        static string YearChecker()
        {
            int startingYear = 0;
            int endingYear = 0;

            do
            {
                Console.Write("Starting year: ");
                int.TryParse(Console.ReadLine(), out startingYear);

            } while (startingYear.ToString().Length != 4 || startingYear < 2000);

            endingYear = startingYear + 1;

            return $"{startingYear} - {endingYear}";
        }
    }

    class TextFile
    {
        public static void TextFileAppender(string jsonString)
        {
            string pathFile = "C:\\Users\\You Are\\Documents\\CSharp\\MiniProject\\MiniProject\\Student_Information_List.txt";

            string existingFile = File.ReadAllText(pathFile);

            using (StreamWriter writer = new StreamWriter(pathFile, true))
            {
                writer.WriteLine(jsonString);
            }

        }
    }
}