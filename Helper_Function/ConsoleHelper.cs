
using Spectre.Console;
namespace University_Academic.Helper_Function;

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


    public static void PrintTable(string[] headers, IEnumerable<string[]> rows, bool pause = true, bool rowSeparators = false)
    {
        if (rows == null || rows.Count() == 0)
        {
            ShowWarningMessage("No records found to display.");
            return;
        }

        var table = new Table();
        table.Border(TableBorder.Rounded);
        table.BorderColor(Color.Cyan1);
        if (rowSeparators)
        {
            table.ShowRowSeparators();
        }
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
        if (pause) Pause();


    }

    #endregion

    #region Manu Display
    public static void PrintMenu(string title, List<string> options)
    {
        Console.Clear();

        // Color definitions using ANSI escape sequences
        string cyan = "\u001b[36;1m";
        string yellow = "\u001b[33;1m";
        string green = "\u001b[32;1m";
        string red = "\u001b[31;1m";
        string reset = "\u001b[0m";

        Console.WriteLine($"{cyan}╔═════════════════════════════════════════════════════════════════════════╗{reset}");
        Console.WriteLine($"{cyan}║{reset}                 {yellow}{title,-56}{reset}{cyan}║{reset}");
        Console.WriteLine($"{cyan}╠═════════════════════════════════════════════════════════════════════════╣{reset}");
        Console.WriteLine($"{cyan}║{reset}                                                                         {cyan}║{reset}");


        for (int i = 0; i < options.Count; i += 2)
        {
            string item1 = $"[ {i + 1} ] {options[i]}";
            string item2 = (i + 1 < options.Count) ? $"[ {i + 2} ] {options[i + 1]}" : "";


            Console.WriteLine($"{cyan}║{reset}    {green}{item1,-34}{green}{item2,-35}{cyan}║{reset}");
        }

        Console.WriteLine($"{cyan}║{reset}    {red}[ 0 ]{reset} Exit                                                           {cyan}║{reset}");
        Console.WriteLine($"{cyan}║{reset}                                                                         {cyan}║{reset}");
        Console.WriteLine($"{cyan}╚═════════════════════════════════════════════════════════════════════════╝{reset}");
        Console.WriteLine();
    }
    #endregion


    public static ServiceResult<int> GetSelected<T>(
        List<T> items,
        Func<T, int> idSelector,
        Func<T, string> displaySelector,
        string itemName,
        string print2 = "Name"
        )
    {
        if (items == null || !items.Any())
        {
            return ServiceResult<int>.Failure($"No {itemName} available. Please create a {itemName} first.");
        }
        //foreach (var item in items)
        //{
        //    Console.WriteLine($"ID = {idSelector(item) , -4} | {print2} = {displaySelector(item)}");
        //}
        string[] header = { "ID", $"{print2}" };
        var rows = items.Select(e => new string[]
        {
            $"[white]{idSelector(e)}[/]",
            $"[cyan]{displaySelector(e)}[/]",
        });
        PrintTable(header, rows, false, true);
        string Prompt = $"Enter {itemName} Id :";
        return ServiceResult<int>.Success(chekInt(Prompt));
    }
}
