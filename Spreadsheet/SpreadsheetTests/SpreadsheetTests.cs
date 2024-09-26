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
        ss.SetCellContents("1a", "invalid name");
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void Spreadsheet_InvalidVar2_InvalidNameException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("1a2", "invalid name");
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void Spreadsheet_InvalidVar3_InvalidNameException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("abc", "invalid name");
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void Spreadsheet_InvalidVar4_InvalidNameException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("123", "invalid name");
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void Spreadsheet_InvalidName_InvalidNameException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents(null, "invalid name");
    }
    [TestMethod]
    public void Spreadsheet_TwoCharsTwoNums_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("ab12", "invalid name");
    }

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
    [TestMethod]
    public void GetNamesEmptyCells_ClearedCell_NotReturned() 
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", 3.0);
        ss.SetCellContents("b1", new Formula("c1*23"));
        ss.SetCellContents("d1", string.Empty);
        // ensure non empty cells are a1 and b1
        List<string> nonempty = ss.GetNamesOfAllNonemptyCells().ToList();
        CollectionAssert.AreEquivalent(new List<string> { "A1", "B1" }, nonempty);
        ss.SetCellContents("b1", string.Empty);
        List<string> nonempty2 = ss.GetNamesOfAllNonemptyCells().ToList();
        CollectionAssert.AreEquivalent(new List<string> { "A1"}, nonempty2);
    }

    // --- GetCellContents Tests ---

    [TestMethod]
    public void GetCellContents_EmptySheet_ReturnsEmptyString()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.AreEqual(string.Empty, ss.GetCellContents("Z99"));  
    }
    [TestMethod]
    public void GetCell_EmptyCell_ReturnsEmptyString()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", string.Empty);
        Assert.AreEqual(string.Empty, ss.GetCellContents("a1"));
        // repeat for "" instead of .Empty
        ss.SetCellContents("b1", "");
        Assert.AreEqual(string.Empty, ss.GetCellContents("b1"));
    }
    [TestMethod]
    public void GetCell_ReturnsEmptyString_WhenCellCleared()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("A1", "test"); 
        ss.SetCellContents("A1", string.Empty);
        Assert.AreEqual(string.Empty, ss.GetCellContents("A1"));
    }
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
    public void GetCellContents_EmptyCell_ReturnsEmptyString() 
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", 2.0);
        Assert.AreEqual(string.Empty, ss.GetCellContents("b1"));
    }
    [TestMethod]
    public void GetCellContents_ClearedCell_ReturnsEmptyString()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("A1", "Some text");
        Assert.AreEqual("Some text", ss.GetCellContents("A1"));
        ss.SetCellContents("A1", string.Empty);
        Assert.AreEqual(string.Empty, ss.GetCellContents("A1"));
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
        ss.SetCellContents("a1", 5);  
        ss.SetCellContents("b1", new Formula("a1 * 2"));   
        ss.SetCellContents("c1", new Formula("a1 + b1"));   
        ss.SetCellContents("d1", 15); 
        IList<string> toRecalculate = ss.SetCellContents("a1", 10.0);
        // changing d1 causes recalculation of d1, c1, b1, a1 in that order
        CollectionAssert.AreEqual(new List<string> { "A1", "B1", "C1" }, toRecalculate.ToList());
    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_SelfReference_ThrowsCircularException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("A1", new Formula("A1 + 1"));  
    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_CircularDependency_ThrowsCircularException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", new Formula("b1 + 1"));  
        ss.SetCellContents("b1", new Formula("c1 + 1"));  
        ss.SetCellContents("c1", new Formula("a1 + 1"));
        // changing a1 to depend on c1 causes circular dependency
        ss.SetCellContents("a1", new Formula("c1 + 1"));
    }
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_ValidThenCircularDependency_ThrowsCircularException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("A1", new Formula("B1 + 1"));  
        ss.SetCellContents("B1", new Formula("C1 + 1"));  
        ss.SetCellContents("C1", 5.0);                   

        // circular dependency by making C1 depend on A1
        ss.SetCellContents("C1", new Formula("A1 + 1"));  
    }
    [TestMethod]
    public void SetCellContents_IndirectDependents_ValidDependentsOrder()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", new Formula("b1 + 1"));  
        ss.SetCellContents("b1", new Formula("c1 + 1"));  
        ss.SetCellContents("c1", 5.0);                    
        IList<string> toRecalculate = ss.SetCellContents("c1", 10.0);
        // changing c1 should result in proper recalculate list 
        CollectionAssert.AreEqual(new List<string> { "C1", "B1", "A1" }, toRecalculate.ToList());
    }

    [TestMethod]
    public void SetCellContents_ComplexIndirectDependents_ValidDependentsOrder()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", new Formula("b1 + c1"));  
        ss.SetCellContents("b1", new Formula("d1 * e1"));   
        ss.SetCellContents("c1", new Formula("f1 + 2"));    
        ss.SetCellContents("d1", new Formula("g1 + 3"));    
        ss.SetCellContents("e1", 10.0);                     
        ss.SetCellContents("f1", 5.0);                      
        ss.SetCellContents("g1", 2.0);                      
        // changing G1 should affect D1, which affects B1 and A1.
        IList<string> toRecalculate = ss.SetCellContents("g1", 4.0);
        CollectionAssert.AreEqual(new List<string> { "G1", "D1", "B1", "A1"}, toRecalculate.ToList());
    }
    /// <summary>
    ///  this is a test designed to fail when AddDependency is used inside of SetCellCOntents and pass
    ///  when ReplaceDependees is used instead properly
    /// </summary>
    [TestMethod]
    public void SetCellContents_AddVsReplaceDependency_FailAddPassReplace() 
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("a1", new Formula("b1+c1"));
        IList<string> toRecalc = ss.SetCellContents("b1", 5.0);
        // check dependents of a1
        Assert.IsTrue(toRecalc.Contains("A1"));
        IList<string> toRecalc2 = ss.SetCellContents("c1", 10);
        Assert.IsTrue(toRecalc2.Contains("A1"));
        // now make a1 depend on d and e
        ss.SetCellContents("a1", new Formula("d1 * e1"));
        // check if b1 and c1 dependents have been updated
        IList<string> toRecalc3 = ss.SetCellContents("b1", 10.0);
        // check dependents of a1
        Assert.IsFalse(toRecalc3.Contains("A1"));
        IList<string> toRecalc4 = ss.SetCellContents("c1", 5);
        Assert.IsFalse(toRecalc4.Contains("A1"));
        // ensure d and e have a as dependent
        IList<string> toRecalc5 = ss.SetCellContents("d1", 10.0);
        Assert.IsTrue(toRecalc5.Contains("A1"));
        IList<string> toRecalc6 = ss.SetCellContents("e1", 10.0);
        Assert.IsTrue(toRecalc6.Contains("A1"));
    }
    [TestMethod]
    public void SetCellContents_EmptyString_RemovesCellAndDependencies()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("A1", new Formula("B1 + C1"));
        ss.SetCellContents("B1", 5.0);  
        ss.SetCellContents("C1", 10.0); 
        // verify that A1 is a non-empty cell and depends on B1 and C1
        ISet<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
        Assert.IsTrue(nonEmptyCells.Contains("A1"));
        Assert.IsTrue(nonEmptyCells.Contains("B1"));
        Assert.IsTrue(nonEmptyCells.Contains("C1"));
        // set A1 to an empty string, which should remove it
        ss.SetCellContents("A1", string.Empty);
        // check for removal
        nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
        Assert.IsFalse(nonEmptyCells.Contains("A1"));
        // check A1 has no more dependencies
        IList<string> toRecalc = ss.SetCellContents("B1", 20.0);
        Assert.IsFalse(toRecalc.Contains("A1"));
    }
    [TestMethod]
    public void SetCellContents_FormulaWithEmptyCell_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("A1", new Formula("B1 + 2"));  
        // A1 should still be treated as a valid formula even though B1 is not yet defined
        object contents = ss.GetCellContents("A1");
        Assert.IsInstanceOfType(contents, typeof(Formula));
        Assert.IsTrue(ss.GetNamesOfAllNonemptyCells().Contains("A1"));
    }
    [TestMethod]
    public void SetCellContents_FormulaToNumber_UpdatesDependencies()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("A1", new Formula("B1 + C1"));
        ss.SetCellContents("B1", 5.0);
        ss.SetCellContents("C1", 3.0);
        ss.SetCellContents("Z1", new Formula("Y1 / X1"));
        ss.SetCellContents("Y1", 2);
        ss.SetCellContents("X1", 3.4);
        IList<string> toRecalculate = ss.SetCellContents("B1", 10.0);
        Assert.IsTrue(toRecalculate.Contains("A1"));
        toRecalculate = ss.SetCellContents("A1", 5.0);
        // ensure A1 is NOT recalculated after changing b1
        toRecalculate = ss.SetCellContents("B1", 7.0);
        Assert.IsFalse(toRecalculate.Contains("A1"));
        // ensure same for text set as well
        ss.SetCellContents("Z1", "test");
        IList<string> toRecalculate2 = ss.SetCellContents("X1", 4.0);
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
        ss.SetCellContents("A1", new Formula("B1 + C1"));
        ss.SetCellContents("B1", new Formula("C1 * 2"));
        ss.SetCellContents("C1", 3.0);
        try
        {
            ss.SetCellContents("C1", new Formula("A1 + 1"));
        }
        catch (CircularException)
        {
            IList<string> toRecalculate = ss.SetCellContents("B1", 5.0);
            Assert.IsTrue(toRecalculate.Contains("A1")); 
            Assert.IsFalse(toRecalculate.Contains("C1"));
            Assert.AreEqual(ss.GetCellContents("C1"), 3.0);
            throw; 
        }
    }


}