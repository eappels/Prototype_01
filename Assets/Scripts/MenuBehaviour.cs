using UnityEngine;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{

    private void OnEnable()
    {
        UIManager.Instance.btn_Server.GetComponent<Button>().onClick.AddListener(btn_Server_Click);
        UIManager.Instance.btn_Client.GetComponent<Button>().onClick.AddListener(btn_Client_Click);
    }

    private void OnDisable()
    {
        UIManager.Instance.btn_Server.GetComponent<Button>().onClick.RemoveListener(btn_Server_Click);
        UIManager.Instance.btn_Client.GetComponent<Button>().onClick.RemoveListener(btn_Client_Click);
    }

    private void btn_Server_Click()
    {
        UIManager.Instance.pnl_Menu.SetActive(false);
        UIManager.Instance.pnl_Server.SetActive(true);
    }

    private void btn_Client_Click()
    {
        UIManager.Instance.pnl_Menu.SetActive(false);
        UIManager.Instance.pnl_Client.SetActive(true);
    }
}