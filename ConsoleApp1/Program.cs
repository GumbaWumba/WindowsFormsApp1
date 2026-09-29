using Spectre.Console;
using System;
using System.Threading;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Failed()
        {
            AnsiConsole.Clear();

            var table = new Table()
                .AddColumn("Quiz")
                .AddColumn("[red]YOU FAILED, GET BETTER![/]");

            AnsiConsole.Write(table);
        }

        static bool AskQuestion(string questionNumber, string question, string correctAnswer, params string[] answers)
        {
            AnsiConsole.Clear();

            var table = new Table()
                .AddColumn("Quiz")
                .AddColumn($"Question {questionNumber}")
                .AddRow("[red]INFO[/]", question);

            AnsiConsole.Write(table);

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an [green]answer[/]:")
                    .AddChoices(answers)
            );

            if (choice != correctAnswer)
            {
                Failed();
                return false;
            }

            AnsiConsole.MarkupLine("[green]Correct![/]");
            Thread.Sleep(1500);

            return true;
        }

        static void Main(string[] args)
        {
            AnsiConsole.MarkupLine("[bold blue]Welcome[/] to [green]The Quiz[/]!");

            AnsiConsole.Status()
                .Start("Processing...", ctx =>
                {
                    Thread.Sleep(2500);
                });

            AnsiConsole.MarkupLine("[green]Done![/]");
            AnsiConsole.Clear();

            var infoTable = new Table()
                .AddColumn("Quiz")
                .AddColumn("for Roman")
                .AddRow("[red]INFO[/]", "The Quiz is hard")
                .AddRow("[green]ADVICE[/]", "Write Start to continue");

            AnsiConsole.Write(infoTable);

            string input = Console.ReadLine();

            if (input != "Start")
            {
                Failed();
                return;
            }

            if (!AskQuestion(
                "1",
                "What day is it?",
                "Monday",
                "Monday",
                "Tuesday",
                "Wednessday",
                "Thursday",
                "Friday",
                "Saturday",
                "Sunday"
            ))
            {
                return;
            }

            if (!AskQuestion(
                "2",
                "What was this made with?",
                "Visual Studio",
                "Windows Forms",
                "Visual Studio",
                "C#",
                "Console"
            ))
            {
                return;
            }

            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold green]You completed the quiz![/]");
        }
    }
}