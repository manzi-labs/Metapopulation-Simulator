using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellDetailManager : MonoBehaviour
{
    public static CellDetailManager instance;
    
    public GameObject resourceGrid;
    public GameObject speciesGrid;
    public GameObject resourceDetailPrefab;
    public GameObject speciesDetailPrefab;
    public TextMeshProUGUI cellCoordinates;
    public GameObject currentCell;
    public Cell cell;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        Clear();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown (0)) {
            Debug.Log ("MouseDown");
            // Reset ray with new mouse position
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
            RaycastHit[] hits = Physics.RaycastAll (ray);
            foreach (RaycastHit hit in hits) {
                if (hit.collider.gameObject.tag == "CellObject") {
                    currentCell = hit.collider.gameObject;

                    UpdateDetails();
                    Debug.Log ("Hit");
                }
            }
        }
    }

    public void UpdateDetails()
    {
        Clear();
        
        if (!SimulationManager.instance.cellLookupDictionary.ContainsKey(currentCell))
        {
            return;
        }
        
        cell = SimulationManager.instance.cellLookupDictionary[currentCell];

        int xCoord = (int) cell.cellCoords.x;
        int yCoord = (int) cell.cellCoords.y;
        cellCoordinates.text = $"{xCoord}, {yCoord}";
        
        if(cell.resoursePopulation.Keys.Count > 0)
        {
            foreach (ResourceAsset resource in cell.resoursePopulation.Keys)
            {
                GameObject entry = Instantiate(resourceDetailPrefab, Vector3.zero, Quaternion.identity,
                    resourceGrid.transform);
                entry.GetComponent<ResourceDetailHook>().Init(resource, cell.resoursePopulation[resource]);
            }
        }
        if(cell.speciesPopulation.Keys.Count > 0)
        {
            foreach (SpeciesAgent speciesAgent in cell.speciesPopulation.Keys)
            {
                GameObject entry = Instantiate(speciesDetailPrefab, Vector3.zero, Quaternion.identity,
                    speciesGrid.transform);
                entry.GetComponent<SpeciesDetailHook>().Init(speciesAgent, cell.speciesPopulation[speciesAgent].Count);
            }
        }
    }

    void Clear()
    {
        for (int i = 0; i < resourceGrid.transform.childCount; i++)
        {
            Destroy(resourceGrid.transform.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < speciesGrid.transform.childCount; i++)
        {
            Destroy(speciesGrid.transform.GetChild(i).gameObject);
        }
    }
}
