using Habr.ConsoleApp.Resources;

namespace Habr.ConsoleApp.Helpers
{
    public static class InputHelper
    {
        public static string GetInputWithValidation(string prompt, Action<string> validate)
        {
            while (true)
            {
                Console.WriteLine(string.Format(Messages.ZeroToExit, prompt));
                var input = Console.ReadLine()?.Trim();

                if (input == Messages.Zero)
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
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
