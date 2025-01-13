//---------------------------------------------------------------
// �\���}�l�[�W���[ [ SoloManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2024/12/10
//---------------------------------------------------------------
using DavidJalbert;
using DG.Tweening;
using KanKikuchi.AudioManager;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoloManager : MonoBehaviour
{
    //===================================
    // �t�B�[���h

    /// <summary>
    /// �X�e�[�WNo
    /// </summary>
    [SerializeField] private int stageID; 

    /// <summary>
    /// �����b�v��
    /// </summary>
    private int currentRapNum = 1;

    /// <summary>
    /// ���Ԍv���p
    /// </summary>
    private float timer;

    /// <summary>
    /// �S�[���^�C���ۑ�
    /// </summary>
    private int goalTime = 0;

    /// <summary>
    /// �v���t���O
    /// </summary>
    private bool isCount = false;

    /// <summary>
    /// ��{���͊Ǘ��I�u�W�F
    /// </summary>
    [SerializeField] private GameObject standardInput;

    /// <summary>
    /// ���o�C�����͊Ǘ��I�u�W�F
    /// </summary>
    [SerializeField] private GameObject mobileInput;

    /// <summary>
    /// �ԑ̔��j�}�l�[�W���[
    /// </summary>
    [SerializeField] private TinyCarExplosiveBody boomManager;

    /// <summary>
    /// �ő僉�b�v��
    /// </summary>
    [SerializeField] private int maxRapNum = 0;

    /// <summary>
    /// ���U���g�p�l��
    /// </summary>
    [SerializeField] private GameObject resultPanel;

    /// <summary>
    /// �v���^�C�}�[�p�l��
    /// </summary>
    [SerializeField] private GameObject timerPanel;

    /// <summary>
    /// �J�E���g�_�E���I�u�W�F
    /// </summary>
    [SerializeField] private GameObject countDownObj;

    /// <summary>
    /// �J�E���g�_�E���p�X�v���C�g
    /// </summary>
    [SerializeField] private Sprite[] countDownSprits;

    /// <summary>
    /// ���Ԍv���p�e�L�X�g
    /// </summary>
    [SerializeField] private Text timerText;

    /// <summary>
    /// ���b�v���\���p�e�L�X�g
    /// </summary>
    [SerializeField] private Text rapText;

    /// <summary>
    /// �����L���O���f���i�[�p
    /// </summary>
    [SerializeField] private RankingModel rankingModel;

    /// <summary>
    /// �V�L�^�摜
    /// </summary>
    [SerializeField] private GameObject newRecordObj;

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        // ���C��BGM�ꎞ��~
        BGMManager.Instance.Pause(BGMPath.MAIN_BGM);
        BGMManager.Instance.Play(BGMPath.TIME_ATTACK);

        // �t���O������
        isCount = false;

        // �J�E���g�_�E���J�n
        Debug.Log("�J�E���g�_�E��");
        StartCoroutine("StartCount");
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    void Update()
    {
        if (!isCount) return;

        // timer�𗘗p���Čo�ߎ��Ԃ��v���E�\��
        timer += Time.deltaTime;
        DisplayTime(timer);
    }

    /// <summary>
    /// ���b�v�����Z����
    /// </summary>
    public async void AddRapCnt()
    {
        currentRapNum++;    // ���b�v�����Z

        if(currentRapNum == maxRapNum + 1)
        {
            // SE�Đ�
            SEManager.Instance.Play(SEPath.GOAL);

            // �I������
            isCount = false;                // �^�C�}�[�X�g�b�v
            Invoke("BoomCar", 1);           // �ԑ̔��j
            resultPanel.SetActive(true);    // ���U���g�\��
            mobileInput.SetActive(false);   // ���o�C��UI��\��
            
            // �^�C�}�[�ړ�����
            var sequence = DOTween.Sequence();
            sequence.Append(timerPanel.transform.DOScale(1.7f, 0.7f));
            sequence.Append(timerPanel.transform.DOLocalMove(new Vector3(0,-25,0), 0.5f));
            sequence.Play();

            timer = (float)Math.Round(timer, 3, MidpointRounding.AwayFromZero);
            goalTime = (int)(timer * 1000);

            UserModel userModel = UserModel.Instance;

            // �L�^�o�^����
            bool isRegist = await rankingModel.RegistClearTimeAsync(stageID, userModel.UserId, goalTime);

            if (isRegist)
            {   // newRecord�\��
                SEManager.Instance.Play(SEPath.NEW_RECORD);
                newRecordObj.SetActive(true);
                newRecordObj.transform.DOScale(1.3f, 1.5f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
            }

            Debug.Log("goal : " + goalTime.ToString() + "m/sec");
        }
        else
        {
            Debug.Log("�����b�v��" + currentRapNum);
            rapText.text = currentRapNum.ToString() + " / " + maxRapNum.ToString();
        }
    }

    /// <summary>
    /// �������Ԃ��X�V����[��:�b]�ŕ\������
    /// </summary>
    private void DisplayTime(float limitTime)
    {
        // �����Ŏ󂯎�����l��[��:�b]�ɕϊ����ĕ\������
        // ToString("00")�Ń[���v���[�X�t�H���_�[���āA�P���̂Ƃ��͓���0������
        string decNum = (limitTime - (int)limitTime).ToString(".000");
        timerText.text = ((int)(limitTime / 60)).ToString("00") + ":" + ((int)limitTime % 60).ToString("00") + decNum;
    }

    /// <summary>
    /// �J�E���g�_�E����\������
    /// </summary>
    /// <param name="obj">��\���ɂ������I�u�W�F</param>
    private void HiddenCount()
    {
        countDownObj.SetActive(false);
    }

    /// <summary>
    /// �ԑ̔��j����
    /// </summary>
    private void BoomCar()
    {
        boomManager.explode();
    }

    /// <summary>
    /// �J�E���g�_�E������
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCount()
    {
        for (int i = 0; i < 4; i++)
        {
            countDownObj.GetComponent<Image>().sprite = countDownSprits[i];

            if (i == 3)
            {
                SEManager.Instance.Play(SEPath.START);
                // �v���J�n�E����\
                countDownObj.GetComponent<Image>().sprite = countDownSprits[i];
                standardInput.SetActive(true);
                mobileInput.SetActive(true);
                isCount = true;

                // �J�E���g��\��
                Invoke("HiddenCount", 0.6f);
            }
            else
            {
                SEManager.Instance.Play(SEPath.COUNT);
                // 1�b�҂��ăR���[�`�����f
                yield return new WaitForSeconds(0.9f);
            }
        }
    }

    /// <summary>
    /// ���j���[�J�ڏ���
    /// </summary>
    public void OnBackMenu()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        SceneManager.LoadScene("02_MenuScene");
    }

    /// <summary>
    /// �X�e�[�W�I��J�ڏ���
    /// </summary>
    public void OnBackSelect()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        SceneManager.LoadScene("03_SoloSelectScene");
    }

    /// <summary>
    /// ���g���C�J�ڏ���
    /// </summary>
    public void OnRetryButton() 
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
