using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssmManager : MonoBehaviour
{
    public static AssmManager assmMgrInstance {get; private set;}

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

    void Awake()
    {
        if(assmMgrInstance != null && assmMgrInstance != this)
        {
            Destroy(this);
        }
        else
        {
            assmMgrInstance = this;
        }
    }

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
            RubricManager.rbmInstance.currentAssignmentButton = _button.GetComponent<Button>();

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
        }

        // When assignment button is selected, create buttons on left side of screen with all the elements
        if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            // TODO: Make sure to clear out references to all old values (maybe the button is being selected) and clear all old project vals

            RubricManager.rbmInstance.ClearPanel(0,1);

            if(RubricManager.rbmInstance.projectItemParent.transform.childCount != 0)
            {
                foreach(Transform child in RubricManager.rbmInstance.projectItemParent.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            RubricManager.rbmInstance.currentAssignmentButton = _button.GetComponent<Button>();
            assignmentSelectorButton.GetComponentInChildren<TMP_Text>().text = _button.gameObject.name;
            GameManager.gmInstance.PersistHideModals(2);
            GameManager.gmInstance.PersistHideModals(1);

            // CREATE A MASTER COPY OF PROJECT BASE, PUT IT ON RUBIRC MANAGER AND MANIPULATE THAT DATA
            // PROBLEM: Unity was doing copy by reference instead of copy by value
            // Deep Copy Technique needed
            string jsonParse = JsonUtility.ToJson(_button.GetComponent<AssignmentType>().localProjectWithRubricData[0]);
            RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource = JsonUtility.FromJson<ProjectBaseElement>(jsonParse);

            // CREATE A REFERENCE to that Master Component Locally
            ProjectBaseElement LocalActiveProjectComponent = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource;

            int tempCountOfButtonsProject = _button.GetComponent<AssignmentType>().localProjectWithRubricData[0].project_associated_assignments.Count();
            GameObject tempComponentButton;

            // _button.AssignmentType.localproject[0].projectassociatedassm for number of buttons
            // then use that to instantiate rubrics
            for(int i=0; i<tempCountOfButtonsProject; i++)
            {   
                tempComponentButton = GameObject.Instantiate(assignmentPrefab,assignmentPrefab.transform.position,assignmentPrefab.transform.rotation,RubricManager.rbmInstance.projectItemParent.transform);

                tempComponentButton.GetComponent<AssignmentType>().localAssignmentWithRubric.Add(LocalActiveProjectComponent.project_associated_assignments[i]);
                
                tempComponentButton.transform.name = LocalActiveProjectComponent.project_associated_assignments[i].assm_name;
                tempComponentButton.GetComponentInChildren<TMP_Text>().text = tempComponentButton.transform.name;

                tempComponentButton.GetComponent<AssignmentType>().associatedAssignment = LocalActiveProjectComponent.project_associated_assignments[i].assm_id;
            }
            
            RecalculateCurrentTotal(RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource);

            foreach(Transform child in RubricManager.rbmInstance.projectItemParent.transform)
            {
                // replace with onbuttonclick2
                child.GetComponent<Button>().onClick.AddListener(() => onButtonClick2(child.gameObject));
            }
        }
    }

    // Once project loads in left panel, then this function loads in the rubrics for whatever component button is clicked
    void onButtonClick2(GameObject _assmButton)
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            RubricManager.rbmInstance.currentAssignmentButton = _assmButton.GetComponent<Button>();

            assignmentSelectorButton.GetComponentInChildren<TMP_Text>().text = _assmButton.gameObject.name;
            currentRubricManager.rubricTypeDropdown.value = _assmButton.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_type;
            GameManager.gmInstance.PersistShowModals2(1);

            // CREATE A REFERENCE to that Master Component Locally
            ProjectBaseElement LocalActiveProjectComponent = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource;

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
                tempRubricItem.GetComponent<CurrentRubricPanel>().UpdateValues();
            }
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
            tempButtonGO2.GetComponent<AssignmentType>().associatedProject = i;
            tempButtonGO2.GetComponent<AssignmentType>().localProjectWithRubricData.Add(DataParser.dpInstance.projectDatasetElements[i]);
            tempButtonGO2.transform.SetParent(parentModal.transform);
            tempButtonGO2 = null;
        }

        foreach(Transform child in parentModal.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() => onButtonClick1(child.gameObject));
        }
    }

    public void RecalculateCurrentTotal(ProjectBaseElement _projectSubset)
    {
        int tempSum1 = 0;
        int tempSum2 = 0;

        // Start at 1 because 0 index holds general error. Need to remove that element from total score if any issues
        for(int i=1; i<_projectSubset.project_associated_assignments.Count(); i++)
        {
            tempSum1 += int.Parse(_projectSubset.project_associated_assignments[i].assm_achieved_points.ToString());
            tempSum2 = _projectSubset.project_associated_assignments[0].assm_achieved_points - _projectSubset.project_associated_assignments[0].assm_total_points;
            RubricManager.rbmInstance.totalAssignmentScoreButton.GetComponentInChildren<TMP_Text>().text = (tempSum1+tempSum2).ToString();
        }
    }
}
