using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndRaycast : MonoBehaviour
{
    RaycastHit hit;     //레이캐스트에 맞은 물체
    float MaxDistance = 150f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            Debug.DrawRay(transform.position, transform.forward * MaxDistance, Color.blue, 0.3f);

            if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance))
            {
                if (hit.collider.tag == "GoToTitle")
                {
                    SceneManager.LoadScene("Title_Scene");
                }

                if (hit.collider.tag == "Quit")
                {
                    Application.Quit();
                }
            }
        }
    }
}
