using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;

public class keyUISetting : MonoBehaviour {

    public Transform groupTrans;
    public Transform restTrans;
    public Transform returnTrans;
    public GameObject keySetTemplate;       //按键设置的模板物体
    public List<GameObject> keySetings;
    [HideInInspector]
    public string[] keyNames;
    [HideInInspector]
    public KeyCode[] codes;
    void Start ()
    {
        keyNames = new string[] { "上", "下", "左", "右", "跳跃", "冲刺", "超级冲刺", "攻击" };
        codes = new KeyCode[] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Space, KeyCode.J, KeyCode.L, KeyCode.K };
        keySetTemplate = Resources.Load<GameObject>("Prefebs/keySetBtn");
        groupTrans = transform.Find("KeySetGroup");
        restTrans = transform.Find("restBtn");
        returnTrans = transform.Find("returnBtn");
        keySetings = new List<GameObject>();
        InitSetBtn();
        InitGroup();
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
    }

    public void OnGUI()
    {
        if (nowSetText!=null)
        {
            if (InputManager.Instance.nowDownKey != KeyCode.Return)
            {
                if (InputManager.Instance.nowDownKey != KeyCode.None)
                {
                    nowSetText.text = InputManager.Instance.nowDownKey.ToString();
                }

            }
            else
            {
                LookKeyText(nowSetText.transform.parent);

                InputManager.Instance.isGetKey = false;
            }

        }
    }

    public void InitSetBtn()
    {
        AddUIEvent(restTrans);
        AddUIEvent(returnTrans);
        var tempKeyClickOne = new KeyValuePair<UnityAction<BaseEventData>, EventTriggerType>(KeyInitClick, EventTriggerType.PointerDown);
        RegisterEvent(restTrans.gameObject, tempKeyClickOne);
        var tempKeyClickTwo = new KeyValuePair<UnityAction<BaseEventData>, EventTriggerType>(ReturnClick, EventTriggerType.PointerDown);
        RegisterEvent(returnTrans.gameObject, tempKeyClickTwo);
    }

    public void InitGroup()
    {
        for (int i=0;i< keyNames.Length; i++)
        {
            GameObject tempSetBtn = Instantiate(keySetTemplate, groupTrans);
            AddUIEvent(tempSetBtn.transform);
           InitBtnData(tempSetBtn.transform,keyNames[i], codes[i]);
            keySetings.Add(tempSetBtn);
            var clickDownEvent = new KeyValuePair<UnityAction<BaseEventData>, EventTriggerType>(KeyClickEvent,EventTriggerType.PointerClick);
            var exitEvent = new KeyValuePair<UnityAction<BaseEventData>, EventTriggerType>(KeyExitEvent, EventTriggerType.PointerExit);
            RegisterEvent(tempSetBtn,clickDownEvent,exitEvent);
        }
    }

    public void InitBtnData(Transform btnTrans,string keyName,KeyCode code)
    {
        Text nameText = btnTrans.Find("keyName").GetComponent<Text>();
        nameText.text = keyName;
        int codeNumber = (int)code;
        string keyVaule = KeyCodeToKeyName(code);
        Transform lookKeyUiShort = btnTrans.Find("keyFrameShot");
        Transform lookKeyUiLong = btnTrans.Find("keyFrameLong");
        if (codeNumber >= 97 && codeNumber <= 122)
        {
            lookKeyUiLong.gameObject.SetActive(false);
            Text keyText = lookKeyUiShort.Find("ketText").GetComponent<Text>();
            keyText.text = keyVaule;
        }
        else
        {
            lookKeyUiShort.gameObject.SetActive(false);
            Text keyText = lookKeyUiLong.Find("ketText").GetComponent<Text>();
            keyText.text = keyVaule;
        }
        GameObject inputTips = btnTrans.Find("setText").gameObject;
        inputTips.SetActive(false);
    }

    public Text nowSetText;

    public void KeyClickEvent(BaseEventData data)
    {
        PointerEventData eventData = (PointerEventData)data;
        HideKeyText(eventData.pointerEnter.transform);
        InputManager.Instance.isGetKey = true;
    }

    public void KeyExitEvent(BaseEventData data)
    {
        PointerEventData eventData = (PointerEventData)data;
        LookKeyText(eventData.pointerEnter.transform);
        InputManager.Instance.isGetKey = false;
    }

    bool isDownPoint = false;
    public void HideKeyText(Transform btnTrans)
    {
        isDownPoint = true;
        GameObject keyNameObj = btnTrans.Find("keyName").gameObject;
        keyNameObj.SetActive(false);
        Transform lookKeyUiShort = btnTrans.Find("keyFrameShot");
        lookKeyUiShort.gameObject.SetActive(false);
        Transform lookKeyUiLong = btnTrans.Find("keyFrameLong");
        lookKeyUiLong.gameObject.SetActive(false);
        Text inputTips = btnTrans.Find("setText").GetComponent<Text>();
        nowSetText = inputTips;
        inputTips.text = "请输入按键";
        inputTips.gameObject.SetActive(true);
    }

    public void LookKeyText(Transform btnTrans)
    {
        if (!isDownPoint)
        {
            return;
        }
        isDownPoint = false;
        GameObject keyNameObj = btnTrans.Find("keyName").gameObject;
        keyNameObj.SetActive(true);
        Text inputTips = btnTrans.Find("setText").GetComponent<Text>();
        int index = keySetings.IndexOf(btnTrans.gameObject);
        KeyCode tempNowKey = SrinigToKeyCode(inputTips.text,index);
        nowSetText = null;
        inputTips.gameObject.SetActive(false);
        Transform lookKeyUiShort = btnTrans.Find("keyFrameShot");
        lookKeyUiShort.gameObject.SetActive(false);
        Transform lookKeyUiLong = btnTrans.Find("keyFrameLong");
        lookKeyUiLong.gameObject.SetActive(false);

        int codeNumber = (int)tempNowKey;
        if (codeNumber >= 97 && codeNumber <= 122)
        {
            lookKeyUiShort.gameObject.SetActive(true);
            Text keyText = lookKeyUiShort.Find("ketText").GetComponent<Text>();
            keyText.text = tempNowKey.ToString();
        }
        else
        {
            lookKeyUiLong.gameObject.SetActive(true);
            Text keyText = lookKeyUiLong.Find("ketText").GetComponent<Text>();
            keyText.text = tempNowKey.ToString();
        }


        SetKey(index, tempNowKey);
        InputManager.Instance.nowDownKey = KeyCode.None;
    }






    public string KeyCodeToKeyName(KeyCode code)
    {
        string codeStr = code.ToString();
        return codeStr;
    }

    public KeyCode SrinigToKeyCode(string keyName,int defultIndex)
    {
        KeyCode code;
        try
        {
            
            code = (KeyCode)Enum.Parse(typeof(KeyCode), keyName);
        }
        catch
        {
            code = codes[defultIndex];
        }

        return code;
    }

    /// <summary>
    /// 添加UI事件
    /// </summary>
    /// <param name="trans"></param>
    public void AddUIEvent(Transform trans)
    {
        IconMove[] moves = InitMaskMove(trans);
        UnityAction<BaseEventData> funcEnter = CreatEvent(moves, 1);
        UnityAction<BaseEventData> funcExit = CreatEvent(moves, 2);
        var tempKeyEnter = new KeyValuePair<UnityAction<BaseEventData>, EventTriggerType>(funcEnter,EventTriggerType.PointerEnter);
        var tempKeyExit = new KeyValuePair<UnityAction<BaseEventData>, EventTriggerType>(funcExit, EventTriggerType.PointerExit);
        RegisterEvent(trans.gameObject, tempKeyEnter, tempKeyExit);
    }

    /// <summary>
    /// 注册UI事件
    /// </summary>
    public void RegisterEvent(GameObject uIObj,params KeyValuePair<UnityAction<BaseEventData>, EventTriggerType>[] events)
    {
        var trigger = uIObj.AddComponent<EventTrigger>();
        if (events!=null)
        {
            for (int i = 0; i < events.Length; i++)
            {
                EventTrigger.Entry tempEntry = new EventTrigger.Entry();
                tempEntry.eventID = events[i].Value;
                tempEntry.callback.AddListener(events[i].Key);
                trigger.triggers.Add(tempEntry);
            }
        }

    }

    public IconMove[] InitMaskMove(Transform trans)
    {
        IconMove[] moves = new IconMove[2];
        GameObject leftIcon = trans.Find("Masks/LeftMask/leftImage").gameObject;
        GameObject rightIcon = trans.Find("Masks/RightMask/rightImage").gameObject;
        IconMove leftMove = leftIcon.AddComponent<IconMove>();
        leftMove.Init(0);
        IconMove rightMove = rightIcon.AddComponent<IconMove>();
        rightMove.Init(1);
        moves[0] = leftMove;
        moves[1] = rightMove;
        return moves;
    }

    /// <summary>
    /// 模式参数 1为鼠标进入 2为鼠标退出
    /// </summary>
    /// <returns></returns>
    public UnityAction<BaseEventData> CreatEvent(IconMove[] moves,int mode)
    {
        UnityAction<BaseEventData> tempEvent = new UnityAction<BaseEventData>((data)=> 
        {
            for (int i=0;i<moves.Length;i++)
            {
                if (mode == 1)
                {
                    moves[i].MouseEnterClick();
                }
                else
                {
                    moves[i].MouseExitClick();
                }

            }
        });
        return tempEvent;
        
    }

    /// <summary>
    /// 重置按键按钮相应事件
    /// </summary>
    /// <param name="data"></param>
    public void KeyInitClick(BaseEventData data)
    {
        RefreshData();
        InputManager.Instance.KeyInit();
    }

    public void ReturnClick(BaseEventData data)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "StartScene")
        {
            UIRoot root = transform.parent.GetComponent<UIRoot>();
            root.CloseKeySetting();
        }
        else 
        {
            LevelUIRoot  uiRoot = transform.parent.GetComponent<LevelUIRoot>();
            uiRoot.CloseKeyPanle();
        }

    }

    /// <summary>
    /// 刷新面版数据
    /// </summary>
    public void RefreshData()
    {
        for (int i=0;i< keySetings.Count;i++)
        {
            InitBtnData(keySetings[i].transform, keyNames[i], codes[i]);
        }
    }

    public void SetKey(int index,KeyCode code)
    {
        if (code == KeyCode.None)
        {
            return;
        }
        switch (index)
        {
            case 0:
                InputManager.Instance.moveUpKey = code;
                break;
            case 1:
                InputManager.Instance.moveDownKey = code;
                break;
            case 2:
                InputManager.Instance.moveLeftKey = code;
                break;
            case 3:
                InputManager.Instance.moveRightKey = code;
                break;
            case 4:
                InputManager.Instance.jumpKey = code;
                break;
            case 5:
                InputManager.Instance.sprintKey = code;
                break;
            case 6:
                InputManager.Instance.superKey = code;
                break;
            case 7:
                InputManager.Instance.attackKey = code;
                break;
        }
    }
}
