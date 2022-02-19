using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceRequirementHook : MonoBehaviour
{
    public ResourceAsset ResourceAsset;
    public TextMeshProUGUI resourceName;
    public int quantity;

    public void Remove()
    {
        Destroy(this.gameObject);
    }

    public void Init(ResourceAsset resourceAsset)
    {
        resourceName.text = resourceAsset.name;
        ResourceAsset = resourceAsset;

    }

    public void SetQuantity(string _quantity)
    {
        quantity = int.Parse(_quantity);
    }
}
