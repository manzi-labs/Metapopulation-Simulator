using System;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesAsset
{
    public string id;
    public string name;
    public int maxAge;
    public string indicatorColor;
    public Vector3 childQuantityRangeProbability;
    public float initialMigrationProbability;
    public Dictionary<string, int> requirements;
    public int maxMigrationDistance;
    public float initialSpawnProbability;

    public SpeciesAsset()
    {
        id = Guid.NewGuid().ToString();
        requirements = new Dictionary<string, int>();
    }
}