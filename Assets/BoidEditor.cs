
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(BoidController))]
public class BoidEditor : Editor
{
    private bool showCameraSettings = false;    
    private bool showBoidSettings = false;
    private bool trailActive = false;

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
        if (GUILayout.Button("Toggle Trail"))
        {
            boidController.setActiveBoidTrail(trailActive = !trailActive);
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Boid Weight Settings", EditorStyles.boldLabel);
        // Add a bold label
        BoidController.alignWeight = EditorGUILayout.IntSlider("Align Weight", BoidController.alignWeight, 0, 10);
        BoidController.cohesionWeight = EditorGUILayout.IntSlider("Cohesion Weight", BoidController.cohesionWeight, 0, 10);
        BoidController.avoidenceWeight = EditorGUILayout.IntSlider("Avoidence Weight", BoidController.avoidenceWeight, 0, 10);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Boid Vision Settings", EditorStyles.boldLabel);
        BoidController.separationRadius = EditorGUILayout.Slider("Separation Radius", BoidController.separationRadius, 0, 5);
        BoidController.alignmentRadius = EditorGUILayout.Slider("Alignment Radius", BoidController.alignmentRadius, 0, 5);  
        BoidController.cohesionRadius = EditorGUILayout.Slider("Cohesion Radius", BoidController.cohesionRadius, 0, 5);


        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
        EditorGUILayout.Space();

        showCameraSettings = EditorGUILayout.Foldout(showCameraSettings, "Camera Settings");
        if (showCameraSettings)
        {
            EditorGUI.BeginChangeCheck();
            boidController.viewPortSize = EditorGUILayout.IntSlider("View Port Size", boidController.viewPortSize, 1, 20);
            boidController.viewPortWidthAspectRatio = EditorGUILayout.Slider("View Port Width Aspect Ratio", boidController.viewPortWidthAspectRatio, 0.1f, 2);
            boidController.viewPortHeightAspectRatio = EditorGUILayout.Slider("View Port Height Aspect Ratio", boidController.viewPortHeightAspectRatio, 0.1f, 2);
            if (EditorGUI.EndChangeCheck())
            {
                boidController.UpdateCamera();
            }
        }

        EditorGUILayout.Space();

        showBoidSettings = EditorGUILayout.Foldout(showBoidSettings, "Boid Settings");

        if (showBoidSettings)
        {
            EditorGUI.BeginChangeCheck();
            //Boid.maxForce = EditorGUILayout.Slider("Max Force", Boid.maxForce, 0, 1);
            //Boid.maxSpeed = EditorGUILayout.Slider("Max Speed", Boid.maxSpeed, 0, 20);
            if (EditorGUI.EndChangeCheck())
            {
                boidController.ResetBoids();
            }
        }
        //Mirkan war hier
    }
}
