using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;
using System.Collections;
using Unity.VisualScripting;
using TMPro;

[System.Serializable]
public struct ShootingStarColorPair
{
    public Color mainColor;
    public Color secondaryColor;
}

public class StarSpawn : MonoBehaviour
{
    [Header("Star Variables")]
    public GameObject starParentObject;
    public GameObject spawnArea;

    [Header("Shooting Star Variable")]
    public GameObject shootingStarParentObject;
    public float shootingStarEventDelayMin = 3f;
    public float shootingStarEventDelayMax = 13f;

    [Header("Shooting Star Scale Variable")]
    public float shootingScaleMin = 1.5f;
    public float shootingScaleMax = 2.5f;
    public int shootingStarAmountforAlien = 5;
    public float twirlMaxScale = 0.11f;
    public float twirlMinScale = 0.04f;
    public float twirlStartMinScale = 0.01f;

    [Header("Shooting Star Transform Variables")]
    public float forceAmountMin = 10f;
    public float forceAmountMax = 20f;
    public float shootingStarStartPosXMin = 35f;
    public float shootingStarStartPosXMax = 60f;
    public float shootingStarStartPosYMin = -3.5f;
    public float shootingStarStartPosYMax = 0f;
    public float shootingStarConstantZPos = -1.9f;
    public float shootingStarShakeIntensity = 1f;

    [Header("Shooting Star Visual Variables")]
    public List<ShootingStarColorPair> shootingStarColors;

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
    public float starConstantZPos = 5.2f;

    [Header("Star Transform Variables")]
    public float shrinkSpeed = 0.1f;
    public float minScale = 0.05f;
    public float starShakeIntensity = 0.4f;
    private float shakeIntensity;

    [Header("Star Visual Variables")]
    [ColorUsage(true, true)]
    public List<Color> starColors;
    public float delayCapture = 0.5f;
    private VisualEffect captureImpact;
    public float spawnVFXDelay = 0.5f;
    public float spawnStarDelay = 0.5f;

    [Header("Alien Variables")]
    public List<GameObject> alienPrefabs;
    public float mergeSpeed = 5f;
    public float alienConstantZPos = -7f;
    private Dictionary<int, List<GameObject>> shootingStarGroups = new Dictionary<int, List<GameObject>>();

    [HideInInspector]
    public List<GameObject> spawnedStars = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> shootingStars = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> capturedStars = new List<GameObject>();


    void Start()
    {
        //Start star spawn
        starParentObject.SetActive(false);
        InvokeRepeating("SpawnSphere", 0f, spawnInterval);

        //Shooting star
        shootingStarParentObject.SetActive(false);
        StartCoroutine(RandomShootingStarEventRoutine());

        //Find bounds for captured area
        capturedAreaBounds = capturedStarArea.GetComponent<Renderer>().bounds;
    }

    public void Update()
    {
        //Stars moving around
        for (int i = 0; i < capturedStars.Count; i++)
        {
            if (capturedStars[i].CompareTag("Star"))
            {
                shakeIntensity = starShakeIntensity;
            }
            else if (capturedStars[i].CompareTag("Shooting Star"))
            {
                shakeIntensity = shootingStarShakeIntensity;
            }

            if (capturedStars[i] != null)
            {
                //Moving Around
                Vector3 floatOffset = new Vector3(
                    Mathf.Sin(Time.time + i) * shakeIntensity,
                    Mathf.Cos(Time.time + i) * shakeIntensity,
                    0
                );

                Rigidbody rb = capturedStars[i].GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.MovePosition(rb.position + floatOffset * Time.deltaTime);
                }
                else
                {
                    capturedStars[i].transform.position += floatOffset * Time.deltaTime;
                }

                if (capturedStars[i].CompareTag("Star")) //Shrink only normal stars
                {
                    //Shrinking
                    Vector3 scale = capturedStars[i].transform.localScale;
                    scale -= Vector3.one * shrinkSpeed * Time.deltaTime;

                    if (scale.x < minScale)
                    {
                        scale = Vector3.one * minScale;
                        GameObject star = capturedStars[i];
                        capturedStars.Remove(star);
                        Destroy(star.gameObject);
                        return;
                    }

                    capturedStars[i].transform.localScale = scale;
                }
            }
        }

        for (int i = 0; i < spawnedStars.Count; i++)
        {
            if (spawnedStars[i] != null)
            {
                //Moving Around
                Vector3 floatOffset = new Vector3(
                    Mathf.Sin(Time.time + i) * starShakeIntensity,
                    Mathf.Cos(Time.time + i) * starShakeIntensity,
                    0
                );

                spawnedStars[i].transform.position += floatOffset * Time.deltaTime;
            }
        }
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

            randomPosition = new Vector3(randomX, randomY, starConstantZPos);

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

                spawnedStars.Add(newStarParent);

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
                    StartCoroutine(StopSpawnVFXAfterDelay(spawnVFX, starRenderer, null));
                }
            }

            attempts++;
        }
    }

    private IEnumerator StopSpawnVFXAfterDelay(VisualEffect spawnVFX, MeshRenderer starRenderer, GameObject sparkleVFXObject)
    {
        yield return new WaitForSeconds(spawnVFXDelay);
        if (spawnVFX != null) spawnVFX.SetFloat("SpawnRate", 0f);

        StartCoroutine(ShowStarAfterDelay(starRenderer, sparkleVFXObject));
    }

    private IEnumerator ShowStarAfterDelay(MeshRenderer starRenderer, GameObject sparkleVFXObject)
    {
        yield return new WaitForSeconds(spawnStarDelay);

        if (sparkleVFXObject != null)
        {
            sparkleVFXObject.SetActive(true);
        }
        if (starRenderer != null) starRenderer.enabled = true;
        //GameObject newStarParent = newStar.transform.parent.gameObject;

        //spawnedStars.Add(newStarParent);
    }

    IEnumerator RandomShootingStarEventRoutine()
    {
        while (true)
        {
            float delay = Random.Range(shootingStarEventDelayMin, shootingStarEventDelayMax);
            yield return new WaitForSeconds(delay);

            SpawnShootingStar();
        }
    }

    void SpawnShootingStar()
    {
        GameObject shootingStarParent = Instantiate(shootingStarParentObject);

        shootingStars.Add(shootingStarParent);

        GameObject shootingStar = shootingStarParent.transform.GetChild(0).gameObject;

        shootingStarParent.SetActive(true);

        //Shooting star color initialization
        int colorIndex = Random.Range(0, shootingStarColors.Count);
        ShootingStarColorPair colorPair = shootingStarColors[colorIndex];

        shootingStarParent.name = "ShootingStar_" + colorIndex;

        Renderer shootingStarRenderer = shootingStar.GetComponent<Renderer>();

        shootingStarRenderer.material.SetColor("_MainColor", colorPair.mainColor);
        shootingStarRenderer.material.SetColor("_SecondaryColor", colorPair.secondaryColor);

        //Shooting star sparkles VFX color
        VisualEffect sparklesVFX = shootingStar.transform.GetChild(1).GetComponent<VisualEffect>(); 
        sparklesVFX.SetVector4("MainColor", colorPair.mainColor);
        sparklesVFX.SetVector4("SecondaryColor", colorPair.secondaryColor);

        //Random Scale
        float randomScale = Random.Range(shootingScaleMin, shootingScaleMax);
        shootingStarParent.transform.localScale = Vector3.one * randomScale;

        //Initialize start position
        float shootingStarStartPosX = Random.Range(shootingStarStartPosXMin, shootingStarStartPosXMax);
        float shootingStarStartPosY = Random.Range(shootingStarStartPosYMin, shootingStarStartPosYMax);

        Vector3 newPos = new Vector3(shootingStarStartPosX, shootingStarStartPosY, shootingStarConstantZPos);
        shootingStarParent.transform.position = newPos;

        //Shoot motion
        float forceAmount = Random.Range(forceAmountMin, forceAmountMax);
        Rigidbody shootingStarRB = shootingStarParent.GetComponent<Rigidbody>();
        shootingStarRB.AddForce(Vector3.left * forceAmount, ForceMode.Impulse);
    }

    public void MoveSequence(GameObject starParent)
    {
        GameObject star = starParent.transform.GetChild(0).gameObject;

        Color starCol = Color.white;
        float constantZPos = -2.5f;
        float captureVFXIntensity = 1f;

        //Remove star from its list
        if (spawnedStars.Contains(starParent))
        {
            constantZPos = starConstantZPos;
            starCol = star.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
            spawnedStars.Remove(starParent);
        }

        if (shootingStars.Contains(starParent))
        {
            captureVFXIntensity = 200f;
            constantZPos = shootingStarConstantZPos;
            Rigidbody shootingStarRB = starParent.GetComponent<Rigidbody>();

            Vector3 v = shootingStarRB.linearVelocity;
            if (v.x < 0f) v.x = 0f;
            shootingStarRB.linearVelocity = v;

            //Destroy(shootingStarRB);
            shootingStarRB.useGravity = false;
            shootingStarRB.linearVelocity = Vector3.zero;
            shootingStarRB.angularVelocity = Vector3.zero;
            //shootingStarRB.isKinematic = true;
            starCol = star.GetComponent<MeshRenderer>().material.GetColor("_SecondaryColor");
            shootingStars.Remove(starParent);
        }

        //Captured VFX
        captureImpact = star.transform.GetChild(0).GetComponent<VisualEffect>();
        if (captureImpact != null)
        {
            captureImpact.SetVector4("ImpactColor", starCol * captureVFXIntensity);
            captureImpact.Play();
        }

        //Find random captured position
        float randomX = Random.Range(capturedAreaBounds.min.x, capturedAreaBounds.max.x);
        float randomY = Random.Range(capturedAreaBounds.min.y, capturedAreaBounds.max.y);
        //float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 randomPos = new Vector3(randomX, randomY, constantZPos);

        //Disable collider
        //SphereCollider sphereCollider = starParent.GetComponent<SphereCollider>();
        //sphereCollider.enabled = false;

        StartCoroutine(MoveStar(starParent, randomPos));
    }

    private IEnumerator MoveStar(GameObject starParent, Vector3 newPos)
    {
        yield return new WaitForSeconds(delayCapture);

        GameObject star = starParent.transform.GetChild(0).gameObject;

        star.transform.GetChild(0).gameObject.SetActive(false); //Stop CapturedImpact VFX

        Rigidbody rb = starParent.GetComponent<Rigidbody>();

        MeshRenderer starRenderer = star.transform.GetComponent<MeshRenderer>();
        starRenderer.enabled = false;

        GameObject sparklesVFXObject = null;
        Vector4 sparklesColor = new Vector4(0, 0, 0, 0);
        GameObject spawnVFXObject = null;

        VisualEffect spawnVFX = null;

        if (rb != null)
        {
            sparklesVFXObject = star.transform.GetChild(1).gameObject;
            VisualEffect sparklesVFX = star.transform.GetChild(1).GetComponent<VisualEffect>();
            sparklesColor = sparklesVFX.GetVector4("SecondaryColor");
            sparklesColor *= 100;
            sparklesVFXObject.SetActive(false);

            spawnVFXObject = star.transform.GetChild(2).gameObject;
            spawnVFX = spawnVFXObject.GetComponent<VisualEffect>();
            spawnVFX.SetVector4("SpawnColor", sparklesColor);
            if (spawnVFX != null) spawnVFX.SetFloat("SpawnRate", 100f);
            spawnVFXObject.SetActive(true);

            rb.position = newPos;
        }
        else
        {
            spawnVFXObject = star.transform.GetChild(1).gameObject;
            spawnVFX = spawnVFXObject.GetComponent<VisualEffect>();
            if (spawnVFX != null) spawnVFX.SetFloat("SpawnRate", 100f);

            starParent.transform.position = newPos;
        }

        capturedStars.Add(starParent);

        //Naming shooting stars and group same colored ones
        if (starParent.name.StartsWith("ShootingStar_"))
        {
            int typeIndex = int.Parse(starParent.name.Split('_')[1]);

            if (!shootingStarGroups.ContainsKey(typeIndex))
            {
                shootingStarGroups[typeIndex] = new List<GameObject>();
            }
            
            shootingStarGroups[typeIndex].Add(starParent);
            
            if (shootingStarGroups[typeIndex].Count >= shootingStarAmountforAlien)
            {
                StartCoroutine(MergeIntoAlien(typeIndex, sparklesColor));
            }
        }

        StartCoroutine(StopSpawnVFXAfterDelay(spawnVFX, starRenderer, sparklesVFXObject));
    }

    private IEnumerator MergeIntoAlien(int typeIndex, Vector4 sparklesColor)
    {
        List<GameObject> group = new List<GameObject>(shootingStarGroups[typeIndex]);
        shootingStarGroups[typeIndex].Clear();

        //Make sure all shooting stars are captured before merging into an alien
        bool allCaptured = false;

        while (!allCaptured)
        {
            allCaptured = true;

            foreach (GameObject shootingStar in group)
            {
                if (!capturedStars.Contains(shootingStar))
                {
                    allCaptured = false;
                    break;
                }
            }

            yield return null;
        }

        //Get center point of shootingStars
        int randomIndex = Random.Range(0, group.Count - 1);
        GameObject randomStar = group[randomIndex];

        GameObject star = randomStar.transform.GetChild(0).gameObject;
        GameObject twirlObject = star.transform.GetChild(3).gameObject;
        Renderer twirlRenderer = twirlObject.GetComponent<Renderer>();
        Material twirlMaterial = twirlRenderer.material;
        sparklesColor /= 100f;
        Color twirlColor = Color.Lerp(sparklesColor, Color.white, 0.3f);
        twirlMaterial.SetColor("_Color", twirlColor);
        twirlObject.SetActive(false);
        twirlObject.transform.localScale = Vector3.one * twirlStartMinScale;

        Vector3 targetPosition = new Vector3(randomStar.transform.position.x, randomStar.transform.position.y, alienConstantZPos);

        StartCoroutine(MergeAlienTwirlDelay(group, targetPosition, typeIndex, twirlObject));
    }

    IEnumerator MergeAlienTwirlDelay(List<GameObject> group, Vector3 targetPosition, int typeIndex, GameObject twirlObject)
    {
        yield return new WaitForSeconds(delayCapture + 2f);

        twirlObject.SetActive(true);
        StartCoroutine(MergeAlienAfterDelay(group, targetPosition, typeIndex, twirlObject));
    }

    private IEnumerator MergeAlienAfterDelay(List<GameObject> group, Vector3 targetPosition, int typeIndex, GameObject twirlObject)
    {
        Vector3 twirlObjectMinScale = new Vector4(twirlMinScale, twirlMinScale, twirlMinScale);
        Vector3 twirlObjectStartMinScale = new Vector4(twirlStartMinScale, twirlStartMinScale, twirlStartMinScale);
        Vector3 twirlObjectMaxScale = new Vector3(twirlMaxScale, twirlMaxScale, twirlMaxScale);

        float growTime = delayCapture + 1f;
        float t1 = 0f;

        while (t1 < growTime)
        {
            float t = t1 / growTime;
            twirlObject.transform.localScale = Vector3.Lerp(twirlObjectStartMinScale, twirlObjectMaxScale, t);

            t1 += Time.deltaTime;
            yield return null;
        }

        twirlObject.transform.localScale = twirlObjectMaxScale;


        //yield return new WaitForSeconds(delayCapture + 1f);

        //Move shooting star to each other
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            twirlObject.transform.localScale = Vector3.Lerp(twirlObject.transform.localScale, twirlObjectMinScale, Time.deltaTime * mergeSpeed);
            foreach (GameObject shootingStar in group)
            {
                if (shootingStar != null)
                {
                    shootingStar.transform.position = Vector3.Lerp(shootingStar.transform.position, targetPosition, Time.deltaTime * mergeSpeed);
                    Rigidbody rb = shootingStar.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        //Remove shootingStar from capturedStars list and Destroy
        foreach (GameObject shootingStar in group)
        {
            capturedStars.Remove(shootingStar);
            Destroy(shootingStar);
        }

        twirlObject.SetActive(false);

        //Spawn Alien
        if (typeIndex < alienPrefabs.Count)
        {
            Quaternion alienRotation = Quaternion.Euler(0, 180f, 0);

            GameObject alien = Instantiate(alienPrefabs[typeIndex], targetPosition, alienRotation);
            capturedStars.Add(alien);
        }
    }
}
