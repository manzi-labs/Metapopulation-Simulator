using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HabitatHook : MonoBehaviour
{
    string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public TextMeshPro textMesh;

    void Start()
    {
        textMesh.enabled = false;
    }

    public void UpdateHook(bool enabled, int habitatId)
    {
        textMesh.enabled = enabled;
        textMesh.text = NumToString(habitatId);
    }

    string NumToString(int num)
    {
        string value = "";
            
        if (num >= letters.Length)
            value += letters[num / letters.Length - 1];

        value += letters[num % letters.Length];

        return value;
    }
}
