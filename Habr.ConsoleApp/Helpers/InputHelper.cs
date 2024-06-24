using System;

namespace Habr.ConsoleApp.Helpers
{
    public static class InputHelper
    {
        public static string GetInputWithValidation(string prompt, Action<string> validate)
        {
            while (true)
            {
                Console.WriteLine($"\n{prompt} (or 0 to exit):");
                var input = Console.ReadLine()?.Trim();

                if (input == "0")
                {
                    return null;
                }

                try
                {
                    validate(input);
                    return input;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"\n{ex.Message}");
                }
            }
        }
    }
}
