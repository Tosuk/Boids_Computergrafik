using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boid : MonoBehaviour
{
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration = new Vector3(1, 1, 0);
    public float maxSpeed = 10f;
    public float maxForce = 0.03f;
    public float mass = 1f;
    public float perceptionRadius = 1f;
    public float avoidanceRadius = 0.5f;
    public float separationRadius = 0.5f;
    public LayerMask obstacleMask; // Layermask for obstacles

    private void Start()
    {
        velocity = new Vector3(Random.Range(-maxSpeed, maxSpeed), Random.Range(-maxSpeed, maxSpeed), 0);
        acceleration = Vector3.zero;
    }

    private void Update()
    {
        UpdateBoid();
    }

    public void UpdateBoid()
    {
        // Zufällige Beschleunigung hinzufügen
        //Vector3 randomAcceleration = new Vector3(Random.Range(-maxForce, maxForce), Random.Range(-maxForce, maxForce), 0);
        //ApplyForce(randomAcceleration);

        // Kollisionsvermeidung
        if (IsHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleAvoidance();
            if (collisionAvoidDir != Vector3.zero)
            {
                Vector3 avoidForce = SteerAvoidence(collisionAvoidDir) * 10f;
                ApplyForce(avoidForce);
            }
        }
 


        // Geschwindigkeit aktualisieren
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        velocity.z = 0; // Z-Koordinate fixieren
        transform.position += velocity * Time.deltaTime;

        // Position aktualisieren
        Vector3 newPosition = transform.position;
        newPosition.z = 0; // Z-Koordinate fixieren
        transform.position = newPosition;

        // Rotation aktualisieren
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, velocity);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 3f);

        // Beschleunigung zurücksetzen
        acceleration = Vector3.zero;
    }

    private void ApplyForce(Vector3 force)
    {
        Debug.DrawRay(transform.position, force, Color.blue);
        acceleration += force;
    }

    private Vector3 SteerAvoidence(Vector3 avoid)
    {
        Vector3 a = avoid.normalized * maxSpeed - velocity;
        return Vector3.ClampMagnitude(a, maxForce);
    }

    public bool IsHeadingForCollision()
    {
        Vector3 dir = velocity.normalized;
        Ray ray = new Ray(transform.position, dir);
        if (!Physics.SphereCast(ray, 0.27f, 5, obstacleMask))
        {
            Debug.DrawRay(transform.position, dir * 5, Color.green);
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, dir * 5, Color.red);
            return false;
        }
    }

    public Vector3 ObstacleAvoidance()
    {
        Vector3[] rayDirections = BoidFieldofview.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = transform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(transform.position, dir);
            if (!Physics.SphereCast(ray, 0.27f, 5, obstacleMask))
            {
                Debug.DrawRay(transform.position, dir * 5, Color.green);
                return dir;
            }
            else
            {
                Debug.DrawRay(transform.position, dir * 5, Color.red);
            }
        }
        return Vector3.zero;
    }
}

