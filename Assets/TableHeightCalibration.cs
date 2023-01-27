using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableHeightCalibration : MonoBehaviour
{

    public Transform rightIndexTipPos;
    [SerializeField]
    public GameObject table;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
         startPosition = table.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            Debug.Log("t was pressed. Table height set to Hand.");
            table.transform.position = new Vector3 (table.transform.position.x, rightIndexTipPos.position.y, table.transform.position.z);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Table height reset");
            table.transform.position = startPosition;
        }
    }
}
