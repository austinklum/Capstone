using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

 
    [CustomEditor(typeof(AnswerButton))]
    public class AnswerButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            AnswerButton targetMyButton = (AnswerButton)target;

            //            targetMyButton.Overlay = (GameObject)EditorGUILayout.ObjectField("Overlay:", targetMyButton.Overlay, typeof(GameObject), true);
            //targetMyButton.AnswerId = EditorGUILayout.TextField("123").;
            base.OnInspectorGUI();
        }
    }

