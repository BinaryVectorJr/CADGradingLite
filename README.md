# CADGradingLite
A bare-bones interactive grading tool, for the purpose of facilitating Instructors and Graders of a large scale engineering course at Purdue University.

This applet allows the Instructor to save the rubrics of assignments and the assignments each one associates to, in the form of a .TXT file with a CSV format, and use that to score their students.

The program then loads in this data dynamically at runtime, and populates the dataset for grading. Each panel, button, and screen (scene) is generated dynamically via script, and based on the assignment chosen. Grading values are pulled in from the prior mentioned dataset and populated in the panel, for ease of use. Once the instructor starts the process, they have interactive buttons to score each rubric item (providing deductions of 50%, 100%, or adding/subtracting 1 point), and the score is calculated real-time. At the end, a button formats the deductions in to easily readable feedback for students, and copies it to the clipboard, so that the instructor can paste it into the appropriate feedback zone of the Learning Management System they use.

The applet also provides a functionality to edit the data in the text files. If the files are detected by the app, then it pulls in the data from there, otherwise it creates the default .TXT files for use.

# Details
Created Using: Unity 2022.3.54f1
Final Build Size: 71 MB (28 MB zipped)

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
