using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lab6Variant1;

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

public class Person : IDateAndCopy, IComparable, IComparable<Person>, IComparer<Person>
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

        return CompareTo(other);
    }

    public int CompareTo(Person? other)
    {
        if (other is null)
        {
            return 1;
        }

        var lastNameComparison = string.Compare(_lastName, other._lastName, StringComparison.Ordinal);
        if (lastNameComparison != 0)
        {
            return lastNameComparison;
        }

        var firstNameComparison = string.Compare(_firstName, other._firstName, StringComparison.Ordinal);
        if (firstNameComparison != 0)
        {
            return firstNameComparison;
        }

        return DateTime.Compare(_birthDate, other._birthDate);
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
    [JsonInclude]
    private List<Test> _tests;
    [JsonInclude]
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

    public override Student DeepCopy()
    {
        MemoryStream? stream = null;

        try
        {
            stream = new MemoryStream();
            JsonSerializer.Serialize(stream, CreateState(), GetSerializerOptions());
            stream.Position = 0;
            var copiedState = JsonSerializer.Deserialize<StudentSerializationData>(stream, GetSerializerOptions());
            return copiedState is null
                ? throw new InvalidOperationException("Не вдалося створити копію об'єкта Student.")
                : CreateFromState(copiedState);
        }
        finally
        {
            stream?.Dispose();
        }
    }

    public bool Save(string filename)
    {
        return Save(filename, this);
    }

    public bool Load(string filename)
    {
        return Load(filename, this);
    }

    public static bool Save(string filename, Student student)
    {
        FileStream? fileStream = null;

        try
        {
            fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            JsonSerializer.Serialize(fileStream, student.CreateState(), GetSerializerOptions());
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            fileStream?.Dispose();
        }
    }

    public static bool Load(string filename, Student student)
    {
        FileStream? fileStream = null;

        try
        {
            fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            var loadedState = JsonSerializer.Deserialize<StudentSerializationData>(fileStream, GetSerializerOptions());
            if (loadedState is null)
            {
                return false;
            }

            student.CopyFrom(CreateFromState(loadedState));
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            fileStream?.Dispose();
        }
    }

    public bool AddFromConsole()
    {
        Console.WriteLine("Введіть дані іспиту одним рядком у форматі: назва предмету; оцінка; дата іспиту.");
        Console.WriteLine("Як розділювачі можна використовувати символи ';' ',' '|'.");
        Console.Write("Ввід: ");

        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Помилка: порожній рядок введення.");
            return false;
        }

        var parts = input
            .Split(new[] { ';', ',', '|' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
        {
            Console.WriteLine("Помилка: потрібно ввести рівно три значення.");
            return false;
        }

        try
        {
            var subject = parts[0];
            var grade = int.Parse(parts[1]);
            var examDate = DateTime.Parse(parts[2]);
            _exams.Add(new Exam(subject, grade, examDate));
            return true;
        }
        catch
        {
            Console.WriteLine("Помилка: не вдалося розібрати введені дані.");
            return false;
        }
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

    private void CopyFrom(Student source)
    {
        _firstName = source._firstName;
        _lastName = source._lastName;
        _birthDate = source._birthDate;
        _education = source._education;
        _groupNumber = source._groupNumber;
        _tests = source._tests.Select(test => (Test)test.DeepCopy()).ToList();
        _exams = source._exams.Select(exam => (Exam)exam.DeepCopy()).ToList();
    }

    private static JsonSerializerOptions GetSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }

    private StudentSerializationData CreateState()
    {
        return new StudentSerializationData
        {
            FirstName = _firstName,
            LastName = _lastName,
            BirthDate = _birthDate,
            EducationForm = _education,
            GroupNumber = _groupNumber,
            Tests = _tests.Select(test => (Test)test.DeepCopy()).ToList(),
            Exams = _exams.Select(exam => (Exam)exam.DeepCopy()).ToList()
        };
    }

    private static Student CreateFromState(StudentSerializationData state)
    {
        var student = new Student(
            new Person(state.FirstName, state.LastName, state.BirthDate),
            state.EducationForm,
            state.GroupNumber
        );

        student._tests = state.Tests?.Select(test => (Test)test.DeepCopy()).ToList() ?? new List<Test>();
        student._exams = state.Exams?.Select(exam => (Exam)exam.DeepCopy()).ToList() ?? new List<Exam>();
        return student;
    }

    private class StudentSerializationData
    {
        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public DateTime BirthDate { get; init; }

        public Education EducationForm { get; init; }

        public int GroupNumber { get; init; }

        public List<Test>? Tests { get; init; }

        public List<Exam>? Exams { get; init; }
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

public delegate void StudentListHandler(object source, StudentListHandlerEventArgs args);

public class StudentListHandlerEventArgs : EventArgs
{
    public StudentListHandlerEventArgs(string collectionName, string changeType, Student student)
    {
        CollectionName = collectionName;
        ChangeType = changeType;
        Student = student;
    }

    public string CollectionName { get; init; }

    public string ChangeType { get; init; }

    public Student Student { get; init; }

    public override string ToString()
    {
        return $"Колекція: {CollectionName}, Тип зміни: {ChangeType}, Студент: {Student.ToShortString()}";
    }
}

public class StudentCollection
{
    private readonly List<Student> _students = new();

    public StudentCollection(string collectionName)
    {
        CollectionName = collectionName;
    }

    public string CollectionName { get; init; }

    public event StudentListHandler? StudentCountChanged;

    public event StudentListHandler? StudentReferenceChanged;

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

        foreach (var student in students)
        {
            _students.Add(student);
            OnStudentCountChanged("Додано новий елемент до колекції.", student);
        }
    }

    public bool Remove(int j)
    {
        if (j < 0 || j >= _students.Count)
        {
            return false;
        }

        var removedStudent = _students[j];
        _students.RemoveAt(j);
        OnStudentCountChanged("Видалено елемент з колекції.", removedStudent);
        return true;
    }

    public Student this[int index]
    {
        get => _students[index];
        set
        {
            _students[index] = value;
            OnStudentReferenceChanged("Замінено елемент у колекції.", value);
        }
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
            return $"{CollectionName}: колекція студентів порожня.";
        }

        return $"{CollectionName}{Environment.NewLine}" + string.Join(
            $"{Environment.NewLine}{new string('-', 70)}{Environment.NewLine}",
            _students.Select(student => student.ToString())
        );
    }

    public string ToShortString()
    {
        if (_students.Count == 0)
        {
            return $"{CollectionName}: колекція студентів порожня.";
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
        return TestDataFactory.GenerateStudent(index);
    }

    private void OnStudentCountChanged(string changeType, Student student)
    {
        StudentCountChanged?.Invoke(this, new StudentListHandlerEventArgs(CollectionName, changeType, student));
    }

    private void OnStudentReferenceChanged(string changeType, Student student)
    {
        StudentReferenceChanged?.Invoke(this, new StudentListHandlerEventArgs(CollectionName, changeType, student));
    }
}

public class JournalEntry
{
    public JournalEntry(string collectionName, string changeType, string studentData)
    {
        CollectionName = collectionName;
        ChangeType = changeType;
        StudentData = studentData;
    }

    public string CollectionName { get; init; }

    public string ChangeType { get; init; }

    public string StudentData { get; init; }

    public override string ToString()
    {
        return $"Колекція: {CollectionName}, Тип зміни: {ChangeType}, Дані студента: {StudentData}";
    }
}

public class Journal
{
    private readonly List<JournalEntry> _entries = new();

    public void OnStudentCountChanged(object source, StudentListHandlerEventArgs args)
    {
        _entries.Add(new JournalEntry(args.CollectionName, args.ChangeType, args.Student.ToShortString()));
    }

    public void OnStudentReferenceChanged(object source, StudentListHandlerEventArgs args)
    {
        _entries.Add(new JournalEntry(args.CollectionName, args.ChangeType, args.Student.ToShortString()));
    }

    public override string ToString()
    {
        if (_entries.Count == 0)
        {
            return "Журнал порожній.";
        }

        return string.Join(Environment.NewLine, _entries.Select(entry => entry.ToString()));
    }
}

public static class TestDataFactory
{
    public static Student GenerateStudent(int index)
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
}

public record SearchScenario(string Label, Person Person, string StringKey, Student Student);

public record SearchMeasurement(
    string CollectionType,
    string ScenarioLabel,
    long PersonLookupTicks,
    long StringLookupTicks,
    long PersonDictionaryKeyLookupTicks,
    long StringDictionaryKeyLookupTicks,
    long DictionaryValueLookupTicks);

public interface IBenchmarkCollections
{
    string CollectionType { get; }
    IReadOnlyList<SearchMeasurement> MeasureSearchTimes();
}

public sealed class StandardTestCollections : IBenchmarkCollections
{
    private readonly List<Person> _persons;
    private readonly List<string> _personStrings;
    private readonly Dictionary<Person, Student> _personStudentDictionary;
    private readonly Dictionary<string, Student> _stringStudentDictionary;

    public StandardTestCollections(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        _persons = new List<Person>(count);
        _personStrings = new List<string>(count);
        _personStudentDictionary = new Dictionary<Person, Student>(count);
        _stringStudentDictionary = new Dictionary<string, Student>(count);

        for (int i = 0; i < count; i++)
        {
            var student = TestDataFactory.GenerateStudent(i);
            var person = student.PersonKey;
            var stringKey = person.ToString();

            _persons.Add(person);
            _personStrings.Add(stringKey);
            _personStudentDictionary.Add(person, student);
            _stringStudentDictionary.Add(stringKey, student);
        }
    }

    public string CollectionType => "Standard";

    public IReadOnlyList<SearchMeasurement> MeasureSearchTimes()
    {
        if (_persons.Count == 0)
        {
            return Array.Empty<SearchMeasurement>();
        }

        return CreateScenarios(_persons, _personStrings, _personStudentDictionary)
            .Select(scenario => new SearchMeasurement(
                CollectionType,
                scenario.Label,
                Measure(() => _persons.Contains(scenario.Person)),
                Measure(() => _personStrings.Contains(scenario.StringKey)),
                Measure(() => _personStudentDictionary.ContainsKey(scenario.Person)),
                Measure(() => _stringStudentDictionary.ContainsKey(scenario.StringKey)),
                Measure(() => _personStudentDictionary.Values.Contains(scenario.Student))
            ))
            .ToList();
    }

    private static IEnumerable<SearchScenario> CreateScenarios(
        IReadOnlyList<Person> persons,
        IReadOnlyList<string> personStrings,
        IReadOnlyDictionary<Person, Student> personStudentDictionary)
    {
        yield return CreateScenario("першого елемента", 0, persons, personStrings, personStudentDictionary);
        yield return CreateScenario("центрального елемента", persons.Count / 2, persons, personStrings, personStudentDictionary);
        yield return CreateScenario("останнього елемента", persons.Count - 1, persons, personStrings, personStudentDictionary);
        yield return CreateMissingScenario();
    }

    private static SearchScenario CreateScenario(
        string label,
        int index,
        IReadOnlyList<Person> persons,
        IReadOnlyList<string> personStrings,
        IReadOnlyDictionary<Person, Student> personStudentDictionary)
    {
        var person = persons[index];
        return new SearchScenario(label, person, personStrings[index], personStudentDictionary[person]);
    }

    private static SearchScenario CreateMissingScenario()
    {
        var student = TestDataFactory.GenerateStudent(-1);
        var person = student.PersonKey;
        return new SearchScenario("елемента, що не входить в колекцію", person, person.ToString(), student);
    }

    private static long Measure(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.ElapsedTicks;
    }
}

public sealed class ImmutableTestCollections : IBenchmarkCollections
{
    private readonly ImmutableList<Person> _persons;
    private readonly ImmutableList<string> _personStrings;
    private readonly ImmutableDictionary<Person, Student> _personStudentDictionary;
    private readonly ImmutableDictionary<string, Student> _stringStudentDictionary;

    public ImmutableTestCollections(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        var personListBuilder = ImmutableList.CreateBuilder<Person>();
        var stringListBuilder = ImmutableList.CreateBuilder<string>();
        var personDictionaryBuilder = ImmutableDictionary.CreateBuilder<Person, Student>();
        var stringDictionaryBuilder = ImmutableDictionary.CreateBuilder<string, Student>();

        for (int i = 0; i < count; i++)
        {
            var student = TestDataFactory.GenerateStudent(i);
            var person = student.PersonKey;
            var stringKey = person.ToString();

            personListBuilder.Add(person);
            stringListBuilder.Add(stringKey);
            personDictionaryBuilder.Add(person, student);
            stringDictionaryBuilder.Add(stringKey, student);
        }

        _persons = personListBuilder.ToImmutable();
        _personStrings = stringListBuilder.ToImmutable();
        _personStudentDictionary = personDictionaryBuilder.ToImmutable();
        _stringStudentDictionary = stringDictionaryBuilder.ToImmutable();
    }

    public string CollectionType => "Immutable";

    public IReadOnlyList<SearchMeasurement> MeasureSearchTimes()
    {
        if (_persons.Count == 0)
        {
            return Array.Empty<SearchMeasurement>();
        }

        return CreateScenarios()
            .Select(scenario => new SearchMeasurement(
                CollectionType,
                scenario.Label,
                Measure(() => _persons.Contains(scenario.Person)),
                Measure(() => _personStrings.Contains(scenario.StringKey)),
                Measure(() => _personStudentDictionary.ContainsKey(scenario.Person)),
                Measure(() => _stringStudentDictionary.ContainsKey(scenario.StringKey)),
                Measure(() => _personStudentDictionary.Values.Contains(scenario.Student))
            ))
            .ToList();
    }

    private IEnumerable<SearchScenario> CreateScenarios()
    {
        yield return CreateScenario("першого елемента", 0);
        yield return CreateScenario("центрального елемента", _persons.Count / 2);
        yield return CreateScenario("останнього елемента", _persons.Count - 1);
        yield return CreateMissingScenario();
    }

    private SearchScenario CreateScenario(string label, int index)
    {
        var person = _persons[index];
        return new SearchScenario(label, person, _personStrings[index], _personStudentDictionary[person]);
    }

    private static SearchScenario CreateMissingScenario()
    {
        var student = TestDataFactory.GenerateStudent(-1);
        var person = student.PersonKey;
        return new SearchScenario("елемента, що не входить в колекцію", person, person.ToString(), student);
    }

    private static long Measure(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.ElapsedTicks;
    }
}

public sealed class SortedTestCollections : IBenchmarkCollections
{
    private readonly SortedList<Person, Person> _persons;
    private readonly SortedList<string, string> _personStrings;
    private readonly SortedDictionary<Person, Student> _personStudentDictionary;
    private readonly SortedDictionary<string, Student> _stringStudentDictionary;

    public SortedTestCollections(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        _persons = new SortedList<Person, Person>(count);
        _personStrings = new SortedList<string, string>(count);
        _personStudentDictionary = new SortedDictionary<Person, Student>();
        _stringStudentDictionary = new SortedDictionary<string, Student>();

        for (int i = 0; i < count; i++)
        {
            var student = TestDataFactory.GenerateStudent(i);
            var person = student.PersonKey;
            var stringKey = person.ToString();

            _persons.Add(person, person);
            _personStrings.Add(stringKey, stringKey);
            _personStudentDictionary.Add(person, student);
            _stringStudentDictionary.Add(stringKey, student);
        }
    }

    public string CollectionType => "Sorted";

    public IReadOnlyList<SearchMeasurement> MeasureSearchTimes()
    {
        if (_persons.Count == 0)
        {
            return Array.Empty<SearchMeasurement>();
        }

        return CreateScenarios()
            .Select(scenario => new SearchMeasurement(
                CollectionType,
                scenario.Label,
                Measure(() => _persons.ContainsKey(scenario.Person)),
                Measure(() => _personStrings.ContainsKey(scenario.StringKey)),
                Measure(() => _personStudentDictionary.ContainsKey(scenario.Person)),
                Measure(() => _stringStudentDictionary.ContainsKey(scenario.StringKey)),
                Measure(() => _personStudentDictionary.Values.Contains(scenario.Student))
            ))
            .ToList();
    }

    private IEnumerable<SearchScenario> CreateScenarios()
    {
        yield return CreateScenario("першого елемента", 0);
        yield return CreateScenario("центрального елемента", _persons.Count / 2);
        yield return CreateScenario("останнього елемента", _persons.Count - 1);
        yield return CreateMissingScenario();
    }

    private SearchScenario CreateScenario(string label, int index)
    {
        var person = _persons.Keys[index];
        var stringKey = _personStrings.Keys[index];
        return new SearchScenario(label, person, stringKey, _personStudentDictionary[person]);
    }

    private static SearchScenario CreateMissingScenario()
    {
        var student = TestDataFactory.GenerateStudent(-1);
        var person = student.PersonKey;
        return new SearchScenario("елемента, що не входить в колекцію", person, person.ToString(), student);
    }

    private static long Measure(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.ElapsedTicks;
    }
}

public static class BenchmarkPrinter
{
    public static void PrintComparison(IEnumerable<SearchMeasurement> measurements)
    {
        foreach (var scenarioGroup in measurements.GroupBy(measurement => measurement.ScenarioLabel))
        {
            Console.WriteLine($"Сценарій пошуку: {scenarioGroup.Key}");
            Console.WriteLine("Тип колекції | Пошук Person | Пошук string | Ключ Person | Ключ string | Значення Student");

            foreach (var measurement in scenarioGroup)
            {
                Console.WriteLine(
                    $"{measurement.CollectionType,-13} | " +
                    $"{measurement.PersonLookupTicks,12:N0} | " +
                    $"{measurement.StringLookupTicks,12:N0} | " +
                    $"{measurement.PersonDictionaryKeyLookupTicks,11:N0} | " +
                    $"{measurement.StringDictionaryKeyLookupTicks,11:N0} | " +
                    $"{measurement.DictionaryValueLookupTicks,15:N0}");
            }

            Console.WriteLine();
        }
    }
}

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var student = CreateCustomStudent(
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
        );

        Console.WriteLine("=== Початковий об'єкт Student ===");
        Console.WriteLine(student);
        Console.WriteLine();

        var copiedStudent = student.DeepCopy();
        student.AddExams(new Exam("Алгоритми", 3, new DateTime(2025, 2, 10)));

        Console.WriteLine("=== Оригінал після зміни ===");
        Console.WriteLine(student);
        Console.WriteLine();

        Console.WriteLine("=== DeepCopy об'єкта Student ===");
        Console.WriteLine(copiedStudent);
        Console.WriteLine();

        const string firstFileName = "student.json";
        const string secondFileName = "student-static.json";

        Console.WriteLine($"Збереження у файл {firstFileName}: {student.Save(firstFileName)}");

        var loadedStudent = new Student();
        Console.WriteLine($"Завантаження з файлу {firstFileName}: {loadedStudent.Load(firstFileName)}");
        Console.WriteLine("=== Об'єкт після Load ===");
        Console.WriteLine(loadedStudent);
        Console.WriteLine();

        Console.WriteLine($"Статичне збереження у файл {secondFileName}: {Student.Save(secondFileName, copiedStudent)}");
        var staticLoadedStudent = new Student();
        Console.WriteLine($"Статичне завантаження з файлу {secondFileName}: {Student.Load(secondFileName, staticLoadedStudent)}");
        Console.WriteLine("=== Об'єкт після static Load ===");
        Console.WriteLine(staticLoadedStudent);
        Console.WriteLine();

        Console.WriteLine("=== Додавання іспиту через консоль ===");
        var addResult = student.AddFromConsole();
        Console.WriteLine($"Результат AddFromConsole: {addResult}");
        Console.WriteLine("=== Student після AddFromConsole ===");
        Console.WriteLine(student);
        Console.WriteLine();
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
