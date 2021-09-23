using System;
using System.Collections.Generic;
using System.Linq;
using FlyTraining.Enums;
using FlyTraining.EventArgs;

namespace FlyTraining
{
    public sealed class PilotSimulator
    {
        #region Fields
        private readonly Plane _plane;
        private int PenaltyPoints { get; set; }
        private const int TableHeight = 9;
        private int _tableWidth = 40;
        private readonly List<string> _recomendations;
        private readonly string[] _recomendationsCopy = new string[30]; //Для красоты:чтобы рекомендации не пропадали после неправильного ввода.
        private const int RecomendationPlace = 85;
        private int _lowestPlace = 85;
        #endregion
        public PilotSimulator()
        {
            Console.Title = "Pilot Simulator";
            Console.BufferHeight = Console.WindowHeight = 18;
            Console.BufferWidth = Console.WindowWidth = 40;
            _plane = Plane.GetInstance();
            _recomendations = new List<string>(5);
            _plane.OutPutEventHandler += OnOutput;
        }

        public void Start()
        {
            try
            {
                MainMenu();
                Utils.Utils.SetUpConsole();

                while (true)//Game Cycle
                {
                    Display();

                    var keyInfo = Console.ReadKey(true);
                    if (UserInput(_plane, keyInfo))
                    {
                        if (_plane.State != PlaneState.Landed) continue;
                        Display();
                        break;
                    }
                    _recomendations.AddRange(_recomendationsCopy.Where(r => r != null));
                    DispatcherMenu(keyInfo);
                }
            }
            catch (Exception e)
            {
                DrawConsole();
                ShowRecomendedHeight();
                ShowPlane();
                Console.SetCursorPosition(0, 82);
                Console.WriteLine(e.Message);
                while (Console.ReadKey(true) != null) { };
            }
            while (Console.ReadKey(true) != null) { };
        }

        private void MainMenu()
        {
            do
            {
                Console.TreatControlCAsInput = false;
                Console.Clear();
                int choice = Utils.Utils.GetCorrectMenuOption(Utils.Utils.MainMenu, 1, 2);
                switch (choice)
                {
                    case 1:
                        Console.Clear();
                        _plane.Start();
                        if (_plane.State == PlaneState.NotReady)
                            Console.ReadKey(true);
                        break;
                    case 2:
                        Console.Clear();
                        CreateDispatcher();
                        break;
                }
            } while (_plane.State == PlaneState.NotReady);
        }
        private void DispatcherMenu(ConsoleKeyInfo keyInfo)
        {
            Console.SetCursorPosition(0, _lowestPlace);
            if (_plane.State != PlaneState.Landed && _plane.State != PlaneState.NotStarted)
            {
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Console.Clear();
                        CreateDispatcher();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        RemoveDispatcher();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        Console.Clear();
                        ChangeDispatcher();
                        break;
                }
            }
        }

        private bool UserInput(Plane plane, ConsoleKeyInfo keyInfo)
        {
            bool shiftHeld =
                (keyInfo.Modifiers & ConsoleModifiers.Shift) != 0;

            switch (keyInfo.Key)
            {
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    plane.ChangeDirection(Direction.Left, shiftHeld);
                    return true;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    plane.ChangeDirection(Direction.Down, shiftHeld);
                    return true;
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    plane.ChangeDirection(Direction.Up, shiftHeld);
                    return true;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    plane.ChangeDirection(Direction.Right, shiftHeld);
                    return true;
                case ConsoleKey.K:
                    plane.ReachedMaxSpeed = true;
                    return true;
                default:
                    return false;
            }
        }
        private void Display()
        {
            DrawConsole();
            ShowPlane();
            ShowRecomendedHeight();
            ShowPlaneStats();
            ShowRecomendations();
            ShowDispatcherMenu();
        }

        #region Display Method Parts
        private void DrawConsole()
        {
            if (_plane.FlightSpeeds.Count > _tableWidth)
                _tableWidth *= 2;
            Console.Clear();
            const int heightScale = 10;
            const int widthScale = 3;

            int padleft = 4;

            for (int i = 0; i < TableHeight; i++)
            {
                Console.SetCursorPosition(0, (TableHeight * heightScale) - (i * heightScale) - 10);
                string number = (i * 1000).ToString().PadLeft(padleft);
                Console.Write(number);
            }

            int j = 10;
            for (int i = 70; i < 80; i++)
            {
                Console.SetCursorPosition(0, i);
                string number = (j-- * 100).ToString().PadLeft(padleft);
                Console.Write(number);
            }


            int offset = 0;
            for (int i = 0; i < _tableWidth; i++)
            {
                if (i % 2 != 0)
                {
                    if (i > 9)
                    {
                        Console.SetCursorPosition(i * widthScale + padleft + (offset++), TableHeight * 10 - 9);
                        Console.Write(i);
                    }
                    else
                    {
                        Console.SetCursorPosition(i * widthScale + padleft, TableHeight * 10 - 9);
                        Console.Write(i);
                    }

                }
            }

        }
        private void ShowPlane()
        {
            Console.ForegroundColor = _plane.PlaneColor;
            for (int i = 0; i < _plane.FlightSpeeds.Count; i++)
            {
                Console.SetCursorPosition(i * 3 + 7, 80 - (_plane.Speed < 0 ? 0 : _plane.FlightSpeeds[i] / 100));
                Console.Write("0");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        private void ShowRecomendedHeight()
        {
            foreach (var dispatcher in _plane.Dispatchers)
            {
                Console.ForegroundColor = dispatcher.DispatcherColor;
                foreach (var offset in dispatcher.RecomendedHeight.Keys)
                {
                    int posHeight = (TableHeight * 1000 - dispatcher.RecomendedHeight[offset]) / 100;
                    if (posHeight >= 9)
                    {
                        posHeight -= 9;
                        Console.SetCursorPosition((offset) * 3 + 7,
                            posHeight);
                        Console.Write("h");
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        private void ShowPlaneStats()
        {
            if (_plane.State == PlaneState.Landed)
            {
                Console.SetCursorPosition(0, 83);
                Console.WriteLine("Successful landing! Congratulations!!!");
                foreach (var dispatcher in _plane.Dispatchers)
                {
                    PenaltyPoints += dispatcher.PenaltyPoints;
                }
                Console.WriteLine($"Penalty points: {PenaltyPoints}");
            }
            else if (_plane.State != PlaneState.NotStarted)
            {
                Console.SetCursorPosition(0, 83);
                Console.WriteLine($"Current altitude: {_plane.Height}");
                Console.WriteLine($"Current speed: {_plane.Speed}");
            }
        }
        //Отображает рекомендации в recomendationPlace, делает копию рекомендаций а затем очищает лист рекомендаций.
        private void ShowRecomendations()
        {
            if (_plane.State == PlaneState.Landed)
                return;
            int place = RecomendationPlace;
            if (_recomendations.Count > 0)
            {
                Console.SetCursorPosition(0, place++);
                Console.WriteLine();
                Console.WriteLine("\tRecommendations");
                place++;
            }
            foreach (var recomendation in _recomendations)
            {
                Console.SetCursorPosition(0, place++);
                Console.WriteLine(recomendation);
            }
            _lowestPlace = place;
            _recomendations.CopyTo(_recomendationsCopy);
            _recomendations.Clear();
        }
        private void ShowDispatcherMenu()
        {
            if (_plane.State != PlaneState.NotStarted && _plane.State != PlaneState.Landed)
            {
                Console.SetCursorPosition(0, _lowestPlace);
                Utils.Utils.DispatcherMenu();
            }
        }
        #endregion

        #region OnEvent Methods
        private void OnRecomendedHeight(object sender, RecomendedHeightEventArgs eventArgs)
        {
            if (!(sender is Dispatcher dispatcher))
                return;
            var recomendation = $"{dispatcher.Name}: {eventArgs.RecomendedHeight}";
            _recomendations.Add(recomendation);
            if (eventArgs.SpecialMessage != null)
            {
                recomendation = $"{dispatcher.Name}: {eventArgs.SpecialMessage}";
                _recomendations.Add(recomendation);
            }
        }
        private void OnOutput(object sender, OutputEventArgs flightEventArgs)
        {
            Console.WriteLine(flightEventArgs.Message);
        }
        #endregion


        #region Choose,Add,Remove Dispatcher
        private Dispatcher ChooseDispatcher()
        {
            int i = 1;
            foreach (var dispatcher in _plane.Dispatchers)
            {
                Console.Write($"{i++}.");
                Console.WriteLine(dispatcher.Name);
            }
            Console.TreatControlCAsInput = false;
            int index = Utils.Utils.GetCorrectInt("\tDispatcher's number: ", 1, i - 1);
            Console.TreatControlCAsInput = true;
            return _plane.Dispatchers[index - 1];
        }

        private void CreateDispatcher(int penaltyPoints = 0)
        {
            Console.TreatControlCAsInput = false;
            string name = Utils.Utils.GetCorrectStringInput("Enter dispatcher's name: ");
            CreateDispatcher(name, penaltyPoints);
            Console.TreatControlCAsInput = true;
        }
        private void CreateDispatcher(string dispatcherName, int penaltyPoints)
        {
            var dispatcher = new Dispatcher(dispatcherName, Utils.Utils.GetRandomDistinctColor(), penaltyPoints);
            dispatcher.RecomendedHeightEventHandler += OnRecomendedHeight;
            _plane.AddDispatcher(dispatcher);
            Console.TreatControlCAsInput = true;
        }

        private void RemoveDispatcher()
        {
            Console.Clear();
            if (_plane.Dispatchers.Count <= 2)
            {
                Console.WriteLine("A minimum of 2 dispatchers must control the aircraft.");
                Console.ReadKey(true);
                return;
            }
            var disp = ChooseDispatcher();
            _plane.RemoveDispatcher(disp);
        }
        private void RemoveDispatcher(Dispatcher dispatcher)
        {
            Console.Clear();
            if (_plane.Dispatchers.Count <= 2)
            {
                Console.WriteLine("A minimum of 2 dispatchers must control the aircraft.");
                Console.ReadKey(true);
                return;
            }
            _plane.RemoveDispatcher(dispatcher);
        }

        private void ChangeDispatcher()
        {
            Console.WriteLine("\t\tDispatcher to delete");
            var removeDisp = ChooseDispatcher();
            var penalties = removeDisp.PenaltyPoints;

            Console.Clear();
            Console.WriteLine("\t\tNew dispatcher");
            CreateDispatcher(penalties);

            RemoveDispatcher(removeDisp);
        }
        #endregion
    }
}