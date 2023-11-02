using System;
using System.Diagnostics;
using System.IO;

internal class Program
{
    /// <summary>
    /// Написать функцию, принимающую в качестве аргумента путь к папке.
    /// Из этой папки параллельно прочитать файлы и вычислить количество пробелов в них.
    /// </summary>
    /// <param name="path">Путь к папке</param>
    /// <param name="fileCount">Количество файлов, которые надо обработать</param>
    /// <returns>Время выполнения в мс</returns>
    private static long CheckFolder(string path, string searchPattern = "*.*", int fileCount = Int32.MaxValue)
    {
        // засекаем время работы
        var sw = Stopwatch.StartNew();

        // получаем список файлов в папке по маске
        var files = Directory.GetFiles(path, searchPattern);

        var tasksToWait = new List<Task>();
        foreach (var f in files)
        {
            if (--fileCount < 0)
                break;

            // таска, в которой подсчитывается количество пробелов в файле
            var task = new Task(() =>
            {
                var spaceCount = File.ReadAllText(f).Count(c => c == ' ');      // считаем пробелы
                Console.WriteLine($"\tFile: {Path.GetFileName(f)}, spaces: {spaceCount}");
            });
            task.Start();
            tasksToWait.Add(task);
        }

        // ожидаем завершение всех тасков
        Task.WaitAll(tasksToWait.ToArray());

        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    /// <summary>
    /// Генерация случайной строки.
    /// </summary>
    /// <param name="maxLength">Максимальная длина строки</param>
    /// <returns></returns>
    private static string RandomString(int maxLength = 1000)
    {
        var random = new Random((int)DateTime.Now.Ticks);
        var pool = "abcdefgABCDEFG0123456789 ";
        var chars = Enumerable.Range(0, random.Next(0, maxLength)).Select(x => pool[random.Next(0, pool.Length)]);
        return new string(chars.ToArray());
    }

    /// <summary>
    /// Генерация случайных текстовых файлов.
    /// </summary>
    /// <param name="path">Папка, где создаются файлы</param>
    /// <param name="count">Количество создаваемых файлов</param>
    /// <returns>Время выполнения в мс</returns>
    private static long GenerateFiles(string path, int count)
    {
        // засекаем время работы
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < count; i++)
            File.WriteAllText($"{path}\\{i}.txt", RandomString());

        sw.Stop();
        return sw.ElapsedMilliseconds;
    }

    /// <summary>
    /// Точка входа.
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        var path = Directory.GetCurrentDirectory();
        Console.WriteLine($"Path: {path}\n");

        // генерируем 3 текстовых файла
        Console.WriteLine("Generate 3 text files.");
        Console.WriteLine($"Executing time: {GenerateFiles(path, 3)} ms\n");

        // проверяем 3 текстовых файла
        Console.WriteLine("Check for spaces 3 text files.");
        Console.WriteLine($"Executing time: {CheckFolder(path, "*.txt", 3)} ms\n");

        // проверяем все файлы в папке
        Console.WriteLine("Check for spaces all files in folder.");
        Console.WriteLine($"Executing time: {CheckFolder(path, "*.*")} ms\n");

        Console.ReadKey();
    }
}
