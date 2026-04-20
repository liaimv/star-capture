using Unity.VisualScripting;
using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour
{
    public StarSpawn starSpawn;
    private void OnTriggerEnter(Collider other)
    {
        if (starSpawn.shootingStars.Contains(other.gameObject)) starSpawn.shootingStars.Remove(other.gameObject);

        Destroy(other.gameObject);
    }
}
