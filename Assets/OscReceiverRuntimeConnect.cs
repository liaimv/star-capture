using UnityEngine;
using extOSC;

public class OscReceiverRuntimeConnect : MonoBehaviour
{
    [SerializeField] private OSCReceiver receiver;

    private void Reset()
    {
        receiver = GetComponent<OSCReceiver>();
    }

    private void Awake()
    {
        if (!receiver) receiver = GetComponent<OSCReceiver>();
        if (!receiver)
        {
            Debug.LogError("OscReceiverRuntimeConnect: No OSCReceiver found on this GameObject.");
            return;
        }

        // Ensure component is enabled, then bind the UDP socket.
        receiver.enabled = true;
        receiver.Connect();

        Debug.Log($"OscReceiverRuntimeConnect: Connected on {receiver.LocalHostMode} port {receiver.LocalPort}.");
    }

    private void OnApplicationQuit()
    {
        if (receiver) receiver.Close();
    }
}