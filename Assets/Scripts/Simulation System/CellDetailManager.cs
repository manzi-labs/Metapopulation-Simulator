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
    public SimulationCell SimulationCell;
    public bool optionsState;
    public GameObject optionsPanel;

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

        optionsState = false;
        optionsPanel.SetActive(optionsState);
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
                    PersistantSceneManager.instance.ConfirmEffect();
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
        
        SimulationCell = SimulationManager.instance.cellLookupDictionary[currentCell];

        int xCoord = (int) SimulationCell.cellCoords.x;
        int yCoord = (int) SimulationCell.cellCoords.y;
        cellCoordinates.text = $"{xCoord}, {yCoord}";
        
        if(SimulationCell.resoursePopulation.Keys.Count > 0)
        {
            foreach (string resourceId in SimulationCell.resoursePopulation.Keys)
            {
                GameObject entry = Instantiate(resourceDetailPrefab, Vector3.zero, Quaternion.identity,
                    resourceGrid.transform);

                ResourceAsset resource = new ResourceAsset();
                
                foreach (ResourceAsset res in SimulationCell.resourceAssets)
                {
                    if (res.id == resourceId)
                    {
                        resource = res;
                    }
                }

                entry.GetComponent<ResourceDetailHook>().Init(resource, SimulationCell.resoursePopulation[resource.id]);
            }
        }
        if(SimulationCell.speciesPopulation.Keys.Count > 0)
        {
            foreach (string speciesAgentName in SimulationCell.speciesPopulation.Keys)
            {
                GameObject entry = Instantiate(speciesDetailPrefab, Vector3.zero, Quaternion.identity,
                    speciesGrid.transform);
                entry.GetComponent<SpeciesDetailHook>().Init(speciesAgentName, SimulationCell.speciesPopulation[speciesAgentName].Count);
            }
        }
    }

    public void ClearCellSpecies()
    {
        PersistantSceneManager.instance.BackEffect();

        SimulationManager.instance.ClearCellSpecies(SimulationCell);
        SimulationManager.instance.UpdateCellVisual((int) SimulationCell.cellCoords.x, (int) SimulationCell.cellCoords.y);
    }

    public void ClearCellResources()
    {
        PersistantSceneManager.instance.BackEffect();

        SimulationManager.instance.ClearCellResources(SimulationCell);
        SimulationManager.instance.UpdateCellVisual((int) SimulationCell.cellCoords.x, (int) SimulationCell.cellCoords.y);
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

    public void ShowHideOptions()
    {
        optionsState = !optionsState;
        
        optionsPanel.SetActive(optionsState);
    }
}
