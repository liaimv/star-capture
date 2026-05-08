using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCatcher : MonoBehaviour
{
    public Camera interactionCamera;
    public float rayDistanceStar = 40f;
    public float rayDistanceShootingStar = 35f;

    public Vector2 CurrentPointerScreen { get; private set; }

    private StarSpawn starSpawn;

    [Header("Blob input")]
    public bool useBlob = true;
    [Range(0, 1)] public float blobX01;
    [Range(0, 1)] public float blobY01;
    public bool blobActive = true; // kept for BlobCursorUI and debugging

    [Header("Blob source (drag BlobCapture here)")]
    public BlobCaptureStar blobSource;

    [Header("Axis fixes")]
    public bool flipX = false;
    public bool flipY = true;
    public bool swapXY = false;

    [Header("Hover to trigger")]
    public float hoverSeconds = 0.2f;

    GameObject hoverObject;
    float hoverTime;

    void Start()
    {
        starSpawn = GetComponent<StarSpawn>();
    }

    float Norm(float v, float min, float max)
    {
        if (Mathf.Approximately(max, min)) return 0f;
        return Mathf.InverseLerp(min, max, v);
    }

    void Update()
    {
        if (!interactionCamera || starSpawn == null) return;
        if (useBlob && blobSource == null) return;

        // Read blob and normalize using BlobCaptureStar calibration
        if (useBlob)
        {
            blobX01 = Norm(blobSource.blobX, blobSource.xMin, blobSource.xMax);
            blobY01 = Norm(blobSource.blobY, blobSource.yMin, blobSource.yMax);
            if (blobSource.flipY) blobY01 = 1f - blobY01;

            blobActive = blobSource.active > 0.5f;
        }

        // Always drive pointer from blob when useBlob=true (no gating on active)
        Vector2 pointerScreen;
        if (useBlob)
        {
            float x = blobX01;
            float y = blobY01;

            if (swapXY) (x, y) = (y, x);
            if (flipX) x = 1f - x;
            if (flipY) y = 1f - y;

            pointerScreen = new Vector2(x * Screen.width, y * Screen.height);
        }
        else
        {
            pointerScreen = Mouse.current.position.ReadValue();
        }

        CurrentPointerScreen = pointerScreen;

        Ray ray = interactionCamera.ScreenPointToRay(pointerScreen);
        RaycastHit hit;

        GameObject candidate = null;

        // shooting star first
        if (Physics.Raycast(ray, out hit, rayDistanceShootingStar))
        {
            var root = hit.collider.transform.root.gameObject;
            if (starSpawn.shootingStars.Contains(root))
                candidate = root;
        }

        // normal star
        if (candidate == null && Physics.Raycast(ray, out hit, rayDistanceStar))
        {
            var root = hit.collider.transform.root.gameObject;
            if (starSpawn.spawnedStars.Contains(root))
                candidate = root;
        }

        // hover-to-trigger
        if (candidate == hoverObject && candidate != null)
        {
            hoverTime += Time.deltaTime;
            if (hoverTime >= hoverSeconds)
            {
                starSpawn.MoveSequence(candidate);
                hoverObject = null;
                hoverTime = 0f;
            }
        }
        else
        {
            hoverObject = candidate;
            hoverTime = 0f;
        }
    }
}