using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Lab3Variant1;

public interface IDateAndCopy
{
    DateTime Date { get; init; }
    object DeepCopy();
}

public enum Education
{
    Master,
    Bachelor,
    SecondEducation
}

public class Person : IDateAndCopy, IComparable, IComparer<Person>
{
    protected string _firstName;
    protected string _lastName;
    protected DateTime _birthDate;

    public Person(string firstName, string lastName, DateTime birthDate)
    {
        _firstName = firstName;
        _lastName = lastName;
        _birthDate = birthDate;
    }

    public Person()
        : this("Ім'яЗаЗамовчуванням", "ПрізвищеЗаЗамовчуванням", new DateTime(2000, 1, 1))
    {
    }

    public string FirstName
    {
        get => _firstName;
        init => _firstName = value;
    }

    public string LastName
    {
        get => _lastName;
        init => _lastName = value;
    }

    public DateTime BirthDate
    {
        get => _birthDate;
        init => _birthDate = value;
    }

    public int BirthYear
    {
        get => _birthDate.Year;
        set => _birthDate = new DateTime(value, _birthDate.Month, _birthDate.Day);
    }

    public virtual DateTime Date
    {
        get => _birthDate;
        init => _birthDate = value;
    }

    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is not Person other)
        {
            throw new ArgumentException("Об'єкт повинен мати тип Person.", nameof(obj));
        }

        return string.Compare(_lastName, other._lastName, StringComparison.Ordinal);
    }

    public int Compare(Person? x, Person? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }

        if (y is null)
        {
            return 1;
        }

        return DateTime.Compare(x.BirthDate, y.BirthDate);
    }

    public override string ToString()
    {
        return $"Ім'я: {_firstName}, Прізвище: {_lastName}, Дата народження: {_birthDate:dd.MM.yyyy}";
    }

    public virtual string ToShortString()
    {
        return $"{_lastName} {_firstName}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Person other)
        {
            return false;
        }

        return _firstName == other._firstName
               && _lastName == other._lastName
               && _birthDate == other._birthDate;
    }

    public static bool operator ==(Person? left, Person? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Person? left, Person? right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_firstName, _lastName, _birthDate);
    }

    public virtual object DeepCopy()
    {
        return new Person(_firstName, _lastName, _birthDate);
    }
}

public class Exam : IDateAndCopy
{
    public Exam(string subject, int grade, DateTime examDate)
    {
        Subject = subject;
        Grade = grade;
        ExamDate = examDate;
    }

    public Exam()
        : this("ПредметЗаЗамовчуванням", 0, DateTime.Today)
    {
    }

    public string Subject { get; set; }

    public int Grade { get; set; }

    public DateTime ExamDate { get; set; }

    public DateTime Date
    {
        get => ExamDate;
        init => ExamDate = value;
    }

    public override string ToString()
    {
        return $"Предмет: {Subject}, Оцінка: {Grade}, Дата іспиту: {ExamDate:dd.MM.yyyy}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Exam other)
        {
            return false;
        }

        return Subject == other.Subject && Grade == other.Grade && ExamDate == other.ExamDate;
    }

    public static bool operator ==(Exam? left, Exam? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Exam? left, Exam? right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Subject, Grade, ExamDate);
    }

    public object DeepCopy()
    {
        return new Exam(Subject, Grade, ExamDate);
    }
}

public class Test : IDateAndCopy
{
    public Test(string subject, bool isPassed)
    {
        Subject = subject;
        IsPassed = isPassed;
    }

    public Test()
        : this("ПредметЗаЗамовчуванням", false)
    {
    }

    public string Subject { get; set; }

    public bool IsPassed { get; set; }

    public DateTime Date { get; init; } = DateTime.Today;

    public override string ToString()
    {
        return $"Предмет: {Subject}, Залік: {(IsPassed ? "зараховано" : "не зараховано")}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Test other)
        {
            return false;
        }

        return Subject == other.Subject && IsPassed == other.IsPassed;
    }

    public static bool operator ==(Test? left, Test? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Test? left, Test? right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Subject, IsPassed);
    }

    public object DeepCopy()
    {
        return new Test(Subject, IsPassed) { Date = Date };
    }
}

public class Student : Person, IEnumerable<string>
{
    private Education _education;
    private int _groupNumber;
    private List<Test> _tests;
    private List<Exam> _exams;

    public Student(Person person, Education education, int groupNumber)
        : base(person.FirstName, person.LastName, person.BirthDate)
    {
        _education = education;
        GroupNumber = groupNumber;
        _tests = new List<Test>();
        _exams = new List<Exam>();
    }

    public Student()
        : this(new Person(), Education.Bachelor, 101)
    {
    }

    public Person PersonData
    {
        get => new Person(_firstName, _lastName, _birthDate);
        set
        {
            _firstName = value.FirstName;
            _lastName = value.LastName;
            _birthDate = value.BirthDate;
        }
    }

    public Person PersonKey => new(_firstName, _lastName, _birthDate);

    public Education EducationForm
    {
        get => _education;
        init => _education = value;
    }

    public int GroupNumber
    {
        get => _groupNumber;
        init
        {
            if (value <= 100 || value >= 699)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(GroupNumber),
                    "Номер групи повинен бути більше 100 і менше 699."
                );
            }

            _groupNumber = value;
        }
    }

    public List<Test> Tests
    {
        get => _tests;
        init => _tests = value ?? new List<Test>();
    }

    public List<Exam> Exams
    {
        get => _exams;
        init => _exams = value ?? new List<Exam>();
    }

    public double AverageGrade => _exams.Count == 0 ? 0 : _exams.Average(exam => exam.Grade);

    public override DateTime Date
    {
        get => _birthDate;
        init => _birthDate = value;
    }

    public void AddExams(params Exam[] exams)
    {
        if (exams is null)
        {
            return;
        }

        _exams.AddRange(exams);
    }

    public void AddTests(params Test[] tests)
    {
        if (tests is null)
        {
            return;
        }

        _tests.AddRange(tests);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine(base.ToString());
        sb.AppendLine($"Форма навчання: {_education}");
        sb.AppendLine($"Номер групи: {_groupNumber}");
        sb.AppendLine($"Середній бал: {AverageGrade:F2}");
        sb.AppendLine("Заліки:");

        if (_tests.Count == 0)
        {
            sb.AppendLine("  Немає заліків");
        }
        else
        {
            foreach (var test in _tests)
            {
                sb.AppendLine($"  {test}");
            }
        }

        sb.AppendLine("Іспити:");
        if (_exams.Count == 0)
        {
            sb.AppendLine("  Немає іспитів");
        }
        else
        {
            foreach (var exam in _exams)
            {
                sb.AppendLine($"  {exam}");
            }
        }

        return sb.ToString().TrimEnd();
    }

    public override string ToShortString()
    {
        return $"{base.ToString()}, Форма навчання: {_education}, Номер групи: {_groupNumber}, Середній бал: {AverageGrade:F2}, К-сть заліків: {_tests.Count}, К-сть іспитів: {_exams.Count}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Student other)
        {
            return false;
        }

        return base.Equals(other)
               && _education == other._education
               && _groupNumber == other._groupNumber
               && _tests.SequenceEqual(other._tests)
               && _exams.SequenceEqual(other._exams);
    }

    public static bool operator ==(Student? left, Student? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Student? left, Student? right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        var hash = HashCode.Combine(base.GetHashCode(), _education, _groupNumber);

        foreach (var test in _tests)
        {
            hash = HashCode.Combine(hash, test);
        }

        foreach (var exam in _exams)
        {
            hash = HashCode.Combine(hash, exam);
        }

        return hash;
    }

    public override object DeepCopy()
    {
        var copy = new Student(new Person(_firstName, _lastName, _birthDate), _education, _groupNumber);
        copy._tests = _tests.Select(test => (Test)test.DeepCopy()).ToList();
        copy._exams = _exams.Select(exam => (Exam)exam.DeepCopy()).ToList();
        return copy;
    }

    public IEnumerable<object> GetAllExamsAndTests()
    {
        foreach (var test in _tests)
        {
            yield return test;
        }

        foreach (var exam in _exams)
        {
            yield return exam;
        }
    }

    public IEnumerable<Exam> GetExamsWithGradeHigherThan(int minGrade)
    {
        foreach (var exam in _exams)
        {
            if (exam.Grade > minGrade)
            {
                yield return exam;
            }
        }
    }

    public IEnumerable<object> GetPassedTestsAndExams()
    {
        foreach (var test in _tests.Where(test => test.IsPassed))
        {
            yield return test;
        }

        foreach (var exam in _exams.Where(exam => exam.Grade > 2))
        {
            yield return exam;
        }
    }

    public IEnumerable<Test> GetPassedTestsWithPassedExams()
    {
        foreach (var test in _tests.Where(test => test.IsPassed))
        {
            if (_exams.Any(exam => exam.Subject == test.Subject && exam.Grade > 2))
            {
                yield return test;
            }
        }
    }

    public IEnumerator<string> GetEnumerator()
    {
        return _tests
            .Select(test => test.Subject)
            .Intersect(_exams.Select(exam => exam.Subject))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class StudentAverageGradeComparer : Comparer<Student>
{
    public override int Compare(Student? x, Student? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }

        if (y is null)
        {
            return 1;
        }

        return x.AverageGrade.CompareTo(y.AverageGrade);
    }
}

public class StudentCollection
{
    private readonly List<Student> _students = new();

    public double MaxAverageGrade => _students.Count == 0 ? 0 : _students.Max(GetAverageGradeSelector);

    public IEnumerable<Student> Masters => _students.Where(IsMasterSelector);

    public void AddDefaults()
    {
        AddStudents(
            CreateStudent(1),
            CreateStudent(2),
            CreateStudent(3)
        );
    }

    public void AddStudents(params Student[] students)
    {
        if (students is null)
        {
            return;
        }

        _students.AddRange(students);
    }

    public void SortByLastName()
    {
        _students.Sort();
    }

    public void SortByBirthDate()
    {
        _students.Sort(new Person());
    }

    public void SortByAverageGrade()
    {
        _students.Sort(new StudentAverageGradeComparer());
    }

    public List<Student> AverageMarkGroup(double value)
    {
        return _students
            .GroupBy(GetAverageGradeSelector)
            .Where(group => Math.Abs(group.Key - value) < 0.0001)
            .SelectMany(group => group)
            .ToList();
    }

    public IEnumerable<IGrouping<double, Student>> GetAverageGradeGroups()
    {
        return _students.GroupBy(GetAverageGradeSelector).OrderBy(group => group.Key);
    }

    public override string ToString()
    {
        if (_students.Count == 0)
        {
            return "Колекція студентів порожня.";
        }

        return string.Join(
            $"{Environment.NewLine}{new string('-', 70)}{Environment.NewLine}",
            _students.Select(student => student.ToString())
        );
    }

    public string ToShortString()
    {
        if (_students.Count == 0)
        {
            return "Колекція студентів порожня.";
        }

        return string.Join(Environment.NewLine, _students.Select(student => student.ToShortString()));
    }

    private static double GetAverageGradeSelector(Student student)
    {
        return student.AverageGrade;
    }

    private static bool IsMasterSelector(Student student)
    {
        return student.EducationForm == Education.Master;
    }

    private static Student CreateStudent(int index)
    {
        return TestCollections.GenerateElement(index);
    }
}

public class TestCollections
{
    private readonly List<Person> _persons;
    private readonly List<string> _personStrings;
    private readonly Dictionary<Person, Student> _personStudentDictionary;
    private readonly Dictionary<string, Student> _stringStudentDictionary;

    public TestCollections(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Кількість елементів не може бути від'ємною.");
        }

        _persons = new List<Person>(count);
        _personStrings = new List<string>(count);
        _personStudentDictionary = new Dictionary<Person, Student>(count);
        _stringStudentDictionary = new Dictionary<string, Student>(count);

        for (int i = 0; i < count; i++)
        {
            var student = GenerateElement(i);
            var person = student.PersonKey;
            var stringKey = person.ToString();

            _persons.Add(person);
            _personStrings.Add(stringKey);
            _personStudentDictionary.Add(person, student);
            _stringStudentDictionary.Add(stringKey, student);
        }
    }

    public static Student GenerateElement(int index)
    {
        var normalizedIndex = Math.Abs(index);
        var person = new Person(
            $"Ім'я{index}",
            $"Прізвище{index}",
            new DateTime(1995 + normalizedIndex % 20, normalizedIndex % 12 + 1, normalizedIndex % 28 + 1)
        );

        var student = new Student(
            person,
            (Education)(normalizedIndex % Enum.GetValues<Education>().Length),
            101 + normalizedIndex % 598
        );

        student.AddTests(
            new Test($"Предмет{index}_1", index % 2 == 0),
            new Test($"Предмет{index}_2", true)
        );

        student.AddExams(
            new Exam($"Предмет{index}_1", normalizedIndex % 5 + 1, new DateTime(2024, normalizedIndex % 12 + 1, normalizedIndex % 28 + 1)),
            new Exam($"Предмет{index}_2", (normalizedIndex + 2) % 5 + 1, new DateTime(2024, (normalizedIndex + 1) % 12 + 1, (normalizedIndex + 2) % 28 + 1))
        );

        return student;
    }

    public void MeasureSearchTimes()
    {
        if (_persons.Count == 0)
        {
            Console.WriteLine("Колекції порожні. Пошук неможливий.");
            return;
        }

        var scenarios = new (string Label, Person Person, string StringKey, Student Student)[]
        {
            CreateScenario("першого елемента", 0),
            CreateScenario("центрального елемента", _persons.Count / 2),
            CreateScenario("останнього елемента", _persons.Count - 1),
            CreateMissingScenario()
        };

        foreach (var scenario in scenarios)
        {
            Console.WriteLine($"Пошук для {scenario.Label}:");
            Console.WriteLine($"  List<Person>.Contains: {Measure(() => _persons.Contains(scenario.Person)):N0} ticks");
            Console.WriteLine($"  List<string>.Contains: {Measure(() => _personStrings.Contains(scenario.StringKey)):N0} ticks");
            Console.WriteLine($"  Dictionary<Person, Student>.ContainsKey: {Measure(() => _personStudentDictionary.ContainsKey(scenario.Person)):N0} ticks");
            Console.WriteLine($"  Dictionary<string, Student>.ContainsKey: {Measure(() => _stringStudentDictionary.ContainsKey(scenario.StringKey)):N0} ticks");
            Console.WriteLine($"  Dictionary<Person, Student>.ContainsValue: {Measure(() => _personStudentDictionary.ContainsValue(scenario.Student)):N0} ticks");
            Console.WriteLine();
        }
    }

    private (string Label, Person Person, string StringKey, Student Student) CreateScenario(string label, int index)
    {
        var person = _persons[index];
        var stringKey = _personStrings[index];
        var student = _personStudentDictionary[person];
        return (label, person, stringKey, student);
    }

    private static (string Label, Person Person, string StringKey, Student Student) CreateMissingScenario()
    {
        var student = GenerateElement(-1);
        var person = student.PersonKey;
        return ("елемента, що не входить в колекцію", person, person.ToString(), student);
    }

    private static long Measure(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.ElapsedTicks;
    }
}

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var studentCollection = new StudentCollection();
        studentCollection.AddDefaults();
        studentCollection.AddStudents(
            CreateCustomStudent(
                "Олена",
                "Кравець",
                new DateTime(2004, 5, 14),
                Education.Master,
                321,
                new[] { new Test("Бази даних", true), new Test("Комп'ютерні мережі", true) },
                new[]
                {
                    new Exam("Бази даних", 5, new DateTime(2025, 1, 17)),
                    new Exam("Комп'ютерні мережі", 4, new DateTime(2025, 1, 20))
                }
            ),
            CreateCustomStudent(
                "Іван",
                "Мельник",
                new DateTime(2003, 11, 3),
                Education.Bachelor,
                215,
                new[] { new Test("Алгоритми", true), new Test("Дискретна математика", false) },
                new[]
                {
                    new Exam("Алгоритми", 3, new DateTime(2025, 1, 18)),
                    new Exam("Дискретна математика", 2, new DateTime(2025, 1, 21))
                }
            )
        );

        Console.WriteLine("=== Початкова колекція StudentCollection ===");
        Console.WriteLine(studentCollection);
        Console.WriteLine();

        Console.WriteLine("=== Коротке представлення StudentCollection ===");
        Console.WriteLine(studentCollection.ToShortString());
        Console.WriteLine();

        studentCollection.SortByLastName();
        Console.WriteLine("=== Сортування за прізвищем ===");
        Console.WriteLine(studentCollection);
        Console.WriteLine();

        studentCollection.SortByBirthDate();
        Console.WriteLine("=== Сортування за датою народження ===");
        Console.WriteLine(studentCollection);
        Console.WriteLine();

        studentCollection.SortByAverageGrade();
        Console.WriteLine("=== Сортування за середнім балом ===");
        Console.WriteLine(studentCollection);
        Console.WriteLine();

        Console.WriteLine($"Максимальний середній бал: {studentCollection.MaxAverageGrade:F2}");
        Console.WriteLine();

        Console.WriteLine("=== Студенти форми навчання Master ===");
        foreach (var student in studentCollection.Masters)
        {
            Console.WriteLine(student.ToShortString());
        }

        Console.WriteLine();

        Console.WriteLine("=== Групування за середнім балом ===");
        foreach (var group in studentCollection.GetAverageGradeGroups())
        {
            Console.WriteLine($"Середній бал: {group.Key:F2}");
            foreach (var student in group)
            {
                Console.WriteLine($"  {student.ToShortString()}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("=== Пошук студентів із заданим середнім балом ===");
        var targetAverage = studentCollection.MaxAverageGrade;
        foreach (var student in studentCollection.AverageMarkGroup(targetAverage))
        {
            Console.WriteLine(student.ToShortString());
        }

        Console.WriteLine();
        var collectionSize = ReadCollectionSize();
        var testCollections = new TestCollections(collectionSize);

        Console.WriteLine();
        Console.WriteLine("=== Вимірювання часу пошуку в колекціях ===");
        testCollections.MeasureSearchTimes();
    }

    private static int ReadCollectionSize()
    {
        while (true)
        {
            Console.Write("Введіть кількість елементів для TestCollections: ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out var count) && count > 0)
            {
                return count;
            }

            Console.WriteLine("Помилка введення. Потрібно ввести додатне ціле число.");
        }
    }

    private static Student CreateCustomStudent(
        string firstName,
        string lastName,
        DateTime birthDate,
        Education education,
        int groupNumber,
        IEnumerable<Test> tests,
        IEnumerable<Exam> exams)
    {
        var student = new Student(new Person(firstName, lastName, birthDate), education, groupNumber);
        student.AddTests(tests.ToArray());
        student.AddExams(exams.ToArray());
        return student;
    }
}
