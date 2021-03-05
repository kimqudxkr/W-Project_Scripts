using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CellCtrl : MonoBehaviour
{
    public int CellHP = 10000;
    public Collider cellCollider;

    private void Start()
    {
        cellCollider = this.GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "claw")
        {
            CellHP = CellHP - 10;
            StartCoroutine("Damage");
            
            if (CellHP <= 0)
            {   
                Destroy(this.gameObject);
                SceneManager.LoadScene("Death_Scene");
            }
        }
    }

    IEnumerator Damage()
    {
        cellCollider.enabled = false;
        yield return new WaitForSeconds(1.0f);
        cellCollider.enabled = true;
    }
}
