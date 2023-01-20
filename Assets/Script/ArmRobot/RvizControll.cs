using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using JointStateMsg = RosMessageTypes.Sensor.JointStateMsg;

public class RvizControll : MonoBehaviour
{
    public ArticulationBody elbow;
    public ArticulationBody shoulder;
    public ArticulationBody wrist;
 
    private ArticulationBody[] joint;
    private ROSConnection ros;


    // Start is called before the first frame update
    void Start()
    {
        joint = new ArticulationBody[5];
        joint[0] = elbow;
        joint[1] = shoulder;
        joint[2] = wrist;

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<JointStateMsg>("/move_group/fake_controller_joint_states", Callback);
    }

    void Callback(JointStateMsg msg)
    {
        for (int i = 0; i < 3; i++)
        {
            ArticulationDrive aDrive = joint[i].xDrive;
            aDrive.target = Mathf.Rad2Deg * (float)msg.position[i];
            joint[i].xDrive = aDrive;
        }
    }
}