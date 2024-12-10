using System;
using System.Collections.Generic;
using System.Linq;

namespace CrimeBook
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> lastNames = new List<string>
            {
                "Кукуев", "Печкин", "Вертляев", "Сидоренко", "Потапенко", "Заслонов", "Креольский", "Чубров", "Семочкин"
            };
            List<string> firstNames = new List<string>
            {
                "Александр", "Виктор", "Алексей", "Сергей", "Дмитрий", "Олег", "Андрей", "Ибрагим", "Варфаломей"
            };
            List<string> patronymics = new List<string>
            {
                "Батькович", "Сергеевич", "Александрович", "Исаакович", "Вессарионович", "Олегович", "Алексеевич"
            };
            List<string> nationalities = new List<string>
            {
                "Русский", "Удмурт", "Турок", "Казах", "Гражданин Мира"
            };
            int[] heightLimits = new int[]
            {
                150, 199
            };
            int[] weightLimits = new int[]
            {
                60, 120
            };
            int criminalsCount = 10;
            CriminalFactory criminalFactory = new CriminalFactory(lastNames, firstNames, patronymics, nationalities, heightLimits, weightLimits);
            CrimeBook crimeBook = new CrimeBook(criminalFactory, criminalsCount);

            crimeBook.Execute();
        }
    }

    class CrimeBook
    {
        private CriminalFactory _criminalFactory;
        private List<Criminal> _criminals = new List<Criminal>();
        private List<Criminal> _requiredCriminals;
        private int _requiredHeight;
        private int _requiredWeight;
        private string _requiredNationality;
        private bool _isArestedRequired;

        public CrimeBook(CriminalFactory criminalFactory, int criminalsCount)
        {
            _criminalFactory = criminalFactory;
            _criminals = _criminalFactory.Create(criminalsCount);
            _isArestedRequired = false;
        }

        public void Execute()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Эво вообще все кто есть.\n");
                ShowCriminalsInfo(_criminals);
                FillRequiredParameters();
                _requiredCriminals = _criminals.Where(criminal => criminal.Height == _requiredHeight && criminal.Weight == _requiredWeight && criminal.Nationality.ToLower() == _requiredNationality.ToLower() && criminal.IsArested == _isArestedRequired).ToList();

                if (_requiredCriminals.Count > 0)
                {
                    Console.WriteLine("\nЭто отсортированные по запросу, которых ещё не отловили.\n");
                    ShowCriminalsInfo(_requiredCriminals);
                }
                else
                {
                    Console.WriteLine("Никого подходящего запросу нету.");
                }
            }
            while (IsRestart());

            Console.WriteLine("Конец.");
        }

        private void FillRequiredParameters()
        {
            Console.WriteLine("Вам известен РОСТ подозреваемого?");
            _requiredHeight = UserUtills.GetIntegerPositiveUserInput();
            Console.WriteLine("Вам известен ВЕС подозреваемого?");
            _requiredWeight = UserUtills.GetIntegerPositiveUserInput();
            Console.WriteLine("Вам известна НАЦИОНАЛЬНОСТЬ подозреваемого?(Капс не важен)");
            _requiredNationality = Console.ReadLine();
        }

        private void ShowCriminalsInfo(List<Criminal> criminals)
        {
            foreach (Criminal criminal in criminals)
            {
                criminal.ShowInfo();
            }

            Console.WriteLine();
        }

        private bool IsRestart()
        {
            ConsoleKey exitKey = ConsoleKey.Escape;
            Console.WriteLine(exitKey + " - нажмите для выхода. Остальные клавиши для повторного поиска.");

            return Console.ReadKey(true).Key != exitKey;
        }
    }

    class CriminalFactory
    {
        private List<string> _lastNames;
        private List<string> _firstNames;
        private List<string> _patronymics;
        private List<string> _nationalities;
        private int[] _heightLimits;
        private int[] _weightLimits;

        public CriminalFactory(List<string> lastNames,
            List<string> firstNames,
            List<string> patronymics,
            List<string> nationalities,
            int[] heightLimits,
            int[] weightLimits)
        {
            _lastNames = lastNames;
            _firstNames = firstNames;
            _patronymics = patronymics;
            _nationalities = nationalities;
            _heightLimits = heightLimits;
            _weightLimits = weightLimits;
        }

        public List<Criminal> Create(int count)
        {
            List<Criminal> criminals = new List<Criminal>();

            for (int i = 0; i < count; i++)
            {
                criminals.Add(new Criminal(GenerateFullName(),
                    UserUtills.GetRandomBool(),
                    UserUtills.GenerateNumberFromArrayLimits(_heightLimits),
                    UserUtills.GenerateNumberFromArrayLimits(_weightLimits),
                    _nationalities[UserUtills.GenerateLimitedPositiveNumber(_nationalities.Count)]));
            }

            return criminals;
        }

        private string GenerateFullName()
        {
            string separator = " ";

            string fullName = _lastNames[UserUtills.GenerateLimitedPositiveNumber(_lastNames.Count)]
                + separator + _firstNames[UserUtills.GenerateLimitedPositiveNumber(_firstNames.Count)]
                + separator + _patronymics[UserUtills.GenerateLimitedPositiveNumber(_patronymics.Count)];

            return fullName;
        }
    }

    class Criminal
    {
        private const string Yes = "да";
        private const string No = "нет";

        public Criminal(string fullName, bool isArested, int height, int weight, string nationality)
        {
            FullName = fullName;
            IsArested = isArested;
            Height = height;
            Weight = weight;
            Nationality = nationality;
        }

        public string FullName { get; }
        public bool IsArested { get; }
        public int Height { get; }
        public int Weight { get; }
        public string Nationality { get; }

        public void ShowInfo()
        {
            Console.WriteLine($"{FullName}. Рост - {Height}. Вес - {Weight}. Национальность - {Nationality}. Арестован - {(IsArested ? Yes : No)}");
        }
    }

    static class UserUtills
    {
        private static Random s_random = new Random();

        public static int GetIntegerPositiveUserInput()
        {
            int userInput;

            do
            {
                Console.Write("Введите целое положительное число:");
            }
            while (int.TryParse(Console.ReadLine(), out userInput) == false || userInput < 0);

            return userInput;
        }

        public static bool GetRandomBool()
        {
            bool[] boolValues = { true, false };

            return boolValues[s_random.Next(boolValues.Length)];
        }

        public static int GenerateLimitedPositiveNumber(int maxValueExclusive)
        {
            return s_random.Next(maxValueExclusive);
        }

        public static int GenerateLimitedNumber(int minValueInclusive, int maxValueExclusive)
        {
            return s_random.Next(minValueInclusive, maxValueExclusive);
        }

        public static int GenerateNumberFromArrayLimits(int[] limits)
        {
            Array.Sort(limits);

            return s_random.Next(limits[0], limits[1]);
        }
    }
}
