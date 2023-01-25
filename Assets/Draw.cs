using SimpleDrawCanvas.Presentation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : SimpleDrawingTool
{
    public Device device;
	private void Start()
	{
        StartDraw(new Vector3());
        device = transform.parent.parent.GetComponent<Device>();
        transform.parent.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        transform.parent.GetComponent<RectTransform>().localScale = new Vector3(.001f, .001f, .001f);
        transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3();
        transform.parent.GetComponent<RectTransform>().localEulerAngles = new Vector3(90,0,180);
//transform.parent.GetComponent<RectTransform>().localRotation = new Quaternion(90,0,0,0);

    }
    void Update()
	{



        // Canvas has (0,0) at the center isntead of bottem left corner
        float screenTouchXToCenterAnchor = Device.screenTouchX - .5f * device.screenX;
        float screenTouchYToCenterAnchor = Device.screenTouchY - .5f * device.screenY;

        MoveDraw(new Vector3(screenTouchXToCenterAnchor, screenTouchYToCenterAnchor));
        //Debug.Log(Device.screenTouchX);
        //Debug.Log(Device.screenTouchY);
        //Debug.Log(screenTouchXToCenterAnchor);
        //Debug.Log(screenTouchYToCenterAnchor);


        
	}
}
