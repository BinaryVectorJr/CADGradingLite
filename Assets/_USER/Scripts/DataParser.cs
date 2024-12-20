using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

// Class Item to Store the Dataset
[System.Serializable]
public class AssignmentDataElement
{
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
    [SerializeField] TextAsset _assignmentDatasetFile;
    [SerializeField] TextAsset _rubricDatasetFile;

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
    public TextAsset assmDatasetFile {get => _assignmentDatasetFile; private set => _assignmentDatasetFile = value;}
    public string[] assmDatasetLines {get => _assmDatasetLines; private set => _assmDatasetLines = value;}
    public AssignmentDataElement[] assmDatasetElements {get => _assmDatasetElements; private set => _assmDatasetElements = value;}

    // Setting up the properties of the Rubric TextAsset, so as to make it read-only from external files
    public TextAsset rubricDatasetFile {get => _rubricDatasetFile; private set => _rubricDatasetFile = value;}
    public string[] rubricDatasetLines {get => _rubricDatasetLines; private set => _rubricDatasetLines = value;}
    public RubricDataElement[] rubricDatasetElements {get => _rubricDatasetElements; private set => _rubricDatasetElements = value;}

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
        ValidateAssignmentTextAsset();
        ValidateRubricTextAsset();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ValidateAssignmentTextAsset()
    {
        // From the loaded txt file, we extract one line using the NEWLINE delimiter, as a row, and set it into an array of lines

        assmDatasetLines = assmDatasetFile ? assmDatasetFile.text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries):null;

        assmLineCount = assmDatasetLines.Length;
        assmDatasetElements = new AssignmentDataElement[assmDatasetLines.Length];

        // For each entry in the line
        for (int i=0; i< assmLineCount; i++)
        {
            // Once line is extracted, further split into constituents using the "comma" delimiter, into an array
            var assmDatasetContentPart = assmDatasetLines[i].Split(',');

            // Validation query
            if(assmDatasetContentPart.Length % 6 != 0)
            {
                Debug.LogError("Atleast 6 comma separated values needed!"); //TODO - make this dynamic
                return;
            }

            // Set the fileds for each element of array of classes
            assmDatasetElements[i] = SetAssmDataElementValues(assmDatasetContentPart);
        }
    }

    void ValidateRubricTextAsset()
    {
        // From the loaded txt file, we extract one line using the NEWLINE delimiter, as a row, and set it into an array of lines

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

    AssignmentDataElement SetAssmDataElementValues(string[] assmDatasetLine)
    {
        return new AssignmentDataElement
        {
            week_no = int.TryParse(assmDatasetLine[0], out var x1) ? x1:0,
            assm_no = int.TryParse(assmDatasetLine[1], out var x2) ? x2:0,
            assm_name = assmDatasetLine[2],
            total_points = int.TryParse(assmDatasetLine[3], out var x3) ? x3:0,
            avail_date = assmDatasetLine[4],
            due_date = assmDatasetLine[5]
        };
    }

    RubricDataElement SetRubricDataElementValues(string[] rubricDatasetLine)
    {
        return new RubricDataElement
        {
            assm_type = int.TryParse(rubricDatasetLine[0], out var y1) ? y1:0,
            error_item_desc = rubricDatasetLine[1],
            error_item_total_points = float.TryParse(rubricDatasetLine[2], out var y2) ? y2:0,error_item_achieved_points = float.TryParse(rubricDatasetLine[3], out var y3) ? y3:0
        };
    }
}
