using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpeciesAsset
{
    public string id;
    public string name;
    public int maxAge;
    public string visColour;
    public Vector3 childQuantityRangeProbability;
    public float initialMigrationProbability;
    public Dictionary<string, int> requirements;
    public int maxMigrationRange;
    public float initialSpawnProbability;

    public SpeciesAsset()
    {
        id = Guid.NewGuid().ToString();
        requirements = new Dictionary<string, int>();
    }
}