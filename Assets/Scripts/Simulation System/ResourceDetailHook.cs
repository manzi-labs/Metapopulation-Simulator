using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceDetailHook : MonoBehaviour
{
    public TextMeshProUGUI resourceName;
    public TextMeshProUGUI resourceQuantity;

    public void Init(ResourceAsset resourceAsset, float quantity)
    {
        resourceName.text = resourceAsset.name;
        resourceQuantity.text = (quantity).ToString();
    }
}
