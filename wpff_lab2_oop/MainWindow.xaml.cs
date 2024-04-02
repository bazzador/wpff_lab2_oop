using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
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
        private Calculator calc; // Поле класу
        private string previous;
        private Dictionary<string, bool> validOperators = new Dictionary<string, bool>()
        {
            {"+", true},
            {"-", true},
            {"×", true},
            {"÷", true}  
        };
        private bool isPrev = false;
        private Receiver rec_;
        public MainWindow()
        {
            InitializeComponent();
            //rec_ = new Receiver();
            calc = new Calculator();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string name = (string)((Button)e.OriginalSource).Content;

            if (name == "=")
            {
                // Перевірка на наявність попередньої операції та числа
                if (!string.IsNullOrEmpty(previous) && !string.IsNullOrEmpty(window.Text))
                {
                    int num;
                    if (int.TryParse(window.Text, out num))
                    {
                        // Виконання відповідної операції на основі попередньої операції та числа
                        switch (previous)
                        {
                            case "+":
                                window.Text = Convert.ToString(calc.Add(num));
                                break;
                            case "-":
                                window.Text = Convert.ToString(calc.Sub(num));
                                break;
                            case "×":
                                window.Text = Convert.ToString(calc.Mul(num));
                                break;
                            case "÷":
                                window.Text = Convert.ToString(calc.Div(num));
                                break;
                        }
                        // Очистка попередньої операції
                        previous = null;
                    }
                    else
                    {
                        MessageBox.Show("Введено нечислове значення!");
                    }
                }
                else
                {
                    MessageBox.Show("Неможливо виконати операцію. Відсутнє число або оператор!");
                }
            }
            else
            {
                // Логіка для додавання цифри або оператора до вікна введення
                if (isPrev)
                {
                    isPrev = false;
                    window.Text = "";
                }
                if (validOperators.ContainsKey(name))
                {
                    window_2.Text = window.Text + name;
                    previous = name;
                    isPrev = true;
                    calc.receiver_.Info = Convert.ToInt32(window.Text);
                }
                else
                {
                    window.Text += name;
                }
            }
            //string name = (string)((Button)e.OriginalSource).Content;
            ////bool isNum = int.TryParse(name, out int num);
            //if(name == "=")

            //if (isPrev)
            //{
            //    isPrev = false;
            //    window.Text = "";
            //}
            //if (validOperators.ContainsKey(name))
            //{
            //   window_2.Text = window.Text + name;
            //   previous = window.Text;
            //   isPrev = true;
            //}
            //else window.Text += name;

            //bool isNum = int.TryParse(name, out int num);
            //if (!isNum)
            //{
            //    switch (name)
            //    {
            //        case "+":
            //            previous = num;
            //            if (previous != 0)
            //            {
            //                window.Text = Convert.ToString(calc.Add(previous) + calc.Add(num));
            //            }
            //            break;
            //        case "-":
            //            window.Text = Convert.ToString(calc.Sub(num));
            //            break;
            //        case "*":
            //            window.Text = Convert.ToString(calc.Mul(num)); 
            //            break;
            //        case "/":
            //            window.Text = Convert.ToString(calc.Div(num));
            //            break;
            //    }
            //}
            //else window.Text += name;


        }
        class Receiver
        {
            public int Info;/*{  get; private set; }*/
            public void Run(char operationCode, int operand) 
            {
                switch(operationCode)
                {
                    case '+':
                        Info += operand;
                        break;
                    case '-':
                        Info -= operand;
                        break;
                    case '*':
                        Info *= operand;
                        break;
                    case '/':
                        Info /= operand;
                        break;
                }
            }
            public void Action()
            {
                Console.WriteLine("Receiver");
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
            protected int operand_;
            public Command(Receiver receiver)
            {
                receiver_ = receiver;
            }
            public abstract void Execute();
            public abstract void UnExecute();
        }
        class Add : Command
        {
            public Add(Receiver receiver, int operand) : base(receiver)
            {
                receiver_ = receiver;
                operand_ = operand; 
            }
            public override void Execute()
            {
                receiver_.Run('+', operand_);
            }
            public override void UnExecute()
            {
                receiver_.Run('-', operand_);
            }
        }
        class Sub : Command
        {
            public Sub(Receiver receiver, int operand) : base(receiver)
            {
                receiver_ = receiver;
                operand_ = operand;
            }
            public override void Execute()
            {
                receiver_.Run('-', operand_);
            }
            public override void UnExecute()
            {
                receiver_.Run('+', operand_);
            }
        }
        class Mul : Command
        {
            public Mul(Receiver receiver, int operand) : base(receiver)
            {
                receiver_ = receiver;
                operand_ = operand;
            }
            public override void Execute()
            {
                receiver_.Run('*', operand_);
            }
            public override void UnExecute()
            {
                receiver_.Run('/', operand_);
            }
        }
        class Div : Command
        {
            public Div(Receiver receiver, int operand) : base(receiver)
            {
                receiver_ = receiver;
                operand_ = operand;
            }
            public override void Execute()
            {
                receiver_.Run('/', operand_);
            }
            public override void UnExecute()
            {
                receiver_.Run('*', operand_);
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
            private int Run(Command command)
            {
                invoker_.StoreComm(command);
                invoker_.ExecuteComm();
                return receiver_.Info;
            }
            public int Add(int operand)
            {
                return Run(new Add(receiver_, operand));
            }
            public int Sub(int operand)
            {
                return Run(new Sub(receiver_, operand));
            }
            public int Mul(int operand)
            {
                return Run(new  Mul(receiver_, operand)); 
            }
            public int Div(int operand)
            {
                return Run(new Div(receiver_, operand));
            }
            public int Undo(int levels)
            {
                invoker_.Undo(levels);
                return receiver_.Info;
            }
            public int Redo(int levels)
            {
                invoker_.Redo(levels);
                return receiver_.Info;
            }
        }
    }
}
