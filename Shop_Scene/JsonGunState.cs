using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class JsonGunState
{
    public List<Gun> guns = new List<Gun>();

    [Serializable]
    public struct Gun
    {
        public uint max_attack;
        public uint min_attack;
        public string name;
        public uint price;
        public uint index;
    }
}
