using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DrawFunct
{
    //kiedys trzeba bedzie posprzatac kod ;)
    class Funct
    {
        public string ParsedString = "";
        public string PreParsed = "";
        const float eulers = 2.718f;

        public Funct( string p )
        {
            PreParsed = p;
            ParsedString = p;
        }

        public void Parse()
        {
            //whitespaces - final
            ParsedString = ParsedString.Replace(" ", "");

            //wielokrotnosc x - final
            foreach (Match m in Regex.Matches(ParsedString, @"(\d+x)"))
            {
                ParsedString = ParsedString.Replace(m.Value, string.Format("{0}*{1}", m.Value.Substring(0, m.Value.IndexOf('x')), "x"));
            }

            //mnozenie przed nawiasem
            foreach (Match m in Regex.Matches(ParsedString, @"([\d|x]+\([\d|x]+\))") )
            {
                ParsedString = ParsedString.Insert(m.Index + m.Value.IndexOf('('), "*");
                ParsedString = ParsedString.Replace("Log10*", "Log10"); // temporary fix
            }

            //mnozenie po nawiasie
            foreach (Match m in Regex.Matches(ParsedString, @"(\([\d|x]+\))[\d|x]+"))
            {
                ParsedString = ParsedString.Insert(m.Index + 1, "*");
            }

            //mnozenie miedzy nawiasami
            foreach (Match m in Regex.Matches(ParsedString, @"\)\("))
            {
                ParsedString = ParsedString.Insert(m.Index + 1, "*");
            }

            //nawiasy do poteg
            foreach (Match m in Regex.Matches(ParsedString, @"(\(.*?\)\^\d+)"))
            {
                ParsedString = ParsedString.Replace(m.Value, string.Format("Pow{0},{1})", m.Value.Split( ')' )));
            }

            //potęgi
            foreach ( Match m in Regex.Matches( ParsedString, @"([xe|\d]+\^[xe|\d+])") )
            {
                ParsedString = ParsedString.Replace(m.Value, string.Format("Pow({0},{1})", m.Value.Split('^')));
            }

            //eulers
            ParsedString = ParsedString.Replace("e", eulers.ToString().Replace(",", "."));

            //pozbycie sie znaków specjalnych
            ParsedString = ParsedString.Replace("^", "");
            ParsedString = ParsedString.Replace("f", "");
        }

        public string Determine( float x )
        {
            Regex parser = new Regex("x");
            string rx = x.ToString();
            rx = rx.Replace(",", ".");
            rx = rx.Replace("f", "");

            return parser.Replace(ParsedString, rx);
        }
    }
}
