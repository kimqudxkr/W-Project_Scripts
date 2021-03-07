using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loom.Client;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class RankContractClient : MonoBehaviour
{
    public static  string RankContractAddress = "0x9bcE3F278eAE236c64004C077Af6337b9D5925B4";
    public static Address address = Address.FromString(RankContractAddress);
    private readonly byte[] privateKey;
    private readonly byte[] publicKey;
    private readonly Address contractAddress;
    private readonly ILogger logger;
    private EvmContract contract;
    private DAppChainClient client;
    private IRpcClient reader;
    private IRpcClient writer;

    public RankContractClient(byte[] privateKey, byte[] publicKey, Address address, ILogger logger)
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
        TextAsset textAsset = Resources.Load<TextAsset>("Rank.abi");
        if (textAsset == null) return null;

        return textAsset.text;
    }


    public async Task StartGame(byte[] publicKey)
    {
        try
        {
            await ConnectToContract();
            var address = CryptoUtils.LocalAddressFromPublicKey(publicKey);
            string str_address = BitConverter.ToString(address).Replace("-", "");
            await this.contract.CallAsync("start", str_address);
            Debug.Log("Game Start");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task EndGame(byte[] publicKey, uint totalScore)
    {
        try
        {
            await ConnectToContract();
            var address = CryptoUtils.LocalAddressFromPublicKey(publicKey);
            string str_address = BitConverter.ToString(address).Replace("-", "");
            await this.contract.CallAsync("GameEnd", str_address, totalScore);
            Debug.Log("Game End");

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task calRanking(byte[] publicKey)
    {
        try
        {
            await ConnectToContract();
            var address = CryptoUtils.LocalAddressFromPublicKey(publicKey);
            string str_address = BitConverter.ToString(address).Replace("-", "");
            await this.contract.CallAsync("calRanking", str_address);
            Debug.Log("Cal Ranking");

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task setRankList(JsonRankState jsonRankState)
    {
        try
        {
            await ConnectToContract();
            var address = CryptoUtils.LocalAddressFromPublicKey(publicKey);
            string str_address = BitConverter.ToString(address).Replace("-", "");

            string ranks = JsonUtility.ToJson(jsonRankState);
            await this.contract.CallAsync("setRankList",ranks);
            Debug.Log("setRankList");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task<JsonRankState> getRankList()
    {
        try
        {
            await ConnectToContract();
            getRankListOutput result = await this.contract.StaticCallDtoTypeOutputAsync<getRankListOutput>("getRankList");
            Debug.Log(result.state);
            JsonRankState jsonRankState = JsonUtility.FromJson<JsonRankState>(result.state);
            Debug.Log("getRankList");
            return jsonRankState;
        }
        catch(Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }


    public async Task<uint> getRanking(byte[] publicKey)
    {
        try
        {
            await ConnectToContract();
            var address = CryptoUtils.LocalAddressFromPublicKey(publicKey);
            string str_address = BitConverter.ToString(address).Replace("-", "");
            getRankingOutput result = await this.contract.StaticCallDtoTypeOutputAsync<getRankingOutput>("getRanking", str_address);
            Debug.Log("getRanking");
            return result.state;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return 0;
        }
    }

    public async Task<uint> getTopScore(byte[] publicKey)
    {
        try
        {
            await ConnectToContract();
            var address = CryptoUtils.LocalAddressFromPublicKey(publicKey);
            string str_address = BitConverter.ToString(address).Replace("-", "");
            getTopScoreOutput result = await this.contract.StaticCallDtoTypeOutputAsync<getTopScoreOutput>("getTopScore", str_address);
            Debug.Log("getTopScore");
            return result.state;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return 0;
        }
    }


    public async Task<uint> GetTotalScore(byte[] publicKey)
    {
        await ConnectToContract();
        var address = CryptoUtils.LocalAddressFromPublicKey(publicKey);
        string str_address = BitConverter.ToString(address).Replace("-", "");
        GetTotalScoreOutput result = await this.contract.StaticCallDtoTypeOutputAsync<GetTotalScoreOutput>("getTotalScore", str_address);

        uint score = result.totalScore;
        return score;
    }




    [FunctionOutput]
    public class getRankListOutput
    {
        [Parameter("string", "state", 1)]
        public string state { get; set; }
    }

    [Function("getRankList", "string")]
    public class getRankListFunction
    {
        [Parameter("string", "state", 1)]
        public string state { get; set; }
    }



    [FunctionOutput]
    public class getRankingOutput
    {
        [Parameter("uint", "state", 1)]
        public uint state { get; set; }
    }

    [Function("getRanking", "uint")]
    public class getRankingFunction
    {
        [Parameter("uint", "state", 1)]
        public uint state { get; set; }
    }

    [FunctionOutput]
    public class getTopScoreOutput
    {
        [Parameter("uint", "state", 1)]
        public uint state { get; set; }
    }

    [Function("getTopScore", "uint")]
    public class getTopScoreFunction
    {
        [Parameter("uint", "state", 1)]
        public uint state { get; set; }
    }

    [FunctionOutput]
    public class GetTotalScoreOutput
    {
        [Parameter("uint", "totalScore", 1)]
        public uint totalScore { get; set; }
    }

    [Function("getTotalScore", "uint")]
    public class GetTotalScoreFunction
    {
        [Parameter("uint", "totalScore", 1)]
        public uint totalScore { get; set; }
    }


}
