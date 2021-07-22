using UnityEditor;
using UnityEditor.UI;
using UnityEngine;


[CustomEditor(typeof(AnswerButton))]
public class AnswerButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        AnswerButton targetMyButton = (AnswerButton)target;

        targetMyButton.AnswerId = EditorGUILayout.IntField("AnswerId:", targetMyButton.AnswerId);
        base.OnInspectorGUI();
    }
}