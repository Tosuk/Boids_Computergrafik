
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(BoidController))]
public class BoidEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BoidController boidController = (BoidController)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Reset Boids"))
        {
            boidController.ResetBoids();
        }
        if (GUILayout.Button("Toggle GUI"))
        {
            boidController.showGui();
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Boid Settings", EditorStyles.boldLabel);
        // Add a bold label
        BoidController.alignWeight = EditorGUILayout.IntSlider("Align Weight", BoidController.alignWeight, 0, 10);
        BoidController.cohesionWeight = EditorGUILayout.IntSlider("Cohesion Weight", BoidController.cohesionWeight, 0, 10);
        BoidController.avoidenceWeight = EditorGUILayout.IntSlider("Avoidence Weight", BoidController.avoidenceWeight, 0, 10);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
        boidController.viewPortSize = EditorGUILayout.IntSlider("View Port Size", boidController.viewPortSize, 1, 20);

        if (EditorGUI.EndChangeCheck())
        {
            boidController.UpdateCamera();
        }
    }
}
