using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RubricManager : MonoBehaviour
{
    [SerializeField]
    private GameObject rubricParentModal;

    [SerializeField]
    private GameObject rubricPrefab;

    [SerializeField]
    private TMP_Dropdown rubricTypeDropdown;

    // Start is called before the first frame update
    void Start()
    {
        int tempRubric = DataParser.dpInstance.rubricLineCount;
        GameObject tempGO = null;

        for(int i=0; i<tempRubric; i++)
        {
            tempGO = GameObject.Instantiate(rubricPrefab);
            tempGO.transform.name = DataParser.dpInstance.rubricDatasetElements[i].error_item_desc.ToString() + " | " + DataParser.dpInstance.rubricDatasetElements[i].error_item_total_points;
            if(tempGO.GetComponentInChildren<TMP_Text>().name == "TXT_ErrDesc")
            {
                tempGO.GetComponentInChildren<TMP_Text>().text = tempGO.transform.name;
            }
            if(tempGO.GetComponentInChildren<TMP_Text>().name == "TXT_ErrorTotal")
            {
                tempGO.GetComponentInChildren<TMP_Text>().text = DataParser.dpInstance.rubricDatasetElements[i].error_item_total_points.ToString();
            }
            tempGO.transform.SetParent(rubricParentModal.transform);
            tempGO = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
