using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryRaycast : MonoBehaviour
{
    RaycastHit hit;     //레이캐스트에 맞은 물체
    float MaxDistance = 150f;

    gameInformationManager manager;
    public Slot_inventory inven;

    private void Start()
    {
        manager = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            Debug.DrawRay(transform.position, transform.forward * MaxDistance, Color.blue, 0.3f);

            if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance))
            {
                if (hit.collider.tag == "invenNext")
                {
                    manager.gotoNextScene();
                }
                if (hit.collider.tag == "QuickRegister")
                {
                    inven = hit.transform.gameObject.GetComponent<Slot_inventory>();
                    inven.selectItem();
                }
            }
        }
    }
}
