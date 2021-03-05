using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Loom.Client;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;


public class createAssetByNameMyItem : MonoBehaviour
{
    string[] prefabname = { "normal_handgun", "normal_riple" };//아이템 이름 -> 스트링 배열로 변경
    public GameObject slotPrefab;//생성할 슬롯 프리팹
    public GameObject backgroundinventory;
    bool inventoryOn = false;

    public int itemcount = 0;//아이템을 구별할 숫


    public const string OwnershipContractAddress = "0x53B9EC89a69B572da7016eb5979383622EF0F3cb";
    private ItemOwnershipContractClient OwnerClient;

    // 유저의 비밀번호
    public string userPassword;

    // 유저의 지갑
    public Wallet playerWallet;

    // 유저가 가진 토큰
    public uint tokenBalance;


    private JsonGunState myGunState = new JsonGunState();

    public List<string> quickSlotItem = new List<string>();//퀵슬롯에 선택된 아이템 리스트 ====>이걸 다음씬에 넘겨주어 사용자의 퀵슬롯 데이터로 사용


    public List<SlotData_inventory> slots = new List<SlotData_inventory>();//생성된 슬롯 칸 리스트


    // Start is called before the first frame update
    private async void Start()//상점 입장 시 아이템 목록 각각 이미지 프리팹으로 불러와서 보여줌
    {
        userPassword = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>().passwd;
        if (ItemOwnershipContractClient.GetAbi() == null)
        {
            Debug.Log("Error: no ABI file found");
            return;
        }

        playerWallet = LoadWallet(userPassword);
        var playerPrivateKey = playerWallet.privateKey;
        var playerPublicKey = playerWallet.publicKey;

        
        Address ownershipContractAddress = Address.FromString(OwnershipContractAddress);
        this.OwnerClient = new ItemOwnershipContractClient(playerPrivateKey, playerPublicKey, ownershipContractAddress, Debug.unityLogger);

        await ConnectOwnerClient();

        GameObject slotPanel = GameObject.Find("inventorypanel");//캔버스에서 슬롯을 생성시킬 위치
                                                                 //LoadObject(prefabname);
        userPassword = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>().passwd;
       

    }

    // Update is called once per frame
    void Update()
    {

    }


    private async Task ConnectOwnerClient()
    {
        try
        {
            await this.OwnerClient.ConnectToContract();
            Debug.Log("Owner Client Start");
            this.myGunState = await this.OwnerClient.GetWeapons();

            int n = 0;
            GameObject slotPanel = GameObject.Find("inventorypanel");//캔버스에서 슬롯을 생성시킬 위치

            foreach (JsonGunState.Gun g in this.myGunState.guns)
            {
                GameObject go = Instantiate(slotPrefab, slotPanel.transform, false);
                //slotPrefab을 slotPanel위치에 생성
                itemcount++;
                go.name = "Slot_" + g.name + "-" + itemcount;//생성된 슬롯의 이름 -0
                SlotData_inventory slot = new SlotData_inventory();//슬롯 객체 생성
                                                                   //slot.ObjName = "";
                slot.isEmpty = true;//슬롯 객체 상태
                slot.slotObj = go;//슬롯 객체에 오브젝트 할당
                slot.equipItemName = g.name;
                Instantiate(Resources.Load("prefabs/" + "image_" + g.name), go.transform, false);
                //현재 문자열 배열에 해당되는 오브젝트명 앞에 image를 붙여 해당 오브젝트의 이미지를 슬롯에 생성

                ////숫자 설정 파트 추가//삭제
                //GameObject aleadymade;
                //if ( aleadymade = GameObject.Find("Slot_" + g.name))//이미 생성하고자 하는 슬롯이 캔버스에 존재한다g
                //{
                //    Instantiate(itemcount, aleadymade.transform, false);
                //}
                //Debug.Log(n);
                n++;
                slots.Add(slot);//슬롯 추가
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }


    public Wallet LoadWallet(string password)
    {
        string path = Application.persistentDataPath + "/playerEncryptWallet";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Wallet wallet = formatter.Deserialize(stream) as Wallet;
            wallet.privateKey = Decrypt(wallet.privateKey, password);
            wallet.publicKey = Decrypt(wallet.publicKey, password);

            var privateKey = wallet.privateKey;
        
            var publicKey = CryptoUtils.PublicKeyFromPrivateKey(privateKey);
            var address = CryptoUtils.LocalAddressFromPublicKey(publicKey);
            string str = address.ToString();
            stream.Close();

            return wallet;
        }
        else
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            Wallet wallet = new Wallet();

            var privateKey = wallet.privateKey;
            var publicKey = wallet.publicKey;

            wallet.privateKey = Encrypt(privateKey, password);
            wallet.publicKey = Encrypt(publicKey, password);
            formatter.Serialize(stream, wallet);

            wallet.privateKey = privateKey;
            wallet.publicKey = publicKey;
            return wallet;
        }
    }


    public static byte[] Encrypt(byte[] clearData, string Password)
    {

        PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
            new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
        return Encrypt(clearData, pdb.GetBytes(32), pdb.GetBytes(16));
    }

    public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
    {
        MemoryStream ms = new MemoryStream();

        Rijndael alg = Rijndael.Create();
        alg.Key = Key;
        alg.IV = IV;

        CryptoStream cs = new CryptoStream(ms,
           alg.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(clearData, 0, clearData.Length);

        cs.Close();

        byte[] encryptedData = ms.ToArray();

        return encryptedData;
    }

    public static byte[] Decrypt(byte[] cipherData, string Password)
    {
        PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
            new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
        return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16));
    }

    // Decrypt a byte array into a byte array using a key and an IV 
    public static byte[] Decrypt(byte[] cipherData,
                                byte[] Key, byte[] IV)
    {
        MemoryStream ms = new MemoryStream();
        Rijndael alg = Rijndael.Create();

        alg.Key = Key;
        alg.IV = IV;

        CryptoStream cs = new CryptoStream(ms,
            alg.CreateDecryptor(), CryptoStreamMode.Write);

        cs.Write(cipherData, 0, cipherData.Length);
        cs.Close();

        byte[] decryptedData = ms.ToArray();

        return decryptedData;
    }

}
