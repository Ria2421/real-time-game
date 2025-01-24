//---------------------------------------------------------------
// ���[�U�[���Ǐ] [ NameTracker.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/16
// Update:2025/01/16
// �Q�lURL:https://tech.pjin.jp/blog/2017/07/14/unity_ugui_sync_rendermode/
//---------------------------------------------------------------
using DavidJalbert;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameTracker : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    /// <summary>
    /// �Ǐ]�Ώ�
    /// </summary>
    private Transform targetTfm;

    /// <summary>
    /// �\��UI��RectTransform
    /// </summary>
    private RectTransform myRectTfm;

    /// <summary>
    /// �\���I�t�Z�b�g
    /// </summary>
    private Vector3 offset;

    /// <summary>
    /// ���O�t�H���g�T�C�Y
    /// </summary>
    private int fontSize;

    /// <summary>
    /// ���Վ��_���̃t�H���g�T�C�Y
    /// </summary>
    private const int topFontSize = 120;

    /// <summary>
    /// ��O�Ҏ��_���̃t�H���g�T�C�Y
    /// </summary>
    private const int thirdFontSize = 85;

    /// <summary>
    /// ���C���J����
    /// </summary>
    [SerializeField] private Transform cameraTrs;

    /// <summary>
    /// �J�����X�N���v�g
    /// </summary>
    [SerializeField] private TinyCarCamera tinyCarCamera;

    /// <summary>
    /// name�e�L�X�g
    /// </summary>
    [SerializeField] private Text nameText;

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        myRectTfm = GetComponent<RectTransform>();
    }

    /// <summary>
    /// ����X�V����
    /// </summary>
    void FixedUpdate()
    {
        if (targetTfm == null) return;

        //++ �\���ʒu��Ǐ]�Ώ�+ �I�t�Z�b�g�̍��W�Ɉړ���������
        myRectTfm.position = targetTfm.position + offset;

        myRectTfm.rotation = cameraTrs.rotation; 

        // �J�������[�h�ɂ���ăt�H���g�T�C�Y��ύX����
        if(tinyCarCamera.viewMode == TinyCarCamera.CAMERA_MODE.ThirdPerson)
        {
            fontSize = thirdFontSize;
        }
        else if (tinyCarCamera.viewMode == TinyCarCamera.CAMERA_MODE.TopDown)
        {
            fontSize = topFontSize;
        }

        nameText.fontSize = fontSize;
    }

    /// <summary>
    /// �Ǐ]�Ώېݒ菈��
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target,int no)
    {
        targetTfm = target;

        // �I�t�Z�b�g�����̕ύX
        if(no == 1)
        {
            offset = new Vector3(0, 2.3f, 0);
        }else if(no == 2)
        {
            offset = new Vector3(0, 3.5f, 0);
        }
    }
}
