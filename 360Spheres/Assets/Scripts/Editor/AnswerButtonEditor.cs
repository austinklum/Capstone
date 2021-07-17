using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

 
    [CustomEditor(typeof(AnswerButton))]
    public class MyButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            AnswerButton targetMyButton = (AnswerButton)target;

        targetMyButton.Overlay = (GameObject)EditorGUILayout.ObjectField("Overlay:", targetMyButton.Overlay, typeof(GameObject), true);
        targetMyButton.AnswerId = EditorGUILayout.IntField("AnswerId:", targetMyButton.AnswerId);
        base.OnInspectorGUI();
        }
    }
