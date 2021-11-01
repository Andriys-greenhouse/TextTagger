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
            Regex rx = new Regex(@"\<(?<tagStart>.((?!\<).)*)\>(?<tagContent>.((?!\<).)*)\<\/\1\>");

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

            //searching for unknown tags
            List<int> unknownTagIndexes = new List<int>();
            bool isKnown = false;
            foreach (Match ma in matches)
            {
                foreach (Tag tg in existingTags)
                {
                    if (ma.Groups["tagStart"].Value == tg.Name) 
                    { 
                        isKnown = true; 
                    }
                }

                if (!isKnown) 
                {
                    unknownTagIndexes.Add(ma.Index);
                }
            }

            //initialize localDelegate
            int lastIndex = 0;
            for (int i = 0; i < matches.Count; i++)
            {
                if (!unknownTagIndexes.Exists(g => g == i))
                {
                    switch (matches[i].Index)
                    {
                        case 0:
                            localDelegate = new WriteOutFunction(existingTags.Find(t => t.Name == matches[i].Groups["tagStart"].Value).Write);
                            methodInputs.Add(matches[i].Groups["tagContent"].Value);
                            lastIndex = matches[i].Length;
                            break;
                        default:
                            methodInputs.Add(input.Substring(0, matches[i].Index));
                            methodInputs.Add(matches[i].Groups["tagContent"].Value);
                            localDelegate = new WriteOutFunction(Console.Write);
                            localDelegate += existingTags.Find(t => t.Name == matches[i].Groups["tagStart"].Value).Write;
                            lastIndex = matches[i].Index + matches[i].Length;
                            break;
                    }
                }
            }


            //ujasnění kde začíná tag s jakým jménem
            foreach (Match match in matches)
            {
                if(match.Index - lastIndex != 0)
                {
                    methodInputs.Add(input.Substring(lastIndex, match.Index - lastIndex));
                    methodInputs.Add(match.Groups["tagContent"].Value);

                }
            }
            //  -> naplnit methodInputs rozkouskovaným input pro jednotlivé metody tagů (tagContenty a normální text)

            

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
    https://www.regular-expressions.info/backref.html
 */