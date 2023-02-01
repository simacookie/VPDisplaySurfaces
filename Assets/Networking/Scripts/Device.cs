using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SimpleDrawCanvas.Presentation;
using System.Collections;
using System.IO;

//States for the UI
public enum UIState
{
    start,
    trace,
    fill,
    ladybug,
    house,
    finished
}

public class Device : MonoBehaviour
{
    //Initial state
    UIState state = UIState.start;

    //Variable that keeps track if the timer is currently running or not
    private bool timerRunning = false;

    //Time counted
    [SerializeField]
    private float timer = 0;

    //Keeps track how many times a .txt has been created
    private int timesSaved = 0;

    //Name for the .txt
    [SerializeField]
    private string file = "timers";

    //List that keeps track of multiple stopped times
    [SerializeField]
    private List<float> timers;

    //First task background
    public GameObject traceEmoji;

    //Second task background
    public GameObject fillFlower;

    //Third task background
    public GameObject ladybug;

    //Fourth task background
    public GameObject house;
    
    //Counter for mouseUp
    int mouseUpCounter = 0;

    //Counter for mouseDown
    int mouseDownCounter = 0;

    //Coordinate for XValue
    static public float screenTouchX = 0;

    //Coordinate for YValue
    static public float screenTouchY = 0;

    //Shows if screen is currently beeing touched on
    static public bool isTouching = false;

    //For access to the button text
    [SerializeField]
    private GameObject previousButtonText;

    //For access to the button text
    [SerializeField]
    private GameObject nextButtonText;

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

    //Canvas for the UI
    [SerializeField]
    private Canvas canvasScreen;

    //Buttons that are synced with the connected device
    private GameObject clearButton;
    private GameObject smolButton;
    private GameObject middleButton;
    private GameObject bigButton;
    private GameObject previousButton;
    private GameObject nextButton;

    //Reference to draw tool for clearing canvas etc.
    [SerializeField]
    private GameObject drawTool;

    //Events for custom Inputs
    static public UnityEvent<float[]> mouseDownEvent = new UnityEvent<float[]>();
    static public UnityEvent<float[]> mouseMoveEvent = new UnityEvent<float[]>();
    static public UnityEvent mouseUpEvent = new UnityEvent();

    //Screen size X
    public float screenX = 2726;

    //Screen size Y
    public float screenY = 1824;


    void Start()
    {
        //Change the UI Size
        ChangeDeviceSize(screenX, screenY);

        //Fin all buttons and map them to the correct function
        clearButton = GameObject.Find("Clear");
        smolButton = GameObject.Find("smol");
        middleButton = GameObject.Find("middle");
        bigButton = GameObject.Find("BIG");
        previousButton = GameObject.Find("Previous");
        nextButton = GameObject.Find("Next");
        clearButton.GetComponent<Button>().onClick.AddListener(() => clearPressed());
        smolButton.GetComponent<Button>().onClick.AddListener(() => smolPressed()); 
        middleButton.GetComponent<Button>().onClick.AddListener(() => middlePressed()); 
        bigButton.GetComponent<Button>().onClick.AddListener(() => bigPressed()); 
        previousButton.GetComponent<Button>().onClick.AddListener(() => previousPressed());  
        nextButton.GetComponent<Button>().onClick.AddListener(() => nextPressed()); 

        //Deactivate the previousButton at the start
        previousButton.SetActive(false);

    }

	private void Update()
	{

        //Timer
        if (timerRunning) {
            timer = timer + 1 * Time.deltaTime;
        }

        //Reset the UI and save the current timers
        if (UnityEngine.Input.GetKeyDown(KeyCode.Return)) {
            state = UIState.start;
            fillFlower.SetActive(false);
            traceEmoji.SetActive(false);
            ladybug.SetActive(false);
            house.SetActive(false);
            drawTool.GetComponent<Draw>().Clear();
            previousButton.SetActive(false);
            nextButtonText.GetComponent<Text>().text = "Start";
            nextButton.SetActive(true);
            Save(timers);
        }
	}

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

        mesh.vertices = vertices;
        mesh.RecalculateNormals();

    }

    //Handles recieved Messages with id for name
    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    //Handles input massages
    [MessageHandler((ushort)ClientToServerId.input)]
    private static void Input(ushort fromClientId, Message message)
    {
        float[] inputs = message.GetFloats(2);
        Device.list[fromClientId].Move(inputs[0], inputs[1], Device.list[fromClientId].screenX, Device.list[fromClientId].screenY);
        screenTouchX = inputs[0];
        screenTouchY = inputs[1];
        mouseMoveEvent.Invoke(inputs);
    }

    //Handles resolution messages
    [MessageHandler((ushort)ClientToServerId.resolution)]
    private static void Resolution(ushort fromClientId, Message message)
    {
        float[] resolution = message.GetFloats(2);
        Debug.Log(resolution[0] + " " + resolution[1] + " ist die aktuelle Aufloesung");
        Device.list[fromClientId].screenX = resolution[0];
        Device.list[fromClientId].screenY = resolution[1];
        Device.list[fromClientId].ChangeDeviceSize(resolution[0], resolution[1]);

    }

    //Handles mouseDown messages
    [MessageHandler((ushort)ClientToServerId.mouseDown)]
    private static void MouseDown(ushort fromClientId, Message message)
    {
        Debug.Log("MOUSE DOWN");
        float[] clickPosition = message.GetFloats(2);
        mouseDownEvent.Invoke(clickPosition);

    }

    //Handles mouseUp messages
    [MessageHandler((ushort)ClientToServerId.mouseUp)]  
    private static void MouseUp(ushort fromClientId, Message message)
    {
        Debug.Log("MOUSE UP");
        float[] clickPosition = message.GetFloats(2);
        mouseUpEvent.Invoke();
    }

    //Handles the inputs from the clear button
    [MessageHandler((ushort)ClientToServerId.clear)]
    //Simulates Button press to sync interaction from device with virtual reality
    private static void clearButtonClick(ushort fromClientId, Message message){
        Button bt = Device.list[fromClientId].clearButton.GetComponent<Button>();
        ExecuteEvents.Execute(bt.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    //Handles the inputs from smol button
    [MessageHandler((ushort)ClientToServerId.smol)]
    //Simulates Button press to sync interaction from device with virtual reality
    private static void smolButtonClick(ushort fromClientId, Message message){
        Button bt = Device.list[fromClientId].smolButton.GetComponent<Button>();
        ExecuteEvents.Execute(bt.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    //Handles the inputs from middle button
    [MessageHandler((ushort)ClientToServerId.middle)]
    private static void middleButtonClick(ushort fromClientId, Message message){
        Button bt = Device.list[fromClientId].middleButton.GetComponent<Button>();
        ExecuteEvents.Execute(bt.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    //Handles the inputs from big button
    [MessageHandler((ushort)ClientToServerId.big)]
    private static void bigButtonClick(ushort fromClientId, Message message){
        Button bt = Device.list[fromClientId].bigButton.GetComponent<Button>();
        ExecuteEvents.Execute(bt.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    //Handles the inputs from previous button
    [MessageHandler((ushort)ClientToServerId.previous)]
    private static void previousButtonClick(ushort fromClientId, Message message){
        Button bt = Device.list[fromClientId].previousButton.GetComponent<Button>();
        ExecuteEvents.Execute(bt.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    //Handles the inputs from next button
    [MessageHandler((ushort)ClientToServerId.next)]
    private static void nextButtonClick(ushort fromClientId, Message message){
        Button bt = Device.list[fromClientId].nextButton.GetComponent<Button>();
        ExecuteEvents.Execute(bt.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }
    
    //Is assigned to button clear
    public void clearPressed (){
        Debug.Log("clear");
        drawTool.GetComponent<Draw>().Clear();
    }

    //Is assigned to button smol
    public void smolPressed () {
        Debug.Log("smol");
        drawTool.GetComponent<Draw>().SetBrushSize(70);
    }

    //Is assigned to button middle
    public void middlePressed () {
        Debug.Log("middle");
        drawTool.GetComponent<Draw>().SetBrushSize(100);
    }

    //Is assigned to button big
    public void bigPressed () {
        Debug.Log("big");
        drawTool.GetComponent<Draw>().SetBrushSize(150);
    }

    //Is assigned to button previous. Handles the stats of the UI.
    public void previousPressed () {
        switch (state)
        {
            case UIState.start:
                break;
            case UIState.trace:
                state = UIState.start;
                fillFlower.SetActive(false);
                traceEmoji.SetActive(false);
                ladybug.SetActive(false);
                house.SetActive(false);
                drawTool.GetComponent<Draw>().Clear();
                this.previousButton.SetActive(false);
                nextButtonText.GetComponent<Text>().text = "Start";
                break;
            case UIState.fill:
                state = UIState.trace;
                fillFlower.SetActive(false);
                traceEmoji.SetActive(true);
                ladybug.SetActive(false);
                house.SetActive(false);
                drawTool.GetComponent<Draw>().Clear();
                break;
            case UIState.ladybug:
                state = UIState.fill;
                fillFlower.SetActive(true);
                traceEmoji.SetActive(false);
                ladybug.SetActive(false);
                house.SetActive(false);
                drawTool.GetComponent<Draw>().Clear();
                break;
            case UIState.house:
                state = UIState.ladybug;
                fillFlower.SetActive(false);
                traceEmoji.SetActive(false);
                ladybug.SetActive(true);
                house.SetActive(false);
                drawTool.GetComponent<Draw>().Clear();
                nextButtonText.GetComponent<Text>().text = "▶";
                break;
            case UIState.finished:
                state = UIState.house;
                fillFlower.SetActive(false);
                traceEmoji.SetActive(false);
                ladybug.SetActive(false);
                house.SetActive(true);
                drawTool.GetComponent<Draw>().Clear();
                nextButton.SetActive(true);
                break;
            default:
                break;
        }
    }

    //Is assigned to button next. handles the states for the UI.
    public void nextPressed () {
        switch (state)
        {
            case UIState.start:
                traceEmoji.SetActive(true);
                fillFlower.SetActive(false);
                ladybug.SetActive(false);
                house.SetActive(false);
                drawTool.GetComponent<Draw>().Clear();
                state = UIState.trace;
                previousButton.SetActive(true);
                nextButtonText.GetComponent<Text>().text = "▶";
                Debug.Log("start");
                if (!timerRunning) {
                    timerRunning = true;
                } else {
                    Debug.Log("Already running!");
                }
                break;
            case UIState.trace:
                traceEmoji.SetActive(false);
                fillFlower.SetActive(true);
                ladybug.SetActive(false);
                house.SetActive(false);
                drawTool.GetComponent<Draw>().Clear();
                state = UIState.fill;
                break;
            case UIState.fill:
                state = UIState.ladybug;
                traceEmoji.SetActive(false);
                fillFlower.SetActive(false);
                ladybug.SetActive(true);
                house.SetActive(false);
                drawTool.GetComponent<Draw>().Clear();
                break;
            case UIState.ladybug:
                state = UIState.house;
                traceEmoji.SetActive(false);
                fillFlower.SetActive(false);
                ladybug.SetActive(false);
                house.SetActive(true);
                drawTool.GetComponent<Draw>().Clear();
                nextButtonText.GetComponent<Text>().text = "End";
                break;
            case UIState.house:
                state = UIState.finished;
                traceEmoji.SetActive(false);
                fillFlower.SetActive(false);
                ladybug.SetActive(false);
                house.SetActive(false);
                nextButton.SetActive(false);
                Debug.Log("stop");
                if (timerRunning) {
                    timerRunning = false;
                    timers.Add(timer);
                    timer = 0;
                } else {
                    Debug.Log("No timer running!");
                }
                break;
            case UIState.finished:
                break;
            default:
                break; 
        }

    }

    //Is called when the timers list needs to be written into a file.
    private void Save(List<float> timersToSave) {
        string json = "";
        if (timersToSave.Count > 0) {
            for (int i = 0; i < timersToSave.Count; i++) {
            json = json + "Timer " + i + ": " + timersToSave[i] + System.Environment.NewLine;
            }
            timesSaved++;
            string fileName = file + "" + timesSaved + ".txt";
            WriteToFile(fileName, json);
            timersToSave.Clear();
        }
    }

    private void WriteToFile(string fileName, string json){
        string path = GetFilePath(fileName);
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using(StreamWriter writer = new StreamWriter(fileStream)){
            writer.Write(json);
        }
    }

    private string GetFilePath(string fileName) {
        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/" + fileName;
    }

}

