using System;
using System.Collections.Generic;

namespace GrammarArithmeticExpressions
{
    /// <summary>
    /// A simple bottom-up parser for arithmetic expressions.
    /// This parser evaluates expressions consisting of integers, addition, and multiplication.
    /// Grammar:
    /// Expr -> Expr + Term | Term
    /// Term -> Term * Factor | Factor
    /// Factor -> ( Expr ) | Number
    /// </summary>
    public class BottomParser
    {
        /// <summary>
        /// Parses and evaluates an arithmetic expression using a bottom-up approach.
        /// </summary>
        /// <param name="input">The arithmetic expression as a string.</param>
        /// <returns>The result of the evaluated expression.</returns>
        public int Parse(string input)
        {
            // Tokenize the input string into numbers and operators
            var tokens = Tokenize(input);

            // Use a stack to evaluate the expression
            var values = new Stack<int>();
            var operators = new Stack<char>();

            for (int i = 0; i < tokens.Count; i++)
            {
                string token = tokens[i];

                if (int.TryParse(token, out int number))
                {
                    // Push numbers directly onto the value stack
                    values.Push(number);
                }
                else if (token == "+" || token == "*")
                {
                    // Push operators onto the operator stack
                    while (operators.Count > 0 && Precedence(operators.Peek()) >= Precedence(token[0]))
                    {
                        EvaluateTop(values, operators);
                    }
                    operators.Push(token[0]);
                }
                else if (token == "(")
                {
                    // Push opening parenthesis onto the operator stack
                    operators.Push('(');
                }
                else if (token == ")")
                {
                    // Evaluate until the matching opening parenthesis is found
                    while (operators.Peek() != '(')
                    {
                        EvaluateTop(values, operators);
                    }
                    operators.Pop(); // Remove the '('
                }
            }

            // Evaluate any remaining operators
            while (operators.Count > 0)
            {
                EvaluateTop(values, operators);
            }

            // The final result is the only value left on the stack
            return values.Pop();
        }

        /// <summary>
        /// Tokenizes the input string into a list of numbers and operators.
        /// </summary>
        private List<string> Tokenize(string input)
        {
            var tokens = new List<string>();
            int i = 0;

            while (i < input.Length)
            {
                if (char.IsWhiteSpace(input[i]))
                {
                    i++;
                }
                else if (char.IsDigit(input[i]))
                {
                    int start = i;
                    while (i < input.Length && char.IsDigit(input[i]))
                    {
                        i++;
                    }
                    tokens.Add(input.Substring(start, i - start));
                }
                else if ("()+*".Contains(input[i]))
                {
                    tokens.Add(input[i].ToString());
                    i++;
                }
                else
                {
                    throw new ArgumentException($"Invalid character in input: {input[i]}");
                }
            }

            return tokens;
        }

        /// <summary>
        /// Evaluates the top operator on the operator stack with the top two values on the value stack.
        /// </summary>
        private void EvaluateTop(Stack<int> values, Stack<char> operators)
        {
            int right = values.Pop();
            int left = values.Pop();
            char op = operators.Pop();

            if (op == '+')
            {
                values.Push(left + right);
            }
            else if (op == '*')
            {
                values.Push(left * right);
            }
        }

        /// <summary>
        /// Returns the precedence of an operator.
        /// Higher numbers indicate higher precedence.
        /// </summary>
        private int Precedence(char op)
        {
            return op == '+' ? 1 : (op == '*' ? 2 : 0);
        }

        /// <summary>
        /// Example usage of the BottomParser from the Main method.
        /// </summary>
        public static void RunExample()
        {
            var parser = new BottomParser();

            // Example input: "3 + 5 * (2 + 4)"
            string input = "3 + 5";
            Console.WriteLine($"Input: {input}");

            // Parse and evaluate the expression
            int result = parser.Parse(input);
            Console.WriteLine($"Result: {result}");
        }
    }
}