using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletCalibration : MonoBehaviour
{
    Vector3 topLeftPos;
    Vector3 bottomLeftPos;
    Vector3 bottomRightPos;
    public Transform rightIndexTipPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            bottomRightPos = rightIndexTipPos.position;
        }
        if (Input.GetKeyDown("d"))
        {
            bottomLeftPos = rightIndexTipPos.position;
        }
        if (Input.GetKeyDown("f"))
        {
            topLeftPos = rightIndexTipPos.position;
           
            Plane test = new Plane(bottomRightPos, bottomLeftPos, topLeftPos);
            //transform.right = (bottomRightPos - bottomLeftPos);
            Quaternion rotation = Quaternion.LookRotation((bottomLeftPos - topLeftPos), test.normal);
            transform.rotation = rotation;
            transform.position = topLeftPos + ((bottomRightPos - topLeftPos) / 2);

        }
    }
}
