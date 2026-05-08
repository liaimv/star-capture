using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseCatcher : MonoBehaviour
{
    public Camera interactionCamera;
    public float rayDistanceStar = 40f;
    public float rayDistanceShootingStar = 35f;

    private StarSpawn starSpawn;

    void Start()
    {
        starSpawn = GetComponent<StarSpawn>();
    }
    void Update()
    {
        Vector2 mouse = Mouse.current.position.ReadValue();

        Ray ray = interactionCamera.ScreenPointToRay(mouse);
        RaycastHit hit;

        //Shooting Stars Ray
        Debug.DrawRay(ray.origin, ray.direction * rayDistanceShootingStar, Color.blue);

        if (Physics.Raycast(ray, out hit, rayDistanceShootingStar))
        {
            GameObject hitObject = hit.collider.transform.root.gameObject;

            if (starSpawn.shootingStars.Contains(hitObject))
            {
                starSpawn.MoveSequence(hitObject);
                return;
            }
        }

        //Normal Stars Ray
        Debug.DrawRay(ray.origin, ray.direction * rayDistanceStar, Color.red);

        if (Physics.Raycast(ray, out hit, rayDistanceStar))
        {
            GameObject hitObject = hit.collider.transform.root.gameObject;

            if (starSpawn.spawnedStars.Contains(hitObject))
            {
                starSpawn.MoveSequence(hitObject);
            }
        }
    }
}
