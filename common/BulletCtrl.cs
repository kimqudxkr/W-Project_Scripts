using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float BulletSpeed=15;

    void Start()
    {
       
       /* Destroy(gameObject, 3.0f);*/    //생선된 후 2초뒤에 사라짐
    }

    void Update()
    {
        //초당 정해진 속도로 앞으로 나아감
        transform.position += transform.forward * BulletSpeed * Time.deltaTime;
    }



}