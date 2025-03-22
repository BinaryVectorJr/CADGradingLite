using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;
using TMPro;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.Analytics;
using System.Runtime.CompilerServices;

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
    
    // This string should match the address you set in Addressables for the scene
    public string mainMenuSceneAddress;     // Main menu scene which should already be loaded
    public string gradingSceneAddress;      // Secondary scene for grading
    public string projectSceneAddress;      // Third scene for grading

    public string assmFilePath;
    public string rubricFilePath;
    public string projectFilePath;
    public string[] addressableTextFileAddresses;

    private string folderPath; 


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
        folderPath = Path.Combine(Application.persistentDataPath,"GradingToolData");
        folderPath = folderPath.Replace('/', '\\');
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
        if(_btn.transform.name == "BTN_ProjectGrading")
        {
            OnSceneChangeClick("02ProjectScene");;
        }
        if(_btn.transform.name == "BTN_Quit")
        {
            OnQuitButtonClick();
        }
        if(_btn.transform.name == "BTN_ChangeAssign")
        {
            OnPanelChangeClick("PNL_AssignmentSelector");
        }
        if(_btn.transform.name == "BTN_ChangeAssmProject")
        {
            OnPanelChangeClick("PNL_AssignmentSelectorProject");
        }
        if(_btn.transform.name == "BTN_CloseAssm")
        {
            OnPanelChangeClick2("PNL_AssignmentSelector");
        }
        if(_btn.transform.name == "BTN_CloseAssmProject")
        {
            OnPanelChangeClick2("PNL_AssignmentSelectorProject");
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
            gmInstance.currentState = GameState.MAIN_MENU;
        }
        else if(_sceneName == "01GradingScene")
        {
            LoadScene(gradingSceneAddress);
            gmInstance.currentState = GameState.REGULAR_GRADING;
        }
        else if(_sceneName == "02ProjectScene")
        {
            LoadScene(projectSceneAddress);
            gmInstance.currentState = GameState.PROJECT_GRADING;
        }

        //DataParser.dpInstance.ValidateAssignmentWithRubrics();
        //DataParser.dpInstance.RubricSetter();
    }

    void OnPanelChangeClick(string _panelName)
    {
        // Use FindIndex to find the index based on the name
        int index = modalPanels.FindIndex(scene => scene.name.Equals(_panelName));
        PersistShowModals(index);
    }

    void OnPanelChangeClick2(string _panelName)
    {
        // Use FindIndex to find the index based on the name
        int index = modalPanels.FindIndex(scene => scene.name.Equals(_panelName));
        PersistHideModals(index);
    }

    void OnQuitButtonClick()
    {
        Application.Quit();
    }

    void OnDirectoryRefreshClick()
    {
        //currentState = GameState.LOAD_FILES;

        // Check if the folder exists
        if (!Directory.Exists(folderPath))
        {
            // If the directory doesn't exist, create it
            Directory.CreateDirectory(folderPath);
        }
            
        // Then check the platform and open the folder accordingly
        #if UNITY_STANDALONE_WIN
            folderPath = folderPath.Replace('/', '\\');
            OpenFolderWindows(folderPath);
        #elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            OpenFolderMacOS(folderPath);
        #elif UNITY_STANDALONE_LINUX
            OpenFolderLinux(folderPath);
        #endif

        LoadAndCopyAddressables();
        //DataParser.dpInstance.RubricSetter();

        // OnPanelChangeClick("PNL_Directory");
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

    void LoadAndCopyAddressables()
    {
        // Loop through each Addressable asset address in the array
        foreach (var address in addressableTextFileAddresses)
        {
            // Load each text file asynchronously
            Addressables.LoadAssetAsync<TextAsset>(address).Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    TextAsset textAsset = handle.Result;

                    // Define the destination folder and file name
                    string destinationFolder = folderPath;
                    string destinationPath = Path.Combine(destinationFolder, $"{handle.Result.name}.txt");

                    if (!File.Exists(destinationPath))
                    {
                        // Ensure the destination folder exists
                        if (!Directory.Exists(destinationFolder))
                        {
                            Directory.CreateDirectory(destinationFolder);
                        }

                        // Write the content of the text file to the destination path
                        File.WriteAllText(destinationPath, textAsset.text);

                        // VERIFIED WORKING: UnityEngine.Debug.Log($"Text file '{address}' copied to: {destinationPath}");

                        if(handle.Result.name == "weekly-assms")
                        {
                            DataParser.dpInstance._assignmentDatasetFile = textAsset;
                            DataParser.dpInstance.ValidateAssignmentTextAsset(textAsset);
                        }
                        else if (handle.Result.name == "grading-rubric")
                        {
                            DataParser.dpInstance._rubricDatasetFile = textAsset;
                            DataParser.dpInstance.ValidateRubricTextAsset(textAsset);
                        }
                        else if (handle.Result.name == "project")
                        {
                            DataParser.dpInstance._projectDatasetFile = textAsset;
                            DataParser.dpInstance.ValidateProjectTextAsset(textAsset);
                        }

                    }
                    else
                    {
                        if(handle.Result.name == "weekly-assms")
                        {
                            string[] assms = File.ReadAllLines(destinationPath);
                            DataParser.dpInstance.ValidateAssignmentTextAsset(assms);
                        }
                        else if (handle.Result.name == "grading-rubric")
                        {
                            string[] rubs = File.ReadAllLines(destinationPath);
                            DataParser.dpInstance.ValidateRubricTextAsset(rubs);
                        }
                        else if (handle.Result.name == "project")
                        {
                            string[] projs = File.ReadAllLines(destinationPath);
                            DataParser.dpInstance.ValidateProjectTextAsset(projs);
                        }

                        // VERIFIED WORKING: UnityEngine.Debug.Log($"Files already exist: {address}");
                        return;
                    }

                }
                else
                {
                    // VERIFIED WORKING: UnityEngine.Debug.LogError($"Failed to load Addressable text file: {address}");
                }
            };
        }
    }

    // Windows: Open the folder using Process.Start
    private void OpenFolderWindows(string folderPath)
    {
        // Escape spaces in the path
        string escapedPath = folderPath.Replace(" ", "^ ");
        Process.Start("explorer.exe", escapedPath);
    }

    // macOS: Open the folder using `open` command
    private void OpenFolderMacOS(string folderPath)
    {
        string escapedPath = folderPath.Replace(" ", "\\ ");
        Process.Start("open", escapedPath);
    }

    // Linux: Open the folder using `xdg-open`
    private void OpenFolderLinux(string folderPath)
    {
        string escapedPath = folderPath.Replace(" ", "\\ ");
        Process.Start("xdg-open", escapedPath);
    }
}
