using System;
using System.Collections;
using RosMessageTypes.Geometry;
using RosMessageTypes.SnakeArtRobot;
//using RosMessageTypes.MyRobotArmService;
using RosMessageTypes.Trajectory;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class TrajectoryPlanner : MonoBehaviour
{
    private static readonly string ServiceName = "/my_robot_arm_server";
    private static readonly Quaternion PickOrientation = Quaternion.Euler(90, 90, 0);

    //private bool getUpJudge;

    public ArticulationBody[] jointArticulationBodies;
//    public GameObject goal;
    private ROSConnection rc;

    void Start()
    {
        this.rc = ROSConnection.GetOrCreateInstance();

        this.rc.RegisterRosService<MoverServiceRequest, MoverServiceResponse>(ServiceName);
        //Publish();
        
        // PoseManagerにアクセス
        //poseManager = pose.GetComponent<PoseManager>();

        var debugNumberKey = new DebugNumberKey();

        // 起きたら(getUpJudgeがTrueに変化した時だけ)ROSにリクエストを送る
        //this.ObserveEveryValueChanged(x => x.getUpJudge)
            //.Where(x => x).Subscribe(_ => GetUpPublish());
    }

    void Update()
    {
        // デバッグ用起床判定呼び出し
        InputKeyNumber();

        //getUpJudge = poseManager.GetUpJudge();
    }

    public void Publish()
    {
        var request = new MoverServiceRequest();

        var joints = new MyRobotArmMoveitJointsMsg();

        for(var i = 0; i < jointArticulationBodies.Length; i++)
        {
            joints.joints[i] = this.jointArticulationBodies[i].jointPosition[0];
        }
        request.joints_input = joints;
        /*
        request.joints_input.goal_pose = new PoseMsg
        {
            position = goal.transform.position.To<FLU>(),
            orientation = Quaternion.Euler(Mathf.PI, 0, 0).To<FLU>()
        };*/

        this.rc.SendServiceMessage<MoverServiceResponse>(ServiceName, request, TrajectoryResponse);
    }

    void TrajectoryResponse(MoverServiceResponse response)
    {
        if(response.trajectory != null && response.trajectory.joint_trajectory.points.Length > 0)
        {
            print("success>>>");
            StartCoroutine(ExecuteTrajectories(response));
        }
        else
        {
            print("failed>>>");
        }
    }


    IEnumerator ExecuteTrajectories(MoverServiceResponse response)
    {
        foreach (var t in response.trajectory.joint_trajectory.points)
        {
            float[] result = new float[6];
            for (var i = 0; i < t.positions.Length; i++)
            {
                result[i] = (float)t.positions[i] * Mathf.Rad2Deg;
            }
            for (var i = 0; i < this.jointArticulationBodies.Length; i++)
            {
                var joint1XDrive = this.jointArticulationBodies[i].xDrive;
                joint1XDrive.target = result[i];
                this.jointArticulationBodies[i].xDrive = joint1XDrive;
            }

            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
        
    }


    // デバッグ用起床判定
    public void InputKeyNumber()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Publish();
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Publish();
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Publish();
        }
    }
}