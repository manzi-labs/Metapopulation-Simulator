using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class SimulationCell
{
    public Vector2 cellCoords;
    public List<SpeciesAgent> speciesList;
    public List<ResourceAsset> resourceAssets;
    public Dictionary<string, List<SpeciesAgent>> speciesPopulation;
    public Dictionary<string, List<Vector2>> possibleDestinations;
    public Dictionary<string, float> resoursePopulation;
    Dictionary<string, int> resourceConsumption;
    
    public CellState currentState;
    
    public SimulationCell(List<SpeciesAgent> _species, List<ResourceAsset> _resourceAssets, Vector2 _cellCoords, Vector2 simulationDimentions)
    {
        this.cellCoords = _cellCoords;
        resoursePopulation = new Dictionary<string, float>();
        speciesPopulation = new Dictionary<string, List<SpeciesAgent>>();
        speciesList = _species;
        resourceConsumption = new Dictionary<string, int>();
        possibleDestinations = new Dictionary<string, List<Vector2>>();
        resourceAssets = _resourceAssets;
        foreach (ResourceAsset asset in _resourceAssets)
        {
            
            float probability = Random.Range(0.0f, 1.0f);
            Debug.Log($"Adding: {asset.name} with r {probability}");
            if (probability >= (1 - asset.spawnProbability))
            {
                resoursePopulation.Add(asset.id, Random.Range(asset.minQuantity, asset.maxQuantity));
                Debug.Log($"Added {asset.name}, {resoursePopulation[asset.id]}");
            }
        }

        foreach (SpeciesAgent species in _species)
        {
            bool habitable = true;
            Debug.Log($"Adding {species.name}");
            foreach (string resourceId in species.resourceRequirements.Keys)
            {
                if (resoursePopulation.ContainsKey(resourceId)) 
                {
                    if (resoursePopulation[resourceId] < species.resourceRequirements[resourceId])
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
                speciesPopulation.Add(species.name, new List<SpeciesAgent>());
            }
            
            possibleDestinations.Add(species.name, new List<Vector2>());

            float minX = Mathf.Clamp(cellCoords.x - species.MaxMigrationRange, 0, simulationDimentions.x);
            float maxX = Mathf.Clamp(cellCoords.x + species.MaxMigrationRange, 0, simulationDimentions.x);
            
            float minY = Mathf.Clamp(cellCoords.y - species.MaxMigrationRange, 0, simulationDimentions.y);
            float maxY = Mathf.Clamp(cellCoords.y + species.MaxMigrationRange, 0, simulationDimentions.y);
            
            Debug.Log($"possible destination search: minx:{minX} = cell coord:{cellCoords.x} - range{species.MaxMigrationRange}"); //WTF MIN AND MAX ARE THE SAME?
            Debug.Log($"possible destination search: maxx:{maxX} = cell coord:{cellCoords.x} + range{species.MaxMigrationRange}"); //WTF MIN AND MAX ARE THE SAME?

            for (int x = (int) minX; x < maxX; x++)
            {
                for (int y = (int) minY; y < maxY; y++)
                {
                    Debug.Log($"possible destination added: {x},{y}");
                    possibleDestinations[species.name].Add(new Vector2(x,y)); //added destinations might not be habitable
                }
            }

        }

        foreach (string speciesName in speciesPopulation.Keys)
        {
            //Is Habitable to all here

            SpeciesAgent species = speciesList.Find(s => s.name == speciesName);
            
            float probability = Random.Range(0.0f, 1.0f);
            float count = Random.Range(species.childQuantityRangeProbability.x,
                species.childQuantityRangeProbability.y);
            
            if (probability >= species.spawnProbability)
            {
                Debug.Log($"adding {species.name} * {count}");
                for (int i = 0; i < count; i++)
                {
                    SpeciesAgent agent = new SpeciesAgent(species.name, 0, species.maxAge, species.spawnProbability, species.childQuantityRangeProbability, species.initialMigrationProbability, species.MaxMigrationRange);
                    agent.resourceRequirements = species.resourceRequirements;
                    agent.location = cellCoords;
                    agent.CurrentSimulationCell = this;

                    speciesPopulation[speciesName].Add(agent);
                }
            }
        }
        
        //update view
        if (speciesPopulation.Keys.Count > 0)
        {
            currentState = CellState.habitable;
            foreach (string speciesName in speciesPopulation.Keys)
            {
                if (speciesPopulation[speciesName].Count > 0)
                {
                    currentState = CellState.occupied;
                }
            }
        }
    }

    public Dictionary<SpeciesAgent, List<Vector2>> CellTick()
    {
        Debug.Log($"Cell {cellCoords.x}, {cellCoords.y} Tick");
        Dictionary<SpeciesAgent, List<Vector2>> migratingAgents = new Dictionary<SpeciesAgent, List<Vector2>>();
        resourceConsumption = new Dictionary<string, int>();

        foreach (KeyValuePair<string, List<SpeciesAgent>> speciesKeyValuePair in speciesPopulation)
        {
            List<SpeciesAgent> deadAgents = new List<SpeciesAgent>();
            List<SpeciesAgent> newAgents = new List<SpeciesAgent>();

            //calculate consumption of resources
            if (speciesKeyValuePair.Value.Count > 0)
            {
                Dictionary<string, int> demand = speciesKeyValuePair.Value[0].resourceRequirements;
                foreach (KeyValuePair<string, int> requirement in demand)
                {
                    if (resourceConsumption.ContainsKey(requirement.Key))
                    {
                        resourceConsumption[requirement.Key] += requirement.Value * speciesKeyValuePair.Value.Count;
                    }
                    else
                    {
                        resourceConsumption.Add(requirement.Key, requirement.Value * speciesKeyValuePair.Value.Count);
                    }
                }
            }

            //tick agents
            if (speciesKeyValuePair.Value.Count > 0)
            {
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
                        int childCount = (int) Random.Range(agent.childQuantityRangeProbability.x,
                            agent.childQuantityRangeProbability.y);

                        for (int i = 0; i < childCount; i++)
                        {
                            SpeciesAgent child = new SpeciesAgent(agent.name, 0, agent.maxAge, agent.spawnProbability,
                                agent.childQuantityRangeProbability, agent.initialMigrationProbability,
                                agent.MaxMigrationRange);
                            child.resourceRequirements = agent.resourceRequirements;

                            child.CurrentSimulationCell = agent.CurrentSimulationCell;
                            child.location = agent.location;

                            newAgents.Add(child);
                        }
                    }
                    Debug.Log($"Check Migration...{agent.willMigrate} && {agent.age} == {agent.migrateAge} && {!agent.migrated}");
                    //process migrating agents
                    if (agent.willMigrate && agent.age == agent.migrateAge && !agent.migrated)
                    {
                        //mark for migration and remove from population pool
                        Debug.Log($"count of full possible destinations {possibleDestinations[agent.name].Count}");
                        migratingAgents.Add(agent, possibleDestinations[agent.name]);
                        deadAgents.Add(agent);
                    }

                    agent.Tick();
                }

                //Add new and remove dead agents from lists
                foreach (SpeciesAgent agent in newAgents)
                {
                    speciesPopulation[agent.name].Add(agent);
                }

                foreach (SpeciesAgent agent in deadAgents)
                {
                    speciesPopulation[agent.name].Remove(agent);
                }
            }
        }
        // update resources
        for (int i = 0; i < resoursePopulation.Keys.Count; i++)
        {
            string resourceAssetID = resoursePopulation.ElementAt(i).Key;
            ResourceAsset resourceAsset = new ResourceAsset();
            
            foreach (ResourceAsset res in resourceAssets)
            {
                if (res.id == resourceAssetID)
                {
                    resourceAsset = res;
                }
            }
            
            float currentQuantity = resoursePopulation.ElementAt(i).Value;
            float newQuantity = currentQuantity + (resourceAsset.growthRate*currentQuantity);
            //subtract consumption and simulate starvation
            if (resourceConsumption.ContainsKey(resourceAsset.id))
            {
                if (newQuantity - resourceConsumption[resourceAsset.id] < 0)
                {
                    //carry capacity is too low so starve
                    foreach (SpeciesAgent species in speciesList)
                    {
                        if (species.resourceRequirements.ContainsKey(resourceAsset.id))
                        {
                            //this species is affected so calculate carry capacity
                            float k = newQuantity / species.resourceRequirements[resourceAsset.id];
                            if (speciesPopulation[species.name].Count > k)
                            {
                                //if higher than carry capacity
                                int removalCount = (int) (speciesPopulation[species.name].Count - k);
                                Debug.Log($"removing {removalCount} of {species.name}");
                                for (int j = 0; j < removalCount; j++)
                                {
                                    speciesPopulation[species.name].RemoveAt(Random.Range(0,speciesPopulation[species.name].Count));
                                }
                            }
                        }
                    }

                    newQuantity = 0;
                }
                else
                {
                    //lower by consumption
                    newQuantity -= resourceConsumption[resourceAsset.id];
                }
            }
            //clamp to 0 and capacity
            resoursePopulation[resourceAsset.id] = Mathf.Clamp(newQuantity, 0, resourceAsset.capacity);
        }
        
        //update State
        UpdateState();
        Debug.Log($"Cell {cellCoords.x}, {cellCoords.y} Tick Complete");
        return migratingAgents;
    }

    private void UpdateState()
    {
        List<bool> habitableChecks = new List<bool>();
        bool occupied = false;

        foreach (string speciesName in speciesPopulation.Keys)
        {
            bool habitable = true;
            
            foreach (string resourceId in speciesList.Find(s => s.name == speciesName).resourceRequirements.Keys)
            {
                if (resoursePopulation.ContainsKey(resourceId))
                {
                    if (resoursePopulation[resourceId] < speciesList.Find(s => s.name == speciesName).resourceRequirements[resourceId])
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
                if (speciesPopulation[speciesName].Count > 0)
                {
                    occupied = true;
                }
            }
            habitableChecks.Add(habitable);
        }

        if (habitableChecks.Contains(true))
        {
            currentState = CellState.habitable;
            if (occupied)
            {
                currentState = CellState.occupied;
            }
        }
        else
        {
            currentState = CellState.clear;
        }
    }

    public void AcceptMigration(SpeciesAgent agent)
    {
        agent.migrated = true;
        agent.willMigrate = false;
        agent.CurrentSimulationCell = this;
        agent.location = this.cellCoords;
        
        //set so agent will not remigrate and set locations
        
        if (speciesPopulation.ContainsKey(agent.name))
        {
            speciesPopulation[agent.name].Add(agent);
            Debug.Log("agent arrived!");
        }
        else
        {
            if (currentState == CellState.habitable)
            {
                currentState = CellState.occupied; //change to occupied if newly moved to
            }
            
            speciesPopulation.Add(agent.name, new List<SpeciesAgent>());
            speciesPopulation[agent.name].Add(agent);
            Debug.Log("agent arrived!");
        }
        
        UpdateState();
    }

    public void ClearCellSpecies()
    {
        speciesPopulation = new Dictionary<string, List<SpeciesAgent>>();
        
        UpdateState();
    }
    
    public void ClearCellResources()
    {
        resoursePopulation = new Dictionary<string, float>();

        UpdateState();
    }
}