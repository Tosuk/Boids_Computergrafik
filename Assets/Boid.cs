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

    public SpriteRenderer spriteRenderer;
    
    public TrailRenderer trail;

    //private Vector3 separationForce, alignmentForce, cohesionForce;


    private void Start()
    {
        trail = GetComponent<TrailRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

        velocity = new Vector3(Random.Range(-maxSpeed, maxSpeed), Random.Range(-maxSpeed, maxSpeed), 0);
        acceleration = Vector3.zero;
    }

    private void Update()
    {
        UpdateBoid();
    }

    public void UpdateBoid()
    {
        // Seperation (Abstand halten)
        Vector3 separation = GetSeparation();
        if (separation != Vector3.zero)
        {
            Vector3 separationForce = SteerTorwards(separation) * BoidController.avoidenceWeight;
            ApplyForce(separationForce);
            //ApplyForce(separation * BoidController.avoidenceWeight);
        }

        // Alignment (Ausrichtung)
        Vector3 alignment = GetAlignment();
        if (alignment != Vector3.zero)
        {
            Vector3 alignmentForce = SteerTorwards(alignment) * BoidController.alignWeight;
            ApplyForce(alignmentForce);
            //ApplyForce(alignment * BoidController.alignWeight);
        }

        // Cohesion (Zusammenhalt)
        Vector3 cohesion = GetCohesion();
        if (cohesion != Vector3.zero)
        {
            Vector3 cohesionForce = SteerTorwards(cohesion) * BoidController.cohesionWeight;
            ApplyForce(cohesionForce);
            //ApplyForce(cohesion * BoidController.cohesionWeight);
        }


        // Geschwindigkeit aktualisieren
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        velocity.z = 0; // Z-Koordinate fixieren
        transform.position += velocity * Time.deltaTime;

        // Rotation aktualisieren
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, velocity);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 3f);

        // Beschleunigung zurücksetzen
        acceleration = Vector3.zero;
    }

    private void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }

    private Vector3 SteerTorwards(Vector3 avoid)
    {
        Vector3 a = avoid.normalized * maxSpeed - velocity;
        return Vector3.ClampMagnitude(a, maxForce);
    }

    //
    //
    //Implentiere hier die drei Regeln für Boids
    //
    //

    public Vector3 GetSeparation()
    {
        return Vector3.zero;
    }

    public Vector3 GetAlignment()
    {
        return Vector3.zero;
    }

    public Vector3 GetCohesion()
    {
        return Vector3.zero;
    }
}

