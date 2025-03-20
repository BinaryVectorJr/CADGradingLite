using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class AssmManager : MonoBehaviour
{
    [SerializeField]
    private GameObject parentModal;

    [SerializeField]
    private GameObject componentModal;

    [SerializeField]
    private GameObject assignmentPrefab;

    [SerializeField]
    private GameObject projectPrefab;

    [SerializeField]
    private Button assignmentSelectorButton;

    [SerializeField]
    private RubricManager currentRubricManager;

    // Start is called before the first frame update
    void Start()
    {
        // Setup buttons when "Select Assignment" button is clicked in Grading Scenes
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            SetupAssignmentButtons();
        }
        else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            SetupProjectButtons();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetupAssignmentButtons()
    {
        int tempCountOfButtons = DataParser.dpInstance.assmLineCount;
        GameObject tempButtonGO = null;

        for(int i=0; i<tempCountOfButtons; i++)
        {   
            tempButtonGO = GameObject.Instantiate(assignmentPrefab);
            tempButtonGO.transform.name = DataParser.dpInstance.assmDatasetElements[i].week_no.ToString("D2") + " | " + DataParser.dpInstance.assmDatasetElements[i].assm_no;
            tempButtonGO.GetComponentInChildren<TMP_Text>().text = tempButtonGO.transform.name;
            tempButtonGO.GetComponent<AssignmentType>().assignmentTypeCode = DataParser.dpInstance.assmDatasetElements[i].assm_type;
            tempButtonGO.transform.SetParent(parentModal.transform);
            tempButtonGO = null;
        }

        foreach(Transform child in parentModal.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() => onButtonClick1(child.gameObject));
        }
    }

    void onButtonClick1(GameObject _button)
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            RubricManager.rbmInstance.currentAssignmentButton = _button.GetComponent<Button>();
            assignmentSelectorButton.GetComponentInChildren<TMP_Text>().text = _button.gameObject.name;
            currentRubricManager.rubricTypeDropdown.value = _button.GetComponent<AssignmentType>().assignmentTypeCode;
            GameManager.gmInstance.PersistHideModals(0);
            GameManager.gmInstance.PersistShowModals2(1);
            currentRubricManager.ChangeRubrics();
        }

        if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            RubricManager.rbmInstance.currentAssignmentButton = _button.GetComponent<Button>();
            currentRubricManager.rubricTypeDropdown.value = _button.GetComponent<AssignmentType>().assignmentTypeCode;
            currentRubricManager.ChangeRubricsProject(_button);
        }
    }

    void SetupProjectButtons()
    {
        int tempCountOfButtons2 = 0;
        int currentIndex = 0;
        List<int> indexList = new List<int>();
        GameObject tempButtonGO2 = null;

        foreach (ProjectWithRubric proj in DataParser.dpInstance.projectWithRubricElements)
        {
            if(proj.project_data.project_id % 10 == 0)
            {
                tempCountOfButtons2+=1;
                // WORK VALIDATED: Debug.Log(proj.project_data.project_id);
                indexList.Add(currentIndex);
                // WORK VALIDATED: Debug.Log(currentIndex.ToString());
            }
            currentIndex++;
        }

        for(int i=0; i<indexList.Count; i++)
        {   
            tempButtonGO2 = GameObject.Instantiate(assignmentPrefab);
            tempButtonGO2.transform.name = DataParser.dpInstance.projectWithRubricElements[indexList[i]].project_data.project_name.ToString();
            tempButtonGO2.GetComponentInChildren<TMP_Text>().text = tempButtonGO2.transform.name;

            tempButtonGO2.GetComponent<AssignmentType>().assignmentTypeCode = DataParser.dpInstance.projectWithRubricElements[indexList[i]].project_data.project_component_assignment_data.assm_type;
            tempButtonGO2.GetComponent<AssignmentType>().projectID = DataParser.dpInstance.projectWithRubricElements[indexList[i]].project_data.project_id;
            tempButtonGO2.GetComponent<AssignmentType>().localProjectWithRubricData.Add(DataParser.dpInstance.projectWithRubricElements[indexList[i]]);

            tempButtonGO2.transform.SetParent(parentModal.transform);
            tempButtonGO2 = null;
        }

        foreach(Transform child in parentModal.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() => onButtonClick2(child.gameObject));
        }
    }

    void onButtonClick2(GameObject _button)
    {
        int tempTotal = 0;

        if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            GameManager.gmInstance.PersistHideModals(2);
            GameManager.gmInstance.PersistShowModals2(1);
            SetupProjectButtons2(_button.gameObject.name);

            foreach(GameObject go in RubricManager.rbmInstance.allAssignmentButtonsProject)
            {
                int tempLen = go.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data.Count();
                for(int i=1; i<tempLen; i++)
                {
                    tempTotal += int.Parse(go.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[i].error_item_achieved_points.ToString());
                }
            }

            RubricManager.rbmInstance.totalAssignmentScoreButton.GetComponentInChildren<TMP_Text>().text = tempTotal.ToString();

            RubricManager.rbmInstance.rubricTotalScoreButton.GetComponentInChildren<TMP_Text>().text = _button.GetComponent<AssignmentType>().localProjectWithRubricData[0].project_data.project_id.ToString();

            if(!RubricManager.rbmInstance.allRubricTotalScoresButtonProject.Contains(RubricManager.rbmInstance.rubricTotalScoreButton))
            {
                RubricManager.rbmInstance.allRubricTotalScoresButtonProject.Add(RubricManager.rbmInstance.rubricTotalScoreButton);
            }

            // WORKING VERIFIED: Debug.Log(RubricManager.rbmInstance.rubricTotalScoreButton.GetComponentInChildren<TMP_Text>().text);

        }
    }

    void SetupProjectButtons2(string _assignment_name)
    {
        int tempCountOfButtons2 = 0;
        int currentIndex2 = 0;
        List<int> indexList2 = new List<int>();
        GameObject tempButtonGO2 = null;
        RubricManager.rbmInstance.allAssignmentButtonsProject.Clear();
        //RubricManager.rbmInstance.allAssignmentAchievedScoresProject.Clear();

        foreach (ProjectWithRubric proj in DataParser.dpInstance.projectWithRubricElements)
        {
            if(_assignment_name == proj.project_data.project_name)
            {
                tempCountOfButtons2+=1;
                indexList2.Add(currentIndex2);
            }
            currentIndex2++;
        }

        foreach(Transform child in componentModal.transform)
        {
            Destroy(child.gameObject);
        }

        for(int i=0; i<indexList2.Count; i++)
        {   
            tempButtonGO2 = GameObject.Instantiate(projectPrefab);
            tempButtonGO2.transform.name = DataParser.dpInstance.projectWithRubricElements[indexList2[i]].project_data.project_name.ToString() + " | " + DataParser.dpInstance.projectWithRubricElements[indexList2[i]].project_data.project_component_assignment_data.assm_name.ToString();
            tempButtonGO2.GetComponentInChildren<TMP_Text>().text = tempButtonGO2.transform.name;

            tempButtonGO2.GetComponent<AssignmentType>().assignmentTypeCode = DataParser.dpInstance.projectWithRubricElements[indexList2[i]].project_data.project_component_assignment_data.assm_type;
            tempButtonGO2.GetComponent<AssignmentType>().projectID = DataParser.dpInstance.projectWithRubricElements[indexList2[i]].project_data.project_id;
            tempButtonGO2.GetComponent<AssignmentType>().localProjectWithRubricData.Add(DataParser.dpInstance.projectWithRubricElements[indexList2[i]]);
            
            tempButtonGO2.transform.SetParent(componentModal.transform);

            RubricManager.rbmInstance.allAssignmentButtonsProject.Add(tempButtonGO2);
            // RubricManager.rbmInstance.allAssignmentAchievedScoresProject.Add(int.Parse(tempButtonGO2.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[i].error_item_achieved_points.ToString()));

            // // WORKS BUT SCORES UPDATE FOR ALL
            // if(currentRubricManager.currentComponentScores.Count == 0)
            // {
            //     currentRubricManager.currentComponentScores.Add(new RubricManager.ProjectComponentScoresElement(DataParser.dpInstance.projectDatasetElements[i].project_id,0,currentRubricManager.currentScores));
            // }
            // else if (currentRubricManager.currentComponentScores.Any(complist => complist.component_id == DataParser.dpInstance.projectDatasetElements[i].project_id))
            // {
            //     Debug.Log("EXISTS");
            // }
            // else
            // {
            //     RubricManager.rbmInstance.currentComponentScores.Add(new RubricManager.ProjectComponentScoresElement(DataParser.dpInstance.projectDatasetElements[i].project_id,0,currentRubricManager.currentScores));
            // }

            tempButtonGO2 = null;
        }

        foreach(Transform child in componentModal.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() => onButtonClick1(child.gameObject));
        }

        currentRubricManager.ChangeRubrics();

    }

}
