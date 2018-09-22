using UnityEngine;
using UnityEngine.UI;

public class LidServerBehaviour : MonoBehaviour
{

    public LidServer lidserver;

    private void OnEnable()
    {
        lidserver = new LidServer();
        lidserver.OnNetworkDebugMessage += OnDebugMessages;
        UIManager.instance.btn_Back.GetComponent<Button>().onClick.AddListener(btn_Back_Click);
        UIManager.instance.btn_StartStopServer.GetComponent<Button>().onClick.AddListener(btn_StartStopServer_Click);
        UIManager.instance.btn_Back.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (lidserver != null) lidserver.MessagePump();
    }

    private void OnDisable()
    {
        UIManager.instance.btn_Back.SetActive(false);
        UIManager.instance.btn_Back.GetComponent<Button>().onClick.RemoveListener(btn_Back_Click);
        UIManager.instance.btn_StartStopServer.GetComponent<Button>().onClick.RemoveListener(btn_StartStopServer_Click);
        lidserver.OnNetworkDebugMessage -= OnDebugMessages;
        lidserver = null;
    }

    private void OnDebugMessages(string debugstring)
    {
        Debug.Log(debugstring);
    }

    public void btn_StartStopServer_Click()
    {
        if (lidserver.netserver.Status == Lidgren.Network.NetPeerStatus.NotRunning)
        {
            lidserver.StartServer();
            UIManager.instance.btn_Back.SetActive(false);
            UIManager.instance.btn_StartStopServer.GetComponentInChildren<Text>().text = "Stop server";
        }
        else
        {
            lidserver.StopServer();
            UIManager.instance.btn_Back.SetActive(true);
            UIManager.instance.btn_StartStopServer.GetComponentInChildren<Text>().text = "Start server";
        }
    }

    private void btn_Back_Click()
    {
        UIManager.instance.pnl_Server.SetActive(false);
        UIManager.instance.pnl_Menu.SetActive(true);
    }
}