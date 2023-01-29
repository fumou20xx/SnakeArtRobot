//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.SnakeArtRobot.Arm
{
    [Serializable]
    public class ArmMoverService2Response : Message
    {
        public const string k_RosMessageName = "snake_art_robot/ArmMoverService2";
        public override string RosMessageName => k_RosMessageName;

        public Moveit.RobotTrajectoryMsg trajectory;

        public ArmMoverService2Response()
        {
            this.trajectory = new Moveit.RobotTrajectoryMsg();
        }

        public ArmMoverService2Response(Moveit.RobotTrajectoryMsg trajectory)
        {
            this.trajectory = trajectory;
        }

        public static ArmMoverService2Response Deserialize(MessageDeserializer deserializer) => new ArmMoverService2Response(deserializer);

        private ArmMoverService2Response(MessageDeserializer deserializer)
        {
            this.trajectory = Moveit.RobotTrajectoryMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.trajectory);
        }

        public override string ToString()
        {
            return "ArmMoverService2Response: " +
            "\ntrajectory: " + trajectory.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize, MessageSubtopic.Response);
        }
    }
}
