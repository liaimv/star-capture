using UnityEngine;
using System.Collections.Generic;

public class BlobCaptureStar : MonoBehaviour
{
    [Header("OSC inputs (raw)")]
    public float blobX;
    public float blobY;
    public float active;

    [Header("Raw -> 0..1 calibration")]
    public float xMin = 0f, xMax = 1f;
    public float yMin = 0f, yMax = 1f;
    public bool flipY = true;

    [Header("Camera")]
    public Camera worldCamera;

    [Header("Stars live on this Z plane")]
    public float starsZ = -1.9f;

    [Header("Capture")]
    public bool requireActive = true;
    public float captureRadiusWorld = 2.0f;

    [Tooltip("Must hover inside the radius this long before the star disappears.")]
    public float hoverSeconds = 0.2f;

    [Tooltip("Set this to the Star layer only.")]
    public LayerMask starLayer;

    // Tracks how long each star has been hovered (inside radius)
    readonly Dictionary<GameObject, float> hoverTimers = new Dictionary<GameObject, float>(256);

    float Norm(float v, float min, float max)
    {
        if (Mathf.Approximately(max, min)) return 0f;
        return Mathf.InverseLerp(min, max, v);
    }

    void Update()
    {
        if (!worldCamera) return;
        if (requireActive && active < 0.5f) return;

        float nx = Norm(blobX, xMin, xMax);
        float ny = Norm(blobY, yMin, yMax);
        if (flipY) ny = 1f - ny;

        // Ray from camera through screen point
        Ray ray = worldCamera.ScreenPointToRay(new Vector3(nx * Screen.width, ny * Screen.height, 0f));

        // Intersect ray with plane z = starsZ
        float denom = ray.direction.z;
        if (Mathf.Abs(denom) < 0.0001f) return;

        float t = (starsZ - ray.origin.z) / denom;
        if (t <= 0f) return;

        Vector3 blobWorld = ray.origin + ray.direction * t;

        // Find stars currently inside the radius
        Collider[] hits = Physics.OverlapSphere(blobWorld, captureRadiusWorld, starLayer, QueryTriggerInteraction.Collide);

        // Mark current frame's hovered stars and update timers
        // Also remove stale dictionary entries for destroyed/disabled stars
        // (keeps memory stable)
        var hoveredThisFrame = new HashSet<GameObject>();

        for (int i = 0; i < hits.Length; i++)
        {
            var go = hits[i].transform.root.gameObject;
            if (!go.activeInHierarchy) continue;

            hoveredThisFrame.Add(go);

            hoverTimers.TryGetValue(go, out float time);
            time += Time.deltaTime;
            hoverTimers[go] = time;

            if (time >= hoverSeconds)
            {
                go.SetActive(false);
                hoverTimers.Remove(go);
            }
        }

        // Reset timers for stars no longer hovered
        // (copy keys to avoid modifying collection while iterating)
        if (hoverTimers.Count > 0)
        {
            var keys = ListPool<GameObject>.Get();
            keys.AddRange(hoverTimers.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                var k = keys[i];
                if (k == null || !k.activeInHierarchy || !hoveredThisFrame.Contains(k))
                    hoverTimers.Remove(k);
            }

            ListPool<GameObject>.Release(keys);
        }
    }

    // Tiny list pool to avoid GC spikes (optional but nice for installs)
    static class ListPool<T>
    {
        static readonly Stack<List<T>> pool = new Stack<List<T>>();

        public static List<T> Get()
        {
            if (pool.Count > 0)
            {
                var list = pool.Pop();
                list.Clear();
                return list;
            }
            return new List<T>(256);
        }

        public static void Release(List<T> list)
        {
            list.Clear();
            pool.Push(list);
        }
    }
}