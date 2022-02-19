using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistantSceneManager : MonoBehaviour
{
    public static PersistantSceneManager instance;
    public List<ResourceAsset> fullResourceList;
    public List<ResourceAsset> resources;
    public List<SpeciesAsset> species;
    
    public string simulationName;
    public Vector2 simulationSize;
    
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        fullResourceList = new List<ResourceAsset>();
        resources = new List<ResourceAsset>();
        species = new List<SpeciesAsset>();
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "SimulationScene")
        {
            SimulationManager simulationManager = GameObject.FindWithTag("GameController").GetComponent<SimulationManager>();
            simulationManager.SimulationSetup(species, resources, fullResourceList, simulationSize);
        }
    }

    public void StartSimulation()
    {
        // AssetCreationManager.instance.SaveSimulation();
        SceneManager.LoadScene("SimulationScene");
    }

    void EndSimulation()
    {
        SceneManager.LoadScene("MenuScene");
    }
    
    

}
