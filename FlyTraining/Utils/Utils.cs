using System;
using System.Collections.Generic;
using System.Linq;

namespace FlyTraining.Utils
{
    public static class Utils
    {
        private static readonly Random Random = new Random();

        private static List<string> _availibleNames =
            GetColors();
        private static List<string> GetColors()
        {
            return Enum.GetNames(typeof(ConsoleColor)).Where(k => 
                    !(k.Equals("Gray") || k.Equals("Black") || k.Equals("Yellow")))
                .ToList();
        }


        public static ConsoleColor GetRandomDistinctColor()
        {
            var index = Random.Next(0, _availibleNames.Count);
            string sColor = _availibleNames[index];
            var returnColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), sColor);
            _availibleNames.Remove(sColor);
            if (_availibleNames.Count == 0)
            {
                _availibleNames = GetColors();
            }
            return returnColor;
        }
        public static void SetUpConsole()
        {
            Console.SetWindowPosition(0, 0);
            Console.BufferWidth = 2000;
            Console.BufferHeight = 140;
            Console.WindowHeight = Console.LargestWindowHeight - 10;
            Console.WindowWidth = Console.LargestWindowWidth - 10;
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;
        }

        public static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("\t\t Menu");
            Console.WriteLine();
            Console.WriteLine("\t  1.Start a flight");
            Console.WriteLine("\t  2.Add a dispatcher");
        }
        public static void DispatcherMenu()
        {
            Console.WriteLine("\t\t   Menu");
            Console.WriteLine("\t  1.Add a dispatcher");
            Console.WriteLine("\t  2.Delete a dispatcher");
            Console.WriteLine("\t  3.Replace a dispatcher");
        }
        public static int GetCorrectMenuOption(Action menuText, int min, int max)
        {
            var menuId = 0;
            var isCorrect = false;
            while (!isCorrect)
            {
                menuText();
                Console.WriteLine();
                Console.Write("\t  Number: ");
                if (int.TryParse(Console.ReadLine(), out menuId))
                {
                    if (menuId >= min && menuId <= max)
                    {
                        isCorrect = true;
                    }
                }
                else
                {
                    Console.Clear();
                }
            }
            return menuId;
        }

        public static string GetCorrectStringInput(string request)
        {
            var field = string.Empty;
            while (string.IsNullOrEmpty(field) || string.IsNullOrWhiteSpace(field))
            {
                Console.Write(request);
                field = Console.ReadLine();
            }
            return field;
        }
        public static int GetCorrectInt(string request, int min, int max)
        {
            var choice = 0;
            var isCorrect = false;
            while (!isCorrect)
            {
                var input = GetCorrectStringInput(request);
                if (int.TryParse(input, out choice))
                {
                    if (choice >= min && choice <= max)
                    {
                        isCorrect = true;
                    }
                }
            }
            return choice;
        }
    }
}