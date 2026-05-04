using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lab7Variant;

public interface IHasName
{
    string Name { get; }
}

public enum Gender
{
    Male,
    Female
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class CoupleAttribute : Attribute
{
    public string Pair { get; init; } = string.Empty;

    public double Probability { get; init; }

    public string ChildType { get; init; } = string.Empty;
}

public sealed class SameGenderCoupleException : Exception
{
    public SameGenderCoupleException(string message)
        : base(message)
    {
    }
}

public abstract class Human : IHasName
{
    protected Human(string name, Gender gender)
    {
        Name = name;
        Gender = gender;
    }

    public string Name { get; set; }

    public Gender Gender { get; }

    public override string ToString()
    {
        return $"{GetType().Name}: {Name}";
    }
}

[Couple(Pair = nameof(Girl), Probability = 0.70, ChildType = nameof(Girl))]
[Couple(Pair = nameof(PrettyGirl), Probability = 1.00, ChildType = nameof(PrettyGirl))]
[Couple(Pair = nameof(SmartGirl), Probability = 0.50, ChildType = nameof(Girl))]
public sealed class Student : Human
{
    public Student(string name)
        : base(name, Gender.Male)
    {
    }
}

[Couple(Pair = nameof(Girl), Probability = 0.70, ChildType = nameof(SmartGirl))]
[Couple(Pair = nameof(PrettyGirl), Probability = 1.00, ChildType = nameof(PrettyGirl))]
[Couple(Pair = nameof(SmartGirl), Probability = 0.80, ChildType = nameof(Book))]
public sealed class Botan : Human
{
    public Botan(string name)
        : base(name, Gender.Male)
    {
    }
}

[Couple(Pair = nameof(Student), Probability = 0.70, ChildType = nameof(Girl))]
[Couple(Pair = nameof(Botan), Probability = 0.30, ChildType = nameof(SmartGirl))]
public class Girl : Human
{
    public Girl()
        : this("Безіменна")
    {
    }

    public Girl(string name)
        : base(name, Gender.Female)
    {
    }

    public string Patronymic { get; set; } = string.Empty;

    public string DreamChildName()
    {
        return "Софія";
    }
}

[Couple(Pair = nameof(Student), Probability = 0.40, ChildType = nameof(PrettyGirl))]
[Couple(Pair = nameof(Botan), Probability = 0.10, ChildType = nameof(PrettyGirl))]
public sealed class PrettyGirl : Human
{
    public PrettyGirl()
        : this("Безіменна")
    {
    }

    public PrettyGirl(string name)
        : base(name, Gender.Female)
    {
    }

    public string Patronymic { get; set; } = string.Empty;

    public string PreferredName(string prefix)
    {
        return $"{prefix}Лада";
    }

    public string DreamChildName()
    {
        return "Лада";
    }
}

[Couple(Pair = nameof(Student), Probability = 0.20, ChildType = nameof(Girl))]
[Couple(Pair = nameof(Botan), Probability = 0.50, ChildType = nameof(Book))]
public sealed class SmartGirl : Human
{
    public SmartGirl()
        : this("Безіменна")
    {
    }

    public SmartGirl(string name)
        : base(name, Gender.Female)
    {
    }

    public string Patronymic { get; set; } = string.Empty;

    public string DreamChildName()
    {
        return "Марта";
    }
}

public sealed class Book : IHasName
{
    public string Name { get; set; } = "Без назви";
}

public sealed record MeetingTrace(
    Human First,
    Human Second,
    bool FirstLikes,
    bool SecondLikes,
    string FirstMessage,
    string SecondMessage,
    string ChildTypeName);

public static class CoupleService
{
    private static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
    private static Random _random = new();

    public static MeetingTrace? LastMeetingTrace { get; private set; }

    public static void SetRandom(Random random)
    {
        _random = random;
    }

    public static IHasName? Couple(Human first, Human second)
    {
        if (first.Gender == second.Gender)
        {
            throw new SameGenderCoupleException(
                $"Неможлива зустріч: {first.Name} і {second.Name} мають однакову стать."
            );
        }

        var firstRule = FindCoupleRule(first.GetType(), second.GetType());
        var secondRule = FindCoupleRule(second.GetType(), first.GetType());

        var firstLikes = firstRule is not null && GetRandomAnswer(firstRule.Probability);
        var secondLikes = secondRule is not null && GetRandomAnswer(secondRule.Probability);

        LastMeetingTrace = new MeetingTrace(
            first,
            second,
            firstLikes,
            secondLikes,
            BuildOpinionMessage(first, second, firstLikes, firstRule?.Probability ?? 0),
            BuildOpinionMessage(second, first, secondLikes, secondRule?.Probability ?? 0),
            firstRule?.ChildType ?? "Немає"
        );

        if (!firstLikes || !secondLikes || firstRule is null)
        {
            return null;
        }

        var childName = ResolveChildName(second) ?? "Таємниче ім'я";
        var child = CreateChild(firstRule.ChildType, childName);
        SetPatronymicIfExists(child, first, second);
        return child;
    }

    public static bool GetRandomAnswer(double probability)
    {
        return _random.NextDouble() <= probability;
    }

    public static string GetObjectTypeName(IHasName? value)
    {
        return value?.GetType().Name ?? "Ніхто";
    }

    public static string GetObjectName(IHasName? value)
    {
        return value?.Name ?? "Немає";
    }

    private static CoupleAttribute? FindCoupleRule(Type sourceType, Type pairType)
    {
        foreach (var attribute in EnumerateCoupleAttributes(sourceType))
        {
            if (string.Equals(attribute.Pair, pairType.Name, StringComparison.Ordinal))
            {
                return attribute;
            }
        }

        return null;
    }

    private static IEnumerable<CoupleAttribute> EnumerateCoupleAttributes(Type type)
    {
        foreach (var attribute in type.GetCustomAttributes<CoupleAttribute>())
        {
            yield return attribute;
        }
    }

    private static string? ResolveChildName(Human second)
    {
        var stringMethods = second
            .GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(method => method.ReturnType == typeof(string) && !method.IsSpecialName);

        foreach (var method in stringMethods)
        {
            try
            {
                return method.Invoke(second, null) as string;
            }
            catch (TargetParameterCountException)
            {
            }
            catch (TargetInvocationException)
            {
            }
        }

        return second.Name;
    }

    private static IHasName CreateChild(string childTypeName, string childName)
    {
        var childType = CurrentAssembly
            .GetTypes()
            .FirstOrDefault(type => string.Equals(type.Name, childTypeName, StringComparison.Ordinal));

        if (childType is null)
        {
            throw new InvalidOperationException($"Не вдалося знайти тип дитини '{childTypeName}'.");
        }

        var child = Activator.CreateInstance(childType)
            ?? throw new InvalidOperationException($"Не вдалося створити тип '{childTypeName}'.");

        var nameProperty = childType.GetProperty(nameof(IHasName.Name));
        if (nameProperty?.CanWrite == true)
        {
            nameProperty.SetValue(child, childName);
        }

        return (IHasName)child;
    }

    private static void SetPatronymicIfExists(IHasName child, Human first, Human second)
    {
        var patronymicProperty = child.GetType().GetProperty("Patronymic");
        if (patronymicProperty?.CanWrite != true)
        {
            return;
        }

        var father = first.Gender == Gender.Male ? first : second;
        var childHuman = child as Human;
        var suffix = childHuman?.Gender == Gender.Male ? "ович" : "овна";
        patronymicProperty.SetValue(child, $"{father.Name}{suffix}");
    }

    private static string BuildOpinionMessage(Human source, Human target, bool likes, double probability)
    {
        var percent = probability * 100;
        return $"{source.GetType().Name} {source.Name} -> {target.GetType().Name} {target.Name}: {(likes ? "подобається" : "не подобається")} ({percent:0}%)";
    }
}

public static class Program
{
    private static Random _random = Random.Shared;

    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
        {
            WriteColoredLine("Сьогодні неділя. Консоль лабораторної роботи 7 не працює.", ConsoleColor.Red);
            return;
        }

        _random = CreateRandom(args);
        CoupleService.SetRandom(_random);
        var people = CreatePeoplePool();

        WriteColoredLine("Лабораторна робота 7", ConsoleColor.Cyan);
        WriteColoredLine("Enter: показати дві випадкові пари | Q/q або F10: вихід", ConsoleColor.DarkCyan);
        WriteColoredLine("Для повторюваного тесту можна запустити з параметром --seed=42", ConsoleColor.DarkGray);
        Console.WriteLine();

        PrintTwoPairs(people);

        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.F10 || key.Key == ConsoleKey.Q)
            {
                break;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                PrintTwoPairs(people);
            }
        }
    }

    private static void PrintTwoPairs(IReadOnlyList<Human> people)
    {
        for (int pairIndex = 1; pairIndex <= 2; pairIndex++)
        {
            var first = RandomHuman(people);
            var second = RandomHuman(people);

            WriteColoredLine($"Пара {pairIndex}: {first} + {second}", ConsoleColor.Yellow);

            try
            {
                var child = CoupleService.Couple(first, second);
                PrintTrace(CoupleService.LastMeetingTrace);
                WriteColoredLine($"Результат тип: {CoupleService.GetObjectTypeName(child)}", ConsoleColor.Green);
                WriteColoredLine($"Результат ім'я: {CoupleService.GetObjectName(child)}", ConsoleColor.Green);

                if (child is not null)
                {
                    var patronymic = child.GetType().GetProperty("Patronymic")?.GetValue(child) as string;
                    if (!string.IsNullOrWhiteSpace(patronymic))
                    {
                        WriteColoredLine($"По батькові: {patronymic}", ConsoleColor.DarkGreen);
                    }
                }
            }
            catch (SameGenderCoupleException ex)
            {
                WriteColoredLine(ex.Message, ConsoleColor.Red);
            }
            catch (Exception ex)
            {
                WriteColoredLine($"Помилка: {ex.Message}", ConsoleColor.Red);
            }

            Console.WriteLine(new string('-', 70));
        }
    }

    private static void PrintTrace(MeetingTrace? trace)
    {
        if (trace is null)
        {
            return;
        }

        WriteColoredLine(trace.FirstMessage, trace.FirstLikes ? ConsoleColor.Magenta : ConsoleColor.DarkGray);
        WriteColoredLine(trace.SecondMessage, trace.SecondLikes ? ConsoleColor.Magenta : ConsoleColor.DarkGray);
        WriteColoredLine($"Очікуваний тип дитини за правилом першого класу: {trace.ChildTypeName}", ConsoleColor.Blue);
    }

    private static IReadOnlyList<Human> CreatePeoplePool()
    {
        return new Human[]
        {
            new Student("Андрій"),
            new Student("Олексій"),
            new Botan("Богдан"),
            new Botan("Роман"),
            new Girl("Олена"),
            new Girl("Марія"),
            new PrettyGirl("Лада"),
            new PrettyGirl("Ірина"),
            new SmartGirl("Марта"),
            new SmartGirl("Дарина")
        };
    }

    private static Human RandomHuman(IReadOnlyList<Human> people)
    {
        return people[_random.Next(people.Count)];
    }

    private static Random CreateRandom(string[] args)
    {
        var seedArgument = args.FirstOrDefault(arg => arg.StartsWith("--seed=", StringComparison.OrdinalIgnoreCase));
        if (seedArgument is null)
        {
            return Random.Shared;
        }

        var seedValue = seedArgument.Split('=', 2)[1];
        return int.TryParse(seedValue, out var seed) ? new Random(seed) : Random.Shared;
    }

    private static void WriteColoredLine(string text, ConsoleColor color)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = previousColor;
    }
}
