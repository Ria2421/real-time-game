//---------------------------------------------------------------
// 車生成 [ CarSpawn.cs ]
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
    // フィールド

    /// <summary>
    /// 始発点
    /// </summary>
    [SerializeField] private GameObject startObj;

    /// <summary>
    /// 終着点
    /// </summary>
    [SerializeField] private GameObject goalObj;

    /// <summary>
    /// 生成する車
    /// </summary>
    [SerializeField] private GameObject[] spawnObj;

    /// <summary>
    /// 生成角度Y
    /// </summary>
    [SerializeField] private float spawnAngleY;

    /// <summary>
    /// 最低移動秒数
    /// </summary>
    [SerializeField] private float minMoveSecond;

    /// <summary>
    /// 最高移動秒数
    /// </summary>
    [SerializeField] private float maxMoveSecond;

    //=====================================
    // メソッド

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnCar",0.5f, 1.5f);
    }

    // 車生成処理
    private void SpawnCar()
    {
        GameObject carObj = Instantiate(spawnObj[Random.Range(0, spawnObj.Length)], startObj.transform.position, Quaternion.Euler(0, spawnAngleY, 0));

        carObj.transform.DOLocalMove(goalObj.transform.position, Random.Range(minMoveSecond, maxMoveSecond));
    }
}
