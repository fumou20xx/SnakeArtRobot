using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using MobileRobotMsg = RosMessageTypes.SnakeArtRobot.MobileRobotMsg;

public class MobileRobotSubscriber : MonoBehaviour
{
    ROSConnection ros;

    // 初期化時に呼ばれる
    void Start()
    {
        // ROSコネクションの取得
        ros = ROSConnection.GetOrCreateInstance();

        // サブスクライバーの登録
        ros.Subscribe<MobileRobotMsg>("mobile_robot_topic", OnSubscribe);
    }

    // サブスクライブ時に呼ばれる
    void OnSubscribe(MobileRobotMsg msg)
    {
        Debug.Log("Subscribe : " + msg.x + "," + msg.y + "," + msg.z);
    }
}
