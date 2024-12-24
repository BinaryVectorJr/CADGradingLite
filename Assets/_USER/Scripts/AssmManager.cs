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
        SetupAssignmentButtons();
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

    void onButtonClick(GameObject _button)
    {
        assignmentSelectorButton.GetComponentInChildren<TMP_Text>().text = _button.gameObject.name;
        currentRubricManager.rubricTypeDropdown.value = _button.GetComponent<AssignmentType>().assignmentTypeCode;
        GameManager.gmInstance.PersistHideModals(0);
        GameManager.gmInstance.PersistShowModals(1);
        currentRubricManager.ChangeRubrics();
    }
}
