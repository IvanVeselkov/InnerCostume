using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyParts
{
    brush,//левая кисть
    elbow,//левый локоть
    shoulder//левое плечо
}

public class Joints
{
    BodyParts part;

    Quaternion first_rotate;
    public Joints(BodyParts p)
    {
        part = p;
    }

    public Quaternion BlockControl(Quaternion actual_rotate)
    {
        Vector3 actual_euler = actual_rotate.eulerAngles;
        Vector3 correct_ratate = Vector3.zero;
        //if(part == BodyParts.shoulder)
        //{
        //    if(actual_euler.x<-15f)
        //    {
        //        correct_ratate.x = -15f;
        //    }
        //    if(actual_euler.x>20f)
        //    {
        //        correct_ratate.x = 20f;
        //    }
        //    if(actual_euler.x>=-15f && actual_euler.x<=20f )
        //    {
        //        correct_ratate.x = actual_euler.x;
        //    }

        //    if (actual_euler.y < -45f)
        //    {
        //        correct_ratate.y = -45f;
        //    }
        //    if (actual_euler.y > 130f)
        //    {
        //        correct_ratate.y = 130f;
        //    }
        //    if (actual_euler.y >= -45f && actual_euler.y <= 130f)
        //    {
        //        correct_ratate.y = actual_euler.y;
        //    }

        //    if (actual_euler.z < -90f)
        //    {
        //        correct_ratate.z = -90f;
        //    }
        //    if (actual_euler.z > 135f)
        //    {
        //        correct_ratate.z = 135f;
        //    }
        //    if (actual_euler.z >= -90f && actual_euler.z <= 135f)
        //    {
        //        correct_ratate.z = actual_euler.z;
        //    }
        //}

        //Quaternion ret = Quaternion.Euler(correct_ratate);
        Quaternion ret = actual_rotate;
        return ret;
    }

    public Quaternion Calibrate(Quaternion actual_rotate)
    {
        if(first_rotate==null)
        {
            first_rotate = actual_rotate;
        }

        Quaternion rez = Quaternion.Euler(actual_rotate.eulerAngles - first_rotate.eulerAngles);

        return rez;
    }

    public Quaternion Difference(Quaternion first,Quaternion second)
    {
        Quaternion a = new Quaternion(first.x - second.x, first.y - second.y, first.z - second.z, first.w - second.w);

        return a;
    }
}
