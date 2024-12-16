//---------------------------------------------------------------
// �Ԑ��� [ CarSpawn.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/16
// Update:2024/12/16
//---------------------------------------------------------------
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarSpawn : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    /// <summary>
    /// �n���_
    /// </summary>
    [SerializeField] private GameObject startObj;

    /// <summary>
    /// �I���_
    /// </summary>
    [SerializeField] private GameObject goalObj;

    /// <summary>
    /// ���������
    /// </summary>
    [SerializeField] private GameObject[] spawnObj;

    /// <summary>
    /// �����p�xY
    /// </summary>
    [SerializeField] private float spawnAngleY;

    /// <summary>
    /// �Œ�ړ��b��
    /// </summary>
    [SerializeField] private float minMoveSecond;

    /// <summary>
    /// �ō��ړ��b��
    /// </summary>
    [SerializeField] private float maxMoveSecond;

    //=====================================
    // ���\�b�h

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnCar",0.5f, 1.5f);
    }

    // �Ԑ�������
    private void SpawnCar()
    {
        GameObject carObj = Instantiate(spawnObj[Random.Range(0, spawnObj.Length)], startObj.transform.position, Quaternion.Euler(0, spawnAngleY, 0));

        carObj.transform.DOLocalMove(goalObj.transform.position, Random.Range(minMoveSecond, maxMoveSecond));
    }
}
