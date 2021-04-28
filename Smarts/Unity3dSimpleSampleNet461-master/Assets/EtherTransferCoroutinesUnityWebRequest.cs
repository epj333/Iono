using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.Extensions;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using UnityEngine;
using UnityEngine.UI;

public class EtherTransferCoroutinesUnityWebRequest : MonoBehaviour {

    public string Url = "http://127.0.0.1:7545";
    public string PrivateKey = "6ec5965510eae76f9c9cad499d65388becd1768922f80fa2a02845ca43adec9f";
    public string AddressTo = "0xA787a374cd9Dac089fDd02737D4C22c127997f5E";
    public decimal Amount = 1.1m;
    public decimal GasPriceGwei = 2;
    public string TransactionHash = "";
    public decimal BalanceAddressTo = 0m;

    public InputField InputUrl;
    public InputField InputPrivateKey;
    public InputField InputAddressTo;
    public InputField InputAmount;

    public InputField ResultBalanceAddressTo;
    public InputField ResultTxnHash;

    // Use this for initialization
    void Start () {

        InputUrl.text = Url;
        InputPrivateKey.text = PrivateKey;
        InputAddressTo.text = AddressTo;
        InputAmount.text = Amount.ToString();

    }

    public void TransferRequest()
    {
        StartCoroutine(TransferEther());
    }

    //Sample of new features / requests
    public IEnumerator TransferEther()
    {
        Url = InputUrl.text;
        PrivateKey = InputPrivateKey.text;
        AddressTo = InputAddressTo.text;
        Amount = System.Decimal.Parse(InputAmount.text);
         

        //initialising the transaction request sender
        var ethTransfer = new EthTransferUnityRequest(Url, PrivateKey);

        var receivingAddress = AddressTo;
        yield return ethTransfer.TransferEther(receivingAddress, Amount, GasPriceGwei);

        if (ethTransfer.Exception != null)
        {
            Debug.Log(ethTransfer.Exception.Message);
            yield break;
        }

        TransactionHash = ethTransfer.Result;
        ResultTxnHash.text = TransactionHash;
        Debug.Log("Transfer transaction hash:" + TransactionHash);

        //create a poll to get the receipt when mined
        var transactionReceiptPolling = new TransactionReceiptPollingRequest(Url);
        //checking every 2 seconds for the receipt
        yield return transactionReceiptPolling.PollForReceipt(TransactionHash, 2);
        
        Debug.Log("Transaction mined");

        var balanceRequest = new EthGetBalanceUnityRequest(Url);
        yield return balanceRequest.SendRequest(receivingAddress, BlockParameter.CreateLatest());

        BalanceAddressTo = UnitConversion.Convert.FromWei(balanceRequest.Result.Value);
        ResultBalanceAddressTo.text = BalanceAddressTo.ToString();

        Debug.Log("Balance of account:" + BalanceAddressTo);
    }



    // Update is called once per frame
    void Update () {
		
	}
}
