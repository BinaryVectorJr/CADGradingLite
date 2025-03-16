using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssmManager : MonoBehaviour
{
    [SerializeField]
    private GameObject parentModal;

    [SerializeField]
    private GameObject assignmentPrefab;

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
            child.GetComponent<Button>().onClick.AddListener(() => onButtonClick(child.gameObject));
        }
    }

    void SetupProjectButtons()
    {
        int tempCountOfButtons2 = DataParser.dpInstance.projectLineCount;
        GameObject tempButtonGO2 = null;

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
            child.GetComponent<Button>().onClick.AddListener(() => onButtonClick(child.gameObject));
        }
    }

    void onButtonClick(GameObject _button)
    {
        assignmentSelectorButton.GetComponentInChildren<TMP_Text>().text = _button.gameObject.name;
        currentRubricManager.rubricTypeDropdown.value = _button.GetComponent<AssignmentType>().assignmentTypeCode;
        GameManager.gmInstance.PersistHideModals(0);
        GameManager.gmInstance.PersistShowModals2(1);
        currentRubricManager.ChangeRubrics();
    }
}
