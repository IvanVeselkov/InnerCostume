using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostulBody : MonoBehaviour
{
    public Transform part;
    public bool first;

    public void SetCalib(bool b)
    {
        first = b;
    }
    // Start is called before the first frame update
    void Start()
    {
        first = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(first==true)
        {
            Vector3 y = new Vector3(0f, part.rotation.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Euler(y);
            first = false;
        }
    }
}
