using Protocol;
using Protocol.Code;
using Protocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistPanel : UIBase {

    private Button btnRegist,btnClose;
    private InputField inputAccount, inputPassword, inputRepeat;

    void Start () {
        Bind(UIEvent.REGIST_PANEL_ACTIVE);

        btnRegist = transform.Find("btnRegist").GetComponent<Button>();
        btnClose = transform.Find("btnClose").GetComponent<Button>();
        inputAccount = transform.Find("inputAccount").GetComponent<InputField>();
        inputPassword = transform.Find("inputPassword").GetComponent<InputField>();
        inputRepeat = transform.Find("inputRepeat").GetComponent<InputField>();

        btnRegist.onClick.AddListener(onRegistClick);
        btnClose.onClick.AddListener(onCloseClick);

        setGameObjectActive(false);
    }

    private void OnDestroy()
    {
        base.OnDestroy();

        btnRegist.onClick.RemoveAllListeners();
        btnClose.onClick.RemoveAllListeners();
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.REGIST_PANEL_ACTIVE:
                setGameObjectActive((bool)message);
                break;
            default:break;
        }
    }

    void onRegistClick()
    {
        if (string.IsNullOrEmpty(inputAccount.text) 
            || string.IsNullOrEmpty(inputPassword.text) 
            || string.IsNullOrEmpty(inputRepeat.text))
            return;
        //组织消息并发送
        AccountDto dto = new AccountDto(inputAccount.text, inputPassword.text);
        SocketMsg msg = new SocketMsg(OpCode.ACCOUNT, AccountCode.REGIST_CREQ, dto);
        Dispatch(AreaCode.NET, 0, msg);
    }

    void onCloseClick()
    {
        setGameObjectActive(false);
    }

}
