using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;
using TMPro;
using UnityEngine.ResourceManagement.ResourceProviders;

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

    //public List<SceneAsset> scenesList;

    public List<GameObject> modalPanels = new List<GameObject>();
    public List<Button> allButtons = new List<Button>();

    public string tagToTrack = "Modal";
    public string assmFilePath = "E:/Projects/1-Unity/CADGradingLite/Assets/_USER/weekly-assms.txt";
    public string rubricFilePath = "E:/Projects/1-Unity/CADGradingLite/Assets/_USER/grading-rubric.txt";
    public string projectFilePath = "E:/Projects/1-Unity/CADGradingLite/Assets/_USER/project.txt";
    
    // This string should match the address you set in Addressables for the scene
    public string mainMenuSceneAddress;     // Main menu scene which should already be loaded
    public string gradingSceneAddress;      // Secondary scene for grading
    private AsyncOperationHandle<SceneInstance> sceneHandle;

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

        // // Add listener to when scene is loaded
        // SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadScene(mainMenuSceneAddress);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string address)
    {
        // Load the scene asynchronously using Addressables
        sceneHandle = Addressables.LoadSceneAsync(address, LoadSceneMode.Single);

        // You can check the completion of the loading operation
        sceneHandle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                LoadSceneElements(op.Result);
                //Debug.Log("Scene loaded successfully!");
            }
            else
            {
                //Debug.LogError("Scene loading failed.");
            }
        };
    }
    
    // To check if the scene is loaded at any point
    public bool IsSceneLoaded()
    {
        return sceneHandle.IsValid() && sceneHandle.Status == AsyncOperationStatus.Succeeded;
    }

    void LoadSceneElements(SceneInstance sceneInstance)
    {
        Scene scene = sceneInstance.Scene;

        // Check if the scene is valid (it's loaded)
        if (scene.isLoaded)
        {
            LoadPanels();
            LoadButtons();
            //Debug.Log("The scene is loaded and active.");
        }
        else
        {
            //Debug.Log("The scene is not loaded.");
        }
        //Add listener to when scene is loaded
        //SceneManager.sceneLoaded += OnSceneLoaded;
        //OnSceneLoaded(sceneInstance,LoadSceneMode.Single);

    }

    // // When scene is loaded, execute function
    // void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    // {
    //     LoadPanels();
    //     LoadButtons();
    // }
    
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

    // public void SwitchScenes(int _sceneNumber)
    // {
    //     SceneManager.LoadScene(gmInstance.scenesList[_sceneNumber].name.ToString());
    //     switch(_sceneNumber)
    //     {
    //         case 0:
    //             currentState = GameState.MAIN_MENU;
    //         break;

    //         case 1:
    //             currentState = GameState.REGULAR_GRADING;
    //         break;

    //         case 3:
    //             currentState = GameState.PROJECT_GRADING;
    //         break;

    //         default:
    //             currentState = GameState.MAIN_MENU;
    //         break;
    //     }
    //     LoadPanels();
    //     //print(scenesList[sceneNumber].name.ToString());
    // }
    
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
        if(modalPanels[_modalNumber].gameObject.activeSelf == false)
        {
            modalPanels[_modalNumber].gameObject.SetActive(true);
        }
        else
        {
            modalPanels[_modalNumber].gameObject.SetActive(false);
        }
    }

    public void PersistShowModals2(int _modalNumber)
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
        if(_btn.transform.name == "BTN_Data")
        {
            OnDirectoryRefreshClick();
        }
        if(_btn.transform.name == "BTN_SetupData")
        {
            OnLoadDataClick();
        }
        if(_btn.transform.name == "BTN_CloseDir")
        {
            int index = modalPanels.FindIndex(scene => scene.name.Equals("PNL_Directory"));
            PersistShowModals(index);
        }
    }

    void OnSceneChangeClick(string _sceneName)
    {
        // Use FindIndex to find the index based on the name
        // int index = scenesList.FindIndex(scene => scene.name.Equals(_sceneName));
        // SwitchScenes(index);
        if(_sceneName == "00StartScene")
        {
            LoadScene(mainMenuSceneAddress);
        }
        else if(_sceneName == "01GradingScene")
        {
            LoadScene(gradingSceneAddress);
        }
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
        currentState = GameState.LOAD_FILES;
        OnPanelChangeClick("PNL_Directory");
        // if (Directory.Exists(folderPath))
        // {
        //     // Open Windows Explorer to the specified folder
        //     Application.OpenURL(@"file://"+folderPath);
        //     OnPanelChangeClick("PNL_Directory");
        // }
        // else
        // {

        // }
    }

    void OnLoadDataClick()
    {
        currentState = GameState.MAIN_MENU;
        int index = modalPanels.FindIndex(scene => scene.name.Equals("PNL_Directory"));
        PersistShowModals(index);
    }
}
