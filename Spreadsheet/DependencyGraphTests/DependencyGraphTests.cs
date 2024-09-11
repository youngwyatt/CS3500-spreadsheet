namespace CS3500.DevelopmentTests;
using CS3500.DependencyGraph;
/// <summary>
/// This is a test class for DependencyGraphTest and is intended
/// to contain all DependencyGraphTest Unit Tests
/// </summary>
[TestClass]
public class DependencyGraphTests
{
    /// <summary>
    /// TODO: Explain carefully what this code tests.
    /// Also, update in-line comments as appropriate.
    /// </summary>
    [TestMethod]
    [Timeout(2000)] // 2 second run time limit
    public void DependencyGraphConstructor_StressTestExample_Valid()
    {
        DependencyGraph dg = new();
        // A bunch of strings to use
        // Initialize a size variable for keys of dependencies
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }
        // The correct answers
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = [];
            dependees[i] = [];
        }
        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }
        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }
        // Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }
        // Remove some more
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }
        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new HashSet<string>(dg.GetDependees(letters[i]))));
        }
    }
    // --- HasDependents/HasDependees Tests ---
    [TestMethod]
    public void HasDependents_SinglePair_Valid()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("A", "B");

        Assert.IsTrue(dg.HasDependents("A"));
        Assert.IsFalse(dg.HasDependents("B"));
    }

    [TestMethod]
    public void HasDependees_SinglePair_Valid()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("A", "B");

        Assert.IsTrue(dg.HasDependees("B"));
        Assert.IsFalse(dg.HasDependees("A"));
    }
    // --- GetDependents/Dependees Tests ---
    [TestMethod]
    public void GetDependents_NonExistentPair_IsZero()
    {
        DependencyGraph dg = new DependencyGraph();

        var dependents = dg.GetDependents("X");

        Assert.AreEqual(0, dependents.Count());
    }

    [TestMethod]
    public void GetDependees_NonExistentPair_IsZero()
    {
        DependencyGraph dg = new DependencyGraph();
        var dependees = dg.GetDependees("X");

        Assert.AreEqual(0, dependees.Count());
    }

    [TestMethod]
    public void GetDependents_SinglePair_TrueIsOne()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        var dependents = dg.GetDependents("a");

        Assert.IsTrue(dependents.Contains("b"));
        Assert.AreEqual(1, dependents.Count());
    }

    [TestMethod]
    public void GetDependees_SinglePair_IsOne()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        var dependees = dg.GetDependees("b");

        Assert.IsTrue(dependees.Contains("a"));
        Assert.AreEqual(1, dependees.Count());
    }

    [TestMethod]
    public void GetDependents_ManyPairs_IsFive()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        dg.AddDependency("a", "d");
        dg.AddDependency("a", "e");
        dg.AddDependency("a", "f");
        var dependents = dg.GetDependents("a");

        Assert.AreEqual(5, dependents.Count());
        CollectionAssert.AreEquivalent(new List<string> { "b", "c", "d", "e", "f"}, dependents.ToList());
    }

    [TestMethod]
    public void GetDependees_ManyPairs_IsFive()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("c", "b");
        dg.AddDependency("d", "b");
        dg.AddDependency("e", "b");
        dg.AddDependency("f", "b");

        var dependees = dg.GetDependees("b");

        Assert.AreEqual(5, dependees.Count());
        CollectionAssert.AreEquivalent(new List<string> { "a", "c", "d", "e", "f" }, dependees.ToList());
    }
    // --- Size Tests ---
    [TestMethod]
    public void Size_AddsRemovesReplace_IsNine() 
    {
        DependencyGraph dg = new DependencyGraph();
        // Adding dependencies
        dg.AddDependency("A", "B");
        dg.AddDependency("A", "C");
        dg.AddDependency("B", "D");
        dg.AddDependency("C", "D");
        dg.AddDependency("E", "F");
        dg.AddDependency("G", "H");
        dg.AddDependency("I", "J");
        dg.AddDependency("K", "L");
        dg.AddDependency("M", "N");
        // 9 ordered pairs
        Assert.AreEqual(9, dg.Size);
        dg.AddDependency("O", "P");
        // 10 ordered pairs
        Assert.AreEqual(10, dg.Size);
        // Removing two dependencies
        dg.RemoveDependency("A", "B");
        dg.RemoveDependency("O", "P");
        // 8 ordered pairs
        Assert.AreEqual(8, dg.Size);
        // Replacing dependents for node A 
        dg.ReplaceDependents("A", new List<string> { "X", "Y" });
        // 9 dependencies
        Assert.AreEqual(8, dg.Size);
    }
    // --- AddDependency Tests ---
    [TestMethod]
    public void AddDependency_OnePair_Equivalent()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        var dependentsOfA = dg.GetDependents("a");
        var dependeesOfB = dg.GetDependees("b");

        // check if B is a dependent of A
        CollectionAssert.AreEquivalent(new List<string> { "b" }, dependentsOfA.ToList());
        // check if A is a dependee of B
        CollectionAssert.AreEquivalent(new List<string> { "a" }, dependeesOfB.ToList());
    }

    [TestMethod]
    public void AddDependency_MultipleDependents_Equivalent()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        var dependentsOfA = dg.GetDependents("a");
        
        // check if B and C are included as dependents of A
        CollectionAssert.AreEquivalent(new List<string> { "b", "c" }, dependentsOfA.ToList());
    }

    [TestMethod]
    public void AddDependency_ManyPairsOneCycle_EquivalentOrZero()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        dg.AddDependency("b", "d");
        dg.AddDependency("d", "d");
        var dependentsOfA = dg.GetDependents("a");
        var dependentsOfB = dg.GetDependents("b");
        var dependentsOfC = dg.GetDependents("c");
        var dependentsOfD = dg.GetDependents("d");
        var dependeesOfA = dg.GetDependees("a");
        var dependeesOfB = dg.GetDependees("b");
        var dependeesOfC = dg.GetDependees("c");
        var dependeesOfD = dg.GetDependees("d");
        // check dependents
        CollectionAssert.AreEquivalent(new List<string> { "b", "c" }, dependentsOfA.ToList());
        CollectionAssert.AreEquivalent(new List<string> { "d" }, dependentsOfB.ToList());
        Assert.AreEqual(0, dependentsOfC.Count());
        CollectionAssert.AreEquivalent(new List<string> { "d" }, dependentsOfD.ToList());
        // check dependees
        Assert.AreEqual(0, dependeesOfA.Count());
        CollectionAssert.AreEquivalent(new List<string> { "a" }, dependeesOfB.ToList());
        CollectionAssert.AreEquivalent(new List<string> { "a" }, dependeesOfC.ToList());
        CollectionAssert.AreEquivalent(new List<string> { "b", "d" }, dependeesOfD.ToList());
    }
    // --- RemoveDependency Tests ---
    [TestMethod]
    public void RemoveDependency_AddPairRemovePair_IsZero()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("A", "B");
        dg.RemoveDependency("A", "B");
        var dependentsOfA = dg.GetDependents("A");
        var dependeesOfB = dg.GetDependees("B");

        // after removal, A should have no dependents, and B should have no dependees
        Assert.AreEqual(0, dependentsOfA.Count());
        Assert.AreEqual(0, dependeesOfB.Count());
    }

    [TestMethod]
    public void RemoveDependency_AddRemoveAddAgain_IsZero()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("A", "B");
        dg.RemoveDependency("A", "B");
        dg.AddDependency("A", "C");
        dg.AddDependency("A", "D");
        var dependentsOfA = dg.GetDependents("A");
        var dependeesOfB = dg.GetDependees("B");
        // after removal, A should have C and D dependents, and B should have no dependees
        CollectionAssert.AreEquivalent(new List<string> { "C", "D" }, dependentsOfA.ToList());
        Assert.AreEqual(0, dependeesOfB.Count());
    }
    // --- ReplaceDependents Tests ---
    [TestMethod]
    public void ReplaceDependents_OneReplace_Equivalent()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("A", "B");
        dg.AddDependency("A", "C");
        dg.ReplaceDependents("A", new List<string> { "D", "E" });
        var dependentsOfA = dg.GetDependents("A");
        // Check if dependents of A have been replaced by D and E
        CollectionAssert.AreEquivalent(new List<string> { "D", "E" }, dependentsOfA.ToList());
        // Check B has no dependees
        Assert.AreEqual(0, dg.GetDependees("B").Count());
        // Check D has A as dependee
        CollectionAssert.AreEquivalent(new List<string> { "A" }, dg.GetDependees("D").ToList());
    }
    // --- ReplaceDependees Tests ---
    [TestMethod]
    public void ReplaceDependees_OneReplace_Equivalent()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("B", "A");
        dg.AddDependency("C", "A");
        dg.ReplaceDependees("A", new List<string> { "D", "E" });
        var dependeesOfA = dg.GetDependees("A");
        // Check if dependees of A have been replaced by D and E
        CollectionAssert.AreEquivalent(new List<string> { "D", "E" }, dependeesOfA.ToList());
        // Check B has no dependents
        Assert.AreEqual(0, dg.GetDependents("B").Count());
        // Check D has A as dependent
        CollectionAssert.AreEquivalent(new List<string> { "A" }, dg.GetDependents("D").ToList());
    }

}