using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeciesDetailHook : MonoBehaviour
{
    public TextMeshProUGUI speciesName;
    public TextMeshProUGUI speciesQuantity;

    public void Init(string name, int _quantity)
    {
        speciesName.text = name;
        speciesQuantity.text = _quantity.ToString();
    }
}
