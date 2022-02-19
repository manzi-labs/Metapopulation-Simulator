using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesListHook : MonoBehaviour
{
    public Image colorIndicator;
    public TextMeshProUGUI speciesName;

    public SpeciesAsset speciesAsset;
    
    public void Init(SpeciesAsset _speciesAsset)
    {
        this.speciesAsset = _speciesAsset;

        if(ColorUtility.TryParseHtmlString("#"+this.speciesAsset.indicatorColor, out var indicator))
        {
            colorIndicator.color = indicator;
        }
        
        speciesName.text = this.speciesAsset.name;
    }

    public void Select()
    {
        Debug.Log("added species!");
        AssetCreationManager.instance.AddedSpecies(speciesAsset);
    }
}