using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletCalibration : MonoBehaviour
{
    [SerializeField]
    Vector3 topLeftPos;
    [SerializeField]
    Vector3 bottomLeftPos;
    [SerializeField]
    Vector3 bottomRightPos;
    public Transform rightIndexTipPos;
    public Transform controllerLocator;
    Oculus.Interaction.HandVisual handVisual;


    [SerializeField]
    private bool sPressed = false;
    [SerializeField]
    private bool dPressed = false;
    [SerializeField]
    private bool fPressed = false;

    //DEBUGING!
    [SerializeField]
    private bool isPlane;
	private void Start()
	{
        handVisual = GameObject.Find("RightHandVisual").GetComponent<Oculus.Interaction.HandVisual>();
	}
	// Update is called once per frame
	void Update()
    {

        //Debug.Log("Running!");

        if (Input.GetKeyDown("s") && !sPressed)
        {
            Debug.Log("S was pressed. Bottom right point is set.");
            if (handVisual.IsVisible) bottomRightPos = rightIndexTipPos.position;
            else bottomRightPos = controllerLocator.position;
            sPressed = true;
        }
        if (Input.GetKeyDown("d") && sPressed && !dPressed)
        {
            Debug.Log("D was pressed. Bottom left point is set.");
            if(handVisual.IsVisible) bottomLeftPos = rightIndexTipPos.position;
            else bottomLeftPos = controllerLocator.position;
            dPressed = true;
        }
        if (Input.GetKeyDown("f") && sPressed && dPressed && !fPressed)
        {
            Debug.Log("F was pressed. Top left point is set.");
            Debug.Log("Parent objects Orientation should be set.");
            if (handVisual.IsVisible) topLeftPos = rightIndexTipPos.position;
            else topLeftPos = controllerLocator.position;

            Plane test = new Plane(bottomRightPos, bottomLeftPos, topLeftPos);

            //Calculate middle point
            Vector3 middle = new Vector3 ((bottomRightPos.x + topLeftPos.x)/2, (bottomRightPos.y + topLeftPos.y)/2, (bottomRightPos.z + topLeftPos.z)/2);


            //transform.right = (bottomRightPos - bottomLeftPos);
            Quaternion rotation = Quaternion.LookRotation((bottomLeftPos - topLeftPos), test.normal);

            this.transform.rotation = rotation;
            //this.transform.localEulerAngles = this.transform.localEulerAngles + new Vector3(0, 0, 180);
            this.transform.position = middle;

            sPressed = false;
            dPressed = false;
            fPressed = false;

            //Debugging
            if (isPlane) {
                
            }

        }

        Debug.DrawLine(bottomRightPos, bottomLeftPos);
        Debug.DrawLine(bottomLeftPos, topLeftPos);

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("Abort Calibration!");
            sPressed = false;
            dPressed = false;
            fPressed = false;
        }
    }
}
