﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : UIBase {

    private Button btnLogin;
    private Button btnClose;
    private InputField inputAccount;
    private InputField inputPassword;

    private void Awake()
    {
        Bind(UIEvent.START_PANEL_ACTIVE);
    }

    // Use this for initialization
    void Start () {
        btnLogin = transform.Find("btnLogin").GetComponent<Button>();
        btnClose = transform.Find("btnClose").GetComponent<Button>();
        inputAccount = transform.Find("inputAccount").GetComponent<InputField>();
        inputPassword = transform.Find("inputPassword").GetComponent<InputField>();

        btnLogin.onClick.AddListener(onLoginClick);
        btnClose.onClick.AddListener(onCloseClick);

        setGameObjectActive(false);
    }

    private void OnDestroy()
    {
        base.OnDestroy();

        btnLogin.onClick.RemoveAllListeners();
        btnClose.onClick.RemoveAllListeners();
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.START_PANEL_ACTIVE:
                setGameObjectActive((bool)message);
                break;
            default:
                break;
        }
    }

    void onLoginClick()
    {
        if (string.IsNullOrEmpty(inputAccount.text) || string.IsNullOrEmpty(inputPassword.text))
            return;
        //TODO
    }

    void onCloseClick()
    {
        setGameObjectActive(false);
    }
}