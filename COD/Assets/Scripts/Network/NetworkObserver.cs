using System.Net.NetworkInformation;
using UnityEngine;

public class NetworkObserver : MonoBehaviour
{
    [SerializeField] private ModalControl networkDisconnectModal;

    void Awake()
    {
        NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;
    }

    private void OnNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
    {
        if (e.IsAvailable)
            OnNetworkConnect();

        else
            OnNetworkDisconnect();
    }

    private void OnNetworkConnect()
    {
        DebugConsole.LogSuccess("Network Found.");
        networkDisconnectModal.Close();
    }

    private void OnNetworkDisconnect()
    {
        DebugConsole.LogError("Network Not Found.");
        networkDisconnectModal.Open();
    }
}
