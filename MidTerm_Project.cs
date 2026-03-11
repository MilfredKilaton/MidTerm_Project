using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Transactions;

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
        public List<Subject> StudentSubjects { get; set; } = new List<Subject>();
        public List<Grade> StudentGrades { get; set; } = new List<Grade>();
        public string? Status { get; set; }
    }

    public class Subject
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
    }

    public class Grade
    {
        public string? SubjectId { get; set; }
        public decimal? SubjectGrade { get; set; }
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

            TextFile.LoadFromTextFile();

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
                            Grades.StudentGradeProcess();
                        break;

                        case 4:
                            Grades.ShowStudentGrade();
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
        public static void ShowStudentGrade()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("---------- Student Grades ----------");

                var studentList = DataStore.StudentList.Where(student => student.Status != "Registered").ToList();

                if (studentList.Count == 0)
                {
                    Console.Write("No student records found.");
                    return;
                }

                char preference;

                Console.WriteLine(" A - Student with BSIT course");
                Console.WriteLine(" B - Student with BSME course");
                Console.WriteLine(" C - specific student ID");
                Console.Write("\nEnter the letter of your preference: ");

                while (!char.TryParse(Console.ReadLine()?.ToUpper(), out preference) || (preference != 'A' && preference != 'B' && preference != 'C'))
                {
                    Console.Write("Choose from 1 of the preferences: ");
                }

                string? studentId = null;

                if (preference == 'C')
                {
                    Console.Write("\nEnter Student ID: ");
                    studentId = Console.ReadLine();
                }

                var targets = preference switch
                {
                    'A' => DataStore.StudentList.Where(student => student.Course == "BSIT"),
                    'B' => DataStore.StudentList.Where(student => student.Course == "BSME"),
                    'C' => DataStore.StudentList.Where(student => student.Id == studentId),
                    _ => Enumerable.Empty<Student>()
                };

                if (!targets.Any() && (preference == 'A' || preference == 'B'))
                {
                    string course = preference == 'A' ? "BSIT" : "BSME";
                    Console.WriteLine($"No student record found enrolled in {course}");
                    return;
                }

                if (!targets.Any() && preference == 'C')
                {
                    Console.WriteLine("No student record found with the inputted ID.");
                    return;
                }

                Console.Clear();
                foreach (var target in targets)
                {
                    string properName = $"{target.LastName}, {target.FirstName}";

                    Console.WriteLine("+==========================================================+");
                    Console.WriteLine($"| {properName,-56} |");
                    Console.WriteLine($"| {target.Course} - Year {target.Year,-44} |");
                    Console.WriteLine("|----------------------------------------------------------|");

                    foreach (var subject in target.StudentSubjects!)
                    {
                        var grade = target.StudentGrades?.FirstOrDefault(grade => grade.SubjectId == subject.Id);

                        string displayGrade = grade?.SubjectGrade?.ToString("0.0") ?? "N/A";

                        string initialSubject = $"{subject.Name} ";

                        string displaySubject = initialSubject.PadRight(50, '-');

                        Console.WriteLine($"| {displaySubject}{displayGrade,6} |");
                    }

                    Console.WriteLine("+----------------------------------------------------------+\n");
                }

                char choice = '\0';

                Console.Write(" \nTry again? (Y | N): ");
                while (!char.TryParse(Console.ReadLine()?.ToLower(), out choice) && (choice != 'y' || choice != 'n'))
                {
                    Console.Write("\nInvalid input. (Y | N) : ");
                }

                if (choice == 'n') return;
            }
        }

        public static void StudentGradeProcess()
        {
            Console.Clear();
            Console.WriteLine("---------- GRADING SYSTEM ----------");

            var studentList = DataStore.StudentList.Where(student => student.Status == "Enrolled").ToList();

            if (studentList.Count == 0)
            {
                Console.Write("All students are either graded or no records found.");
                return;
            }

            foreach (var student in studentList)
            {
                Console.WriteLine($"{student.Id} | {student.LastName}, {student.FirstName} | {student.Course}");
            }

            Console.Write("\nStudent ID (enter 'exit' to return): ");
            string inputId = Console.ReadLine() ?? "";

            if (inputId.ToLower() == "exit") 
            {
                return; 
            }

            var target = DataStore.StudentList.FirstOrDefault(x => x.Id == inputId);

            if (target?.Status == "Registered")
            {
                Console.WriteLine("This student has no subjects to be graded.");
                return;
            }

            if (target == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }

            StudentGrader(target);

            bool allGraded = target.StudentSubjects != null 
                && target.StudentSubjects.All(s => target.StudentGrades?.Any(g => g.SubjectId == s.Id) == true);

            if (allGraded)
            {
                target.Status = "Graded";
            }
        }

        static void StudentGrader(Student student)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{student.LastName}, {student.FirstName}");
                Console.WriteLine("");

                for (int i = 0; i < student.StudentSubjects!.Count; i++)
                {
                    var sub = student.StudentSubjects[i];

                    var toBeGraded = student.StudentGrades?.FirstOrDefault(g => g.SubjectId == sub.Id);

                    string gradeText = toBeGraded != null && toBeGraded.SubjectGrade.HasValue ? toBeGraded.SubjectGrade.Value.ToString() : "Not Graded";

                    Console.WriteLine($"{i + 1 + ".", -3} {sub.Id, -12} - {sub.Name, -42} | {gradeText}");
                }

                Console.WriteLine("\n0. Exit grading");

                Console.Write("\nChoose subject number: ");

                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                    continue;

                if (choice == 0)
                    break;

                if (choice < 1 || choice > student.StudentSubjects.Count)
                    continue;

                var subject = student.StudentSubjects[choice - 1];

                decimal temporaryGrade = 0;

                Console.Write($"Enter grade for {subject.Name}; 50 - 100:\n > ");
                while (!decimal.TryParse(Console.ReadLine(), out temporaryGrade) || temporaryGrade < 50)
                {
                    Console.Write("Enter a number from 50 to 100:\n > ");
                }

                string input = GradeConverter(temporaryGrade);

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                decimal grade;

                while (!decimal.TryParse(input, out grade))
                {
                    Console.Write("Invalid grade. Try again: ");
                    input = Console.ReadLine() ?? "";
                }

                var existingGrade = student.StudentGrades?.FirstOrDefault(grade => grade.SubjectId == subject.Id);

                if (existingGrade != null)
                {
                    existingGrade.SubjectGrade = grade;
                }
                else
                {
                    student.StudentGrades?.Add(new Grade
                    {
                        SubjectId = subject.Id,
                        SubjectGrade = grade
                    });
                }

                TextFile.SaveToTextFile();
            }

        }

        static string GradeConverter(decimal grade)
        {
            string convertedGrade = grade switch
            {
                >= 97 => Convert.ToString(1.00m),
                >= 94 => Convert.ToString(1.25m),
                >= 91 => Convert.ToString(1.50m),
                >= 88 => Convert.ToString(1.75m),
                >= 85 => Convert.ToString(2.00m),
                >= 82 => Convert.ToString(2.25m),
                >= 79 => Convert.ToString(2.50m),
                >= 76 => Convert.ToString(2.75m),
                >= 75 => Convert.ToString(3.00m),
                _     => Convert.ToString(5.00m)
            };

            return convertedGrade;
        }
    }

    class Enroll
    {
        public static void SubjectEnrollProcess()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("---------- ENROLL STUDENTS ----------");

                var studentList = DataStore.StudentList.Where(x => x.Status == "Registered").ToList();

                if (studentList.Count == 0)
                {
                    Console.Write("All students are either been enrolled or no student records found.");
                    return;
                }

                foreach (var student in studentList)
                {
                    Console.WriteLine($"{student.Id} | {student.LastName}, {student.FirstName} | {student.Course}");
                }

                Console.Write("\nStudent ID (enter 'exit' to return): ");
                string inputId = Console.ReadLine() ?? "";

                if (inputId.ToLower() == "exit") return;

                var target = DataStore.StudentList.FirstOrDefault(x => x.Id == inputId);

                if (target == null)
                {
                    Console.WriteLine("Student not found.");
                    return;
                }

                if (target.Status == "Enrolled")
                {
                    Console.WriteLine($"Student {target.FirstName} {target.LastName} already enrolled.");
                    return;
                }

                var subjects = GenerateSubjects(target.Course ?? "");

                target.StudentSubjects = subjects;
                target.Status = "Enrolled";

                TextFile.SaveToTextFile();

                Console.WriteLine($"\n{target.FirstName} {target.LastName} successfully enrolled.");

                foreach (var subject in subjects)
                {
                    Console.WriteLine($"{subject.Id} - {subject.Name}");
                }
                Console.ReadKey();
            }
        }

        static List<Subject> GenerateSubjects(string course)
        {
            List<Subject> subjects = new List<Subject>();

            if (course.ToUpper() == "BSIT")
            {
                subjects.Add(new Subject { Id = "COMP 102IT", Name = "Computer Applications" });
                subjects.Add(new Subject { Id = "IT 104A", Name = "Computer Programmin 2, Lec." });
                subjects.Add(new Subject { Id = "IT 104B", Name = "Computer Programming 2, Lab." });
                subjects.Add(new Subject { Id = "IT 105A", Name = "Platform Technologies" });
                subjects.Add(new Subject { Id = "IT 106A", Name = "IT Social and Professional Issues" });
                subjects.Add(new Subject { Id = "GEC 103A", Name = "Understanding the Self" });
                subjects.Add(new Subject { Id = "GEC 104A", Name = "Arts Appreciation" });
                subjects.Add(new Subject { Id = "RZL 104A", Name = "Life and Works of Rizal" });
                subjects.Add(new Subject { Id = "PE 102A", Name = "Physical Education 2" });
                subjects.Add(new Subject { Id = "THEO 102A", Name = "Scriptures, the Sacraments and the Liturgy" });
            }
            else if (course.ToUpper() == "BSME")
            {
                subjects.Add(new Subject { Id = "ME 201A", Name = "Engineering Mechanics - Statics" });
                subjects.Add(new Subject { Id = "ME 202A", Name = "Engineering Mechanics - Dynamics" });
                subjects.Add(new Subject { Id = "ME 203A", Name = "Thermodynamics 1" });
                subjects.Add(new Subject { Id = "ME 204A", Name = "Materials Science for Engineers" });
                subjects.Add(new Subject { Id = "ME 205B", Name = "Mechanical Engineering Drawing, Lab." });
                subjects.Add(new Subject { Id = "MATH 203A", Name = "Differential Equations" });
                subjects.Add(new Subject { Id = "PHY 204A", Name = "Engineering Physics" });
                subjects.Add(new Subject { Id = "GEC 105A", Name = "Science, Technology and Society" });
                subjects.Add(new Subject { Id = "RZL 105A", Name = "Life and Works of Rizal" });
                subjects.Add(new Subject { Id = "PE 103A", Name = "Physical Education 3" });
                subjects.Add(new Subject { Id = "THEO 103A", Name = "Christian Moral Principles" });
            }

            return subjects;
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

                    Console.Write(" - Unique ID: ");
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

                    Console.Write(" - First Name: ");
                    firstName = Console.ReadLine() ?? string.Empty;

                    string input = "Place Holder";
                    do
                    {
                        Console.Write(" - Middle Initial (N/A if none): ");
                        input = Console.ReadLine()?.ToUpper() ?? string.Empty;

                    } while (input != "N/A" && !(input.Length == 1 && char.IsLetter(input[0])));

                    middleInitial = input;

                    Console.Write(" - Last Name: ");
                    lastName = Console.ReadLine() ?? string.Empty;

                    Console.Write(" - Address: ");
                    address = Console.ReadLine() ?? string.Empty;

                    do
                    {
                        Console.Write(" - (11 Digits required) Contact Number: ");
                        input = Console.ReadLine() ?? string.Empty;

                        contactNumber = input.Replace(" ", String.Empty);
                    } while (contactNumber.Length != 11);

                    while (true)
                    {
                        Console.Write(" - Date of Birth (MM/dd/yyyy): ");
                        while (!DateTime.TryParseExact(Console.ReadLine(), dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
                        {
                            Console.Write("Invalid Input. (MM/dd/yyyy): ");
                        }

                        age = DateTime.Today.Year - birthDate.Year;

                        if (birthDate.Date > DateTime.Today.AddYears(-age)) age--;

                        Console.Write($"[!] Calculated age is {age}. Is this correct? (Y | N): ");
                        if (Console.ReadLine()?.ToLower() == "y") break;
                    }

                    Console.Write(" - Course (BSIT | BSME): ");
                    course = CourseChecker();

                    year = YearChecker();
                    while (true)
                    {
                        Console.Write($"[!] {year}. Is this correct? (Y | N): ");
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

                DataStore.StudentList.Add(new Student
                {
                    Id = newStudentId,
                    FirstName = firstName,
                    MiddleInitial = middleInitial,
                    LastName = lastName,
                    Age = age,
                    Address = address,
                    ContactNumber = contactNumber,
                    BirthDate = birthDate,
                    StudentSubjects = new List<Subject>(),
                    StudentGrades = new List<Grade>(),
                    Course = course,
                    Year = year,
                    Status = "Registered"

                });

                TextFile.SaveToTextFile();

                char choice = '\0';

                Console.Write(" \nInsert Another Student? (Y | N): ");
                while (!char.TryParse(Console.ReadLine()?.ToLower(), out choice) && (choice != 'y' || choice != 'n'))
                {
                    Console.Write("\nInvalid input. (Y | N) : ");
                }

                if (choice == 'n') return;
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
                Console.Write(" - Starting year: ");
                int.TryParse(Console.ReadLine(), out startingYear);

            } while (startingYear.ToString().Length != 4 || startingYear < 2000);

            endingYear = startingYear + 1;

            return $"{startingYear} - {endingYear}";
        }
    }

    class TextFile
    {
        private const string PathFile = "C:\\Users\\You Are\\Documents\\CSharp\\MiniProject\\MiniProject\\Student_Information_List.txt";
        
        public static void LoadFromTextFile()
        {

            if (!File.Exists(PathFile)) return;
            
            string existingJson = File.ReadAllText(PathFile);
            if (string.IsNullOrWhiteSpace(existingJson)) return;

            DataStore.StudentList = JsonSerializer.Deserialize<List<Student>>(existingJson) ?? new List<Student>();
        }

        public static void SaveToTextFile()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string jsonString = JsonSerializer.Serialize(DataStore.StudentList, options);

            File.WriteAllText(PathFile, jsonString);
        }
    }
}