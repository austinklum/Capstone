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
        m_Image.color = m_NormalColor;
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
        m_Image.color = m_DownColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //print("Enter");
        m_Image.color = m_HoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //print("Exit");
        m_Image.color = m_NormalColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //print("Up");
    }

}
