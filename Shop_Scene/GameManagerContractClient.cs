using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loom.Client;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class GameManagerContractClient : MonoBehaviour
{
    private readonly byte[] privateKey;
    private readonly byte[] publicKey;
    private readonly Address contractAddress;
    private readonly ILogger logger;
    private EvmContract contract;
    private DAppChainClient client;
    private IRpcClient reader;
    private IRpcClient writer;

    public GameManagerContractClient(byte[] privateKey, byte[] publicKey, Address address, ILogger logger)
    {
        this.privateKey = privateKey;
        this.publicKey = publicKey;
        this.contractAddress = address;
        this.logger = logger;
    }

    public bool isConnected =>
            this.contract != null &&
            this.contract.Client.ReadClient.ConnectionState == RpcConnectionState.Connected &&
            this.contract.Client.WriteClient.ConnectionState == RpcConnectionState.Connected;

    public async Task ConnectToContract()
    {
        if (this.contract == null)
        {
            this.contract = await GetContract();
        }
    }

    public async Task<EvmContract> GetContract()
    {
        this.writer = RpcClientFactory.Configure()
            .WithLogger(Debug.unityLogger)
            .WithWebSocket("ws://extdev-plasma-us1.dappchains.com:80/websocket")
            .Create();
        this.reader = RpcClientFactory.Configure()
            .WithLogger(Debug.unityLogger)
            .WithWebSocket("ws://extdev-plasma-us1.dappchains.com:80/queryws")
            .Create();

        this.client = new DAppChainClient(this.writer, this.reader)
        { Logger = this.logger };

        this.client.TxMiddleware = new TxMiddleware(new ITxMiddlewareHandler[]
            {
                new NonceTxMiddleware(this.publicKey, this.client),
                new SignedTxMiddleware(this.privateKey)
            });
        await this.client.ReadClient.ConnectAsync();
        await this.client.WriteClient.ConnectAsync();

        var calller = Address.FromPublicKey(this.publicKey);
        EvmContract evmContract = new EvmContract(this.client, this.contractAddress, calller, GetAbi());

        //evmContract.EventReceived += this.EventReceiveHandler;
        await evmContract.Client.SubscribeToAllEvents();
        return evmContract;
    }

    public static string GetAbi()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("GameManager.abi");
        if (textAsset == null) return null;

        return textAsset.text;
    }

    public async Task register(byte[] publicKey, string nickname)
    {
        await ConnectToContract();
        Debug.Log("register");
        try
        {
            var address = CryptoUtils.LocalAddressFromPublicKey(publicKey);
            string str_address = BitConverter.ToString(address).Replace("-", "");

            await this.contract.CallAsync("register", str_address, nickname);
            Debug.Log("register finish");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task setMyNickname(byte[] publicKey, string nickname)
    {
        await ConnectToContract();
        Debug.Log("set My Nickname");
        try
        {
            var address = CryptoUtils.LocalAddressFromPublicKey(publicKey);
            string str_address = BitConverter.ToString(address).Replace("-", "");

            await this.contract.CallAsync("setMyNickcname", str_address, nickname);
            Debug.Log("setMyNickcname finish");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task<string> getMyNickname()
    {
        await ConnectToContract();
        Debug.Log("getMyNickname");
        try
        {
            getMyNicknameOutput result = await this.contract.StaticCallDtoTypeOutputAsync<getMyNicknameOutput>("getMyNickname");
            Debug.Log(result.state);
            Debug.Log("getMyNickname finish");
            return result.state;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public async Task<bool> isRegister()
    {
        await ConnectToContract();
        Debug.Log("isRegister");
        try
        {
            IsRegisterOutput result = await this.contract.StaticCallDtoTypeOutputAsync<IsRegisterOutput>("isRegisteredUser");
            Debug.Log(result.state);
            Debug.Log("isRegister finish");
            return result.state;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }

    public async Task<JsonGunState> getUnownedWeapons()
    {
        try
        {
            await ConnectToContract();
            Debug.Log("getUnownedWeapons start");
            getUnownedWeaponsOutput result = await this.contract.StaticCallDtoTypeOutputAsync<getUnownedWeaponsOutput>("getUnownedWeapons");
            Debug.Log(result.state);
            Debug.Log("getUnownedWeapons Finish");
            JsonGunState jsonGunState = JsonUtility.FromJson<JsonGunState>(result.state);
            return jsonGunState;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return new JsonGunState();
        }
    }

    public async Task buyWeapon(uint _price, ItemOwnershipContractClient OwnerClient, JsonGunState.Gun gun)
    {
        try
        {
            await ConnectToContract();
            Debug.Log("buyWepon start");
            Debug.Log("게임매니저속 buyweapon : " + _price);
            Debug.Log("게임매니저속 gun.price : " + gun.price);

            await this.contract.CallAsync("buyWeapon", _price);

            JsonGunState jsonGunState = await OwnerClient.GetWeapons();

            if (jsonGunState == null)
            {
                jsonGunState = new JsonGunState();
            }

            jsonGunState.guns.Add(gun);
            await OwnerClient.setUserWeapon(jsonGunState);

            Debug.Log("buyWepon end");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    public async Task sellWeapon(uint _price, ItemOwnershipContractClient OwnerClient, JsonGunState.Gun gun)
    {
        try
        {
            await ConnectToContract();
            Debug.Log("SellWeapon start");
            await this.contract.CallAsync("sellWeapon", _price);
            Debug.Log("겜매니저 컨트ㅐㄺ트 클라이언트 가격 : " + _price);
            Debug.Log("Sell");

            JsonGunState jsonGunState = await OwnerClient.GetWeapons();
            Debug.Log("Get");

            if (jsonGunState == null)
            {
                return;
            }

            int i = 0;
            Debug.Log("find Remove");
            Debug.Log(jsonGunState.guns);
            Debug.Log(gun.index);

            foreach (JsonGunState.Gun g in jsonGunState.guns)
            {
                if (g.name == gun.name)
                {
                    break;
                }
                i++;
            }

            Debug.Log("sell Wepon@@@@@@@@@@@@@@:" + jsonGunState.guns[i].name);
            jsonGunState.guns.RemoveAt(i);

            Debug.Log("Remove");


            await OwnerClient.setUserWeapon(jsonGunState);
            Debug.Log("Sell Weapon End");

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task saveGameStage(byte[] publicKey, uint stage)
    {
        try
        {
            var address = CryptoUtils.LocalAddressFromPublicKey(publicKey);
            string str_address = BitConverter.ToString(address).Replace("-", "");
            await this.contract.CallAsync("saveGameStage", str_address, stage);
            Debug.Log("saveGameStage");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task<uint> getGameStage()
    {
        try
        {
            await ConnectToContract();
            getGameStageOutput result = await this.contract.StaticCallDtoTypeOutputAsync<getGameStageOutput>("getGameStage");
            Debug.Log("getGameStage: " + result.state);
            return result.state;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return 0;
        }
    }

    public async Task<uint> BalanceOf(byte[] publicKey)
    {
        await ConnectToContract();
        Debug.Log("BalanceOF");
        try
        {
            var address = CryptoUtils.LocalAddressFromPublicKey(publicKey);
            string str_address = BitConverter.ToString(address).Replace("-", "");
            BalanceOfOutput result = await this.contract.StaticCallDtoTypeOutputAsync<BalanceOfOutput>("balanceOf", str_address);
            Debug.Log(result.state);
            Debug.Log("balanceOf finish");
            return result.state;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return 0;
        }
    }

    [FunctionOutput]
    public class getGameStageOutput
    {
        [Parameter("uint", "getGameStage", 1)]
        public uint state { get; set; }
    }

    [Function("getGameStage", "uint")]
    public class getGameStageFunction
    {
        [Parameter("uint", "getGameStage", 1)]
        public uint state { get; set; }
    }


    [FunctionOutput]
    public class getMyNicknameOutput
    {
        [Parameter("string", "getMyNickname", 1)]
        public string state { get; set; }
    }

    [Function("getMyNickname", "string")]
    public class getMyNicknameFunction
    {
        [Parameter("string", "getMyNickname", 1)]
        public string state { get; set; }
    }


    [FunctionOutput]
    public class IsRegisterOutput
    {
        [Parameter("bool", "isRegister", 1)]
        public bool state { get; set; }
    }

    [Function("isRegister", "bool")]
    public class IsRegisterFunction
    {
        [Parameter("bool", "isRegister", 1)]
        public bool state { get; set; }
    }


    [FunctionOutput]
    public class BalanceOfOutput
    {
        [Parameter("uint", "balanceOf", 1)]
        public uint state { get; set; }
    }

    [Function("balanceOf", "uint")]
    public class BalanceOfFunction
    {
        [Parameter("uint", "balanceOf", 1)]
        public uint state { get; set; }
    }

    [FunctionOutput]
    public class getUnownedWeaponsOutput
    {
        [Parameter("string", "state", 1)]
        public string state { get; set; }
    }

    [Function("getUnownedWeapons", "string")]
    public class getUnownedWeaponsFunction
    {
        [Parameter("string", "state", 1)]
        public string state { get; set; }
    }
}