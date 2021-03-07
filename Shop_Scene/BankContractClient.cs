using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loom.Client;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using System.Threading.Tasks;

public class BankContractClient : MonoBehaviour
{
    private readonly byte[] privateKey;
    private readonly byte[] publicKey;
    private readonly Address contractAddress;
    private readonly ILogger logger; 
    private EvmContract contract;
    private DAppChainClient client; 
    private IRpcClient reader;
    private IRpcClient writer;

    public BankContractClient(byte[] privateKey, byte[] publicKey, Address address, ILogger logger)
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
        TextAsset textAsset = Resources.Load<TextAsset>("Bank.abi");
        if (textAsset == null) return null;

        return textAsset.text;
    }

    public async Task plusAmount(uint _amount)
    {
        await ConnectToContract();
        Debug.Log("plusAmount Start");
        await this.contract.CallAsync("plusAmount", _amount);
    }

    public async Task<uint> getMyAmount()
    {
        await ConnectToContract();
        Debug.Log("getMyAmount Start");
        GetMyAmountOutput result = await this.contract.StaticCallDtoTypeOutputAsync<GetMyAmountOutput>("getMyAmount"); 
        Debug.Log(result.state);
        return result.state;
    }
 
    [FunctionOutput]
    public class GetMyAmountOutput
    {
        [Parameter("uint", "state", 1)]
        public uint state { get; set; }
    }

    [Function("getMyAmount", "uint")]
    public class getMyAmountFunction
    {
        [Parameter("uint", "state", 1)]
        public uint state { get; set; }
    }
}
