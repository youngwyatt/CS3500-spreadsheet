namespace SpreadsheetTests;
using CS3500.Spreadsheet;
using CS3500.Formula;
/// <summary>
/// This is a test class for the Spreadsheet class and is intended
/// to contain all SpreadsheetTest Tests
/// </summary>
/// <authors> Wyatt Young </authors>
/// <date> October 17th, 2024 </date>
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
    [TestMethod]
    public void GetNamesOfAllUnemptyCells_EmptySpreadsheet_EmptyList()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.AreEqual(0.0, ss.GetNamesOfAllNonemptyCells().Count());
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

    /// --- SetContentsOfCell Tests ---

    [TestMethod]
    public void SetContentsOfCell_EmptyString_NoChange()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("A1", "");
        Assert.AreEqual(0, ss.GetNamesOfAllNonemptyCells().Count);
    }

    [TestMethod]
    public void SetContentsOfCell_ToEmptyString_DecreaseNonEmptyCells()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("A1", "10");
        Assert.AreEqual(1.0, ss.GetNamesOfAllNonemptyCells().Count);
        ss.SetContentsOfCell("A1", "");
        Assert.AreEqual(0.0, ss.GetNamesOfAllNonemptyCells().Count);
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void SetContentsOfCell_ContentIsJustEquals_FormulaFormatException() 
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "=");
    }
    [TestMethod]
    public void SetContentsOfCell_FormulaWithLeadingWhitespace_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", " = 5 + 3"); 
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
    public void GetCellValue_UnknownFormulaValue_FormulaError() 
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("a1", "=b1+3");
        var result = ss.GetCellValue("A1");
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void GetCellValue_ImproperFormula_FormulaFormatException()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("a1", "=((b1+3) / 2");
        ss.SetContentsOfCell("b1", "2.0");
        var result = ss.GetCellValue("A1");
    }
    [TestMethod]
    public void GetCellValue_EmptySheet_EmptySting()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.AreEqual("", ss.GetCellValue("a1"));
    }
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellValue_InvalidName_InvalidNameException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.GetCellValue("1a");
    }
    [TestMethod]
    public void GetCellValue_DoubleCell_Is10()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "10");
        Assert.AreEqual(10.0, ss.GetCellValue("A1"));
    }
    [TestMethod]
    public void GetCellValue_StringCell_IsLol()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "lol");
        Assert.AreEqual("lol", ss.GetCellValue("A1"));
    }
    [TestMethod]
    public void GetCellValue_Formula_Is10()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "5");
        ss.SetContentsOfCell("b1", "=A1+5");
        Assert.AreEqual(10.0, ss.GetCellValue("B1"));
    }
    [TestMethod]
    public void GetCellValue_FormulaChangedDependent_Is10()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "10.0000");
        ss.SetContentsOfCell("b1", "=A1+5");
        Assert.AreEqual(15.0, ss.GetCellValue("B1"));
        ss.SetContentsOfCell("A1", "5");
        Assert.AreEqual(10.0, ss.GetCellValue("B1"));
    }
    [TestMethod]
    public void GetCellValue_FormulaDependentChangeToString_FormulaError()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "5");
        ss.SetContentsOfCell("b1", "=A1+5");
        Assert.AreEqual(10.0, ss.GetCellValue("B1"));
        ss.SetContentsOfCell("a1", "lol");
        var result = ss.GetCellValue("b1");
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    }

    [TestMethod]
    public void GetCellValue_FormulaDependentChangeDivideByZero_FormulaError()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "10");
        ss.SetContentsOfCell("B1", "=10/A1");
        Assert.AreEqual(1.0, ss.GetCellValue("B1"));
        ss.SetContentsOfCell("A1", "0");
        var result = ss.GetCellValue("b1");
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    }

    [TestMethod]
    public void GetCellValue_FormulaDependentChangeDownstream_ValueChange()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "10");
        ss.SetContentsOfCell("b1", "=10+A1");
        ss.SetContentsOfCell("c1", "=5+B1");
        ss.SetContentsOfCell("a1", "0");
        Assert.AreEqual(15.0, ss.GetCellValue("c1"));
    }

    /// --- LookupVar Tests ---

    [TestMethod]
    public void LookupVar_SimpleDoubleVal_Is10()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("c1", "5");
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
    public void LookupVar_UndefinedVar_FormulaError() 
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("a1", "=b1*2");
        var result = ss.GetCellValue("A1");
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    }

    /// --- this and Changed Property Tests
    
    [TestMethod]
    public void this_EmptySheet_EmptyString()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.AreEqual("", ss["A1"]);
    }

    [TestMethod]
    public void this_InvalidName_InvalidNameException()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.ThrowsException<InvalidNameException>(() => ss["1A"]);
    }

    [TestMethod]
    public void this_ValidName_IsHello()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("B1", "Hello");
        Assert.AreEqual("Hello", ss["B1"]);
    }

    [TestMethod]
    public void this_LowerCaseName_Is10()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("B1", "10");
        Assert.AreEqual(10.0, ss["b1"]);
    }

    [TestMethod]
    public void Changed_NewSheet_False()
    {
        Spreadsheet ss = new();
        Assert.IsFalse(ss.Changed);
    }
    [TestMethod]
    public void Changed_AddToSheet_False()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("a1", "10");
        Assert.IsTrue(ss.Changed);
    }
    [TestMethod]
    public void Changed_SaveFileSheet_False()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("a1", "100");
        Assert.IsTrue(ss.Changed);
        ss.Save("test.txt");
        Assert.IsFalse(ss.Changed);
    }
    [TestMethod]
    public void Changed_NewCircularDependency_False()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("b1", "1");
        ss.SetContentsOfCell("c1", "3");
        ss.SetContentsOfCell("a1", "=B1 + C1");
        ss.SetContentsOfCell("b1", "=C1");
        ss.SetContentsOfCell("d1", "10");
        ss.SetContentsOfCell("c1", "=D1");
        ss.Save("tester.txt");
        Assert.IsFalse(ss.Changed);
        try
        {
            ss.SetContentsOfCell("d1", "=B1");
        }
        catch (CircularException) { }
        Assert.IsFalse(ss.Changed);
    }

    /// --- File Constructor tests ---

    [TestMethod]
    public void FileConstructor_EmptyFile_EmptySpreadsheet()
    {
        Spreadsheet s = new();
        s.Save("test.txt");
        Spreadsheet ss = new("test.txt");
        Assert.AreEqual(s.GetNamesOfAllNonemptyCells().Count, ss.GetNamesOfAllNonemptyCells().Count);
    }

    [TestMethod]
    public void FileConstructor_FilePathDoesNotExist_SpreadsheetReadWriteException()
    {
        Spreadsheet ss;
        Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new(Path.Combine("some", "random", "path.txt")));
    }

    [TestMethod]
    public void FileConstructor_InvalidName_SpreadsheetReadWriteException()
    {
        string sampleText = "{\"Cells\":{\"A1\":{\"StringForm\":\"100\"},\"1A1\":{\"StringForm\":\"=A1\"},\"B23\":{\"StringForm\":\"hello\"}}}";
        File.WriteAllText("sample1.txt", sampleText);
        Spreadsheet ss;
        Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new("sample1.txt"));
    }

    [TestMethod]
    public void FileConstructor_CircularDependency_SpreadsheetReadWriteException()
    {
        string sampleText = "{\"Cells\": {\"A1\": {\"StringForm\": \"100\" },\"A11\": { \"StringForm\": \"=A11\"},\"B23\": {\"StringForm\": \"hello\"}}}";
        File.WriteAllText("sample.txt", sampleText);
        Spreadsheet ss;
        Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new("sample.txt"));
    }

    [TestMethod]
    public void FileConstructor_InvalidFormula_SpreadsheetReadWriteException()
    {
        string sampleText = "{\"Cells\": {\"A1\": {\"StringForm\": \"100\" },\"A11\": {\"StringForm\": \"=13 + 1A45\"},\"B23\": {\"StringForm\": \"hello\"}}}";
        File.WriteAllText("sampleFile.txt", sampleText);
        Spreadsheet ss;
        Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new("sampleFile.txt"));
    }

    [TestMethod]
    public void FileConstructor_InvalidJSONFormat_SpreadsheetReadWriteException()
    {
        string sampleText = "hello";
        File.WriteAllText("sampleFile0.txt", sampleText);
        Spreadsheet ss;
        Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new("sampleFile0.txt"));
    }

    [TestMethod]
    public void FileConstructor_InvalidObjectFormat_SpreadsheetReadWriteException()
    {
        string sampleText = "{\"hello\":\"32\"}";
        File.WriteAllText("sampleFile1.txt", sampleText);
        Spreadsheet ss;
        Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new("sampleFile1.txt"));
    }

    [TestMethod]
    public void FileConstructor_EmptyJSON_SpreadsheetReadWriteException()
    {
        string sampleText = "null";
        File.WriteAllText("sampleFile2.txt", sampleText);
        Spreadsheet ss;
        Assert.ThrowsException<SpreadsheetReadWriteException>(() => ss = new("sampleFile2.txt"));
    }

    /// --- Save Tests ---
    
    [TestMethod]
    public void Save_EmptySpreadsheet_FileSaved()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.Save("empty.txt");
        string fileContents = File.ReadAllText("empty.txt");
        Assert.AreEqual(RemoveWhiteSpace("{\"Cells\": {}}"), RemoveWhiteSpace(fileContents));
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_BadFilePath_SpreadsheetReadWriteException()
    {
        Spreadsheet ss = new();
        ss.Save(Path.Combine("some", "random", "path.txt"));
    }

    [TestMethod]
    public void Save_ExistingCells_CorrectJson()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "100");
        ss.SetContentsOfCell("A11", "=A1");
        ss.SetContentsOfCell("B23", "hello");
        ss.Save("sheet.txt");
        string fileContents = File.ReadAllText("sheet.txt");
        Assert.AreEqual(RemoveWhiteSpace("{\"Cells\": {\"A1\": {\"StringForm\": \"100\"},\"A11\": {\"StringForm\": \"=A1\"},\"B23\": {\"StringForm\": \"hello\"}}}"), RemoveWhiteSpace(fileContents));
    }
    /// <summary>
    /// Private testing helper method that removes tabs, whitepace, and returns, so we can only compare JSON content. 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string RemoveWhiteSpace(string input)
    {
        return string.Concat(input.Where(c => !char.IsWhiteSpace(c) && c != '\n' && c != '\r')); ;
    }

    /// --- Stress Tests ---
    [TestMethod, Timeout(5000)]

    public void StressTests_LargeNumberOfCells_EfficientComplexity()
    {
        Spreadsheet ss = new Spreadsheet();
        // populate 10,000 cells with numbers
        for (int i = 1; i <= 10000; i++)
        {
            ss.SetContentsOfCell($"A{i}", i.ToString());
        }
        // ensure all cells are populated correctly and GetCellValue runs within timeout
        for (int i = 1; i <= 10000; i++)
        {
            Assert.AreEqual((double)i, ss.GetCellValue($"A{i}"));
        }
    }
    [TestMethod, Timeout(5000)]
    public void StressTest_DependencyChain_EfficientComplexity()
    {
        Spreadsheet ss = new Spreadsheet();

        // populate long chain of dependencies, such as A1 = B1 + 1, B1 = C1 + 1, etc.
        for (int i = 1; i <= 5000; i++)
        {
            ss.SetContentsOfCell($"A{i}", i == 1 ? "1" : $"=A{i - 1} + 1");
        }

        // ensure recalculations are correct and GetCellValues runs efficently
        for (int i = 1; i <= 5000; i++)
        {
            Assert.AreEqual((double)i, ss.GetCellValue($"A{i}"));
        }
    }
    [TestMethod, Timeout(5000)]
    public void StressTest_ComplexFormulasRecalculation_EfficientComplexity()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "1");
        ss.SetContentsOfCell("B1", "2");
        ss.SetContentsOfCell("C1", "=A1 + B1 * 10");  // C1 = 1 + 2 * 10 = 21
        ss.SetContentsOfCell("D1", "=C1 / 2");        // D1 = 21 / 2 = 10.5
        ss.SetContentsOfCell("E1", "=D1 + A1 * 5");   // E1 = 10.5 + 1 * 5 = 15.5
        ss.SetContentsOfCell("F1", "=E1 * 2 + B1");   // F1 = 15.5 * 2 + 2 = 33
        ss.SetContentsOfCell("G1", "=F1 - C1");       // G1 = 33 - 21 = 12
        ss.SetContentsOfCell("H1", "=G1 + F1 / D1");  // H1 = 12 + 33 / 10.5 ? 15.142857
        // change A1 and propagate recalculations
        ss.SetContentsOfCell("A1", "10");
        Assert.AreEqual(2.0, ss.GetCellValue("B1"));  // Unchanged
        // New recalculated values:
        // C1 = 10 + 2 * 10 = 30
        Assert.AreEqual(30.0, ss.GetCellValue("C1"));
        // D1 = 30 / 2 = 15
        Assert.AreEqual(15.0, ss.GetCellValue("D1"));
        // E1 = 15 + 10 * 5 = 65
        Assert.AreEqual(65.0, ss.GetCellValue("E1"));
        // F1 = 65 * 2 + 2 = 132
        Assert.AreEqual(132.0, ss.GetCellValue("F1"));
        // G1 = 132 - 30 = 102
        Assert.AreEqual(102.0, ss.GetCellValue("G1"));
        // H1 = 102 + 132 / 15 = 102 + 8.8 = 110.8
        Assert.AreEqual(110.8, ss.GetCellValue("H1"));
    }

}
