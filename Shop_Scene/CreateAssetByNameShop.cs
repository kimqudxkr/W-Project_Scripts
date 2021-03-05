using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using Loom.Client;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;


public class CreateAssetByNameShop : MonoBehaviour
{
    public Text priceString;
    public Text itemName;

    string[] prefabname = new string[10];//아이템 이름 -> 스트링 배열로 변경
    public GameObject slotPrefab;//생성할 슬롯 프리팹
    public List<SlotData> slots = new List<SlotData>();//생성된 슬롯 칸 리스트

    // blockchain
    public const string FactoryContractAddress = "0xD6b3EE82828C3a27759940fa3DD41b105c636475";
    public const string GameManagerContractAddress = "0x4bd24104b48a0EfCeE18315d88F0967a2ce2Dc2E";
    public const string OwnershipContractAddress = "0x53B9EC89a69B572da7016eb5979383622EF0F3cb";


    private GameManagerContractClient ManagerClient;
    private ItemFactoryContractClient FactoryClient;
    private ItemOwnershipContractClient OwnerClient;
    
    private JsonGunState shopGunState = new JsonGunState();
    private JsonGunState getManagerGunState = new JsonGunState();
    private JsonGunState jsonGunState = new JsonGunState();

    // 유저의 비밀번호 
    public string userPassword;

    // 유저의 지갑
    public Wallet playerWallet;

    // 유저가 가진 토큰
    public uint tokenBalance;

    public uint money;
    public Text text;
    public string gunname;
    public string sellgunname;
    public bool clicked = false;

    public string destroyName;

    public string itemname="";

    gameInformationManager infoManager;

    public GameObject slotPanel;
    JsonGunState myGunState = new JsonGunState();
    public string[] newGunsInShop = new string[5];
    public weapons weaponList = new weapons();

    public GameObject loading;

    async void Start()//상점 입장 시 아이템 목록 각각 이미지 프리팹으로 불러와서 보여줌
    {
        Instantiate(loading, GameObject.Find("PopUpLocation").transform, false);

        ////인포매니저에서 비밀번호 받아옴
        infoManager = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>();
        userPassword = infoManager.passwd;
        newGunsInShop = infoManager.newGuns;



        text = GameObject.Find("Money").GetComponent<Text>();
        if (GameManagerContractClient.GetAbi() == null)
        {
            Debug.Log("Error: no ABI file found");
            return;
        }

        if (ItemFactoryContractClient.GetAbi() == null)
        {
            Debug.Log("Error: no ABI file found");
            return;
        }

        if (ItemOwnershipContractClient.GetAbi() == null)
        {
            Debug.Log("Error: no ABI file found");
            return;
        }

        playerWallet = LoadWallet(userPassword);
        var playerPrivateKey = playerWallet.privateKey;
        var playerPublicKey = playerWallet.publicKey;

        Address factoryContractAddress = Address.FromString(FactoryContractAddress);
        Address managerContractAddress = Address.FromString(GameManagerContractAddress);
        Address ownershipContractAddress = Address.FromString(OwnershipContractAddress);

        this.FactoryClient = new ItemFactoryContractClient(playerPrivateKey, playerPublicKey, factoryContractAddress, Debug.unityLogger);
        this.OwnerClient = new ItemOwnershipContractClient(playerPrivateKey, playerPublicKey, ownershipContractAddress, Debug.unityLogger);
        this.ManagerClient = new GameManagerContractClient(playerPrivateKey, playerPublicKey, managerContractAddress, Debug.unityLogger);


        await setUserWeapons();
        await ConnectFactoryClient();
        await ConnectManagerClient();

        slotPanel = GameObject.Find("useitempanel");//캔버스에서 슬롯을 생성시킬 위치
        int j = 0;

        foreach (JsonGunState.Gun g in this.getManagerGunState.guns)//이 슬롯들에 레이캐스트 입력 받는 컴포넌트 추가 - 선택하면 색 변하는 효과?(캔버스에 반투명 덮어씌우는 식으로?)
        {
            if (j < 10)
            {
                Debug.Log("Start Foreach  " + g.index);
                GameObject go = Instantiate(slotPrefab, slotPanel.transform, false);     //slotPrefab을 slotPanel위치에 생성
                prefabname[j] = g.name;
                priceString.GetComponent<Text>().text = g.price.ToString();
                itemName.GetComponent<Text>().text = g.name;
                go.name = prefabname[j] + "/" + g.price;//생성된 슬롯의 이름
                SlotData slot = new SlotData();//슬롯 객체 생성
                slot.isEmpty = true;            //슬롯 객체 상태
                slot.slotObj = go;//슬롯 객체에 오브젝트 할당
                Instantiate(Resources.Load("prefabs/" + "image_" + prefabname[j]), go.transform, false);
                //현재 문자열 배열에 해당되는 오브젝트명 앞에 image를 붙여 해당 오브젝트의 이미지를 슬롯에 생성
                Instantiate(priceString, go.transform, false);
                Instantiate(itemName, go.transform, false);
                slots.Add(slot);//슬롯 추가
                j++;
            }
            else
                break;
        }

        money = this.tokenBalance;
        text.GetComponent<Text>().text = "보유금액:" + money;

        Destroy(GameObject.Find("Loading(Clone)").gameObject);
    }

    // Update is called once per frame
    async void Update()
    {
        if (clicked == true)
        {
            Debug.Log("스크립트 넘어온 무기 이름:" + gunname);
            clicked = false;
            await buyWeapon(gunname);
            money = this.tokenBalance;
            text.GetComponent<Text>().text = "보유금액:" + money;
        }
    }

    public async Task setUserWeapons()
    {
        try
        {
            Debug.Log("Shop setUserWeapons"); 
            this.myGunState = await this.OwnerClient.GetWeapons();
            if (this.myGunState == null) this.myGunState = new JsonGunState(); 
            Debug.Log("getWeapons End");
            Debug.Log(myGunState.guns);
            for(int i = 0; i < newGunsInShop.Length; i++)
            {
                if (String.IsNullOrEmpty(newGunsInShop[i]))
                    break;
                else
                {
                    string gunName = newGunsInShop[i];
                    Debug.Log(gunName);
                    foreach (weapons.Gun g in this.weaponList.guns)
                    {
                        Debug.Log("weapon guname" + g.name);
                        if (g.name == gunName)
                        {
                            Debug.Log("setUserWeapons:"+gunName);
                            JsonGunState.Gun tempGun;
                            tempGun.name = g.name;
                            tempGun.price = g.price;
                            tempGun.min_attack = g.min_attack;
                            tempGun.max_attack = g.max_attack;
                            tempGun.index = g.index;
                            this.myGunState.guns.Add(tempGun);
                            await this.OwnerClient.setUserWeapon(this.myGunState);
                        }
                    }
                }
                infoManager.newGuns[i] = null;
            }
        }catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task buyWeapon(string gunName)
    {
        try
        {
            Debug.Log("buyWeapon안에 넘어온 무기 이름 : " + gunName);
            await this.ManagerClient.ConnectToContract();
            foreach (JsonGunState.Gun g in getManagerGunState.guns)
            {
                if(gunName == g.name)
                {
                    Debug.Log("BuyWeapon");
                    Debug.Log(gunName + "의 가격 : " + g.price);
                    await this.ManagerClient.buyWeapon(g.price, this.OwnerClient, g);
                    tokenBalance = await this.ManagerClient.BalanceOf(playerWallet.publicKey);
              
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);

        }
    }

    public async Task sellWeapon()
    {
        try
        {
            Debug.Log("sellWeapon Start");
            Debug.Log(sellgunname);

            await this.ManagerClient.ConnectToContract();

            Debug.Log(getManagerGunState.guns);

            foreach (JsonGunState.Gun g in getManagerGunState.guns)
            {
                Debug.Log(g.name);

                if (sellgunname == g.name)
                {
                    Debug.Log(g.name + "가격 : " + g.price);
                    await this.ManagerClient.sellWeapon(g.price, this.OwnerClient, g);
                    tokenBalance = await this.ManagerClient.BalanceOf(playerWallet.publicKey);

                }
            }

            Destroy(GameObject.Find(destroyName).gameObject);
            money = this.tokenBalance;
            text.GetComponent<Text>().text = "보유금액:" + money;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private async Task ConnectManagerClient()
    {
        try
        {
            await this.ManagerClient.ConnectToContract();
            Debug.Log("Manager Client Start");
            
            this.tokenBalance = await this.ManagerClient.BalanceOf(playerWallet.publicKey);
            getManagerGunState = await this.ManagerClient.getUnownedWeapons();
            JsonGunState.Gun gun;
            Debug.Log(getManagerGunState);

            foreach (JsonGunState.Gun g in getManagerGunState.guns)
            {
                Debug.Log(g.name);
                Debug.Log(g.price);
                Debug.Log(g.index);
                gun = g;
                //await this.ManagerClient.buyWeapon(0, this.OwnerClient, g);

                if (g.index == 0)
                {
                    //await this.ManagerClient.sellWeapon(0, this.OwnerClient, g);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);

        }
    }

    private async Task ConnectFactoryClient()
    {
        try
        {
            await this.FactoryClient.ConnectToContract();
            Debug.Log("Factory Client Start");

            Debug.Log("start JsonGunState:");

            // 블록체인에 저장할 데이터
            JsonGunState.Gun gun = new JsonGunState.Gun
            {
                max_attack = 100000,
                min_attack = 43434,
                name = "rare_handgun",
                price = 4000,      
                index = 0
            };

            JsonGunState.Gun gun2 = new JsonGunState.Gun
            {
                max_attack = 1000,
                min_attack = 5534,
                name = "rare_riple",
                price = 6000,      
                index = 1
            };

            JsonGunState.Gun gun3 = new JsonGunState.Gun
            {
                max_attack = 1001,
                min_attack = 5531,
                name = "rare_machinegun",
                price = 8000,      
                index = 2
            };

            JsonGunState.Gun gun4 = new JsonGunState.Gun
            {
                max_attack = 1002,
                min_attack = 5532,
                name = "rare_powergun",
                price = 10000,      
                index = 3
            };

            JsonGunState.Gun gun5 = new JsonGunState.Gun
            {
                max_attack = 1003,
                min_attack = 5533,
                name = "rare_destroygun",
                price = 12000,      
                index = 4
            };

            JsonGunState.Gun gun6 = new JsonGunState.Gun
            {
                max_attack = 100000,
                min_attack = 43434,
                name = "epic_handgun",
                price = 7000,     
                index = 5
            };

            JsonGunState.Gun gun7 = new JsonGunState.Gun
            {
                max_attack = 1000,
                min_attack = 5534,
                name = "epic_riple",
                price = 10000,     
                index = 6
            };

            JsonGunState.Gun gun8 = new JsonGunState.Gun
            {
                max_attack = 1001,
                min_attack = 5531,
                name = "epic_machinegun",
                price = 13000,     
                index = 7
            };

            JsonGunState.Gun gun9 = new JsonGunState.Gun
            {
                max_attack = 1002,
                min_attack = 5532,
                name = "epic_powergun",
                price = 16000,     
                index = 8
            };

            JsonGunState.Gun gun10 = new JsonGunState.Gun
            {
                max_attack = 1003,
                min_attack = 5533,
                name = "epic_destroygun",
                price = 19000,    
                index = 9
            };

            JsonGunState.Gun gun11 = new JsonGunState.Gun
            {
                max_attack = 100000,
                min_attack = 43434,
                name = "normal_handgun",
                price = 1000,     
                index = 10
            };

            JsonGunState.Gun gun12 = new JsonGunState.Gun
            {
                max_attack = 1000,
                min_attack = 5534,
                name = "normal_riple",
                price = 2000,      
                index = 11
            };

            JsonGunState.Gun gun13 = new JsonGunState.Gun
            {
                max_attack = 1001,
                min_attack = 5531,
                name = "normal_machinegun",
                price = 3000,      
                index = 12
            };

            JsonGunState.Gun gun14 = new JsonGunState.Gun
            {
                max_attack = 1002,
                min_attack = 5532,
                name = "normal_powergun",
                price = 4000,    
                index = 13
            };

            JsonGunState.Gun gun15 = new JsonGunState.Gun
            {
                max_attack = 1003,
                min_attack = 5533,
                name = "normal_destroygun",
                price = 5000,     
                index = 14
            };

            this.jsonGunState.guns.Add(gun);
            this.jsonGunState.guns.Add(gun2);
            this.jsonGunState.guns.Add(gun3);
            this.jsonGunState.guns.Add(gun4);
            this.jsonGunState.guns.Add(gun5);
            this.jsonGunState.guns.Add(gun6);
            this.jsonGunState.guns.Add(gun7);
            this.jsonGunState.guns.Add(gun8);
            this.jsonGunState.guns.Add(gun9);
            this.jsonGunState.guns.Add(gun10);
            this.jsonGunState.guns.Add(gun11);
            this.jsonGunState.guns.Add(gun12);
            this.jsonGunState.guns.Add(gun13);
            this.jsonGunState.guns.Add(gun14);
            this.jsonGunState.guns.Add(gun15);


            await this.FactoryClient.MakeWeapon(this.jsonGunState);
            JsonGunState testGuns = await this.FactoryClient.GetWeapons();
            foreach (JsonGunState.Gun g in testGuns.guns)
            {
                Debug.Log(g.name);
                Debug.Log(g.price);
                Debug.Log(g.index);
            }
            Debug.Log("test JsonGunState:");
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
