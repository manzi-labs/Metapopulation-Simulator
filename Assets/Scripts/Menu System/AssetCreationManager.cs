using System;
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
    public Color speciesColor;
    public int speciesMinChildCount;
    public int speciesMaxChildCount;
    public float speciesChildProbability;
    public int speciesMaxMigrationRange;
    public float speciesMigrationProbability;
    public float speciesInitSpawnProbability;

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

    public GameObject simulationListGrid;
    public GameObject simulationListGridEntry;
    public List<GameObject> simulationListEntries;

    public GameObject resourceSimList;
    public GameObject speciesSimList;
    public List<GameObject> simResources;
    public List<GameObject> simSpecies;
    public GameObject resourceEntry;
    public GameObject speciesEntry;

    public List<ResourceAsset> globalResources;
    public List<SpeciesAsset> globalSpecies;
    public List<SimulationAsset> globalSimulations;

    public FileUtility _fileUtility;

    #region UnityFunctions

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("instance set");
        }
        else
        {
            Destroy(this);
        }
        
        _fileUtility = new FileUtility();
        
        //populate grids
        globalResources = new List<ResourceAsset>();
        globalSimulations = new List<SimulationAsset>();
        globalSpecies = new List<SpeciesAsset>();
        
        PopulateGrids();    
        
        resourceColor = Color.clear;
        speciesColor = Color.clear;

        simResources = new List<GameObject>();
        simSpecies = new List<GameObject>();

        if (_fileUtility.GetResourceList().Count == 0 || _fileUtility.GetSimulationList().Count == 0 ||
            _fileUtility.GetSpeciesList().Count == 0)
        {
            CreateDemoAssets();
        }
    }

    private void CreateDemoAssets()
    {
        ResourceAsset demoResource = new ResourceAsset();
        SpeciesAsset demoSpecies = new SpeciesAsset();
        SimulationAsset demoSimulation = new SimulationAsset();

        demoResource.name = "Carrot";
        demoResource.capacity = 30;
        demoResource.growthRate = 0.9f;
        demoResource.indicatorColor = "FEA47F";
        demoResource.maxQuantity = 10;
        demoResource.minQuantity = 5;
        demoResource.spawnProbability = 0.3f;

        demoSpecies.name = "Rabbit";
        demoSpecies.requirements = new Dictionary<string, int>();
        demoSpecies.requirements.Add(demoResource.id, 2);
        demoSpecies.maxAge = 10;
        demoSpecies.visColour = "FBC531";
        demoSpecies.initialMigrationProbability = 0.2f;
        demoSpecies.initialSpawnProbability = 0.6f;
        demoSpecies.childQuantityRangeProbability = new Vector3(1, 8, 0.3f);
        demoSpecies.maxMigrationRange = 3;

        demoSimulation.name = "Demo Rabbit Simulation";
        demoSimulation.simSize = new Vector2(10, 10);
        demoSimulation.resourceList = new List<string>();
        demoSimulation.resourceList.Add(demoResource.id);
        demoSimulation.speciesList = new List<string>();
        demoSimulation.speciesList.Add(demoSpecies.id);
        
        _fileUtility.SaveSpecies(demoSpecies);
        _fileUtility.SaveResource(demoResource);
        _fileUtility.SaveSimulation(demoSimulation);
        
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
        resourceName = _name.ToUpper();
    }
    
    public void SetResourceMinProbability(string _min)
    {
        int parsed;
        int.TryParse(_min, out parsed);

        if (parsed != null)
        {
            resourceMinQuantity = parsed;
        }
        
    }

    public void SetResourceMaxProbability(string _max)
    {
        int parsed;
        int.TryParse(_max, out parsed);

        if (parsed != null)
        {
            resourceMaxQuantity = parsed;
        }
    }

    public void SetCellCarryCapacity(string _cap)
    {
        int parsed;
        int.TryParse(_cap, out parsed);

        if (parsed != null)
        {
            resourceCapacity = parsed;
        }
    }

    public void SetResourceSpawnProbability(string _prob)
    {
        float parsed;
        float.TryParse(_prob, out parsed);

        if (parsed != null)
        {
            resourceSpawnProbability = parsed;
        }
    }

    public void SetResourceGrowthRate(string _rate)
    {
        float parsed;
        float.TryParse(_rate, out parsed);

        if (parsed != null)
        {
            growthRate = parsed;
        }
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
        PersistantSceneManager.instance.ConfirmEffect();
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
        speciesName = _name.ToUpper();
    }

    public void SetSpeciesMaxAge(string _age)
    {
        int parsed;
        int.TryParse(_age, out parsed);
        
        if(parsed != null)
        {
            speciesMaxAge = parsed;
        }
    }

    public void SetSpeciesMinChildCount(string _min)
    {
        int parsed;
        int.TryParse(_min, out parsed);

        if (parsed != null)
        {
            speciesMinChildCount = parsed;
        }
    }
    
    public void SetSpeciesMaxChildCount(string _max)
    {
        int parsed;
        int.TryParse(_max, out parsed);

        if (parsed != null)
        {
            speciesMaxChildCount = parsed;
        }
    }
    
    public void SetSpeciesChildProbability(string _probability)
    {
        float parsed;
        float.TryParse(_probability, out parsed);

        if (parsed != null)
        {
            speciesChildProbability = parsed;
        }
    }

    public void SetSpeciesMaxMigrationRange(string _range)
    {
        int parsed;
        int.TryParse(_range, out parsed);

        if (parsed != null)
        {
            speciesMaxMigrationRange = parsed;
        }
    }
    
    public void SetSpeciesMigrationProbability(string _probability)
    {
        float parsed;
        float.TryParse(_probability, out parsed);

        if (parsed != null)
        {
            speciesMigrationProbability = parsed;
        }
    }
    
    public void SetSpeciesSpawnProbability(string _probability)
    {
        float parsed;
        float.TryParse(_probability, out parsed);

        if (parsed != null)
        {
            speciesInitSpawnProbability = parsed;
        }
    }

    public void AddResource()
    {
        this.GetComponent<MenuController>().AddResourceButton();
        PersistantSceneManager.instance.ConfirmEffect();
    }

    public void SaveSpecies()
    {
        SpeciesAsset speciesAsset = new SpeciesAsset();
        speciesAsset.name = speciesName;
        speciesAsset.visColour = ColorUtility.ToHtmlStringRGB(speciesColor);
        speciesAsset.initialSpawnProbability = speciesInitSpawnProbability;
        speciesAsset.initialMigrationProbability = speciesMigrationProbability;
        speciesAsset.maxMigrationRange = speciesMaxMigrationRange;
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
        PersistantSceneManager.instance.ConfirmEffect();

        _fileUtility.SaveSpecies(speciesAsset);
    }

    #endregion

    #region Simulations

    public void SetSimulationName(string _name)
    {
        simulationName = _name.ToUpper();
    }

    public void SetSimulationSizeX(string _x)
    {
        int parsed;
        int.TryParse(_x, out parsed);

        if (parsed != null)
        {
            simulationSize.x = parsed;
        }
    }

    public void SetSimulationSizeY(string _y)
    {
        int parsed;
        int.TryParse(_y, out parsed);

        if (parsed != null)
        {
            simulationSize.y = parsed;
        }
    }

    public void SaveSimulation()
    {
        SimulationAsset newSimulationAsset = new SimulationAsset
        {
            name = simulationName, 
            simSize = simulationSize
        };

        List<string> resourcesL = new List<string>();
        List<string> speciesL = new List<string>();
        
        foreach (GameObject resource in simResources)
        {
            ResourceAsset resourceAsset = resource.GetComponent<ResourceListSimHook>().resourceAsset;
            resourcesL.Add(resourceAsset.id);
            // resourcesL.Add(resourceAsset.id);
            PersistantSceneManager.instance.resources.Add(resourceAsset);
        }
        foreach (GameObject species in simSpecies)
        {
            SpeciesAsset speciesAsset = species.GetComponent<SpeciesListSimHook>().speciesAsset;
            speciesL.Add(speciesAsset.id);
            PersistantSceneManager.instance.species.Add(speciesAsset);
        }

        newSimulationAsset.resourceList = resourcesL;
        newSimulationAsset.speciesList = speciesL;
        
        _fileUtility.SaveSimulation(newSimulationAsset);

        
        PersistantSceneManager.instance.simulationName = simulationName;
        PersistantSceneManager.instance.simulationSize = simulationSize;
        PersistantSceneManager.instance.fullResourceList = globalResources;
        PersistantSceneManager.instance.ConfirmEffect();
        PersistantSceneManager.instance.StartSimulation();
    }

    public void LoadSimulation(SimulationAsset asset)
    {
        PersistantSceneManager.instance.simulationName = asset.name;
        PersistantSceneManager.instance.simulationSize = asset.simSize;
        PersistantSceneManager.instance.fullResourceList = globalResources;
        PersistantSceneManager.instance.resources = new List<ResourceAsset>();
        PersistantSceneManager.instance.species = new List<SpeciesAsset>();
        
        foreach (string resourceid in asset.resourceList)
        {
            ResourceAsset resource = null;
            foreach (ResourceAsset entry in _fileUtility.GetResourceList())
            {
                if (entry.id == resourceid)
                {
                    resource = entry;
                    break;
                }
            }

            if (resource == null)
            {
                Debug.Log($"Not found resource! {resourceid}");
            }
            else
            {
                PersistantSceneManager.instance.resources.Add(resource);
            }
        }
        
        foreach (string speciesid in asset.speciesList)
        {
            SpeciesAsset species = null;
            foreach (SpeciesAsset entry in _fileUtility.GetSpeciesList())
            {
                if (entry.id == speciesid)
                {
                    species = entry;
                    break;
                }
            }

            if (species == null)
            {
                Debug.Log($"Not found species! {speciesid}");
            }
            else
            {
                PersistantSceneManager.instance.species.Add(species);
            }
        }
        PersistantSceneManager.instance.ConfirmEffect();

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
        resourceListEnteries.Clear();
        speciesGridEnteries.Clear();
        simulationListEntries.Clear();

        foreach (Transform child in resourceListGrid.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in speciesListGrid.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in simulationListGrid.transform)
        {
            Destroy(child.gameObject);
        }
        
        
        foreach (ResourceAsset resource in _fileUtility.GetResourceList())
        {
            if(!globalResources.Contains(resource))
            {
                Debug.Log("Added resource to list");
                GameObject resourceEntry = Instantiate(resourceListGridEntry, Vector3.zero, Quaternion.identity,
                    resourceListGrid.transform);

                resourceEntry.GetComponent<ResourceListHook>().Init(resource);
                resourceListEnteries.Add(resourceEntry);
                globalResources.Add(resource);
            }
        }

        foreach (SpeciesAsset species in _fileUtility.GetSpeciesList())
        {
            if (!globalSpecies.Contains(species))
            {
                GameObject speciesEntry = Instantiate(speciesListGridEntry, Vector3.zero, Quaternion.identity,
                    speciesListGrid.transform);
            
                speciesEntry.GetComponent<SpeciesListHook>().Init(species);
                speciesGridEnteries.Add(speciesEntry);
                globalSpecies.Add(species);
            }
            
        }

        foreach (SimulationAsset simulation in _fileUtility.GetSimulationList())
        {
            if (!globalSimulations.Contains(simulation))
            {
                GameObject simulationEntry = Instantiate(simulationListGridEntry, Vector3.zero, Quaternion.identity,
                    simulationListGrid.transform);
            
                simulationEntry.GetComponent<SimulationListHook>().Init(simulation);
                simulationListEntries.Add(simulationEntry);
                globalSimulations.Add(simulation);
            }
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
