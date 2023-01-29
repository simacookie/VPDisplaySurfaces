using SimpleDrawCanvas.Presentation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : SimpleDrawingTool
{
    public Device device;
	private void Start()
	{
        SetColor(Color.red);
        SetBrushSize(100);

        device = transform.parent.parent.GetComponent<Device>();
        transform.parent.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        transform.parent.GetComponent<RectTransform>().localScale = new Vector3(.001f, .001f, .001f);
        transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3();
        transform.parent.GetComponent<RectTransform>().localEulerAngles = new Vector3(90,0,180);
        //transform.parent.GetComponent<RectTransform>().localRotation = new Quaternion(90,0,0,0);
        Device.mouseDownEvent.AddListener((clickPosition) => {
            // Canvas has (0,0) at the center isntead of bottem left corner, offset by half screen

            StartDraw(new Vector3(clickPosition[0] - .5f * device.screenX, clickPosition[1] - .5f * device.screenY));
            MoveDraw(new Vector3(clickPosition[0] - .5f * device.screenX, clickPosition[1] - .5f * device.screenY-1));

        });
        Device.mouseMoveEvent.AddListener((clickPosition) => {
            // Canvas has (0,0) at the center isntead of bottem left corner, offset by half screen

            MoveDraw(new Vector3(clickPosition[0] - .5f * device.screenX, clickPosition[1] - .5f * device.screenY));
        });
        // start drawing on receive mousedown
        Device.mouseUpEvent.AddListener(() => {
            EndDraw();
        });
    }
    void Update()
	{
        // start drawing on receive mousedown

        if (Input.GetKeyDown("c"))
            Clear();
        if (Input.GetKeyDown("1"))
            SetBrushSize(1);
        if (Input.GetKeyDown("2"))
            SetBrushSize(2);
        if (Input.GetKeyDown("3"))
            SetBrushSize(3);
        if (Input.GetKeyDown("4"))
            SetBrushSize(4);
        if (Input.GetKeyDown("5"))
            SetBrushSize(5);        
        if (Input.GetKeyDown("6"))
            SetBrushSize(6);
        if (Input.GetKeyDown("7"))
            SetBrushSize(7);
        if (Input.GetKeyDown("8"))
            SetBrushSize(8);
        if (Input.GetKeyDown("9"))
            SetBrushSize(9);

        // Canvas has (0,0) at the center isntead of bottem left corner, offset by half screen
        //Debug.Log(Device.screenTouchX);
        //Debug.Log(Device.screenTouchY);
        //Debug.Log(screenTouchXToCenterAnchor);
        //Debug.Log(screenTouchYToCenterAnchor);


        
	}
}
