using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    #region VARIABLES

    public static MenuController instance;
    //Level 0 Screens
    public GameObject homeUIObject;
    public GameObject newSimulationUIObject;
    public GameObject loadSimulationUIObject;
    public GameObject aboutUIObject;
    //Level 1 Screens
    public GameObject resourceListUIObject;
    public GameObject speciesListUIObject;
    //Level 2 Screens
    public GameObject newResourceUIObject;
    public GameObject newSpeciesUIObject;
    //Navigation screens
    public UIScreens _currentScreen;
    public UIScreens _contentRoute;
    public UIScreens _previousScreen;

    public int fullScreen;
    private bool updated = false;

    //For handling files
    
    #endregion
    
    #region Unity Functions
    
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        if (PlayerPrefs.HasKey("FS"))
        {
            fullScreen = PlayerPrefs.GetInt("FS");
        }
        else
        {
            PlayerPrefs.SetInt("FS", 0);
        }
        
        _currentScreen = UIScreens.HOME;
        _contentRoute = UIScreens.NEWSIMULATION;
        UpdateFS();
        updated = false;
    }

    void Update()
    {
        if (AssetCreationManager.instance != null && updated == false)
        {
            updated = true;
            UpdateUIScreen();
        }
    }

    private void UpdateFS()
    {
        if (fullScreen == 0)
        {
            Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.ExclusiveFullScreen);
            Screen.fullScreen = true;
        }
        else
        {
            Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.MaximizedWindow);
            Screen.fullScreen = false;
        }
    }

    public void ToggleFS()
    {
        if (fullScreen == 0)
        {
            fullScreen = 1;
        }
        else
        {
            fullScreen = 0;
        }
        
        UpdateFS();
    }

    #endregion

    #region MenuNavigation
    public void AddSpeciesButton()
    {
        PersistantSceneManager.instance.ConfirmEffect();

        _previousScreen = _currentScreen;
        _currentScreen = UIScreens.SPECIESLIST;
        UpdateUIScreen();
    }

    public void AddResourceButton()
    {
        PersistantSceneManager.instance.ConfirmEffect();

        _previousScreen = _currentScreen;
        _currentScreen = UIScreens.RESOURCELIST;
        UpdateUIScreen();
    }

    public void NewSpeciesButton()
    {
        PersistantSceneManager.instance.ConfirmEffect();

        _previousScreen = _currentScreen;
        _currentScreen = UIScreens.NEWSPECIES;
        _contentRoute = UIScreens.NEWSPECIES;
        UpdateUIScreen();
    }

    public void NewResourceButton()
    {
        PersistantSceneManager.instance.ConfirmEffect();

        _previousScreen = _currentScreen;
        _currentScreen = UIScreens.NEWRESOURCE;
        _contentRoute = UIScreens.NEWRESOURCE;
        UpdateUIScreen();
    }

    public void BackButtons()
    {
        PersistantSceneManager.instance.BackEffect();
        switch (_currentScreen)
        {
            case UIScreens.ABOUT:
            case UIScreens.NEWSIMULATION:
            case UIScreens.LOADSIMULATION:
                _currentScreen = UIScreens.HOME;
                break;
            case UIScreens.NEWSPECIES:
                _currentScreen = _previousScreen == UIScreens.HOME ? UIScreens.HOME : UIScreens.SPECIESLIST;
                break;
            case UIScreens.NEWRESOURCE:
                _currentScreen = _previousScreen == UIScreens.HOME ? UIScreens.HOME : UIScreens.RESOURCELIST;
                break;
            case UIScreens.SPECIESLIST:
                _currentScreen = UIScreens.NEWSIMULATION;
                _contentRoute = UIScreens.NEWSIMULATION;
                break;
            case UIScreens.RESOURCELIST:
                //go to new species or new sim page?
                if (_contentRoute == UIScreens.NEWSPECIES)
                {
                    _currentScreen = UIScreens.NEWSPECIES;
                }
                else
                {
                    _currentScreen = UIScreens.NEWSIMULATION;
                }
                break;
        }
        UpdateUIScreen();
    }

    public void NewSimulationButton()
    {
        PersistantSceneManager.instance.ConfirmEffect();

        _previousScreen = _currentScreen;
        _currentScreen = UIScreens.NEWSIMULATION;
        _contentRoute = UIScreens.NEWSIMULATION;
        UpdateUIScreen();
    }

    public void LoadSimulationButton()
    {
        PersistantSceneManager.instance.ConfirmEffect();

        _previousScreen = _currentScreen;
        _currentScreen = UIScreens.LOADSIMULATION;
        UpdateUIScreen();
    }

    public void AboutButton()
    {
        PersistantSceneManager.instance.ConfirmEffect();

        _previousScreen = _currentScreen;
        _currentScreen = UIScreens.ABOUT;
        UpdateUIScreen();
    }
    private void UpdateUIScreen()
    {
        if (AssetCreationManager.instance == null)
        {
            return;
        }
        
        AssetCreationManager.instance.PopulateGrids();
        
        HideUI();
        
        switch (_currentScreen) //Show relevant object
        {
            case UIScreens.HOME:
                homeUIObject.SetActive(true);
                break;
            case UIScreens.NEWSIMULATION:
                newSimulationUIObject.SetActive(true);
                break;
            case UIScreens.LOADSIMULATION:
                loadSimulationUIObject.SetActive(true);
                break;
            case UIScreens.ABOUT:
                aboutUIObject.SetActive(true);
                break;
            case UIScreens.RESOURCELIST:
                resourceListUIObject.SetActive(true);
                break;
            case UIScreens.SPECIESLIST:
                speciesListUIObject.SetActive(true);
                break;
            case UIScreens.NEWRESOURCE:
                newResourceUIObject.SetActive(true);
                break;
            case UIScreens.NEWSPECIES:
                newSpeciesUIObject.SetActive(true);
                break;
        }
    }
    private void HideUI()
    {
        homeUIObject.SetActive(false);
        newSimulationUIObject.SetActive(false);
        loadSimulationUIObject.SetActive(false);
        aboutUIObject.SetActive(false);
        
        resourceListUIObject.SetActive(false);
        speciesListUIObject.SetActive(false);
        
        newResourceUIObject.SetActive(false);
        newSpeciesUIObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion
    
}

public enum UIScreens
{
    HOME,
    NEWSIMULATION,
    LOADSIMULATION,
    ABOUT,
    RESOURCELIST,
    SPECIESLIST,
    NEWRESOURCE,
    NEWSPECIES,
}
