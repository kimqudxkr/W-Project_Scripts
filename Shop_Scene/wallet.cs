using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using Loom.Client;

[System.Serializable]
public class Wallet
{
    public byte[] privateKey;
    public byte[] publicKey; 
    public Wallet()
    {
        privateKey = CryptoUtils.GeneratePrivateKey();
        publicKey = CryptoUtils.PublicKeyFromPrivateKey(privateKey);
    }
}
