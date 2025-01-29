//---------------------------------------------------------------
// �\���}�l�[�W���[ [ SoloManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2025/01/27
//---------------------------------------------------------------
using DavidJalbert;
using DG.Tweening;
using KanKikuchi.AudioManager;
using Newtonsoft.Json;
using Shared.Interfaces.StreamingHubs;
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

    [Header("===== StageOption =====")]

    /// <summary>
    /// �X�e�[�WNo
    /// </summary>
    [SerializeField] private int stageID;

    /// <summary>
    /// �ő僉�b�v��
    /// </summary>
    [SerializeField] private int maxRapNum = 0;

    /// <summary>
    /// �S�[�X�g�f�[�^�L�^�Ԋu
    /// </summary>
    [SerializeField] private float saveSpeed;

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
    /// �S�[�X�g�f�[�^���X�g
    /// </summary>
    private List<GhostData> ghostList = new List<GhostData>();

    /// <summary>
    /// �Đ��S�[�X�g�f�[�^
    /// </summary>
    private List<GhostData> playGhost = new List<GhostData>();

    /// <summary>
    /// �S�[�X�g�f�[�^�J�E���g
    /// </summary>
    private int ghostCnt = 0;

    /// <summary>
    /// �S�[�X�g�\���␳���W
    /// </summary>
    private Vector3 ghostCorrection;

    [Space(25)]
    [Header("===== DataObject =====")]

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
    /// �����L���O���f���i�[�p
    /// </summary>
    [SerializeField] private RankingModel rankingModel;

    /// <summary>
    /// �ʒu���擾�I�u�W�F
    /// </summary>
    [SerializeField] private Transform visualObj;

    /// <summary>
    /// �^�C���p�擾�I�u�W�F
    /// </summary>
    [SerializeField] private Transform wheelRot;

    /// <summary>
    /// �S�[�X�g�ԃI�u�W�F
    /// </summary>
    [SerializeField] private GameObject ghostCarObj;

    /// <summary>
    /// �S�[�X�g�Ԉʒu���
    /// </summary>
    [SerializeField] private Transform ghostCatTrs;

    /// <summary>
    /// �S�[�X�g�Ԉʒu���
    /// </summary>
    [SerializeField] private Transform ghostWheelR;

    /// <summary>
    /// �S�[�X�g�Ԉʒu���
    /// </summary>
    [SerializeField] private Transform ghostWheelL;

    [Space(25)]
    [Header("===== UI =====")]

    [Space(10)]
    [Header("---- Panel ----")]

    /// <summary>
    /// ���U���g�p�l��
    /// </summary>
    [SerializeField] private GameObject resultPanel;

    /// <summary>
    /// �v���^�C�}�[�p�l��
    /// </summary>
    [SerializeField] private GameObject timerPanel;

    [Space(10)]
    [Header("---- Image ----")]

    /// <summary>
    /// �J�E���g�_�E���I�u�W�F
    /// </summary>
    [SerializeField] private GameObject countDownObj;

    /// <summary>
    /// �J�E���g�_�E���p�X�v���C�g
    /// </summary>
    [SerializeField] private Sprite[] countDownSprits;

    /// <summary>
    /// �V�L�^�摜
    /// </summary>
    [SerializeField] private GameObject newRecordObj;

    [Space(10)]
    [Header("---- Text ----")]

    /// <summary>
    /// ���Ԍv���p�e�L�X�g
    /// </summary>
    [SerializeField] private Text timerText;

    /// <summary>
    /// ���b�v���\���p�e�L�X�g
    /// </summary>
    [SerializeField] private Text rapText;

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

        // �ϐ�����������
        isCount = false;
        ghostCnt = 0;
        ghostCorrection = new Vector3(0, -0.74f, 0);

        rapText.text = currentRapNum.ToString() + " / " + maxRapNum.ToString();

        // �Đ�����S�[�X�g�f�[�^���擾
        if (UserModel.Instance.GhostData != "")
        {
            playGhost = JsonConvert.DeserializeObject<List<GhostData>>(UserModel.Instance.GhostData);
            Debug.Log("�S�[�X�g�f�[�^�擾");
        }
        else
        {
            ghostCarObj.SetActive(false);
        }

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
        currentRapNum++;    // ���b�v���Z

        if(currentRapNum == maxRapNum + 1)
        {
            CancelInvoke("SaveGhost");

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

            // �N���A�^�C����m/sec�ɕϊ�����
            timer = (float)Math.Round(timer, 3, MidpointRounding.AwayFromZero);
            goalTime = (int)(timer * 1000);

            UserModel userModel = UserModel.Instance;

            // ���M�p�S�[�X�g�f�[�^�̍쐬
            SaveGhost();
            string ghostData = JsonConvert.SerializeObject(ghostList);

            // �L�^�o�^����
            RegistResult result = await rankingModel.RegistClearTimeAsync(stageID, userModel.UserId, goalTime, ghostData);

            if (result.timeRegistFlag)
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
                // �v���J�n�E����\�E�S�[�X�g�f�[�^�ۑ������N������
                countDownObj.GetComponent<Image>().sprite = countDownSprits[i];

                if (UserModel.Instance.GhostData != "")
                {   // �S�[�X�g�f�[�^�����鎞�̂ݍĐ�
                    InvokeRepeating("PlayGhost",0,saveSpeed);
                }

                standardInput.SetActive(true);
                mobileInput.SetActive(true);
                isCount = true;
                InvokeRepeating("SaveGhost", 0.1f, saveSpeed);

                // �J�E���g��\��
                Invoke("HiddenCount", 0.6f);
            }
            else
            {
                SEManager.Instance.Play(SEPath.COUNT);
                // 0.9�b�҂��ăR���[�`�����f
                yield return new WaitForSeconds(0.9f);
            }
        }
    }

    /// <summary>
    /// �S�[�X�g�f�[�^�ۑ�����
    /// </summary>
    private void SaveGhost()
    {
        GhostData ghostData = new GhostData();
        ghostData.Pos = visualObj.position;        // �ʒu
        ghostData.Rot = visualObj.eulerAngles;     // �p�x
        ghostData.WRot = wheelRot.localEulerAngles.y;   // �^�C���p

        ghostList.Add(ghostData);
    }

    /// <summary>
    /// �S�[�X�g�Đ�����
    /// </summary>
    private void PlayGhost()
    {
        // �{�̈ʒu�̍X�V
        ghostCarObj.transform.DOMove(playGhost[ghostCnt].Pos + ghostCorrection, saveSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);
        ghostCarObj.transform.DORotate(playGhost[ghostCnt].Rot, saveSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);

        // �^�C���p�̍X�V
        ghostWheelL.transform.localEulerAngles = new Vector3 (ghostWheelL.transform.localEulerAngles.x,playGhost[ghostCnt].WRot,0);
        ghostWheelR.transform.localEulerAngles = new Vector3(ghostWheelR.transform.localEulerAngles.x, playGhost[ghostCnt].WRot, 0);

        ghostCnt++;

        if (playGhost.Count - 1 < ghostCnt)
        {   // �Đ�����f�[�^���������͍Đ���~
            CancelInvoke("PlayGhost");
            return;
        }
    }

    /// <summary>
    /// ���j���[�J�ڏ���
    /// </summary>
    public void OnBackMenu()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        SceneManager.LoadScene("2_MenuScene");
    }

    /// <summary>
    /// �X�e�[�W�I��J�ڏ���
    /// </summary>
    public void OnBackSelect()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        SceneManager.LoadScene("3_SoloSelectScene");
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

    /// <summary>
    /// �p�l�����鏈��
    /// </summary>
    /// <param name="panel"></param>
    public void OnCloseButton(GameObject panel)
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// ���j���[�{�^��������
    /// </summary>
    /// <param name="menuPanel"></param>
    public void OnMenuButton(GameObject menuPanel)
    {
        if (menuPanel.activeSelf)
        {
            menuPanel.SetActive(false);
        }
        else
        {
            menuPanel.SetActive(true);
        }
    }
}
