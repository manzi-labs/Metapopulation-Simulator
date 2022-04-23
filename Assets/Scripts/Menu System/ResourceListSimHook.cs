using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceListSimHook : MonoBehaviour
{
    public TextMeshProUGUI resourceName;
    public ResourceAsset resourceAsset;
    
    public void Init(ResourceAsset _resourceAsset)
    {
        resourceName.text = _resourceAsset.name;
        this.resourceAsset = _resourceAsset;
    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }
}
