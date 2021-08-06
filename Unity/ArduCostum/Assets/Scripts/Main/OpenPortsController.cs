using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OpenPortsController : MonoBehaviour
{
    public GameObject[] points = new GameObject[2];
    public bool Open = false;
    public Toggle[] checkers = new Toggle[3];
    public void SetOpen(bool p)
    {
        Open = p;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Open)
        {
            //foreach(var i in points)
            //{
            //    if()
            //    {

            //    }
            //    i.GetComponent<Show>().OpenPort(true);
            //}

            for(int i =0;i<points.Length;i++)
            {
                if(checkers[i].isOn)
                {
                    points[i].GetComponent<Show>().OpenPort(true);
                }
            }
        }
        else
        {
            foreach (var i in points)
            {
                i.GetComponent<Show>().OpenPort(false);
            }
        }
    }
}
