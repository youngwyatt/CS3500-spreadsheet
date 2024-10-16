// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta and de St. Germain, Fall 2021, Fall 2024
//     - Updated return types
//     - Updated documentation
// <authors> Wyatt Young </authors>
// <date> October 13th, 2024 </date>
namespace CS3500.Spreadsheet;

using CS3500.Formula;
using CS3500.DependencyGraph;
using System.Text.RegularExpressions;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
///   <para>
///     Thrown to indicate that a change to a cell will cause a circular dependency.
///   </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///   <para>
///     Thrown to indicate that a name parameter was invalid.
///   </para>
/// </summary>
public class InvalidNameException : Exception
{
}
/// <summary>
/// <para>
///   Thrown to indicate that a read or write attempt has failed with
///   an expected error message informing the user of what went wrong.
/// </para>
/// </summary>
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    ///   <para>
    ///     Creates the exception with a message defining what went wrong.
    ///   </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException(string msg)
    : base(msg)
    {
    }
}
/// <summary>
///   <para>
///     An Spreadsheet object represents the state of a simple spreadsheet.  A
///     spreadsheet represents an infinite number of named cells.
///   </para>
/// <para>
///     Valid Cell Names: A string is a valid cell name if and only if it is one or
///     more letters followed by one or more numbers, e.g., A5, BC27.
/// </para>
/// <para>
///    Cell names are case insensitive, so "x1" and "X1" are the same cell name.
///    Your code should normalize (uppercased) any stored name but accept either.
/// </para>
/// <para>
///     A spreadsheet represents a cell corresponding to every possible cell name.  (This
///     means that a spreadsheet contains an infinite number of cells.)  In addition to
///     a name, each cell has a contents and a value.  The distinction is important.
/// </para>
/// <para>
///     The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
///     If the contents of a cell is set to the empty string, the cell is considered empty.
/// </para>
/// <para>
///     By analogy, the contents of a cell in Excel is what is displayed on
///     the editing line when the cell is selected.
/// </para>
/// <para>
///     In a new spreadsheet, the contents of every cell is the empty string. Note:
///     this is by definition (it is IMPLIED, not stored).
/// </para>
/// <para>
///     The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
///     in the grid.) We are not concerned with cell values yet, only with their contents,
///     but for context:
/// </para>
/// <list type="number">
///   <item>If a cell's contents is a string, its value is that string.</item>
///   <item>If a cell's contents is a double, its value is that double.</item>
///   <item>
///     <para>
///       If a cell's contents is a Formula, its value is either a double or a FormulaError,
///       as reported by the Evaluate method of the Formula class.  For this assignment,
///       you are not dealing with values yet.
///     </para>
///   </item>
/// </list>
/// <para>
///     Spreadsheets are never allowed to contain a combination of Formulas that establish
///     a circular dependency.  A circular dependency exists when a cell depends on itself,
///     either directly or indirectly.
///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
///     dependency.
/// </para>
/// </summary>
public class Spreadsheet
{

    /// <summary>
    /// newsted cell class to represent a single cell in the backing dictionary
    /// </summary>
    private class Cell 
    {
        // private member variable for Text field of Cell class
        private string? Text { get; set; }
        // private member variable for Number field of Cell class
        private double? Number { get; set; }
        // private member variable for Formula field of Cell class
        private Formula? Formula { get; set; }
        /// <summary>
        /// Constructor for cell class takes in either text, a number, or a Formula.
        /// </summary>
        /// <param name="text">Text cell</param>
        /// <param name="number">Number cell</param>
        /// <param name="formula">Formula cell</param>
        public Cell(string? text, double? number = null, Formula? formula = null)
        {
            if (text != null)
            {
                this.Text = text;
            }
            else if (number != null)
            {
                this.Number = number;
            }
            else
            {
                this.Formula = formula;
            }
        }
        /// <summary>
        /// Helper method to return the cells contents 
        /// </summary>
        /// <returns>Cell object contents(string, double, or Formula)</returns>
        public object? GetCell() 
        {
            if (Text != null && Text != string.Empty) return Text;
            if (Number.HasValue) return Number.Value;
            else return Formula;
        }
        ///<summary>
        /// Helper method to help identify the type of the cell; i.e text, Number, or Formula
        /// </summary>
        public string GetCellType() 
        {
            if (Text != null && Text != string.Empty) return "Text";
            if (Number != null) return "Number";
            return "Formula";   
        }
    }

    ///<summary>
    /// private member DependencyGraph object to back spreadsheet  
    /// </summary>
    private DependencyGraph graph = new DependencyGraph();

    ///<summary>
    /// private member Dictionary object to represent cell contents
    /// </summary>
    [JsonInclude]
    [JsonPropertyName("Cells")]
    private Dictionary<string, Cell> cellSheet = new Dictionary<string, Cell>();

    /// <summary>
    ///   <para>
    ///     Return the value of the named cell, as defined by
    ///     <see cref="GetCellValue(string)"/>.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   <see cref="GetCellValue(string)"/>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object this[string name]
    {
        get { return GetCellValue(NormalizeValidateName(name)); }
    }


    /// <summary>
    /// True if this spreadsheet has been changed since it was 
    /// created or saved (whichever happened most recently),
    /// False otherwise.
    /// </summary>
    [JsonIgnore]
    public bool Changed { get; private set; }

    ///<summary>
    /// Default constructor that atkes in zero arguments. Crteates an empty spreadsheet in which every cell 
    /// is empty.
    /// </summary>
    public Spreadsheet() 
    {
        this.graph = new DependencyGraph();
        this.cellSheet = new Dictionary<string, Cell>();
        this.Changed = false;
    }

    /// <summary>
    /// Constructs a spreadsheet using the saved data in the file refered to by
    /// the given filename. 
    /// <see cref="Save(string)"/>
    /// </summary>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   Thrown if the file can not be loaded into a spreadsheet for any reason
    /// </exception>
    /// <param name="filename">The path to the file containing the spreadsheet to load</param>
    public Spreadsheet(string filename)
    {
        try
        {
            // read JSON file
            string file = File.ReadAllText(filename);
            // deserialize
            var deserializedFile = JsonSerializer.Deserialize<SpreadsheetJson>(file);
            // check for null/empty file content
            if (deserializedFile == null || deserializedFile.CellsJson == null)
            {
                throw new SpreadsheetReadWriteException("File content is empty or invalid");
            }
            // initialize new Spreadsheet
            this.graph = new DependencyGraph();
            this.cellSheet = new Dictionary<string, Cell>();
            this.Changed = false;
            // check each cell in file and add to new spreadsheet if valid cell data
            foreach (var data in deserializedFile.CellsJson)
            {
                // validate name
                string cellName = NormalizeValidateName(data.Key);
                string content = data.Value.StringForm;
                SetContentsOfCell(cellName, content);
            }
        }
        catch (Exception ex)
        {
            throw new SpreadsheetReadWriteException($"Error deserializing and reading passed Json file: {ex.Message}");
        }
    }
    ///<summary>
    /// Helper class for matching structure of deserializing passed Json file
    /// </summary>
    private class SpreadsheetJson 
    {
        ///<summary>
        /// dictionary object for storing cell data
        /// </summary>
        public Dictionary<string, CellData>? CellsJson { get; set; }
    }
    ///<summary>
    /// Helper class that contains StringForm for use in Json deserialization
    /// </summary>
    private class CellData 
    {
        ///<summary>
        /// StringForm property for Json deserialization to match structure of passed file
        /// </summary>
        public string StringForm { get; set; } = "";
    }

    /// <summary>
    ///   <para>
    ///     Writes the contents of this spreadsheet to the named file using a JSON format.
    ///     If the file already exists, overwrite it.
    ///   </para>
    ///   <para>
    ///     The output JSON should look like the following.
    ///   </para>
    ///   <para>
    ///     For example, consider a spreadsheet that contains a cell "A1" 
    ///     with contents being the double 5.0, and a cell "B3" with contents 
    ///     being the Formula("A1+2"), and a cell "C4" with the contents "hello".
    ///   </para>
    ///   <para>
    ///      This method would produce the following JSON string:
    ///   </para>
    ///   <code>
    ///   {
    ///     "Cells": {
    ///       "A1": {
    ///         "StringForm": "5"
    ///       },
    ///       "B3": {
    ///         "StringForm": "=A1+2"
    ///       },
    ///       "C4": {
    ///         "StringForm": "hello"
    ///       }
    ///     }
    ///   }
    ///   </code>
    ///   <para>
    ///     You can achieve this by making sure your data structure is a dictionary 
    ///     and that the contained objects (Cells) have property named "StringForm"
    ///     (if this name does not match your existing code, use the JsonPropertyName 
    ///     attribute).
    ///   </para>
    ///   <para>
    ///     There can be 0 cells in the dictionary, resulting in { "Cells" : {} } 
    ///   </para>
    ///   <para>
    ///     Further, when writing the value of each cell...
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       If the contents is a string, the value of StringForm is that string
    ///     </item>
    ///     <item>
    ///       If the contents is a double d, the value of StringForm is d.ToString()
    ///     </item>
    ///     <item>
    ///       If the contents is a Formula f, the value of StringForm is "=" + f.ToString()
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   If there are any problems opening, writing, or closing the file, 
    ///   the method should throw a SpreadsheetReadWriteException with an
    ///   explanatory message.
    /// </exception>
    public void Save(string filename)
    {
        try
        {
            // Json options
            var JsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            // build string from serializer and write to file
            string JsonSerialized = JsonSerializer.Serialize(this, JsonOptions);
            File.WriteAllText(filename, JsonSerialized);
            // change to true as sheet was saved
            Changed = false;
        }
        catch (Exception ex) 
        {
            throw new SpreadsheetReadWriteException($"Error serializing and saving spreadsheet to file: {ex.Message}");
        }
    }

    /// <summary>
    ///   <para>
    ///     Return the value of the named cell.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   Returns the value (as opposed to the contents) of the named cell.  The return
    ///   value should be either a string, a double, or a CS3500.Formula.FormulaError.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue(string name)
    {
        string normName = NormalizeValidateName(name);
        object content = GetCellContents(normName);
        if (content is string)
        {
            string strContent = (string)content;
            // check for formula
            if (strContent.StartsWith("="))
            {
                // evaluate and return if so
                Formula f = new Formula(strContent.Substring(1));
                return f.Evaluate(LookupVar);
            }
            // normal text box
            else return content;
        }
        // check for double
        else if (content is double) 
        {
            return (double)content;
        }
        else if(content is Formula)
        {
            return ((Formula)content).Evaluate(LookupVar);
        }
        // return empty string if cell is not found or value is null
        return string.Empty;
    }

    /// <summary>
    ///   <para>
    ///     Set the contents of the named cell to be the provided string
    ///     which will either represent (1) a string, (2) a number, or 
    ///     (3) a formula (based on the prepended '=' character).
    ///   </para>
    ///   <para>
    ///     Rules of parsing the input string:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       <para>
    ///         If 'content' parses as a double, the contents of the named
    ///         cell becomes that double.
    ///       </para>
    ///     </item>
    ///     <item>
    ///         If the string does not begin with an '=', the contents of the 
    ///         named cell becomes 'content'.
    ///     </item>
    ///     <item>
    ///       <para>
    ///         If 'content' begins with the character '=', an attempt is made
    ///         to parse the remainder of content into a Formula f using the Formula
    ///         constructor.  There are then three possibilities:
    ///       </para>
    ///       <list type="number">
    ///         <item>
    ///           If the remainder of content cannot be parsed into a Formula, a 
    ///           CS3500.Formula.FormulaFormatException is thrown.
    ///         </item>
    ///         <item>
    ///           Otherwise, if changing the contents of the named cell to be f
    ///           would cause a circular dependency, a CircularException is thrown,
    ///           and no change is made to the spreadsheet.
    ///         </item>
    ///         <item>
    ///           Otherwise, the contents of the named cell becomes f.
    ///         </item>
    ///       </list>
    ///     </item>
    ///   </list>
    /// </summary>
    /// <returns>
    ///   <para>
    ///     The method returns a list consisting of the name plus the names 
    ///     of all other cells whose value depends, directly or indirectly, 
    ///     on the named cell. The order of the list should be any order 
    ///     such that if cells are re-evaluated in that order, their dependencies 
    ///     are satisfied by the time they are evaluated.
    ///   </para>
    ///   <example>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list {A1, B1, C1} is returned.
    ///   </example>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If name is invalid, throws an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///     If a formula would result in a circular dependency, throws CircularException.
    /// </exception>
    public IList<string> SetContentsOfCell(string name, string content)
    {
        string normName = NormalizeValidateName(name);
        // if content is empty or null dont add/remove from spreadsheet
        if (string.IsNullOrEmpty(content)) 
        {
            return SetCellContents(normName, string.Empty);
        }
        // check content is a number
        if (double.TryParse(content, out double num))
        {
            return SetCellContents(normName, num);
        }
        // check content is a string
        else if (content[0] != '=')
        {
            return SetCellContents(normName, content);
        }
        // content is a formula
        Formula f;
        string formContent = content.Substring(1);
        try
        {
            // attempt to parse
            f = new Formula(formContent);
        }
        catch (FormulaFormatException)
        {
            throw new FormulaFormatException("Content couldn't be parsed as Formula");
        }
        // attempt to place in spreadsheet w/out any circular dependencies
        return SetCellContents(normName, f);
    }

    /// <summary>
    ///   Provides a copy of the normalized names of all of the cells in the spreadsheet
    ///   that contain information (i.e., non-empty cells).
    /// </summary>
    /// <returns>
    ///   A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        return new HashSet<string>(cellSheet.Keys);
    }

    /// <summary>
    ///   Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   Thrown if the name is invalid.
    /// </exception>
    ///
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    ///   The contents as either a string, a double, or a Formula.
    ///   See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        string normName = NormalizeValidateName(name);
        if (cellSheet.TryGetValue(normName, out var cell))
        {
            return cell.GetCell() ?? string.Empty;
        }
        // return empty string if cell is not found or content is null
        return string.Empty;  
    }
    
    /// </summary>
    /// <summary>
    ///  Set the contents of the named cell to the given number.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    ///
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new contents of the cell. </param>
    /// <returns>
    ///   <para>
    ///     This method returns an ordered list consisting of the passed in name
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell.
    ///   </para>
    ///   <para>
    ///     The order must correspond to a valid dependency ordering for recomputing
    ///     all of the cells, i.e., if you re-evaluate each cells in the order of the list,
    ///     the overall spreadsheet will be correctly updated.
    ///   </para>
    ///   <para>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1 must be
    ///     evaluated, followed by B1, followed by C1.
    ///   </para>
    /// </returns>
    private IList<string> SetCellContents(string name, double number)
    {
        cellSheet[name] = new Cell(null, number, null);
        graph.ReplaceDependees(name, new List<string>());   
        return GetCellsToRecalculate(name).ToList();
    }

    /// <summary>
    ///   The contents of the named cell becomes the given text.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, string text)
    {
        // remove empty cells
        if (text == string.Empty || text == "")
        {
            cellSheet.Remove(name);
            graph.ReplaceDependees(name, new List<string>());
        }
        else
        {
            cellSheet[name] = new Cell(text);
            graph.ReplaceDependees(name, new List<string>());
        }
        return GetCellsToRecalculate(name).ToList();
    }

    /// <summary>
    ///   Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   <para>
    ///     If changing the contents of the named cell to be the formula would
    ///     cause a circular dependency, throw a CircularException, and no
    ///     change is made to the spreadsheet.
    ///   </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {
        // store old dependees in case of circular exception
        IEnumerable<string> oldDependees = graph.GetDependees(name);
        // replace dependees of adjusted/added cell
        graph.ReplaceDependees(name, formula.GetVariables());
        try
        {
            IList<string> toRecalc = GetCellsToRecalculate(name).ToList();
            cellSheet[name] = new Cell(null, null, formula);
            return GetCellsToRecalculate(name).ToList();
        }
        catch (CircularException) 
        {
            // if circular exception is thrown revert to old dependees
            graph.ReplaceDependees(name, oldDependees);
            throw;
        }
    }
    ///<summary>
    ///Lookup delegate of type "Lookup" for looking up variables in the spreadsheet for their values
    /// <exception cref="ArgumentException">
    /// If value is unknown
    /// </exception>
    /// <returns>
    /// the value of the input variable name
    /// </returns>
    private double LookupVar(string variableName) 
    {
        string normName = NormalizeValidateName(variableName);
        // determine contents of variable 
        //HashSet<string> cells = (HashSet<string>)GetNamesOfAllNonemptyCells();
        if (cellSheet.TryGetValue(normName, out var cell)) 
        {
            object? content = cell.GetCell();
            if (content != null)
            {
                if (double.TryParse((string)content, out double num)) 
                {
                    return num;
                }
            }
        }
        throw new ArgumentException("Variable not found on lookup");
    }
    /// <summary>
    ///   Returns an enumeration, without duplicates, of the names of all cells whose
    ///   values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name.  </param>
    /// <returns>
    ///   <para>
    ///     Returns an enumeration, without duplicates, of the names of all cells
    ///     that contain formulas containing name.
    ///   </para>
    ///   <para>For example, suppose that: </para>
    ///   <list type="bullet">
    ///      <item>A1 contains 3</item>
    ///      <item>B1 contains the formula A1 * A1</item>
    ///      <item>C1 contains the formula B1 + A1</item>
    ///      <item>D1 contains the formula B1 - C1</item>
    ///   </list>
    ///   <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        string normName = NormalizeValidateName(name);
        return graph.GetDependents(normName);
    }

    /// <summary>
    ///   <para>
    ///     This method is implemented for you, but makes use of your GetDirectDependents.
    ///   </para>
    ///   <para>
    ///     Returns an enumeration of the names of all cells whose values must
    ///     be recalculated, assuming that the contents of the cell referred
    ///     to by name has changed.  The cell names are enumerated in an order
    ///     in which the calculations should be done.
    ///   </para>
    ///   <exception cref="CircularException">
    ///     If the cell referred to by name is involved in a circular dependency,
    ///     throws a CircularException.
    ///   </exception>
    ///   <para>
    ///     For example, suppose that:
    ///   </para>
    ///   <list type="number">
    ///     <item>
    ///       A1 contains 5
    ///     </item>
    ///     <item>
    ///       B1 contains the formula A1 + 2.
    ///     </item>
    ///     <item>
    ///       C1 contains the formula A1 + B1.
    ///     </item>
    ///     <item>
    ///       D1 contains the formula A1 * 7.
    ///     </item>
    ///     <item>
    ///       E1 contains 15
    ///     </item>
    ///   </list>
    ///   <para>
    ///     If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
    ///     and they must be recalculated in an order which has A1 first, and B1 before C1
    ///     (there are multiple such valid orders).
    ///     The method will produce one of those enumerations.
    ///   </para>
    ///   <para>
    ///      PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
    ///      IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
    ///   </para>
    /// </summary>
    /// <param name="name"> The name of the cell.  Requires that name be a valid cell name.</param>
    /// <returns>
    ///    Returns an enumeration of the names of all cells whose values must
    ///    be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        LinkedList<string> changed = new();
        HashSet<string> visited = [];
        Visit(name, name, visited, changed);
        return changed;
    }

    /// <summary>
    ///   A helper for the GetCellsToRecalculate method.
    ///   FIXME: You should fully comment what is going on below using XML tags as appropriate,
    ///   as well as inline comments in the code.
    /// </summary>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        visited.Add(name);
        foreach (string n in GetDirectDependents(name))
        {
            if (n.Equals(start))
            {
                throw new CircularException();
            }
            else if (!visited.Contains(n))
            {
                Visit(start, n, visited, changed);
            }
        }

        changed.AddFirst(name);
    }

    ///<summary>
    /// private helper method for validating/normalizing cell names
    /// </summary>
    /// <param> Name to validate/normalize </param>
    /// <returns> normalized name, if valid.</returns>
    /// <exception cref="InvalidNameException"> Thrown if invalid name</exception>
    private string NormalizeValidateName(string name) 
    {
        if (string.IsNullOrEmpty(name) || !Regex.IsMatch(name, @"^[a-zA-Z]+[0-9]+$"))
        {
            throw new InvalidNameException();
        }
        else return name.ToUpper();
    }
}

