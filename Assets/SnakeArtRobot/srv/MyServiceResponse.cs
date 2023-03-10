//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.SnakeArtRobot
{
    [Serializable]
    public class MyServiceResponse : Message
    {
        public const string k_RosMessageName = "snake_art_robot/MyService";
        public override string RosMessageName => k_RosMessageName;

        public string res;

        public MyServiceResponse()
        {
            this.res = "";
        }

        public MyServiceResponse(string res)
        {
            this.res = res;
        }

        public static MyServiceResponse Deserialize(MessageDeserializer deserializer) => new MyServiceResponse(deserializer);

        private MyServiceResponse(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.res);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.res);
        }

        public override string ToString()
        {
            return "MyServiceResponse: " +
            "\nres: " + res.ToString();
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
