using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loom.Client;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using System.Threading.Tasks;
using System;

public class ShopContractClient : MonoBehaviour
{
    private readonly byte[] privateKey;
    private readonly byte[] publicKey;
    private readonly Address contractAddress;
    private readonly ILogger logger; // ?
    private EvmContract contract;
    private DAppChainClient client; // ? 
    private IRpcClient reader;
    private IRpcClient writer;

    public ShopContractClient(byte[] privateKey, byte[] publicKey, Address address, ILogger logger)
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
        TextAsset textAsset = Resources.Load<TextAsset>("Shop.abi");
        if (textAsset == null) return null;

        return textAsset.text;
    }

    public async Task<JsonGunState> getUnownedWeaponList()
    {
        await ConnectToContract();
        Debug.Log("getUnownedWeaponList");
        GetWeaponsOutput result = await this.contract.StaticCallDtoTypeOutputAsync<GetWeaponsOutput>("getWeaponList");
        Debug.Log(result.state);
        JsonGunState jsonGunState = JsonUtility.FromJson<JsonGunState>(result.state);
        return jsonGunState;
    }
    
    public async Task buyWeapon(uint _id, ItemOwnershipContractClient OwnerClient, JsonGunState.Gun gun)
    {
        try
        {
            await ConnectToContract();
            Debug.Log("buyWepon start");
            await this.contract.CallAsync("buyWeapon", _id);

            JsonGunState jsonGunState = await OwnerClient.GetWeapons();

            if(jsonGunState == null)
            {
                jsonGunState = new JsonGunState();
            }
           
            jsonGunState.guns.Add(gun);
            await OwnerClient.setUserWeapon(jsonGunState);

            Debug.Log("buyWepon end");
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task sellWeapon(int _id,  ItemOwnershipContractClient OwnerClient, BankContractClient BankClient)
    {
        try{
            await ConnectToContract();
            Debug.Log("sellWepon start");
            JsonGunState jsonGunState = await OwnerClient.GetWeapons();
            
            if(jsonGunState == null){
                return; 
            }
            
            jsonGunState.guns.RemoveAt(_id);  
            await OwnerClient.setUserWeapon(jsonGunState); 
            Debug.Log("sellWeapon End"); 

        }catch(Exception e){
            Debug.Log(e);
        }
    }
   

    [FunctionOutput]
    public class GetWeaponsOutput
    {
        [Parameter("string", "state", 1)]
        public string state { get; set; }
    }

    [Function("getWeaponList", "string")]  
    public class GetWeaponsFunction
    {
        [Parameter("string", "state", 1)]
        public string state { get; set; }
    }
}
