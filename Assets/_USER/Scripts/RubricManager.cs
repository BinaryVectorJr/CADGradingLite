using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RubricManager : MonoBehaviour
{
    public static RubricManager rbmInstance {get; private set;}

    [SerializeField]
    private GameObject rubricParentModal;

    [SerializeField]
    private GameObject rubricPrefab;

    [SerializeField]
    public TMP_Dropdown rubricTypeDropdown;

    [SerializeField]
    public Button rubricTotalScore;

    [SerializeField]
    public Button rubricFinalFeedback;

    public Button currentAssignmentButton;

    public List<int> currentScores;
    public List<string> currentFeedback;
    public int currentSum = 0;

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
        ChangeRubrics();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
    }

    public void ChangeRubrics()
    {
        // Each panel has a temporary ID when it is generated and is on-screen
        // This enables us to assign dynamic IDs to each panel, as the rubrics update based on assignment type
        int currentPanelID = 0;
        if(rubricParentModal.transform.childCount > 0 && GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            ClearPanel(1,1);
        }
        else if (rubricParentModal.transform.childCount > 0 && GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            ClearPanel(1,0);
        }

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

        rubricTotalScore.GetComponentInChildren<TMP_Text>().text = sum.ToString();
        currentSum = sum;
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
