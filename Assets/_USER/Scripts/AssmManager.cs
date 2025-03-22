using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.Animations;
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

    // Actual buttons that when clicked on, one can navigate around different assignments
    // Contains local assignment data based on button name
    void SetupAssignmentButtons()
    {
        int tempCountOfButtons = DataParser.dpInstance.assmLineCount;
        GameObject tempButtonGO = null;

        for(int i=0; i<tempCountOfButtons; i++)
        {   
            tempButtonGO = GameObject.Instantiate(assignmentPrefab);
            tempButtonGO.transform.name = DataParser.dpInstance.assmDatasetElements[i].week_no.ToString() + " | " + DataParser.dpInstance.assmDatasetElements[i].assm_no + " - " + DataParser.dpInstance.assmDatasetElements[i].assm_name;
            tempButtonGO.GetComponentInChildren<TMP_Text>().text = tempButtonGO.transform.name;
            //tempButtonGO.GetComponent<AssignmentType>().associatedAssignment = DataParser.dpInstance.assmDatasetElements[i].assm_type;
            tempButtonGO.GetComponent<AssignmentType>().associatedAssignment = i;
            tempButtonGO.GetComponent<AssignmentType>().localAssignmentWithRubric.Add(DataParser.dpInstance.assmDatasetElements[i]);
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
            // GameObject tempRubricPanel = null;
            // tempRubricPanel = GameObject.Instantiate(RubricManager.rbmInstance.rubricPanelPrefab, RubricManager.rbmInstance.rubricPanelPrefab.transform.position, RubricManager.rbmInstance.rubricPanelPrefab.transform.rotation, RubricManager.rbmInstance.rubricPanelPrefabParent.transform);

            RubricManager.rbmInstance.currentAssignmentButton = _button.GetComponent<Button>();
            //RubricManager.rbmInstance.prevAssignmentButton = RubricManager.rbmInstance.currentAssignmentButton;
            assignmentSelectorButton.GetComponentInChildren<TMP_Text>().text = _button.gameObject.name;
            currentRubricManager.rubricTypeDropdown.value = _button.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_type;
            GameManager.gmInstance.PersistHideModals(0);
            GameManager.gmInstance.PersistShowModals2(1);

            foreach(Transform child in RubricManager.rbmInstance.rubricItemParent.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject tempRubricItem = null;
            for(int i=0; i<_button.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_rubrics.Count; i++)
            {
                tempRubricItem = GameObject.Instantiate(RubricManager.rbmInstance.rubricPrefab,RubricManager.rbmInstance.rubricPrefab.transform.position,RubricManager.rbmInstance.rubricPrefab.transform.rotation,RubricManager.rbmInstance.rubricItemParent.transform);
                tempRubricItem.GetComponent<CurrentRubricPanel>().panelID = i;
                tempRubricItem.GetComponent<CurrentRubricPanel>().localAssociatedAssignment = _button.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_id;
                tempRubricItem.GetComponent<CurrentRubricPanel>().associatedAssignmentButton = _button;
                tempRubricItem.GetComponent<CurrentRubricPanel>().ResetScoresToOriginalTotal();
                tempRubricItem.GetComponent<CurrentRubricPanel>().UpdateValues();
            }

            RubricManager.rbmInstance.currentFeedback.Clear();
            //currentRubricManager.ChangeRubrics(_button);


            // RubricManager.rbmInstance.currentAssignmentButton = _button.GetComponent<Button>();
            // assignmentSelectorButton.GetComponentInChildren<TMP_Text>().text = _button.gameObject.name;
            // currentRubricManager.rubricTypeDropdown.value = _button.GetComponent<AssignmentType>().assignmentTypeCode;
            // GameManager.gmInstance.PersistHideModals(0);
            // GameManager.gmInstance.PersistShowModals2(1);
            // currentRubricManager.ChangeRubrics(_button);
        }

        // When assignment button is selected, create buttons on left side of screen with all the elements
        if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            // TODO: Make sure to clear out references to all old values (maybe the button is being selected) and clear all old project vals

            if(RubricManager.rbmInstance.projectItemParent.transform.childCount != 0)
            {
                foreach(Transform child in RubricManager.rbmInstance.projectItemParent.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            RubricManager.rbmInstance.currentAssignmentButton = _button.GetComponent<Button>();
            assignmentSelectorButton.GetComponentInChildren<TMP_Text>().text = _button.gameObject.name;
            //currentRubricManager.rubricTypeDropdown.value = _button.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_type;
            GameManager.gmInstance.PersistHideModals(2);
            GameManager.gmInstance.PersistShowModals2(1);

            // CREATE A MASTER COPY OF PROJECT BASE, PUT IT ON RUBIRC MANAGER AND MANIPULATE THAT DATA
            // PROBLEM: Unity is doing copy by reference instead of copy by value
            // Deep Copy Technique needed
            string jsonParse = JsonUtility.ToJson(_button.GetComponent<AssignmentType>().localProjectWithRubricData[0]);
            RubricManager.rbmInstance.MasterActiveProjectComponent = JsonUtility.FromJson<ProjectBaseElement>(jsonParse);

            // RubricManager.rbmInstance.MasterActiveProjectComponent = _button.GetComponent<AssignmentType>().localProjectWithRubricData[0];

            // CREATE A REFERENCE to that Master Component Locally
            ProjectBaseElement LocalActiveProjectComponent = RubricManager.rbmInstance.MasterActiveProjectComponent;

            int tempCountOfButtonsProject = _button.GetComponent<AssignmentType>().localProjectWithRubricData[0].project_associated_assignments.Count();
            GameObject tempComponentButton;

            // _button.AssignmentType.localproject[0].projectassociatedassm for number of buttons
            // then use that to instantiate rubrics
            for(int i=0; i<tempCountOfButtonsProject; i++)
            {   
                tempComponentButton = GameObject.Instantiate(assignmentPrefab,assignmentPrefab.transform.position,assignmentPrefab.transform.rotation,RubricManager.rbmInstance.projectItemParent.transform);

                // tempComponentButton.GetComponent<AssignmentType>().localAssignmentWithRubric.Add(_button.GetComponent<AssignmentType>().localProjectWithRubricData[0].project_associated_assignments[i]);

                tempComponentButton.GetComponent<AssignmentType>().localAssignmentWithRubric.Add(LocalActiveProjectComponent.project_associated_assignments[i]);
                
                tempComponentButton.transform.name = LocalActiveProjectComponent.project_associated_assignments[i].assm_name;
                tempComponentButton.GetComponentInChildren<TMP_Text>().text = tempComponentButton.transform.name;

                tempComponentButton.GetComponent<AssignmentType>().associatedAssignment = LocalActiveProjectComponent.project_associated_assignments[i].assm_id;

                // tempComponentButton.GetComponent<AssignmentType>().associatedAssignment = tempComponentButton.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_type;
                
                // tempComponentButton.GetComponent<AssignmentType>().localAssignmentWithRubric.Add(tempComponentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].project_associated_assignments[i]);

                // //tempComponentButton.transform.SetParent(parentModal.transform);
                // tempComponentButton = null;
            }

            foreach(Transform child in RubricManager.rbmInstance.projectItemParent.transform)
            {
                // replace with onbuttonclick2
                child.GetComponent<Button>().onClick.AddListener(() => onButtonClick2(child.gameObject));
            }

            // GameObject tempRubricItem2;
            // for(int i=0; i<_button.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_rubrics.Count; i++)
            // {
            //     tempRubricItem2 = GameObject.Instantiate(RubricManager.rbmInstance.rubricPrefab,RubricManager.rbmInstance.rubricPrefab.transform.position,RubricManager.rbmInstance.rubricPrefab.transform.rotation,RubricManager.rbmInstance.rubricItemParent.transform);
            //     tempRubricItem2.GetComponent<CurrentRubricPanel>().panelID = i;
            //     tempRubricItem2.GetComponent<CurrentRubricPanel>().associatedAssignment = _button.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_id;
            //     tempRubricItem2.GetComponent<CurrentRubricPanel>().associatedAssignmentButton = _button;
            //     tempRubricItem2.GetComponent<CurrentRubricPanel>().ResetScoresToOriginalTotal();
            //     tempRubricItem2.GetComponent<CurrentRubricPanel>().UpdateValues();
            // }
            // RubricManager.rbmInstance.currentFeedback.Clear();


        //     RubricManager.rbmInstance.currentAssignmentButton = _button.GetComponent<Button>();
        //     currentRubricManager.rubricTypeDropdown.value = _button.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_type;
        //     currentRubricManager.ChangeRubricsProject(_button);
        }
    }

    // Once project loads in left panel, then this function loads in the rubrics for whatever component button is clicked
    void onButtonClick2(GameObject _assmButton)
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            // GameObject tempRubricPanel = null;
            // tempRubricPanel = GameObject.Instantiate(RubricManager.rbmInstance.rubricPanelPrefab, RubricManager.rbmInstance.rubricPanelPrefab.transform.position, RubricManager.rbmInstance.rubricPanelPrefab.transform.rotation, RubricManager.rbmInstance.rubricPanelPrefabParent.transform);

            RubricManager.rbmInstance.currentAssignmentButton = _assmButton.GetComponent<Button>();
            //RubricManager.rbmInstance.prevAssignmentButton = RubricManager.rbmInstance.currentAssignmentButton;
            assignmentSelectorButton.GetComponentInChildren<TMP_Text>().text = _assmButton.gameObject.name;
            currentRubricManager.rubricTypeDropdown.value = _assmButton.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_type;
           //GameManager.gmInstance.PersistHideModals(0);
            GameManager.gmInstance.PersistShowModals2(1);

            // CREATE A REFERENCE to that Master Component Locally
            ProjectBaseElement LocalActiveProjectComponent = RubricManager.rbmInstance.MasterActiveProjectComponent;

            foreach(Transform child in RubricManager.rbmInstance.rubricItemParent.transform)
            {
                Destroy(child.gameObject);
            }

            // LINQ to search for the index of assignment rubric (L3) associated to a specific assignment (L2) in project (L1) and compare to the assignment ID stored in the button that was clicked
            var tempIndex = LocalActiveProjectComponent.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == _assmButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            GameObject tempRubricItem = null;

            // Use the previously created temporary index "class"'s index memeber to select the correct assignment (L2) in project (L1) and find the length of its rubrics structure
            for(int i=0; i<LocalActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics.Count; i++)
            {
                tempRubricItem = GameObject.Instantiate(RubricManager.rbmInstance.rubricPrefab,RubricManager.rbmInstance.rubricPrefab.transform.position,RubricManager.rbmInstance.rubricPrefab.transform.rotation,RubricManager.rbmInstance.rubricItemParent.transform);
                tempRubricItem.GetComponent<CurrentRubricPanel>().panelID = i;
                tempRubricItem.GetComponent<CurrentRubricPanel>().localAssociatedAssignment = LocalActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_id;
                tempRubricItem.GetComponent<CurrentRubricPanel>().associatedAssignmentButton = _assmButton;
                //tempRubricItem.GetComponent<CurrentRubricPanel>().ResetScoresToOriginalTotal();
                tempRubricItem.GetComponent<CurrentRubricPanel>().UpdateValues();
            }

            //RubricManager.rbmInstance.currentFeedback.Clear();
            //currentRubricManager.ChangeRubrics(_button);


            // RubricManager.rbmInstance.currentAssignmentButton = _button.GetComponent<Button>();
            // assignmentSelectorButton.GetComponentInChildren<TMP_Text>().text = _button.gameObject.name;
            // currentRubricManager.rubricTypeDropdown.value = _button.GetComponent<AssignmentType>().assignmentTypeCode;
            // GameManager.gmInstance.PersistHideModals(0);
            // GameManager.gmInstance.PersistShowModals2(1);
            // currentRubricManager.ChangeRubrics(_button);
        }
    }

    void SetupProjectButtons()
    {
        int tempCountOfButtonsProject = DataParser.dpInstance.projectDatasetElements.Count();
        GameObject tempButtonGO2 = null;

        for(int i=0; i<tempCountOfButtonsProject; i++)
        {   
            tempButtonGO2 = GameObject.Instantiate(assignmentPrefab);
            // Divide by 10 is just for formatting
            tempButtonGO2.transform.name = (DataParser.dpInstance.projectDatasetElements[i].project_id / 10).ToString()+ " | " + DataParser.dpInstance.projectDatasetElements[i].project_name;
            tempButtonGO2.GetComponentInChildren<TMP_Text>().text = tempButtonGO2.transform.name;
            //tempButtonGO.GetComponent<AssignmentType>().associatedAssignment = DataParser.dpInstance.assmDatasetElements[i].assm_type;
            tempButtonGO2.GetComponent<AssignmentType>().associatedProject = i;
            tempButtonGO2.GetComponent<AssignmentType>().localProjectWithRubricData.Add(DataParser.dpInstance.projectDatasetElements[i]);
            tempButtonGO2.transform.SetParent(parentModal.transform);
            tempButtonGO2 = null;
        }

        foreach(Transform child in parentModal.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() => onButtonClick1(child.gameObject));
        }

    //     int tempCountOfButtons2 = 0;
    //     int currentIndex = 0;
    //     List<int> indexList = new List<int>();
    //     GameObject tempButtonGO2 = null;

    //     foreach (ProjectWithRubric proj in DataParser.dpInstance.projectWithRubricElements)
    //     {
    //         if(proj.project_data.project_id % 10 == 0)
    //         {
    //             tempCountOfButtons2+=1;
    //             // WORK VALIDATED: Debug.Log(proj.project_data.project_id);
    //             indexList.Add(currentIndex);
    //             // WORK VALIDATED: Debug.Log(currentIndex.ToString());
    //         }
    //         currentIndex++;
    //     }

    //     for(int i=0; i<indexList.Count; i++)
    //     {   
    //         tempButtonGO2 = GameObject.Instantiate(assignmentPrefab);
    //         tempButtonGO2.transform.name = DataParser.dpInstance.projectWithRubricElements[indexList[i]].project_data.project_name.ToString();
    //         tempButtonGO2.GetComponentInChildren<TMP_Text>().text = tempButtonGO2.transform.name;

    //         //tempButtonGO2.GetComponent<AssignmentType>().localProjectWithRubricData = DataParser.dpInstance.projectWithRubricElements[indexList[i]].project_data.project_component_assignment_data.assm_type;
    //         tempButtonGO2.GetComponent<AssignmentType>().associatedProject = DataParser.dpInstance.projectWithRubricElements[indexList[i]].project_data.project_id;
    //         tempButtonGO2.GetComponent<AssignmentType>().localProjectWithRubricData.Add(DataParser.dpInstance.projectWithRubricElements[indexList[i]]);

    //         tempButtonGO2.transform.SetParent(parentModal.transform);
    //         tempButtonGO2 = null;
    //     }

    //     foreach(Transform child in parentModal.transform)
    //     {
    //         child.GetComponent<Button>().onClick.AddListener(() => onButtonClick2(child.gameObject));
    //     }
    // }

    // void onButtonClick2(GameObject _button)
    // {
    //     int tempTotal = 0;

    //     if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
    //     {
    //         GameManager.gmInstance.PersistHideModals(2);
    //         GameManager.gmInstance.PersistShowModals2(1);
    //         //SetupProjectButtons2(_button.gameObject.name);

    //         foreach(GameObject go in RubricManager.rbmInstance.allAssignmentButtonsProject)
    //         {
    //             int tempLen = go.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data.Count();
    //             for(int i=1; i<tempLen; i++)
    //             {
    //                 tempTotal += int.Parse(go.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[i].error_item_achieved_points.ToString());
    //             }
    //         }

    //         RubricManager.rbmInstance.totalAssignmentScoreButton.GetComponentInChildren<TMP_Text>().text = tempTotal.ToString();

    //         RubricManager.rbmInstance.rubricTotalScoreButton.GetComponentInChildren<TMP_Text>().text = _button.GetComponent<AssignmentType>().localProjectWithRubricData[0].project_data.project_id.ToString();

    //         if(!RubricManager.rbmInstance.allRubricTotalScoresButtonProject.Contains(RubricManager.rbmInstance.rubricTotalScoreButton))
    //         {
    //             RubricManager.rbmInstance.allRubricTotalScoresButtonProject.Add(RubricManager.rbmInstance.rubricTotalScoreButton);
    //         }

    //         // WORKING VERIFIED: Debug.Log(RubricManager.rbmInstance.rubricTotalScoreButton.GetComponentInChildren<TMP_Text>().text);

    //     }
    }

    // void SetupProjectButtons2(string _assignment_name)
    // {
    //     int tempCountOfButtons2 = 0;
    //     int currentIndex2 = 0;
    //     List<int> indexList2 = new List<int>();
    //     GameObject tempButtonGO2 = null;
    //     RubricManager.rbmInstance.allAssignmentButtonsProject.Clear();
    //     //RubricManager.rbmInstance.allAssignmentAchievedScoresProject.Clear();

    //     foreach (ProjectWithRubric proj in DataParser.dpInstance.projectWithRubricElements)
    //     {
    //         if(_assignment_name == proj.project_data.project_name)
    //         {
    //             tempCountOfButtons2+=1;
    //             indexList2.Add(currentIndex2);
    //         }
    //         currentIndex2++;
    //     }

    //     foreach(Transform child in componentModal.transform)
    //     {
    //         Destroy(child.gameObject);
    //     }

    //     for(int i=0; i<indexList2.Count; i++)
    //     {   
    //         tempButtonGO2 = GameObject.Instantiate(projectPrefab);
    //         tempButtonGO2.transform.name = DataParser.dpInstance.projectWithRubricElements[indexList2[i]].project_data.project_name.ToString() + " | " + DataParser.dpInstance.projectWithRubricElements[indexList2[i]].project_data.project_component_assignment_data.assm_name.ToString();
    //         tempButtonGO2.GetComponentInChildren<TMP_Text>().text = tempButtonGO2.transform.name;

    //         //tempButtonGO2.GetComponent<AssignmentType>().assignmentTypeCode = DataParser.dpInstance.projectWithRubricElements[indexList2[i]].project_data.project_component_assignment_data.assm_type;
    //         tempButtonGO2.GetComponent<AssignmentType>().associatedProject = DataParser.dpInstance.projectWithRubricElements[indexList2[i]].project_data.project_id;
    //         tempButtonGO2.GetComponent<AssignmentType>().localProjectWithRubricData.Add(DataParser.dpInstance.projectWithRubricElements[indexList2[i]]);
            
    //         tempButtonGO2.transform.SetParent(componentModal.transform);

    //         RubricManager.rbmInstance.allAssignmentButtonsProject.Add(tempButtonGO2);
    //         // RubricManager.rbmInstance.allAssignmentAchievedScoresProject.Add(int.Parse(tempButtonGO2.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[i].error_item_achieved_points.ToString()));

    //         // // WORKS BUT SCORES UPDATE FOR ALL
    //         // if(currentRubricManager.currentComponentScores.Count == 0)
    //         // {
    //         //     currentRubricManager.currentComponentScores.Add(new RubricManager.ProjectComponentScoresElement(DataParser.dpInstance.projectDatasetElements[i].project_id,0,currentRubricManager.currentScores));
    //         // }
    //         // else if (currentRubricManager.currentComponentScores.Any(complist => complist.component_id == DataParser.dpInstance.projectDatasetElements[i].project_id))
    //         // {
    //         //     Debug.Log("EXISTS");
    //         // }
    //         // else
    //         // {
    //         //     RubricManager.rbmInstance.currentComponentScores.Add(new RubricManager.ProjectComponentScoresElement(DataParser.dpInstance.projectDatasetElements[i].project_id,0,currentRubricManager.currentScores));
    //         // }

    //         tempButtonGO2 = null;
    //     }

    //     foreach(Transform child in componentModal.transform)
    //     {
    //         child.GetComponent<Button>().onClick.AddListener(() => onButtonClick1(child.gameObject));
    //     }

    //     currentRubricManager.ChangeRubrics(null);

    // }

}
