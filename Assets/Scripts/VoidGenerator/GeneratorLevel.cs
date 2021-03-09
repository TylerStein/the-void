using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Generator Level", menuName = "Level Generation/Level")]
public class GeneratorLevel : ScriptableObject
{
    [Tooltip("Higher means less challenge, lower means more")]
    [SerializeField] public int leniencyTarget = 0;
}
