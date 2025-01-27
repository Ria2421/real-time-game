//---------------------------------------------------------------
// ��C�X�N���v�g [ Cannon.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/27
// Update:2025/01/27
//---------------------------------------------------------------
using DG.Tweening;
using RealTimeServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    //-------------------------------------------------------
    // �t�B�[���h

    /// <summary>
    /// �e�̑��x
    /// </summary>
    [SerializeField] private float bulletSpeed = 4.0f;

    /// <summary>
    /// �e�v���n�u
    /// </summary>
    [SerializeField] private GameObject bulletprefab;

    /// <summary>
    ///  ���ˈʒu
    /// </summary>
    [SerializeField] private Transform startPosition;

    /// <summary>
    /// �����ʒu
    /// </summary>
    [SerializeField] private Transform endPosition;

    //-------------------------------------------------------
    // ���\�b�h

    /// <summary>
    /// �e���ˏ���
    /// </summary>
    public void ShotBullet()
    {
        // �e�̐���
        var bulletObj = Instantiate(bulletprefab, startPosition.position, Quaternion.identity);

        // �e�̈ړ�
        bulletObj.transform.DOMove(endPosition.position, bulletSpeed).SetEase(Ease.Linear)
            .SetUpdate(UpdateType.Fixed, true).OnComplete(() =>
            {
                // �ړ�������ɏ���
                Destroy(bulletObj);
            });
    }
}
