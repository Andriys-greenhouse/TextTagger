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
            args = new string[] { @"K této knize se svět chová velice <capital>laskavě</capital>. Jen z různých edicí vydaných v <red>Anglii</red> se už prodalo přes půldruhého miliónu výtisků. A v <hide>Chicagu</hide> se mi už před mnoha lety dostalo ujištění - z úst jistého podnikavého piráta nyní v. v. - že víc než <highlight>milión výtisků</highlight> se prodalo ve <green>Spojených státech</green>." };
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
            Regex rx = new Regex(@"\<(?<tagStart>.*)\>(?<tagContent>.[^\<]*)\<\/\1\>", RegexOptions.Multiline);

            //iniciation of tags
            List<Tag> existingTags = new List<Tag>();
            existingTags.Add(new Tag("red", new WriteOutFunction(WriteInRed)));
            existingTags.Add(new Tag("capital", new WriteOutFunction(WriteInCapital)));
            existingTags.Add(new Tag("hide", new WriteOutFunction(WriteHidden)));
            existingTags.Add(new Tag("green", new WriteOutFunction(WriteInGreen)));
            existingTags.Add(new Tag("blue", new WriteOutFunction(WriteInBlue)));

            MatchCollection matches = rx.Matches(input);
            List<string> methodInputs = new List<string>();
            WriteOutFunction localDelegate = new WriteOutFunction(Console.Write);

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
                isKnown = false;
            }

            //initialize localDelegate
            int lastIndex = 0;
            foreach (Match mch in matches)
            {
                if (!unknownTagIndexes.Exists(g => g == mch.Index))
                {
                    switch (mch.Index)
                    {
                        case 0:
                            localDelegate = new WriteOutFunction(existingTags.Find(t => t.Name == mch.Groups["tagStart"].Value).Write);
                            methodInputs.Add(mch.Groups["tagContent"].Value);
                            lastIndex = mch.Length;
                            break;
                        default:
                            methodInputs.Add(input.Substring(0, mch.Index));
                            methodInputs.Add(mch.Groups["tagContent"].Value);
                            localDelegate = new WriteOutFunction(Console.Write);
                            localDelegate += existingTags.Find(t => t.Name == mch.Groups["tagStart"].Value).Write;
                            lastIndex = mch.Index + mch.Length;
                            break;
                    }
                    break;
                }
            }


            //add rest of methods and inputs
            foreach (Match mch in matches)
            {
                if (!unknownTagIndexes.Exists(g => g == mch.Index) && mch.Index != matches[0].Index)
                {
                    if (mch.Index - lastIndex != 0)
                    {
                        methodInputs.Add(input.Substring(lastIndex, mch.Index - lastIndex));
                        methodInputs.Add(mch.Groups["tagContent"].Value);
                        localDelegate += Console.Write;
                        localDelegate += existingTags.Find(t => t.Name == mch.Groups["tagStart"].Value).Write;
                    }
                    else
                    {
                        methodInputs.Add(mch.Groups["tagContent"].Value);
                        localDelegate += existingTags.Find(t => t.Name == mch.Groups["tagStart"].Value).Write;
                    }
                    lastIndex = mch.Index + mch.Length;
                }
            }

            for (int i = 0; i < methodInputs.Count; i++)
            {
                localDelegate.Invoke(methodInputs[i]);
            }

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