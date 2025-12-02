
Calculator calc = new Calculator();
SimplePanel panel = new SimplePanel(calc);
panel.SetupCustomButtons();
panel.ShowHelp();
while (true)
{
  Console.Write("\n> ");
  string input = Console.ReadLine();

     if (input.ToLower() == "выход" || input.ToLower() == "exit")
     {
       Console.WriteLine("Выход из калькулятора");
       break;
     }

    panel.ProcessInput(input);
}
public class Calculator
{
    public double Value { get; set; }
    public double Memory { get; set; }
    public string LastOperation { get; set; } = "";
    public double LastOperand { get; set; }
    private bool _awaitingNewNumber = true;
    private bool _isEnteringNumber = false;
    private string _currentNumber = "";

    public void Clear()
    {
        Value = 0;
        LastOperation = "";
        _awaitingNewNumber = true;
        _isEnteringNumber = false;
        _currentNumber = "";
        Console.WriteLine("Очищено");
    }

    public void EnterDigit(string digit)
    {
        if (!_isEnteringNumber)
        {
            _currentNumber = "";
            _isEnteringNumber = true;
        }

        _currentNumber += digit;

        if (double.TryParse(_currentNumber, out double number))
        {
            Value = number;
            Console.WriteLine($"Ввод: {_currentNumber}");
        }
    }

    public void FinishNumber()
    {
        if (_isEnteringNumber && !string.IsNullOrEmpty(_currentNumber))
        {
            if (double.TryParse(_currentNumber, out double number))
            {
                Value = number;
                Console.WriteLine($"Введено: {Value}");
            }
            _isEnteringNumber = false;
            _currentNumber = "";
            _awaitingNewNumber = false;
        }
    }

    public void EnterNumber(double number)
    {
        Value = number;
        _isEnteringNumber = false;
        _currentNumber = "";
        _awaitingNewNumber = false;
        Console.WriteLine($"Введено: {Value}");
    }

    public void SetOperation(string operation)
    {
        FinishNumber();

        LastOperand = Value;
        LastOperation = operation;
        _awaitingNewNumber = true;
        Console.WriteLine($"Операция: {operation}");
    }

    public void Calculate()
    {
        FinishNumber();

        if (!string.IsNullOrEmpty(LastOperation))
        {
            double result = LastOperation switch
            {
                "+" => LastOperand + Value,
                "-" => LastOperand - Value,
                "*" => LastOperand * Value,
                "/" => LastOperand / Value,
                _ => Value
            };

            Value = result;
            Console.WriteLine($"= {result}");
            LastOperation = "";
            _awaitingNewNumber = true;
        }
    }
    public bool IsEnteringNumber => _isEnteringNumber;
}
public class SimplePanel
{
    private readonly Calculator _calc;
    private readonly Dictionary<string, Action> _customButtons = new();
    private readonly Dictionary<string, string> _buttonLabels = new();
    private readonly List<string> _buttonKeys = new();

    public SimplePanel(Calculator calc)
    {
        _calc = calc;
    }

    public void SetupCustomButtons()
    {
        Console.WriteLine("Настройте 4 кнопки:");
        Console.WriteLine("1 - Сохранить в память");
        Console.WriteLine("2 - Восстановить из памяти");
        Console.WriteLine("3 - Квадратный корень");
        Console.WriteLine("4 - Квадрат");
        Console.WriteLine("5 - Процент");
        Console.WriteLine("6 - Очистить");
        for (int i = 1; i <= 4; i++)
        {
            Console.Write($"\n{i}: выберите функцию (1-6): ");
            string choice = Console.ReadLine();
            string key = $"f{i}";
            switch (choice)
            {
                case "1":
                    _customButtons[key] = () =>
                    {
                        _calc.FinishNumber();
                        _calc.Memory = _calc.Value;
                        Console.WriteLine($"Сохранено в память: {_calc.Value}");
                    };
                    _buttonLabels[key] = "MS";
                    break;

                case "2":
                    _customButtons[key] = () =>
                    {
                        _calc.FinishNumber();
                        _calc.Value = _calc.Memory;
                        Console.WriteLine($"Загружено из памяти: {_calc.Memory}");
                    };
                    _buttonLabels[key] = "MR";
                    break;

                case "3":
                    _customButtons[key] = () =>
                    {
                        _calc.FinishNumber();
                        double original = _calc.Value;
                        _calc.Value = Math.Sqrt(_calc.Value);
                        Console.WriteLine($"√{original} = {_calc.Value}");
                    };
                    _buttonLabels[key] = "Квадратный корень";
                    break;

                case "4":
                    _customButtons[key] = () =>
                    {
                        _calc.FinishNumber();
                        double original = _calc.Value;
                        _calc.Value *= _calc.Value;
                        Console.WriteLine($"{original}² = {_calc.Value}");
                    };
                    _buttonLabels[key] = "Квадрат";
                    break;

                case "5":
                    _customButtons[key] = () =>
                    {
                        _calc.FinishNumber();
                        double original = _calc.Value;
                        _calc.Value /= 100;
                        Console.WriteLine($"{original}% = {_calc.Value}");
                    };
                    _buttonLabels[key] = "Процент";
                    break;

                case "6":
                    _customButtons[key] = () =>
                    {
                        _calc.FinishNumber();
                        _calc.Clear();
                    };
                    _buttonLabels[key] = "C";
                    break;
                default:
                    _customButtons[key] = () =>
                    {
                        _calc.FinishNumber();
                        Console.WriteLine("Не настроено");
                    };
                    _buttonLabels[key] = "?";
                    break;
            }
            _buttonKeys.Add(key);
            Console.WriteLine($"Кнопка {i} настроена: {_buttonLabels[key]}");
        }
    }
    public void ProcessInput(string input)
    {
        input = input.Trim().ToLower();
        if (input.Length == 1 && char.IsDigit(input[0]) && _calc.IsEnteringNumber)
        {
            _calc.EnterDigit(input);
            return;
        }
        if (input == "f1" || input == "f2" || input == "f3" || input == "f4")
        {
            if (_customButtons.ContainsKey(input))
            {
                _customButtons[input]();
            }
            return;
        }
        if (double.TryParse(input, out double number))
        {
            _calc.FinishNumber();
            _calc.EnterNumber(number);
            return;
        }
        if (input == "+" || input == "-" || input == "*" || input == "/")
        {
            _calc.SetOperation(input);
            return;
        }
        if (input == "=")
        {
            _calc.Calculate();
            return;
        }
        if (input == " ")
        {
            _calc.FinishNumber();
            return;
        }
        switch (input)
        {
            case "значение":
            case "v":
                _calc.FinishNumber();
                Console.WriteLine($"Текущее значение: {_calc.Value}");
                Console.WriteLine($"Память: {_calc.Memory}");
                if (!string.IsNullOrEmpty(_calc.LastOperation))
                    Console.WriteLine($"Ожидается: {_calc.LastOperand} {_calc.LastOperation} ?");
                break;

            case "с":
            case "clear":
                _calc.Clear();
                break;

            case "мс":
                _calc.FinishNumber();
                _calc.Memory = _calc.Value;
                Console.WriteLine($"Сохранено: {_calc.Value}");
                break;

            case "мв":
                _calc.FinishNumber();
                _calc.Value = _calc.Memory;
                Console.WriteLine($"Загружено: {_calc.Memory}");
                break;

            case "корень":
                _calc.FinishNumber();
                double original = _calc.Value;
                _calc.Value = Math.Sqrt(_calc.Value);
                Console.WriteLine($"√{original} = {_calc.Value}");
                break;

            default:
                Console.WriteLine("еизвестная команда'");
                break;
        }
    }

    public void ShowHelp()
    {
        Console.WriteLine("\nвыбранные кнопки:");
        if (_buttonKeys.Count > 0)
        {
            for (int i = 0; i < _buttonKeys.Count; i++)
            {
                string key = _buttonKeys[i];
                int num = i + 1;
                Console.WriteLine($"  f{num} - {_buttonLabels[key]}");
            }
        }
    }

    public void ShowButtons()
    {
        if (_buttonKeys.Count == 0)
        {
            Console.WriteLine("кнопки не настроены");
            return;
        }

        Console.WriteLine("\n настроенные кнопки:");
        for (int i = 0; i < _buttonKeys.Count; i++)
        {
            string key = _buttonKeys[i];
            int num = i + 1;
            Console.WriteLine($"  f{num} ({_buttonLabels[key]}) - нажмите 'f{num}'");
        }
    }
}