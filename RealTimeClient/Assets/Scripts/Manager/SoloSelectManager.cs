//---------------------------------------------------------------
// �\���X�e�[�W�I���}�l�[�W���[ [ SoloSelectManager.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/11
// Update:2025/01/12
//---------------------------------------------------------------
using Cysharp.Net.Http;
using Grpc.Net.Client;
using KanKikuchi.AudioManager;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using Shared.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoloSelectManager : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    /// <summary>
    /// ���[�U�[���i�[�p
    /// </summary>
    [SerializeField] private Text[] nameTexts;

    /// <summary>
    /// �N���A�^�C���i�[�p
    /// </summary>
    [SerializeField] private Text[] clearTimeTexts;

    /// <summary>
    /// �����L���O���f���i�[�p
    /// </summary>
    [SerializeField] private RankingModel rankingModel;

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    async void Start()
    {
        //�Đ�����BGM�̖��O��S�Ď擾
        var currentBGMNames = BGMManager.Instance.GetCurrentAudioNames();

        if (currentBGMNames[0] != "MainBGM")
        {   // MainBGM���ĊJ
            BGMManager.Instance.Stop(BGMPath.TIME_ATTACK);
            BGMManager.Instance.Stop(BGMPath.MULTI_PLAY);
            BGMManager.Instance.Play(BGMPath.MAIN_BGM, 0.75f, 0, 1, true, true);
        }

        // �����L���O�f�[�^�̎擾
        List<RankingData> rankingDatas = await rankingModel.GetRankingAsync(1);

        // ��ʂɔ��f
        for (int i = 0; i < rankingDatas.Count; i++)
        {
            nameTexts[i].text = rankingDatas[i].UserName;   // ���O�i�[

            // �N���A�^�C�����e�L�X�g�ɔ��f
            float clearTIme = (float)rankingDatas[i].ClearTime / 1000.0f;
            string decNum = (clearTIme - (int)clearTIme).ToString(".000");
            clearTimeTexts[i].text = ((int)(clearTIme / 60)).ToString("00") + ":" + ((int)clearTIme % 60).ToString("00") + decNum;
            
        }
    }

    /// <summary>
    /// �v���C�{�^����������
    /// </summary>
    public void OnPlayButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // �\���v���C���[�h�J��
        SceneManager.LoadScene("04_SoloPlayScene");
    }

    /// <summary>
    /// ���j���[�{�^����������
    /// </summary>
    public void OnMenuButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // ���j���[���[�h�J��
        SceneManager.LoadScene("02_MenuScene");
    }
}