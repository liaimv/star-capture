using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class StarSpawn : MonoBehaviour
{
    public GameObject starObject;
    public GameObject spawnArea;
    public float scaleMin = 0.3f;
    public float scaleMax = 0.6f;
    public float spawnInterval = 4f;
    public float minDistanceBetweenStars = 0.6f;
    public int maxStars = 50;

    private List<GameObject> spawnedStars = new List<GameObject>();


    public List<Color> starColors;

    void Start()
    {
        starObject.SetActive(false);
        InvokeRepeating("SpawnSphere", 0f, spawnInterval);
    }

    void SpawnSphere()
    {
        if (spawnedStars.Count >= maxStars)
            return;

        Bounds bounds = spawnArea.GetComponent<Renderer>().bounds;

        Vector3 randomPosition;
        bool validPosition = false;

        int attempts = 0;
        int maxAttempts = 20;

        while (!validPosition && attempts < maxAttempts)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);

            randomPosition = new Vector3(randomX, randomY, starObject.transform.position.z);

            validPosition = true;

            foreach (GameObject star in spawnedStars)
            {
                if (Vector3.Distance(randomPosition, star.transform.position) < minDistanceBetweenStars)
                {
                    validPosition = false;
                    break;
                }
            }

            if (validPosition)
            {
                GameObject newSphere = Instantiate(starObject, randomPosition, Quaternion.identity);

                //Random scale
                float randomScale = Random.Range(scaleMin, scaleMax);
                newSphere.transform.localScale = Vector3.one * randomScale;

                //Random color
                Color randomColor = starColors[Random.Range(0, starColors.Count)];
                Renderer renderer = newSphere.GetComponent<Renderer>();
                renderer.material.SetColor("_EmissionColor", randomColor);

                newSphere.SetActive(true);

                spawnedStars.Add(newSphere);
            }

            attempts++;
        }
    }

    void Update()
    {
        
    }
}
