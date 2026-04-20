using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;
using System.Collections;

public class StarSpawn : MonoBehaviour
{
    [Header("Star Variables")]
    public GameObject starParentObject;
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
    public float constantZPos = 5.2f;

    [Header("Star Transform Variables")]
    public float moveBackSpeed = 0.1f;
    public float shrinkSpeed = 0.1f;
    public float minScale = 0.05f;
    public float shakeIntensity = 0.4f;

    [Header("Star Visual Variables")]
    [ColorUsage(true, true)]
    public List<Color> starColors;
    public float delayCapture = 0.5f;
    private VisualEffect captureImpact;
    private Color captureImpactColor;
    public float spawnVFXDelay = 0.5f;
    public float spawnStarDelay = 0.5f;

    [Header("Star Rotation Variables")]
    public float starRotationSpeedMin;
    public float starRotationSpeedMax;
    private Dictionary<GameObject, float> starRotationSpeeds = new Dictionary<GameObject, float>();

    [HideInInspector]
    public List<GameObject> spawnedStars = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> capturedStars = new List<GameObject>();

    void Start()
    {
        //Start star spawn
        starParentObject.SetActive(false);
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

            randomPosition = new Vector3(randomX, randomY, constantZPos);

            validPosition = true;

            foreach (GameObject starParent in spawnedStars)
            {
                if (Vector3.Distance(randomPosition, starParent.transform.position) < minDistanceBetweenStars)
                {
                    validPosition = false;
                    break;
                }
            }

            if (validPosition)
            {
                GameObject newStarParent = Instantiate(starParentObject, randomPosition, Quaternion.identity); //Use parent for transform (position, scale, and collider)
                GameObject newStar = newStarParent.transform.GetChild(0).gameObject;

                //Create random rotation speed for each star and save it in a dictionary
                float starRotationSpeed = Random.Range(starRotationSpeedMin, starRotationSpeedMax);
                if (Random.value > 0.5f) starRotationSpeed *= -1f;
                starRotationSpeeds.Add(newStarParent, starRotationSpeed);

                //Random color
                Color randomColor = starColors[Random.Range(0, starColors.Count)];
                Renderer renderer = newStar.GetComponent<Renderer>();
                renderer.material.SetColor("_EmissionColor", randomColor);

                VisualEffect spawnVFX = newStar.transform.GetChild(1).GetComponent<VisualEffect>();
                MeshRenderer starRenderer = newStar.GetComponent<MeshRenderer>();

                Color starCol = starRenderer.material.GetColor("_EmissionColor");
                spawnVFX.SetVector4("SpawnColor", starCol);

                //Random scale
                float randomScale = Random.Range(scaleMin, scaleMax);
                newStarParent.transform.localScale = Vector3.one * randomScale;

                newStarParent.SetActive(true);

                if (spawnVFX != null && starRenderer != null)
                {
                    StartCoroutine(StopSpawnVFXAfterDelay(spawnVFX, starRenderer, newStar));
                }
            }

            attempts++;
        }
    }

    private IEnumerator StopSpawnVFXAfterDelay(VisualEffect spawnVFX, MeshRenderer starRenderer, GameObject newStar)
    {
        yield return new WaitForSeconds(spawnVFXDelay);
        if (spawnVFX != null) spawnVFX.SetFloat("SpawnRate", 0f);

        StartCoroutine(ShowStarAfterDelay(starRenderer, newStar));
    }

    private IEnumerator ShowStarAfterDelay(MeshRenderer starRenderer, GameObject newStar)
    {
        yield return new WaitForSeconds(spawnStarDelay);

        if (starRenderer != null) starRenderer.enabled = true;
        GameObject newStarParent = newStar.transform.parent.gameObject;

        spawnedStars.Add(newStarParent);
    }

    public void MoveSequence(GameObject starParent)
    {
        GameObject star = starParent.transform.GetChild(0).gameObject;

        spawnedStars.Remove(starParent);

        //Captured VFX
        captureImpact = star.transform.GetChild(0).GetComponent<VisualEffect>();
        if (captureImpact != null)
        {
            Color starCol = star.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
            captureImpact.SetVector4("ImpactColor", starCol);
            captureImpact.Play();
        }

        //Find random captured position
        float randomX = Random.Range(capturedAreaBounds.min.x, capturedAreaBounds.max.x);
        float randomY = Random.Range(capturedAreaBounds.min.y, capturedAreaBounds.max.y);
        //float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 randomPos = new Vector3(randomX, randomY, constantZPos);

        StartCoroutine(MoveStar(starParent, randomPos));
    }

    private IEnumerator MoveStar(GameObject starParent, Vector3 newPos)
    {
        yield return new WaitForSeconds(delayCapture);

        GameObject star = starParent.transform.GetChild(0).gameObject; 

        star.transform.GetChild(0).gameObject.SetActive(false); //Stop CapturedImpact VFX

        starParent.transform.position = newPos;
        capturedStars.Add(starParent);
    }

    public void Update()
    {
        //Stars moving around
        for (int i = 0; i < capturedStars.Count; i++)
        {
            if (capturedStars[i] != null)
            {
                //Moving Around
                Vector3 floatOffset = new Vector3(
                    Mathf.Sin(Time.time + i) * shakeIntensity,
                    Mathf.Cos(Time.time + i) * shakeIntensity,
                    0
                );

                capturedStars[i].transform.position += floatOffset * Time.deltaTime;

                //Shrinking
                Vector3 scale = capturedStars[i].transform.localScale;
                scale -= Vector3.one * shrinkSpeed * Time.deltaTime;

                if (scale.x < minScale)
                {
                    scale = Vector3.one * minScale;
                }

                capturedStars[i].transform.localScale = scale;

                //Rotation (make sure no error happens when something gets removed)
                GameObject star = capturedStars[i];

                if (starRotationSpeeds.ContainsKey(star))
                {
                    float starRotationSpeed = starRotationSpeeds[star];
                    star.transform.Rotate(Vector3.forward * starRotationSpeed * Time.deltaTime);
                }
            }
        }

        for (int i = 0; i < spawnedStars.Count; i++)
        {
            if (spawnedStars[i] != null)
            {
                //Moving Around
                Vector3 floatOffset = new Vector3(
                    Mathf.Sin(Time.time + i) * shakeIntensity,
                    Mathf.Cos(Time.time + i) * shakeIntensity,
                    moveBackSpeed
                );

                spawnedStars[i].transform.position += floatOffset * Time.deltaTime;

                //Rotation (make sure no error happens when something gets removed)
                GameObject star = spawnedStars[i];

                if (starRotationSpeeds.ContainsKey(star))
                {
                    float starRotationSpeed = starRotationSpeeds[star];
                    star.transform.Rotate(Vector3.forward * starRotationSpeed * Time.deltaTime);
                }
            }
        }
    }
}
