# CADGradingLite
A bare-bones interactive grading tool, for the purpose of facilitating Instructors and Graders of a large scale engineering course at Purdue University.

This applet allows the Instructor to save the rubrics of assignments and the assignments each one associates to, in the form of a .TXT file with a CSV format, and use that to score their students.

The program then loads in this data dynamically at runtime, and populates the dataset for grading. Each panel, button, and screen (scene) is generated dynamically via script, and based on the assignment chosen. Grading values are pulled in from the prior mentioned dataset and populated in the panel, for ease of use. Once the instructor starts the process, they have interactive buttons to score each rubric item (providing deductions of 50%, 100%, or adding/subtracting 1 point), and the score is calculated real-time. At the end, a button formats the deductions in to easily readable feedback for students, and copies it to the clipboard, so that the instructor can paste it into the appropriate feedback zone of the Learning Management System they use.

The applet also provides a functionality to edit the data in the text files. If the files are detected by the app, then it pulls in the data from there, otherwise it creates the default .TXT files for use.

<br />

# Features
- Ability to load in assignments in the form of text files (which get combined to a common data structure in-app)
- Ability to keep track of multiple rubric item score and feedback, per part assignment and per assembly (max of 2 levels)
- Ability to copy final score and clearly formatted feedback, to help student's understanding
- Ability to open Assemblies along with their component parts, and grade all within one screen
- Hot-reload changes to assignment, rubric, and project data (at the click of a button)

<br />

# Usage
- Structure of rubric for assignment (grading-rubric.txt) per row:
    - id to associate with project type (primary key)
    - rubric item error description
    - total points associated with rubric item
    - total points achieved after grading the part
- Structure of weekly assignment (weekly-assms.txt) per row:
    - id to associate with project type (primary key)
    - week number
    - assignment number in that week
    - name of assignment
    - total points associated with assignment
    - CUSTOM ATTRIB: available date (unused)
    - CUSTOM ATTRIB: due date (unused)
- Strucutre of project assembly assignment (project.txt) per row:
    - project id where first 2 digits are week number (primary key)
    - project name
    - total number of component parts

<br />

# Details
- Created Using: Unity 2022.3.54f1 with Windows Build Support (IL2CPP)
- Final Build Size: 71 MB (28 MB zipped)
- Last Update: 25 April, 2025

<br />

# Package Dependencies
Packages Used:
- Addressables 1.22.3
- Custom NUnit 1.0.6
- Editor Coroutines 1.0.0
- Scriptable Build Pipeline 1.21.25
- Test Framework 1.1.33
- TextMeshPro 3.0.9
- Unity UI 1.0.0
- Visual Studio Code Editor 1.2.5

<br />

# Disclosure
ChatGPT was utilized to clear confusion and rapid iterate tests for some code block variants. Primary usage was to make LINQ code more efficient.