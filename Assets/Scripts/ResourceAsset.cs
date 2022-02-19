using System;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceAsset
{
        public string id;
        public string name;
        public int minQuantity;
        public int maxQuantity;
        public float growthRate;
        public int capacity;
        public string indicatorColor;
        public float spawnProbability;

        public ResourceAsset()
        { 
            id = Guid.NewGuid().ToString();
        }
}