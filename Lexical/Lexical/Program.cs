using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Interpreter
{
    class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public Token(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type}: {Value} ";
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Enter the expression: ");
            string input = Console.ReadLine();
            var tokens = LexicalAnalysis(input);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
            Console.ReadLine();
        }

        static List<Token> LexicalAnalysis(string input)
        {
            var tokenPatterns = new List<(string type, string pattern)>()
            {
                ("NUMBER", @"\d+(\.\d*)?"), // 9.8 , 9, 1 Integer or float Number
                ("ASSIGN", @"="), // =
                ("END", @";"), 
                ("ID", @"[a-zA-Z_]\w*"), // Variable name
                ("OP", @"[+\-*/]"), // +, -, *, /
                ("WHITESPACE", @"\s+"), // space
                ("ERROR", @".") // Any character
            };

            string pattern = string.Join("|", tokenPatterns.ConvertAll(t => $"(?<{t.type}>{t.pattern})"));
            var regex = new Regex(pattern);

            List<Token> tokens = new List<Token>();
            
            foreach(Match match in regex.Matches(input))
            {
                foreach(var tokenType in tokenPatterns)
                {
                    if(match.Groups[tokenType.type].Success)
                    {
                        if(tokenType.type == "WHITESPACE") break;


                        tokens.Add(new Token(tokenType.type, match.Value));
                        break;
                    }
                }
            }
            return tokens;
        }

    }

}