using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Loom.Client;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading.Tasks;

[Serializable]
public class create_userList : MonoBehaviour //슬롯창 생성하는 부분
{

    //public List<SlotData> slots = new List<SlotData>();
    public int maxSlot = 5;
    public GameObject userList_Slot;
    public GameObject MyScore_rank;
    public GameObject MyScore_nicknamd;
    public GameObject MyScore_score;



    private const string RankContractAddress = "0x9bcE3F278eAE236c64004C077Af6337b9D5925B4";
    private RankContractClient RankClient; 
    private string password;
    private Wallet PlayerWallet;
    private RegisterWallet RegisterWallet = new RegisterWallet();
    private JsonRankState jsonRankState = new JsonRankState();

    private async void Start()
    {
        gameInformationManager manager_logic = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>();
        MyScore_nicknamd.GetComponent<Text>().text = manager_logic.nickName;
        password = manager_logic.passwd;

        manager_logic.sceneNum = 0;

        PlayerWallet = LoadWallet(this.password);
        
        RankClient = new RankContractClient(
            RegisterWallet.privateKey,
            RegisterWallet.publicKey,
            Address.FromString(RankContractAddress),
            Debug.unityLogger);


        if (manager_logic.total_score == 0)
        {
            //타이틀에서 넘어오고
            uint myTopScore = await getTopScore();
            uint myRank = await getRanking();
            await getTop100RankPlayer();

            MyScore_score.GetComponent<Text>().text = myTopScore.ToString();
            if (myTopScore == 0)
            {
                MyScore_rank.GetComponent<Text>().text = "unrank";
            }
            else
                MyScore_rank.GetComponent<Text>().text = myRank.ToString();

        }
        else
        {
            uint totalScore = (uint)manager_logic.total_score;
            await GameEnd(totalScore, manager_logic.nickName);
            await getTop100RankPlayer();

            uint myRank = await getRanking();
            MyScore_rank.GetComponent<Text>().text = myRank.ToString();

            totalScore = await getTotalScore();
            MyScore_score.GetComponent<Text>().text = totalScore.ToString();
        }

        //여기서 부터 로컬 테스트
        JsonRankState.Rank testRanker2;
        testRanker2.nickname = "test2";
        testRanker2.score = 600;

        JsonRankState.Rank testRanker3;
        testRanker3.nickname = "test1";
        testRanker3.score = 450;

        jsonRankState.Ranks.Add(testRanker2);
        jsonRankState.Ranks.Add(testRanker3);

        GameObject slotPanel = GameObject.Find("User_List");
        
        maxSlot = jsonRankState.Ranks.Count;

        foreach(JsonRankState.Rank rank in jsonRankState.Ranks)
        {
            GameObject go = Instantiate(userList_Slot, slotPanel.transform, false);
            go.transform.GetChild(0).GetComponent<Text>().text = (jsonRankState.Ranks.IndexOf(rank) + 1).ToString();
            go.transform.GetChild(1).GetComponent<Text>().text = rank.nickname;//닉네임    
            go.transform.GetChild(2).GetComponent<Text>().text = rank.score.ToString();//점수
        }
    }

    public async Task getTop100RankPlayer()
    {
        try
        {
            await RankClient.ConnectToContract();
            jsonRankState = await RankClient.getRankList();
            foreach(JsonRankState.Rank rank in jsonRankState.Ranks)
            {
                Debug.Log(rank.nickname + ": " + rank.score);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e); 
        }
    }

    public async Task<uint> getTotalScore()
    {
        try
        {
            Debug.Log("getTotalScore Start");
            await RankClient.ConnectToContract();
            uint myTotalScore = await RankClient.GetTotalScore(PlayerWallet.publicKey);
            Debug.Log("myTotalScore" + myTotalScore);
            return myTotalScore;
        }catch(Exception e)
        {
            Debug.Log(e);
            return 0;
        }
    }

    public async Task<uint> getTopScore()
    {
        try
        {
            Debug.Log("getTopScore Start");
            await RankClient.ConnectToContract();
            uint myTopScore = await RankClient.getTopScore(PlayerWallet.publicKey);
            Debug.Log("myTopScore" + myTopScore);
            return myTopScore;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return 0;
        }
    }

    public async Task<uint> getRanking()
    {
        try
        {
            Debug.Log("getRanking Start");
            await RankClient.ConnectToContract();
            await RankClient.calRanking(PlayerWallet.publicKey);
            uint myRank = await RankClient.getRanking(PlayerWallet.publicKey);
            Debug.Log("myRanking: " + myRank);
            return myRank;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return 0;
        }
    }

    public async Task GameStart()
    {
        try
        {
            Debug.Log("GameStart Start");
            await RankClient.ConnectToContract();
            await RankClient.StartGame(PlayerWallet.publicKey);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task GameEnd(uint totalScore, string nickname)
    {
        try
        {
            Debug.Log("GameEnd Start");
            await RankClient.ConnectToContract();
            await RankClient.EndGame(PlayerWallet.publicKey, totalScore); // Rank 점수 계산
            JsonRankState currentRankState = await RankClient.getRankList();
            if (currentRankState == null) currentRankState = new JsonRankState();
            // Top 100 Rank Update
            JsonRankState.Rank myRank;
            myRank.nickname = nickname;
            myRank.score = totalScore;
            currentRankState.RankUpdate(myRank);
            await RankClient.setRankList(currentRankState);

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }


    public Wallet LoadWallet(string password)
    {
        string path = Application.persistentDataPath + "/playerEncryptWallet";
        Debug.Log(path);
        if (File.Exists(path))
        {
            try
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
                Debug.Log("Address:");
                Debug.Log(BitConverter.ToString(address).Replace("-", ""));
                stream.Close();


                return wallet;
            }
            catch (Exception e)
            {
                Debug.Log("LoadWallet error");
                Debug.Log(e);
                return null;
            }
        }
        else
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Debug.Log("Save Wallet Path: " + path);
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
}
