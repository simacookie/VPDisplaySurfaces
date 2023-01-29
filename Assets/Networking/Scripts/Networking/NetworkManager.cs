using RiptideNetworking;
using RiptideNetworking.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ids for recieved messages
public enum ClientToServerId : ushort {
    name = 1,
    input,
    resolution,
    mouseDown,
    mouseUp,
    clear,
    undo,
    red,
    blue,
    green,
    yellow
}

public class NetworkManager : MonoBehaviour
{
    //Making sure only one instance of this object exists
    private static NetworkManager _singelton;

    public static NetworkManager Singleton {
        get => _singelton;
        private set {
            if (_singelton == null) {
                _singelton = value;
            } else if (_singelton != value) {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public Server Server {get; private set;}

    //Port to use
    [SerializeField] private ushort port;
    //Amount of Clients that can connect
    [SerializeField] private ushort maxClientCount;

    //Set Instance
    private void Awake() {
        Singleton = this;
        Application.runInBackground = true;
    }

    private void Start() {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        //Create new Server
        Server = new Server();
        //Start Server
        Server.Start(port, maxClientCount);
        //Subscribe local funtion to Event from Riptide 
        Server.ClientDisconnected += DeviceLeft;
        Debug.Log("SERVER IS STARTED!");

    }

    private void FixedUpdate() {
        //Update Server
        Server.Tick();
    }

    //Stop Server when Application is quit
    private void OnApplicationQuit() {
        Server.Stop();
    }

    //Destroy local Prefab, when associated device disconnets
    private void DeviceLeft(object sender, ClientDisconnectedEventArgs e){
        Destroy(Device.list[e.Id].gameObject);
    }
}
