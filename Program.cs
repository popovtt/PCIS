using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Lab2Variant1;

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

public class Person : IDateAndCopy
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
    {
        _firstName = "Ім'яЗаЗамовчуванням";
        _lastName = "ПрізвищеЗаЗамовчуванням";
        _birthDate = new DateTime(2000, 1, 1);
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
        if (obj is null || obj.GetType() != GetType())
            return false;

        Person other = (Person)obj;
        return _firstName == other._firstName
               && _lastName == other._lastName
               && _birthDate == other._birthDate;
    }

    public static bool operator ==(Person? left, Person? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

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
    public string Subject { get; set; }
    public int Grade { get; set; }
    public DateTime ExamDate { get; set; }

    public Exam(string subject, int grade, DateTime examDate)
    {
        Subject = subject;
        Grade = grade;
        ExamDate = examDate;
    }

    public Exam()
    {
        Subject = "ПредметЗаЗамовчуванням";
        Grade = 0;
        ExamDate = DateTime.Today;
    }

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
        if (obj is null || obj.GetType() != GetType())
            return false;

        Exam other = (Exam)obj;
        return Subject == other.Subject
               && Grade == other.Grade
               && ExamDate == other.ExamDate;
    }

    public static bool operator ==(Exam? left, Exam? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

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
    public string Subject { get; set; }
    public bool IsPassed { get; set; }

    public Test(string subject, bool isPassed)
    {
        Subject = subject;
        IsPassed = isPassed;
    }

    public Test()
    {
        Subject = "ПредметЗаЗамовчуванням";
        IsPassed = false;
    }

    public DateTime Date { get; init; } = DateTime.Today;

    public override string ToString()
    {
        return $"Предмет: {Subject}, Залік: {(IsPassed ? "зараховано" : "не зараховано")}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        Test other = (Test)obj;
        return Subject == other.Subject && IsPassed == other.IsPassed;
    }

    public static bool operator ==(Test? left, Test? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

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

public class Student : Person, IDateAndCopy, IEnumerable
{
    private Education _education;
    private int _groupNumber;
    private ArrayList _tests;
    private ArrayList _exams;

    public Student(Person person, Education education, int groupNumber)
        : base(person.FirstName, person.LastName, person.BirthDate)
    {
        _education = education;
        GroupNumber = groupNumber;
        _tests = new ArrayList();
        _exams = new ArrayList();
    }

    public Student() : base()
    {
        _education = Education.Bachelor;
        _groupNumber = 101;
        _tests = new ArrayList();
        _exams = new ArrayList();
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

    public ArrayList Tests
    {
        get => _tests;
        init => _tests = value ?? new ArrayList();
    }

    public ArrayList Exams
    {
        get => _exams;
        init => _exams = value ?? new ArrayList();
    }

    public double AverageGrade
    {
        get
        {
            if (_exams.Count == 0)
                return 0;

            double sum = 0;
            foreach (Exam exam in _exams)
            {
                sum += exam.Grade;
            }

            return sum / _exams.Count;
        }
    }

    public override DateTime Date
    {
        get => _birthDate;
        init => _birthDate = value;
    }

    public void AddExams(params Exam[] exams)
    {
        if (exams == null)
            return;

        foreach (var exam in exams)
        {
            _exams.Add(exam);
        }
    }

    public void AddTests(params Test[] tests)
    {
        if (tests == null)
            return;

        foreach (var test in tests)
        {
            _tests.Add(test);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("=== Student ===");
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
            foreach (Test test in _tests)
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
            foreach (Exam exam in _exams)
            {
                sb.AppendLine($"  {exam}");
            }
        }

        return sb.ToString();
    }

    public override string ToShortString()
    {
        return $"{base.ToString()}, Форма навчання: {_education}, Номер групи: {_groupNumber}, Середній бал: {AverageGrade:F2}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        Student other = (Student)obj;

        if (!base.Equals(other))
            return false;

        if (_education != other._education || _groupNumber != other._groupNumber)
            return false;

        if (_tests.Count != other._tests.Count || _exams.Count != other._exams.Count)
            return false;

        for (int i = 0; i < _tests.Count; i++)
        {
            if (!_tests[i]!.Equals(other._tests[i]))
                return false;
        }

        for (int i = 0; i < _exams.Count; i++)
        {
            if (!_exams[i]!.Equals(other._exams[i]))
                return false;
        }

        return true;
    }

    public static bool operator ==(Student? left, Student? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Student? left, Student? right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        int hash = HashCode.Combine(base.GetHashCode(), _education, _groupNumber);

        foreach (Test test in _tests)
        {
            hash = HashCode.Combine(hash, test.GetHashCode());
        }

        foreach (Exam exam in _exams)
        {
            hash = HashCode.Combine(hash, exam.GetHashCode());
        }

        return hash;
    }

    public override object DeepCopy()
    {
        var copy = new Student(
            new Person(_firstName, _lastName, _birthDate),
            _education,
            _groupNumber
        );

        foreach (Test test in _tests)
        {
            copy._tests.Add(test.DeepCopy());
        }

        foreach (Exam exam in _exams)
        {
            copy._exams.Add(exam.DeepCopy());
        }

        return copy;
    }

    // Ітератор: об'єднання заліків та іспитів
    public IEnumerable GetAllExamsAndTests()
    {
        foreach (Test test in _tests)
            yield return test;

        foreach (Exam exam in _exams)
            yield return exam;
    }

    // Ітератор з параметром: іспити з оцінкою вище за задану
    public IEnumerable<Exam> GetExamsWithGradeHigherThan(int minGrade)
    {
        foreach (Exam exam in _exams)
        {
            if (exam.Grade > minGrade)
                yield return exam;
        }
    }

    // Додаткове: всі здані заліки та іспити
    public IEnumerable GetPassedTestsAndExams()
    {
        foreach (Test test in _tests)
        {
            if (test.IsPassed)
                yield return test;
        }

        foreach (Exam exam in _exams)
        {
            if (exam.Grade > 2)
                yield return exam;
        }
    }

    // Додаткове: всі здані заліки, для яких іспит також зданий
    public IEnumerable<Test> GetPassedTestsWithPassedExams()
    {
        foreach (Test test in _tests)
        {
            if (!test.IsPassed)
                continue;

            foreach (Exam exam in _exams)
            {
                if (exam.Subject == test.Subject && exam.Grade > 2)
                {
                    yield return test;
                    break;
                }
            }
        }
    }

    // IEnumerable: предмети, які є і в заліках, і в іспитах
    public IEnumerator GetEnumerator()
    {
        return new StudentEnumerator(_tests, _exams);
    }
}

public class StudentEnumerator : IEnumerator
{
    private readonly List<string> _intersectionSubjects;
    private int _position = -1;

    public StudentEnumerator(ArrayList tests, ArrayList exams)
    {
        _intersectionSubjects = new List<string>();

        foreach (Test test in tests)
        {
            foreach (Exam exam in exams)
            {
                if (test.Subject == exam.Subject && !_intersectionSubjects.Contains(test.Subject))
                {
                    _intersectionSubjects.Add(test.Subject);
                }
            }
        }
    }

    public object Current
    {
        get
        {
            if (_position < 0 || _position >= _intersectionSubjects.Count)
                throw new InvalidOperationException();

            return _intersectionSubjects[_position];
        }
    }

    public bool MoveNext()
    {
        _position++;
        return _position < _intersectionSubjects.Count;
    }

    public void Reset()
    {
        _position = -1;
    }
}

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // 1. Два об'єкти Person з однаковими даними
        var person1 = new Person("Роман", "Попов", new DateTime(2006, 8, 23));
        var person2 = new Person("Роман", "Попов", new DateTime(2006, 8, 23));

        Console.WriteLine("=== Перевірка Person ===");
        Console.WriteLine($"ReferenceEquals(person1, person2): {ReferenceEquals(person1, person2)}");
        Console.WriteLine($"person1 == person2: {person1 == person2}");
        Console.WriteLine($"person1.Equals(person2): {person1.Equals(person2)}");
        Console.WriteLine($"Hash person1: {person1.GetHashCode()}");
        Console.WriteLine($"Hash person2: {person2.GetHashCode()}");
        Console.WriteLine();

        // 2. Об'єкт Student
        var student = new Student(person1, Education.Master, 321);

        student.AddTests(
            new Test("Математика", true),
            new Test("Фізика", true),
            new Test("Історія", false),
            new Test("Програмування", true)
        );

        student.AddExams(
            new Exam("Математика", 5, new DateTime(2025, 6, 10)),
            new Exam("Фізика", 4, new DateTime(2025, 6, 12)),
            new Exam("Історія", 2, new DateTime(2025, 6, 15)),
            new Exam("Програмування", 5, new DateTime(2025, 6, 20))
        );

        Console.WriteLine("=== Student ===");
        Console.WriteLine(student);

        // 3. Властивість типу Person
        Console.WriteLine("=== PersonData ===");
        Console.WriteLine(student.PersonData);
        Console.WriteLine();

        // 4. DeepCopy
        Student copiedStudent = (Student)student.DeepCopy();

        student.AddExams(new Exam("Англійська", 3, new DateTime(2025, 6, 25)));
        student.AddTests(new Test("Англійська", true));
        student.PersonData = new Person("ЗміненеІмя", "ЗміненеПрізвище", new DateTime(2000, 1, 1));

        Console.WriteLine("=== Оригінал після змін ===");
        Console.WriteLine(student);

        Console.WriteLine("=== Глибока копія ===");
        Console.WriteLine(copiedStudent);

        // 5. try/catch для недопустимого номера групи
        Console.WriteLine("=== Перевірка виключення ===");
        try
        {
            student = new Student(person1, Education.Bachelor, 50);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }

        Console.WriteLine();

        // 6. Іспити з оцінкою > 3
        Console.WriteLine("=== Іспити з оцінкою > 3 ===");
        foreach (Exam exam in copiedStudent.GetExamsWithGradeHigherThan(3))
        {
            Console.WriteLine(exam);
        }

        Console.WriteLine();

        // 7. Предмети, які є і в заліках, і в іспитах
        Console.WriteLine("=== Предмети, які є і в заліках, і в іспитах ===");
        foreach (string subject in copiedStudent)
        {
            Console.WriteLine(subject);
        }

        Console.WriteLine();

        // 8. Всі здані заліки та іспити
        Console.WriteLine("=== Всі здані заліки та іспити ===");
        foreach (object item in copiedStudent.GetPassedTestsAndExams())
        {
            Console.WriteLine(item);
        }

        Console.WriteLine();

        // 9. Здані заліки, для яких іспит також зданий
        Console.WriteLine("=== Здані заліки, для яких іспит також зданий ===");
        foreach (Test test in copiedStudent.GetPassedTestsWithPassedExams())
        {
            Console.WriteLine(test);
        }

        Console.WriteLine();

        // 10. Об'єднання заліків та іспитів
        Console.WriteLine("=== Усі заліки та іспити разом ===");
        foreach (object item in copiedStudent.GetAllExamsAndTests())
        {
            Console.WriteLine(item);
        }
    }
}