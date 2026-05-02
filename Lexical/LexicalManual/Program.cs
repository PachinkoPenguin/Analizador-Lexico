using System;
using System.Collections.Generic;
using System.Linq;

namespace LexicalManualAnalyzer
{
    /// <summary>
    /// Token class represents a lexical token with type and value
    /// </summary>
    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public Token(string type, string value, int line, int column)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return $"[{Line}:{Column}] {Type}: {Value}";
        }
    }

    /// <summary>
    /// Symbol Table entry for tracking identifiers and their types
    /// </summary>
    public class SymbolTableEntry
    {
        public string Token { get; set; }
        public string Identifier { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Error { get; set; }

        public SymbolTableEntry(string token, string identifier, string value, string type, string error)
        {
            Token = token;
            Identifier = identifier;
            Value = value;
            Type = type;
            Error = error;
        }

        public override string ToString()
        {
            return $"{Token,-15} | {Identifier,-20} | {Value,-20} | {Type,-15} | {Error}";
        }
    }

    /// <summary>
    /// Manual Lexical Analyzer using state machine (finite automaton)
    /// No regex, pure character-by-character processing
    /// </summary>
    public class LexicalAnalyzer
    {
        private string _input;
        private int _position;
        private int _line;
        private int _column;
        private List<Token> _tokens;
        private List<SymbolTableEntry> _symbolTable;
        private HashSet<string> _keywords;

        public LexicalAnalyzer(string input)
        {
            _input = input;
            _position = 0;
            _line = 1;
            _column = 1;
            _tokens = new List<Token>();
            _symbolTable = new List<SymbolTableEntry>();

            // Initialize Python-like keywords
            _keywords = new HashSet<string>
            {
                "def", "return", "if", "else", "elif", "for", "while", "in", "is",
                "and", "or", "not", "True", "False", "None", "pass", "break", "continue"
            };
        }

        public List<Token> GetTokens() => _tokens;
        public List<SymbolTableEntry> GetSymbolTable() => _symbolTable;

        /// <summary>
        /// Main lexical analysis method using finite state machine
        /// </summary>
        public void Analyze()
        {
            while (_position < _input.Length)
            {
                char current = _input[_position];

                // Skip whitespace except newlines
                if (char.IsWhiteSpace(current) && current != '\n')
                {
                    AdvancePosition();
                    continue;
                }

                // Handle newlines
                if (current == '\n')
                {
                    _line++;
                    _column = 0;
                    AdvancePosition();
                    continue;
                }

                // String literals (single or double quotes)
                if (current == '"' || current == '\'')
                {
                    ScanString(current);
                    continue;
                }

                // Numbers
                if (char.IsDigit(current))
                {
                    ScanNumber();
                    continue;
                }

                // Identifiers and keywords
                if (char.IsLetter(current) || current == '_')
                {
                    ScanIdentifierOrKeyword();
                    continue;
                }

                // Multi-character operators
                if (_position + 1 < _input.Length)
                {
                    string twoChar = _input.Substring(_position, 2);
                    if (ScanMultiCharOperator(twoChar))
                        continue;
                }

                // Single-character operators and delimiters
                if (ScanSingleCharOperator(current))
                    continue;

                // Unknown character - error
                AddToken("ERROR", current.ToString(), $"Unknown character: '{current}'");
                AdvancePosition();
            }
        }

        private void ScanString(char quote)
        {
            int startLine = _line;
            int startCol = _column;
            string value = string.Empty;
            value += quote;
            AdvancePosition();

            while (_position < _input.Length && _input[_position] != quote)
            {
                if (_input[_position] == '\\' && _position + 1 < _input.Length)
                {
                    value += _input[_position];
                    AdvancePosition();
                    value += _input[_position];
                    AdvancePosition();
                }
                else
                {
                    value += _input[_position];
                    AdvancePosition();
                }
            }

            if (_position < _input.Length)
            {
                value += _input[_position];
                AdvancePosition();
                AddToken("STRING", value);
                _symbolTable.Add(new SymbolTableEntry("STRING", "(literal)", value, "string", "—"));
            }
            else
            {
                // Unterminated string
                AddToken("ERROR", value, "Unterminated string literal");
                _symbolTable.Add(new SymbolTableEntry("STRING", "(literal)", value, "string", "String not terminated: missing closing quote"));
            }
        }

        private void ScanNumber()
        {
            int startLine = _line;
            int startCol = _column;
            string value = string.Empty;

            // Integer part
            while (_position < _input.Length && char.IsDigit(_input[_position]))
            {
                value += _input[_position];
                AdvancePosition();
            }

            // Decimal part
            if (_position < _input.Length && _input[_position] == '.')
            {
                value += _input[_position];
                AdvancePosition();

                while (_position < _input.Length && char.IsDigit(_input[_position]))
                {
                    value += _input[_position];
                    AdvancePosition();
                }
            }

            // Scientific notation (e.g., 1e10)
            if (_position < _input.Length && (_input[_position] == 'e' || _input[_position] == 'E'))
            {
                value += _input[_position];
                AdvancePosition();

                if (_position < _input.Length && (_input[_position] == '+' || _input[_position] == '-'))
                {
                    value += _input[_position];
                    AdvancePosition();
                }

                while (_position < _input.Length && char.IsDigit(_input[_position]))
                {
                    value += _input[_position];
                    AdvancePosition();
                }
            }

            // Detect invalid identifier like: 2bad
            if (_position < _input.Length && (char.IsLetter(_input[_position]) || _input[_position] == '_'))
            {
                while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
                {
                    value += _input[_position];
                    AdvancePosition();
                }

                AddToken("ERROR", value, "Identifier cannot start with digit");
                _symbolTable.Add(new SymbolTableEntry("NAME", value, "—", "identifier", "Invalid identifier: cannot start with digit"));
                return;
            }

            AddToken("NUMBER", value);
            _symbolTable.Add(new SymbolTableEntry("NUMBER", value, value, "number", "—"));
        }

        private void ScanIdentifierOrKeyword()
        {
            int startLine = _line;
            int startCol = _column;
            string value = string.Empty;

            // Check for invalid identifier (starts with digit)
            if (char.IsDigit(_input[_position]))
            {
                value += _input[_position];
                AdvancePosition();
                AddToken("ERROR", value, "Identifier cannot start with digit");
                _symbolTable.Add(new SymbolTableEntry("NAME", value, "—", "identifier", "Invalid identifier: cannot start with digit"));
                return;
            }

            while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
            {
                value += _input[_position];
                AdvancePosition();
            }

            if (_keywords.Contains(value))
            {
                AddToken(value.ToUpperInvariant(), value);
            }
            else
            {
                AddToken("NAME", value);
                _symbolTable.Add(new SymbolTableEntry("NAME", value, "—", "identifier", "—"));
            }
        }

        private bool ScanMultiCharOperator(string twoChar)
        {
            // Two-character operators
            switch (twoChar)
            {
                case "==":
                    AddToken("EQEQ", twoChar);
                    AdvancePosition();
                    AdvancePosition();
                    return true;
                case "!=":
                    AddToken("NEQ", twoChar);
                    AdvancePosition();
                    AdvancePosition();
                    return true;
                case "<=":
                    AddToken("LTE", twoChar);
                    AdvancePosition();
                    AdvancePosition();
                    return true;
                case ">=":
                    AddToken("GTE", twoChar);
                    AdvancePosition();
                    AdvancePosition();
                    return true;
                case "//":
                    AddToken("FLOORDIV", twoChar);
                    AdvancePosition();
                    AdvancePosition();
                    return true;
                case "**":
                    AddToken("POWER", twoChar);
                    AdvancePosition();
                    AdvancePosition();
                    return true;
                case "->":
                    AddToken("ARROW", twoChar);
                    AdvancePosition();
                    AdvancePosition();
                    return true;
                case ":=":
                    AddToken("WALRUS", twoChar);
                    AdvancePosition();
                    AdvancePosition();
                    return true;
                default:
                    return false;
            }
        }

        private bool ScanSingleCharOperator(char c)
        {
            switch (c)
            {
                case '+':
                    AddToken("PLUS", c.ToString());
                    AdvancePosition();
                    return true;
                case '-':
                    AddToken("MINUS", c.ToString());
                    AdvancePosition();
                    return true;
                case '*':
                    AddToken("TIMES", c.ToString());
                    AdvancePosition();
                    return true;
                case '/':
                    AddToken("DIVIDE", c.ToString());
                    AdvancePosition();
                    return true;
                case '%':
                    AddToken("MOD", c.ToString());
                    AdvancePosition();
                    return true;
                case '=':
                    AddToken("ASSIGN", c.ToString());
                    AdvancePosition();
                    return true;
                case '<':
                    AddToken("LT", c.ToString());
                    AdvancePosition();
                    return true;
                case '>':
                    AddToken("GT", c.ToString());
                    AdvancePosition();
                    return true;
                case '(':
                    AddToken("LPAREN", c.ToString());
                    AdvancePosition();
                    return true;
                case ')':
                    AddToken("RPAREN", c.ToString());
                    AdvancePosition();
                    return true;
                case '[':
                    AddToken("LBRACKET", c.ToString());
                    AdvancePosition();
                    return true;
                case ']':
                    AddToken("RBRACKET", c.ToString());
                    AdvancePosition();
                    return true;
                case '{':
                    AddToken("LBRACE", c.ToString());
                    AdvancePosition();
                    return true;
                case '}':
                    AddToken("RBRACE", c.ToString());
                    AdvancePosition();
                    return true;
                case ',':
                    AddToken("COMMA", c.ToString());
                    AdvancePosition();
                    return true;
                case ':':
                    AddToken("COLON", c.ToString());
                    AdvancePosition();
                    return true;
                case ';':
                    AddToken("SEMICOLON", c.ToString());
                    AdvancePosition();
                    return true;
                case '.':
                    AddToken("DOT", c.ToString());
                    AdvancePosition();
                    return true;
                case '@':
                    AddToken("AT", c.ToString());
                    AdvancePosition();
                    return true;
                default:
                    return false;
            }
        }

        private void AddToken(string type, string value, string? error = null)
        {
            _tokens.Add(new Token(type, value, _line, _column));
        }

        private void AdvancePosition()
        {
            if (_position < _input.Length)
            {
                if (_input[_position] == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }
                _position++;
            }
        }

        public void PrintTokens()
        {
            Console.WriteLine("\n=== TOKENS ===");
            foreach (var token in _tokens)
            {
                Console.WriteLine(token);
            }
        }

        public void PrintSymbolTable()
        {
            Console.WriteLine("\n=== SYMBOL TABLE ===");
            Console.WriteLine($"{"Token",-15} | {"Identifier",-20} | {"Value",-20} | {"Type",-15} | Error");
            Console.WriteLine(new string('-', 95));
            foreach (var entry in _symbolTable)
            {
                Console.WriteLine(entry);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== MANUAL LEXICAL ANALYZER (State Machine) ===\n");
            Console.WriteLine("Enter a Python-like expression:");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input ?? ""))
            {
                Console.WriteLine("Error: Input cannot be empty.");
                return;
            }

            LexicalAnalyzer analyzer = new LexicalAnalyzer(input ?? "");
            analyzer.Analyze();

            analyzer.PrintTokens();
            analyzer.PrintSymbolTable();
        }
    }
}
