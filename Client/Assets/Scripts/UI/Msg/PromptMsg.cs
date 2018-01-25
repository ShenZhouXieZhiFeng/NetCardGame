using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptMsg {

    public string Msg;
    public Color Color;

    public PromptMsg()
    {

    }

    public PromptMsg(string msg,Color color)
    {
        Msg = msg;
        Color = color;
    }

    public void Change(string msg, Color color)
    {
        Msg = msg;
        Color = color;
    }

}
