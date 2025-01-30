//---------------------------------------------------------------
// �\���X�e�[�W�I���}�l�[�W���[ [ SoloSelectManager.cs ]
// Author:Kenta Nakamoto
// Data:2025/01/11
// Update:2025/01/30
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
    /// �擾�S�[�X�g�f�[�^
    /// </summary>
    private string getGhostData;

    /// <summary>
    /// �v���C�X�e�[�WID
    /// </summary>
    private int playStageID = 1;

    /// <summary>
    /// �e�X�e�[�W�̃����L���O�����擾
    /// </summary>
    private List<List<RankingData>> stageRnakings = new List<List<RankingData>>();

    /// <summary>
    /// �ő�X�e�[�W��
    /// </summary>
    [SerializeField] private int maxStage;

    /// <summary>
    /// ���[�U�[���i�[�p
    /// </summary>
    [SerializeField] private Text[] nameTexts;

    /// <summary>
    /// �N���A�^�C���i�[�p
    /// </summary>
    [SerializeField] private Text[] clearTimeTexts;

    /// <summary>
    /// �X�e�[�W�摜�i�[�p
    /// </summary>
    [SerializeField] private Sprite[] stageSprits;

    /// <summary>
    /// �X�e�[�W�摜
    /// </summary>
    [SerializeField] private Image stageImage;

    /// <summary>
    /// �����L���O���f���i�[�p
    /// </summary>
    [SerializeField] private RankingModel rankingModel;

    /// <summary>
    /// �S�[�X�g�I���{�^��
    /// </summary>
    [SerializeField] private GameObject onGhostButton;

    /// <summary>
    /// �S�[�X�g�I�t�{�^��
    /// </summary>
    [SerializeField] private GameObject offGhostButton;

    /// <summary>
    /// �l�N�X�g�X�e�[�W�{�^��
    /// </summary>
    [SerializeField] private GameObject nextButton;

    /// <summary>
    /// �o�b�N�X�e�[�W�{�^��
    /// </summary>
    [SerializeField] private GameObject backButton;

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

        for (int i=0;i < maxStage; i++)
        {   // �X�e�[�W�����̃����L���O��񃊃X�g�𐶐�
            stageRnakings.Add (new List<RankingData>());
        }

        // �����L���O�f�[�^�̎擾 (���݂̓X�e�[�W1�ɌŒ�)
        stageRnakings[0] = await rankingModel.GetRankingAsync(1);

        // �����L���O1�ʂ̃S�[�X�g�f�[�^���擾
        UserModel.Instance.GhostData = "";  // ���Z�b�g
        UserModel.Instance.GhostData = stageRnakings[0][0].GhostData;
        getGhostData = stageRnakings[0][0].GhostData;

        // ��ʂɔ��f
        for (int i = 0; i < stageRnakings[0].Count; i++)
        {
            nameTexts[i].text = stageRnakings[0][i].UserName;   // ���O���i�[

            // �N���A�^�C�����e�L�X�g�ɔ��f
            float clearTIme = (float)stageRnakings[0][i].ClearTime / 1000.0f;
            string decNum = (clearTIme - (int)clearTIme).ToString(".000");
            clearTimeTexts[i].text = ((int)(clearTIme / 60)).ToString("00") + ":" + ((int)clearTIme % 60).ToString("00") + decNum;
        }
    }

    /// <summary>
    /// �X�e�[�W�؂�ւ�����
    /// </summary>
    private async void SelectStageButton()
    {
        //--- �{�^���̗L���ؑ�
        if(playStageID == 1)
        {
            nextButton.GetComponent<Button>().interactable = true;
            backButton.GetComponent<Button>().interactable = false;
        }else if(playStageID == maxStage) 
        {
            nextButton.GetComponent<Button>().interactable = false;
            backButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            nextButton.GetComponent<Button>().interactable = true;
            backButton.GetComponent<Button>().interactable = true;
        }

        //--- �I���X�e�[�W�̃����L���O����\��

        // ���Ƀ����L���O�����擾���Ă��邩���f
        if (stageRnakings[playStageID - 1].Count == 0)
        {
            // �I�������X�e�[�W�����L���O�f�[�^�̎擾
            stageRnakings[playStageID - 1] = await rankingModel.GetRankingAsync(playStageID);
        }

        // �S�[�X�g�f�[�^�̐؂�ւ�
        UserModel.Instance.GhostData = "";  // ���Z�b�g
        UserModel.Instance.GhostData = stageRnakings[playStageID - 1][0].GhostData;
        getGhostData = stageRnakings[playStageID - 1][0].GhostData;

        // ��ʂɔ��f
        for (int i = 0; i < stageRnakings[playStageID - 1].Count; i++)
        {
            nameTexts[i].text = stageRnakings[playStageID - 1][i].UserName;   // ���O���i�[

            // �N���A�^�C�����e�L�X�g�ɔ��f
            float clearTIme = (float)stageRnakings[playStageID - 1][i].ClearTime / 1000.0f;
            string decNum = (clearTIme - (int)clearTIme).ToString(".000");
            clearTimeTexts[i].text = ((int)(clearTIme / 60)).ToString("00") + ":" + ((int)clearTIme % 60).ToString("00") + decNum;
        }

        // �摜�؂�ւ�
        stageImage.sprite = stageSprits[playStageID - 1];
    }

    /// <summary>
    /// �X�e�[�W�I���{�^�� (����)
    /// </summary>
    public void OnNextButton()
    {
        playStageID++;
        if (playStageID >= maxStage) playStageID = maxStage;

        SelectStageButton();
    }

    /// <summary>
    /// �X�e�[�W�I���{�^�� (�O��)
    /// </summary>
    public void OnBackButton()
    {
        playStageID--;
        if(playStageID <= 1) playStageID = 1;

        SelectStageButton();
    }

    /// <summary>
    /// �v���C�{�^����������
    /// </summary>
    public void OnPlayButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // �\���v���C���[�h�J��
        Initiate.DoneFading();
        Initiate.Fade(playStageID.ToString() + "_SoloPlayScene", Color.white, 2.5f);
    }

    /// <summary>
    /// ���j���[�{�^����������
    /// </summary>
    public void OnMenuButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // ���j���[���[�h�J��
        Initiate.DoneFading();
        Initiate.Fade("2_MenuScene", Color.white, 2.5f);
    }

    /// <summary>
    /// �S�[�X�g�I���{�^��
    /// </summary>
    public void OnGhostPlayButton()
    {
        UserModel.Instance.GhostData = getGhostData;

        onGhostButton.SetActive(false);
        offGhostButton.SetActive(true);
    }

    /// <summary>
    /// �S�[�X�g�I�t�{�^��
    /// </summary>
    public void OnGhostNotButton()
    {
        UserModel.Instance.GhostData = "";

        offGhostButton.SetActive(false);
        onGhostButton.SetActive(true);
    }
}
