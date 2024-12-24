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
    public TMP_Dropdown rubricTypeDropdown;

    // Start is called before the first frame update
    void Start()
    {
        ChangeRubrics();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeRubrics()
    {
        if(rubricParentModal.transform.childCount > 0)
        {
            ClearPanel();
        }

        int tempRubric = DataParser.dpInstance.rubricLineCount;
        GameObject tempRubricGO = null;

        for(int i=0; i<tempRubric; i++)
        {
            if(DataParser.dpInstance.rubricDatasetElements[i].assm_type == rubricTypeDropdown.value)
            {
                tempRubricGO = GameObject.Instantiate(rubricPrefab);
                tempRubricGO.GetComponent<CurrentRubricPanel>().ErrorDesc.text = DataParser.dpInstance.rubricDatasetElements[i].error_item_desc.ToString();
                tempRubricGO.GetComponent<CurrentRubricPanel>().ErrorTotal.text = DataParser.dpInstance.rubricDatasetElements[i].error_item_total_points.ToString();
                tempRubricGO.GetComponent<CurrentRubricPanel>().ErrorAchieved.text = DataParser.dpInstance.rubricDatasetElements[i].error_item_achieved_points.ToString();
                tempRubricGO.transform.name = DataParser.dpInstance.rubricDatasetElements[i].error_item_desc.ToString();
                tempRubricGO.transform.SetParent(rubricParentModal.transform);
                tempRubricGO = null;
            }
        }
    }

    public void ClearPanel()
    {
        foreach(Transform child in rubricParentModal.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
