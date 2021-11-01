using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TextTagger
{
    class Program
    {
        static void Main(string[] args)
        {
            string input;
            if (args.Length < 1) { throw new ArgumentException("No argument given!"); }
            if (args.Length > 1)
            {
                StringBuilder sb = new StringBuilder("");
                foreach (string argument in args)
                {
                    sb.Append(argument);
                }
                input = sb.ToString();
            }
            else { input = args[0]; }
            Regex rx = new Regex(@"\<(?<tagStart>.((?!\<).)*)\>(?<tagContent>.((?!\<).)*)\<\/(?<tagEnd>.((?!\<).)*)\>");

            //iniciation of tags
            List<Tag> existingTags = new List<Tag>();
            existingTags.Add(new Tag("red", new WriteOutFunction(WriteInRed)));
            existingTags.Add(new Tag("capital", new WriteOutFunction(WriteInCapital)));
            existingTags.Add(new Tag("hide", new WriteOutFunction(WriteHidden)));
            existingTags.Add(new Tag("green", new WriteOutFunction(WriteInGreen)));
            existingTags.Add(new Tag("blue", new WriteOutFunction(WriteInBlue)));

            MatchCollection matches = rx.Matches(input);
            List<string> methodInputs = new List<string>();
            WriteOutFunction localDelegate;

            //kontrola pořadí tagů
            //ujasnění kde začíná tag s jakým jménem
            //  -> naplnit methodInputs rozkouskovaným input pro jednotlivé metody tagů (tagContenty a normální text)

            switch (matches[0].Index)
            {
                case 0:
                    //localDelegate = new WriteOutFunction( metodaPrvnihoTagu );
                    break;
                default:
                    localDelegate = new WriteOutFunction(Console.Write);
                    break;
            }

            //navěšení metod na localDelegate
            //volání metod pod localDelegate pomocí foreach se vstupy z methodInputs

            Console.ReadKey();
        }

        //methods of tags
        public static void WriteInRed(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(input);
            Console.ResetColor();
        }

        public static void WriteInCapital(string input)
        {
            Console.Write(input.ToUpper());
        }

        public static void WriteHidden(string input)
        {
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case ' ':
                        sb.Append(' ');
                        break;
                    default:
                        sb.Append('*');
                        break;
                }
            }
            Console.Write(sb.ToString());
        }

        public static void WriteInGreen(string input)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(input);
            Console.ResetColor();
        }

        public static void WriteInBlue(string input)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(input);
            Console.ResetColor();
        }
    }
}
/*
used resources:
    https://regex101.com/
    https://stackoverflow.com/questions/406230/regular-expression-to-match-a-line-that-doesnt-contain-a-word
    https://www.regular-expressions.info/refext.html
 */