using extOSC;
using UnityEngine;

public class BlobReceiver : MonoBehaviour
{
    public OSCReceiver receiver;
    public float blobValue;

    void Start()
    {
        receiver.Bind("/blob", OnBlobReceived);
    }

    void OnBlobReceived(OSCMessage message)
    {
        blobValue = message.Values[0].FloatValue;
        Debug.Log("Blob Value: " + blobValue);
    }
}