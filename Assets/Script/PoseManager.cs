using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.rfilkov.kinect;


namespace com.rfilkov.components
{
    /// <summary>
    /// 静的ポーズ検出器は、ユーザーのポーズが定義済みの静的モデルのポーズと一致するかどうかを確認します。
    /// </summary>
    public class PoseManager : MonoBehaviour
    {
        [Tooltip("User avatar model, who needs to reach the target pose.")]
        // ターゲット ポーズに到達する必要があるユーザー アバター モデル。
        public PoseModelHelper avatarModel;

        [Tooltip("Model in pose that need to be reached by the user.")]
        // ユーザーが到達する必要があるポーズのモデル。
        //public PoseModelHelper poseModel;

        public List<PoseModelHelper> poseModelList;

        [Tooltip("List of joints to compare.")]
        // 比較するジョイントのリスト。
        public List<KinectInterop.JointType> poseJoints = new List<KinectInterop.JointType>();

        [Tooltip("Allowed delay in pose match, in seconds. 0 means no delay allowed.")]
        // ポーズ マッチの許容遅延 (秒単位)。 0 は、遅延が許可されないことを意味します。
        [Range(0f, 10f)]
        public float delayAllowed = 2f;

        [Tooltip("Time between pose-match checks, in seconds. 0 means check each frame.")]
        // ポーズ一致チェック間の時間 (秒単位)。 0 は各フレームをチェックすることを意味します。
        [Range(0f, 1f)]
        public float timeBetweenChecks = 0.1f;

        [Tooltip("Threshold, above which we consider the pose is matched.")]
        // それを超えるとポーズが一致すると見なされるしきい値。
        [Range(0.5f, 1f)]
        public float matchThreshold = 0.7f;

        [Tooltip("GUI-Text to display information messages.")]
        // 情報メッセージを表示する GUI テキスト。
        public UnityEngine.UI.Text infoText;

        // ポーズが合っているかどうか
        private bool bPoseMatched = false;
        // 一致率 (0 から 1 の間)
        private float fMatchPercent = 0f;
        // ベストマッチングのポーズタイム
        private float fMatchPoseTime = 0f;

        // 回転値の初期化
        private Quaternion initialAvatarRotation = Quaternion.identity;
        private Quaternion initialPoseRotation = Quaternion.identity;

        // アバター コントローラへの参照
        private AvatarController avatarController = null;

        // コメントを外してデバッグ情報を取得
        private StringBuilder sbDebug = null; // new StringBuilder();

        // 起きたかどうか判定　起きたらTrue
        private bool getUpPose = false;

        // 保存された各ポーズのデータ
        public class PoseModelData
        {
            public float fTime;
            public Vector3[] avBoneDirs;
        }

        // 保存ポーズデータ一覧
        private List<PoseModelData> alSavedPoses = new List<PoseModelData>();

        // 現在のアバターのポーズ
        private PoseModelData poseAvatar = new PoseModelData();

        // モデルのポーズが最後に保存された時刻
        private float lastPoseSavedTime = 0f;

        /// <summary>
        /// 起床判定
        /// </summary>
        /// <returns></returns>
        public bool GetUpJudge()
        {
            return getUpPose;
        }

        /// <summary>
        /// ターゲット ポーズが一致しているかどうかを判断します。
        /// </summary>
        /// <returns><c>true</c> ターゲットポーズが一致する場合; それ以外, <c>false</c>.</returns>
        public bool IsPoseMatched()
        {
            return bPoseMatched;
        }

        /// <summary>
        /// ポーズの一致率を取得します。
        /// </summary>
        /// <returns>一致率 (0 ～ 1 の値)</returns>
        public float GetMatchPercent()
        {
            return fMatchPercent;
        }


        /// <summary>
        /// 最適なポーズの時間を取得します。
        /// </summary>
        /// <returns>ベストマッチポーズの時間</returns>
        public float GetMatchPoseTime()
        {
            return fMatchPoseTime;
        }


        /// <summary>
        /// 最終チェック時刻を取得します。
        /// </summary>
        /// <returns>最後のチェック時間</returns>
        public float GetPoseCheckTime()
        {
            return lastPoseSavedTime;
        }


        private void Awake()
        {
            if(avatarModel)
            {
                initialAvatarRotation = avatarModel.transform.rotation;
                avatarController = avatarModel.gameObject.GetComponent<AvatarController>();
            }

            /*if(poseModel)
            {
                initialPoseRotation = poseModel.transform.rotation;
            }*/

            initialPoseRotation = Quaternion.identity;
        }

        private void Start()
        {
            foreach (var item in poseModelList)
            {
                AddCurrentPoseToSaved(item, Time.realtimeSinceStartup, false);
                //Destroy(item.gameObject);
            }
            //poseModelList.Clear();

            
        }


        void Update()
        {
            KinectManager kinectManager = KinectManager.Instance;

            // ミラー状態を取得する
            bool isMirrored = avatarController ? avatarController.mirroredMovement : true;  // true by default

            // 現在の時刻
            float fCurrentTime = Time.realtimeSinceStartup;

            // 必要に応じて、モデルのポーズを保存します
            /*if ((fCurrentTime - lastPoseSavedTime) >= timeBetweenChecks)
            {
                lastPoseSavedTime = fCurrentTime;

                // 古いポーズを削除して
                // 現在のポーズを保存
                RemoveOldSavedPoses(fCurrentTime);
                AddCurrentPoseToSaved(poseModel, fCurrentTime, isMirrored);
            }*/

            if(kinectManager != null && kinectManager.IsInitialized())
            {
                if (avatarModel != null && avatarController && kinectManager.IsUserTracked(avatarController.playerId))
                {
                    // 現在のアバターのポーズを取得する
                    GetAvatarPose(fCurrentTime, isMirrored);

                    // 違いを得る
                    GetPoseDifference(isMirrored);

                    if (infoText != null)
                    {
                        //string sPoseMessage = string.Format("Pose match: {0:F0}% {1:F1}s ago {2}", fMatchPercent * 100f, Time.realtimeSinceStartup - fMatchPoseTime,
                        //                                    (bPoseMatched ? "- Matched" : ""));
                        string sPoseMessage = string.Format("Pose match: {0:F0}% {1}", fMatchPercent * 100f, (bPoseMatched ? "- Matched" : ""));
                        if (sbDebug != null)
                            sPoseMessage += sbDebug.ToString();
                        infoText.text = sPoseMessage;

                        // 起きたかどうかの判定
                        if (getUpPose == false && fMatchPercent < 0.4f)
                        {
                            getUpPose = true;
                            Debug.Log(getUpPose + "：起きた");
                        }

                        // 寝たかどうかの判定
                        if (getUpPose == true && fMatchPercent > 0.6f)
                        {
                            getUpPose = false;
                            Debug.Log(getUpPose + "：寝た");
                        } 
                    }
                }
                else
                {
                    // ユーザーが見つからない場合
                    fMatchPercent = 0f;
                    fMatchPoseTime = 0f;
                    bPoseMatched = false;

                    if (infoText != null)
                    {
                        infoText.text = "Try to follow the model pose.";
                    }
                }
            }
        }

        // delayAllowed より古い保存済みポーズをリストから削除します
        private void RemoveOldSavedPoses(float fCurrentTime)
        {
            for (int i = alSavedPoses.Count - 1; i >= 0; i--)
            {
                if ((fCurrentTime - alSavedPoses[i].fTime) >= delayAllowed)
                {
                    alSavedPoses.RemoveAt(i);
                }
            }
        }

        // 保存されたポーズにposeModelの現在のポーズを追加します
        private void AddCurrentPoseToSaved(PoseModelHelper model, float fCurrentTime, bool isMirrored)
        {
            KinectManager kinectManager = KinectManager.Instance;
            if (kinectManager == null || model == null || poseJoints == null)
                return;

            PoseModelData pose = new PoseModelData();
            pose.fTime = fCurrentTime;
            pose.avBoneDirs = new Vector3[poseJoints.Count];

            // save model rotation
            // モデルの回転を保存
            Quaternion poseModelRotation = model.transform.rotation;

            if(avatarController)
            {
                ulong avatarUserId = avatarController.playerId;
                bool isAvatarMirrored = avatarController.mirroredMovement;

                Quaternion userRotation = kinectManager.GetUserOrientation(avatarUserId, !isAvatarMirrored);
                model.transform.rotation = initialPoseRotation * userRotation;

            }

            int jointCount = kinectManager.GetJointCount();
            for (int i = 0; i < poseJoints.Count; i++)
            {
                KinectInterop.JointType joint = poseJoints[i];
                KinectInterop.JointType nextJoint = kinectManager.GetNextJoint(joint);

                if (nextJoint != joint && (int)nextJoint >= 0 && (int)nextJoint < jointCount)
                {
                    Transform poseTransform1 = model.GetBoneTransform(model.GetBoneIndexByJoint(joint, isMirrored));
                    Transform poseTransform2 = model.GetBoneTransform(model.GetBoneIndexByJoint(nextJoint, isMirrored));

                    if (poseTransform1 != null && poseTransform2 != null)
                    {
                        pose.avBoneDirs[i] = (poseTransform2.position - poseTransform1.position).normalized;
                    }
                }
            }

            // add pose to the list
            // ポーズをリストに追加
            alSavedPoses.Add(pose);

            // restore model rotation
            // モデルの回転を復元する
            model.transform.rotation = poseModelRotation;
        }


        // gets the current avatar pose
        // 現在のアバターのポーズを取得します
        private void GetAvatarPose(float fCurrentTime, bool isMirrored)
        {
            KinectManager kinectManager = KinectManager.Instance;
            if (kinectManager == null || avatarModel == null || poseJoints == null)
                return;

            poseAvatar.fTime = fCurrentTime;
            if (poseAvatar.avBoneDirs == null)
            {
                poseAvatar.avBoneDirs = new Vector3[poseJoints.Count];
            }

            for (int i = 0; i < poseJoints.Count; i++)
            {
                KinectInterop.JointType joint = poseJoints[i];
                KinectInterop.JointType nextJoint = kinectManager.GetNextJoint(joint);

                int jointCount = kinectManager.GetJointCount();
                if (nextJoint != joint && (int)nextJoint >= 0 && (int)nextJoint < jointCount)
                {
                    Transform avatarTransform1 = avatarModel.GetBoneTransform(avatarModel.GetBoneIndexByJoint(joint, isMirrored));
                    Transform avatarTransform2 = avatarModel.GetBoneTransform(avatarModel.GetBoneIndexByJoint(nextJoint, isMirrored));

                    if (avatarTransform1 != null && avatarTransform2 != null)
                    {
                        poseAvatar.avBoneDirs[i] = (avatarTransform2.position - avatarTransform1.position).normalized;
                    }
                }
            }
        }


        // gets the difference between the avatar pose and the list of saved poses
        // アバターのポーズと保存されたポーズのリストの違いを取得します
        private void GetPoseDifference(bool isMirrored)
        {
            // by-default values
            bPoseMatched = false;
            fMatchPercent = 0f;
            fMatchPoseTime = 0f;

            KinectManager kinectManager = KinectManager.Instance;
            if (poseJoints == null || poseAvatar.avBoneDirs == null)
                return;

            if (sbDebug != null)
            {
                sbDebug.Clear();
                sbDebug.AppendLine();
            }

            // check the difference with saved poses, starting from the last one
            // 前回のポーズから保存したポーズとの違いを確認
            for (int p = alSavedPoses.Count - 1; p >= 0; p--)
            {
                float fAngleDiff = 0f;
                float fMaxDiff = 0f;

                PoseModelData poseModel = alSavedPoses[p];
                for (int i = 0; i < poseJoints.Count; i++)
                {
                    Vector3 vPoseBone = poseModel.avBoneDirs[i];
                    Vector3 vAvatarBone = poseAvatar.avBoneDirs[i];

                    if (vPoseBone == Vector3.zero || vAvatarBone == Vector3.zero)
                        continue;

                    float fDiff = Vector3.Angle(vPoseBone, vAvatarBone);
                    if (fDiff > 90f)
                        fDiff = 90f;

                    fAngleDiff += fDiff;
                    fMaxDiff += 90f;  // we assume the max diff could be 90 
                                      // 最大差分は 90 度であると仮定します

                    if (sbDebug != null)
                    {
                        sbDebug.AppendFormat("SP: {0}, {1} - angle: {2:F0}, match: {3:F0}%", p, poseJoints[i], fDiff, (1f - fDiff / 90f) * 100f);
                        sbDebug.AppendLine();
                    }
                }

                float fPoseMatch = fMaxDiff > 0f ? (1f - fAngleDiff / fMaxDiff) : 0f;
                if (fPoseMatch > fMatchPercent)
                {
                    fMatchPercent = fPoseMatch;
                    fMatchPoseTime = poseModel.fTime;
                    bPoseMatched = (fMatchPercent >= matchThreshold);
                }
            }
        }

    }
}
