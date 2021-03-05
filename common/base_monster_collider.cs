using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class base_monster_collider : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        //if the gameobject that hits the character is a projectile
        if (col.gameObject.tag == "Bullet")
        {
            col.gameObject.SetActive(false);
            gameObject.GetComponent<MonsterCtrl>().ChangeHP(col.gameObject.GetComponent<ProjectileScript>().Damage, col.contacts[0].point);


        }

        // I should note that these functions can be called from anyother script as well
        // for example in the ProjectileScript.cs these functions can be called within the OnCollisionEnter function

    }
}
