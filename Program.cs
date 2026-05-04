using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Lab8Variant1;

public readonly record struct MatrixPosition(int Row, int Column);

public readonly record struct MatrixCellResult(int Row, int Column, double Value);

public sealed class Matrix
{
    private readonly double[,] _data;

    public Matrix(int rows, int columns)
    {
        if (rows <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rows), "Кількість рядків повинна бути додатною.");
        }

        if (columns <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(columns), "Кількість стовпців повинна бути додатною.");
        }

        _data = new double[rows, columns];
    }

    public int Rows => _data.GetLength(0);

    public int Columns => _data.GetLength(1);

    public double this[int row, int column]
    {
        get => _data[row, column];
        set => _data[row, column] = value;
    }
}

public static class MatrixGenerator
{
    public static Matrix Generate(int rows, int columns, int minValue = 0, int maxValue = 10)
    {
        var matrix = new Matrix(rows, columns);
        var random = Random.Shared;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                matrix[row, column] = random.Next(minValue, maxValue);
            }
        }

        return matrix;
    }
}

public static class MatrixPrinter
{
    public static void PrintIfSmall(string title, Matrix matrix, int maxRows = 8, int maxColumns = 8)
    {
        Console.WriteLine(title);

        if (matrix.Rows > maxRows || matrix.Columns > maxColumns)
        {
            Console.WriteLine($"Матриця {matrix.Rows}x{matrix.Columns} завелика для повного виводу.");
            Console.WriteLine();
            return;
        }

        for (int row = 0; row < matrix.Rows; row++)
        {
            for (int column = 0; column < matrix.Columns; column++)
            {
                Console.Write($"{matrix[row, column],8:F0}");
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }
}

public static class MatrixMultiplier
{
    public static async Task<Matrix> MultiplyAsync(
        Matrix left,
        Matrix right,
        CancellationToken cancellationToken)
    {
        if (left.Columns != right.Rows)
        {
            throw new InvalidOperationException(
                $"Неможливо перемножити матриці {left.Rows}x{left.Columns} та {right.Rows}x{right.Columns}."
            );
        }

        var result = new Matrix(left.Rows, right.Columns);
        var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
        var executionOptions = new ExecutionDataflowBlockOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            BoundedCapacity = Environment.ProcessorCount * 4,
            EnsureOrdered = false
        };

        var writeOptions = new ExecutionDataflowBlockOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = 1,
            BoundedCapacity = Environment.ProcessorCount * 4,
            EnsureOrdered = false
        };

        var computeCellBlock = new TransformBlock<MatrixPosition, MatrixCellResult>(
            position => ComputeCell(left, right, position, cancellationToken),
            executionOptions
        );

        var writeCellBlock = new ActionBlock<MatrixCellResult>(
            cell =>
            {
                result[cell.Row, cell.Column] = cell.Value;
            },
            writeOptions
        );

        computeCellBlock.LinkTo(writeCellBlock, linkOptions);

        try
        {
            for (int row = 0; row < left.Rows; row++)
            {
                for (int column = 0; column < right.Columns; column++)
                {
                    await computeCellBlock.SendAsync(new MatrixPosition(row, column), cancellationToken);
                }
            }

            computeCellBlock.Complete();
            await writeCellBlock.Completion;
            return result;
        }
        catch
        {
            computeCellBlock.Complete();
            throw;
        }
    }

    private static MatrixCellResult ComputeCell(
        Matrix left,
        Matrix right,
        MatrixPosition position,
        CancellationToken cancellationToken)
    {
        double sum = 0;

        for (int index = 0; index < left.Columns; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            sum += left[position.Row, index] * right[index, position.Column];
        }

        return new MatrixCellResult(position.Row, position.Column, sum);
    }
}

public static class Program
{
    public static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        PrintIntro();

        while (true)
        {
            var leftRows = ReadPositiveInt("Введіть кількість рядків першої матриці: ");
            var leftColumns = ReadPositiveInt("Введіть кількість стовпців першої матриці: ");
            var rightRows = ReadPositiveInt("Введіть кількість рядків другої матриці: ");
            var rightColumns = ReadPositiveInt("Введіть кількість стовпців другої матриці: ");

            if (leftColumns != rightRows)
            {
                WriteColoredLine(
                    $"Помилка: матриці {leftRows}x{leftColumns} і {rightRows}x{rightColumns} не можна перемножити.",
                    ConsoleColor.Red
                );

                if (!AskRepeat())
                {
                    return;
                }

                Console.WriteLine();
                continue;
            }

            WriteColoredLine("Генерація матриць...", ConsoleColor.Cyan);
            var leftMatrix = MatrixGenerator.Generate(leftRows, leftColumns);
            var rightMatrix = MatrixGenerator.Generate(rightRows, rightColumns);

            MatrixPrinter.PrintIfSmall("Перша матриця:", leftMatrix);
            MatrixPrinter.PrintIfSmall("Друга матриця:", rightMatrix);

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += OnCancelKeyPress;

            var cancelWatcher = Task.Run(() => WatchForQuitKey(cts), CancellationToken.None);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                WriteColoredLine("Почалося множення. Натисніть Q для скасування або Ctrl+C.", ConsoleColor.Yellow);
                var result = await MatrixMultiplier.MultiplyAsync(leftMatrix, rightMatrix, cts.Token);
                stopwatch.Stop();

                WriteColoredLine(
                    $"Множення завершено успішно за {stopwatch.Elapsed.TotalMilliseconds:F2} мс.",
                    ConsoleColor.Green
                );
                MatrixPrinter.PrintIfSmall("Результуюча матриця:", result);
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();
                WriteColoredLine("Операцію множення скасовано користувачем.", ConsoleColor.Red);
            }
            finally
            {
                cts.Cancel();
                await cancelWatcher;
                Console.CancelKeyPress -= OnCancelKeyPress;
            }

            if (!AskRepeat())
            {
                return;
            }

            Console.WriteLine();

            void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs args)
            {
                args.Cancel = true;
                cts.Cancel();
            }
        }
    }

    private static void WatchForQuitKey(CancellationTokenSource cts)
    {
        while (!cts.IsCancellationRequested)
        {
            if (!Console.KeyAvailable)
            {
                Thread.Sleep(50);
                continue;
            }

            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Q)
            {
                cts.Cancel();
                return;
            }
        }
    }

    private static bool AskRepeat()
    {
        Console.Write("Повторити обчислення? (Enter = так, Q = вихід): ");
        var key = Console.ReadKey(true);
        Console.WriteLine();
        return key.Key != ConsoleKey.Q;
    }

    private static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) && value > 0)
            {
                return value;
            }

            WriteColoredLine("Помилка введення. Потрібно ввести додатне ціле число.", ConsoleColor.Red);
        }
    }

    private static void PrintIntro()
    {
        WriteColoredLine("Лабораторна робота 8. Множення прямокутних матриць через Dataflow.", ConsoleColor.Cyan);
        WriteColoredLine("Програма генерує дві матриці за заданими розмірами і перемножує їх.", ConsoleColor.DarkCyan);
        WriteColoredLine("Кожен елемент результату обчислюється як окреме повідомлення Dataflow.", ConsoleColor.DarkCyan);
        WriteColoredLine("Під час множення можна натиснути Q або Ctrl+C для скасування.", ConsoleColor.DarkCyan);
        Console.WriteLine();
    }

    private static void WriteColoredLine(string text, ConsoleColor color)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = previousColor;
    }
}
