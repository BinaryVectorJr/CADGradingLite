using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public ProjectBaseElement MasterActiveProjectSubsetFromSource;

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
