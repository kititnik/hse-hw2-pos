using System.Globalization;

namespace Pos.ConsoleApp;

public sealed class ConsoleInput
{
    public string ReadText(string prompt)
    {
        while (true)
        {
            var line = ReadLine(prompt).Trim();
            if (line.Length > 0)
                return line;

            Console.WriteLine("Значение не может быть пустым.");
        }
    }

    public string ReadOptionalText(string prompt)
    {
        return ReadLine(prompt);
    }

    public int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            var line = ReadLine(prompt);
            if (int.TryParse(line, out var value)
                && value >= min && value <= max)
                return value;

            Console.WriteLine($"Введите целое число от {min} до {max}.");
        }
    }

    public decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            var line = ReadLine(prompt).Replace(',', '.');
            if (decimal.TryParse(line, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                return value;

            Console.WriteLine("Введите число.");
        }
    }

    private static string ReadLine(string prompt)
    {
        Console.Write($"{prompt}: ");
        return Console.ReadLine() ?? throw new EndOfStreamException();
    }
}
