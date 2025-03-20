using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class RubricManager : MonoBehaviour
{
    public static RubricManager rbmInstance {get; private set;}

    [System.Serializable]
    public class SerializableKeyValuePair
    {
        public string key;        // The key (string)
        public List<int> value;   // The value (List<int>)
    }

    [SerializeField]
    private GameObject rubricParentModal;

    [SerializeField]
    public GameObject rubricPrefab;

    // [SerializeField]
    // public GameObject rubricPanelPrefab;

    //[SerializeField]
    //public GameObject rubricPanelPrefabParent;

    [SerializeField]
    public GameObject rubricItemParent;

    [SerializeField]
    public GameObject projectItemParent;

    [SerializeField]
    public TMP_Dropdown rubricTypeDropdown;

    [SerializeField]
    public Button rubricTotalScoreButton;

    [SerializeField]
    public List<Button> allRubricTotalScoresButtonProject;

    [SerializeField]
    public Button totalAssignmentScoreButton;

    [SerializeField]
    public Button rubricFinalFeedback;

    [SerializeField]
    public List<GameObject> allAssignmentButtonsProject;

    [SerializeField]
    public List<List<int>> allAssignmentAchievedScoresProject;

    public Button currentAssignmentButton;
    
    public Button prevAssignmentButton;

    public List<int> currentScores;

    [SerializeField]
    public Dictionary<string, List<int>> currentScoresProjectDict = new Dictionary<string, List<int>>();

    [SerializeField]
    public List<SerializableKeyValuePair> currentScoresProjectDict2;

    public List<string> currentFeedback;

    [System.Serializable]
    public class ProjectComponentScoresElement
    {
        public int component_id;
        public int associated_panel_id;
        public List<int> component_score;

        public ProjectComponentScoresElement(int _componentID, int _panelID, List<int> _scores)
        {
            component_id = _componentID;
            associated_panel_id = _panelID;
            component_score = _scores;
        }
    }

    [SerializeField]
    public List<ProjectComponentScoresElement> currentComponentScores;

    public int currentSum = 0;

    [SerializeField]
    public ProjectBaseElement MasterActiveProjectComponent;

    void Awake()
    {
        if(rbmInstance != null && rbmInstance != this)
        {
            Destroy(this);
        }
        else
        {
            rbmInstance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            ChangeRubrics(null);
        }
        else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            //ChangeRubricsProject(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            UpdateScore();
        }
        else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            UpdateScoreProject(0);
        }
    }

    public void ChangeRubrics(GameObject _button)
    {
        // Each panel has a temporary ID when it is generated and is on-screen
        // This enables us to assign dynamic IDs to each panel, as the rubrics update based on assignment type
        int currentPanelID = 0;
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            ClearPanel(1,1);
            int tempCountRubricItems = DataParser.dpInstance.rubricLineCount;
            GameObject tempRubricGO = null;

            for(int i=0; i<tempCountRubricItems; i++)
            {
                if(DataParser.dpInstance.rubricDatasetElements[i].assm_type == rubricTypeDropdown.value)
                {
                    tempRubricGO = GameObject.Instantiate(rubricPrefab);
                    tempRubricGO.GetComponent<CurrentRubricPanel>().panelID = currentPanelID;
                    currentPanelID++;   // Updating the IDs based on order of generation
                    tempRubricGO.GetComponent<CurrentRubricPanel>().ErrorDesc.text = DataParser.dpInstance.rubricDatasetElements[i].error_item_desc.ToString();
                    tempRubricGO.GetComponent<CurrentRubricPanel>().ErrorTotal.text = DataParser.dpInstance.rubricDatasetElements[i].error_item_total_points.ToString();
                    tempRubricGO.GetComponent<CurrentRubricPanel>().ErrorAchieved.text = DataParser.dpInstance.rubricDatasetElements[i].error_item_achieved_points.ToString();
                    tempRubricGO.transform.name = DataParser.dpInstance.rubricDatasetElements[i].error_item_desc.ToString();
                    tempRubricGO.transform.SetParent(rubricParentModal.transform);
                    tempRubricGO = null;
                    currentScores.Add(Mathf.CeilToInt(DataParser.dpInstance.rubricDatasetElements[i].error_item_achieved_points));
                }
            }
        }
    }

    // public void ChangeRubricsProject(GameObject _button)
    // {
    //     // Each panel has a temporary ID when it is generated and is on-screen
    //     // This enables us to assign dynamic IDs to each panel, as the rubrics update based on assignment type
    //     int currentPanelID = 0;
    //     if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
    //     {
    //         if(_button == null)
    //         {
    //             return;
    //         }

    //         ClearPanel(0,0);
    //         // Index will always be 0 as the button only is associated with one assignment type and rubric set for that assignment type
    //         int tempCountRubricItems2 = _button.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data.Count();
    //         GameObject tempRubricGO = null;

    //         for(int i=0; i<tempCountRubricItems2; i++)
    //         {
    //             //if(_button.GetComponent<AssignmentType>().assignmentTypeCode == rubricTypeDropdown.value)
    //             {
    //                 tempRubricGO = GameObject.Instantiate(rubricPrefab);
    //                 tempRubricGO.GetComponent<CurrentRubricPanel>().panelID = currentPanelID;
    //                 currentPanelID++;   // Updating the IDs based on order of generation

    //                 tempRubricGO.GetComponent<CurrentRubricPanel>().ErrorDesc.text = _button.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[i].error_item_desc;
    //                 tempRubricGO.GetComponent<CurrentRubricPanel>().ErrorTotal.text = _button.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[i].error_item_total_points.ToString();
    //                 tempRubricGO.GetComponent<CurrentRubricPanel>().ErrorAchieved.text = _button.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[i].error_item_achieved_points.ToString();
    //                 tempRubricGO.transform.name = _button.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[i].error_item_desc;
                    
    //                 //DataParser.dpInstance.rubricDatasetElements[i].error_item_desc.ToString();
    //                 //tempRubricGO.GetComponent<CurrentRubricPanel>().ErrorTotal.text = DataParser.dpInstance.rubricDatasetElements[i].error_item_total_points.ToString();
    //                 //tempRubricGO.GetComponent<CurrentRubricPanel>().ErrorAchieved.text = DataParser.dpInstance.rubricDatasetElements[i].error_item_achieved_points.ToString();
    //                 //tempRubricGO.transform.name = DataParser.dpInstance.rubricDatasetElements[i].error_item_desc.ToString();

    //                 tempRubricGO.transform.SetParent(rubricParentModal.transform);
    //                 tempRubricGO = null;

    //                 // currentScoresProjectDict.Add(_button.GetComponent<AssignmentType>().projectID.ToString(), new List<int> {Mathf.CeilToInt(_button.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[i].error_item_achieved_points)});

    //                 if (!currentScoresProjectDict2.Exists(pair => pair.key == _button.GetComponent<AssignmentType>().associatedProject.ToString()))
    //                 {
    //                     currentScoresProjectDict2.Add(new SerializableKeyValuePair 
    //                                                     {
    //                                                         key = _button.GetComponent<AssignmentType>().associatedProject.ToString(), 
    //                                                         value = new List<int> 
    //                                                         {
    //                                                             Mathf.CeilToInt(_button.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[i].error_item_achieved_points)
    //                                                         }
    //                                                     }
    //                                                 );
    //                 }
                    
    //             }
    //         }
    //     }
    // }

    public void ClearPanel(int _clearScores, int _clearFeedback)
    {
        foreach(Transform child in rubricParentModal.transform)
        {
            Destroy(child.gameObject);
        }

        if(_clearScores == 1)
        {
            currentScores.Clear();
        }

        if(_clearFeedback == 1)
        {
            currentFeedback.Clear();
        }
    }

    public void UpdateScore()
    {
        int sum = 0;

        foreach(Transform child in rubricParentModal.transform)
        {
            GameObject res2 = FindTagInHierarchy(child.transform,"AchievedScore");

            if(res2 != null)
            {
                sum += Mathf.CeilToInt(float.TryParse(res2.GetComponent<TMP_Text>().text, out var y1) ? y1:0);
            }
            else
            {
                sum += 0;
            }
            // foreach(Transform childchild in child)
            // {
                
            //     // if(childchild.tag == "AchievedScore")
            //     // {
            //     //     sum += Mathf.CeilToInt(float.TryParse(childchild.GetComponent<TMP_Text>().text, out var y1) ? y1:0);
            //     // }
            // }
        }

        rubricTotalScoreButton.GetComponentInChildren<TMP_Text>().text = sum.ToString();
        currentSum = sum;
    }

    public void UpdateScoreProject(int _achievedScore)
    {
        int currentRubricSum = 0;

        foreach(Transform child in rubricParentModal.transform)
        {
            GameObject res2 = FindTagInHierarchy(child.transform,"AchievedScore");

            if(res2 != null)
            {
                currentRubricSum += Mathf.CeilToInt(float.TryParse(res2.GetComponent<TMP_Text>().text, out var y1) ? y1:0);
            }
            else
            {
                currentRubricSum += 0;
            }
        }

        // rubricTotalScoreButton.GetComponentInChildren<TMP_Text>().text = (int.TryParse(rubricTotalScoreButton.GetComponentInChildren<TMP_Text>().text, out var y2) ? y2:0 - currentSum).ToString();

        rubricTotalScoreButton.GetComponentInChildren<TMP_Text>().text = currentRubricSum.ToString();
        currentSum = currentRubricSum;

    }

    GameObject FindTagInHierarchy(Transform _parentTransform, string _tag)
    {
        if(_parentTransform.CompareTag(_tag))
        {
            return _parentTransform.gameObject;
        }

        // Recursion check
        foreach (Transform child in _parentTransform)
        {
            GameObject result = FindTagInHierarchy(child,_tag);
            if(result != null)
            {
                return result;      // Return as soon as gameobject with tag was found
            }
        }

        // If not found return null
        return null;
    }

    public void CopyScoreToClipboard()
    {
        // Copy the integer score text to the clipboard
        GUIUtility.systemCopyBuffer = currentSum.ToString();
    }

    public void CopyFeedbackToClipboard()
    {
        // Join the list items into a single string, separated by newlines
        string joinedText = string.Join("\n", currentFeedback);

        // Copy the joined text to the clipboard
        GUIUtility.systemCopyBuffer = joinedText;
    }
}
