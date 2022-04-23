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

    public AudioClip confirm;
    public AudioClip back;
    public AudioClip launch;
    public AudioClip migrate;

    public AudioSource audioSource;
    
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

        audioSource = this.GetComponent<AudioSource>();
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
            LaunchEffect();
        }
    }

    public void StartSimulation()
    {
        // AssetCreationManager.instance.SaveSimulation();
        SceneManager.LoadScene("SimulationScene");
    }

    public void EndSimulation()
    {
        fullResourceList = new List<ResourceAsset>();
        resources = new List<ResourceAsset>();
        species = new List<SpeciesAsset>();
        SceneManager.LoadScene("MenuScene");
    }

    public void ConfirmEffect()
    {
        audioSource.PlayOneShot(confirm);
    }

    public void BackEffect()
    {
        audioSource.PlayOneShot(back);
    }

    public void LaunchEffect()
    {
        audioSource.PlayOneShot(launch);
    }

    public void MigrateEffect()
    {
        audioSource.PlayOneShot(migrate);
    }
}
