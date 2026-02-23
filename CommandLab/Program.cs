using System;
using System.Collections.Generic;

Calculator calc = new Calculator();
SimplePanel panel = new SimplePanel(calc);

panel.SetupCustomButtons();
while (true)
{
    Console.Write("\n> ");
    string input = Console.ReadLine();

    if (input == null) continue;

    if (input.ToLower() == "выход")
    {
        Console.WriteLine("выхо");
        break;
    }

    panel.ProcessInput(input);
}
public class Calculator
{
    public double Value { get; private set; }
    public double Memory { get; set; }

    private double _leftOperand;
    private string _operation;

    public void SetNumber(double number)
    {
        Value = number;
        Console.WriteLine($"Число: {Value}");
    }

    public void SetOperation(string op)
    {
        _leftOperand = Value;
        _operation = op;
        Console.WriteLine($"Операция: {_operation}");
    }

    public void Calculate()
    {
        if (string.IsNullOrEmpty(_operation))
            return;

        switch (_operation)
        {
            case "+":
                Value = _leftOperand + Value;
                break;
            case "-":
                Value = _leftOperand - Value;
                break;
            case "*":
                Value = _leftOperand * Value;
                break;
            case "/":
                if (Value == 0)
                {
                    Console.WriteLine("Ошибка: деление на 0");
                    return;
                }
                Value = _leftOperand / Value;
                break;
        }

        Console.WriteLine($"= {Value}");
        _operation = null;
    }

    public void Clear()
    {
        Value = 0;
        _leftOperand = 0;
        _operation = null;
        Console.WriteLine("Очищено");
    }
}
public interface ICommand
{
    void Execute();
    string Name { get; }
}

public class SaveMemoryCommand : ICommand
{
    private readonly Calculator _calc;
    public string Name => "MS";

    public SaveMemoryCommand(Calculator calc)
    {
        _calc = calc;
    }

    public void Execute()
    {
        _calc.Memory = _calc.Value;
        Console.WriteLine("Сохранено в память");
    }
}

public class LoadMemoryCommand : ICommand
{
    private readonly Calculator _calc;
    public string Name => "MR";

    public LoadMemoryCommand(Calculator calc)
    {
        _calc = calc;
    }

    public void Execute()
    {
        _calc.SetNumber(_calc.Memory);
        Console.WriteLine("Загружено из памяти");
    }
}

public class SquareCommand : ICommand
{
    private readonly Calculator _calc;
    public string Name => "квадратный корень";

    public SquareCommand(Calculator calc)
    {
        _calc = calc;
    }

    public void Execute()
    {
        _calc.SetNumber(_calc.Value * _calc.Value);
    }
}

public class SqrtCommand : ICommand
{
    private readonly Calculator _calc;
    public string Name => "корень";

    public SqrtCommand(Calculator calc)
    {
        _calc = calc;
    }

    public void Execute()
    {
        _calc.SetNumber(Math.Sqrt(_calc.Value));
    }
}

public class PercentCommand : ICommand
{
    private readonly Calculator _calc;
    public string Name => "процент";

    public PercentCommand(Calculator calc)
    {
        _calc = calc;
    }

    public void Execute()
    {
        _calc.SetNumber(_calc.Value / 100);
    }
}

public class ClearCommand : ICommand
{
    private readonly Calculator _calc;
    public string Name => "очистить";

    public ClearCommand(Calculator calc)
    {
        _calc = calc;
    }

    public void Execute()
    {
        _calc.Clear();
    }
}
public class SimplePanel
{
    private readonly Calculator _calc;
    private readonly Dictionary<string, ICommand> _buttons = new();

    public SimplePanel(Calculator calc)
    {
        _calc = calc;
    }

    public void SetupCustomButtons()
    {
        Console.WriteLine("Настройте 4 кнопки:");
        Console.WriteLine("1 - Сохранить в память");
        Console.WriteLine("2 - Восстановить из памяти");
        Console.WriteLine("3 - Квадрат");
        Console.WriteLine("4 - Корень");
        Console.WriteLine("5 - Процент");
        Console.WriteLine("6 - Очистить");

        for (int i = 1; i <= 4; i++)
        {
            Console.Write($"f{i}: ");
            string choice = Console.ReadLine();

            ICommand command = choice switch
            {
                "1" => new SaveMemoryCommand(_calc),
                "2" => new LoadMemoryCommand(_calc),
                "3" => new SquareCommand(_calc),
                "4" => new SqrtCommand(_calc),
                "5" => new PercentCommand(_calc),
                "6" => new ClearCommand(_calc),
                _ => null
            };

            if (command != null)
            {
                _buttons[$"f{i}"] = command;
                Console.WriteLine($"Назначено: {command.Name}");
            }
            else
            {
                Console.WriteLine("Функция не выбрана");
            }
        }
    }

    public void ProcessInput(string input)
    {
        input = input.Trim().ToLower();

        if (_buttons.ContainsKey(input))
        {
            _buttons[input].Execute();
            return;
        }

        if (double.TryParse(input, out double number))
        {
            _calc.SetNumber(number);
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

        if (input == "clear")
        {
            _calc.Clear();
            return;
        }
        Console.WriteLine("Неизвестная команда");
    }
}