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
    // --- GetNamesOfAllEmptyCells Tests ---
    [TestMethod]
    public void GetNamesEmptyCells_SimpleCells_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", 5.0);
        ss.SetCellContents("b1", "test");
        // get names of non empty cells
        ISet<string> popCells = ss.GetNamesOfAllNonemptyCells();
        Assert.IsTrue(popCells.Contains("A1"));
        Assert.IsTrue(popCells.Contains("B1"));
        Assert.AreEqual(2, popCells.Count);
    }

    // --- GetCellContents Tests ---

    [TestMethod]
    public void GetCellContents_SimpleCells_Valid() 
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", 5.0);
        ss.SetCellContents("b1", "test");
        Assert.AreEqual(5.0, ss.GetCellContents("a1"));
        Assert.AreEqual("test", (string)ss.GetCellContents("b1"));
    }
    [TestMethod]
    public void GetCellContents_EmptyCell_EmptyString() 
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", 2.0);
        Assert.AreEqual(string.Empty, ss.GetCellContents("b1"));
    }

    // --- SetCellContents Tests ---

    [TestMethod]
    public void SetCellContents_SimpleNumberStringFormula_Valid() 
    {
        Spreadsheet ss = new Spreadsheet();
        IList<string> num = ss.SetCellContents("a1", 5.0);
        IList<string> str = ss.SetCellContents("b2", "Test");
        IList<string> formula = ss.SetCellContents("c3", new Formula("12 - 2"));
        Assert.IsTrue(num.Contains("A1"));
        Assert.AreEqual(5.0, ss.GetCellContents("a1"));
        Assert.IsTrue(str.Contains("B2"));
        Assert.AreEqual("Test", ss.GetCellContents("b2"));
        Assert.IsTrue(formula.Contains("C3"));
        Assert.AreEqual(new Formula("12-2"), ss.GetCellContents("c3"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContents_InvalidName_InavlidNameException()
    {
        Spreadsheet ss = new Spreadsheet();
        // set cell to invalid name
        ss.SetCellContents("1a", "invalid name test");
    }

    [TestMethod]
    public void SetCellContents_FormulaDirectDependents_ValidDependentsOrder()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", 5);  // a1 is constant
        ss.SetCellContents("b1", new Formula("a1 * 2"));   // b1 depends on a1
        ss.SetCellContents("c1", new Formula("a1 + b1"));   // c1 depends on a1 and b1
        ss.SetCellContents("d1", 15); //e1 cosntant
        IList<string> toRecalculate = ss.SetCellContents("a1", 10.0);
        // changing d1 causes recalculation of d1, c1, b1, a1 in that order
        CollectionAssert.AreEqual(new List<string> { "A1", "B1", "C1" }, toRecalculate.ToList());
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_CircularDependency_ThrowsCircularException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", new Formula("b1 + 1"));  // a1 depends on b1
        ss.SetCellContents("b1", new Formula("c1 + 1"));  // b1 depends on c1
        ss.SetCellContents("c1", new Formula("a1 + 1"));  // c1 depends on a1 causing circular dependency
        // changing a1 to depend on c1 causes circular dependency
        ss.SetCellContents("a1", new Formula("c1 + 1"));
    }

    [TestMethod]
    public void SetCellContents_IndirectDependents_ValidDependentsOrder()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", new Formula("b1 + 1"));  // a1 depends on b1
        ss.SetCellContents("b1", new Formula("c1 + 1"));  // b1 depends on c1
        ss.SetCellContents("c1", 5.0);                    // c1 is a constant
        IList<string> toRecalculate = ss.SetCellContents("c1", 10.0);
        // changing c1 should result in proper recalculate list 
        CollectionAssert.AreEqual(new List<string> { "C1", "B1", "A1" }, toRecalculate.ToList());
    }

    [TestMethod]
    public void SetCellContents_ComplexIndirectDependents_ValidDependentsOrder()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", new Formula("b1 + c1"));  // A1 depends on B1 and C1
        ss.SetCellContents("b1", new Formula("d1 * e1"));   // B1 depends on D1 and E1
        ss.SetCellContents("c1", new Formula("f1 + 2"));    // C1 depends on F1
        ss.SetCellContents("d1", new Formula("g1 + 3"));    // D1 depends on G1
        ss.SetCellContents("e1", 10.0);                     // E1 is a constant
        ss.SetCellContents("f1", 5.0);                      // F1 is a constant
        ss.SetCellContents("g1", 2.0);                      // G1 is a constant
        // changing G1 should affect D1, which affects B1 and A1.
        IList<string> toRecalculate = ss.SetCellContents("g1", 4.0);
        CollectionAssert.AreEqual(new List<string> { "G1", "D1", "B1", "A1"}, toRecalculate.ToList());
    }


}