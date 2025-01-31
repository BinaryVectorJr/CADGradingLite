using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

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
public class RubricDataElement
{
    public int assm_type;
    public string error_item_desc;
    public float error_item_total_points;
    public float error_item_achieved_points;
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

    [Header("Misc")]
    public int assmLineCount = 0;
    public int rubricLineCount = 0;

    // Setting up the properties of the Assignment TextAsset, so as to make it read-only from external files
    public TextAsset assmDatasetFile; //{get => _assignmentDatasetFile; private set => _assignmentDatasetFile = value;}
    public string[] assmDatasetLines; //{get => _assmDatasetLines; private set => _assmDatasetLines = value;}
    public AssignmentDataElement[] assmDatasetElements; //{get => _assmDatasetElements; private set => _assmDatasetElements = value;}

    // Setting up the properties of the Rubric TextAsset, so as to make it read-only from external files
    public TextAsset rubricDatasetFile; //{get => _rubricDatasetFile; private set => _rubricDatasetFile = value;}
    public string[] rubricDatasetLines; //{get => _rubricDatasetLines; private set => _rubricDatasetLines = value;}
    public RubricDataElement[] rubricDatasetElements; //{get => _rubricDatasetElements; private set => _rubricDatasetElements = value;}

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
            due_date = assmDatasetLine[6]
        };
    }

    RubricDataElement SetRubricDataElementValues(string[] rubricDatasetLine)
    {
        return new RubricDataElement
        {
            assm_type = int.TryParse(rubricDatasetLine[0], out var y0) ? y0:0,  // Primary Key
            error_item_desc = rubricDatasetLine[1],
            error_item_total_points = float.TryParse(rubricDatasetLine[2], out var y1) ? y1:0,error_item_achieved_points = float.TryParse(rubricDatasetLine[3], out var y2) ? y2:0
        };
    }
}
