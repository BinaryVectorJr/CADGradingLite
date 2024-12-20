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

    // Start is called before the first frame update
    void Start()
    {
        int tempRubric = DataParser.dpInstance.rubricLineCount;
        GameObject tempGO = null;

        for(int i=0; i<tempRubric; i++)
        {
            tempGO = GameObject.Instantiate(rubricPrefab);
            tempGO.transform.name = DataParser.dpInstance.rubricDatasetElements[i].error_item_desc.ToString() + " | " + DataParser.dpInstance.rubricDatasetElements[i].error_item_total_points;
            tempGO.GetComponentInChildren<TMP_Text>().text = tempGO.transform.name;
            tempGO.transform.SetParent(rubricParentModal.transform);
            tempGO = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
