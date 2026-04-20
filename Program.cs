using System;
using System.Globalization;
using System.Text;

namespace Lab1Variant1;

public enum Education
{
    Master,
    Bachelor,
    SecondEducation
}

public class Person
{
    private string _firstName;
    private string _lastName;
    private DateTime _birthDate;

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

    public override string ToString()
    {
        return $"Ім'я: {_firstName}, Прізвище: {_lastName}, Дата народження: {_birthDate:dd.MM.yyyy}";
    }

    public virtual string ToShortString()
    {
        return $"{_lastName} {_firstName}";
    }
}

public class Exam
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

    public override string ToString()
    {
        return $"Предмет: {Subject}, Оцінка: {Grade}, Дата іспиту: {ExamDate:dd.MM.yyyy}";
    }
}

public class Student
{
    private Person _person;
    private Education _education;
    private int _groupNumber;
    private Exam[] _exams;

    public Student(Person person, Education education, int groupNumber)
    {
        _person = person;
        _education = education;
        _groupNumber = groupNumber;
        _exams = Array.Empty<Exam>();
    }

    public Student()
    {
        _person = new Person();
        _education = Education.Bachelor;
        _groupNumber = 101;
        _exams = Array.Empty<Exam>();
    }

    public Person PersonData
    {
        get => _person;
        init => _person = value;
    }

    public Education EducationForm
    {
        get => _education;
        init => _education = value;
    }

    public int GroupNumber
    {
        get => _groupNumber;
        init => _groupNumber = value;
    }

    public Exam[] Exams
    {
        get => _exams;
        init => _exams = value ?? Array.Empty<Exam>();
    }

    public double AverageGrade
    {
        get
        {
            if (_exams.Length == 0)
                return 0.0;

            double sum = 0;
            foreach (var exam in _exams)
            {
                sum += exam.Grade;
            }

            return sum / _exams.Length;
        }
    }

    public bool this[Education education] => _education == education;

    public void AddExams(params Exam[] exams)
    {
        if (exams == null || exams.Length == 0)
            return;

        var newArray = new Exam[_exams.Length + exams.Length];

        for (int i = 0; i < _exams.Length; i++)
        {
            newArray[i] = _exams[i];
        }

        for (int i = 0; i < exams.Length; i++)
        {
            newArray[_exams.Length + i] = exams[i];
        }

        _exams = newArray;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Дані студента:");
        sb.AppendLine(_person.ToString());
        sb.AppendLine($"Форма навчання: {_education}");
        sb.AppendLine($"Номер групи: {_groupNumber}");
        sb.AppendLine($"Середній бал: {AverageGrade:F2}");
        sb.AppendLine("Іспити:");

        if (_exams.Length == 0)
        {
            sb.AppendLine("  Немає іспитів.");
        }
        else
        {
            for (int i = 0; i < _exams.Length; i++)
            {
                sb.AppendLine($"  {i + 1}. {_exams[i]}");
            }
        }

        return sb.ToString();
    }

    public virtual string ToShortString()
    {
        return $"Студент: {_person.ToShortString()}, Форма навчання: {_education}, " +
               $"Група: {_groupNumber}, Середній бал: {AverageGrade:F2}";
    }
}

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // 1. Створити один об'єкт Student, викликати ToShortString()
        var student = new Student();
        Console.WriteLine("1. Об'єкт Student за замовчуванням:");
        Console.WriteLine(student.ToShortString());
        Console.WriteLine(new string('-', 70));

        // 2. Вивести значення індексатора
        Console.WriteLine("2. Значення індексатора:");
        Console.WriteLine($"Education.Master: {student[Education.Master]}");
        Console.WriteLine($"Education.Bachelor: {student[Education.Bachelor]}");
        Console.WriteLine($"Education.SecondEducation: {student[Education.SecondEducation]}");
        Console.WriteLine(new string('-', 70));

        // 3. Присвоїти значення всім властивостям та вивести ToString()
        student = new Student(
            new Person("Роман", "Попов", new DateTime(2006, 8, 23)),
            Education.Master,
            321
        )
        {
            Exams = new Exam[]
            {
                new Exam("Математика", 95, new DateTime(2025, 6, 10)),
                new Exam("Програмування", 98, new DateTime(2025, 6, 15))
            }
        };

        Console.WriteLine("3. Після присвоєння значень властивостям:");
        Console.WriteLine(student);
        Console.WriteLine(new string('-', 70));

        // 4. Додати іспити через AddExams()
        student.AddExams(
            new Exam("Фізика", 88, new DateTime(2025, 6, 20)),
            new Exam("Англійська", 91, new DateTime(2025, 6, 25))
        );

        Console.WriteLine("4. Після AddExams():");
        Console.WriteLine(student);
        Console.WriteLine(new string('-', 70));

        // 5. Порівняння часу роботи з масивами Exam
        Console.WriteLine("5. Порівняння часу виконання операцій з масивами Exam");
        Console.WriteLine("Введіть кількість рядків і стовпців через один із розділювачів: пробіл, кома, крапка з комою");
        Console.Write("Приклад: 1000,200 або 1000 200 або 1000;200\n> ");

        string? input = Console.ReadLine();
        char[] separators = [' ', ',', ';'];

        string[] parts = (input ?? "").Split(separators, StringSplitOptions.RemoveEmptyEntries);

        int nRows = int.Parse(parts[0], CultureInfo.InvariantCulture);
        int nColumns = int.Parse(parts[1], CultureInfo.InvariantCulture);

        int totalElements = nRows * nColumns;

        // Одновимірний масив
        Exam[] oneDimensional = new Exam[totalElements];
        for (int i = 0; i < totalElements; i++)
        {
            oneDimensional[i] = new Exam();
        }

        // Двовимірний прямокутний масив
        Exam[,] rectangular = new Exam[nRows, nColumns];
        for (int i = 0; i < nRows; i++)
        {
            for (int j = 0; j < nColumns; j++)
            {
                rectangular[i, j] = new Exam();
            }
        }

        // Зубчастий масив з однаковою кількістю елементів у кожному рядку
        Exam[][] jaggedEqual = new Exam[nRows][];
        for (int i = 0; i < nRows; i++)
        {
            jaggedEqual[i] = new Exam[nColumns];
            for (int j = 0; j < nColumns; j++)
            {
                jaggedEqual[i][j] = new Exam();
            }
        }

        // Зубчастий масив типу 1,2,3,..., nAll - nActual у відповідному стовпці
        // Робимо довжини рядків так, щоб сумарна кількість елементів була nRows * nColumns
        Exam[][] jaggedVariable = CreateJaggedVariable(nRows, totalElements);

        // Вимірюємо час однієї й тієї ж операції:
        // присвоєння значення властивості Grade
        int start;
        int end;

        start = Environment.TickCount;
        for (int i = 0; i < oneDimensional.Length; i++)
        {
            oneDimensional[i].Grade = 100;
        }
        end = Environment.TickCount;
        int oneDimensionalTime = end - start;

        start = Environment.TickCount;
        for (int i = 0; i < nRows; i++)
        {
            for (int j = 0; j < nColumns; j++)
            {
                rectangular[i, j].Grade = 100;
            }
        }
        end = Environment.TickCount;
        int rectangularTime = end - start;

        start = Environment.TickCount;
        for (int i = 0; i < jaggedEqual.Length; i++)
        {
            for (int j = 0; j < jaggedEqual[i].Length; j++)
            {
                jaggedEqual[i][j].Grade = 100;
            }
        }
        end = Environment.TickCount;
        int jaggedEqualTime = end - start;

        start = Environment.TickCount;
        for (int i = 0; i < jaggedVariable.Length; i++)
        {
            for (int j = 0; j < jaggedVariable[i].Length; j++)
            {
                jaggedVariable[i][j].Grade = 100;
            }
        }
        end = Environment.TickCount;
        int jaggedVariableTime = end - start;

        Console.WriteLine($"Кількість рядків: {nRows}");
        Console.WriteLine($"Кількість стовпців: {nColumns}");
        Console.WriteLine($"Загальна кількість елементів: {totalElements}");
        Console.WriteLine();

        Console.WriteLine($"Одновимірний масив: {oneDimensionalTime} мс");
        Console.WriteLine($"Двовимірний прямокутний масив: {rectangularTime} мс");
        Console.WriteLine($"Двовимірний зубчастий масив (рівні рядки): {jaggedEqualTime} мс");
        Console.WriteLine($"Двовимірний зубчастий масив (змінні рядки): {jaggedVariableTime} мс");
    }

    private static Exam[][] CreateJaggedVariable(int nRows, int totalElements)
    {
        var jagged = new Exam[nRows][];

        // Базова схема: 1,2,3,...,nRows
        int baseSum = nRows * (nRows + 1) / 2;

        int[] lengths = new int[nRows];

        if (baseSum <= totalElements)
        {
            for (int i = 0; i < nRows; i++)
            {
                lengths[i] = i + 1;
            }

            int remaining = totalElements - baseSum;
            int rowIndex = nRows - 1;

            while (remaining > 0)
            {
                lengths[rowIndex]++;
                remaining--;
                rowIndex--;

                if (rowIndex < 0)
                    rowIndex = nRows - 1;
            }
        }
        else
        {
            // Якщо totalElements замало для чистої схеми 1..nRows,
            // просто розподіляємо елементи нерівномірно, але так,
            // щоб усі рядки існували і сумарна кількість збігалася.
            Array.Fill(lengths, 0);

            int remaining = totalElements;
            int currentLength = 1;
            int row = 0;

            while (remaining > 0)
            {
                int add = Math.Min(currentLength, remaining);
                lengths[row] = add;
                remaining -= add;
                row++;
                currentLength++;

                if (row >= nRows)
                    break;
            }

            // Якщо ще залишилися елементи, додаємо їх з кінця
            row = nRows - 1;
            while (remaining > 0)
            {
                lengths[row]++;
                remaining--;
                row--;
                if (row < 0)
                    row = nRows - 1;
            }
        }

        for (int i = 0; i < nRows; i++)
        {
            jagged[i] = new Exam[lengths[i]];
            for (int j = 0; j < lengths[i]; j++)
            {
                jagged[i][j] = new Exam();
            }
        }

        return jagged;
    }
}