using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEnabled : MonoBehaviour
{
    // �v���C���[�𔽉f���邽�߂̃��f��
    [SerializeField] private Renderer avaterModel;

    // �|�[�Y���ۑ�����Ă��郂�f��
    //[SerializeField] private Renderer robotModel;

    void Start()
    {
        avaterModel.enabled = false;
        //robotModel.enabled = false;
    }

}
