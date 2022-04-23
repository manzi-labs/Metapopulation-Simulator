using TMPro;
using UnityEngine;

public class SimulationListHook : MonoBehaviour
{
    public TextMeshProUGUI entryName;
    public SimulationAsset asset;

    public void Init(SimulationAsset _asset)
    {
        asset = _asset;
        entryName.text = asset.name;
    }

    public void Load()
    {
        AssetCreationManager.instance.LoadSimulation(asset);
    }
}
