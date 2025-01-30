//---------------------------------------------------------------
// 霆顔函謌� [ CarSpawn.cs ]
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
    // 繝輔ぅ繝ｼ繝ｫ繝�

    /// <summary>
    /// 蟋狗匱轤ｹ
    /// </summary>
    [SerializeField] private GameObject startObj;

    /// <summary>
    /// 邨ら捩轤ｹ
    /// </summary>
    [SerializeField] private GameObject goalObj;

    /// <summary>
    /// 逕滓��縺吶ｋ霆�
    /// </summary>
    [SerializeField] private GameObject[] spawnObj;

    /// <summary>
    /// 逕滓��隗貞ｺｦY
    /// </summary>
    [SerializeField] private float spawnAngleY;

    /// <summary>
    /// 譛�菴守ｧｻ蜍慕ｧ呈焚
    /// </summary>
    [SerializeField] private float minMoveSecond;

    /// <summary>
    /// 譛�鬮倡ｧｻ蜍慕ｧ呈焚
    /// </summary>
    [SerializeField] private float maxMoveSecond;

    //=====================================
    // 繝｡繧ｽ繝��ラ

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnCar",0.5f, 1.5f);
    }

    // 霆顔函謌仙��逅�
    private void SpawnCar()
    {
        GameObject carObj = Instantiate(spawnObj[Random.Range(0, spawnObj.Length)], startObj.transform.position, Quaternion.Euler(0, spawnAngleY, 0));

        carObj.transform.DOLocalMove(goalObj.transform.position, Random.Range(minMoveSecond, maxMoveSecond));
    }
}
