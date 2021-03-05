using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointScript    : MonoBehaviour
{
    public static Transform[] RightPoints;
    public static Transform[] LeftPoints;
    public static Transform[] CenterPoints;

    void Awake() 
    {
        if (this.name == "RightPoint")
        {
            RightPoints = new Transform[transform.childCount];
            for (int i = 0; i < RightPoints.Length; i++)
            {
                RightPoints[i] = transform.GetChild(i);
            }
        }

        if (this.name == "LeftPoint")
        {
            LeftPoints = new Transform[transform.childCount];
            for (int i = 0; i < LeftPoints.Length; i++)
            {
                LeftPoints[i] = transform.GetChild(i);
            }
        }

        if (this.name == "CenterPoint")
        {
            CenterPoints = new Transform[transform.childCount];
            for (int i = 0; i < CenterPoints.Length; i++)
            {
                CenterPoints[i] = transform.GetChild(i);
            }
        }
    }
}