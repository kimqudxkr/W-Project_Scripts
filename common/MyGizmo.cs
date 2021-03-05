using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmo : MonoBehaviour
{
    public Color color = Color.green;

    public float radius = 0.1f;

    void OnDrawGizmos()
    {
        Gizmos.color = color;   //기즈모 색상설정

        Gizmos.DrawSphere(transform.position, radius);  //기즈모를 해당위치에서 반지름만큼 구를 그린다.
    }
}
