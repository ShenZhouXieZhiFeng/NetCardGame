using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptPanel : UIBase {

    private Text txtTip;
    private CanvasGroup cg;

    [SerializeField]
    [Range(0,3)]
    private float showTime = 1f;
    private float timer = 0f;

    private void Start()
    {
        Bind(UIEvent.PROMPT_MSG);
        txtTip = transform.Find("txtTip").GetComponent<Text>();
        cg = GetComponent<CanvasGroup>();

        setGameObjectActive(false);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.PROMPT_MSG:
                PromptMsg msg = message as PromptMsg;
                promptMessage(msg.Msg, msg.Color);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 提示消息
    /// </summary>
    private void promptMessage(string msg,Color color)
    {
        txtTip.text = msg;
        txtTip.color = color;
        cg.alpha = 0;
        timer = 0;
        setGameObjectActive(true);
        StartCoroutine(anim());
    }
    IEnumerator anim()
    {
        while (cg.alpha < 1)
        {
            cg.alpha += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
        while (timer <= showTime)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while (cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
        setGameObjectActive(false);
    }
}
