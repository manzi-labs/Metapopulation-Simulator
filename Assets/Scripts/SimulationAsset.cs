using System;
using System.Collections.Generic;
using UnityEngine;

public class SimulationAsset
{
   public string id;
   public string name;
   public Vector2 simSize;
   public List<string> speciesList;
   public List<string> resourceList;

   public SimulationAsset()
   {
      id = Guid.NewGuid().ToString();
      speciesList = new List<string>();
      resourceList = new List<string>();
   }
}