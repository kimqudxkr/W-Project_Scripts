using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class weapons
{
    public List<Gun> guns = new List<Gun>();

    public weapons()
    {
        weapons.Gun item1;
        item1.name = "normal_handgun";
        item1.price = 1000;
        item1.min_attack = 30;
        item1.max_attack = 50;
        item1.index = 0;

        weapons.Gun item2;
        item2.name = "normal_riple";
        item2.price = 1000;
        item2.min_attack = 30;
        item2.max_attack = 50;
        item2.index = 1;

        weapons.Gun item3;
        item3.name = "normal_machinegun";
        item3.price = 1000;
        item3.min_attack = 30;
        item3.max_attack = 50;
        item3.index = 2;

        weapons.Gun item4;
        item4.name = "normal_powergun";
        item4.price = 1000;
        item4.min_attack = 30;
        item4.max_attack = 50;
        item4.index = 3;

        weapons.Gun item5;
        item5.name = "normal_destroygun";
        item5.price = 1000;
        item5.min_attack = 30;
        item5.max_attack = 50;
        item5.index = 3;

        guns.Add(item1);
        guns.Add(item2);
        guns.Add(item3);
        guns.Add(item4);
        guns.Add(item5);
    }

    [Serializable]
    public struct Gun
    {
        public uint price;
        public string name;
        public uint min_attack;
        public uint max_attack;
        public uint index;
    }

}


