using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTransitioner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private Color32 m_NormalColor = Color.white;
    private Color32 m_HoverColor = new Color(.8f, .8f, .8f, 1);
    private Color32 m_DownColor = Color.grey;
    private Color32 m_SuccessColor = new Color(.01f, .98f, .01f, 1);
    private Color32 m_FailColor = Color.red;

    public VRInputModule VRInputModule;

    private Image m_Image = null;


    private void Awake()
    {
        m_Image = GetComponent<Image>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        print("Click");
        AnswerButton btn = (AnswerButton)eventData.pointerPress.GetComponent("AnswerButton");

        bool success = VRInputModule.ProcessClick(btn);
        if(success)
        {
            m_Image.color = m_SuccessColor;
        }
        else
        {
            m_Image.color = m_FailColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //print("Down");
        Color.RGBToHSV(m_Image.color, out float h, out float s, out float v);
        v = Clamp(v);
        m_Image.color = Color.HSVToRGB(h, s, Clamp(v - .01f));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //print("Enter");
        Color.RGBToHSV(m_Image.color, out float h, out float s, out float v);
        v = Clamp(v);
        m_Image.color = Color.HSVToRGB(h, s, Clamp(v - .5f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //print("Exit");
        Color.RGBToHSV(m_Image.color, out float h, out float s, out float v);
        v = Clamp(v);
        m_Image.color = Color.HSVToRGB(h, s, Clamp(v + .5f));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //print("Up");
        Color.RGBToHSV(m_Image.color, out float h, out float s, out float v);
        v = Clamp(v);
        m_Image.color = Color.HSVToRGB(h, s, Clamp(v + .01f));
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
}
