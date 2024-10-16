namespace SpreadsheetTests;
using CS3500.Spreadsheet;
using CS3500.Formula;
/// <summary>
/// This is a test class for the Spreadsheet class and is intended
/// to contain all SpreadsheetTest Tests
/// </summary>
/// <authors> Wyatt Young </authors>
/// <date> September 21st, 2024 </date>
[TestClass]
public class SpreadsheetTests
{
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void Spreadsheet_InvalidVar_InvalidNameException() 
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("1a", "invalid name");
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void Spreadsheet_InvalidVar2_InvalidNameException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("1a2", "invalid name");
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void Spreadsheet_InvalidVar3_InvalidNameException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("abc", "invalid name");
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void Spreadsheet_InvalidVar4_InvalidNameException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("123", "invalid name");
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void Spreadsheet_InvalidName_InvalidNameException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell(null, "invalid name");
    }
    [TestMethod]
    public void Spreadsheet_TwoCharsTwoNums_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("ab12", "valid name");
    }

    // --- GetNamesOfAllEmptyCells Tests ---

    [TestMethod]
    public void GetNamesEmptyCells_SimpleCells_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "5");
        ss.SetContentsOfCell("b1", "test");
        // get names of non empty cells
        ISet<string> popCells = ss.GetNamesOfAllNonemptyCells();
        Assert.IsTrue(popCells.Contains("A1"));
        Assert.IsTrue(popCells.Contains("B1"));
        Assert.AreEqual(2, popCells.Count);
    }
    [TestMethod]
    public void GetNamesEmptyCells_ClearedCell_NotReturned()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "3.0");
        ss.SetContentsOfCell("b1", "=C1*23");
        ss.SetContentsOfCell("d1", string.Empty);
        // ensure non empty cells are a1 and b1
        List<string> nonempty = ss.GetNamesOfAllNonemptyCells().ToList();
        CollectionAssert.AreEquivalent(new List<string> { "A1", "B1" }, nonempty);
        ss.SetContentsOfCell("b1", string.Empty);
        List<string> nonempty2 = ss.GetNamesOfAllNonemptyCells().ToList();
        CollectionAssert.AreEquivalent(new List<string> { "A1" }, nonempty2);
    }

    // --- GetCellContents Tests ---

    [TestMethod]
    public void GetCell_EmptyCell_ReturnsEmptyString()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", string.Empty);
        Assert.AreEqual(string.Empty, ss.GetCellContents("a1"));
        // repeat for "" instead of .Empty
        ss.SetContentsOfCell("b1", "");
        Assert.AreEqual(string.Empty, ss.GetCellContents("b1"));
    }
    [TestMethod]
    public void GetCell_ReturnsEmptyString_WhenCellCleared()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "test");
        ss.SetContentsOfCell("A1", string.Empty);
        Assert.AreEqual(string.Empty, ss.GetCellContents("A1"));
    }
    [TestMethod]
    public void GetCellContents_SimpleCells_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "5");
        ss.SetContentsOfCell("b1", "test");
        Assert.AreEqual(5.0, ss.GetCellContents("a1"));
        Assert.AreEqual("test", ss.GetCellContents("b1"));
    }
    [TestMethod]
    public void GetCellContents_EmptyCell_ReturnsEmptyString()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "2.0");
        Assert.AreEqual(string.Empty, ss.GetCellContents("b1"));
    }
    [TestMethod]
    public void GetCellContents_ClearedCell_ReturnsEmptyString()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "Some text");
        Assert.AreEqual("Some text", ss.GetCellContents("A1"));
        ss.SetContentsOfCell("A1", string.Empty);
        Assert.AreEqual(string.Empty, ss.GetCellContents("A1"));
    }

    // --- SetContentsOfCell Tests ---

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void SetContentsOfCell_ContentIsJustEquals_FormulaFormatException() 
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "=");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void SetContentsOfCell_FormulaWithLeadingWhitespace_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "= 5 + 3"); 
    }
    [TestMethod]
    public void SetContentsOfCell_SimpleNumberStringFormula_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        IList<string> num = ss.SetContentsOfCell("a1", "5");
        IList<string> str = ss.SetContentsOfCell("b2", "Test");
        IList<string> formula = ss.SetContentsOfCell("c3", "=12 - 2");
        Assert.IsTrue(num.Contains("A1"));
        Assert.AreEqual(5.0, ss.GetCellContents("a1"));
        Assert.IsTrue(str.Contains("B2"));
        Assert.AreEqual("Test", ss.GetCellContents("b2"));
        Assert.IsTrue(formula.Contains("C3"));
        Assert.AreEqual(new Formula("12-2"), ss.GetCellContents("c3"));
    }
    [TestMethod]
    public void SetContentsOfCell_FormulaDirectDependents_ValidDependentsOrder()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "5");
        ss.SetContentsOfCell("b1", "=A1 * 2");
        ss.SetContentsOfCell("c1", "=A1 + B1");
        ss.SetContentsOfCell("d1", "15");
        IList<string> toRecalculate = ss.SetContentsOfCell("a1", "10");
        // changing a1 causes recalculation of b1 and c1
        CollectionAssert.AreEqual(new List<string> { "A1", "B1", "C1" }, toRecalculate.ToList());
    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_SelfReference_ThrowsCircularException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=A1 + 1");
    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_CircularDependency_ThrowsCircularException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "=b1 + 1");
        ss.SetContentsOfCell("b1", "=c1 + 1");
        ss.SetContentsOfCell("c1", "=a1 + 1");
        // changing a1 to depend on c1 causes circular dependency
        ss.SetContentsOfCell("a1", "=c1 + 1");
    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_ValidThenCircularDependency_ThrowsCircularException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=B1 + 1");
        ss.SetContentsOfCell("B1", "=C1 + 1");
        ss.SetContentsOfCell("C1", "5");

        // circular dependency by making C1 depend on A1
        ss.SetContentsOfCell("C1", "=A1 + 1");
    }
    [TestMethod]
    public void SetContentsOfCell_IndirectDependents_ValidDependentsOrder()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "=b1 + 1");
        ss.SetContentsOfCell("b1", "=c1 + 1");
        ss.SetContentsOfCell("c1", "5");
        IList<string> toRecalculate = ss.SetContentsOfCell("c1", "10");
        // changing c1 should result in proper recalculate list
        CollectionAssert.AreEqual(new List<string> { "C1", "B1", "A1" }, toRecalculate.ToList());
    }
    [TestMethod]
    public void SetContentsOfCell_ComplexIndirectDependents_ValidDependentsOrder()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "=b1 + c1");
        ss.SetContentsOfCell("b1", "=d1 * e1");
        ss.SetContentsOfCell("c1", "=f1 + 2");
        ss.SetContentsOfCell("d1", "=g1 + 3");
        ss.SetContentsOfCell("e1", "10");
        ss.SetContentsOfCell("f1", "5");
        ss.SetContentsOfCell("g1", "2");
        // changing G1 should affect D1, which affects B1 and A1.
        IList<string> toRecalculate = ss.SetContentsOfCell("g1", "4");
        CollectionAssert.AreEqual(new List<string> { "G1", "D1", "B1", "A1" }, toRecalculate.ToList());
    }
    [TestMethod]
    public void SetContentsOfCell_AddVsReplaceDependency_FailAddPassReplace()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "=b1+c1");
        IList<string> toRecalc = ss.SetContentsOfCell("b1", "5");
        // check dependents of a1
        Assert.IsTrue(toRecalc.Contains("A1"));
        IList<string> toRecalc2 = ss.SetContentsOfCell("c1", "10");
        Assert.IsTrue(toRecalc2.Contains("A1"));
        // now make a1 depend on d and e
        ss.SetContentsOfCell("a1", "=d1 * e1");
        // check if b1 and c1 dependents have been updated
        IList<string> toRecalc3 = ss.SetContentsOfCell("b1", "10");
        // check dependents of a1
        Assert.IsFalse(toRecalc3.Contains("A1"));
        IList<string> toRecalc4 = ss.SetContentsOfCell("c1", "5");
        Assert.IsFalse(toRecalc4.Contains("A1"));
        // ensure d and e have a as dependent
        IList<string> toRecalc5 = ss.SetContentsOfCell("d1", "10");
        Assert.IsTrue(toRecalc5.Contains("A1"));
        IList<string> toRecalc6 = ss.SetContentsOfCell("e1", "10");
        Assert.IsTrue(toRecalc6.Contains("A1"));
    }
    [TestMethod]
    public void SetContentsOfCell_EmptyString_RemovesCellAndDependencies()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=B1 + C1");
        ss.SetContentsOfCell("B1", "5");
        ss.SetContentsOfCell("C1", "10");
        // verify that A1 is a non-empty cell and depends on B1 and C1
        ISet<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
        Assert.IsTrue(nonEmptyCells.Contains("A1"));
        Assert.IsTrue(nonEmptyCells.Contains("B1"));
        Assert.IsTrue(nonEmptyCells.Contains("C1"));
        // set A1 to an empty string, which should remove it
        ss.SetContentsOfCell("A1", string.Empty);
        // check for removal
        nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
        Assert.IsFalse(nonEmptyCells.Contains("A1"));
        // check A1 has no more dependencies
        IList<string> toRecalc = ss.SetContentsOfCell("B1", "20");
        Assert.IsFalse(toRecalc.Contains("A1"));
    }
    [TestMethod]
    public void SetContentsOfCell_FormulaWithEmptyCell_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=B1 + 2");
        // A1 should still be treated as a valid formula even though B1 is not yet defined
        object contents = ss.GetCellContents("A1");
        Assert.IsInstanceOfType(contents, typeof(Formula));
        Assert.IsTrue(ss.GetNamesOfAllNonemptyCells().Contains("A1"));
    }
    [TestMethod]
    public void SetContentsOfCell_FormulaToNumber_UpdatesDependencies()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=B1 + C1");
        ss.SetContentsOfCell("B1", "5");
        ss.SetContentsOfCell("C1", "3");
        ss.SetContentsOfCell("Z1", "=Y1 / X1");
        ss.SetContentsOfCell("Y1", "2");
        ss.SetContentsOfCell("X1", "3.4");
        IList<string> toRecalculate = ss.SetContentsOfCell("B1", "10");
        Assert.IsTrue(toRecalculate.Contains("A1"));
        toRecalculate = ss.SetContentsOfCell("A1", "5");
        // ensure A1 is NOT recalculated after changing b1
        toRecalculate = ss.SetContentsOfCell("B1", "7");
        Assert.IsFalse(toRecalculate.Contains("A1"));
        // ensure same for text set as well
        ss.SetContentsOfCell("Z1", "test");
        IList<string> toRecalculate2 = ss.SetContentsOfCell("X1", "4");
        Assert.IsFalse(toRecalculate2.Contains("Z1"));
    }
    /// <summary>
    /// This is a test designed to fail if a circular exception is thrown in Set method but the graph and backing dictionary
    /// already stored the invalid formula.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_CircularDependency_ProperlyRestoresOldDependencies()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=B1 + C1");
        ss.SetContentsOfCell("B1", "=C1 * 2");
        ss.SetContentsOfCell("C1", "3.0");
        try
        {
            ss.SetContentsOfCell("C1", "=A1 + 1");
        }
        catch (CircularException)
        {
            IList<string> toRecalculate = ss.SetContentsOfCell("B1", "5.0");
            Assert.IsTrue(toRecalculate.Contains("A1")); 
            Assert.IsFalse(toRecalculate.Contains("C1"));
            Assert.AreEqual(ss.GetCellContents("C1"), 3.0);
            throw; 
        }
    }

    /// --- GetCellValue Tests ---

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetCellValue_UnknownFormulaValue_ArgumentException() 
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("a1", "=b1+3");
        ss.GetCellValue("A1");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaError))]
    public void GetCellValue_ImproperFormula_FormulaError()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("a1", "=((b1+3) / 2");
        ss.SetContentsOfCell("b1", "2.0");
        ss.GetCellValue("A1");
    }

    /// --- LookupVar Tests ---

    [TestMethod]
    public void LookupVar_SimpleDoubleVal_Is10()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("c1", "2");
        ss.SetContentsOfCell("b1", "=C1*2");
        Assert.AreEqual(10.0, ss.GetCellValue("B1"));
    }
    [TestMethod]
    public void LookupVar_SimpleExpressionVal_Is10()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("c1", "=3 + 2");
        ss.SetContentsOfCell("b1", "=C1*2");
        Assert.AreEqual(10.0, ss.GetCellValue("B1"));
    }
    [TestMethod]
    public void LookupVar_SimpleDependency_Is10() 
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("c1", "2");
        ss.SetContentsOfCell("b1", "=C1*4");
        ss.SetContentsOfCell("a1", "=B1+2");
        Assert.AreEqual(10.0, ss.GetCellValue("A1"));
    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void LookupVar_UndefinedVar_ArgumentException() 
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("a1", "=b1*2");
        ss.GetCellValue("A1");
    }
    //// EMPTY SPREADSHEETS
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("2")]
    //[ExpectedException(typeof(InvalidNameException))]
    //public void GetCellContents_InvalidName_Throws()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.GetCellContents("1AA");
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("3")]
    //public void GetCellContents_EmptyCell_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    Assert.AreEqual("", s.GetCellContents("A2"));
    //}

    //// SETTING CELL TO A DOUBLE
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("5")]
    //[ExpectedException(typeof(InvalidNameException))]
    //public void SetCellContents_InvalidNameDouble_Throws()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("1A1A", 1.5);
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("6")]
    //public void SetAndGet_Double_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("Z7", 1.5);
    //    Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
    //}

    //// SETTING CELL TO A STRING
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("9")]
    //[ExpectedException(typeof(InvalidNameException))]
    //public void SetCellContents_InvalidNameString_Throws()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("1AZ", "hello");
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("10")]
    //public void SetAndGet_String_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("Z7", "hello");
    //    Assert.AreEqual("hello", s.GetCellContents("Z7"));
    //}

    //// SETTING CELL TO A FORMULA
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("13")]
    //[ExpectedException(typeof(InvalidNameException))]
    //public void SetCellContents_InvalidNameFormula_Throws()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("1AZ", new Formula("2"));
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("14")]
    //public void SetAndGet_Formula_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("Z7", new Formula("3"));
    //    Formula f = (Formula)s.GetCellContents("Z7");
    //    Assert.AreEqual(new Formula("3"), f);
    //    Assert.AreNotEqual(new Formula("2"), f);
    //}

    //// CIRCULAR FORMULA DETECTION
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("15")]
    //[ExpectedException(typeof(CircularException))]
    //public void SetCellContents_Circular_Throws()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("A1", new Formula("A2"));
    //    s.SetCellContents("A2", new Formula("A1"));
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("16")]
    //[ExpectedException(typeof(CircularException))]
    //public void SetCellContents_IndirectCircular_Throws()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("A1", new Formula("A2+A3"));
    //    s.SetCellContents("A3", new Formula("A4+A5"));
    //    s.SetCellContents("A5", new Formula("A6+A7"));
    //    s.SetCellContents("A7", new Formula("A1+A1"));
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("17")]
    //[ExpectedException(typeof(CircularException))]
    //public void SetCellContents_Circular_UndoesCellChanges()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    try
    //    {
    //        s.SetCellContents("A1", new Formula("A2+A3"));
    //        s.SetCellContents("A2", 15);
    //        s.SetCellContents("A3", 30);
    //        s.SetCellContents("A2", new Formula("A3*A1"));
    //    }
    //    catch (CircularException)
    //    {
    //        Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
    //        throw; // C# shortcut to rethrow the same exception that was caught
    //    }
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("17b")]
    //[ExpectedException(typeof(CircularException))]
    //public void SetCellContents_Circular_UndoesGraphChanges()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    try
    //    {
    //        s.SetCellContents("A1", new Formula("A2"));
    //        s.SetCellContents("A2", new Formula("A1"));
    //    }
    //    catch (CircularException)
    //    {
    //        Assert.AreEqual("", s.GetCellContents("A2"));
    //        Assert.IsTrue(new HashSet<string> { "A1" }.SetEquals(s.GetNamesOfAllNonemptyCells()));
    //        throw; // C# shortcut to rethrow the same exception that was caught
    //    }
    //}

    //// NONEMPTY CELLS
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("18")]
    //public void GetNames_Empty_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("19")]
    //public void GetNames_ExplicitlySetEmpty_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("B1", "");
    //    Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("20")]
    //public void GetNames_NonemptyCellString_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("B1", "hello");
    //    Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("21")]
    //public void GetNames_NonemptyCellDouble_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("B1", 52.25);
    //    Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("22")]
    //public void GetNames_NonemptyCellFormula_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("B1", new Formula("3.5"));
    //    Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("23")]
    //public void GetNames_NonemptyCells_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("A1", 17.2);
    //    s.SetCellContents("C1", "hello");
    //    s.SetCellContents("B1", new Formula("3.5"));
    //    Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
    //}

    //// RETURN VALUE OF SET CELL CONTENTS
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("24")]
    //public void SetCellContents_Double_NoFalseDependencies()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("B1", "hello");
    //    s.SetCellContents("C1", new Formula("5"));
    //    Assert.IsTrue(s.SetCellContents("A1", 17.2).SequenceEqual(new List<string>() { "A1" }));
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("25")]
    //public void SetCellContents_String_NoFalseDependencies()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("A1", 17.2);
    //    s.SetCellContents("C1", new Formula("5"));
    //    Assert.IsTrue(s.SetCellContents("B1", "hello").SequenceEqual(new List<string>() { "B1" }));
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("26")]
    //public void SetCellContents_Formula_NoFalseDependencies()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("A1", 17.2);
    //    s.SetCellContents("B1", "hello");
    //    Assert.IsTrue(s.SetCellContents("C1", new Formula("5")).SequenceEqual(new List<string>() { "C1" }));
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("27")]
    //public void SetCellContents_ChainDependencies_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("A1", new Formula("A2+A3"));
    //    s.SetCellContents("A2", 6);
    //    s.SetCellContents("A3", new Formula("A2+A4"));
    //    s.SetCellContents("A4", new Formula("A2+A5"));
    //    Assert.IsTrue(s.SetCellContents("A5", 82.5).SequenceEqual(new List<string>() { "A5", "A4", "A3", "A1" }));
    //}

    //// CHANGING CELLS
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("28")]
    //public void SetCellContents_FormulaToDouble_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("A1", new Formula("A2+A3"));
    //    s.SetCellContents("A1", 2.5);
    //    Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("29")]
    //public void SetCellContents_FormulaToString_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("A1", new Formula("A2+A3"));
    //    s.SetCellContents("A1", "Hello");
    //    Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("30")]
    //public void SetCellContents_StringToFormula_Works()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("A1", "Hello");
    //    s.SetCellContents("A1", new Formula("23"));
    //    Assert.AreEqual(new Formula("23"), (Formula)s.GetCellContents("A1"));
    //    Assert.AreNotEqual(new Formula("24"), (Formula)s.GetCellContents("A1"));
    //}

    //// STRESS TESTS
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("31")]
    //public void TestStress1()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetCellContents("A1", new Formula("B1+B2"));
    //    s.SetCellContents("B1", new Formula("C1-C2"));
    //    s.SetCellContents("B2", new Formula("C3*C4"));
    //    s.SetCellContents("C1", new Formula("D1*D2"));
    //    s.SetCellContents("C2", new Formula("D3*D4"));
    //    s.SetCellContents("C3", new Formula("D5*D6"));
    //    s.SetCellContents("C4", new Formula("D7*D8"));
    //    s.SetCellContents("D1", new Formula("E1"));
    //    s.SetCellContents("D2", new Formula("E1"));
    //    s.SetCellContents("D3", new Formula("E1"));
    //    s.SetCellContents("D4", new Formula("E1"));
    //    s.SetCellContents("D5", new Formula("E1"));
    //    s.SetCellContents("D6", new Formula("E1"));
    //    s.SetCellContents("D7", new Formula("E1"));
    //    s.SetCellContents("D8", new Formula("E1"));
    //    IList<String> cells = s.SetCellContents("E1", 0);
    //    Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
    //}

    //// Repeated for extra weight
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("32")]
    //public void TestStress1a()
    //{
    //    TestStress1();
    //}
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("33")]
    //public void TestStress1b()
    //{
    //    TestStress1();
    //}
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("34")]
    //public void TestStress1c()
    //{
    //    TestStress1();
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("35")]
    //public void TestStress2()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    ISet<String> cells = new HashSet<string>();
    //    for (int i = 1; i < 200; i++)
    //    {
    //        cells.Add("A" + i);
    //        Assert.IsTrue(cells.SetEquals(s.SetCellContents("A" + i, new Formula("A" + (i + 1)))));
    //    }
    //}
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("36")]
    //public void TestStress2a()
    //{
    //    TestStress2();
    //}
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("37")]
    //public void TestStress2b()
    //{
    //    TestStress2();
    //}
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("38")]
    //public void TestStress2c()
    //{
    //    TestStress2();
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("39")]
    //public void TestStress3()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    for (int i = 1; i < 200; i++)
    //    {
    //        s.SetCellContents("A" + i, new Formula("A" + (i + 1)));
    //    }
    //    try
    //    {
    //        s.SetCellContents("A150", new Formula("A50"));
    //        Assert.Fail();
    //    }
    //    catch (CircularException)
    //    {
    //    }
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("40")]
    //public void TestStress3a()
    //{
    //    TestStress3();
    //}
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("41")]
    //public void TestStress3b()
    //{
    //    TestStress3();
    //}
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("42")]
    //public void TestStress3c()
    //{
    //    TestStress3();
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("43")]
    //public void TestStress4()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    for (int i = 0; i < 500; i++)
    //    {
    //        s.SetCellContents("A1" + i, new Formula("A1" + (i + 1)));
    //    }
    //    LinkedList<string> firstCells = new LinkedList<string>();
    //    LinkedList<string> lastCells = new LinkedList<string>();
    //    for (int i = 0; i < 250; i++)
    //    {
    //        firstCells.AddFirst("A1" + i);
    //        lastCells.AddFirst("A1" + (i + 250));
    //    }
    //    Assert.IsTrue(s.SetCellContents("A1249", 25.0).SequenceEqual(firstCells));
    //    Assert.IsTrue(s.SetCellContents("A1499", 0).SequenceEqual(lastCells));
    //}
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("44")]
    //public void TestStress4a()
    //{
    //    TestStress4();
    //}
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("45")]
    //public void TestStress4b()
    //{
    //    TestStress4();
    //}
    //[TestMethod(), Timeout(2000)]
    //[TestCategory("46")]
    //public void TestStress4c()
    //{
    //    TestStress4();
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("47")]
    //public void TestStress5()
    //{
    //    RunRandomizedTest(47, 2519);
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("48")]
    //public void TestStress6()
    //{
    //    RunRandomizedTest(48, 2521);
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("49")]
    //public void TestStress7()
    //{
    //    RunRandomizedTest(49, 2526);
    //}

    //[TestMethod(), Timeout(2000)]
    //[TestCategory("50")]
    //public void TestStress8()
    //{
    //    RunRandomizedTest(50, 2521);
    //}

    ///// <summary>
    ///// Sets random contents for a random cell 10000 times
    ///// </summary>
    ///// <param name="seed">Random seed</param>
    ///// <param name="size">The known resulting spreadsheet size, given the seed</param>
    //public void RunRandomizedTest(int seed, int size)
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    Random rand = new Random(seed);
    //    for (int i = 0; i < 10000; i++)
    //    {
    //        try
    //        {
    //            switch (rand.Next(3))
    //            {
    //                case 0:
    //                    s.SetCellContents(randomName(rand), 3.14);
    //                    break;
    //                case 1:
    //                    s.SetCellContents(randomName(rand), "hello");
    //                    break;
    //                case 2:
    //                    s.SetCellContents(randomName(rand), randomFormula(rand));
    //                    break;
    //            }
    //        }
    //        catch (CircularException)
    //        {
    //        }
    //    }
    //    ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
    //    Assert.AreEqual(size, set.Count);
    //}

    ///// <summary>
    ///// Generates a random cell name with a capital letter and number between 1 - 99
    ///// </summary>
    ///// <param name="rand"></param>
    ///// <returns></returns>
    //private string randomName(Random rand)
    //{
    //    return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
    //}

    ///// <summary>
    ///// Generates a random Formula
    ///// </summary>
    ///// <param name="rand"></param>
    ///// <returns></returns>
    //private string randomFormula(Random rand)
    //{
    //    string f = randomName(rand);
    //    for (int i = 0; i < 10; i++)
    //    {
    //        switch (rand.Next(4))
    //        {
    //            case 0:
    //                f += "+";
    //                break;
    //            case 1:
    //                f += "-";
    //                break;
    //            case 2:
    //                f += "*";
    //                break;
    //            case 3:
    //                f += "/";
    //                break;
    //        }
    //        switch (rand.Next(2))
    //        {
    //            case 0:
    //                f += 7.2;
    //                break;
    //            case 1:
    //                f += randomName(rand);
    //                break;
    //        }
    //    }
    //    return f;
    //}

}
