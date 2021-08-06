using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using System;

public class Data
{
    public List<float> time = new List<float>();
    public List<float> x = new List<float>();
    public List<float> y = new List<float>();
    public List<float> z = new List<float>();
    public List<float> w = new List<float>();

    public (double[],double[],double[],double[],double[]) ToDouble()
    {
        double[] d_time = new double[time.Count];
        double[] d_x = new double[x.Count];
        double[] d_y = new double[y.Count];
        double[] d_z = new double[z.Count];
        double[] d_w = new double[w.Count];

        for(int i=0;i<time.Count;i++)
        {
            d_time[i] = (double)time[i];
        }

        for (int i = 0; i < x.Count; i++)
        {
            d_x[i] = (double)x[i];
        }

        for (int i = 0; i < y.Count; i++)
        {
            d_y[i] = (double)y[i];
        }

        for (int i = 0; i < z.Count; i++)
        {
            d_z[i] = (double)z[i];
        }

        try
        {
            for (int i = 0; i < w.Count; i++)
            {
                d_w[i] = (double)w[i];
            }
        }catch(NullReferenceException)
        {
            Debug.LogWarning("w is nullable");
        }

        return (d_time, d_x, d_y, d_z, d_w);
    }
}
public class VisualiseData : MonoBehaviour
{
    Data date = new Data();
    public Data GetData()
    {
        return date;
    }

    //For graph
    public Transform pointPrefab;
    [SerializeField] private int resolution;


    public string port_name;
    [SerializeField] private bool OpenADoor = false;
    [SerializeField] private bool CreateVisual = false;
    SerialPort port;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        port = new SerialPort();
        port.PortName = port_name;//default COM3
        port.BaudRate = 115200;
        port.Parity = Parity.None;
        port.StopBits = StopBits.One;
        port.DataBits = 8;
        port.Handshake = Handshake.None;
        port.RtsEnable = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (OpenADoor)
        {
            if (!port.IsOpen)
                port.Open();
            port.Write("f");
            using (StreamWriter outputfile = new StreamWriter("D:/Programs/UnityProject/ArduCostum/Assets/Document/Data.txt", true))
            {
                print("Запись пошла!!!");
                var str = System.Text.RegularExpressions.Regex.Split(port.ReadLine(), "\\t");
                timer += Time.deltaTime;

                ////Quaternion
                //date.x.Add(float.Parse(str[0], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo));
                //date.z.Add(float.Parse(str[2], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo));
                //date.y.Add(float.Parse(str[1], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo));
                //date.w.Add(float.Parse(str[3], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo));
                //date.time.Add(timer);

                //Accel
                date.x.Add(float.Parse(str[0], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo));
                date.z.Add(float.Parse(str[2], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo));
                date.y.Add(float.Parse(str[1], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo));
                date.time.Add(timer);

                ////Gyro
                //date.x.Add(float.Parse(str[0], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo));
                //date.z.Add(float.Parse(str[2], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo));
                //date.y.Add(float.Parse(str[1], System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo));
                //date.time.Add(timer);
            }
        }
        else
        {
            if (port.IsOpen)
            {
                port.Close();
            }
        }

        if(CreateVisual)
        {
            //double[] d_time = new double[date.time.Count];
            //double[] d_x = new double[date.x.Count];
            //double[] d_y = new double[date.y.Count];
            //double[] d_z = new double[date.z.Count];
            //double[] d_w = new double[date.w.Count];

            //(d_time, d_x, d_y, d_z, d_w) = date.ToDouble();
            ////picture x axis
            resolution = date.time.Count;

            float step = 2f / resolution;
            Vector3 scale = Vector3.one * step;
            Vector3 position;
            position.y = 0f;
            position.z = 0f;
            for (int i = 0; i < resolution; i++)
            {
                Transform point = Instantiate(pointPrefab);
                position.x = date.time[i];
                position.y = date.z[i];
                point.localPosition = position;
                point.localScale = scale;
                point.SetParent(transform, false);
            }
            CreateVisual = false;
        }
    }
}
