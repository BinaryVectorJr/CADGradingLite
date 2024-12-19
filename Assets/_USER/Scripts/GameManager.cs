using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gmInstance {get; private set;}

    [SerializeField] 
    private List<SceneAsset> scenesList;

    [SerializeField]
    private GameObject[] modalPanels;

    [SerializeField]
    private GameObject[] buttonTexts;

    void Awake()
    {
        if(gmInstance != null && gmInstance != this)
        {
            Destroy(this);
        }
        else
        {
            gmInstance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchScenes(int _sceneNumber)
    {
        SceneManager.LoadScene(gmInstance.scenesList[_sceneNumber].name.ToString());
        modalPanels = GameObject.FindGameObjectsWithTag("Modal");
        //print(scenesList[sceneNumber].name.ToString());
    }

    public void LaunchModals(int _modalNumber)
    {
        if(modalPanels[_modalNumber].gameObject != null)
        {
            if(modalPanels[_modalNumber].gameObject.activeSelf)
            {
                modalPanels[_modalNumber].gameObject.SetActive(false);
            }
            else
            {
                modalPanels[_modalNumber].gameObject.SetActive(true);
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
