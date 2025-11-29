using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();        
    }

    public void StartClient()
    {        
        NetworkManager.Singleton.StartClient();        
    }

    public void Stop()
    {
        NetworkManager.Singleton.Shutdown();       
    }
}
