Spreadsheet Application (C# / .NET)

A modular spreadsheet engine built from scratch as part of the CS 3500: Software Practice course. The project implements the core functionality of a spreadsheet, including formula parsing, dependency tracking, and automatic recalculation, with full unit test coverage.

⸻

Core Features
	•	Formula parsing and evaluation
(supports +, -, *, /, parentheses, and variables)
	•	Directed dependency graph to track references between cells
	•	Circular dependency detection with safe error handling
	•	Efficient propagation of updates across dependent cells
	•	Persistent save/load of spreadsheet data
	•	GUI support for editing cells and viewing live updates

⸻

Project Structure

Formula Module</br>
Handles parsing and evaluation of expressions.</br>
Files:</br>
	•	Formula/Formula.cs

Dependency Graph Module
Manages cell relationships and supports cycle detection.</br>
Files:</br>
	•	DependencyGraph/DependencyGraph.cs

Spreadsheet Module
Implements the main spreadsheet logic (cell storage, recalculation, validation). </br>
Files:</br>
	•	Spreadsheet/Spreadsheet.cs

⸻

Testing

The project uses a test-driven development approach. Each module has its own dedicated test suite:</br>
	•	FormulaTests/</br>
	•	DependencyGraphTests/</br>
	•	SpreadsheetTests/</br>

Tests validate:</br>
	•	Parsing behavior</br>
	•	Evaluation correctness</br>
	•	Dependency resolution</br>
	•	Circular dependency detection</br>
	•	Update propagation</br>
	•	Spreadsheet state consistency</br>

⸻

Skills Demonstrated</br>
	•	Object-oriented design in C#</br>
	•	Expression parsing and evaluation</br>
	•	Graph algorithms (DAG maintenance, topological ordering)</br>
	•	Abstraction and modular system design</br>
	•	Test-Driven Development (TDD)</br>
	•	GUI development for interactive spreadsheet editing
