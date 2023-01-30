using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RosMessageTypes.Geometry;
using RosMessageTypes.SnakeArtRobot;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using RosMessageTypes.Actionlib;

public class arm_mover_action : MonoBehaviour
{
    private static readonly string ServiceName = "/arm0/my_robot_arm_server";
    private static readonly Quaternion PickOrientation = Quaternion.Euler(90, 90, 0);


    public ArticulationBody[] jointArticulationBodies;
    private ROSConnection rc;

    // Start is called before the first frame update
    void Start()
    {
        //ROS�R�l�N�V�����̏���
        this.rc = ROSConnection.GetOrCreateInstance();

        this.rc.RegisterRosService<MoverServiceRequest, MoverServiceResponse>(ServiceName);
        Publish();

        // PoseManager�ɃA�N�Z�X
        //poseManager = pose.GetComponent<PoseManager>();
        
        // �N������(getUpJudge��True�ɕω�����������)ROS�Ƀ��N�G�X�g�𑗂�
        //this.ObserveEveryValueChanged(x => x.getUpJudge)
        //.Where(x => x).Subscribe(_ => GetUpPublish());
    }

    // Update is called once per frame
    void Update()
    {
        // �f�o�b�O�p�N������Ăяo��
        InputKeyNumber();
    }

    public void Publish()
    {
        var request = new MoverServiceRequest();

        var joints = new MyRobotArmMoveitJointsMsg();

        for (var i = 0; i < jointArticulationBodies.Length; i++)
        {
            joints.joints[i] = this.jointArticulationBodies[i].jointPosition[0];
        }
        request.joints_input = joints;

        this.rc.SendServiceMessage<MoverServiceResponse>(ServiceName, request, TrajectoryResponse);
    }

    void TrajectoryResponse(MoverServiceResponse response)
    {
        if (response.trajectory != null && response.trajectory.joint_trajectory.points.Length > 0)
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

    // �f�o�b�O�p�N������
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
