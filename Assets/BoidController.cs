using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UI;

public class BoidController : MonoBehaviour
{
    private List<Boid> boids = new List<Boid>();
    public GameObject boidPrefab;
    [Range(1, 100)]
    public int numBoids = 10;
    public bool ShowDirection = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numBoids; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-8, 8), Random.Range(-4.9f, 4.9f), 0);
            GameObject boid = Instantiate(boidPrefab, pos, Quaternion.identity);
            boid.transform.parent = transform;
            boids.Add(boid.GetComponent<Boid>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        BoidCount();
        OnBecameInvisible();
        //foreach (Boid boid in boids)
        //{
        //    boid.UpdateBoid();
        //}
    }

    private void OnDrawGizmos()
    {
        foreach (Boid boid in boids)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(boid.transform.position, boid.velocity);
        }
    }

   private void BoidCount() 
    {
        int currentBoids = boids.Count;
        if (currentBoids < numBoids)
        {
            for (int i = 0; i < numBoids - boids.Count; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-8, 8), Random.Range(-4.9f, 4.9f), 0);
                GameObject boid = Instantiate(boidPrefab, pos, Quaternion.identity);
                boid.transform.parent = transform;
                boids.Add(boid.GetComponent<Boid>());
            }
        }
        if (currentBoids > numBoids)
        {
            for (int i = 0; i < boids.Count - numBoids; i++)
            {
                Destroy(boids[boids.Count - 1].gameObject);
                boids.RemoveAt(boids.Count - 1);
            }
        }
    }


    private void OnBecameInvisible()
    {
        foreach (Boid boid in boids)
        {
            Vector3 pos = boid.transform.position;
            if (pos.x > 9)
            {
                boid.transform.position = new Vector3(-9, pos.y, pos.z);
            }
            else if (pos.x < -9)
            {
                boid.transform.position = new Vector3(9, pos.y, pos.z);
            }
            if (pos.y > 6f)
            {
                boid.transform.position = new Vector3(pos.x, -6, pos.z);
            }
            else if (pos.y < -6f)
            {
                boid.transform.position = new Vector3(pos.x, 6f, pos.z);
            }
        }
    }
}
