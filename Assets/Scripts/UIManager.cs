using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    public GameObject pnl_Menu, pnl_Server, pnl_Client, btn_Server, btn_Client, btn_StartStopServer, btn_Connect, btn_Disconnect, btn_Spawn, btn_Back, tgl_AutoConnect, txt_Ipaddress;

    private void Start()
    {
        Application.runInBackground = true;
        pnl_Menu.SetActive(true);
    }
}