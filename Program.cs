using System;
using System.Globalization;
using System.Text;

namespace LinearInequalitiesApp
{
    /// <summary>
    /// Допоміжний клас для спільних методів введення.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Зчитує дійсне число з консолі з перевіркою.
        /// </summary>
        public static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                    && double.IsFinite(value))
                    return value;
                Console.WriteLine("❌ Некоректне число. Використовуйте крапку для дробових значень (наприклад: 1.5).");
            }
        }
    }

    /// <summary>
    /// Абстрактний клас — базовий шаблон для різних систем.
    /// </summary>
    public abstract class BaseSystem
    {
        public int InequalitiesCount { get; protected set; }
        public int VariablesCount { get; protected set; }

        /// <summary>
        /// Конструктор абстрактного класу.
        /// </summary>
        protected BaseSystem(int inequalitiesCount, int variablesCount)
        {
            InequalitiesCount = inequalitiesCount;
            VariablesCount = variablesCount;
            Console.WriteLine("▶ Викликано конструктор BaseSystem");
        }

        /// <summary>
        /// Абстрактний метод введення коефіцієнтів.
        /// </summary>
        public abstract void InputCoefficients();

        /// <summary>
        /// Абстрактний метод перевірки вектора.
        /// </summary>
        public abstract bool CheckVector(params double[] variables);

        /// <summary>
        /// Віртуальний метод для виведення системи.
        /// </summary>
        public virtual void PrintSystem()
        {
            Console.WriteLine("Вивід системи (базовий клас).");
        }

        /// <summary>
        /// Деструктор (для демонстрації).
        /// </summary>
        ~BaseSystem()
        {
            Console.WriteLine("❎ Звільнення ресурсів BaseSystem");
        }
    }

    /// <summary>
    /// Клас для звичайної системи лінійних нерівностей.
    /// </summary>
    public class InequalitiesSystem : BaseSystem
    {
        private readonly double[,] _coefficients;
        private readonly double[] _constants;

        public InequalitiesSystem(int inequalitiesCount, int variablesCount)
            : base(inequalitiesCount, variablesCount)
        {
            if (inequalitiesCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(inequalitiesCount));
            if (variablesCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(variablesCount));

            _coefficients = new double[inequalitiesCount, variablesCount];
            _constants = new double[inequalitiesCount];

            Console.WriteLine("▶ Викликано конструктор InequalitiesSystem");
        }

        public override void InputCoefficients()
        {
            Console.WriteLine($"\nВведіть коефіцієнти для системи з {InequalitiesCount} нерівностей " +
                              $"та {VariablesCount} змінних:");
            for (int i = 0; i < InequalitiesCount; i++)
            {
                Console.WriteLine($"\nНерівність {i + 1}:");
                for (int j = 0; j < VariablesCount; j++)
                    _coefficients[i, j] = Helper.ReadDouble($"  Введіть a{i + 1}{j + 1}: ");
                _constants[i] = Helper.ReadDouble($"  Введіть b{i + 1}: ");
            }
        }

        public override void PrintSystem()
        {
            Console.WriteLine("\nСистема лінійних нерівностей має вигляд:");
            for (int i = 0; i < InequalitiesCount; i++)
            {
                var sb = new StringBuilder();
                for (int j = 0; j < VariablesCount; j++)
                {
                    double coeff = _coefficients[i, j];
                    string sign = coeff >= 0 && j > 0 ? " + " : (j > 0 ? " - " : coeff < 0 ? "-" : "");
                    sb.Append($"{sign}{Math.Abs(coeff).ToString(CultureInfo.InvariantCulture)}*x{j + 1}");
                }
                sb.Append($" ≤ {_constants[i].ToString(CultureInfo.InvariantCulture)}");
                Console.WriteLine(sb);
            }
        }

        public override bool CheckVector(params double[] variables)
        {
            if (variables.Length != VariablesCount)
                throw new ArgumentException("Невірна кількість змінних.");

            for (int i = 0; i < InequalitiesCount; i++)
            {
                double sum = 0;
                for (int j = 0; j < VariablesCount; j++)
                    sum += _coefficients[i, j] * variables[j];
                if (sum > _constants[i])
                    return false;
            }

            return true;
        }

        ~InequalitiesSystem()
        {
            Console.WriteLine("❎ Звільнення ресурсів InequalitiesSystem");
        }
    }

    /// <summary>
    /// Похідний клас для спеціальної системи (демонстрація поліморфізму).
    /// </summary>
    public class SpecialInequalitiesSystem : InequalitiesSystem
    {
        public SpecialInequalitiesSystem(int inequalitiesCount, int variablesCount)
            : base(inequalitiesCount, variablesCount)
        {
            Console.WriteLine("▶ Викликано конструктор SpecialInequalitiesSystem");
        }

        public override void PrintSystem()
        {
            Console.WriteLine("\n--- Спеціальна система нерівностей ---");
            base.PrintSystem();
        }

        public override bool CheckVector(params double[] variables)
        {
            Console.WriteLine("Перевірка вектора у спеціальній системі...");
            return base.CheckVector(variables);
        }

        public new void DemoNewMethod()
        {
            Console.WriteLine("Цей метод приховано (new), викликається лише через SpecialInequalitiesSystem");
        }

        ~SpecialInequalitiesSystem()
        {
            Console.WriteLine("❎ Звільнення ресурсів SpecialInequalitiesSystem");
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== Абстрактні класи, конструктори, деструктори, поліморфізм ===");
            Console.WriteLine("Оберіть тип системи:");
            Console.WriteLine("1 — звичайна система");
            Console.WriteLine("2 — спеціальна система");

            string? choice = Console.ReadLine();

            // Поліморфізм — базовий тип, об'єкт визначається під час виконання
            BaseSystem system = choice == "1"
                ? new InequalitiesSystem(2, 2)
                : new SpecialInequalitiesSystem(2, 2);

            system.InputCoefficients();
            system.PrintSystem();

            Console.WriteLine("\nВведіть 2 змінні для перевірки:");
            double x1 = Helper.ReadDouble("x1 = ");
            double x2 = Helper.ReadDouble("x2 = ");

            bool result = system.CheckVector(x1, x2);
            Console.WriteLine(result ? "✅ Вектор задовольняє систему" : "❌ Вектор не задовольняє систему");

            if (system is SpecialInequalitiesSystem special)
            {
                special.DemoNewMethod();
            }

            Console.WriteLine("\nПрограма завершила роботу. Натисніть Enter...");
            Console.ReadLine();
        }
    }
}
