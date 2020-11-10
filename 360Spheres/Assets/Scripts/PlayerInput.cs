using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using Valve.VR;
using Valve.VR.Extras;

public class PlayerInput : MonoBehaviour
{
    public EnvironmentLibrary EnvironmentLibrary;

    [Serializable]
    public class NewEnvironment : UnityEvent<Environment> { }
    public NewEnvironment OnNewEnvironment;

    private int currentEnvironmentIndex = 0;

    private void Start()
    {
        Select();
        SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerPressed, SteamVR_Input_Sources.Any);
    }

    private void Update()
    {
        
    }

    private void TriggerPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        currentEnvironmentIndex++;
        UnityEngine.Debug.Log("NextEnv = " + currentEnvironmentIndex);

        if (currentEnvironmentIndex >= EnvironmentLibrary.Environments.Count)
        {
            currentEnvironmentIndex = 0;
        }

        Select();
    }

    private void Select()
    {
        OnNewEnvironment.Invoke(EnvironmentLibrary.Environments[currentEnvironmentIndex]);
    }
}
