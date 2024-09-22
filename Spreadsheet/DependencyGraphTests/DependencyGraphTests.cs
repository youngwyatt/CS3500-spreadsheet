namespace CS3500.DevelopmentTests;
using CS3500.DependencyGraph;
/// <summary>
/// This is a test class for DependencyGraphTest and is intended
/// to contain all DependencyGraphTest Unit Tests
/// </summary>
/// <authors> Wyatt Young </authors>
/// <date> September 21st, 2024 </date>
[TestClass]
public class DependencyGraphTests
{
    /// <summary>
    /// This test is an overall stress test for the DependencyGraph test class
    /// by simulating a large number of operations such as adding, removing, 
    /// and replacing dependencies. It ensures the DependencyGraph maintains
    /// consistency and accuracy under heavy loads validating the classes
    /// integrity at a large scale.
    /// </summary>
    [TestMethod]
    [Timeout(2000)] // 2 second run time limit
    public void DependencyGraphConstructor_StressTestExample_Valid()
    {
        // creates DependencyGraph instance
        DependencyGraph dg = new(); 
        // A bunch of strings to use
        // Initialize a size variable for keys of dependencies
        const int SIZE = 200;
        // Intialize string array to hold node names
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            // Generate variable names using single chars and index i cast to a char
            letters[i] = string.Empty + ((char)('a' + i));
        }
        // Initialize expected results for dependents and dependees
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        // Initialize empty sets for each dictionary through size
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = [];
            dependees[i] = [];
        }
        // Add a bunch of dependencies into graph and expected sets
        // Added in pairs (i, j) (dependee, dependent); pattern to be 
        // repeated for each operation
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }
        // Remove a bunch of dependencies in steps of 4
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }
        // For each node, add back dependencies in steps of 2
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }
        // For every other node, remove dependencies in steps of 3
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }
        // Assert expected results from separate HashSets and DependencyGraph object
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
        DependencyGraph dg = new();
        dg.AddDependency("A", "B");

        Assert.IsTrue(dg.HasDependents("A"));
        Assert.IsFalse(dg.HasDependents("B"));
    }
    [TestMethod]
    public void HasDependees_SinglePair_Valid()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A", "B");

        Assert.IsTrue(dg.HasDependees("B"));
        Assert.IsFalse(dg.HasDependees("A"));
    }
    // --- GetDependents/Dependees Tests ---
    [TestMethod]
    public void GetDependents_NonExistentPair_IsZero()
    {
        DependencyGraph dg = new();

        var dependents = dg.GetDependents("X");

        Assert.AreEqual(0, dependents.Count());
    }
    [TestMethod]
    public void GetDependees_NonExistentPair_IsZero()
    {
        DependencyGraph dg = new();
        var dependees = dg.GetDependees("X");

        Assert.AreEqual(0, dependees.Count());
    }
    [TestMethod]
    public void GetDependents_SinglePair_TrueIsOne()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        var dependents = dg.GetDependents("a");

        Assert.IsTrue(dependents.Contains("b"));
        Assert.AreEqual(1, dependents.Count());
    }
    [TestMethod]
    public void GetDependees_SinglePair_IsOne()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        var dependees = dg.GetDependees("b");

        Assert.IsTrue(dependees.Contains("a"));
        Assert.AreEqual(1, dependees.Count());
    }
    [TestMethod]
    public void GetDependents_ManyPairs_IsFive()
    {
        DependencyGraph dg = new();
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
        DependencyGraph dg = new();
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
        DependencyGraph dg = new();
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
        Assert.AreEqual(9, dg.Size);
    }
    // --- AddDependency Tests ---
    [TestMethod]
    public void AddDependency_OnePair_Equivalent()
    {
        DependencyGraph dg = new();
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
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        var dependentsOfA = dg.GetDependents("a");
        
        // check if B and C are included as dependents of A
        CollectionAssert.AreEquivalent(new List<string> { "b", "c" }, dependentsOfA.ToList());
    }

    [TestMethod]
    public void AddDependency_ManyPairsOneCycle_EquivalentOrZero()
    {
        DependencyGraph dg = new();
        // add a couple dependencies to ensure general structure of graph is correct
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
    [TestMethod]
    public void AddDependency_Duplicate_NoChangeInSize()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("A", "B");
        // add the same dependency again
        dg.AddDependency("A", "B");
        Assert.AreEqual(1, dg.Size);
        CollectionAssert.AreEquivalent(new List<string> { "B" }, dg.GetDependents("A").ToList());
    }
    // --- RemoveDependency Tests ---
    [TestMethod]
    public void RemoveDependency_AddPairRemovePair_IsZero()
    {
        DependencyGraph dg = new();
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
        DependencyGraph dg = new();
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
    [TestMethod]
    public void RemoveDependency_NonExistent_NoChange()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A", "B");
        dg.AddDependency("B", "C");
        // attempt to remove a non-existent dependency
        dg.RemoveDependency("C", "D");
        // size should remain the same since (C, D) was never added
        Assert.AreEqual(2, dg.Size);
        CollectionAssert.AreEquivalent(new List<string> { "B" }, dg.GetDependents("A").ToList());
        CollectionAssert.AreEquivalent(new List<string> { "A" }, dg.GetDependees("B").ToList());
    }
    // --- ReplaceDependents Tests ---
    [TestMethod]
    public void ReplaceDependents_OneReplace_Equivalent()
    {
        DependencyGraph dg = new();
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
    [TestMethod]
    public void ReplaceDependents_EmptySet_RemovesAllDependents()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A", "B");
        dg.AddDependency("A", "C");
        // replace dependents of A with an empty set
        dg.ReplaceDependents("A", new List<string>());
        // ensure A has no dependents, vica versa for B and C dependees
        Assert.AreEqual(0, dg.GetDependents("A").Count());
        Assert.AreEqual(0, dg.GetDependees("B").Count());
        Assert.AreEqual(0, dg.GetDependees("C").Count());
        Assert.AreEqual(0, dg.Size);
    }
    // --- ReplaceDependees Tests ---
    [TestMethod]
    public void ReplaceDependees_OneReplace_Equivalent()
    {
        DependencyGraph dg = new();
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


    [TestMethod]
    [Timeout(2000)]
    [TestCategory("33")]
    public void ReplaceDependees_OnEmptyGraph_AddsNewDependees()
    {
        DependencyGraph dg = new();

        dg.ReplaceDependees("b", ["a"]);

        Assert.AreEqual(1, dg.Size);
        Assert.IsTrue(new HashSet<string> { "b" }.SetEquals(dg.GetDependents("a")));
    }

    // ********************************** A THIRD STESS TEST, REPEATED ******************** //

    /// <summary>
    ///    Using lots of data with replacement.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("45")]
    public void StressTest15_1()
    {
        // Dependency graph
        DependencyGraph t = new();

        // A bunch of strings to use
        const int SIZE = 1000;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = "a" + i;
        }

        // The correct answers
        HashSet<string>[] dents = new HashSet<string>[SIZE];
        HashSet<string>[] dees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dents[i] = [];
            dees[i] = [];
        }

        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]);
                dees[j].Add(letters[i]);
            }
        }

        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
            Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
        }

        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 2; j < SIZE; j += 3)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]);
                dees[j].Remove(letters[i]);
            }
        }

        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
            Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
        }

        // Replace a bunch of dependees
        for (int i = 0; i < SIZE; i += 2)
        {
            HashSet<string> newDees = [];
            for (int j = 0; j < SIZE; j += 9)
            {
                newDees.Add(letters[j]);
            }

            t.ReplaceDependees(letters[i], newDees);

            foreach (string s in dees[i])
            {
                dents[int.Parse(s[1..])].Remove(letters[i]);
            }

            foreach (string s in newDees)
            {
                dents[int.Parse(s[1..])].Add(letters[i]);
            }

            dees[i] = newDees;
        }

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
            Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
        }
    }

    /// <summary>
    ///   Increase weight of StressTest15.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("46")]
    public void StressTest15_2()
    {
        StressTest15_1();
    }

    /// <summary>
    ///   Increase weight of StressTest15.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("47")]
    public void StressTest17()
    {
        StressTest15_1();
    }

    /// <summary>
    ///   Helper code to build a simple dependency graph.
    /// </summary>
    /// <returns> The new graph. </returns>
    private static DependencyGraph SetupComplexDependencies()
    {
        DependencyGraph t = new();
        t.AddDependency("x", "b");
        t.AddDependency("a", "z");
        t.ReplaceDependents("b", []);
        t.AddDependency("y", "b");
        t.ReplaceDependents("a", ["c"]);
        t.AddDependency("w", "d");
        t.ReplaceDependees("b", ["a", "c"]);
        t.ReplaceDependees("d", ["b"]);
        return t;
    }
}

/// <summary>
///   Helper methods for the tests above.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    ///   Check to see if the two "sets" (source and items) match, i.e.,
    ///   contain exactly the same values.
    /// </summary>
    /// <param name="source"> original container.</param>
    /// <param name="items"> elements to match against.</param>
    /// <returns> true if every element in source is in items and vice versa. They are the "same set".</returns>
    public static bool Matches(this IEnumerable<string> source, params string[] items)
    {
        return (source.Count() == items.Length) && items.All(item => source.Contains(item));
    }
}