using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootingStarShoot : MonoBehaviour
{
    [Header("Shooting Star Transform Variables")]
    public float forceAmountMin = 10f;
    public float forceAmountMax = 20f;
    public float shootingStarRotationSpeedMin = 5f;
    public float shootingStarRotationSpeedMax = 10f;
    public float shootingStarStartPosXMin = 35f;
    public float shootingStarStartPosXMax = 60f;
    public float shootingStarStartPosYMin = -3.5f;
    public float shootingStarStartPosYMax = 0f;

    [Header("Shooting Star Colors")]
    private Dictionary<Color, Color> shootingStarColors = new Dictionary<Color, Color>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShootingStarMovement();
    }

    // Update is called once per frame
    void Update()
    {
        //Rotation
        float shootingStarRotationSpeed = Random.Range(shootingStarRotationSpeedMin, shootingStarRotationSpeedMax);
        shootingStarRotationSpeed *= 100;
        this.transform.Rotate(Vector3.forward * shootingStarRotationSpeed * Time.deltaTime);
    }

    public void ShootingStarMovement()
    {
        //Initialize start position
        float shootingStarStartPosX = Random.Range(shootingStarStartPosXMin, shootingStarStartPosXMax);
        float shootingStarStartPosY = Random.Range(shootingStarStartPosYMin, shootingStarStartPosYMax);

        Vector3 newPos = new Vector3(shootingStarStartPosX, shootingStarStartPosY, this.transform.position.z);


        //Shoot
        float forceAmount = Random.Range(forceAmountMin, forceAmountMax);
        Rigidbody rb = this.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.left * forceAmount, ForceMode.Impulse);
    }
}
