using System.Collections;
using System.Collections.Generic;
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

    public List<int> currentScores;
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
        if(rubricParentModal.transform.childCount > 0)
        {
            ClearPanel();
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

    public void ClearPanel()
    {
        foreach(Transform child in rubricParentModal.transform)
        {
            Destroy(child.gameObject);
        }

        currentScores.Clear();
    }

    public void UpdateScore()
    {
        int sum = 0;
        foreach(Transform child in rubricParentModal.transform)
        {
            foreach(Transform childchild in child)
            {
                if(childchild.tag == "AchievedScore")
                {
                    sum += Mathf.CeilToInt(float.TryParse(childchild.GetComponent<TMP_Text>().text, out var y1) ? y1:0);
                }
            }
        }

        rubricTotalScore.GetComponentInChildren<TMP_Text>().text = sum.ToString();
        currentSum = sum;
    }

    public void CopyScoreToClipboard()
    {
        GUIUtility.systemCopyBuffer = currentSum.ToString();
    }

    public void UpdateScoreInList()
    {

    }
}
