using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class AnswerButton : Button
{
    public GameObject Overlay; //Our extra field, which won't show up in the inspector
    public int AnswerId;
}