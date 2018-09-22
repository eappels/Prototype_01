using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LidClientBehaviour : MonoBehaviour
{

    public LidClient lidclient;

    private void OnEnable()
    {
        lidclient = new LidClient();
        lidclient.OnNetworkDebugMessage += OnDebugMessages;
        lidclient.Connected += OnConnected;
        lidclient.Disconnected += OnDisconnected;
        lidclient.StartClient();
        UIManager.instance.btn_Back.GetComponent<Button>().onClick.AddListener(btn_Back_Click);
        UIManager.instance.btn_Connect.GetComponent<Button>().onClick.AddListener(btn_Connect_Click);
        UIManager.instance.btn_Disconnect.GetComponent<Button>().onClick.AddListener(btn_Disconnect_Click);
        UIManager.instance.btn_Spawn.GetComponent<Button>().onClick.AddListener(btn_Spawn_Click);
        UIManager.instance.btn_Back.SetActive(true);
        UIManager.instance.btn_Spawn.SetActive(false);
        UIManager.instance.btn_Disconnect.SetActive(false);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        UIManager.instance.txt_Ipaddress.GetComponent<InputField>().text = "127.0.0.1:25000";
#endif
    }

    private void FixedUpdate()
    {
        if (lidclient != null) lidclient.MessagePump();
    }

    private void OnDisable()
    {
        UIManager.instance.btn_Back.SetActive(false);
        UIManager.instance.btn_Spawn.GetComponent<Button>().onClick.RemoveListener(btn_Spawn_Click);
        UIManager.instance.btn_Back.GetComponent<Button>().onClick.RemoveListener(btn_Back_Click);
        UIManager.instance.btn_Connect.GetComponent<Button>().onClick.RemoveListener(btn_Connect_Click);
        UIManager.instance.btn_Disconnect.GetComponent<Button>().onClick.RemoveListener(btn_Disconnect_Click);
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
        UIManager.instance.pnl_Client.SetActive(false);
        UIManager.instance.pnl_Menu.SetActive(true);
    }

    private void btn_Connect_Click()
    {
        lidclient.Connect(UIManager.instance.txt_Ipaddress.GetComponent<InputField>().text);
    }

    private void btn_Disconnect_Click()
    {
        lidclient.Disconnect();
    }

    private void OnConnected()
    {
        UIManager.instance.tgl_AutoConnect.SetActive(false);
        UIManager.instance.txt_Ipaddress.SetActive(false);
        UIManager.instance.btn_Connect.SetActive(false);
        UIManager.instance.btn_Back.SetActive(false);
        UIManager.instance.btn_Disconnect.SetActive(true);
        UIManager.instance.btn_Spawn.SetActive(true);
    }

    private void OnDisconnected()
    {
        UIManager.instance.tgl_AutoConnect.SetActive(true);
        UIManager.instance.txt_Ipaddress.SetActive(true);
        UIManager.instance.btn_Connect.SetActive(true);
        UIManager.instance.btn_Disconnect.SetActive(false);
        UIManager.instance.btn_Spawn.SetActive(false);
        UIManager.instance.btn_Back.SetActive(true);
    }

    private void btn_Spawn_Click()
    {
        NetworkRemoteCallSender.CallOnServer("RPC_RequestSpawn", "PlayerPrefab", Vector3.zero, Quaternion.identity);
    }
}