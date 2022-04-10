﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;

public class ButtonTransitioner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private Color32 m_NormalColor = Color.white;
    private Color32 m_HoverColor = new Color(.8f, .8f, .8f, 1);
    private Color32 m_DownColor = Color.grey;
    private Color32 m_SuccessColor = new Color(.01f, .98f, .01f, 1);
    private Color32 m_FailColor = Color.red;
    private float enterExitDifference = .5f;
    private float upDownDifference = .1f;

    public SteamVR_Action_Vibration hapticAction;
    public SteamVR_Action_Boolean trackpadAction;

    public VRInputModule VRInputModule;

    public Image m_Image { get; set; }


    private void Awake()
    {
        m_Image = GetComponent<Image>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        print("Click");
        Pulse(1, 150, 150, SteamVR_Input_Sources.RightHand);
        AnswerButton btn = (AnswerButton)eventData.pointerPress.GetComponent("AnswerButton");
       

        if (btn.AnswerId == -1 || btn.AnswerId == -2 || btn.AnswerId == -3)
        {
            VRInputModule.ChangeCourse(btn.AnswerId);
            return;
        }

        if (btn.AnswerId == -4)
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
            return;
        }

        bool success = VRInputModule.IsCorrect(btn.AnswerId);
        if(success)
        {
            m_Image.color = m_SuccessColor;
        }
      
        VRInputModule.ProcessClick(btn);

        if(!success)
        {
            m_Image.color = m_FailColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //print("Down");
        Color.RGBToHSV(m_Image.color, out float h, out float s, out float v);
        v = Clamp(v);
        m_Image.color = Color.HSVToRGB(h, s, Clamp(v - upDownDifference));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //print("Enter");
        Pulse(1, 50, 25, SteamVR_Input_Sources.RightHand);
        Color.RGBToHSV(m_Image.color, out float h, out float s, out float v);
        v = Clamp(v);
        m_Image.color = Color.HSVToRGB(h, s, Clamp(v - enterExitDifference));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //print("Exit");
        Color.RGBToHSV(m_Image.color, out float h, out float s, out float v);
        v = Clamp(v);
        m_Image.color = Color.HSVToRGB(h, s, Clamp(v + enterExitDifference));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //print("Up");
        Color.RGBToHSV(m_Image.color, out float h, out float s, out float v);
        v = Clamp(v);
        m_Image.color = Color.HSVToRGB(h, s, Clamp(v + upDownDifference));
    }

    public void ResetButtonColor()
    {
        m_Image.color = m_NormalColor;
    }

    private float Clamp(float value)
    {
        if (value > 1)
        {
            return 1;
        }

        if (value < 0)
        {
            return 0;
        }
        return value;
    }

    private void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source)
    {
        hapticAction.Execute(0, duration, frequency, amplitude, source);
       // print("Pulse triggered");
    }
}
