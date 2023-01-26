using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugNumberKey : MonoBehaviour
{
    //メンバ変数
    //public string judgeMsg;


    // デバッグ用起床判定
    public void InputKeyNumber()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            //judgeMsg = "get up";
            //Publish();
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            //judgeMsg = "get up";
            //Publish();
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            //judgeMsg = "get up";
            //Publish();
        }
    }
}
