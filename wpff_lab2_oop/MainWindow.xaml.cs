using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpff_lab2_oop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Calculator calc;
        private Command comm;
        private string previous;
        private bool scienceMode = false;
        private char[] validOperators = new char[]
        {
            '+',
            '-',
            '×',
            '÷'
        };
        public Dictionary<string, double> Pi = new Dictionary<string, double>()
        {
            { "π", Math.PI }
        };
        private string input;
        private bool isPrev = false;
        public delegate void delErase();
        public bool isScienceMode = false;
        public MainWindow()
        {
            InitializeComponent();
            calc = new Calculator();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            input = (string)((Button)e.OriginalSource).Content;

            if (input == ".")
            {
                if (NormalizeDot())
                    window.Text += input;
            }

            else if (input == "0" || input == "00")
            {
                if (NormalizeZero())
                    window.Text += input;
            }
            else if (window.Text == "0")
                window.Text = input;
            else if (input == "π")
            {
                if(NormalizePI())
                    window.Text += input;
            }
            else
                window.Text += input;

        }
        private bool NormalizeZero()
        {
            if (window.Text.Length < 2)
                return false;
            return true;
        }
        private bool NormalizeDot()
        {
            for (int i = 0; i < window.Text.Length; i++)
            {
                if (window.Text[i] == '.')
                    return false;
            }
            return true;
        }
        private bool NormalizePI()
        {
            for (int i = 0; i < window.Text.Length; i++)
            {
                if (window.Text[i] == 'π')
                    return false;
            }
            return true;
        }

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            input = (string)((Button)e.OriginalSource).Content;
            if (input == "=")
            {
                previous = Regex.Replace(previous, "π", Math.PI.ToString());
                window.Text = Regex.Replace(window.Text, "π", Math.PI.ToString());
                double.TryParse(window.Text, out double num);
                calc.receiver_.Info = Convert.ToDouble(previous.Substring(0, previous.Length - 1));
                switch (previous[previous.Length - 1])
                {
                    case '+':
                        window.Text = Convert.ToString(calc.Add(num));
                        break;
                    case '-':
                        window.Text = Convert.ToString(calc.Sub(num));
                        break;
                    case '×':
                        window.Text = Convert.ToString(calc.Mul(num));
                        break;
                    case '÷':
                        window.Text = Convert.ToString(calc.Div(num));
                        break;
                    case '^':
                        window.Text = Convert.ToString(calc.Pow(num));
                        break;
                }
                calc.PreviousCommand = previous + num;
                window_2.Text = "";
                previous = "";
                isPrev = false;
            }
            else if (input == "e")
            {
                window.Text = Convert.ToString(calc.Exp(Convert.ToDouble(window.Text)));
            }
            else if (input == "√")
            {
                window.Text = Convert.ToString(calc.Sqrt(Convert.ToDouble(window.Text)));
            }
            else if (input == "ln")
            {
                window.Text = Convert.ToString(calc.Ln10(Convert.ToDouble(window.Text)));
            }
            else if (!isPrev)
            {
                previous = window.Text + input;
                window_2.Text = previous;
                window.Text = "0";
                isPrev = true;
            }

        }

        private void AdvanceOperations_Click(object sender, RoutedEventArgs e)
        {
            input = (string)((Button)e.OriginalSource).Content;
            switch (input)
            {
                case "C":
                    window.Text = calc.C();
                    window_2.Text = "";
                    isPrev = false;
                    break;
                case "CE":
                    int b = -1;
                    try
                    {
                        for (int i = 0; i < calc.PreviousCommand.Length; i++)
                        {
                            for (int j = 0; j < validOperators.Length; j++)
                            {
                                if (calc.PreviousCommand[i] == validOperators[j])
                                {
                                    b = i;
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Попередня команда відсутня!");
                    }
                    window_2.Text = calc.PreviousCommand.Substring(0, b + 1);
                    window.Text = calc.PreviousCommand.Substring(b + 1, calc.PreviousCommand.Length - b - 1);
                    previous = window_2.Text;
                    break;
                case "⌫":
                    window.Text = calc.Erase();
                    break;
            }
        }

        private void ScienceMode_Click(object sender, RoutedEventArgs e)
        {
            if (!scienceMode)
            {
                main.Width += 200;
                grid.Width += 200;
                scienceMode = true;
                pi.Visibility = Visibility.Visible;
                exp.Visibility = Visibility.Visible;
                sqrt.Visibility = Visibility.Visible;
                pow.Visibility = Visibility.Visible;
                ln.Visibility = Visibility.Visible;
            }
            else
            {
                main.Width -= 200;
                grid.Width -= 200;
                scienceMode = false;
                pi.Visibility = Visibility.Collapsed;
                exp.Visibility = Visibility.Collapsed;
                sqrt.Visibility = Visibility.Collapsed;
                pow.Visibility = Visibility.Collapsed;
                ln.Visibility = Visibility.Collapsed;
            }
        }
    }
    class Receiver
    {
        public double Info;
        public void Run(string operationCode, double operand)
        {
            switch (operationCode)
            {
                case "+":
                    Info += operand;
                    break;
                case "-":
                    Info -= operand;
                    break;
                case "*":
                    Info *= operand;
                    break;
                case "/":
                    Info /= operand;
                    break;
                case "^":
                    Info = Math.Pow(Info, operand);
                    break;
                case "e":
                    Info = Math.Exp(operand);
                    break;
                case "√":
                    Info = Math.Sqrt(operand);
                    break;
                case "ln":
                    Info = Math.Log10(operand);
                    break;
            }
        }
    }// логіка обрахування
    class Invoker
    {
        private List<Command> commands = new List<Command>();
        int curr = 0;
        public void StoreComm(Command command)
        {
            commands.Add(command);
        }
        public void ExecuteComm()
        {
            commands[curr].Execute();
            curr++;
        }
        public void Undo(int levels)
        {
            for (int i = 0; i < levels; i++)
            {
                if (curr > 0)
                    commands[--curr].UnExecute();
            }
        }
        public void Redo(int levels)
        {
            for (int i = 0; i < levels; i++)
            {
                if (curr < commands.Count - 1)
                    commands[curr++].Execute();
            }
        }
    }// збереження та откат команд + ініціалізація
    abstract class Command
    {
        protected Receiver receiver_;
        protected double operand_;
        public Command(Receiver receiver)
        {
            receiver_ = receiver;
        }
        public abstract void Execute();
        public abstract void UnExecute();

    }
    class Add : Command
    {
        public Add(Receiver receiver, double operand) : base(receiver)
        {
            receiver_ = receiver;
            operand_ = operand;
        }
        public override void Execute()
        {
            receiver_.Run("+", operand_);
        }
        public override void UnExecute()
        {
            receiver_.Run("-", operand_);
        }
    }
    class Sub : Command
    {
        public Sub(Receiver receiver, double operand) : base(receiver)
        {
            receiver_ = receiver;
            operand_ = operand;
        }
        public override void Execute()
        {
            receiver_.Run("-", operand_);
        }
        public override void UnExecute()
        {
            receiver_.Run("+", operand_);
        }
    }
    class Mul : Command
    {
        public Mul(Receiver receiver, double operand) : base(receiver)
        {
            receiver_ = receiver;
            operand_ = operand;
        }
        public override void Execute()
        {
            receiver_.Run("*", operand_);
        }
        public override void UnExecute()
        {
            receiver_.Run("/", operand_);
        }
    }
    class Div : Command
    {
        public Div(Receiver receiver, double operand) : base(receiver)
        {
            receiver_ = receiver;
            operand_ = operand;
        }
        public override void Execute()
        {
            receiver_.Run("/", operand_);
        }
        public override void UnExecute()
        {
            receiver_.Run("*", operand_);
        }
    }
    class Pow : Command
    {
        public Pow(Receiver receiver, double operand) : base(receiver)
        {
            receiver_ = receiver;
            operand_ = operand;
        }
        public override void Execute()
        {
            receiver_.Run("^", operand_);
        }
        public override void UnExecute()
        {
        }
    }
    class Exp : Command
    {
        public Exp(Receiver receiver, double operand) : base(receiver)
        {
            receiver_ = receiver;
            operand_ = operand;
        }
        public override void Execute()
        {
            receiver_.Run("e", operand_);
        }
        public override void UnExecute()
        {
        }
    }
    class Sqrt : Command
    {
        public Sqrt(Receiver receiver, double operand) : base(receiver)
        {
            receiver_ = receiver;
            operand_ = operand;
        }
        public override void Execute()
        {
            receiver_.Run("√", operand_);
        }
        public override void UnExecute()
        {
        }
    }
    class Ln10 : Command
    {
        public Ln10(Receiver receiver, double operand) : base(receiver)
        {
            receiver_ = receiver;
            operand_ = operand;
        }
        public override void Execute()
        {
            receiver_.Run("ln", operand_);
        }
        public override void UnExecute()
        {
        }
    }
    class Calculator
        {
            public Receiver receiver_;
            Invoker invoker_;
            public Calculator()
            {
                receiver_= new Receiver();
                invoker_ = new Invoker();
            }
            public Calculator(Receiver receiver)
            {
                receiver_ = new Receiver();
                receiver_.Info = receiver.Info;
                invoker_ = new Invoker();
            }
            private double Run(Command command)
            {
                invoker_.StoreComm(command);
                invoker_.ExecuteComm();
                return receiver_.Info;
            }
            public double Add(double operand)
            {
                return Run(new Add(receiver_, operand));
            }
            public double Sub(double operand)
            {
                return Run(new Sub(receiver_, operand));
            }
            public double Mul(double operand)
            {
                return Run(new  Mul(receiver_, operand)); 
            }
            public double Div(double operand)
            {
                return Run(new Div(receiver_, operand));
            }
            public double Pow(double operand)
            {
                return Run(new Pow(receiver_, operand));
            }
            public double Exp(double operand)
            {
                return Run(new Exp(receiver_, operand));
            }
            public double Sqrt(double operand)
            {
                return Run(new Sqrt(receiver_, operand));
            }
            public double Ln10(double operand) 
            {
                return Run(new Ln10(receiver_, operand));
            }
            public double Undo(int levels)
            {
                invoker_.Undo(levels);
                return receiver_.Info;
            }
            public double Redo(int levels)
            {
                invoker_.Redo(levels);
                return receiver_.Info;
            }
            public string C()
            {
                receiver_.Info = 0;
                return "0";
            }
            public string Erase()
            {
                string erasedInfo = Convert.ToString(receiver_.Info);
                erasedInfo.Substring(0, erasedInfo.Length - 1);
                receiver_.Info = Convert.ToDouble(erasedInfo);
                return erasedInfo;
            }
            public string PreviousCommand { get; set;}
        }
    }