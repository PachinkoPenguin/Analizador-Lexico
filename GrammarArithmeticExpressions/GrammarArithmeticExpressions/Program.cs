using System;  
using System.Collections.Generic;  
  
class Program  
{  
    static void Main(string[] args)  
    {  
        // Prompt the user to enter an arithmetic expression  
        Console.WriteLine("Enter an arithmetic expression (e.g., 3 + 4 * 2):");  
        string input = Console.ReadLine();  
  
        try  
        {  
            // Evaluate the expression and store the result  
            double result = EvaluateExpression(input);  
            // Output the result of the evaluated expression  
            Console.WriteLine($"The result of the expression '{input}' is: {result}");  
        }  
        catch (Exception ex)  
        {  
            // Handle any errors that occur during evaluation  
            Console.WriteLine($"Error: {ex.Message}");  
        }  
    }  
  
    // Method to evaluate the entire expression  
    static double EvaluateExpression(string expression)  
    {  
        // Tokenize the input expression into manageable parts  
        Queue<string> tokens = Tokenize(expression);  
        // Parse the expression and compute the result  
        double result = ParseExpression(tokens);  
        return result; // Return the final result  
    }  
  
    // Method to tokenize the input expression into numbers and operators  
    static Queue<string> Tokenize(string expression)  
    {  
        Queue<string> tokens = new Queue<string>(); // Queue to hold tokens  
        string number = ""; // Temporary string to build multi-digit numbers  
  
        // Iterate through each character in the expression  
        foreach (char c in expression)  
        {  
            if (char.IsDigit(c))  
            {  
                number += c; // Build the number character by character  
            }  
            else if (c == '+' || c == '*' || c == '(' || c == ')')  
            {  
                // If a number has been built, enqueue it  
                if (number.Length > 0)  
                {  
                    tokens.Enqueue(number);  
                    number = ""; // Reset the number for the next token  
                }  
                // Enqueue the operator  
                tokens.Enqueue(c.ToString());  
            }  
            else if (char.IsWhiteSpace(c))  
            {  
                // Ignore whitespace characters  
                if (number.Length > 0)  
                {  
                    tokens.Enqueue(number); // Enqueue the last built number  
                    number = ""; // Reset for the next token  
                }  
            }  
        }  
  
        // If there's a number left at the end, enqueue it  
        if (number.Length > 0)  
        {  
            tokens.Enqueue(number);  
        }  
  
        return tokens; // Return the queue of tokens  
    }  
  
    // Method to parse and evaluate expressions  
    static double ParseExpression(Queue<string> tokens)  
    {  
        // Parse the first term of the expression  
        double left = ParseTerm(tokens);  
        // Print the derivation step  
        Console.WriteLine($"E → T");  
        Console.WriteLine($"T = {left}");  
  
        // Check for additional terms connected by the '+' operator  
        while (tokens.Count > 0)  
        {  
            string op = tokens.Peek(); // Look at the next token  
            if (op == "+")  
            {  
                tokens.Dequeue(); // Consume the '+' operator  
                double right = ParseTerm(tokens); // Parse the next term  
                // Print the derivation step for addition  
                Console.WriteLine($"E → E + T");  
                Console.WriteLine($"E = {left} + {right}");  
                left += right; // Perform the addition  
            }  
            else  
            {  
                break; // No more addition operations  
            }  
        }  
  
        return left; // Return the evaluated result for this expression  
    }  
  
    // Method to parse and evaluate terms in the expression  
        // Method to parse and evaluate terms in the expression  
    static double ParseTerm(Queue<string> tokens)  
    {  
        // Parse the first factor of the term  
        double left = ParseFactor(tokens);  
        // Print the derivation step  
        Console.WriteLine($"T → F");  
        Console.WriteLine($"F = {left}");  
  
        // Check for additional factors connected by the '*' operator  
        while (tokens.Count > 0)  
        {  
            string op = tokens.Peek(); // Look at the next token  
            if (op == "*")  
            {  
                tokens.Dequeue(); // Consume the '*' operator  
                double right = ParseFactor(tokens); // Parse the next factor  
                // Print the derivation step for multiplication  
                Console.WriteLine($"T → T * F");  
                Console.WriteLine($"T = {left} * {right}");  
                left *= right; // Perform the multiplication  
            }  
            else  
            {  
                break; // No more multiplication operations  
            }  
        }  
  
        return left; // Return the evaluated result for this term  
    }  
  
    // Method to parse and evaluate factors in the expression  
    static double ParseFactor(Queue<string> tokens)  
    {  
        // Dequeue the next token from the queue  
        string token = tokens.Dequeue();  
        // Try to parse the token as a double  
        if (double.TryParse(token, out double value))  
        {  
            // If successful, print the derivation step  
            Console.WriteLine($"F → n");  
            Console.WriteLine($"n = {value}");  
            return value; // Return the numeric value  
        }  
        // If the token is not a valid number, throw an exception  
        throw new Exception("Invalid expression");  
    }  
}  