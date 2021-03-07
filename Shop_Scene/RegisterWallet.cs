using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using Loom.Client;

public class RegisterWallet : MonoBehaviour
{
    public byte[] privateKey = {
        204,66,226,16,213,126,202,230,5,234,
        25,12,24,115,145,244,179,211,45,143,
        160,231,37,255,198,191,80,195,191,171,
        76,233,21,59,171,24,31,55,178,170,
        178,71,176,124,47,124,123,106,45,223,
        127,190,51,180,50,254,58,130,80,6,
        45,112,141,226};
    public byte[] publicKey;

    public RegisterWallet()
    {
        publicKey = CryptoUtils.PublicKeyFromPrivateKey(privateKey);
    }
}
