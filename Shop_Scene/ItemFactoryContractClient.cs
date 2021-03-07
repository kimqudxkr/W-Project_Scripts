using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loom.Client;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using System.Threading.Tasks;

public class ItemFactoryContractClient : MonoBehaviour
{
    private readonly byte[] privateKey;
    private readonly byte[] publicKey;
    private readonly Address contractAddress;
    private readonly ILogger logger; // ?
    private EvmContract contract;
    private DAppChainClient client; // ? 
    private IRpcClient reader;
    private IRpcClient writer;

    // public event Action<JsonTileMapState> TileMapStateUpdated;

    public ItemFactoryContractClient(byte[] privateKey, byte[] publicKey, Address address, ILogger logger)
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
        TextAsset textAsset = Resources.Load<TextAsset>("ItemFactory.abi");
        if (textAsset == null) return null;

        return textAsset.text;
    }

    public async Task CreateWeapon(uint _main_type, uint _sub_type, uint _grade, string _name, uint _min_attack, uint _max_attack, uint _probability)
    {
        await ConnectToContract();
        Debug.Log("CreateWeapon");

        await this.contract.CallAsync("createWeapon", _main_type, _sub_type, _grade, _name, _min_attack, _max_attack, _probability);
        Debug.Log("end CreateWeapon");
    }

    public async Task MakeWeapon(JsonGunState jsonGunState)
    {
        Debug.Log("MakeWeapon");
        await ConnectToContract();

        string guns = JsonUtility.ToJson(jsonGunState);
        Debug.Log(guns);
        await this.contract.CallAsync("makeWeapon", guns);
        Debug.Log("end MakeWeapon");
    }

    public async Task<JsonGunState> GetWeapons()
    {
        await ConnectToContract();
        Debug.Log("getWeapons");
        GetWeaponsOutput result = await this.contract.StaticCallDtoTypeOutputAsync<GetWeaponsOutput>("getUnownedWeapons");
        Debug.Log(result.state);
        JsonGunState jsonGunState = JsonUtility.FromJson<JsonGunState>(result.state);
        return jsonGunState;
    }


    [FunctionOutput]
    public class GetWeaponsOutput
    {
        [Parameter("string", "state", 1)]
        public string state { get; set; }
    }

    [Function("getWeapon", "string")]
    public class GetWeaponsFunction
    {
        [Parameter("string", "state", 1)]
        public string state { get; set; }
    }

}
