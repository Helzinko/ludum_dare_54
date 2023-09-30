using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameSubmitWindow : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private PlayFabManager playFabManager;

    public void SubmitName()
    {
        if (input.text != string.Empty)
        {
            playFabManager.SubmitName(input.text);
        }
    }
}