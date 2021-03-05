using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alien_11corrider : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        //if the gameobject that hits the character is a projectile
        if (col.gameObject.tag == "Bullet")
        {
            col.gameObject.SetActive(false);
            gameObject.GetComponent<Alien_trace>().ChangeHP(col.gameObject.GetComponent<ProjectileScript>().Damage, col.contacts[0].point);
        }
    }
}
