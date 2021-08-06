using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using System;


public class Show : MonoBehaviour
{
    public BodyParts part;
    public string port_name;
    [SerializeField] private bool OpenADoor;
    SerialPort port;
    float timer;
    Quaternion start;
    bool bstart;
    public void OpenPort(bool t)
    {
        OpenADoor = t;
    }

    public void Starting()
    {
        bstart = true;
    }
    Joints J;
    // Start is called before the first frame update
    void Start()
    {
        bstart = false;
        port = new SerialPort();
        port.PortName = port_name;//default COM3
        port.BaudRate = 115200;
        port.Parity = Parity.None;
        port.StopBits = StopBits.One;
        port.DataBits = 8;
        port.Handshake = Handshake.None;
        port.RtsEnable = true;
        J = new Joints(part);
    }

    // Update is called once per frame
    void Update()
    {
        if (OpenADoor)
        {
            if (!port.IsOpen)
                port.Open();
            port.Write("f");
            using (StreamWriter outputfile = new StreamWriter("D:/Programs/UnityProject/ArduCostum/Assets/Document/Data.txt", true))
            {
                timer += Time.deltaTime;
                print("Запись пошла!!!");
                var str = System.Text.RegularExpressions.Regex.Split(port.ReadLine(), "\\t");

                    transform.rotation = new Quaternion(float.Parse(str[0], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo),
                            -float.Parse(str[2], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo),
                            float.Parse(str[1], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo),
                            float.Parse(str[3], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo));
                 outputfile.WriteLine(port.ReadLine() + "\t" + timer);
            }
        }
        else
        {
            if (port.IsOpen)
            {
                port.Close();
                print(timer);
            }
        }
    }
}
