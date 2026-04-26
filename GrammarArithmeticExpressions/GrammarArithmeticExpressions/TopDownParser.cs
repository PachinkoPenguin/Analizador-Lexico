using System;
using System.Collections.Generic;

namespace GrammarArithmeticExpressions
{
    public static class TopDownParserHelper
    {
        /// <summary>
        /// Evaluates a given arithmetic expression string based on the implemented grammar.
        /// This method creates an instance of the <see cref="TopDownParser"/> class and parses the input string.
        /// </summary>
        /// <param name="input">The arithmetic expression to evaluate. The input should be a valid expression
        /// consisting of numbers, addition ('+'), multiplication ('*'), and parentheses ('(', ')').</param>
        /// <returns>The result of evaluating the arithmetic expression as an integer.</returns>
        /// <exception cref="Exception">Throws an exception if the input contains syntax errors or invalid characters.</exception>
        /// <example>
        /// Example usage:
        /// <code>
        /// int result = TopDownParserHelper.Evaluate("3 + 5 * (2 + 4)");
        /// Console.WriteLine(result); // Output: 33
        /// </code>
        /// </example>
        public static int Evaluate(string input)
        {
            var parser = new TopDownParser(input); // Create a new parser instance
            return parser.Parse(); // Parse and evaluate the input
        }
    }

    public class TopDownParser
    {
        private readonly string _input; // The input string to be parsed
        private int _position; // Current position in the input string
        private char _currentChar; // The current character being processed

        public TopDownParser(string input)
        {
            // Remove spaces from the input for simplicity
            _input = input.Replace(" ", "");
            _position = 0;
            // Initialize the current character to the first character of the input
            _currentChar = _input.Length > 0 ? _input[0] : '\0';
        }

        // Advances the position to the next character in the input
        private void Advance()
        {
            _position++;
            // Update the current character or set it to '\0' if at the end of the input
            _currentChar = _position < _input.Length ? _input[_position] : '\0';
        }

        // Matches the current character with the expected character
        // If they match, it advances to the next character
        // Otherwise, it throws a syntax error
        private void Match(char expected)
        {
            if (_currentChar == expected)
            {
                Advance();
            }
            else
            {
                throw new Exception($"Syntax Error: Expected '{expected}' but found '{_currentChar}'");
            }
        }

        // Parses the input string and returns the result of the evaluation
        public int Parse()
        {
            return Expression(); // Start parsing from the top-level rule (Expression)
        }

        // Parses an Expression based on the grammar:
        // E -> T | T + E
        private int Expression()
        {
            int result = Term(); // Parse the first term

            // Handle addition operations
            while (_currentChar == '+')
            {
                Match('+'); // Match the '+' symbol
                result += Term(); // Parse the next term and add it to the result
            }

            return result;
        }

        // Parses a Term based on the grammar:
        // T -> F | F * T
        private int Term()
        {
            int result = Factor(); // Parse the first factor

            // Handle multiplication operations
            while (_currentChar == '*')
            {
                Match('*'); // Match the '*' symbol
                result *= Factor(); // Parse the next factor and multiply it with the result
            }

            return result;
        }

        // Parses a Factor based on the grammar:
        // F -> (E) | number
        private int Factor()
        {
            if (_currentChar == '(')
            {
                Match('('); // Match the '(' symbol
                int result = Expression(); // Parse the expression inside the parentheses
                Match(')'); // Match the ')' symbol
                return result;
            }
            else if (char.IsDigit(_currentChar))
            {
                return Number(); // Parse a number
            }
            else
            {
                throw new Exception($"Syntax Error: Unexpected character '{_currentChar}'");
            }
        }

        // Parses a number (sequence of digits) and returns its integer value
        private int Number()
        {
            string number = string.Empty;

            // Collect all consecutive digits
            while (char.IsDigit(_currentChar))
            {
                number += _currentChar;
                Advance(); // Move to the next character
            }

            return int.Parse(number); // Convert the collected digits to an integer
        }
    }
}