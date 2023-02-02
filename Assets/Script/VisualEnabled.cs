using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEnabled : MonoBehaviour
{
    // プレイヤーを反映するためのモデル
    [SerializeField] private Renderer avaterModel;

    // ポーズが保存されているモデル
    //[SerializeField] private Renderer robotModel;

    void Start()
    {
        avaterModel.enabled = false;
        //robotModel.enabled = false;
    }

}
