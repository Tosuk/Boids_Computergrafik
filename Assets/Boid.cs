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
    public float avoidanceRadius = 1;
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
        // Seperation (Abstand halten)
        Vector3 separation = GetSeparation();
        if (separation != Vector3.zero)
        {
            Vector3 separationForce = SteerTorwards(separation) * BoidController.avoidenceWeight;
            ApplyForce(separationForce);
        }

        // Alignment (Ausrichtung)
        Vector3 alignment = GetAlignment();
        if (alignment != Vector3.zero)
        {
            Vector3 alignmentForce = SteerTorwards(alignment) * BoidController.alignWeight;
            ApplyForce(alignmentForce);
        }

        // Cohesion (Zusammenhalt)
        Vector3 cohesion = GetCohesion();
        if (cohesion != Vector3.zero)
        {
            Vector3 cohesionForce = SteerTorwards(cohesion) * BoidController.cohesionWeight;
            ApplyForce(cohesionForce);
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

    public Vector3 GetSeparation()
    {
        Vector3 avoidenceVector = Vector3.zero;
        int avoidenceCount = 0; // Anzahl der Boids, die vermieden werden
        List<Boid> boids = BoidController.GetBoids();
        foreach (Boid boid in boids)
        {
            if (boid != this)
            {
                Vector3 diff = transform.position - boid.transform.position;
                //Debug.DrawRay(transform.position, diff, Color.green);
                if (diff.magnitude < avoidanceRadius)
                {
                    avoidenceVector += diff;
                    avoidenceCount++;
                    //Debug.DrawRay(transform.position, diff, Color.red);
                }
            }
        }
        if (avoidenceCount > 0)
        {
            avoidenceVector /= avoidenceCount;
        }
        return avoidenceVector.normalized;
    }

    public Vector3 GetAlignment()
    {
        List<Boid> boids = BoidController.GetBoids();
        Vector3 averageHeading = Vector3.zero;
        int neighbourCount = 0;
        foreach (Boid boid in boids)
        {
            if (boid != this)
            {
                float distance = Vector3.Distance(transform.position, boid.transform.position);
                if (distance < perceptionRadius)
                {
                    averageHeading += boid.velocity;
                    neighbourCount++;
                }
            }
        }
        if (neighbourCount > 0)
        {
            averageHeading /= neighbourCount;
        }
        return averageHeading.normalized;
    }

    public Vector3 GetCohesion()
    {
        List<Boid> boids = BoidController.GetBoids();
        Vector3 averagePosition = Vector3.zero;
        int neighbourCount = 0;
        foreach (Boid boid in boids)
        {
            if (boid != this)
            {
                float distance = Vector3.Distance(transform.position, boid.transform.position);
                if (distance < perceptionRadius)
                {
                    //Debug.DrawRay(transform.position, boid.transform.position - transform.position, Color.blue);
                    averagePosition += boid.transform.position;
                    neighbourCount++;
                }
            }
        }
        if (neighbourCount > 0)
        {
            averagePosition /= neighbourCount;
            averagePosition -= transform.position;
        }
        return averagePosition.normalized;
    }
}

