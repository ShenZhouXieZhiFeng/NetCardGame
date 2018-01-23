using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : UIBase {

    private Button btnStart;
    private Button btnRegist;

	void Start () {
        btnStart = transform.Find("btnStart").GetComponent<Button>();
        btnRegist = transform.Find("btnRegist").GetComponent<Button>();

        btnStart.onClick.AddListener(onStartClick);
        btnRegist.onClick.AddListener(onRegistClick);
    }

    private void OnDestroy()
    {
        base.OnDestroy();

        btnStart.onClick.RemoveAllListeners();
        btnRegist.onClick.RemoveAllListeners();
    }

    void onStartClick()
    {
        Dispatch(AreaCode.UI, UIEvent.START_PANEL_ACTIVE, true);
    }

    void onRegistClick()
    {
        Dispatch(AreaCode.UI, UIEvent.REGIST_PANEL_ACTIVE, true);
    }
	
}
