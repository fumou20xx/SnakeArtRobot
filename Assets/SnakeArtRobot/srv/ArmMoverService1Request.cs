//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.SnakeArtRobot.Arm
{
    [Serializable]
    public class ArmMoverService1Request : Message
    {
        public const string k_RosMessageName = "snake_art_robot/ArmMoverService1";
        public override string RosMessageName => k_RosMessageName;

        public RobotArmMoveitJointsMsg joints_input;

        public ArmMoverService1Request()
        {
            this.joints_input = new RobotArmMoveitJointsMsg();
        }

        public ArmMoverService1Request(RobotArmMoveitJointsMsg joints_input)
        {
            this.joints_input = joints_input;
        }

        public static ArmMoverService1Request Deserialize(MessageDeserializer deserializer) => new ArmMoverService1Request(deserializer);

        private ArmMoverService1Request(MessageDeserializer deserializer)
        {
            this.joints_input = RobotArmMoveitJointsMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.joints_input);
        }

        public override string ToString()
        {
            return "ArmMoverService1Request: " +
            "\njoints_input: " + joints_input.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
