using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class RubricDataElement
{
    public int assm_type;
    public string error_item_desc;
    public float error_item_total_points;
    public float error_item_achieved_points;

    public RubricDataElement(int _assm_type, string _error_item_desc, float _error_item_total_points, float _error_item_achieved_points)
    {
        assm_type = _assm_type;
        error_item_desc = _error_item_desc;
        error_item_total_points = _error_item_total_points;
        error_item_achieved_points = _error_item_achieved_points;
    }
}

// [System.Serializable]
// public class RubricDataListL1
// {
//     public List<RubricDataElement> sameAssmRubrics;

//     public RubricDataListL1()
//     {
//         sameAssmRubrics = new List<RubricDataElement>();
//     }
// }

// Class Item to Store the Dataset
[System.Serializable]
public class AssignmentDataElement
{
    public int assm_type;
    public int week_no;
    public int assm_no;
    public string assm_name;
    public int total_points;
    public string avail_date;
    public string due_date;
}

[System.Serializable]
public class AssignmentWithRubric
{
    public AssignmentDataElement assignment_data;
    public RubricDataElement[] rubric_data;

    public AssignmentWithRubric(AssignmentDataElement _assm, RubricDataElement[] _rubs)
    {
        assignment_data = _assm;
        rubric_data = _rubs;
    }
}


[System.Serializable]
public class ProjectDataElement
{
    public int project_id;
    public string project_name;
    public int project_components_count;
    public string project_component_type;
    public float project_total_points;
    public AssignmentDataElement project_component_assignment_data;
}

[System.Serializable]
public class ProjectWithRubric
{
    public ProjectDataElement project_data;
    public RubricDataElement[] rubric_data;

    public ProjectWithRubric(ProjectDataElement _project,RubricDataElement[] _rubs)
    {
        project_data = _project;
        rubric_data = _rubs;
        
        for(int i=1; i<rubric_data.Count(); i++)
        {
            project_data.project_total_points += rubric_data[i].error_item_achieved_points;
        }
    }
}

public class DataParser : MonoBehaviour
{

    // Creating a singleton
    public static DataParser dpInstance;

    [Header("Required")]
    public TextAsset _assignmentDatasetFile;
    public TextAsset _rubricDatasetFile;
    public TextAsset _projectDatasetFile;

    [Header("Assignment Dataset")]
    [SerializeField] string[] _assmDatasetLines;
    [SerializeField] AssignmentDataElement[] _assmDatasetElements;

    [Header("Rubric Dataset")]
    [SerializeField] string[] _rubricDatasetLines;
    [SerializeField] RubricDataElement[] _rubricDatasetElements;

    [Header("Project Dataset")]
    [SerializeField] string[] _projectDatasetLines;
    [SerializeField] RubricDataElement[] _projectDatasetElements;

    [Header("Misc")]
    public int assmLineCount = 0;
    public int rubricLineCount = 0;
    public int projectLineCount = 0;

    // Setting up the properties of the Assignment TextAsset, so as to make it read-only from external files
    public TextAsset assmDatasetFile; //{get => _assignmentDatasetFile; private set => _assignmentDatasetFile = value;}
    public string[] assmDatasetLines; //{get => _assmDatasetLines; private set => _assmDatasetLines = value;}
    public AssignmentDataElement[] assmDatasetElements; //{get => _assmDatasetElements; private set => _assmDatasetElements = value;}

    // Setting up the properties of the Rubric TextAsset, so as to make it read-only from external files
    public TextAsset rubricDatasetFile; //{get => _rubricDatasetFile; private set => _rubricDatasetFile = value;}
    public string[] rubricDatasetLines; //{get => _rubricDatasetLines; private set => _rubricDatasetLines = value;}
    public RubricDataElement[] rubricDatasetElements; //{get => _rubricDatasetElements; private set => _rubricDatasetElements = value;}

    // Setting up the properties of the Project TextAsset, so as to make it read-only from external files
    public TextAsset projectDatasetFile; //{get => _rubricDatasetFile; private set => _rubricDatasetFile = value;}
    public string[] projectDatasetLines; //{get => _rubricDatasetLines; private set => _rubricDatasetLines = value;}
    public ProjectDataElement[] projectDatasetElements; //{get => _rubricDatasetElements; private set => _rubricDatasetElements = value;}

    // [SerializeField]
    // public List<RubricDataElement> SameAssignmentTypeRubricsL1 = new List<RubricDataElement>();

    // [SerializeField]
    // public RubricDataListL1 SameAssignmentTypeRubricsL1 = new RubricDataListL1();

    // // [SerializeField]
    // // public List<List<RubricDataElement>> TypeRubricsL0 = new List<List<RubricDataElement>>();

    // [SerializeField]
    // public List<RubricDataListL1> TypeRubricsL0 = new List<RubricDataListL1>();

    [SerializeField]
    public List<AssignmentWithRubric> assignmentWithRubricElements = new List<AssignmentWithRubric>();

    [SerializeField]
    public List<ProjectWithRubric> projectWithRubricElements = new List<ProjectWithRubric>();

    [SerializeField]
    public RubricDataElement[][] groupedRubricsByType;

    [SerializeField]
    public RubricDataElement[][] finalGroupedArray;

    // Creating a singleton
    private void Awake()
    {
        if(dpInstance != null && dpInstance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            dpInstance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnValidate()
    {
        //ValidateAssignmentTextAsset();
        //ValidateRubricTextAsset();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // RUBRIC

    public void ValidateRubricTextAsset(TextAsset _rubricDatasetFile)
    {
        // From the loaded txt file, we extract one line using the NEWLINE delimiter, as a row, and set it into an array of lines

        rubricDatasetFile = _rubricDatasetFile;
        rubricDatasetLines = rubricDatasetFile ? rubricDatasetFile.text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries):null;

        rubricLineCount = rubricDatasetLines.Length;
        rubricDatasetElements = new RubricDataElement[rubricDatasetLines.Length];

        // For each entry in the line
        for (int i=0; i< rubricLineCount; i++)
        {
            // Once line is extracted, further split into constituents using the "comma" delimiter, into an array
            var rubricDatasetContentPart = rubricDatasetLines[i].Split(',');

            // Validation query
            if(rubricDatasetContentPart.Length % 4 != 0)
            {
                Debug.LogError("Atleast 4 comma separated values needed!"); //TODO - make this dynamic
                return;
            }

            // Set the fileds for each element of array of classes
            rubricDatasetElements[i] = SetRubricDataElementValues(rubricDatasetContentPart);
        }
    }

    public void ValidateRubricTextAsset(string[] _rubricDatasetLines)
    {
        rubricLineCount = _rubricDatasetLines.Length;
        rubricDatasetElements = new RubricDataElement[_rubricDatasetLines.Length];

        // For each entry in the line
        for (int i=0; i< rubricLineCount; i++)
        {
            // Once line is extracted, further split into constituents using the "comma" delimiter, into an array
            var rubricDatasetContentPart = _rubricDatasetLines[i].Split(',');

            // Validation query
            if(rubricDatasetContentPart.Length % 4 != 0)
            {
                Debug.LogError("Atleast 4 comma separated values needed!"); //TODO - make this dynamic
                return;
            }

            // Set the fileds for each element of array of classes
            rubricDatasetElements[i] = SetRubricDataElementValues(rubricDatasetContentPart);
        }

        Dictionary<int, List<RubricDataElement>> groupedRubricsDict = new Dictionary<int, List<RubricDataElement>>();

        // Group elements by assm_type
        foreach(var item in rubricDatasetElements)
        {
            if(!groupedRubricsDict.ContainsKey(item.assm_type))
            {
                groupedRubricsDict[item.assm_type] = new List<RubricDataElement>();
            }
            groupedRubricsDict[item.assm_type].Add(item);
        }

        // Sort Dictionary
        List<int> sortedKeys = new List<int>(groupedRubricsDict.Keys);
        sortedKeys.Sort();

        // Convert grouped dictionary to final array
        finalGroupedArray = new RubricDataElement[sortedKeys.Count][];
        for(int i=0; i< sortedKeys.Count; i++)
        {
            finalGroupedArray[i] = groupedRubricsDict[sortedKeys[i]].ToArray();
        }

        // // Display the result in the console
        // foreach (var group in finalGroupedArray)
        // {
        //     Debug.Log("Group with assm type = " + group[0].assm_type + ":");
        //     foreach (var item in group)
        //     {
        //         Debug.Log("  assm type = " + item.assm_type);
        //     }
        // }

    }

    public void RubricSetter()
    {
        if(assignmentWithRubricElements.Count == 0)
        {
            for(int i = 0; i<assmDatasetElements.Length; i++)
            {
                assignmentWithRubricElements.Add(new AssignmentWithRubric(assmDatasetElements[i],finalGroupedArray[assmDatasetElements[i].assm_type]));
            }
        }

        if(projectWithRubricElements.Count == 0)
        {
            for(int i = 0; i<projectDatasetElements.Length; i++)
            {
                projectWithRubricElements.Add(new ProjectWithRubric(projectDatasetElements[i],finalGroupedArray[projectDatasetElements[i].project_component_assignment_data.assm_type]));
            }
        }
    }

    // ASSIGNMENT

    public void ValidateAssignmentTextAsset(TextAsset _assignmentDatasetFile)
    {
        // From the loaded txt file, we extract one line using the NEWLINE delimiter, as a row, and set it into an array of lines

        assmDatasetFile = _assignmentDatasetFile;
        assmDatasetLines = assmDatasetFile ? assmDatasetFile.text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries):null;

        assmLineCount = assmDatasetLines.Length;
        assmDatasetElements = new AssignmentDataElement[assmDatasetLines.Length];

        // For each entry in the line
        for (int i=0; i< assmLineCount; i++)
        {
            // Once line is extracted, further split into constituents using the "comma" delimiter, into an array
            var assmDatasetContentPart = assmDatasetLines[i].Split(',');

            // Validation query
            if(assmDatasetContentPart.Length % 7 != 0)
            {
                Debug.LogError("Atleast 7 comma separated values needed!"); //TODO - make this dynamic
                return;
            }

            // Set the fileds for each element of array of classes
            assmDatasetElements[i] = SetAssmDataElementValues(assmDatasetContentPart);
        }
        
    }

    public void ValidateAssignmentTextAsset(string[] _assmDatasetLines)
    {
        assmLineCount = _assmDatasetLines.Length;
        assmDatasetElements = new AssignmentDataElement[_assmDatasetLines.Length];

        // For each entry in the line
        for (int i=0; i< assmLineCount; i++)
        {
            // Once line is extracted, further split into constituents using the "comma" delimiter, into an array
            var assmDatasetContentPart = _assmDatasetLines[i].Split(',');

            // Validation query
            if(assmDatasetContentPart.Length % 7 != 0)
            {
                Debug.LogError("Atleast 7 comma separated values needed!"); //TODO - make this dynamic
                return;
            }

            // Set the fileds for each element of array of classes
            assmDatasetElements[i] = SetAssmDataElementValues(assmDatasetContentPart);
        }
    }

    // PROJECT

    public void ValidateProjectTextAsset(TextAsset _projectDatasetFile)
    {
        // From the loaded txt file, we extract one line using the NEWLINE delimiter, as a row, and set it into an array of lines

        projectDatasetFile = _projectDatasetFile;
        projectDatasetLines = projectDatasetFile ? projectDatasetFile.text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries):null;

        projectLineCount = projectDatasetLines.Length;
        projectDatasetElements = new ProjectDataElement[projectDatasetLines.Length];

        // For each entry in the line
        for (int i=0; i< projectLineCount; i++)
        {
            // Once line is extracted, further split into constituents using the "comma" delimiter, into an array
            var projectDatasetContentPart = projectDatasetLines[i].Split(',');

            // Validation query
            if(projectDatasetContentPart.Length % 12 != 0)
            {
                Debug.LogError("Atleast 12 comma separated values needed!"); //TODO - make this dynamic
                return;
            }

            // Set the fileds for each element of array of classes
            projectDatasetElements[i] = SetProjectDataElementValues(projectDatasetContentPart);
        }
    }

    public void ValidateProjectTextAsset(string[] _projectDatasetLines)
    {
        projectLineCount = _projectDatasetLines.Length;
        projectDatasetElements = new ProjectDataElement[_projectDatasetLines.Length];

        // For each entry in the line
        for (int i=0; i< projectLineCount; i++)
        {
            // Once line is extracted, further split into constituents using the "comma" delimiter, into an array
            var projectDatasetContentPart = _projectDatasetLines[i].Split(',');

            // Validation query
            if(projectDatasetContentPart.Length % 12 != 0)
            {
                Debug.LogError("Atleast 12 comma separated values needed!"); //TODO - make this dynamic
                return;
            }

            // Set the fileds for each element of array of classes
            projectDatasetElements[i] = SetProjectDataElementValues(projectDatasetContentPart);
        }
    }

    // public List<RubricDataElement> tempSameAssmRubrics = new List<RubricDataElement>();
    // public void ValidateAssignmentWithRubrics()
    // {
    //     int rubricIndex = 0;
    //     foreach(AssignmentDataElement assm in assmDatasetElements)
    //     {
    //         tempSameAssmRubrics.Clear();
    //         // foreach(RubricDataElement rub in rubricDatasetElements)
    //         // {
    //         //     if(assm.assm_type == rub.assm_type)
    //         //     {
    //         //         tempSameAssmRubrics.Add(rub);
    //         //     }
    //         // }

    //         // for(int i=0; i<rubricDatasetElements.Length; i++)
    //         // {
    //         //     if(assm.assm_type == rubricDatasetElements[i].assm_type)
    //         //     {
    //         //         tempSameAssmRubrics.Add(rubricDatasetElements[i]);
    //         //     }
    //         // }

    //         assignmentWithRubricElements.Add(SetAssignmentWithRubricValues(assm,tempSameAssmRubrics));

    //         rubricIndex++;
    //     }

    //     if(assignmentWithRubricElements.)
    //     {
    //         if(assignmentWithRubricElements == rubricDatasetElements.)
    //         assmr.rubric_data.AddRange(rubricDatasetElements.Where(n=>n.assm_type == assmr.assignment_data.assm_type));
    //     }

    //     // foreach(AssignmentWithRubric assmr in assignmentWithRubricElements)
    //     // {
    //     //     tempSameAssmRubrics.Clear();
    //     //     foreach(RubricDataElement rub in rubricDatasetElements)
    //     //     {
    //     //         if(assmr.assignment_data.assm_type == rub.assm_type)
    //     //         {
    //     //             assmr.rubric_data.Add(rub);
    //     //         }
    //     //     }

    //     //     // for(int i=0; i<rubricDatasetElements.Length; i++)
    //     //     // {
    //     //     //     if(assm.assm_type == rubricDatasetElements[i].assm_type)
    //     //     //     {
    //     //     //         tempSameAssmRubrics.Add(rubricDatasetElements[i]);
    //     //     //     }
    //     //     // }

    //     //     // assignmentWithRubricElements.Add(SetAssignmentWithRubricValues(assm,tempSameAssmRubrics));

    //     //     // rubricIndex++;
    //     // }
    // }

    // SETTING VALUES

    RubricDataElement SetRubricDataElementValues(string[] rubricDatasetLine)
    {
        return new RubricDataElement(
            int.TryParse(rubricDatasetLine[0], out var y0) ? y0:0,  // Primary Key
            rubricDatasetLine[1],
            float.TryParse(rubricDatasetLine[2], out var y1) ? y1:0, 
            float.TryParse(rubricDatasetLine[3], out var y2) ? y2:0)
        {
            // assm_type = int.TryParse(rubricDatasetLine[0], out var y3) ? y3:0,  // Primary Key
            // error_item_desc = rubricDatasetLine[1],
            // error_item_total_points = float.TryParse(rubricDatasetLine[2], out var y4) ? y4:0
            // error_item_achieved_points = float.TryParse(rubricDatasetLine[3], out var y5) ? y5:0
            
            assm_type = y0,
            error_item_desc = rubricDatasetLine[1],
            error_item_total_points = y1,
            error_item_achieved_points = y2
        };
    }

    AssignmentDataElement SetAssmDataElementValues(string[] assmDatasetLine)
    {
        return new AssignmentDataElement
        {
            assm_type = int.TryParse(assmDatasetLine[0], out var x0) ? x0:0,    // Primary Key
            week_no = int.TryParse(assmDatasetLine[1], out var x1) ? x1:0,
            assm_no = int.TryParse(assmDatasetLine[2], out var x2) ? x2:0,
            assm_name = assmDatasetLine[3],
            total_points = int.TryParse(assmDatasetLine[4], out var x3) ? x3:0,
            avail_date = assmDatasetLine[5],
            due_date = assmDatasetLine[6],
        };
    }

    AssignmentWithRubric SetAssignmentWithRubricValues(AssignmentDataElement _assm, RubricDataElement[] _rubric)
    {
        return new AssignmentWithRubric(_assm,_rubric)
        {
            assignment_data = _assm,
            rubric_data = _rubric
        };

    }

    ProjectDataElement SetProjectDataElementValues(string[] projectDatasetLine)
    {
        return new ProjectDataElement
        {
            project_id = int.TryParse(projectDatasetLine[0], out var z0) ? z0:0,  // Primary Key
            project_name = projectDatasetLine[1],
            project_components_count = int.TryParse(projectDatasetLine[2], out var z1) ? z1:0,
            project_component_type = projectDatasetLine[3],
            project_total_points = float.TryParse(projectDatasetLine[4], out var z2) ? z2:0,
            project_component_assignment_data = new AssignmentDataElement
            {
                assm_type = int.TryParse(projectDatasetLine[5], out var z3) ? z3:0,    // Primary Key of assignment
                week_no = int.TryParse(projectDatasetLine[6], out var z4) ? z4:0,
                assm_no = int.TryParse(projectDatasetLine[7], out var z5) ? z5:0,
                assm_name = projectDatasetLine[8],
                total_points = int.TryParse(projectDatasetLine[9], out var z6) ? z6:0,
                avail_date = projectDatasetLine[10],
                due_date = projectDatasetLine[11]
            }
        };
    }

    //     // Function to display the contents of var2
    // void DisplayNestedContents()
    // {
    //     // // Iterate through var2 and display each group
    //     // foreach (var group in TypeRubricsL0)
    //     // {
    //     //     string groupContents = string.Join(", ", group.Select(item => item.assm_type.ToString()));
    //     //     Debug.Log("Group: " + groupContents);
    //     // }

    //     for (int i = 0; i < TypeRubricsL0.Count; i++)
    //     {
    //         // Display the index of the outer list
    //         Debug.Log($"Outer List {i + 1}:");

    //         // Get the inner List<MyClass>
    //         // List<RubricDataElement> innerList = TypeRubricsL0[i];

    //         RubricDataListL1 innerList = TypeRubricsL0[i];

    //         // Iterate through each element in the inner list and display their values
    //         for (int j = 0; j < innerList.sameAssmRubrics.Count; j++)
    //         {
    //             RubricDataElement item = innerList.sameAssmRubrics[j];
    //             Debug.Log($"    Element {j + 1}: a = {item.assm_type}, b = {item.error_item_desc}");
    //         }
    //     }
    // }
}
