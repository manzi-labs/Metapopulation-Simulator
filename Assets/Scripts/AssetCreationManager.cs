using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class AssetCreationManager : MonoBehaviour
{
    public static AssetCreationManager instance;
    public Button saveResourceButton;
    public Button saveSpeciesButton;
    public Button startSimulationButton;
    
    private SpeciesAsset _newSpeciesAsset;
    private ResourceAsset _newResourceAsset;
    
    //Species variables
    public string speciesName;
    public int speciesMaxAge;
    public int speciesMinChildCount;
    public int speciesMaxChildCount;
    public float speciesChildProbability;
    public float speciesMigrationProbability;
    public float speciesInitSpawnProbability;
    public Color speciesColor;

    //Resource variables
    public string resourceName;
    public float resourceSpawnProbability;
    public int resourceMinQuantity;
    public int resourceMaxQuantity;
    public int resourceCapacity;
    public float growthRate;
    public Color resourceColor;
    
    //Simulation variables
    public string simulationName;
    public Vector2 simulationSize;

    //grids
    public GameObject resourceListGrid;
    public GameObject resourceListGridEntry;
    public List<GameObject> resourceListEnteries;
    
    public GameObject speciesListGrid;
    public GameObject speciesListGridEntry;
    public List<GameObject> speciesGridEnteries;
    
    public GameObject resourceRequirementsGrid;
    public GameObject resourceRequirementsGridEntry;
    public List<GameObject> resourceRequirementsGridEntries;

    public GameObject resourceSimList;
    public GameObject speciesSimList;
    public List<GameObject> simResources;
    public List<GameObject> simSpecies;
    public GameObject resourceEntry;
    public GameObject speciesEntry;

    public List<ResourceAsset> globalResources;

    public FileUtility _fileUtility;

    #region UnityFunctions

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
        
        _fileUtility = new FileUtility();
        
        //populate grids
        globalResources = new List<ResourceAsset>();
        
        PopulateGrids();    
        
        resourceColor = Color.clear;
        speciesColor = Color.clear;

        simResources = new List<GameObject>();
        simSpecies = new List<GameObject>();
    }

    void FixedUpdate()
    {
        //Save buttons functionality toggles
        if (resourceName == "" || resourceSpawnProbability == 0 || resourceColor == Color.clear)
        {
            saveResourceButton.interactable = false;
        }
        else
        {
            saveResourceButton.interactable = true;
        }

        if (speciesName == "" || speciesMaxAge == 0 || speciesColor == Color.clear ||
            resourceRequirementsGridEntries.Count == 0)
        {
            saveSpeciesButton.interactable = false;
        }
        else
        {
            saveSpeciesButton.interactable = true;
        }

        if (simulationName == "" || simulationSize == Vector2.zero || simResources.Count == 0 || simSpecies.Count == 0)
        {
            startSimulationButton.interactable = false;
        }
        else
        {
            startSimulationButton.interactable = true;
        }
    }

    #endregion

    #region Resources

    public void SetResourceName(string _name)
    {
        resourceName = _name;
    }
    
    public void SetResourceMinProbability(string _min)
    {
        resourceMinQuantity = int.Parse(_min);
    }

    public void SetResourceMaxProbability(string _max)
    {
        resourceMaxQuantity = int.Parse(_max);
    }

    public void SetCellCarryCapacity(string _cap)
    {
        resourceCapacity = int.Parse(_cap);
    }

    public void SetResourceSpawnProbability(string _prob)
    {
        resourceSpawnProbability = float.Parse(_prob);
    }

    public void SetResourceGrowthRate(string _rate)
    {
        growthRate = float.Parse(_rate);
    }

    public void SaveResource()
    {
        ResourceAsset resourceAsset = new ResourceAsset();
        resourceAsset.name = resourceName;
        resourceAsset.spawnProbability = resourceSpawnProbability;
        resourceAsset.minQuantity = resourceMinQuantity;
        resourceAsset.maxQuantity = resourceMaxQuantity;
        resourceAsset.capacity = resourceCapacity;
        resourceAsset.growthRate = growthRate;
        resourceAsset.indicatorColor = ColorUtility.ToHtmlStringRGB(resourceColor);
        _fileUtility.SaveResource(resourceAsset);
        resourceName = "";
        resourceSpawnProbability = 0;
        resourceMinQuantity = 0;
        resourceMaxQuantity = 0;
        resourceColor = Color.clear;
    }

    #endregion

    #region Species

    public void SetSpeciesName(string _name)
    {
        speciesName = _name;
    }

    public void SetSpeciesMaxAge(string _age)
    {
        speciesMaxAge = int.Parse(_age);
    }

    public void SetSpeciesMinChildCount(string _min)
    {
        speciesMinChildCount = int.Parse(_min);
    }
    
    public void SetSpeciesMaxChildCount(string _max)
    {
        speciesMaxChildCount = int.Parse(_max);
    }
    
    public void SetSpeciesChildProbability(string _probability)
    {
        speciesChildProbability = float.Parse(_probability);
    }
    
    public void SetSpeciesMigrationProbability(string _probability)
    {
        speciesMigrationProbability = float.Parse(_probability);
    }
    
    public void SetSpeciesSpawnProbability(string _probability)
    {
        speciesInitSpawnProbability = float.Parse(_probability);
    }

    public void AddResource()
    {
        this.GetComponent<MenuController>().AddResourceButton();
    }

    public void SaveSpecies()
    {
        SpeciesAsset speciesAsset = new SpeciesAsset();
        speciesAsset.name = speciesName;
        speciesAsset.indicatorColor = ColorUtility.ToHtmlStringRGB(speciesColor);
        speciesAsset.initialSpawnProbability = speciesInitSpawnProbability;
        speciesAsset.initialMigrationProbability = speciesMigrationProbability;
        speciesAsset.maxAge = speciesMaxAge;
        speciesAsset.childQuantityRangeProbability =
            new Vector3(speciesMinChildCount, speciesMaxChildCount, speciesChildProbability);
        
        //convert prefab requirements to dictionary
        Dictionary<string, int> resourceRequirements = new Dictionary<string, int>();
        foreach (GameObject requirement in resourceRequirementsGridEntries)
        {
            ResourceRequirementHook hook = requirement.GetComponent<ResourceRequirementHook>();
            resourceRequirements.Add(hook.ResourceAsset.id, hook.quantity);
        }

        speciesAsset.requirements = resourceRequirements;
        
        _fileUtility.SaveSpecies(speciesAsset);
    }

    #endregion

    #region Simulations

    public void SetSimulationName(string _name)
    {
        simulationName = _name;
    }

    public void SetSimulationSizeX(string _x)
    {
        simulationSize.x = int.Parse(_x);
    }

    public void SetSimulationSizeY(string _y)
    {
        simulationSize.y = int.Parse(_y);
    }

    public void SaveSimulation()
    {
        SimulationAsset newSimulationAsset = new SimulationAsset
        {
            name = simulationName, 
            simSize = simulationSize
        };

        // List<string> resourcesL = new List<string>();
        // List<string> speciesL = new List<string>();
        
        foreach (GameObject resource in simResources)
        {
            ResourceAsset resourceAsset = resource.GetComponent<ResourceListSimHook>().resourceAsset;
            // resourcesL.Add(resourceAsset.id);
            PersistantSceneManager.instance.resources.Add(resourceAsset);
        }
        foreach (GameObject species in simSpecies)
        {
            SpeciesAsset speciesAsset = species.GetComponent<SpeciesListSimHook>().speciesAsset;
            // speciesL.Add(speciesAsset.id);
            PersistantSceneManager.instance.species.Add(speciesAsset);
        }

        // newSimulationAsset.resourceList = resourcesL;
        // newSimulationAsset.speciesList = speciesL;
        
        // _fileUtility.SaveSimulation(newSimulationAsset);
        
        //Start Simulation
        PersistantSceneManager.instance.simulationName = simulationName;
        PersistantSceneManager.instance.simulationSize = simulationSize;
        PersistantSceneManager.instance.fullResourceList = globalResources;
        
        PersistantSceneManager.instance.StartSimulation();
    }

    #endregion
    
    public void AddedResource(ResourceAsset resourceAsset)
    {
        //check currentpage to add resource to correct menu
        Debug.Log($"content route: {MenuController.instance._contentRoute}");

        if (MenuController.instance._contentRoute == UIScreens.NEWSIMULATION)
        {
            GameObject entry = Instantiate(resourceEntry, Vector3.zero, Quaternion.identity, resourceSimList.transform);
            
            entry.GetComponent<ResourceListSimHook>().Init(resourceAsset);
            simResources.Add(entry);
            MenuController.instance.BackButtons();
        }
        else
        {
            GameObject entry = Instantiate(resourceRequirementsGridEntry, Vector3.zero, Quaternion.identity,
                resourceRequirementsGrid.transform);
        
            entry.GetComponent<ResourceRequirementHook>().Init(resourceAsset);
        
            resourceRequirementsGridEntries.Add(entry);
            MenuController.instance.BackButtons();
        }
    }
    public void AddedSpecies(SpeciesAsset speciesAsset)
    {
        Debug.Log($"content route: {MenuController.instance._contentRoute}");
        if (MenuController.instance._contentRoute == UIScreens.NEWSIMULATION)
        {
            GameObject entry = Instantiate(speciesEntry, Vector3.zero, Quaternion.identity, speciesSimList.transform);
            
            entry.GetComponent<SpeciesListSimHook>().Init(speciesAsset);
            simSpecies.Add(entry);
            MenuController.instance.BackButtons();
        }
    }
    public void PopulateGrids()
    {
        foreach (GameObject entry in resourceListEnteries)
        {
            Destroy(entry);
        }
        foreach (GameObject entry in speciesGridEnteries)
        {
            Destroy(entry);
        }
        resourceListEnteries.Clear();
        speciesGridEnteries.Clear();
        
        foreach (ResourceAsset resource in _fileUtility.GetResourceList())
        {
            Debug.Log("Added resource to list");
            GameObject resourceEntry = Instantiate(resourceListGridEntry, Vector3.zero, Quaternion.identity,
                resourceListGrid.transform);
            
            resourceEntry.GetComponent<ResourceListHook>().Init(resource);
            resourceListEnteries.Add(resourceEntry);
            globalResources.Add(resource);
        }

        foreach (SpeciesAsset species in _fileUtility.GetSpeciesList())
        {
            GameObject speciesEntry = Instantiate(speciesListGridEntry, Vector3.zero, Quaternion.identity,
                speciesListGrid.transform);
            
            speciesEntry.GetComponent<SpeciesListHook>().Init(species);
            speciesGridEnteries.Add(speciesEntry);
        }
        
    }
    
    public void SetColor(string _color)
    {
        Color newCol;
        if (ColorUtility.TryParseHtmlString(_color, out newCol))
        {
            if (this.GetComponent<MenuController>()._currentScreen == UIScreens.NEWSPECIES)
            {
                speciesColor = newCol;
            }
            else if (this.GetComponent<MenuController>()._currentScreen == UIScreens.NEWRESOURCE)
            {
                resourceColor = newCol;
            }
        }
    }
}
