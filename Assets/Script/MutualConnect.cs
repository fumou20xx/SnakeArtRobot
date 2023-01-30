using System.Collections;
using System.Collections.Generic;
using com.rfilkov.components;
using RosMessageTypes.Geometry;
using RosMessageTypes.SnakeArtRobot;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;
using UniRx;

public class MutualConnect : MonoBehaviour
{
    // 定数
    private static readonly string ServiceName = "/snake_art_robot_server";

    // メンバ変数
    public GameObject pose;
    private PoseManager poseManager;

    private ROSConnection rc;
    private string judgeMsg;

    private bool getUpJudge;

    void Start()
    {
        // ROSコネクションの準備
        this.rc = ROSConnection.GetOrCreateInstance();

        // ROSサーバーのレスポンスのコールバックの登録
        this.rc.RegisterRosService<MyServiceRequest, MyServiceResponse>(ServiceName);

        // PoseManagerにアクセス
        //poseManager = pose.GetComponent<PoseManager>();

        // デバッグ用
        Debug.Log("1、2、3を押すと起床");

        // 起きたら(getUpJudgeがTrueに変化した時だけ)ROSにリクエストを送る
        this.ObserveEveryValueChanged(x => x.getUpJudge)
            .Where(x => x).Subscribe(_ => GetUpPublish());
    }

    void Update()
    {
        // デバッグ用起床判定呼び出し
        InputKeyNumber();

        //getUpJudge = poseManager.GetUpJudge();
    }


    // ROSサーバーへのリクエスト送信
    public void Publish(){

        // 新規メッセージの作成
        var request = new MyServiceRequest();

        request.msg = judgeMsg;
        Debug.Log("request.msg =" + request.msg);

        // ROSサーバーへのリクエスト送信
        this.rc.SendServiceMessage<MyServiceResponse>(ServiceName, request, SAResponse);
    }

    // ROSサーバーのレスポンス受信時に呼ばれる
    void SAResponse(MyServiceResponse response){
        Debug.Log(response.res); 
    }

    // 起床時ROSにリクエストを送る
    private void GetUpPublish()
    {
        judgeMsg = "get up";
        Publish();
        //Debug.Log("a");
    }

    // デバッグ用起床判定
    private void InputKeyNumber()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            judgeMsg = "get up";
            Publish();
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            judgeMsg = "get up";
            Publish();
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            judgeMsg = "get up";
            Publish();
        }
    }
}
