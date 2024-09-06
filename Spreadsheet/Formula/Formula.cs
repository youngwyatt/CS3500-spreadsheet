// <copyright file="Formula_PS2.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
//   <para>
//     This code is provides to start your assignment.  It was written
//     by Profs Joe, Danny, and Jim.  You should keep this attribution
//     at the top of your code where you have your header comment, along
//     with the other required information.
//   </para>
//   <para>
//     You should remove/add/adjust comments in your file as appropriate
//     to represent your work and any changes you make.
//   </para>
// </summary>


namespace CS3500.Formula;

using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Text;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one ore more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///     and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///   </para>
///   <para>
///     For Assignment Two, you are to implement the following functionality:
///   </para>
///   <list type="bullet">
///     <item>
///        Formula Constructor which checks the syntax of a formula.
///     </item>
///     <item>
///        Get Variables
///     </item>
///     <item>
///        ToString
///     </item>
///   </list>
/// </summary>
public class Formula
{
    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    /// <summary>
    ///   Numbers can be in any format, including scientific notation using 
    ///   e or E. Negative numbers not included. This pattern represents all 
    ///   valid numbers.
    /// </summary>
    private const string AnyNumberRegExPattern = @"\d*\.?\d+([eE][-+]?\d+)?";

    /// <summary>
    ///   Accepted operators are +, *, /, and -. This pattern represents the 
    ///   valid operators.
    /// </summary>
    private const string OperatorRegExPattern = "[+-/*]";

    ///<summary>
    ///  Private member variable for holding the parsed tokens to avoid 
    ///  recalling GetTokens()
    ///</summary>
    private List<string> allTokens = new List<string>();

    /// <summary>
    ///   String for canonical form of formula returned by ToString() method.
    ///   Built in constructor.
    /// </summary>
    private string canonicalFormula;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     Non Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        // parse formula into List of strings of tokens
        List<string> tokens = GetTokens(formula);
        allTokens = tokens;
        string? prevToken = null;
        int openingCount = 0;
        int closingCount = 0;
        // one token rule
        if (tokens.Count <= 0)
        {
            throw new FormulaFormatException("Expression Empty");
        }

        StringBuilder canonicalForm = new StringBuilder();

        foreach (string token in tokens)
        {
            // first pass check if token is valid 
            if (!IsVar(token) && !IsOper(token) && !IsNum(token) && token != "(" && token != ")")
            {
                throw new FormulaFormatException(token.ToString());
            }
            // first token rule
            if (prevToken == null && !IsVar(token) && !IsNum(token) && token != "(")
            {
                throw new FormulaFormatException("Invalid first token: " + token.ToString());
            }
            if (token == "(")
            {
                openingCount++;
            }
            else if (token == ")")
            {
                closingCount++;
            }

            // check closing parenthesis rule
            if (closingCount > openingCount)
            {
                throw new FormulaFormatException("Number of closing parenthesis is greater then number of opening parenthesis.");
            }

            // check parenthesis/operator following rule
            if (prevToken != null && (prevToken == "(" || IsOper(prevToken)) && !(IsNum(token) || IsVar(token) || token == "("))
            {
                throw new FormulaFormatException("Token following an opening parenthesis or an operator must be a number, variable, or opening parenthesis");
            }

            // check extra following rule
            if (prevToken != null && (IsVar(prevToken) || IsNum(prevToken) || prevToken == ")") && !(IsOper(token) || token == ")"))
            {
                throw new FormulaFormatException("Invalid token following a number, variable, or closing parenthesis: " + token);
            }

            // build the canonical form
            if (IsNum(token))
            {
                // use double.ToString for numbers
                double temp = double.Parse(token);
                canonicalForm.Append(temp.ToString()); 
            }
            else if (IsVar(token))
            {
                // normalize variables to uppercase
                canonicalForm.Append(token.ToUpper()); 
            }
            else
            {
                // parenthesis/operators
                canonicalForm.Append(token);
            }

            prevToken = token;
        }
        // check balanced parenthesis rule 
        if (openingCount != closingCount)
        {
            throw new FormulaFormatException("Parenthesis unbalanced");
        }
        // check last token rule
        if (prevToken != ")" && prevToken != null && !IsVar(prevToken) && !IsNum(prevToken)) 
        {
            throw new FormulaFormatException("Invalid last token: " + prevToken.ToString());
        }
        // store canonical form
        canonicalFormula = canonicalForm.ToString();
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
    ///   </remarks>
    ///   <para>
    ///     For example, if N is a method that converts all the letters in a string to upper case:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should enumerate "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should enumerate "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        HashSet<string> returnVars = new HashSet<string>();
        // loop through each token and store variables
        foreach (string token in allTokens) 
        {
            // if token is variable, normalize to uppercase then add to HashSet
            if (IsVar(token)) 
            {
                returnVars.Add(token.ToUpper());
            }
        }
        return returnVars;
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f 
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All of the variables in the string will be normalized.  This
    ///     means capital letters.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + y1").ToString() should return "X1+Y1"
    ///       new("X1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This code should execute in O(1) time.
    ///   <para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString()
    {
        return canonicalFormula;
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///   Reports whether "token" is an operator.  It must be one of +,/,-,*
    /// </summary>
    /// <param name="token"> A token that may be an operator. </param>
    /// <returns> true if the string matches the requirements, e.g., + or *. </returns>
    private static bool IsOper(string token)
    {
        string standaloneOperatorPattern = $"^{OperatorRegExPattern}";
        return Regex.IsMatch(token, standaloneOperatorPattern);
    }

    /// <summary>
    ///   Reports whether "token" is a valid number. Accepts scientific notation with e or E and negative
    ///   exponents. Does not accept negative numbers.
    /// </summary>
    /// <param name="token"> A token that may be a number. </param>
    /// <returns> true if the string matches the requirements, e.g., 350.45 or 2.3E-6. </returns>
    private static bool IsNum(string token)
    {
        string standaloneNumberPattern = $"^{AnyNumberRegExPattern}";
        return Regex.IsMatch(token, standaloneNumberPattern);
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results = [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
                                        "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern,
                                        rpPattern,
                                        opPattern,
                                        VariableRegExPattern,
                                        doublePattern,
                                        spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                results.Add(s);
            }
        }

        return results;
    }
}


/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
        // All this does is call the base constructor. No extra code needed.
    }
}
