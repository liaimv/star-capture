using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.VFX;
using System.Collections;

public class StarSpawn : MonoBehaviour
{
    [Header("Star Variables")]
    public GameObject starObject;
    public GameObject spawnArea;

    [Header("Captured Star Variables")]
    public GameObject capturedStarArea;
    private Bounds capturedAreaBounds;

    [Header("Star Scale Variables")]
    public float scaleMin = 0.3f;
    public float scaleMax = 0.6f;

    [Header("Star Spawn Variables")]
    public float spawnInterval = 4f;
    public float minDistanceBetweenStars = 0.6f;
    public int maxStars = 50;

    [Header("Star Transform Variables")]
    public float moveBackSpeed = 0.1f;
    public float shrinkSpeed = 0.1f;
    public float minScale = 0.05f;

    [Header("Star Visual Variables")]
    [ColorUsage(true, true)]
    public List<Color> starColors;
    public float delayCapture = 0.5f;
    private VisualEffect captureImpact;
    private Color captureImpactColor;
    public float spawnVFXdelay = 0.5f;

    [HideInInspector]
    public List<GameObject> spawnedStars = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> capturedStars = new List<GameObject>();

    void Start()
    {
        //Start star spawn
        starObject.SetActive(false);
        InvokeRepeating("SpawnSphere", 0f, spawnInterval);

        //Find bounds for captured area
        capturedAreaBounds = capturedStarArea.GetComponent<Renderer>().bounds;
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

                VisualEffect spawnVFX = newSphere.GetComponentInChildren<VisualEffect>();
                MeshRenderer starRenderer = newSphere.GetComponent<MeshRenderer>();

                //Random scale
                float randomScale = Random.Range(scaleMin, scaleMax);
                newSphere.transform.localScale = Vector3.one * randomScale;

                //Random color
                Color randomColor = starColors[Random.Range(0, starColors.Count)];
                Renderer renderer = newSphere.GetComponent<Renderer>();
                renderer.material.SetColor("_EmissionColor", randomColor);

                newSphere.SetActive(true);

                spawnedStars.Add(newSphere);

                if (spawnVFX != null && starRenderer != null)
                {
                    StartCoroutine(StopSpawnVFXAfterDelay(spawnVFX, starRenderer));
                }
            }

            attempts++;
        }
    }

    private IEnumerator StopSpawnVFXAfterDelay(VisualEffect spawnVFX, MeshRenderer starRenderer)
    {
        yield return new WaitForSeconds(spawnVFXdelay);
        if (spawnVFX != null) spawnVFX.SetFloat("SpawnRate", 0f);

        StartCoroutine(ShowStarAfterDelay(starRenderer));
    }

    private IEnumerator ShowStarAfterDelay(MeshRenderer starRenderer)
    {
        yield return new WaitForSeconds(spawnVFXdelay + 0.4f);

        if (starRenderer != null) starRenderer.enabled = true;
    }

    public void MoveSequence(GameObject star)
    {
        spawnedStars.Remove(star);

        //Captured VFX
        Transform firstChild = star.transform.GetChild(0);
        captureImpact = firstChild.GetComponent<VisualEffect>();
        if (captureImpact != null)
        {
            Color starCol = star.GetComponent<MeshRenderer>().material.color;
            captureImpact.SetVector4("ImpactColor", starCol);
            captureImpact.Play();
        }

        //Find random captured position
        float randomX = Random.Range(capturedAreaBounds.min.x, capturedAreaBounds.max.x);
        float randomY = Random.Range(capturedAreaBounds.min.y, capturedAreaBounds.max.y);
        //float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 randomPos = new Vector3(randomX, randomY, 0);

        StartCoroutine(MoveStar(star, randomPos));
    }

    private IEnumerator MoveStar(GameObject star, Vector3 newPos)
    {
        yield return new WaitForSeconds(delayCapture);

        star.transform.position = newPos;
        capturedStars.Add(star);
    }

    public void Update()
    {
        for (int i = 0; i < capturedStars.Count; i++)
        {
            if (capturedStars[i] != null)
            {
                Vector3 floatOffset = new Vector3(
                    Mathf.Sin(Time.time + i) * 0.2f,
                    Mathf.Cos(Time.time + i) * 0.2f,
                    0
                );

                capturedStars[i].transform.position += floatOffset * Time.deltaTime;

                Vector3 scale = capturedStars[i].transform.localScale;
                scale -= Vector3.one * shrinkSpeed * Time.deltaTime;

                if (scale.x < minScale)
                {
                    scale = Vector3.one * minScale;
                }

                capturedStars[i].transform.localScale = scale;
            }
        }

        for (int i = 0; i < spawnedStars.Count; i++)
        {
            if (spawnedStars[i] != null)
            {
                Vector3 floatOffset = new Vector3(
                    Mathf.Sin(Time.time + i) * 0.2f,
                    Mathf.Cos(Time.time + i) * 0.2f,
                    moveBackSpeed
                );

                spawnedStars[i].transform.position += floatOffset * Time.deltaTime;
            }
        }
    }
}
