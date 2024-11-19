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

    BoidCharakteristic boidCharakteristic;

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
        Vector3 collisionAvoidDir = ObstacleAvoidance();
        if (collisionAvoidDir != Vector3.zero)
        {
            Vector3 avoidForce = SteerAvoidence(collisionAvoidDir);
            ApplyForce(avoidForce * BoidController.avoidenceWeight);
        }

        // Alignment (Ausrichtung)
        Vector3 alignment = GetLineAlignment();
        if (alignment != Vector3.zero)
        {
            ApplyForce(alignment * BoidController.alignWeight);
        }

        // Cohesion (Zusammenhalt)
        Vector3 cohesion = GetCohesion();
        if (cohesion != Vector3.zero)
        {
            Vector3 cohesionForce = SteerAvoidence(cohesion);
            ApplyForce(cohesionForce * BoidController.cohesionWeight);
        }

        // Wanderkraft hinzufügen, wenn keine anderen Kräfte wirken
        if (acceleration == Vector3.zero)
        {
            ApplyForce(Wander());
        }

        keepWithinBounds(); // Boids innerhalb der Grenzen halten

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


    private void keepWithinBounds()
    {
        Vector3 desired = Vector3.zero;
        if (transform.position.x < -16)
        {
            desired = new Vector3(maxSpeed, velocity.y, 0);
        }
        else if (transform.position.x > 16)
        {
            desired = new Vector3(-maxSpeed, velocity.y, 0);
        }
        if (transform.position.y < -9.9f)
        {
                desired = new Vector3(velocity.x, maxSpeed, 0);
        }
        else if (transform.position.y > 9.9f)
        {
            desired = new Vector3(velocity.x, -maxSpeed, 0);
        }
        Vector3 steer = desired - velocity;
        ApplyForce(steer);
    }

    private void ApplyForce(Vector3 force)
    {
        //Debug.DrawRay(transform.position, force, Color.blue);
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

    public Vector3 GetLineAlignment()
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

    private Vector3 Wander()
    {
        float wanderRadius = 1.5f;
        float wanderDistance = 2f;
        float wanderJitter = 0.2f;

        Vector3 wanderTarget = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0
        );

        wanderTarget = wanderTarget.normalized * wanderRadius;
        Vector3 targetLocal = wanderTarget + new Vector3(0, wanderDistance, 0);
        Vector3 targetWorld = transform.TransformPoint(targetLocal);

        return (targetWorld - transform.position).normalized * maxForce;
    }

}

