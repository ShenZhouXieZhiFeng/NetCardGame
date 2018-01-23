using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 消息处理中心
/// </summary>
public class MsgCenter : MonoBehaviour
{
    public static MsgCenter Instance = null;

    private AudioManager mAudioManager;
    private UIManager mUIManager;
    private CharacterManager mCharacterManager;
    private NetManager mNetManager;

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        addComponent();

        initGame();
    }

    /// <summary>
    /// 挂载管理器
    /// </summary>
    private void addComponent()
    {
        mAudioManager = gameObject.AddComponent<AudioManager>();
        mUIManager = gameObject.AddComponent<UIManager>();
        mCharacterManager = gameObject.AddComponent<CharacterManager>();
        mNetManager = gameObject.AddComponent<NetManager>();
    }

    /// <summary>
    /// 初始化游戏
    /// </summary>
    private void initGame()
    {
        //连接服务器
        mNetManager.Connetced("127.0.0.1", 6666);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void Dispatch(int areaCode, int eventCode, object message)
    {
        switch (areaCode)
        {
            case AreaCode.AUDIO:
                mAudioManager.Execute(eventCode, message);
                break;
            case AreaCode.CHARACTER:
                mCharacterManager.Execute(eventCode, message);
                break;
            case AreaCode.NET:
                mNetManager.Execute(eventCode, message);
                break;
            case AreaCode.GAME:
                break;
            case AreaCode.UI:
                mUIManager.Execute(eventCode, message);
                break;
            default:
                break;
        }
    }

}