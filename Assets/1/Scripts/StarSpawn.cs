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

    public List<GameObject> spawnedStars = new List<GameObject>();

    [ColorUsage(true, true)]
    public List<Color> starColors;
<<<<<<< Updated upstream
=======
    public float delayCapture = 0.5f;
    private VisualEffect captureImpact;
    public float spawnVFXDelay = 0.5f;
    public float spawnStarDelay = 0.5f;

    [Header("Star Audio")]
    public List<AudioClip> starAudioClips;         // Same order as starColors list
    public List<AudioClip> shootingStarAudioClips; // Same order as shootingStarColors list

    [HideInInspector]
    public List<GameObject> spawnedStars = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> shootingStars = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> capturedStars = new List<GameObject>();

>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
                GameObject newSphere = Instantiate(starObject, randomPosition, Quaternion.identity);
=======
                GameObject newStarParent = Instantiate(starParentObject, randomPosition, Quaternion.identity); //Use parent for transform (position, scale, and collider)
                GameObject newStar = newStarParent.transform.GetChild(0).gameObject;

                spawnedStars.Add(newStarParent);

                //Random color
                int colorIndex = Random.Range(0, starColors.Count);
                Color randomColor = starColors[colorIndex];
                Renderer renderer = newStar.GetComponent<Renderer>();
                renderer.material.SetColor("_EmissionColor", randomColor);

                // Assign matching audio clip to this star
                AudioClip starClip = (colorIndex < starAudioClips.Count) ? starAudioClips[colorIndex] : null;
                newStarParent.GetComponent<StarAudio>()?.SetClip(starClip);

                VisualEffect spawnVFX = newStar.transform.GetChild(1).GetComponent<VisualEffect>();
                MeshRenderer starRenderer = newStar.GetComponent<MeshRenderer>();

                Color starCol = starRenderer.material.GetColor("_EmissionColor");
                spawnVFX.SetVector4("SpawnColor", starCol);
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
        Vector3 pos = star.transform.position;
        star.transform.position = -pos;
=======
        yield return new WaitForSeconds(spawnStarDelay);

        if (starRenderer != null) starRenderer.enabled = true;
        GameObject newStarParent = newStar.transform.parent.gameObject;

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
        int colorPairIndex = Random.Range(0, shootingStarColors.Count);
        ShootingStarColorPair colorPair = shootingStarColors[colorPairIndex];

        Renderer shootingStarRenderer = shootingStar.GetComponent<Renderer>();

        shootingStarRenderer.material.SetColor("_MainColor", colorPair.mainColor);
        shootingStarRenderer.material.SetColor("_SecondaryColor", colorPair.secondaryColor);

        // Assign matching audio clip to this shooting star
        AudioClip shootingClip = (colorPairIndex < shootingStarAudioClips.Count) ? shootingStarAudioClips[colorPairIndex] : null;
        shootingStarParent.GetComponent<StarAudio>()?.SetClip(shootingClip);

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
            Destroy(shootingStarRB);
            //shootingStarRB.useGravity = false;
            //shootingStarRB.linearVelocity = Vector3.zero;
            //shootingStarRB.angularVelocity = Vector3.zero;
            //shootingStarRB.isKinematic = true;
            starCol = star.GetComponent<MeshRenderer>().material.GetColor("_SecondaryColor");
            shootingStars.Remove(starParent);
        }

        // Play the color-matched capture sound
        starParent.GetComponent<StarAudio>()?.PlayCaptureSound();

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
        SphereCollider sphereCollider = starParent.GetComponent<SphereCollider>();
        sphereCollider.enabled = false;

        StartCoroutine(MoveStar(starParent, randomPos));
    }

    private IEnumerator MoveStar(GameObject starParent, Vector3 newPos)
    {
        yield return new WaitForSeconds(delayCapture);

        GameObject star = starParent.transform.GetChild(0).gameObject;

        star.transform.GetChild(0).gameObject.SetActive(false); //Stop CapturedImpact VFX

        starParent.transform.position = newPos;
        capturedStars.Add(starParent);
>>>>>>> Stashed changes
    }
}