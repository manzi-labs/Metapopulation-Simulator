using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeciesListSimHook : MonoBehaviour
{
    public TextMeshProUGUI speciesName;
    public SpeciesAsset speciesAsset;
    public void Init(SpeciesAsset _speciesAsset)
    {
        speciesName.text = _speciesAsset.name;
        this.speciesAsset = _speciesAsset;
    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }
}
