using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LidClientBehaviour : Singleton<LidClientBehaviour>
{

    public LidClient lidclient;

    private void OnEnable()
    {
        lidclient = new LidClient();
        lidclient.OnNetworkDebugMessage += OnDebugMessages;
        lidclient.Connected += OnConnected;
        lidclient.Disconnected += OnDisconnected;
        lidclient.StartClient();
        UIManager.Instance.btn_Back.GetComponent<Button>().onClick.AddListener(btn_Back_Click);
        UIManager.Instance.btn_Connect.GetComponent<Button>().onClick.AddListener(btn_Connect_Click);
        UIManager.Instance.btn_Disconnect.GetComponent<Button>().onClick.AddListener(btn_Disconnect_Click);
        UIManager.Instance.btn_Spawn.GetComponent<Button>().onClick.AddListener(btn_Spawn_Click);
        UIManager.Instance.btn_Back.SetActive(true);
        UIManager.Instance.btn_Spawn.SetActive(false);
        UIManager.Instance.btn_Disconnect.SetActive(false);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        UIManager.Instance.txt_Ipaddress.GetComponent<InputField>().text = "127.0.0.1:25000";
#endif
    }

    private void FixedUpdate()
    {
        if (lidclient != null) lidclient.MessagePump();
    }

    private void OnDisable()
    {
        lidclient.OnNetworkDebugMessage -= OnDebugMessages;
        lidclient = null;
    }

    private void OnDebugMessages(string debugstring)
    {
        Debug.Log(debugstring);
    }

    private void btn_Back_Click()
    {
        if (lidclient.netclient.Status == Lidgren.Network.NetPeerStatus.Running)
        {
            lidclient.StopClient();
            StartCoroutine("DelayedBack", 1);
        }
        else
        {
            StartCoroutine("DelayedBack", 0);
        }
    }

    private IEnumerator DelayedBack(int delay)
    {
        yield return new WaitForSeconds(delay);
        UIManager.Instance.pnl_Client.SetActive(false);
        UIManager.Instance.pnl_Menu.SetActive(true);

        UIManager.Instance.btn_Back.SetActive(false);
        UIManager.Instance.btn_Spawn.GetComponent<Button>().onClick.RemoveListener(btn_Spawn_Click);
        UIManager.Instance.btn_Back.GetComponent<Button>().onClick.RemoveListener(btn_Back_Click);
        UIManager.Instance.btn_Connect.GetComponent<Button>().onClick.RemoveListener(btn_Connect_Click);
        UIManager.Instance.btn_Disconnect.GetComponent<Button>().onClick.RemoveListener(btn_Disconnect_Click);
    }

    private void btn_Connect_Click()
    {
        lidclient.Connect(UIManager.Instance.txt_Ipaddress.GetComponent<InputField>().text);
    }

    private void btn_Disconnect_Click()
    {
        lidclient.Disconnect();
    }

    private void OnConnected()
    {
        UIManager.Instance.tgl_AutoConnect.SetActive(false);
        UIManager.Instance.txt_Ipaddress.SetActive(false);
        UIManager.Instance.btn_Connect.SetActive(false);
        UIManager.Instance.btn_Back.SetActive(false);
        UIManager.Instance.btn_Disconnect.SetActive(true);
        UIManager.Instance.btn_Spawn.SetActive(true);
    }

    private void OnDisconnected()
    {
        UIManager.Instance.tgl_AutoConnect.SetActive(true);
        UIManager.Instance.txt_Ipaddress.SetActive(true);
        UIManager.Instance.btn_Connect.SetActive(true);
        UIManager.Instance.btn_Disconnect.SetActive(false);
        UIManager.Instance.btn_Spawn.SetActive(false);
        UIManager.Instance.btn_Back.SetActive(true);
    }

    private void btn_Spawn_Click()
    {
        if (UIManager.Instance.btn_Spawn.GetComponentInChildren<Text>().text == "Spawn") NetworkRemoteCallSender.CallOnServer("RPC_RequestSpawn", "PlayerPrefab", new Vector3(Random.Range(-5,5), 3, 0), Quaternion.identity);
        else NetworkRemoteCallSender.CallOnServer("RPC_RequestDespawn");
    }
}