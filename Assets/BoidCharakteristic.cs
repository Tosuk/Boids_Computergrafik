using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoidCharakteristic", menuName = "BoidCharakteristic")]
public class BoidCharakteristic : ScriptableObject
{
    public float maxSpeed = 10f;
    public float maxForce = 0.03f;
    public float avoidanceRadius = 1;
    
    public float alignWeight = 1;
    public float cohesionWeight = 1;
    public float avoidenceWeight = 1;
  
}
