using UnityEngine;
using extOSC;

public class BlobDebugListener : MonoBehaviour
{
    [Header("extOSC")]
    public OSCReceiver receiver;

    [Header("Addresses")]
    public string xAddress = "/x";
    public string yAddress = "/y";
    public string activeAddress = "/active";

    [Header("Target script to drive")]
    public BlobCaptureStar capture;   // drag your BlobCapture GameObject here

    [Header("Debug")]
    public bool logMessages = false;

    void Awake()
    {
        Debug.Log("BlobDebugListener Awake (editor/build)");

        if (receiver == null) receiver = GetComponent<OSCReceiver>();
        if (receiver == null)
        {
            Debug.LogError("BlobDebugListener: No OSCReceiver found.");
            enabled = false;
            return;
        }

        receiver.Bind(xAddress, OnX);
        receiver.Bind(yAddress, OnY);
        receiver.Bind(activeAddress, OnActive);
    }

    private void OnX(OSCMessage message)
    {
        if (capture == null || message.Values.Count == 0) return;
        capture.blobX = message.Values[0].FloatValue;
        if (logMessages) Debug.Log($"OSC {message.Address} x={capture.blobX}");
    }

    private void OnY(OSCMessage message)
    {
        if (capture == null || message.Values.Count == 0) return;
        capture.blobY = message.Values[0].FloatValue;
        if (logMessages) Debug.Log($"OSC {message.Address} y={capture.blobY}");
    }

    private void OnActive(OSCMessage message)
    {
        if (capture == null || message.Values.Count == 0) return;
        capture.active = message.Values[0].FloatValue;
        if (logMessages) Debug.Log($"OSC {message.Address} active={capture.active}");
    }
    
}