using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
using Valve.VR.Extras;
using System.Diagnostics;

public class EnvironmentLibrary : MonoBehaviour
{
    public List<Environment> Environments;
    public SkyBoxController skyBoxController;
}

[Serializable]
public class Environment
{
    public int WorldRotation;// { get => worldRotation; set => worldRotation = value; }
    public Texture Background;// { get => background; set => background = value; }
}
