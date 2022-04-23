using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class FileUtility
{

    public List<SpeciesAsset> GetSpeciesList()
    {
        List<SpeciesAsset> speciesList = new List<SpeciesAsset>();
        if (Directory.Exists(Application.persistentDataPath + "/CustomAssets/Species/"))
        {
            if (Directory.GetFiles(Application.persistentDataPath + "/CustomAssets/Species/", "*.json").ToList().Count > 0)
            {
                List<string> addedIds = new List<string>();
                
                foreach (var path in Directory.GetFiles(Application.persistentDataPath + "/CustomAssets/Species/", "*.json"))
                {
                    string json = File.ReadAllText(path);
                    SpeciesAsset speciesAsset = JsonConvert.DeserializeObject<SpeciesAsset>(json); //convert back into object from json
                    if (!addedIds.Contains(speciesAsset.id)) //dont add duplicates
                    {
                        addedIds.Add(speciesAsset.id);
                        speciesList.Add(speciesAsset);
                    }
                }
            }
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/CustomAssets/Species/");
        }

        return speciesList;
    }
    public List<ResourceAsset> GetResourceList()
    {
        Debug.Log("finding resources..."+Application.persistentDataPath + "/CustomAssets/Resources/");
        List<ResourceAsset> resourceList = new List<ResourceAsset>();

        if (Directory.Exists(Application.persistentDataPath + "/CustomAssets/Resources/"))
        {
            if (Directory.GetFiles(Application.persistentDataPath + "/CustomAssets/Resources/", "*.json").ToList()
                .Count > 0)
            {
                List<string> addedIds = new List<string>();
                
                foreach (var path in Directory.GetFiles(Application.persistentDataPath + "/CustomAssets/Resources/", "*.json"))
                {
                    Debug.Log("Found Resource: \n"+path);
                    string json = File.ReadAllText(path);
                    ResourceAsset resourceAsset = JsonConvert.DeserializeObject<ResourceAsset>(json); //convert back into object from json
                    if (!addedIds.Contains(resourceAsset.id))
                    {
                        addedIds.Add(resourceAsset.id);
                        resourceList.Add(resourceAsset);
                        
                    }
                }
            }
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/CustomAssets/Resources/");
        }

        return resourceList;
    }

    public List<SimulationAsset> GetSimulationList()
    {
        Debug.Log("finding Simulations..."+Application.persistentDataPath + "/CustomAssets/Simulations/");
        List<SimulationAsset> simulationList = new List<SimulationAsset>();

        if (Directory.Exists(Application.persistentDataPath + "/CustomAssets/Simulations/"))
        {
            if (Directory.GetFiles(Application.persistentDataPath + "/CustomAssets/Simulations/", "*.json").ToList()
                .Count > 0)
            {
                List<string> addedIds = new List<string>();
                
                foreach (var path in Directory.GetFiles(Application.persistentDataPath + "/CustomAssets/Simulations/", "*.json"))
                {
                    Debug.Log("Found Resource: \n"+path);
                    string json = File.ReadAllText(path);
                    SimulationAsset simulationAsset = JsonConvert.DeserializeObject<SimulationAsset>(json); //convert back into object from json
                    if (!addedIds.Contains(simulationAsset.id))
                    {
                        addedIds.Add(simulationAsset.id);
                        simulationList.Add(simulationAsset);
                    }
                }
            }
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/CustomAssets/Simulations/");
        }
        Debug.Log("RETURNED SIM LIST");
        return simulationList;

    }

    public void SaveResource(ResourceAsset resourceAsset)
    {
        string json = JsonConvert.SerializeObject(resourceAsset);
        string resourceModelName = "RESOURCE_"+resourceAsset.name+".json";
        if (Directory.Exists(Application.persistentDataPath + "/CustomAssets/Resources/"))
        {
            System.IO.File.WriteAllText(Application.persistentDataPath+"/CustomAssets/Resources/"+resourceModelName, json);
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/CustomAssets/Resources/");
            System.IO.File.WriteAllText(Application.persistentDataPath+"/CustomAssets/Resources/"+resourceModelName, json);
        }
        Debug.Log("Saved Resource");
    }
    public void SaveSpecies(SpeciesAsset speciesAsset)
    {
        string json = JsonConvert.SerializeObject(speciesAsset);
        string speciesModelName = "SPECIES_"+speciesAsset.name+".json";
        if (Directory.Exists(Application.persistentDataPath + "/CustomAssets/Species/"))
        {
            System.IO.File.WriteAllText(Application.persistentDataPath+"/CustomAssets/Species/"+speciesModelName, json);
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/CustomAssets/Species/");
            System.IO.File.WriteAllText(Application.persistentDataPath+"/CustomAssets/Species/"+speciesModelName, json);
        }

        Debug.Log("Saved Species");
    }

    public void SaveSimulation(SimulationAsset simulationAsset)
    {
        string json = JsonConvert.SerializeObject(simulationAsset);
        string simulationModelName = "SIM_"+simulationAsset.name+".json";
        if (Directory.Exists(Application.persistentDataPath + "/CustomAssets/Simulations/"))
        {
            System.IO.File.WriteAllText(Application.persistentDataPath+"/CustomAssets/Simulations/"+simulationModelName, json);
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/CustomAssets/Simulations/");
            System.IO.File.WriteAllText(Application.persistentDataPath+"/CustomAssets/Simulations/"+simulationModelName, json);
        }
        Debug.Log("Saved Simulation");
    }
    
}