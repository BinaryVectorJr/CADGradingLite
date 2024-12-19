using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssmManager : MonoBehaviour
{
    [SerializeField]
    private GameObject parentModal;

    [SerializeField]
    private GameObject assignmentPrefab;

    // Start is called before the first frame update
    void Start()
    {
        int temp = DataParser.dpInstance.lineCount;
        GameObject tempGO = null;

        for(int i=0; i<temp; i++)
        {
            tempGO = GameObject.Instantiate(assignmentPrefab);
            tempGO.transform.name = DataParser.dpInstance.datasetElements[i].week_no.ToString() + " | " + DataParser.dpInstance.datasetElements[i].assm_no;
            tempGO.GetComponentInChildren<TMP_Text>().text = tempGO.transform.name;
            tempGO.transform.SetParent(parentModal.transform);
            tempGO = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
