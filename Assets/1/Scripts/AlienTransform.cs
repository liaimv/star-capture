using System.Collections;
using UnityEngine;

public class AlienTransform : MonoBehaviour
{
    private StarSpawn starSpawn;

    private float shrinkSpeed;
    private float rotationSpeed;

    private bool isPopped = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        starSpawn = FindFirstObjectByType<StarSpawn>();

        rotationSpeed = Random.Range(starSpawn.alienRotationSpeedMin, starSpawn.alienRotationSpeedMax);
        if (Random.value > 0.5f) rotationSpeed *= -1f;

        shrinkSpeed = Random.Range(starSpawn.alienShrinkSpeedMin, starSpawn.alienShrinkSpeedMax);
        shrinkSpeed /= 100f;

        StartCoroutine(PopAnimation());
    }

    private IEnumerator PopAnimation()
    {
        float growTime = starSpawn.alienPopDuration;
        float t1 = 0f;

        while (t1 < growTime)
        {
            float t = t1 / growTime;
            transform.localScale = Vector3.Lerp(Vector3.one * starSpawn.alienStartMinScale, Vector3.one * starSpawn.alienStartMaxScale, t);

            t1 += Time.deltaTime;
            yield return null;
        }

        float t2 = 0f;

        while (t2 < growTime)
        {
            float t = t2 / growTime;
            transform.localScale = Vector3.Lerp(Vector3.one * starSpawn.alienStartMaxScale, Vector3.one * starSpawn.alienOGScale, t);

            t2 += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one * starSpawn.alienOGScale;
        isPopped = true;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        //Shrinking
        if (isPopped)
        {
            Vector3 scale = Vector3.one * starSpawn.alienOGScale;
            transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;

            if (transform.localScale.x < 0.01f)
            {
                starSpawn.capturedStars.Remove(gameObject);
                Destroy(gameObject);
            }
        }

        transform.position += Vector3.forward * starSpawn.alienBackwardsSpeed * Time.deltaTime;
    }
}
