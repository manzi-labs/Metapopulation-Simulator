using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public GameObject simulationCellDetailPanel;
    public GameObject quitSimulationPanel;
    public Slider volume;
    public bool cellPanelState;
    public bool paused = false;
    private Button playSimulationButton;
    private Button pauseSimulationButton;
    private Button nextTickButton;
    private TextMeshProUGUI simulationSpeedEntry;
    private TextMeshProUGUI simulationTickCounter;
    private int _simulationTickCount;

    public TextMeshProUGUI totalPopulationUI;
    
    public SimulationState simulationState;

    #endregion

    #region Scene Object Variables
    
    [Header("Object Variables")]
    public GameObject cellParent; //Gameobject to parent patch tiles
    public float cellTileSize;
    public float cellTileBorder;
    public GameObject cellPrefab;
    public GameObject migrationInstancePrefab;
    public GameObject[,] visualArray;
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
    public SimulationCell[,] simulationArray;
    public Dictionary<SimulationCell, int> habitats;
    public Dictionary<GameObject, SimulationCell> cellLookupDictionary;
    public Dictionary<string, int> totalPopulation;

    #endregion
    
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }

        cellPanelState = false;
        simulationCellDetailPanel.SetActive(cellPanelState);
        
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

    public void ShowHidePanel()
    {
        cellPanelState = !cellPanelState;

        simulationCellDetailPanel.SetActive(cellPanelState);
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
        
        // CellDetailManager.instance.UpdateDetails();
        
        SimulationTick();
    }
    

    #endregion

    #region Simulation Functionality

    public void SimulationSetup(List<SpeciesAsset> _speciesAssets, List<ResourceAsset> _resourceAssets, List<ResourceAsset> _globalResourceAssets, Vector2 _simulationDimentions)
    {
        // nextTickButton.interactable = false;
        Debug.Log($"RECIEVED DATA - SIM STARTUP {_resourceAssets.Count} , {_speciesAssets.Count}");
        resourceList = _resourceAssets;
        habitats = new Dictionary<SimulationCell, int>();
        globalResourceList = _globalResourceAssets;
        simulationDimentions = _simulationDimentions;
        cellLookupDictionary = new Dictionary<GameObject, SimulationCell>();
        simulationArray = new SimulationCell[(int) simulationDimentions.x, (int) simulationDimentions.y];
        visualArray = new GameObject[(int) simulationDimentions.x, (int) simulationDimentions.y];
        speciesList = new List<SpeciesAgent>();
        
        float posX = (simulationDimentions.x / 2)*(cellTileSize+cellTileBorder);
        float posZ = (simulationDimentions.y / 2)*(cellTileSize+cellTileBorder);
        Vector3 nPosition = new Vector3(posX, 10, posZ);
        Camera.main.gameObject.transform.position = nPosition;
        Camera.main.gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);

        foreach (SpeciesAsset asset in _speciesAssets)
        {
            Debug.Log("ASSET RANGE: "+asset.maxMigrationRange);
            //convert asset to agent
            SpeciesAgent agent = new SpeciesAgent(asset.name, 0, asset.maxAge, asset.initialSpawnProbability,
                asset.childQuantityRangeProbability, asset.initialMigrationProbability, asset.maxMigrationRange);

            Debug.Log($"Species: {agent.name} loaded");
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
                        agent.resourceRequirements.Add(resource.id, value);
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
                SimulationCell simulationSimulationCell = new SimulationCell(speciesList, resourceList, new Vector2(x,z), simulationDimentions);
                simulationArray[x, z] = simulationSimulationCell;
                cellLookupDictionary.Add(cellGameObject, simulationSimulationCell);
                visualArray[x, z] = cellGameObject;
                UpdateCellVisual(x,z);
            }
        }
    }

    public void UpdateCellVisual(int x, int y)
    {
        GameObject cellObject = visualArray[x,y];

        SimulationCell simulationCell = simulationArray[x, y];
        switch (simulationCell.currentState)
        {
            case CellState.clear:
                cellObject.GetComponent<Renderer>().material.SetColor("_Color", clearCellColor);
                break;
            case CellState.habitable:
                cellObject.GetComponent<Renderer>().material.SetColor("_Color", habitableCellColor);
                break;
            case CellState.occupied:
                cellObject.GetComponent<Renderer>().material.SetColor("_Color", occupiedCellColor);
                break;
        }
        
        Debug.Log("UPDATED VISUALS");
    }

    public void SimulationTick()
    {
        Debug.Log("TICK!");
        nextTickButton.interactable = false;
        totalPopulation = new Dictionary<string, int>();
        
        for (int x = 0; x < simulationArray.GetLength(0); x++)
        {
            for (int y = 0; y < simulationArray.GetLength(1); y++)
            {
                MigrateAgents(simulationArray[x,y].CellTick());
                UpdateCellVisual(x,y);
                //caclulate total population
                foreach (KeyValuePair<string, List<SpeciesAgent>> keyValuePair in simulationArray[x,y].speciesPopulation)
                {
                    if (totalPopulation.ContainsKey(keyValuePair.Key))
                    {
                        totalPopulation[keyValuePair.Key] += keyValuePair.Value.Count;
                    }
                    else
                    {
                        totalPopulation.Add(keyValuePair.Key, keyValuePair.Value.Count);
                    }
                }
            }
        }
        PersistantSceneManager.instance.ConfirmEffect();
        UpdateTotalCountUI();
        nextTickButton.interactable = true;
        
        Debug.Log("TICK COMPLETE!");
    }

    private void UpdateTotalCountUI()
    {
        string total = String.Empty;

        foreach (KeyValuePair<string, int> entry in totalPopulation)
        {
            total = $"{entry.Key}: {entry.Value} \n";
        }

        totalPopulationUI.text = total;

    }

    public void MigrateAgents(Dictionary<SpeciesAgent, List<Vector2>> migratoryAgents)
    {
        if (migratoryAgents.Keys.Count <=0 )
        {
            Debug.Log("No Migrations");
        }
        else{
            foreach (KeyValuePair<SpeciesAgent, List<Vector2>> kvp in migratoryAgents)
            {
                //find suitable migration route
                List<Vector2> potentialHabitats = new List<Vector2>();
                Debug.Log($"DESTINATIONS TO CHECK {kvp.Value.Count}");
                foreach (Vector2 destination in kvp.Value)
                {
                    Debug.Log(
                        $"checking coord:{destination}, {simulationArray[(int) destination.x, (int) destination.y].currentState}");
                    if (simulationArray[(int) destination.x, (int) destination.y].currentState == CellState.habitable ||
                        simulationArray[(int) destination.x, (int) destination.y].currentState == CellState.occupied)
                    {
                        Debug.Log("potential habitat found");
                        potentialHabitats.Add(destination);
                        PersistantSceneManager.instance.MigrateEffect();
                    }
                }

                if (potentialHabitats.Count > 0)
                {
                    Vector2 destination = potentialHabitats[Random.Range(0, potentialHabitats.Count)];
                    Debug.Log("migrating to random found habitat");

                    GameObject migrationInstance = Instantiate(migrationInstancePrefab);
                    Debug.Log("Migration");
                    Vector3 start = visualArray[(int) kvp.Key.location.x, (int) kvp.Key.location.y].transform.position;
                    Vector3 end = visualArray[(int) destination.x, (int) destination.y].transform.position;

                    migrationInstance.GetComponent<MigrationInstanceHook>().hook(start, end);
                    
                    
                    simulationArray[(int) destination.x, (int) destination.y].AcceptMigration(kvp.Key);
                    UpdateCellVisual((int) destination.x, (int) destination.y);
                }
                else
                {
                    Debug.Log($"No potential habitats so dead agent at {kvp.Key.CurrentSimulationCell.cellCoords}");
                }
            }
        }
    }

    #endregion

    public void ClearCellSpecies(SimulationCell simulationCell)
    {
        simulationArray[(int) simulationCell.cellCoords.x, (int) simulationCell.cellCoords.y].ClearCellSpecies();
    }

    public void ClearCellResources(SimulationCell simulationCell)
    {
        simulationArray[(int) simulationCell.cellCoords.x, (int) simulationCell.cellCoords.y].ClearCellResources();
    }

    public void Pause()
    {
        paused = !paused;
        PersistantSceneManager.instance.BackEffect();
        if (paused)
        {
            quitSimulationPanel.SetActive(true);
        }
        else
        {
            quitSimulationPanel.SetActive(false);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Resume()
    {
        PersistantSceneManager.instance.ConfirmEffect();

        paused = false;
        quitSimulationPanel.SetActive(false);
    }
    
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void SetVolume(float val)
    {
        PersistantSceneManager.instance.audioSource.volume = val;
        PersistantSceneManager.instance.ConfirmEffect();
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