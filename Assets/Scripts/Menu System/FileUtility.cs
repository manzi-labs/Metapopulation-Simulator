using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class FileUtility
{

    public List<SpeciesAsset> GetSpeciesList()
    {
        List<SpeciesAsset> speciesList = new List<SpeciesAsset>();
        
        foreach (var path in Directory.GetFiles(Application.dataPath + "/CustomAssets/Species/", "*.json"))
        {
            string json = File.ReadAllText(path);
            SpeciesAsset speciesAsset = JsonConvert.DeserializeObject<SpeciesAsset>(json); //convert back into object from json
            speciesList.Add(speciesAsset);
        }

        return speciesList;
    }
    public List<ResourceAsset> GetResourceList()
    {
        Debug.Log("finding resources..."+Application.dataPath + "/CustomAssets/Resources/");
        List<ResourceAsset> resourceList = new List<ResourceAsset>();

        foreach (var path in Directory.GetFiles(Application.dataPath + "/CustomAssets/Resources/", "*.json"))
        {
            Debug.Log("Found Resource: \n"+path);
            string json = File.ReadAllText(path);
            ResourceAsset resourceAsset = JsonConvert.DeserializeObject<ResourceAsset>(json); //convert back into object from json
            resourceList.Add(resourceAsset);
        }

        return resourceList;
    }

    public void SaveResource(ResourceAsset resourceAsset)
    {
        string json = JsonConvert.SerializeObject(resourceAsset);
        string resourceModelName = "RESOURCE_"+resourceAsset.name+".json";
        System.IO.File.WriteAllText(Application.dataPath+"/CustomAssets/Resources/"+resourceModelName, json);
        Debug.Log("Saved Resource");
    }
    public void SaveSpecies(SpeciesAsset speciesAsset)
    {
        string json = JsonConvert.SerializeObject(speciesAsset);
        string speciesModelName = "SPECIES_"+speciesAsset.name+".json";
        System.IO.File.WriteAllText(Application.dataPath+"/CustomAssets/Species/"+speciesModelName, json);
        Debug.Log("Saved Species");
    }

    public void SaveSimulation(SimulationAsset simulationAsset)
    {
        string json = JsonConvert.SerializeObject(simulationAsset);
        string simulationModelName = "SIM_"+simulationAsset.name+".json";
        System.IO.File.WriteAllText(Application.dataPath+"/CustomAssets/Simulations/"+simulationModelName, json);
        Debug.Log("Saved Simulation");
    }
    
}