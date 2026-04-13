using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class StarSpawn : MonoBehaviour
{
    public GameObject starObject;
    public GameObject spawnArea;
    public GameObject catchedSpawnArea;
    public float scaleMin = 0.3f;
    public float scaleMax = 0.6f;
    public float spawnInterval = 4f;
    public float minDistanceBetweenStars = 0.6f;
    public int maxStars = 50;
    public float moveBackSpeed = 0.1f;
    public float shrinkSpeed = 0.1f;
    public float minScale = 0.05f;

    public List<GameObject> spawnedStars = new List<GameObject>();
    public List<GameObject> catchedStars = new List<GameObject>();

    [ColorUsage(true, true)]
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
            //float randomZ = Random.Range(bounds.min.z, bounds.max.z);

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

    public void RemoveStar(GameObject star)
    {
        spawnedStars.Remove(star);
    }

    public void MoveStar(GameObject star)
    {
        Vector3 pos = star.transform.position;
        star.transform.position = new Vector3(pos.x, -pos.y, pos.z);

        catchedStars.Add(star);
    }

    public void Update()
    {
        for (int i = 0; i < catchedStars.Count; i++)
        {
            if (catchedStars[i] != null)
            {
                Vector3 floatOffset = new Vector3(
                    Mathf.Sin(Time.time + i) * 0.2f,
                    Mathf.Cos(Time.time + i) * 0.2f,
                    0
                );

                catchedStars[i].transform.position += floatOffset * Time.deltaTime;

                Vector3 scale = catchedStars[i].transform.localScale;
                scale -= Vector3.one * shrinkSpeed * Time.deltaTime;

                if (scale.x < minScale)
                {
                    scale = Vector3.one * minScale;
                }

                catchedStars[i].transform.localScale = scale;
            }
        }

        for (int i = 0; i < spawnedStars.Count; i++)
        {
            if (spawnedStars[i] != null)
            {
                Vector3 floatOffset = new Vector3(
                    Mathf.Sin(Time.time + i) * 0.2f,
                    Mathf.Cos(Time.time + i) * 0.2f,
                    0
                );

                spawnedStars[i].transform.position += floatOffset * Time.deltaTime;
            }
        }
    }
}
