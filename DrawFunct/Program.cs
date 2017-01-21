using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using NCalc;
using ElegantForms;

//#bugs
//- w przypadku łączenia asymptot - zawężyć zakres
//

namespace DrawFunct
{
    class Program
    {
        static void Parse( Funct exp, bool preparsed, int _start, float _staging, int _end )
        {
            //Funct exp = new Funct("x^3 + x^2 - 16x - 16"); // zadanie A, final
            //Funct exp = new Funct("x^4 - (5/4)x + (1/4)"); // zadanie B, final
            //Funct exp = new Funct("x^2(x^2-4)^3"); // zadanie C, final
            //Funct exp = new Funct("x + (4/(x-5))"); // zadanie D, final - ale możnaby dodać rysowanie asymptot
            //Funct exp = new Funct("(x+1)^2/(2x)"); // zadanie E, final
            //Funct exp = new Funct("(x^2+x+1)/(x^2-1)"); // zadanie F, final
            //Funct exp = new Funct("(x^4)/(2-x^3)"); // zadanie G, ale łączy asymptoty więc należy zmniejszyć staging
            //Funct exp = new Funct("Sqrt((1-x)/(1+x))"); // zadanie H, ale liczy nierealne i należy zaweżyć zakres
            //Funct exp = new Funct("(3x-1)/(2x+1)"); // zadanie I, final
            //Funct exp = new Funct("(x)/(1+x^2)"); // zadanie J, final
            //Funct exp = new Funct("(x^2-x-1)/(x-1)"); // zadanie K, final
            //Funct exp = new Funct("x^3/(x-1)^2"); // zadanie L, final
            //Funct exp = new Funct("(Cos(x)^2) - 2(Sin(x)^2)"); // zadanie M, bad
            //Funct exp = new Funct("(Sin(x)^2)(Cos(x))"); // Zadanie N, bad
            //Funct exp = new Funct("(e^x)/(x+1)"); // Zadanie P, final

            if (!preparsed)
            {
                exp.Parse();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Formula: { exp.PreParsed }");
            Console.WriteLine($"Parsed: { exp.ParsedString }\n");
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            int start = _start;
            float staging = _staging;
            int end = _end;
            bool unreals = false;
            bool asymptosis = false;

            List<PointF> points = new List<PointF>();
            for (float i = start; i <= end; i += staging)
            {
                Expression t = new Expression(exp.Determine(staging * i));
                Console.WriteLine(exp.Determine(staging * i));

                float x = 0;
                float.TryParse(t.Evaluate().ToString(), out x);
                if (Double.IsNaN(-x))
                {
                    unreals = true;
                    x = 0;
                }

                float a = 0;
                a = -i < start ? start : -i;
                a = -i > end ? end : -i;

                PointF point = new PointF(a, -x);
                points.Add(point);

                Console.WriteLine($"X:{staging * i} \t {t.Evaluate()}");
            }

            ElegantForm main = new ElegantForm();
            main.SetBounds(5, 5, 800, 800);
            main.points = points.ToArray();
            main.start = start;
            main.staging = staging;
            main.end = end;
            main.PreParsedFormula = exp.PreParsed;
            main.Unreals = unreals;
            main.Asymptotis = asymptosis;
            main.ShowDialog();
            main.Update();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Wybierz tryb wpisywania formuły:");
            Console.WriteLine("1 - Surowy");
            Console.WriteLine("2 - Przetworzony");
            int choice = Int32.Parse( Console.ReadLine() );
            Console.Clear();
            switch (choice)
            {
                case 1:
                    Console.WriteLine("Wybrano tryb surowy, nie wszystkie formuły będą przetworzone poprawnie");
                    break;
                case 2:
                    Console.WriteLine("Wybrano tryb przetworzony, wszystkie formuły (o ile wpisane poprawnie) będą interpretowane bezpośrednio\n");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Dozwolone komendy:\nLog10( x ) - logarytm dziesiętny z x\nPow( x, y ) - x do potęgi y\nLog( x, y ) - Logarytm o podstawie x do y\nSqrt( x ) - Pierwiastek z x\ne - stała Eulera");
                    break;
                default:
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nFormuła: ");
            Funct exp = new Funct( Console.ReadLine() );
            Console.Clear();

            Console.Write("Zakres od: ");
            int start = Int32.Parse( Console.ReadLine() );

            Console.Write("\nZakres do: ");
            int end = Int32.Parse(Console.ReadLine());

            Console.Write("\nPrzesunięcie X: ");
            string input = Console.ReadLine();
            float f;
            float.TryParse(input, out f);

            Console.WriteLine(f);

            Parse(exp, choice == 2 ? true : false, start, f, end);

            while (true)
            {
            }
        }
    }
}
