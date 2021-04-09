using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugTestURL))]
public class TestURLEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Load"))
        {
            ((DebugTestURL)serializedObject.targetObject).Load();
        }
    }
}
