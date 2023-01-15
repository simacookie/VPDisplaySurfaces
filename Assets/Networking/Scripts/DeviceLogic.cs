using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceLogic : MonoBehaviour
{  
    //Making sure only one instance of this object exists
    private static DeviceLogic _singelton;

    public static DeviceLogic Singleton {
        get => _singelton;
        private set {
            if (_singelton == null) {
                _singelton = value;
            } else if (_singelton != value) {
                Debug.Log($"{nameof(DeviceLogic)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    //Make prefab accessible
    public GameObject DevicePrefab => devicePrefab;

    public Transform Parent => parent;

    [Header("Prefabs")]
    [SerializeField] private GameObject devicePrefab;

    [SerializeField] private Transform parent;

    private void Awake() {
        Singleton = this;
    }
}
