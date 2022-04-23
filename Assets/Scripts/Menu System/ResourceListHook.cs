using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ResourceListHook : MonoBehaviour
{
    public Image colorIndicator;
    public TextMeshProUGUI resourceName;

    public ResourceAsset ResourceAsset;

    public void Select()
    {
        AssetCreationManager.instance.AddedResource(ResourceAsset);
    }

    public void Init(ResourceAsset resourceAsset)
    {
        ResourceAsset = resourceAsset;

        if(ColorUtility.TryParseHtmlString("#"+ResourceAsset.indicatorColor, out var indicator))
        {
            colorIndicator.color = indicator;
        }
        
        resourceName.text = ResourceAsset.name;
    }
}
