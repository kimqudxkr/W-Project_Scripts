using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loom.Client;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using System.Threading.Tasks;

public class ItemOwnershipContractClient : MonoBehaviour
{
    private readonly byte[] privateKey;
    private readonly byte[] publicKey;
    private readonly Address contractAddress;
    private readonly ILogger logger; // ?
    private EvmContract contract;
    private DAppChainClient client; // ? 
    private IRpcClient reader;
    private IRpcClient writer;

    public ItemOwnershipContractClient(byte[] privateKey, byte[] publicKey, Address address, ILogger logger)
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
        TextAsset textAsset = Resources.Load<TextAsset>("ItemOwnership.abi");
        if (textAsset == null) return null;

        return textAsset.text;
    }

    public async Task setUserWeapon(JsonGunState jsonGunState)
    {
        Debug.Log("setUserWeapon");
        await ConnectToContract();

        string guns = JsonUtility.ToJson(jsonGunState);
        await this.contract.CallAsync("setUserWeapon", guns);
        Debug.Log("end setUserWeapon");
    }


    public async Task<JsonGunState> GetWeapons()
    {
        await ConnectToContract();
        Debug.Log("itemOwernship getWeapons");
        GetWeaponsOutput result = await this.contract.StaticCallDtoTypeOutputAsync<GetWeaponsOutput>("getWeapons");
        JsonGunState jsonGunState = JsonUtility.FromJson<JsonGunState>(result.state);
        Debug.Log(jsonGunState.guns);
        return jsonGunState;
    }


    public async Task<Address> OwnerOf(uint _tokenId, uint _type)
    {
        await ConnectToContract();
        Debug.Log("Before Address: ");

        OwnerOfOutput result = await this.contract.StaticCallDtoTypeOutputAsync<OwnerOfOutput>("ownerOf", _tokenId, _type);
        Debug.Log("Address: " + result);

        if (result == null) return new Address() ;

        Address ownerAddress = (result.ownerAddress); 
        return ownerAddress; 
    }

    public async Task<bool> IsOwner(uint _tokenId, uint _type)
    {
        await ConnectToContract();
        IsOwnerOutput result = await this.contract.StaticCallDtoTypeOutputAsync<IsOwnerOutput>("isOwner", _tokenId, _type);

        bool isOwner = result.isOwner;
        return isOwner; 
    }

    [FunctionOutput]
    public class IsOwnerOutput
    {
        [Parameter("bool","isOwner",1)]
        public bool isOwner { get; set; }
    }

    [Function("isOwner","bool")]
    public class IsOwnerFunction
    {
        [Parameter("bool", "isOwner", 1)]
        public bool isOwner { get; set; }
    }

    [FunctionOutput]
    public class OwnerOfOutput
    {
        [Parameter("Address", "ownerAddress", 1)]
        public Address ownerAddress { get; set; }
    }

    [Function("ownerOf", "Address")]
    public class OwnerOfFunction
    {
        [Parameter("Address","ownerAddress",1)]
        public Address ownerAddress { get; set; }
    }

    [FunctionOutput]
    public class GetWeaponsOutput
    {
        [Parameter("string", "state", 1)]
        public string state { get; set; }
    }

    [Function("getWeapons", "string")]
    public class GetWeaponsFunction
    {
        [Parameter("string", "state", 1)]
        public string state { get; set; }
    }
}
