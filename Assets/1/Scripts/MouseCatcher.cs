using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCatcher : MonoBehaviour
{
    public float rayDistance = 100f;

    private StarSpawn starSpawn;

    void Start()
    {
        starSpawn = GetComponent<StarSpawn>();
    }

    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            Debug.Log("hit object");
            GameObject hitObject = hit.collider.transform.root.gameObject;

            if (starSpawn.spawnedStars.Contains(hitObject))
            {
                Debug.Log("hit star");
                starSpawn.RemoveStar(hitObject);
                starSpawn.MoveStar(hitObject);
            }
        }
    }
}
