// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
//   Copyright 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> [Wyatt Young] </authors>
// <date> [August 21, 2024] </date>

namespace CS3500.FormulaTests;

using CS3500.Formula; // Change this using statement to use different formula implementations.

/// <summary>
///   <para>
///     The following class shows the basics of how to use the MSTest framework,
///     including:
///   </para>
///   <list type="number">
///     <item> How to catch exceptions. </item>
///     <item> How a test of valid code should look. </item>
///   </list>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    // --- Tests for One Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    ///   <remarks>
    ///     <list type="bullet">
    ///       <item>
    ///         We use the _ (discard) notation because the formula object
    ///         is not used after that point in the method.  Note: you can also
    ///         use _ wh
    ///         string.Empty is often considered best practice (rather than using "") because it
    ///         is explicen a method must match an interface but does not use
    ///         some of the required arguments to that method.
    ///       </item>
    ///       <item>it in intent (e.g., perhaps the coder forgot to but something in "").
    ///       </item>
    ///       <item>
    ///         The name of a test method should follow the MS standard:
    ///         https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
    ///       </item>
    ///       <item>
    ///         All methods should be documented, but perhaps not to the same extent
    ///         as this one.  The remarks here are for your educational
    ///         purposes (i.e., a developer would assume another developer would know these
    ///         items) and would be superfluous in your code.
    ///       </item>
    ///       <item>
    ///         Notice the use of the attribute tag [ExpectedException] which tells the test
    ///         that the code should throw an exception, and if it doesn't an error has occurred;
    ///         i.e., the correct implementation of the constructor should result
    ///         in this exception being thrown based on the given poorly formed formula.
    ///       </item>
    ///     </list>
    ///   </remarks>
    ///   <example>
    ///     <code>
    ///        // here is how we call the formula constructor with a string representing the formula
    ///        _ = new Formula( "5+5" );
    ///     </code>
    ///   </example>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNoTokens_Invalid()
    {
        _ = new Formula(string.Empty);  // note: it is arguable that you should replace "" with string.Empty for readability and clarity of intent (e.g., not a cut and paste error or a "I forgot to put something there" error).
    }

    // Tests for invalid character tokens
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExclamationPointToken_Invalid()
    {
        _ = new Formula("1 ! 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestAtToken_Invalid()
    {
        _ = new Formula("1 @ 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestPoundToken_Invalid()
    {
        _ = new Formula("1 # 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestDollarToken_Invalid()
    {
        _ = new Formula("1 $ 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestPercentToken_Invalid()
    {
        _ = new Formula("1 % 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestCarrotToken_Invalid()
    {
        _ = new Formula("1 ^ 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestAmpersandToken_Invalid()
    {
        _ = new Formula("1 & 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestUnderscoreToken_Invalid()
    {
        _ = new Formula("1 _ 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestEqualsToken_Invalid()
    {
        _ = new Formula("1 = 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBracesToken_Invalid()
    {
        _ = new Formula("{ 1 }");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBracketsToken_Invalid()
    {
        _ = new Formula("[ 1 ]");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSlashToken_Invalid()
    {
        _ = new Formula("1 | 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestColonToken_Invalid()
    {
        _ = new Formula("1 : 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSemicolonToken_Invalid()
    {
        _ = new Formula("1 ; 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestQuoteToken_Invalid()
    {
        _ = new Formula("1 ' 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLeftAngleToken_Invalid()
    {
        _ = new Formula("1 < 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestRightAngleToken_Invalid()
    {
        _ = new Formula("1 > 1");
    }

    // --- Tests for Valid Token Rule ---

    // Tests for single invalid tokens

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSingleTokenSingleLetterLowerToken_Invalid()
    {
        _ = new Formula("c");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokensSingleLetterUpperToken_Invalid()
    {
        _ = new Formula("Z");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSingleTokenSingleLetterAfterNumberToken_Invalid()
    {
        _ = new Formula("3A");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSingleTokenNumberBetweenLettersToken_Invalid()
    {
        _ = new Formula("A2B");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSingleTokensEmptyParenthesisToken_Invalid()
    {
        _ = new Formula("()");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSingleTokenSingleLetterBeforeNumberSpaceToken_Invalid()
    {
        _ = new Formula("A 3");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSingleTokenSingleNumberBeforeLetterToken_Invalid()
    {
        _ = new Formula("2a");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSingleTokenNumberNegativeToken_Invalid()
    {
        _ = new Formula("-2");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSingleTokenNumberNegativeDecimalToken_Invalid()
    {
        _ = new Formula("-2.40");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSingleTokenNumberNegativeScientificUpperEToken_Invalid()
    {
        _ = new Formula("-2E9");
    }

    // Tests for single valid tokens

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenSingleNumberToken_Valid()
    {
        _ = new Formula("3");
    }

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenSingleNumberWithDecimalToken_Valid()
    {
        _ = new Formula("23.");
    }

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenVariableOneLetterOneNumberToken_Valid()
    {
        _ = new Formula("a1");
    }

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenVariableTwoLetterOneNumberToken_Valid()
    {
        _ = new Formula("aZ1");
    }

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenVariableOneLetterThreeNumberToken_Valid()
    {
        _ = new Formula("V123");
    }

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenVariableThreeLetterThreeNumberToken_Valid()
    {
        _ = new Formula("Tes123");
    }

    [TestMethod]
    public void FormulaConstructor_TestOperatorTokensBackslashToken_Valid()
    {
        _ = new Formula("2 / 0");
    }

    [TestMethod]
    public void FormulaConstructor_TestOperatorTokensAsteriskToken_Valid()
    {
        _ = new Formula("2 * 0");
    }

    [TestMethod]
    public void FormulaConstructor_TestOperatorTokensPlusToken_Valid()
    {
        _ = new Formula("2 + 2");
    }

    [TestMethod]
    public void FormulaConstructor_TestOperatorTokensMinusToken_Valid()
    {
        _ = new Formula("2 - 2");
    }

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenNumberDecimalToken_Valid()
    {
        _ = new Formula("3.14");
    }

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenNumberScientificLowEToken_Valid()
    {
        _ = new Formula("6e7");
    }

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenNumberScientificUpperEToken_Valid()
    {
        _ = new Formula("2E7");
    }

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenNumberScientificDecimalToken_Valid()
    {
        _ = new Formula("3.14E8");
    }

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenNumberScientificDecimalUpperEToken_Valid()
    {
        _ = new Formula("0.147869E12");
    }

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenNumberScientificNegativeToken_Valid()
    {
        _ = new Formula("3E-7");
    }

    [TestMethod]
    public void FormulaConstructor_TestSingleTokenNumberScientificDecimalNegativeUpperEToken_Valid()
    {
        _ = new Formula("3.067E-7");
    }

    // --- Tests for Closing Parenthesis Rule

    // invalid Closing Parenthesis Rule

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisRuleSingleClosingParenthesis_Invalid()
    {
        _ = new Formula(")");
    }

    // valid Closing Parenthesis Rule

    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisRuleOneOpeningOneClosingParenthesis_Valid()
    {
        _ = new Formula("(0)");
    }

    // --- Tests for Balanced Parentheses Rule

    // invalid Balanced Parenthesis Rule
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBalancedParenthesisRuleOneOpeningTwoClosingParenthesis_Invalid()
    {
        _ = new Formula("(1))");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBalancedParenthesisRuleOneOpeningInMiddle_Invalid()
    {
        _ = new Formula("21 + (2*3+9");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBalancedParenthesisRuleOneOpeningTwoClosingInMiddle_Invalid()
    {
        _ = new Formula("21 + (2*3))+9");
    }

    // valid Balanced Parenthesis Rule

    [TestMethod]
    public void FormulaConstructor_TestBalancedParenthesisOneOpeningOneClosingParenthesisExpression_Valid()
    {
        _ = new Formula("(1+1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestBalancedParenthesisTwoOpeningTwoClosingParenthesisExpression_Valid()
    {
        _ = new Formula("((1+8)*2)");
    }

    // --- Tests for First Token Rule

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenRuleNumber_Valid()
    {
        _ = new Formula("1+1");
    }

    // valid First Token Rule continued

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenRuleVariable_Valid()
    {
        _ = new Formula("b6 + (3 - 2)");
    }

    // invalid First Token Rule 

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenRuleOperator_Invalid()
    {
        _ = new Formula("+ 3");
    }

    // --- Tests for  Last Token Rule ---

    // valid Last Token Rule

    [TestMethod]
    public void FormulaConstructor_TestLastTokenRuleVariable_Valid()
    {
        _ = new Formula("3 - B9");
    }

    // invalid Last Token Rule

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenRuleOperator_Invalid()
    {
        _ = new Formula("2 *");
    }

    // --- Tests for Parentheses/Operator Following Rule ---

    // valid Parenthesis/Operator Following Rule

    [TestMethod]
    public void FormulaConstructor_TestParenthesisOperatorFollowingRuleParenthesisVariable_Valid()
    {
        _ = new Formula("(B7)");
    }

    [TestMethod]
    public void FormulaConstructor_TestParenthesisOperatorFollowingRuleOperatorToParenthesis_Valid()
    {
        _ = new Formula("2+(2)");
    }

    // invalid Parenthesis/Operator Following Rule

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestParenthesisOperatorFollowingRuleParenthesisOperator_Invalid()
    {
        _ = new Formula("2 + (/3)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestParenthesisOperatorFollowingRuleOperatorToOperator_Invalid()
    {
        _ = new Formula("2+*3");
    }

    // --- Tests for Extra Following Rule ---

    // valid Extra Following Rule

    [TestMethod]
    public void FormulaConstructor_TestExtraFollowingRuleNumberToOperator_Valid()
    {
        _ = new Formula("2/4");
    }

    [TestMethod]
    public void FormulaConstructor_TestExtraFollowingRuleClosingParenthesisToOperator_Valid()
    {
        _ = new Formula("(e2)+4");
    }


    // invalid Extra Following Rule 

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingRuleNumberToParenthesis_Invalid()
    {
        _ = new Formula("2(3)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingRuleParenthesisToNumber_Invalid()
    {
        _ = new Formula("(3)9");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingRuleVariableToParenthesis_Invalid()
    {
        _ = new Formula("z27(3)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingRuleClosingParenthesisToParenthesis_Invalid()
    {
        _ = new Formula("(7)(3)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingRuleNumberAfterNumberNoOperator_Invalid()
    {
        _ = new Formula("1 12 + 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingRuleVariableAfterNumberNoOperator_Invalid()
    {
        _ = new Formula("1 a1 + 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingRuleVariableAfterVariable_Invalid()
    {
        _ = new Formula("Z1 a1 + 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingRuleClosingParenthesisAfterOperator_Invalid()
    {
        _ = new Formula("(1 + )");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingRuleVariableBeforeNumberNoOperator_Invalid()
    {
        _ = new Formula("A1 1 + 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingRuleVariableAfterClosingParenthesis_Invalid()
    {
        _ = new Formula("(1+1)a1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestExtraFollowingRuleOpenParenthesisAfterExpression_Invalid()
    {
        _ = new Formula("(1+1)()");
    }

    // --- Tests for ToString Method ---

    [TestMethod]
    public void FormulaToString_UniqueVariablesWithSpaces_Valid() 
    {
        Formula formula = new Formula("x1 + y1");
        Assert.AreEqual("X1+Y1", formula.ToString());
    }

    [TestMethod]
    public void FormulaToString_UniqueVariablesDecimalNumber_Valid()
    {
        Formula formula = new Formula("Ab1 + y23 - 67.120000");
        Assert.AreEqual("AB1+Y23-67.12", formula.ToString());
    }

    [TestMethod]
    public void FormulaToString_ParenthesisScientificNotation_Valid()
    {
        Formula formula = new Formula("((B1* 2) - 6.32E3)");
        Assert.AreEqual("((B1*2)-6320)", formula.ToString());
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaToString_Empty_Invalid()
    {
        Formula formula = new Formula(" ");
        Assert.AreNotEqual(" ", formula.ToString());
    }

    // --- Tests for GetVariables() Method ---

    [TestMethod]
    public void FormulaGetVariables_UniqueVariablesNumbers_ValidCount() 
    {
        Formula formula = new Formula("x2 + BY4 /(12 * 2)");
        ISet<string> vars = formula.GetVariables();
        Assert.AreEqual(2, vars.Count);
    }

    [TestMethod]
    public void FormulaGetVariables_UniqueVariablesNumbers_Valid()
    {
        Formula formula = new Formula("x2 + BY4 /(12 * 2)");
        ISet<string> vars = formula.GetVariables();
        CollectionAssert.AreEquivalent(new HashSet<string> {"X2", "BY4"}.ToList(), vars.ToList());
    }

    [TestMethod]
    public void FormulaGetVariables_EmptySet_ValidCount()
    {
        Formula formula = new Formula("21 + 2E7 *(4-2)");
        ISet<string> vars = formula.GetVariables();
        Assert.AreEqual(0, vars.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokenOther_Invalid()
    {
        _ = new Formula("1 , 2");
    }


}