using System.Collections;
using System.Collections.Generic;
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

    void SetupProjectButtons()
    {
        int tempCountOfButtons2 = 0;
        GameObject tempButtonGO2 = null;

        foreach (ProjectDataElement proj in DataParser.dpInstance.projectDatasetElements)
        {
            if(proj.project_id % 10 == 0)
            {
                tempCountOfButtons2+=1;
            }
        }

        for(int i=0; i<tempCountOfButtons2; i++)
        {   
            tempButtonGO2 = GameObject.Instantiate(assignmentPrefab);
            tempButtonGO2.transform.name = DataParser.dpInstance.projectDatasetElements[i].project_name.ToString();
            tempButtonGO2.GetComponentInChildren<TMP_Text>().text = tempButtonGO2.transform.name;
            tempButtonGO2.GetComponent<AssignmentType>().assignmentTypeCode = DataParser.dpInstance.projectDatasetElements[i].project_component_assignment_data.assm_type;
            tempButtonGO2.transform.SetParent(parentModal.transform);
            tempButtonGO2 = null;
        }

        foreach(Transform child in parentModal.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() => onButtonClick2(child.gameObject));
        }
    }

    void SetupProjectButtons2(string _assignment_name)
    {
        int tempCountOfButtons2 = 0;
        GameObject tempButtonGO2 = null;

        foreach (ProjectDataElement proj in DataParser.dpInstance.projectDatasetElements)
        {
            if(_assignment_name == proj.project_name)
            {
                tempCountOfButtons2+=1;
            }
        }

        if(componentModal.transform.childCount == 0)
        {
            for(int i=0; i<tempCountOfButtons2; i++)
            {   
                tempButtonGO2 = GameObject.Instantiate(projectPrefab);
                tempButtonGO2.transform.name = DataParser.dpInstance.projectDatasetElements[i].project_name.ToString() + " | " + DataParser.dpInstance.projectDatasetElements[i].project_component_assignment_data.assm_name.ToString();
                tempButtonGO2.GetComponentInChildren<TMP_Text>().text = tempButtonGO2.transform.name;
                tempButtonGO2.GetComponent<AssignmentType>().assignmentTypeCode = DataParser.dpInstance.projectDatasetElements[i].project_component_assignment_data.assm_type;
                tempButtonGO2.transform.SetParent(componentModal.transform);
                tempButtonGO2 = null;
            }

            foreach(Transform child in componentModal.transform)
            {
                child.GetComponent<Button>().onClick.AddListener(() => onButtonClick1(child.gameObject));
            }
        }
        else
        {
            currentRubricManager.ChangeRubrics();
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
            currentRubricManager.ChangeRubrics();
        }
    }

    void onButtonClick2(GameObject _button)
    {
        if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            GameManager.gmInstance.PersistHideModals(2);
            GameManager.gmInstance.PersistShowModals2(1);
            SetupProjectButtons2(_button.gameObject.name);
        }
    }
}
