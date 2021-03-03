using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorFramerateController : MonoBehaviour
{
    public int targetFrameRate = 30;

    void Start()
    {
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
#endif
    }
}
