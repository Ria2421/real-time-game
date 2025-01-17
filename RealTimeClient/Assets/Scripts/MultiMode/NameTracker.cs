//---------------------------------------------------------------
// ���[�U�[���Ǐ] [ NameTracker.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/16
// Update:2025/01/16
// �Q�lURL:https://tech.pjin.jp/blog/2017/07/14/unity_ugui_sync_rendermode/
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Vector3 offset = new Vector3(0, 1.5f, 0);

    //=====================================
    // ���\�b�h

    void Start()
    {
        myRectTfm = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (targetTfm == null) return;

        // �\���ʒu��Ǐ]�Ώ�+ �I�t�Z�b�g�̍��W�Ɉړ���������
        myRectTfm.position
            = RectTransformUtility.WorldToScreenPoint(Camera.main, targetTfm.position + offset);
    }

    /// <summary>
    /// �Ǐ]�Ώېݒ菈��
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        targetTfm = target;
    }
}
