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
    private GameObject focusPanel;

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
        focusPanel = GameObject.FindGameObjectWithTag("Modal");
        //print(scenesList[sceneNumber].name.ToString());
    }

    public void LaunchDueDateCalculator(int _state)
    {
        if(focusPanel.gameObject != null)
        {
            switch(_state)
            {
                case 0:
                    focusPanel.gameObject.SetActive(false);
                break;

                case 1:
                    focusPanel.gameObject.SetActive(true);
                break;

                default:
                    focusPanel.gameObject.SetActive(false);
                break;
            }
        }
    }

    public void LaunchDirectorySetter()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
