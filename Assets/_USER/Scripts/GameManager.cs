using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MAIN_MENU = 0,
        REGULAR_GRADING = 1,
        PROJECT_GRADING = 2,
        LOAD_FILES = 3
    };

    public static GameManager gmInstance;

    public GameState currentState = GameState.MAIN_MENU;

    public List<SceneAsset> scenesList;

    public List<GameObject> modalPanels = new List<GameObject>();
    public List<Button> allButtons = new List<Button>();

    public string tagToTrack = "Modal";
    public string folderPath = "E:/Projects/1-Unity/CADGradingLite/Assets/_USER";


    void Awake()
    {
        if(gmInstance != null && gmInstance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            gmInstance = this;
            currentState = GameState.MAIN_MENU;
            DontDestroyOnLoad(gameObject);
        }

        // Add listener to when scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // When scene is loaded, execute function
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        LoadPanels();
        LoadButtons();
    }
    
    // Custom function to find panel gameobjects even if they are hidden
    void LoadPanels()
    {
        modalPanels.Clear();
        GameObject[] allPanels = GameObject.FindObjectsOfType<GameObject>(true);
        foreach(GameObject go in allPanels)
        {
            if(go.CompareTag(tagToTrack))
            {
                modalPanels.Add(go);
            }
        }

    }

    void LoadButtons()
    {
        allButtons.Clear();
        Button[] _allButtons = GameObject.FindObjectsOfType<Button>(true);
        foreach(Button btn_go in _allButtons)
        {
            allButtons.Add(btn_go);
            btn_go.onClick.AddListener(() => OnButtonClick(btn_go));
        }
    }

    public void UpdateTrackedObjects()
    {
        LoadPanels();
        LoadButtons();
    }

    public void SwitchScenes(int _sceneNumber)
    {
        SceneManager.LoadScene(gmInstance.scenesList[_sceneNumber].name.ToString());
        switch(_sceneNumber)
        {
            case 0:
                currentState = GameState.MAIN_MENU;
            break;

            case 1:
                currentState = GameState.REGULAR_GRADING;
            break;

            case 3:
                currentState = GameState.PROJECT_GRADING;
            break;

            default:
                currentState = GameState.MAIN_MENU;
            break;
        }
        LoadPanels();
        //print(scenesList[sceneNumber].name.ToString());
    }
    
    // Not being used currently, but can be used as a switcher
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

    public void PersistShowModals(int _modalNumber)
    {
        modalPanels[_modalNumber].gameObject.SetActive(true);
    }

    public void PersistHideModals(int _modalNumber)
    {
        modalPanels[_modalNumber].gameObject.SetActive(false);
    }

    void OnButtonClick(Button _btn)
    {
        if(_btn.transform.name == "BTN_Return")
        {
            OnSceneChangeClick("00StartScene");
        }
        if(_btn.transform.name == "BTN_RegularGrading")
        {
            OnSceneChangeClick("01GradingScene");;
        }
        if(_btn.transform.name == "BTN_Quit")
        {
            OnQuitButtonClick();
        }
        if(_btn.transform.name == "BTN_ChangeAssign")
        {
            OnPanelChangeClick("PNL_AssignmentSelector");
        }
        if(_btn.transform.name == "BTN_Directory")
        {
            OnDirectoryRefreshClick();
        }
    }

    void OnSceneChangeClick(string _sceneName)
    {
        // Use FindIndex to find the index based on the name
        int index = scenesList.FindIndex(scene => scene.name.Equals(_sceneName));
        SwitchScenes(index);
    }

    void OnPanelChangeClick(string _panelName)
    {
        // Use FindIndex to find the index based on the name
        int index = modalPanels.FindIndex(scene => scene.name.Equals(_panelName));
        PersistShowModals(index);
    }

    void OnQuitButtonClick()
    {
        Application.Quit();
    }

    void OnDirectoryRefreshClick()
    {
        if (Directory.Exists(folderPath))
        {
            // Open Windows Explorer to the specified folder
            Application.OpenURL(@"file://"+folderPath);
            OnPanelChangeClick("PNL_Directory");
        }
        else
        {

        }
    }
}
