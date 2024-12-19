using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

// Class Item to Store the Dataset
[System.Serializable]
public class DataElement
{
    public int week_no;
    public int assm_no;
    public string assm_name;
    public int total_points;
    public string avail_date;
    public string due_date;
}

public class DataParser : MonoBehaviour
{

    // Creating a singleton
    public static DataParser dpInstance;

    [Header("Required")]
    [SerializeField] TextAsset _datasetFile;

    [SerializeField] string[] _datasetLines;
    [SerializeField] DataElement[] _datasetElements;

    public int lineCount = 0;

    // Setting up the properties of the TextAsset, so as to make it read-only from external files
    public TextAsset datasetFile {get => _datasetFile; private set => _datasetFile = value;}
    public string[] datasetLines {get => _datasetLines; private set => _datasetLines = value;}
    public DataElement[] datasetElements {get => _datasetElements; private set => _datasetElements = value;}

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
        // From the loaded txt file, we extract one line using the NEWLINE delimiter, as a row, and set it into an array of lines

        datasetLines = datasetFile ? datasetFile.text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries):null;

        lineCount = datasetLines.Length;
        datasetElements = new DataElement[datasetLines.Length];

        // For each entry in the line
        for (int i=0; i< lineCount; i++)
        {
            // Once line is extracted, further split into constituents using the "comma" delimiter, into an array
            var datasetContentPart = datasetLines[i].Split(',');

            // Validation query
            if(datasetContentPart.Length % 6 != 0)
            {
                Debug.LogError("Atleast 6 comma separated values needed!"); //TODO - make this dynamic
                return;
            }

            // Set the fileds for each element of array of classes
            datasetElements[i] = SetDataElementValues(datasetContentPart);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    DataElement SetDataElementValues(string[] datasetLine)
    {
        return new DataElement
        {
            week_no = int.TryParse(datasetLine[0], out var x1) ? x1:0,
            assm_no = int.TryParse(datasetLine[1], out var x2) ? x2:0,
            assm_name = datasetLine[2],
            total_points = int.TryParse(datasetLine[3], out var x3) ? x3:0,
            avail_date = datasetLine[4],
            due_date = datasetLine[5]
        };
    }
}
