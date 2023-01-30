using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SimpleDrawCanvas.Presentation;


public class Device : MonoBehaviour
{
    int mouseUpCounter = 0;
    int mouseDownCounter = 0;
    static public float screenTouchX = 0;
    static public float screenTouchY = 0;
    static public bool isTouching = false;
    void Start()
    {
        ChangeDeviceSize(screenX, screenY);
        clearButton = GameObject.Find("Clear");
        undoButton = GameObject.Find("Undo");
        redButton = GameObject.Find("Red");
        blueButton = GameObject.Find("Blue");
        greenButton = GameObject.Find("Green");
        yellowButton = GameObject.Find("Yellow");
        clearButton.GetComponent<Button>().onClick.AddListener(() => clearPressed()); 
        undoButton.GetComponent<Button>().onClick.AddListener(() => undoPressed()); 
        redButton.GetComponent<Button>().onClick.AddListener(() => redPressed()); 
        blueButton.GetComponent<Button>().onClick.AddListener(() => bluePressed()); 
        greenButton.GetComponent<Button>().onClick.AddListener(() => greenPressed()); 
        yellowButton.GetComponent<Button>().onClick.AddListener(() => yellowPressed()); 

    }

	private void Update()
	{

        //Debug.Log(Device.screenTouchX);
        //Debug.Log(Device.screenTouchY);
	}

	//List for all Connected Devices
	public static Dictionary<ushort, Device> list = new Dictionary<ushort, Device>();

    //Id of device
    public ushort Id { get; private set; }

    //Name of device
    public string deviceName { get; private set; }

    //Make inputs accessible
    public DeviceInputs Inputs => Inputs;

    //Reference to Object that should be moved
    [SerializeField]
    private GameObject pointer;

    //Screen of the Object that can be scaled
    [SerializeField]
    private MeshFilter screen;
    [SerializeField]
    private Canvas canvasScreen;

    //First button that is synced with device
    [SerializeField]
    private GameObject clearButton;
    [SerializeField]
    private GameObject undoButton;
    [SerializeField]
    private GameObject redButton;
    [SerializeField]
    private GameObject blueButton;
    [SerializeField]
    private GameObject greenButton;
    [SerializeField]
    private GameObject yellowButton;
    //Reference to draw tool for clearing canvas etc.
    [SerializeField]
    private GameObject drawTool;

    static public UnityEvent<float[]> mouseDownEvent = new UnityEvent<float[]>();
    static public UnityEvent<float[]> mouseMoveEvent = new UnityEvent<float[]>();
    static public UnityEvent mouseUpEvent = new UnityEvent();

    public float screenX = 2726;

    public float screenY = 1824;

    //Remove device from list, if destroyed
    private void OnDestroy()
    {
        list.Remove(Id);
    }

    //Instantiates new Device with name and Id. Adds device to list.
    public static void Spawn(ushort id, string deviceName)
    {
        Device device = Instantiate(DeviceLogic.Singleton.DevicePrefab, DeviceLogic.Singleton.Parent.position, Quaternion.identity, DeviceLogic.Singleton.Parent).GetComponent<Device>();
        device.name = $"Device {id} ({(string.IsNullOrEmpty(deviceName) ? "UnnamedDevice" : deviceName)})";
        device.Id = id;
        device.deviceName = string.IsNullOrEmpty(deviceName) ? $"UnnamedDevice {id}" : deviceName;

        list.Add(id, device);
        
    }

    public void Move(float newPositionX, float newPositionZ, float sizeX, float sizeY)
    {
        //move canvas touch point


        //move pointer
        pointer.transform.localPosition = new Vector3((newPositionX - (sizeX / 2)) / 1000, pointer.transform.localPosition.y, (newPositionZ - (sizeY / 2)) / 1000);
    }

    public void ChangeDeviceSize(float newX, float newY)
    {

        //Canvas screen
        RectTransform canvasRectTransform = canvasScreen.GetComponent<RectTransform>();
        canvasRectTransform.sizeDelta = new Vector2(newX, newY);
        canvasRectTransform.transform.GetChild(1).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newY);
        canvasRectTransform.transform.GetChild(1).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newY);

        //screen.transform.localScale = new Vector3((newX/2)/105.5f, screen.transform.localScale.y, (newY/2)/105.5f);

        Debug.Log("DEVICE SIZE SHOULD BE CHANGED!");

        Mesh mesh = screen.mesh;
        Vector3[] vertices = mesh.vertices;

        //left side: 18, 17, 19, 16
        //right side: 21, 22, 20, 23

        //for (int i = 0; i < vertices.Length; i++) {
        // Debug.Log("Stelle " + i + ": " + vertices[i]);
        //}

        //Debug.Log(vertices[16]);
        //Debug.Log(vertices[17]);
        //Debug.Log(vertices[18]);
        //Debug.Log(vertices[19]);
        /*
                Default

                    //Left Face 
                    vertices[16] = new Vector3 ((-newX/2)/1000, vertices[16].y, (-newY/2)/1000);
                    vertices[17] = new Vector3 ((-newX/2)/1000, vertices[17].y, (-newY/2)/1000);
                    vertices[18] = new Vector3 ((-newX/2)/1000, vertices[18].y, (newY/2)/1000);
                    vertices[19] = new Vector3 ((-newX/2)/1000, vertices[19].y, (newY/2)/1000);

                    //Right Face
                    vertices[20] = new Vector3 ((newX/2)/1000, vertices[20].y, (newY/2)/1000);
                    vertices[21] = new Vector3 ((newX/2)/1000, vertices[21].y, (newY/2)/1000);
                    vertices[22] = new Vector3 ((newX/2)/1000, vertices[22].y, (-newY/2)/1000);
                    vertices[23] = new Vector3 ((newX/2)/1000, vertices[23].y, (-newY/2)/1000);

                    //Top Face
                    vertices[8] = new Vector3 ((newX/2)/1000, vertices[8].y, (-newY/2)/1000);
                    vertices[9] = new Vector3 ((-newX/2)/1000, vertices[9].y, (-newY/2)/1000);
                    vertices[10] = new Vector3 ((newX/2)/1000, vertices[10].y, (newY/2)/1000);
                    vertices[11] = new Vector3 ((-newX/2)/1000, vertices[11].y, (newY/2)/1000);

                    //Bottom Face
                    vertices[12] = new Vector3 ((newX/2)/1000, vertices[12].y, (newY/2)/1000);
                    vertices[13] = new Vector3 ((newX/2)/1000, vertices[13].y, (-newY/2)/1000);
                    vertices[14] = new Vector3 ((-newX/2)/1000, vertices[14].y, (-newY/2)/1000);
                    vertices[15] = new Vector3 ((-newX/2)/1000, vertices[15].y, (newY/2)/1000);

                    //Front Face
                    vertices[0] = new Vector3 ((newX/2)/1000, vertices[0].y, (-newY/2)/1000);
                    vertices[1] = new Vector3 ((-newX/2)/1000, vertices[1].y, (-newY/2)/1000);
                    vertices[2] = new Vector3 ((newX/2)/1000, vertices[2].y, (-newY/2)/1000);
                    vertices[3] = new Vector3 ((-newX/2)/1000, vertices[3].y, (-newY/2)/1000);

                    //Back Face
                    vertices[4] = new Vector3 ((newX/2)/1000, vertices[4].y, (newY/2)/1000);
                    vertices[5] = new Vector3 ((-newX/2)/1000, vertices[5].y, (newY/2)/1000);
                    vertices[6] = new Vector3 ((newX/2)/1000, vertices[6].y, (newY/2)/1000);
                    vertices[7] = new Vector3 ((-newX/2)/1000, vertices[7].y, (newY/2)/1000);

            */


            /*
                    Default

                    //Left Face
                    vertices[16] = new Vector3 ((newX/2)/1000, vertices[16].y, (newY/2)/1000);
                    vertices[17] = new Vector3 ((newX/2)/1000, vertices[17].y, (newY/2)/1000);
                    vertices[18] = new Vector3 ((newX/2)/1000, vertices[18].y, (-newY/2)/1000);
                    vertices[19] = new Vector3 ((newX/2)/1000, vertices[19].y, (-newY/2)/1000);

                    //Right Face
                    vertices[20] = new Vector3 ((-newX/2)/1000, vertices[20].y, (-newY/2)/1000);
                    vertices[21] = new Vector3 ((-newX/2)/1000, vertices[21].y, (-newY/2)/1000);
                    vertices[22] = new Vector3 ((-newX/2)/1000, vertices[22].y, (newY/2)/1000);
                    vertices[23] = new Vector3 ((-newX/2)/1000, vertices[23].y, (newY/2)/1000);

                    //Top Face
                    vertices[8] = new Vector3 ((-newX/2)/1000, vertices[8].y, (newY/2)/1000);
                    vertices[9] = new Vector3 ((newX/2)/1000, vertices[9].y, (newY/2)/1000);
                    vertices[10] = new Vector3 ((-newX/2)/1000, vertices[10].y, (-newY/2)/1000);
                    vertices[11] = new Vector3 ((newX/2)/1000, vertices[11].y, (-newY/2)/1000);

                    //Bottom Face
                    vertices[12] = new Vector3 ((-newX/2)/1000, vertices[12].y, (-newY/2)/1000);
                    vertices[13] = new Vector3 ((-newX/2)/1000, vertices[13].y, (newY/2)/1000);
                    vertices[14] = new Vector3 ((newX/2)/1000, vertices[14].y, (newY/2)/1000);
                    vertices[15] = new Vector3 ((newX/2)/1000, vertices[15].y, (-newY/2)/1000);

                    //Front Face
                    vertices[0] = new Vector3 ((-newX/2)/1000, vertices[0].y, (newY/2)/1000);
                    vertices[1] = new Vector3 ((newX/2)/1000, vertices[1].y, (newY/2)/1000);
                    vertices[2] = new Vector3 ((-newX/2)/1000, vertices[2].y, (newY/2)/1000);
                    vertices[3] = new Vector3 ((newX/2)/1000, vertices[3].y, (newY/2)/1000);

                    //Back Face
                    vertices[4] = new Vector3 ((-newX/2)/1000, vertices[4].y, (-newY/2)/1000);
                    vertices[5] = new Vector3 ((newX/2)/1000, vertices[5].y, (-newY/2)/1000);
                    vertices[6] = new Vector3 ((-newX/2)/1000, vertices[6].y, (-newY/2)/1000);
                    vertices[7] = new Vector3 ((newX/2)/1000, vertices[7].y, (-newY/2)/1000);

            */

        //Blender

        //Right Face
        vertices[0] = new Vector3((newX / 2) / 1000, vertices[0].y, (-newY / 2) / 1000);
        vertices[1] = new Vector3((newX / 2) / 1000, vertices[1].y, (-newY / 2) / 1000);
        vertices[2] = new Vector3((newX / 2) / 1000, vertices[2].y, (newY / 2) / 1000);
        vertices[3] = new Vector3((newX / 2) / 1000, vertices[3].y, (newY / 2) / 1000);

        //Top Face
        vertices[4] = new Vector3((newX / 2) / 1000, vertices[4].y, (-newY / 2) / 1000);
        vertices[5] = new Vector3((-newX / 2) / 1000, vertices[5].y, (-newY / 2) / 1000);
        vertices[6] = new Vector3((-newX / 2) / 1000, vertices[6].y, (newY / 2) / 1000);
        vertices[7] = new Vector3((newX / 2) / 1000, vertices[7].y, (newY / 2) / 1000);

        //Left Face
        vertices[8] = new Vector3((-newX / 2) / 1000, vertices[8].y, (-newY / 2) / 1000);
        vertices[9] = new Vector3((-newX / 2) / 1000, vertices[9].y, (-newY / 2) / 1000);
        vertices[10] = new Vector3((-newX / 2) / 1000, vertices[10].y, (newY / 2) / 1000);
        vertices[11] = new Vector3((-newX / 2) / 1000, vertices[11].y, (newY / 2) / 1000);

        //Bottom Face
        vertices[12] = new Vector3((-newX / 2) / 1000, vertices[12].y, (-newY / 2) / 1000);
        vertices[13] = new Vector3((newX / 2) / 1000, vertices[13].y, (-newY / 2) / 1000);
        vertices[14] = new Vector3((newX / 2) / 1000, vertices[14].y, (newY / 2) / 1000);
        vertices[15] = new Vector3((-newX / 2) / 1000, vertices[15].y, (newY / 2) / 1000);

        //Back Face
        vertices[16] = new Vector3((newX / 2) / 1000, vertices[16].y, (-newY / 2) / 1000);
        vertices[17] = new Vector3((newX / 2) / 1000, vertices[17].y, (-newY / 2) / 1000);
        vertices[18] = new Vector3((-newX / 2) / 1000, vertices[18].y, (-newY / 2) / 1000);
        vertices[19] = new Vector3((-newX / 2) / 1000, vertices[19].y, (-newY / 2) / 1000);

        //Front Face
        vertices[20] = new Vector3((-newX / 2) / 1000, vertices[20].y, (newY / 2) / 1000);
        vertices[21] = new Vector3((-newX / 2) / 1000, vertices[21].y, (newY / 2) / 1000);
        vertices[22] = new Vector3((newX / 2) / 1000, vertices[22].y, (newY / 2) / 1000);
        vertices[23] = new Vector3((newX / 2) / 1000, vertices[23].y, (newY / 2) / 1000);

        //vertices[20] = new Vector3 (vertices[20].x, vertices[20].y, (newX/2)/1000);
        //vertices[21] = new Vector3 (vertices[21].x, vertices[21].y, (newX/2)/1000);
        //vertices[22] = new Vector3 (vertices[22].x, vertices[22].y, (newX/2)/1000);
        //vertices[23] = new Vector3 (vertices[23].x, vertices[23].y, (newX/2)/1000);

        //Debug.Log(vertices[16]);
        //Debug.Log(vertices[17]);
        //Debug.Log(vertices[18]);
        //Debug.Log(vertices[19]);

        mesh.vertices = vertices;
        mesh.RecalculateNormals();

        //for (int i = 0; i < mesh.normals.Length; i++) {
        //    mesh.normals[i] = -1 * mesh.normals[i];
        //}

        /*

        int[] tris = mesh.GetTriangles(0);
        for (int j = 0; j < tris.Length; j +=3) {
            int temp = tris[j];
            tris[j] = tris[j+1];
            tris[j+1] = temp;
        }
        mesh.SetTriangles(tris, 0);

        */
    }

    //Handles recieved Messages with id for name
    [MessageHandler((ushort)ClientToServerId.name)]

    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    [MessageHandler((ushort)ClientToServerId.input)]

    private static void Input(ushort fromClientId, Message message)
    {
        float[] inputs = message.GetFloats(2);
        Device.list[fromClientId].Move(inputs[0], inputs[1], Device.list[fromClientId].screenX, Device.list[fromClientId].screenY);
        screenTouchX = inputs[0];
        screenTouchY = inputs[1];
        mouseMoveEvent.Invoke(inputs);
        //Debug.Log(inputs[0] + " " + inputs[1]);
    }

    [MessageHandler((ushort)ClientToServerId.resolution)]
    private static void Resolution(ushort fromClientId, Message message)
    {
        float[] resolution = message.GetFloats(2);
        Debug.Log(resolution[0] + " " + resolution[1] + " ist die aktuelle Aufloesung");
        Device.list[fromClientId].screenX = resolution[0];
        Device.list[fromClientId].screenY = resolution[1];
        Device.list[fromClientId].ChangeDeviceSize(resolution[0], resolution[1]);

    }
    [MessageHandler((ushort)ClientToServerId.mouseDown)]

    private static void MouseDown(ushort fromClientId, Message message)
    {
        Debug.Log("MOUSE DOWN");
        float[] clickPosition = message.GetFloats(2);
        mouseDownEvent.Invoke(clickPosition);

    }
    [MessageHandler((ushort)ClientToServerId.mouseUp)]
    
    private static void MouseUp(ushort fromClientId, Message message)
    {
        Debug.Log("MOUSE UP");
        float[] clickPosition = message.GetFloats(2);
        mouseUpEvent.Invoke();
    }

    [MessageHandler((ushort)ClientToServerId.clear)]
    //Simulates Button press to sync interaction from device with virtual reality
    private static void clearButtonClick(ushort fromClientId, Message message){
        Button bt = Device.list[fromClientId].clearButton.GetComponent<Button>();
        //bt.onClick.Invoke();

        ExecuteEvents.Execute(bt.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);

        string content = message.GetString();
        Debug.Log(content);
    }
    [MessageHandler((ushort)ClientToServerId.undo)]
    //Simulates Button press to sync interaction from device with virtual reality
    private static void undoButtonClick(ushort fromClientId, Message message){
        Button bt = Device.list[fromClientId].undoButton.GetComponent<Button>();
        //bt.onClick.Invoke();

        ExecuteEvents.Execute(bt.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);

        string content = message.GetString();
        Debug.Log(content);
    }
    [MessageHandler((ushort)ClientToServerId.red)]
    //Simulates Button press to sync interaction from device with virtual reality
    private static void redButtonClick(ushort fromClientId, Message message){
        Button bt = Device.list[fromClientId].redButton.GetComponent<Button>();
        //bt.onClick.Invoke();

        ExecuteEvents.Execute(bt.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);

        string content = message.GetString();
        Debug.Log(content);
    }
    [MessageHandler((ushort)ClientToServerId.green)]
    //Simulates Button press to sync interaction from device with virtual reality
    private static void greenButtonClick(ushort fromClientId, Message message){
        Button bt = Device.list[fromClientId].greenButton.GetComponent<Button>();
        //bt.onClick.Invoke();

        ExecuteEvents.Execute(bt.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);

        string content = message.GetString();
        Debug.Log(content);
    }
    [MessageHandler((ushort)ClientToServerId.blue)]
    //Simulates Button press to sync interaction from device with virtual reality
    private static void blueButtonClick(ushort fromClientId, Message message){
        Button bt = Device.list[fromClientId].blueButton.GetComponent<Button>();
        //bt.onClick.Invoke();

        ExecuteEvents.Execute(bt.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);

        string content = message.GetString();
        Debug.Log(content);
    }
    [MessageHandler((ushort)ClientToServerId.yellow)]
    //Simulates Button press to sync interaction from device with virtual reality
    private static void yellowButtonClick(ushort fromClientId, Message message){
        Button bt = Device.list[fromClientId].yellowButton.GetComponent<Button>();
        //bt.onClick.Invoke();

        ExecuteEvents.Execute(bt.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);

        string content = message.GetString();
        Debug.Log(content);
    }

    //Is assigned to a button
    public void clearPressed (){
        Debug.Log("clear");
        drawTool.GetComponent<Draw>().Clear();
    }
    //Is assigned to a button
    public void undoPressed(){
        Debug.Log("undo");
        drawTool.GetComponent<Draw>().Undo();
    }
    //Is assigned to a button
    public void redPressed(){
        Debug.Log("red");
        drawTool.GetComponent<Draw>().SetColor(Color.red);
    }
    //Is assigned to a button
    public void greenPressed(){
        Debug.Log("green");
        drawTool.GetComponent<Draw>().SetColor(Color.green);
    }
    //Is assigned to a button
    public void bluePressed(){
        Debug.Log("blue");
        drawTool.GetComponent<Draw>().SetColor(Color.blue);
    }
    //Is assigned to a button
    public void yellowPressed(){
        Debug.Log("yellow");
        drawTool.GetComponent<Draw>().SetColor(Color.yellow);
    }

}
