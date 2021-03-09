using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Generator Beat", menuName = "Level Generation/Beat")]
public class GeneratorBeat : ScriptableObject
{
    [Tooltip("How this level referred to in the lexicon")]
    [SerializeField] public string lexiconHandle = "DEFAULT";

    [Tooltip("How difficult this beat is: Higher is easier, lower is more difficult")]
    [SerializeField] public int leniencyRating = 0;

    [Tooltip("The object to spawn in")]
    [SerializeField] public GameObject objectPrefab = null;
}
