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
    public GameObject settingsUIObject;
    //Level 1 Screens
    public GameObject resourceListUIObject;
    public GameObject speciesListUIObject;
    //Level 2 Screens
    public GameObject newResourceUIObject;
    public GameObject newSpeciesUIObject;
    //Navigation screens
    public UIScreens _currentScreen;
    public UIScreens _contentRoute;

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
        _currentScreen = UIScreens.HOME;
        _contentRoute = UIScreens.NEWSIMULATION;
        UpdateUIScreen();
    }
    
    #endregion

    #region MenuNavigation
    public void AddSpeciesButton()
    {
        _currentScreen = UIScreens.SPECIESLIST;
        UpdateUIScreen();
    }

    public void AddResourceButton()
    {
        _currentScreen = UIScreens.RESOURCELIST;
        UpdateUIScreen();
    }

    public void NewSpeciesButton()
    {
        _currentScreen = UIScreens.NEWSPECIES;
        _contentRoute = UIScreens.NEWSPECIES;
        UpdateUIScreen();
    }

    public void NewResourceButton()
    {
        _currentScreen = UIScreens.NEWRESOURCE;
        _contentRoute = UIScreens.NEWRESOURCE;
        UpdateUIScreen();
    }

    public void BackButtons()
    {
        switch (_currentScreen)
        {
            case UIScreens.SETTINGS:
            case UIScreens.NEWSIMULATION:
            case UIScreens.LOADSIMULATION:
                _currentScreen = UIScreens.HOME;
                break;
            case UIScreens.NEWSPECIES:
                _currentScreen = UIScreens.SPECIESLIST;
                break;
            case UIScreens.NEWRESOURCE:
                _currentScreen = UIScreens.RESOURCELIST;
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
        _currentScreen = UIScreens.NEWSIMULATION;
        _contentRoute = UIScreens.NEWSIMULATION;
        UpdateUIScreen();
    }

    public void LoadSimulationButton()
    {
        _currentScreen = UIScreens.LOADSIMULATION;
        UpdateUIScreen();
    }

    public void SettingsButton()
    {
        _currentScreen = UIScreens.SETTINGS;
        UpdateUIScreen();
    }
    private void UpdateUIScreen()
    {
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
            case UIScreens.SETTINGS:
                settingsUIObject.SetActive(true);
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
        settingsUIObject.SetActive(false);
        
        resourceListUIObject.SetActive(false);
        speciesListUIObject.SetActive(false);
        
        newResourceUIObject.SetActive(false);
        newSpeciesUIObject.SetActive(false);
    }
    
    #endregion
    
}

public enum UIScreens
{
    HOME,
    NEWSIMULATION,
    LOADSIMULATION,
    SETTINGS,
    RESOURCELIST,
    SPECIESLIST,
    NEWRESOURCE,
    NEWSPECIES,
}
