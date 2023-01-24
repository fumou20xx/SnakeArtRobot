using System;
using System.Collections;
using RosMessageTypes.Geometry;
using RosMessageTypes.SnakeArtRobot.Arm;
//using RosMessageTypes.MyRobotArmService;
using RosMessageTypes.Trajectory;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class TrajectoryPlannerMulti : MonoBehaviour
{
    private static readonly string ServiceName = "/robot_arm_server";
    private static readonly Quaternion PickOrientation = Quaternion.Euler(90, 90, 0);

    public ArticulationBody[] jointArticulationBodies;
    private ROSConnection rc;

    void Start()
    {
        this.rc = ROSConnection.GetOrCreateInstance();

        this.rc.RegisterRosService<ArmMoverServiceRequest, ArmMoverServiceResponse>(ServiceName);
        Publish();
    }

    public void Publish()
    {
        var request = new ArmMoverServiceRequest();

        var joints = new RobotArmMoveitJointsMsg();

        for(var i = 0; i < jointArticulationBodies.Length; i++)
        {
            joints.joints[i] = this.jointArticulationBodies[i].jointPosition[0];
        }
        request.joints_input = joints;

        this.rc.SendServiceMessage<ArmMoverServiceResponse>(ServiceName, request, TrajectoryResponse);
    }

    private void TrajectoryResponse(ArmMoverServiceResponse response)
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

    private IEnumerator ExecuteTrajectories(ArmMoverServiceResponse response)
    {
        foreach (var t in response.trajectory.joint_trajectory.points)
        {
            float[] result = new float[8];
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

}