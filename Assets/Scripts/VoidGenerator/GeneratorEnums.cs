using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGeneratorComponent
{
    PLATFORM = 0,
}

[System.Flags]
public enum EGeneratorRequirement
{
    NONE = 0,
    JUMP = 1,
    BOOST_JUMP = 2,
    AIR_DASH = 4,
    GROUND_DASH = 8,
}