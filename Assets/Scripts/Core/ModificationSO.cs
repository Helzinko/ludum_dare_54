using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Modification", menuName = "Data/ModificationSO")]
public class ModificationSO : ScriptableObject
{
    [SerializeReference] public IModification Modification;
}
