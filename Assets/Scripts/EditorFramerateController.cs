using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorFramerateController : MonoBehaviour
{
    public int targetFrameRate = 30;
    public bool enableFrameLimit = false;

    public bool applySettings = false;

    public int defaultVSync = 0;
    public int defaultFrameRate = 0;

    void Start()
    {
        defaultVSync = QualitySettings.vSyncCount;
        defaultFrameRate = Application.targetFrameRate;

        ApplyFramerate();
    }

    private void Update() {
        if (applySettings) {
            ApplyFramerate();
            applySettings = false;
        }
    }

    private void ApplyFramerate() {
#if UNITY_EDITOR
        if (enableFrameLimit) {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
        } else {
            QualitySettings.vSyncCount = defaultVSync;
            Application.targetFrameRate = defaultFrameRate;
        }
#endif
    }
}
