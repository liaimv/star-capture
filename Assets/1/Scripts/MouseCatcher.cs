using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseCatcher : MonoBehaviour
{
    public Camera interactionCamera;
    public float rayDistance = 100f;

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

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            GameObject hitObject = hit.collider.transform.root.gameObject;

            if (starSpawn.spawnedStars.Contains(hitObject))
            {
                starSpawn.RemoveStar(hitObject);
                starSpawn.MoveStar(hitObject);
            }
        }
    }
}
