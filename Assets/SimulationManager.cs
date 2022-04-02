using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager instance;
    
    #region User Interface Variables

    [Header("User Interface")] 
    public GameObject nextTickGameObject;
    public GameObject playSimulationGameObject;
    public GameObject pauseSimulationGameObject;
    public GameObject simulationSpeedEntryField;
    public GameObject simulationTickCounterField;
    private Button playSimulationButton;
    private Button pauseSimulationButton;
    private Button nextTickButton;
    private TextMeshProUGUI simulationSpeedEntry;
    private TextMeshProUGUI simulationTickCounter;
    private int _simulationTickCount;
    
    public SimulationState simulationState;

    #endregion

    #region Scene Object Variables
    
    [Header("Object Variables")]
    public GameObject cellParent; //Gameobject to parent patch tiles
    public float cellTileSize;
    public float cellTileBorder;
    public GameObject cellPrefab;
    public GameObject[,] visualCells;
    public Color clearCellColor;
    public Color habitableCellColor;
    public Color occupiedCellColor;

    #endregion

    #region Simulation Variables
    
    [Header("Simulation Variables")]
    public List<ResourceAsset> globalResourceList;
    public Vector2 simulationDimentions;
    public List<SpeciesAgent> speciesList;
    public List<ResourceAsset> resourceList;
    public Cell[,] simulationArray;
    public Dictionary<Cell, int> habitats;
    public Dictionary<GameObject, Cell> cellLookupDictionary;

    #endregion
    
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
        
        
        InterfaceSetup();
    }


    #region SimulationInterface

    private void InterfaceSetup()
    {
        //Get all needed button components to control the ui
        playSimulationButton = playSimulationGameObject.GetComponent<Button>();
        pauseSimulationButton = pauseSimulationGameObject.GetComponent<Button>();
        nextTickButton = nextTickGameObject.GetComponent<Button>();
        simulationSpeedEntry = simulationSpeedEntryField.GetComponent<TextMeshProUGUI>();
        simulationTickCounter = simulationTickCounterField.GetComponent<TextMeshProUGUI>();
        
        pauseSimulationButton.interactable = false;
        simulationState = SimulationState.Paused;
    }

    public void PlayButtonPress()
    {
        simulationState = SimulationState.Playing;
        playSimulationButton.interactable = false;
        pauseSimulationButton.interactable = true;
    }

    public void PauseButtonPress()
    {
        simulationState = SimulationState.Paused;
        playSimulationButton.interactable = true;
        pauseSimulationButton.interactable = false;
    }

    public void NextTickButtonPress()
    {
        PauseButtonPress();

        _simulationTickCount++;
        simulationTickCounter.text = _simulationTickCount.ToString();
        
        CellDetailManager.instance.UpdateDetails();
        
        SimulationTick();
    }

    public void UpdateCellStates(KeyValuePair<GameObject, Cell> keyValuePair)
    {
        GameObject cellGameObject = keyValuePair.Key;
        Cell cell = keyValuePair.Value;

        switch (cell.currentState)
        {
            case CellState.clear:
                cellGameObject.GetComponent<Renderer>().material.SetColor("_Color", clearCellColor);
                cellGameObject.GetComponent<HabitatHook>().UpdateHook(false, 0);
                break;
            case CellState.habitable:
                cellGameObject.GetComponent<Renderer>().material.SetColor("_Color", habitableCellColor);
                break;
            case CellState.occupied:
                cellGameObject.GetComponent<Renderer>().material.SetColor("_Color", occupiedCellColor);
                if (habitats.ContainsKey(cell))
                {
                    cellGameObject.GetComponent<HabitatHook>().UpdateHook(true, habitats[cell]);
                }
                else
                {
                    habitats.Add(cell, habitats.Count);
                    cellGameObject.GetComponent<HabitatHook>().UpdateHook(true, habitats[cell]);
                }
                break;
        }
    }

    #endregion

    #region Simulation Functionality

    public void SimulationSetup(List<SpeciesAsset> _speciesAssets, List<ResourceAsset> _resourceAssets, List<ResourceAsset> _globalResourceAssets, Vector2 _simulationDimentions)
    {
        // nextTickButton.interactable = false;
        
        resourceList = _resourceAssets;
        habitats = new Dictionary<Cell, int>();
        globalResourceList = _globalResourceAssets;
        simulationDimentions = _simulationDimentions;
        cellLookupDictionary = new Dictionary<GameObject, Cell>();
        simulationArray = new Cell[(int) simulationDimentions.x, (int) simulationDimentions.y];
        speciesList = new List<SpeciesAgent>();
        
        float posX = (simulationDimentions.x / 2)*(cellTileSize+cellTileBorder);
        float posZ = (simulationDimentions.y / 2)*(cellTileSize+cellTileBorder);
        Vector3 nPosition = new Vector3(posX, 10, posZ);
        Camera.main.gameObject.transform.position = nPosition;
        Camera.main.gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);

        foreach (SpeciesAsset asset in _speciesAssets)
        {
            //convert asset to agent
            SpeciesAgent agent = new SpeciesAgent(asset.name, 0, asset.maxAge, asset.initialSpawnProbability,
                asset.childQuantityRangeProbability, asset.initialMigrationProbability, asset.maxMigrationDistance);

            Debug.Log($"Species: {agent.name}");
            //get requirements from serialized assets
            foreach (string key in asset.requirements.Keys)
            {
                Debug.Log($"asset requirement: {key}");
                //get resource asset by id
                foreach (ResourceAsset resource in globalResourceList)
                {
                    if (resource.id == key)
                    {
                        int value = asset.requirements[key];
                        agent.resourceRequirements.Add(resource, value);
                        break;
                    }
                }
            }
            
            speciesList.Add(agent);
        }

        for (int x = 0; x < simulationDimentions.x; x++)
        {
            for (int z = 0; z < simulationDimentions.y; z++)
            {
                Vector3 cellScenePosition = new Vector3(x * (cellTileSize + cellTileBorder), 0, z *
                    (cellTileSize + cellTileBorder));
                GameObject cellGameObject = Instantiate(cellPrefab, cellScenePosition, Quaternion.identity, cellParent.transform);
                Cell simulationCell = new Cell(speciesList, resourceList, new Vector2(x,z), simulationDimentions);
                simulationArray[x, z] = simulationCell;
                cellLookupDictionary.Add(cellGameObject, simulationCell);
            }
        }

        foreach (KeyValuePair<GameObject, Cell> _keyValuePair in cellLookupDictionary)
        {
            UpdateCellStates(_keyValuePair);
        }

        // nextTickButton.interactable = true;
    }

    public void SimulationTick()
    {
        nextTickButton.interactable = false;
        
        // foreach (KeyValuePair<GameObject, Cell> _keyValuePair in cellLookupDictionary)
        // {
        //     _keyValuePair.Value.CellTick();
        //     UpdateCellStates(_keyValuePair);
        // }

        for (int i = 0; i < cellLookupDictionary.Count; i++)
        {
            KeyValuePair<GameObject, Cell> kvp = cellLookupDictionary.ElementAt(i);
            MigrateAgents(kvp.Value.CellTick()); //update each cell, and migrate the agents
            
        }
        
        nextTickButton.interactable = true;
    }

    public void MigrateAgents(Dictionary<SpeciesAgent, Vector2> migratoryAgents)
    {
        foreach (KeyValuePair<SpeciesAgent, Vector2> kvp in migratoryAgents)
        {
            
        }
    }
    
    #endregion
}

public class Cell
{
    public Vector2 cellCoords;
    public Dictionary<SpeciesAgent, List<SpeciesAgent>> speciesPopulation;
    public Dictionary<SpeciesAgent, List<Vector2>> possibleDestinations;
    public Dictionary<ResourceAsset, float> resoursePopulation;
    Dictionary<ResourceAsset, int> resourceConsumption;
    
    public CellState currentState;
    
    public Cell(List<SpeciesAgent> _species, List<ResourceAsset> _resourceAssets, Vector2 _cellCoords, Vector2 simulationDimentions)
    {
        this.cellCoords = _cellCoords;
        resoursePopulation = new Dictionary<ResourceAsset, float>();
        speciesPopulation = new Dictionary<SpeciesAgent, List<SpeciesAgent>>();
        resourceConsumption = new Dictionary<ResourceAsset, int>();
        possibleDestinations = new Dictionary<SpeciesAgent, List<Vector2>>();

        foreach (ResourceAsset asset in _resourceAssets)
        {
            
            float probability = Random.Range(0.0f, 1.0f);
            Debug.Log($"Adding: {asset.name} with r {probability}");
            if (probability >= (1 - asset.spawnProbability))
            {
                resoursePopulation.Add(asset, Random.Range(asset.minQuantity, asset.maxQuantity));
                Debug.Log($"Added {asset.name}");
            }
        }

        foreach (SpeciesAgent species in _species)
        {
            bool habitable = true;
            foreach (ResourceAsset resource in species.resourceRequirements.Keys)
            {
                if (resoursePopulation.ContainsKey(resource)) 
                {
                    if (resoursePopulation[resource] < species.resourceRequirements[resource])
                    {
                        habitable = false;
                    }
                }
                else
                {
                    habitable = false;
                }
            }

            if (habitable)
            {
                speciesPopulation.Add(species, new List<SpeciesAgent>());
            }
            
            possibleDestinations.Add(species, new List<Vector2>());

            int minX = (int) Mathf.Clamp(cellCoords.x - species.maxMigrationDistance, 0, simulationDimentions.x);
            int maxX = (int) Mathf.Clamp(cellCoords.x + species.maxMigrationDistance, 0, simulationDimentions.x);
            
            int minY = (int) Mathf.Clamp(cellCoords.y - species.maxMigrationDistance, 0, simulationDimentions.y);
            int maxY = (int) Mathf.Clamp(cellCoords.y + species.maxMigrationDistance, 0, simulationDimentions.y);

            for (int i = minX; i >= maxX; i++)
            {
                for (int j = minY; j >= maxY; j++)
                {
                    possibleDestinations[species].Add(new Vector2(i,j)); //added destinations might not be habitable
                }
            }

        }

        foreach (SpeciesAgent species in speciesPopulation.Keys)
        {
            //Is Habitable to all here
            Debug.Log($"checking {species.name}");
            float probability = Random.Range(0.0f, 1.0f);
            float count = Random.Range(species.childQuantityRangeProbability.x,
                species.childQuantityRangeProbability.y);
            
            if (probability >= species.spawnProbability)
            {
                Debug.Log($"adding {species.name} * {count}");
                for (int i = 0; i < count; i++)
                {
                    SpeciesAgent agent = new SpeciesAgent(species.name, 0, species.maxAge, species.spawnProbability, species.childQuantityRangeProbability, species.initialMigrationProbability, species.maxMigrationDistance);
                    agent.resourceRequirements = species.resourceRequirements;
                    agent.location = cellCoords;
                    agent.currentCell = this;

                    speciesPopulation[species].Add(agent);
                }
            }
        }
        
        //update view
        if (speciesPopulation.Keys.Count > 0)
        {
            currentState = CellState.habitable;
            foreach (SpeciesAgent species in speciesPopulation.Keys)
            {
                if (speciesPopulation[species].Count > 0)
                {
                    currentState = CellState.occupied;
                }
            }
        }
    }

    public Dictionary<SpeciesAgent, Vector2> CellTick()
    {
        Dictionary<SpeciesAgent, Vector2> migratingAgents = new Dictionary<SpeciesAgent, Vector2>();
        //update consumption amounts
        foreach (KeyValuePair<SpeciesAgent, List<SpeciesAgent>> speciesKeyValuePair in speciesPopulation)
        {
            List<SpeciesAgent> deadAgents = new List<SpeciesAgent>();
            List<SpeciesAgent> newAgents = new List<SpeciesAgent>();

            //process consumption of resources
            foreach (ResourceAsset resource in speciesKeyValuePair.Key.resourceRequirements.Keys)
            {
                if(!resourceConsumption.ContainsKey(resource))
                {
                    resourceConsumption.Add(resource,
                        (speciesKeyValuePair.Key.resourceRequirements[resource] * speciesKeyValuePair.Value.Count));
                }
                else
                {
                    resourceConsumption[resource] = (speciesKeyValuePair.Key.resourceRequirements[resource] * speciesKeyValuePair.Value.Count);
                }
            }
            
            // Tick Agent
            foreach (SpeciesAgent agent in speciesKeyValuePair.Value)
            {
                 //Kill dead agents
                 if (agent.age >= agent.maxAge)
                 {
                     deadAgents.Add(agent);
                 }
                 
                 //spawn new children
                 if ((agent.carryChild && agent.age == agent.birthAge))
                 {
                     Debug.Log("calculating childs");
                     int childCount = (int) Random.Range(agent.childQuantityRangeProbability.x,
                         agent.childQuantityRangeProbability.y);
                     Debug.Log($"{childCount} {agent.name}s have been born at {cellCoords}");
                     for (int i = 0; i < childCount; i++)
                     {
                         SpeciesAgent child = new SpeciesAgent(agent.name, 0, agent.maxAge, agent.spawnProbability, agent.childQuantityRangeProbability, agent.initialMigrationProbability, agent.maxMigrationDistance);
                         child.resourceRequirements = agent.resourceRequirements;
                         
                         newAgents.Add(child);
                     }
                 }
                 
                 //process migrating agents
                 if (agent.willMigrate && agent.age == agent.migrateAge)
                 {
                     //mark for migration
                     // List<Vector2> habitableDestinations = possibleDestinations[agent.name]
                 }
                 
                 
                 // if (agent.willMigrate && agent.age == agent.migrateAge)
                 // {
                 //     Vector2 newCell = possibleMigrationCells[Random.Range(0, possibleMigrationCells.Count-1)];
                 //     migratedAgents.Add(agent, newCell);
                 //     deadAgents.Add(agent); //remove from this cell
                 // }
                 
                 agent.Tick();
            }
            
            //Add new and remove dead agents from lists
            foreach (SpeciesAgent agent in newAgents)
             {
                 foreach (SpeciesAgent speciesAgent in speciesPopulation.Keys)
                 {
                     if (speciesAgent.name == agent.name)
                     {
                         speciesPopulation[speciesAgent].Add(agent);
                         Debug.Log("New Agent!");
                     }
                 }
             }
            foreach (SpeciesAgent agent in deadAgents){
                 foreach (SpeciesAgent speciesAgent in speciesPopulation.Keys)
                 {
                     if (speciesAgent.name == agent.name)
                     {
                         speciesPopulation[speciesAgent].Remove(agent);
                         Debug.Log("Killed Agent!");
                     }
                 }
             }
            
            //Process the migrated agents 
            // foreach (KeyValuePair<SpeciesAgent, Vector2> migration in migratedAgents)
            // {
            //     Debug.Log("calculating migration");
            //     foreach (KeyValuePair<SpeciesAgent, List<SpeciesAgent>> species in SimulationManager.instance.simulationArray[(int) migration.Value.x, (int) migration.Value.y].speciesPopulation)
            //     {
            //         if (species.Key.name == migration.Key.name)
            //         {
            //             SpeciesAgent agent = migration.Key;
            //             agent.migrated = true;
            //             species.Value.Add(agent);
            //             Debug.Log("Migrated!");
            //         }
            //     }
            // }
            
            
        }
        
        // update resources
        for (int i = 0; i < resoursePopulation.Keys.Count; i++)
        {
            ResourceAsset resourceAsset = resoursePopulation.ElementAt(i).Key;
            float currentQuantity = resoursePopulation.ElementAt(i).Value;
            float newQuantity = currentQuantity + (resourceAsset.growthRate*currentQuantity);

            //subtract consumption
            if (resourceConsumption.ContainsKey(resourceAsset))
            {
                newQuantity -= resourceConsumption[resourceAsset];
            }
            
            resoursePopulation[resourceAsset] = Mathf.Clamp(newQuantity, 0, resourceAsset.capacity);
        }
        
        //update State
        foreach (SpeciesAgent species in speciesPopulation.Keys)
        {
            bool habitable = true;
            foreach (ResourceAsset resource in species.resourceRequirements.Keys)
            {
                if (resoursePopulation.ContainsKey(resource))
                {
                    if (resoursePopulation[resource] < species.resourceRequirements[resource])
                    {
                        habitable = false;
                    }
                }
                else
                {
                    habitable = false;
                }
            }

            if (habitable)
            {
                if (speciesPopulation[species].Count > 0)
                {
                    currentState = CellState.occupied;
                }
                else
                {
                    currentState = CellState.habitable;
                }
            }
            else
            {
                currentState = CellState.clear;
            }
        }

        return migratingAgents;
    }
}

public class SpeciesAgent
{
    public Cell currentCell;
    public string name;
    public int age;
    public int maxAge;
    public float spawnProbability;
    public Vector3 childQuantityRangeProbability;
    public float initialMigrationProbability;
    public int maxMigrationDistance;
    public Vector2 location;
    public Dictionary<ResourceAsset, int> resourceRequirements;

    public int childCount;
    public bool carryChild;
    public bool willMigrate;
    public int migrateAge;
    public int birthAge;
    public bool migrated;

    public SpeciesAgent(string _name, int _age, int _maxAge, float _spawnProbability, Vector3 _childQuantityRangeProbability, float _initialMigrationProbability, int _maxMigrationDistance)
    {
        name = _name;
        age = _age;
        maxAge = _maxAge;
        spawnProbability = _spawnProbability;
        childQuantityRangeProbability = _childQuantityRangeProbability;
        initialMigrationProbability = _initialMigrationProbability;
        maxMigrationDistance = maxMigrationDistance;
        
        resourceRequirements = new Dictionary<ResourceAsset, int>();
        migrated = false;
        
        float r = Random.Range(0.0f, 1.0f);
        Debug.Log($"r: {r}, p: {childQuantityRangeProbability.z}");
        if (r >= (1-childQuantityRangeProbability.z))
        {
            Debug.Log("Has Child");
            carryChild = true;
            childCount = (int) Random.Range(childQuantityRangeProbability.x, childQuantityRangeProbability.y);
            birthAge = Random.Range(1, maxAge);
            
        }
        else
        {
            carryChild = false;
        }
        
        r = Random.Range(0.0f, 1.0f);
        if (r >= (1-initialMigrationProbability))
        {
            willMigrate = true;
            migrateAge = Random.Range(1, maxAge);
            Debug.Log("Will Migrate!");
            carryChild = true;
            childCount = (int) Random.Range(childQuantityRangeProbability.x, childQuantityRangeProbability.y);
            birthAge = Random.Range(1, maxAge);
            //If migratory it should be able to have offspring when migrating.
        }
        else
        {
            willMigrate = false;
        }
    }

    public void Tick()
    {
        age++;
    }
}

public enum SimulationState
{
    Paused,
    Playing,
}

public enum CellState
{
    clear,
    habitable,
    occupied,
}