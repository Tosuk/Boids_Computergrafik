using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class BoidController : MonoBehaviour
{
    //[HideInInspector]
    private static List<Boid> boids = new List<Boid>();

    //weights for the boid behaviours and the vision radius
    public static int alignWeight = 5;
    public static int cohesionWeight = 5;
    public static int avoidenceWeight = 7;
    public static int visionRadius = 5;

    //prefab for the boid
    [HideInInspector]
    public GameObject boidPrefab;
    [HideInInspector]
    public Camera cam;
    public int viewPortSize = 10;
    private float viewPortWidth;
    private float viewPortHeight;


    [Range(1, 100)]
    public int numBoids = 10;
    private bool ShowGui = false;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numBoids; i++)
        {
            AddBoid();
        }
        UpdateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        BoidCount();
        keepWithinBounds();
    }

    //draws the gizmos for the boids for debugging purposes
    private void OnDrawGizmos()
    {
        foreach (Boid boid in boids)
        {
            if (ShowGui)
            {
                BoidsGui(boid);
            }
        }
    }

    //draws the gui for the boids for debugging purposes
    private void BoidsGui(Boid boid)
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(boid.transform.position, visionRadius);
        Gizmos.DrawRay(boid.transform.position, boid.velocity);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(boid.transform.position, boid.acceleration);
    }

    //toggles the gui on and off
    public bool showGui()
    {
        return ShowGui = !ShowGui;
    }

    //checks the number of boids in the scene and adds or removes boids to match the number of boids set in the editor
    private void BoidCount()
    {
        int currentBoids = boids.Count;
        if (currentBoids < numBoids)
        {
            for (int i = 0; i < numBoids - boids.Count; i++)
            {
                AddBoid();
            }
        }
        if (currentBoids > numBoids)
        {
            for (int i = 0; i < boids.Count - numBoids; i++)
            {
                RemoveBoid();
            }
        }
    }

    //returns the list of boids
    public static List<Boid> GetBoids()
    {
        return boids;
    }

    //removes a boid from the scene and the list of boids
    public void RemoveBoid()
    {
        if (boids.Count > 0)
        {
            Boid boid = boids[boids.Count - 1];
            boids.RemoveAt(boids.Count - 1);
            Destroy(boid.gameObject);
        }
    }

    //adds a boid to the scene and the list of boids
    public void AddBoid()
    {
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-8, 8), UnityEngine.Random.Range(-4.9f, 4.9f), 0);
        GameObject boid = Instantiate(boidPrefab, pos, Quaternion.identity);
        boid.transform.parent = transform;
        boids.Add(boid.GetComponent<Boid>());
    }

    //generates boids not used in the final version
    public void GenerateBoids()
    {
        for (int i = 0; i < numBoids; i++)
        {
            AddBoid();
        }
    }

    //clears all boids 
    public void ClearBoids()
    {
        while (boids.Count > 0)
        {
            RemoveBoid();
        }
    }

    //resets the boids uses only the clear boids function but could be used to reset if BoidCount() is changed
    internal void ResetBoids()
    {
        ClearBoids();
        //GenerateBoids();
    }

    internal void UpdateCamera()
    {
        cam.orthographicSize = viewPortSize;
        UpdateBounds();
    }

    internal void UpdateBounds()
    {
        viewPortHeight = 0.9f;
        viewPortWidth = 1.6f;
        viewPortHeight *= viewPortSize;
        viewPortWidth *= viewPortSize;
        Debug.Log("viewPortHeight: " + viewPortHeight);
        Debug.Log("viewPortWidth" + viewPortWidth);
    }

    //keeps the boids within the bounds of the camera
    private void keepWithinBounds()
    {
        foreach (Boid boid in boids)
        {
            Vector3 desired = Vector3.zero;
            bool isOutOfBounds = false;

            if (boid.transform.position.x < -viewPortWidth)
            {
                Debug.Log("Out of bounds: " + boid.transform.position);
                Debug.Log("viewPortWidth: " + viewPortWidth);
                desired = new Vector3(boid.maxSpeed, boid.velocity.y, 0);
                isOutOfBounds = true;
            }
            else if (boid.transform.position.x > viewPortWidth)
            {
                desired = new Vector3(-boid.maxSpeed, boid.velocity.y, 0);
                isOutOfBounds = true;
            }
            if (boid.transform.position.y < -viewPortHeight)
            {
                desired = new Vector3(boid.velocity.x, boid.maxSpeed, 0);
                isOutOfBounds = true;
            }
            else if (boid.transform.position.y > viewPortHeight)
            {
                desired = new Vector3(boid.velocity.x, -boid.maxSpeed, 0);
                isOutOfBounds = true;
            }

            if (isOutOfBounds)
            {
                Vector3 steer = desired - boid.velocity;
                boid.acceleration += steer;
            }
        }
    }
}