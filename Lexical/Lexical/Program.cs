using System;
using System.Collections.Generic;
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

    class SymbolTableEntry
    {
        public string Token { get; set; }
        public string Identifier { get; set; }
        public string Value { get; set; }
        public string Error { get; set; }

        public SymbolTableEntry(string token, string identifier, string value, string error)
        {
            Token = token;
            Identifier = identifier;
            Value = value;
            Error = error;
        }

        public override string ToString()
        {
            return $"{Token,-12} | {Identifier,-20} | {Value,-20} | {Error}";
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter the expression: ");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Error: Input cannot be empty.");
                return;
            }

            var symbolTable = new List<SymbolTableEntry>();
            var tokens = LexicalAnalysis(input, symbolTable);

            Console.WriteLine("\n=== TOKENS ===");
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }

            Console.WriteLine("\n=== SYMBOL TABLE ===");
            Console.WriteLine($"{"Token",-12} | {"Identifier",-20} | {"Value",-20} | Error");
            Console.WriteLine(new string('-', 72));
            foreach (var entry in symbolTable)
            {
                Console.WriteLine(entry);
            }
        }

        static List<Token> LexicalAnalysis(string input, List<SymbolTableEntry> symbolTable)
        {
            var tokenPatterns = new List<(string type, string pattern)>()
            {
                ("STRING", "\"([^\"\\\\]|\\\\.)*\"|'([^'\\\\]|\\\\.)*'"),
                ("UNTERMINATED_STRING", "\"(?:[^\"\\\\]|\\\\.)*|'(?:[^'\\\\]|\\\\.)*"),
                ("INVALID_ID", @"\d+[A-Za-z_]\w*"), // Identifier starting with digit (invalid)
                ("NUMBER", @"\d+(\.\d*)?([eE][+-]?\d+)?"), // Integer, float, scientific
                ("FLOORDIV", @"//"),
                ("POWER", @"\*\*"),
                ("EQEQ", @"=="),
                ("NEQ", @"!="),
                ("LTE", @"<="),
                ("GTE", @">="),
                ("ASSIGN", @"="), // =
                ("LT", @"<"),
                ("GT", @">"),
                ("END", @";"),
                ("DELIM", @"[(){}\[\],:]"),
                ("ID", @"[a-zA-Z_]\w*"), // Variable name
                ("OP", @"[+\-*/%]"), // +, -, *, /, %
                ("WHITESPACE", @"\s+"), // space
                ("ERROR", @".") // Any character
            };

            var keywords = new HashSet<string>(StringComparer.Ordinal)
            {
                "def", "return", "if", "else", "elif", "for", "while", "in", "is",
                "and", "or", "not", "True", "False", "None", "pass", "break", "continue"
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

                        if (tokenType.type == "ID" && keywords.Contains(match.Value))
                        {
                            tokens.Add(new Token("KEYWORD", match.Value));
                            break;
                        }
                        
                        if (tokenType.type == "INVALID_ID")
                        {
                            symbolTable.Add(new SymbolTableEntry("NAME", match.Value, "-", "Invalid identifier: cannot start with digit"));
                            tokens.Add(new Token("ERROR", match.Value));
                            break;
                        }

                        if (tokenType.type == "UNTERMINATED_STRING")
                        {
                            symbolTable.Add(new SymbolTableEntry("STRING", "(literal)", match.Value, "String not terminated: missing closing quote"));
                            tokens.Add(new Token("ERROR", match.Value));
                            break;
                        }

                        if (tokenType.type == "ID")
                        {
                            symbolTable.Add(new SymbolTableEntry("NAME", match.Value, "-", "-"));
                        }
                        else if (tokenType.type == "NUMBER")
                        {
                            symbolTable.Add(new SymbolTableEntry("NUMBER", match.Value, match.Value, "-"));
                        }
                        else if (tokenType.type == "STRING")
                        {
                            symbolTable.Add(new SymbolTableEntry("STRING", "(literal)", match.Value, "-"));
                        }
                        else if (tokenType.type == "UNTERMINATED_STRING")
                        {
                            // Already handled above
                        }
                        else if (tokenType.type == "ERROR")
                        {
                            symbolTable.Add(new SymbolTableEntry("UNKNOWN", match.Value, "-", "Invalid character"));
                        }

                        tokens.Add(new Token(tokenType.type, match.Value));
                        break;
                    }
                }
            }
            return tokens;
        }

    }

}