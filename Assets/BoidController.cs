using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    public static float separationRadius = 0.5f;
    public static float alignmentRadius = 1f;
    public static float cohesionRadius = 1f;

    //prefab for the boid
    [HideInInspector]
    public GameObject boidPrefab;
    [HideInInspector]
    public Camera cam;
    [HideInInspector]
    public int viewPortSize = 10;
    [HideInInspector]
    public float viewPortWidthAspectRatio= 1.6f;
    [HideInInspector]
    public float viewPortHeightAspectRatio = 0.9f;
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
        //BoidColor();
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

    //private void BoidColor()
    //{
    //    foreach (Boid boid in boids)
    //    {
    //        float diff = Vector3.Distance(boid.transform.position, boid.transform.position);
    //        if (diff < alignmentRadius )
    //        {
    //            if (Mathf.Abs(boid.velocity.x - boid.velocity.x) <= 0.3f &&
    //                    Mathf.Abs(boid.velocity.y - boid.velocity.y) <= 0.3f)
    //            {
    //                boid.spriteRenderer.color = boid.spriteRenderer.color;
    //            }
    //        }
    //    }
    //}
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
        viewPortHeight = viewPortSize * viewPortHeightAspectRatio;
        viewPortWidth = viewPortSize * viewPortWidthAspectRatio;
        Debug.Log("viewPortHeight: " + viewPortHeight);
        Debug.Log("viewPortWidth" + viewPortWidth);
    }

    public void setActiveBoidTrail(bool active)
    {
        foreach (Boid boid in boids)
        {
            boid.trail.enabled = active;
        }  
    }

    //keeps the boids within the bounds of the camera
    private void keepWithinBounds()
    {
        foreach (Boid boid in boids)
        {
            Vector3 desired = Vector3.zero;
            bool isOutOfBounds = false;

            if (boid.transform.position.x < -viewPortWidth + 3)
            {
                Debug.Log("viewPortWidth: " + (-viewPortWidth));
                desired = new Vector3(boid.maxSpeed, boid.velocity.y, 0);
                isOutOfBounds = true;
            }
            else if (boid.transform.position.x > viewPortWidth - 3)
            {
                desired = new Vector3(-boid.maxSpeed, boid.velocity.y, 0);
                isOutOfBounds = true;
            }
            if (boid.transform.position.y < -viewPortHeight + 1)
            {
                desired = new Vector3(boid.velocity.x, boid.maxSpeed, 0);
                isOutOfBounds = true;
            }
            else if (boid.transform.position.y > viewPortHeight - 1)
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
    //private void keepWithinBounds()
    //{
    //    foreach (Boid boid in boids)
    //    {
    //        if (boid.transform.position.x < -viewPortWidth)
    //        {
    //            boid.transform.position = new Vector3(viewPortWidth, boid.transform.position.y, 0);
    //        }
    //        if (boid.transform.position.x > viewPortWidth)
    //        {
    //            boid.transform.position = new Vector3(-viewPortWidth, boid.transform.position.y, 0);
    //        }
    //        if (boid.transform.position.y < -viewPortHeight)
    //        {
    //            boid.transform.position = new Vector3(boid.transform.position.x, viewPortHeight, 0);
    //        }
    //        if (boid.transform.position.y > viewPortHeight)
    //        {
    //            boid.transform.position = new Vector3(boid.transform.position.x, -viewPortHeight, 0);
    //        }
    //    }
}
