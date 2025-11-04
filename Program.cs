using System;
using System.Globalization;
using System.Text;

namespace LinearInequalitiesApp
{
    // ────────────────────────────── ІНТЕРФЕЙСИ ──────────────────────────────

    public interface ILogger
    {
        void Log(string message);
    }

    public interface ICheckable
    {
        bool CheckVector(params double[] variables);
    }

    // ────────────────────────────── КЛАС ЛОГУВАННЯ ──────────────────────────────

    public class ConsoleLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine(message);
    }

    // ────────────────────────────── АБСТРАКТНИЙ КЛАС ──────────────────────────────

    public abstract class BaseSystem : ICheckable, IDisposable
    {
        protected readonly ILogger Logger;
        protected bool disposed = false;

        protected BaseSystem(ILogger logger)
        {
            Logger = logger;
            Logger.Log($"[Створено об’єкт {GetType().Name}]");
        }

        public abstract void InputCoefficients();
        public abstract void PrintSystem();
        public abstract bool CheckVector(params double[] variables);

        // Патерн Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Logger.Log($"[Ресурси класу {GetType().Name} звільнено через Dispose]");
                }
                disposed = true;
            }
        }

        ~BaseSystem()
        {
            Dispose(false);
        }
    }

    // ────────────────────────────── ДОПОМОЖНИЙ КЛАС ──────────────────────────────

    public static class Helper
    {
        public static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) &&
                    double.IsFinite(value))
                    return value;

                Console.WriteLine("❌ Некоректне число. Використовуйте крапку для дробових значень (напр. 1.5).");
            }
        }

        public static int ReadIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int value) && value >= min && value <= max)
                    return value;

                Console.WriteLine($"❌ Введіть число від {min} до {max}.");
            }
        }
    }

    // ────────────────────────────── ОСНОВНИЙ КЛАС СИСТЕМИ ──────────────────────────────

    public class InequalitiesSystem : BaseSystem
    {
        protected readonly double[,] Coefficients;
        protected readonly double[] Constants;

        public int InequalitiesCount { get; }
        public int VariablesCount { get; }

        public InequalitiesSystem(int inequalitiesCount, int variablesCount, ILogger logger)
            : base(logger)
        {
            if (inequalitiesCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(inequalitiesCount));
            if (variablesCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(variablesCount));

            InequalitiesCount = inequalitiesCount;
            VariablesCount = variablesCount;
            Coefficients = new double[inequalitiesCount, variablesCount];
            Constants = new double[inequalitiesCount];
        }

        public override void InputCoefficients()
        {
            Logger.Log($"\nВведіть коефіцієнти для системи з {InequalitiesCount} нерівностей " +
                       $"та {VariablesCount} змінних:");
            for (int i = 0; i < InequalitiesCount; i++)
            {
                Logger.Log($"\nНерівність {i + 1}:");
                for (int j = 0; j < VariablesCount; j++)
                    Coefficients[i, j] = Helper.ReadDouble($"  Введіть a{i + 1}{j + 1}: ");
                Constants[i] = Helper.ReadDouble($"  Введіть b{i + 1}: ");
            }
        }

        public override void PrintSystem() => Logger.Log(ToString());

        public override bool CheckVector(params double[] variables)
        {
            if (variables.Length != VariablesCount)
                throw new ArgumentException("Кількість змінних не збігається.");

            for (int i = 0; i < InequalitiesCount; i++)
            {
                double sum = 0;
                for (int j = 0; j < VariablesCount; j++)
                    sum += Coefficients[i, j] * variables[j];
                if (sum > Constants[i]) return false;
            }
            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder("\nСистема лінійних нерівностей:\n");
            for (int i = 0; i < InequalitiesCount; i++)
            {
                for (int j = 0; j < VariablesCount; j++)
                {
                    double coeff = Coefficients[i, j];
                    string sign = coeff >= 0 && j > 0 ? " + " : (j > 0 ? " - " : coeff < 0 ? "-" : "");
                    sb.Append($"{sign}{Math.Abs(coeff).ToString(CultureInfo.InvariantCulture)}*x{j + 1}");
                }
                sb.Append($" ≤ {Constants[i].ToString(CultureInfo.InvariantCulture)}\n");
            }
            return sb.ToString();
        }
    }

    // ────────────────────────────── ПОХІДНИЙ КЛАС ──────────────────────────────

    public class SpecialInequalitiesSystem : InequalitiesSystem
    {
        public SpecialInequalitiesSystem(int inequalitiesCount, int variablesCount, ILogger logger)
            : base(inequalitiesCount, variablesCount, logger)
        {
            Logger.Log("🔹 Ініціалізовано спеціальну систему нерівностей");
        }

        public override void PrintSystem()
        {
            Logger.Log("\n--- Спеціальний формат системи ---");
            base.PrintSystem();
        }

        public override bool CheckVector(params double[] variables)
        {
            Logger.Log("Виконується перевірка вектора у спеціальній системі...");
            return base.CheckVector(variables);
        }
    }

    // ────────────────────────────── MAIN ──────────────────────────────

    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            ILogger logger = new ConsoleLogger();

            logger.Log("=== Демонстрація абстрактного класу, інтерфейсів та Dispose ===\n");
            int choice = Helper.ReadIntInRange("Оберіть режим (1 — звичайна система, 2 — спеціальна): ", 1, 2);

            using BaseSystem system = choice == 1
                ? new InequalitiesSystem(2, 2, logger)
                : new SpecialInequalitiesSystem(2, 2, logger);

            system.InputCoefficients();
            system.PrintSystem();

            logger.Log("\nВведіть 2 змінні для перевірки:");
            double x1 = Helper.ReadDouble("x1 = ");
            double x2 = Helper.ReadDouble("x2 = ");

            bool result = system.CheckVector(x1, x2);
            logger.Log(result ? "✅ Вектор задовольняє систему" : "❌ Вектор не задовольняє систему");

            logger.Log("\n--- Кінець роботи ---");
        }
    }
}
