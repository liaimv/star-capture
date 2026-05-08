using UnityEngine;

public class StarRotation : MonoBehaviour
{
    public float starRotationSpeedMin = 5f;
    public float starRotationSpeedMax = 25f;

    public float shootingStarRotationSpeedMin = 2f;
    public float shootingStarRotationSpeedMax = 6f;

    private float rotationSpeed;

    public bool isShootingStar;

    private void Start()
    {
        if (isShootingStar)
        {
            rotationSpeed = Random.Range(shootingStarRotationSpeedMin, shootingStarRotationSpeedMax);
            rotationSpeed *= 10f;
        }
        else
        {
            rotationSpeed = Random.Range(starRotationSpeedMin, starRotationSpeedMax);
            if (Random.value > 0.5f) rotationSpeed *= -1f;
        }
    }

    void Update()
    {
        this.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
