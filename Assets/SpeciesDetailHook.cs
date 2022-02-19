using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeciesDetailHook : MonoBehaviour
{
    public TextMeshProUGUI speciesName;
    public TextMeshProUGUI speciesQuantity;

    public void Init(SpeciesAgent _speciesAsset, int _quantity)
    {
        speciesName.text = _speciesAsset.name;
        speciesQuantity.text = _quantity.ToString();
    }
}
