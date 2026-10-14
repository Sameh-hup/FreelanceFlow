
using Spectre.Console;
namespace University_Academic_System.Helper_Function;

public static class ConsoleHelper
{

    #region Input Handlers

    public static int ReadInt(string prompt, string errorMessage = "Invalid input! Please enter a number ")

    {

        Console.Write(prompt);

        if (!int.TryParse(Console.ReadLine(), out int result))

        {

            Console.ForegroundColor = ConsoleColor.Red;

            ShowErrorMessage($"Error: {errorMessage}");

            Console.ResetColor();

            return -1;

        }

        return result;

    }
    public static string chekstring(string prompt, string errorMessage = "Input cannot be empty! Please try again.")
    {
        string input;
        do
        {
            input = AnsiConsole.Ask<string>($"[green]{prompt}[/]");
            if (string.IsNullOrWhiteSpace(input))
            {
                ShowErrorMessage(errorMessage);
            }
        } while (string.IsNullOrWhiteSpace(input));

        return input;
    }

    public static int chekInt(string prompt, string errorMessage = "Invalid input! Please enter a valid number.")
    {
        while (true)
        {
            string input = AnsiConsole.Ask<string>($"[green]{prompt}[/]");
            if (int.TryParse(input, out int result) && result >= 0)
            {
                return result;
            }

            ShowErrorMessage(errorMessage);
        }
    }

    public static double chekdouble(string prompt, string errorMessage = "Invalid input! Please enter a valid number.")
    {
        while (true)
        {
            string input = AnsiConsole.Ask<string>($"[green]{prompt}[/]");
            if (double.TryParse(input, out double result) && result >= 0)
            {
                return result;
            }

            ShowErrorMessage(errorMessage);
        }
    }

    #endregion

    #region Title & Messages 

    public static void PrintTitle(string title)
    {
        AnsiConsole.Write(
            new Rule($"[yellow]{title.ToUpper()}[/]")
                .RuleStyle("cyan")
                .Centered());
        Console.WriteLine();
    }

    public static void ShowSuccessMessage(string message)
    {
        Console.WriteLine();
        AnsiConsole.Write(
            new Panel($"[bold green] {message}[/]")
                .BorderColor(Color.Green)
                .Padding(1, 0));
        Pause();
    }

    public static void ShowErrorMessage(string message)
    {
        Console.WriteLine();
        AnsiConsole.Write(
            new Panel($"[bold red] {message}[/]")
                .BorderColor(Color.Red)
                .Padding(1, 0));
        Pause();
    }

    public static void ShowWarningMessage(string message)
    {
        Console.WriteLine();
        AnsiConsole.Write(
            new Panel($"[bold yellow] {message}[/]")
                .BorderColor(Color.Yellow)
                .Padding(1, 0));
        Pause();
    }

    public static void Pause()
    {
        Console.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to return to menu...[/]");
        Console.ReadKey(true);
    }

    #endregion

    #region Generic Table Display 

   
    public static void PrintTable(string[] headers, IEnumerable<string[]> rows)
    {
        if (rows == null || rows.Count() == 0)
        {
            ShowWarningMessage("No records found to display.");
            return;
        }

        var table = new Table();
        table.Border(TableBorder.Rounded);
        table.BorderColor(Color.Cyan1);
        table.Expand();

        
        foreach (var header in headers)
        {
            table.AddColumn(new TableColumn($"[bold yellow]{header}[/]").Centered());
        }

        
        foreach (var row in rows)
        {
            table.AddRow(row);
        }

        AnsiConsole.Write(table);
        Pause();
    }

    #endregion
}
