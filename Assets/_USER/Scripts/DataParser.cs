using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// WORKING
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

// Class Item to Store the Dataset
// WORKS
[System.Serializable]
public class AssignmentDataElement
{
    public int assm_id;
    public int assm_type;
    public string week_no;
    public string assm_no;
    public string assm_name;

    private int assm_total;
    private int assm_achieved;

    public int assm_total_points
    {
        get
        {
            int sum = 0;

            for(int i=0; i<assm_rubrics.Count; i++)
            {
                sum += int.Parse(assm_rubrics[i].error_item_total_points.ToString());
            }

            return sum;
        }
        set
        {
            assm_total = value;
        }
    }
    public int assm_achieved_points
    {
        get
        {
            int sum = 0;

            for(int i=0; i<assm_rubrics.Count; i++)
            {
                sum += int.Parse(assm_rubrics[i].error_item_achieved_points.ToString());
            }

            return sum;
        }
        set
        {
            assm_achieved = value;
        }
    }

    public string avail_date;
    public string due_date;

    public List<RubricDataElement> assm_rubrics = new List<RubricDataElement>();
}

// NEW
[System.Serializable]
public class ProjectBaseElement
{
    public int project_id;
    public string project_name;
    public int project_components_count;
    public List<AssignmentDataElement> project_associated_assignments = new List<AssignmentDataElement>();

    public float project_total_points
    {
        get
        {
            int sumRub = 0;
            int sumTot = 0;

            for(int i=0; i<project_associated_assignments.Count; i++)
            {
                for(int j=0; j<project_associated_assignments[i].assm_rubrics.Count; j++)
                {
                    sumRub += int.Parse(project_associated_assignments[i].assm_rubrics[j].error_item_total_points.ToString());
                }
                sumTot += sumRub;
            }

            return sumTot;
        }
    }
    public int project_achieved_points
    {
        get
        {
            int sumRub = 0;
            int sumTot = 0;

            for(int i=0; i<project_associated_assignments.Count; i++)
            {
                for(int j=0; j<project_associated_assignments[i].assm_rubrics.Count; j++)
                {
                    sumRub += int.Parse(project_associated_assignments[i].assm_rubrics[j].error_item_achieved_points.ToString());
                }
                sumTot += sumRub;
            }

            return sumTot;
        }
    }


}

public class DataParser : MonoBehaviour
{

    // Creating a singleton
    public static DataParser dpInstance;

    [Header("Required")]
    [HideInInspector] public TextAsset _assignmentDatasetFile;
    [HideInInspector] public TextAsset _rubricDatasetFile;
    [HideInInspector] public TextAsset _projectDatasetFile;

    [Header("Misc")]
    public int assmLineCount = 0;
    public int rubricLineCount = 0;
    public int projectLineCount = 0;

    // Setting up the properties of the Assignment TextAsset, so as to make it read-only from external files
    [HideInInspector] public TextAsset assmDatasetFile;
    [HideInInspector] public string[] assmDatasetLines;
    public AssignmentDataElement[] assmDatasetElements;

    // Setting up the properties of the Rubric TextAsset, so as to make it read-only from external files
    [HideInInspector] public TextAsset rubricDatasetFile;
    [HideInInspector] public string[] rubricDatasetLines;
    public RubricDataElement[] rubricDatasetElements;

    // Setting up the properties of the Project TextAsset, so as to make it read-only from external files
    [HideInInspector] public TextAsset projectDatasetFile;
    [HideInInspector] public string[] projectDatasetLines;

    // NEW BASE
    public ProjectBaseElement[] projectDatasetElements;

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
                // VALIDATED WORKING: Debug.LogError("Atleast 4 comma separated values needed!"); //TODO - make this dynamic
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
                // VALIDATED WORKING: Debug.LogError("Atleast 4 comma separated values needed!"); //TODO - make this dynamic
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
            assmDatasetElements[i] = SetAssmDataElementValues(assmDatasetContentPart,i);

            if(assmDatasetElements[i].assm_rubrics.Count == 0)
            {
                assmDatasetElements[i].assm_rubrics.AddRange(rubricDatasetElements.Where(x=>x.assm_type == assmDatasetElements[i].assm_type));
            }
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
            assmDatasetElements[i] = SetAssmDataElementValues(assmDatasetContentPart,i);

            if(assmDatasetElements[i].assm_rubrics.Count == 0)
            {
                assmDatasetElements[i].assm_rubrics.AddRange(rubricDatasetElements.Where(x=>x.assm_type == assmDatasetElements[i].assm_type));
            }
        }
    }

    // PROJECT

    public void ValidateProjectTextAsset(TextAsset _projectDatasetFile)
    {
        // From the loaded txt file, we extract one line using the NEWLINE delimiter, as a row, and set it into an array of lines

        projectDatasetFile = _projectDatasetFile;
        projectDatasetLines = projectDatasetFile ? projectDatasetFile.text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries):null;

        projectLineCount = projectDatasetLines.Length;
        projectDatasetElements = new ProjectBaseElement[projectDatasetLines.Length];

        // For each entry in the line
        for (int i=0; i< projectLineCount; i++)
        {
            // Once line is extracted, further split into constituents using the "comma" delimiter, into an array
            var projectDatasetContentPart = projectDatasetLines[i].Split(',');

            // Validation query
            // if(projectDatasetContentPart.Length % 12 != 0)
            // {
            //     Debug.LogError("Atleast 12 comma separated values needed!"); //TODO - make this dynamic
            //     return;
            // }

            // Set the fileds for each element of array of classes
            projectDatasetElements[i] = SetProjectDataElementValues2(projectDatasetContentPart);

            projectDatasetElements[i].project_associated_assignments.AddRange(assmDatasetElements.Where(x=>x.assm_name.Contains(projectDatasetElements[i].project_name)));
        }
    }

    public void ValidateProjectTextAsset(string[] _projectDatasetLines)
    {
        projectLineCount = _projectDatasetLines.Length;
        //projectDatasetElements = new ProjectDataElement[_projectDatasetLines.Length]; //OLD
        projectDatasetElements = new ProjectBaseElement[_projectDatasetLines.Length];

        // For each entry in the line
        for (int i=0; i< projectLineCount; i++)
        {
            // Once line is extracted, further split into constituents using the "comma" delimiter, into an array
            var projectDatasetContentPart = _projectDatasetLines[i].Split(',');

            // Validation query
            // if(projectDatasetContentPart.Length % 12 != 0)
            // {
            //     Debug.LogError("Atleast 12 comma separated values needed!"); //TODO - make this dynamic
            //     return;
            // }

            // Set the fileds for each element of array of classes OLD
            projectDatasetElements[i] = SetProjectDataElementValues2(projectDatasetContentPart);

            if(projectDatasetElements[i].project_associated_assignments.Count == 0)
            {
                // VERIFIED WORKING: Debug.Log("HEHRE");
                string tempAssm = projectDatasetElements[i].project_name;

                projectDatasetElements[i].project_associated_assignments.AddRange(assmDatasetElements.Where(x=>x.assm_name.Contains(tempAssm)));
            }
        }
    }

    // SETTING VALUES
    // WORKS
    RubricDataElement SetRubricDataElementValues(string[] rubricDatasetLine)
    {
        return new RubricDataElement(
            int.TryParse(rubricDatasetLine[0], out var y0) ? y0:0,  // Primary Key
            rubricDatasetLine[1],
            float.TryParse(rubricDatasetLine[2], out var y1) ? y1:0, 
            float.TryParse(rubricDatasetLine[3], out var y2) ? y2:0)
        {
            assm_type = y0,
            error_item_desc = rubricDatasetLine[1],
            error_item_total_points = y1,
            error_item_achieved_points = y2
        };
    }

    // WORKS
    AssignmentDataElement SetAssmDataElementValues(string[] assmDatasetLine, int _id)
    {
        return new AssignmentDataElement
        {
            assm_id = _id,
            assm_type = int.TryParse(assmDatasetLine[0], out var x0) ? x0:0,    // Primary Key
            week_no = assmDatasetLine[1],
            assm_no = assmDatasetLine[2],
            assm_name = assmDatasetLine[3],
            avail_date = assmDatasetLine[5],
            due_date = assmDatasetLine[6],
        };
    }

    // NEW
    ProjectBaseElement SetProjectDataElementValues2(string[] projectDatasetLine)
    {
        return new ProjectBaseElement
        {
            project_id = int.TryParse(projectDatasetLine[0], out var z0) ? z0:0,
            project_name = projectDatasetLine[1],
            project_components_count = int.TryParse(projectDatasetLine[2], out var z1) ? z1:0
        };
    }
}
