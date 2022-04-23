using System.Collections.Generic;
using UnityEngine;

public class SpeciesAgent
{
    public SimulationCell CurrentSimulationCell;
    public string name;
    public int age;
    public int maxAge;
    public float spawnProbability;
    public Vector3 childQuantityRangeProbability;
    public float initialMigrationProbability;
    public int MaxMigrationRange;
    public Vector2 location;
    public Dictionary<string, int> resourceRequirements;
    public Color visColor;

    public int childCount;
    public bool carryChild;
    public bool willMigrate;
    public int migrateAge;
    public int birthAge;
    public bool migrated;

    public SpeciesAgent(string _name, int _age, int _maxAge, float _spawnProbability, Vector3 _childQuantityRangeProbability, float _initialMigrationProbability, int _maxMigrationRange)
    {
        name = _name;
        age = _age;
        maxAge = _maxAge;
        spawnProbability = _spawnProbability;
        childQuantityRangeProbability = _childQuantityRangeProbability;
        initialMigrationProbability = _initialMigrationProbability;
        MaxMigrationRange = _maxMigrationRange;
        Debug.Log("SPECIES RANGE:"+MaxMigrationRange);
        
        resourceRequirements = new Dictionary<string, int>();
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
            // carryChild = true;
            // childCount = (int) Random.Range(childQuantityRangeProbability.x, childQuantityRangeProbability.y);
            // birthAge = Random.Range(1, maxAge);
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
        if (age == migrateAge)
        {
            Debug.Log("I SHOULD MIGRATE...");
        }
    }
}