//---------------------------------------------------------------
// �Q�[���}�l�[�W���[ [ GameManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2024/12/05
//---------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using DG.Tweening;
using RealTimeServer.Model.Entity;
using DavidJalbert;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks.Triggers;
using KanKikuchi.AudioManager;

public class GameManager : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    /// <summary>
    /// �Q�[����Ԏ��
    /// </summary>
    private enum GameState
    {
        None = 0,
        Join,
        InGame
    }

    /// <summary>
    /// ���o�C���C���v�b�g�X�N���v�g
    /// </summary>
    [SerializeField] private TinyCarMobileInput tinyCarMobileInput;

    /// <summary>
    /// ���o�C���C���v�b�gobj
    /// </summary>
    [SerializeField] private GameObject mobileInputObj;

    /// <summary>
    /// ���[�����f���i�[�p
    /// </summary>
    private RoomModel roomModel;

    /// <summary>
    /// �ڑ�ID���L�[�ɃL�����N�^�̃I�u�W�F�N�g���Ǘ�
    /// </summary>
    private Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    /// <summary>
    /// �Q�[�����
    /// </summary>
    private GameState gameState = GameState.None;

    /// <summary>
    /// �␳pos
    /// </summary>
    private Vector3 posCorrection = new Vector3(0.0f,-0.9f,0.0f);

    /// <summary>
    /// �v���C���[�R���g���[���[
    /// </summary>
    private GameObject playerController;

    /// <summary>
    /// ����R���g���[���[
    /// </summary>
    private GameObject inputController;

    /// <summary>
    /// �z�C�[���̊p�x�擾�p
    /// </summary>
    private Transform wheelAngle;

    /// <summary>
    /// �v���C���[�̃^�[�{�p�[�e�B�N��
    /// </summary>
    private ParticleSystem playerTurboParticle;

    /// <summary>
    /// �v���C���[�̃h���t�g�p�[�e�B�N��
    /// </summary>
    private ParticleSystem playerDriftParticle;

    [Header("�e��Object���A�^�b�`")]

    /// <summary>
    /// ��������v���C���[�̃L�����N�^�[�v���n�u
    /// </summary>
    [SerializeField] private GameObject playerPrefab;

    /// <summary>
    /// �������鑼�v���C���[�̃L�����N�^�[�v���n�u
    /// </summary>
    [SerializeField] private GameObject otherPrefab;

    /// <summary>
    /// ���͏����v���n�u
    /// </summary>
    [SerializeField] private GameObject inputPrefab;

    /// <summary>
    /// �v���C���[���i�[����e�I�u�W�F�N�g
    /// </summary>
    [SerializeField] private GameObject parentObj;

    /// <summary>
    /// ���C���J����
    /// </summary>
    [SerializeField] private GameObject mainCamera;

    /// <summary>
    /// �v���C���[���Ƃ̃��X�n�_
    /// </summary>
    [SerializeField] private Transform[] respownList;

    /// <summary>
    /// �ʐM���x
    /// </summary>
    [SerializeField] private float internetSpeed = 0.1f;

    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField] private float timeLimit = 3.0f;

    /// <summary>
    /// �����p�[�e�B�N��
    /// </summary>
    [SerializeField] private GameObject explosionPrefab;

    [Space (25)]
    [Header("===== UI�֘A =====")]

    [Space(10)]
    [Header("---- Text ----")]

    /// <summary>
    /// �^�C�}�[
    /// </summary>
    [SerializeField] private Text timerText;

    /// <summary>
    /// ���j�ʒm�\��
    /// </summary>
    [SerializeField] private GameObject crushText;

    /// <summary>
    /// ���[�g�e�L�X�g
    /// </summary>
    [SerializeField] private Text rateText;

    /// <summary>
    /// �����\���e�L�X�g
    /// </summary>
    [SerializeField] private Text signText;

    /// <summary>
    /// ���[�g�����e�L�X�g
    /// </summary>
    [SerializeField] private Text changeRateText;

    [Space(10)]
    [Header("---- Panel ----")]

    /// <summary>
    /// ���U���g�p�l��
    /// </summary>
    [SerializeField] private GameObject resultPanel;

    [Space(10)]
    [Header("---- Image ----")]

    /// <summary>
    /// ���ʕ\���摜
    /// </summary>
    [SerializeField] private GameObject[] rankImages;

    /// <summary>
    /// �J�E���g�_�E���摜�I�u�W�F
    /// </summary>
    [SerializeField] private GameObject countDownImageObj;

    /// <summary>
    /// �I���\���摜
    /// </summary>
    [SerializeField] private GameObject endImageObj;

    /// <summary>
    /// �J�E���g�_�E���摜
    /// </summary>
    [SerializeField] private Image countDownImage;

    [Space(10)]
    [Header("---- Sprit ----")]

    /// <summary>
    /// �J�E���g�_�E���X�v���C�g
    /// </summary>
    [SerializeField] private Sprite[] countSprits;

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    async void Start()
    {
        // BGM�Đ�
        BGMManager.Instance.Pause(BGMPath.MAIN_BGM);
        BGMManager.Instance.Play(BGMPath.MULTI_PLAY);

        // ���[�����f���̎擾
        roomModel = GameObject.Find("RoomModel").GetComponent<RoomModel>();

        // �e�ʒm���͂����ۂɍs�����������f���ɓo�^����
        roomModel.OnJoinedUser += OnJoinedUser;         // ����
        roomModel.OnExitedUser += OnExitedUser;         // �ގ�
        roomModel.OnMovedUser += OnMovedUser;           // �ړ�
        roomModel.OnInGameUser += OnInGameUser;         // �C���Q�[��
        roomModel.OnStartGameUser += OnStartGameUser;   // �Q�[���X�^�[�g
        roomModel.OnEndGameUser += OnEndGameUser;       // �Q�[���I��
        roomModel.OnCrushingUser += OnCrushingUser;     // ���j

        // �ڑ�
        await roomModel.ConnectAsync();
        // ���� (���[�����ƃ��[�U�[ID��n���ē����B�ŏI�I�ɂ̓��[�J���f�[�^�̃��[�U�[ID���g�p
        await roomModel.JoinAsync();

        Debug.Log("����");
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// �ړ��f�[�^���M����
    /// </summary>
    private async void SendMoveData()
    {
        if (gameState == GameState.None) return;

        // �ړ��f�[�^�̍쐬
        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,
            Position = playerController.transform.position + posCorrection,
            Rotation = playerController.transform.eulerAngles,
            WheelAngle = wheelAngle.eulerAngles.y,
            IsTurbo = playerTurboParticle.isPlaying,
            IsDrift = playerDriftParticle.isPlaying
        };

        await roomModel.MoveAsync(moveData);
    }

    /// <summary>
    /// �ڑ�����
    /// </summary>
    public async void OnConnect()
    {

    }

    /// <summary>
    /// �ؒf����
    /// </summary>
    public async void OnDisconnect()
    {
        CancelInvoke();

        // �ޏo
        await roomModel.DisconnectionAsync();

        // �v���C���[�I�u�W�F�N�g�̍폜
        foreach (Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        gameState = GameState.None;
        Debug.Log("�ޏo");
    }

    /// <summary>
    /// �Q�[���X�^�[�g�ʒm����
    /// </summary>
    public async void OnStart()
    {
        await roomModel.GameStartAsync();
    }

    // --------------------------------------------------------------
    // ���f���o�^�p�֐�

    /// <summary>
    /// �����ʒm��M���̏���
    /// </summary>
    /// <param name="user"></param>
    private void OnJoinedUser(JoinedUser user)
    {
        if (characterList.ContainsKey(user.ConnectionId)) return;

        GameObject characterObj;    // ���������I�u�W�F�N�g

        // ���������v���C���[��
        if (user.ConnectionId == roomModel.ConnectionId)
        {
            // ���@�E����p�I�u�W�F�̐���
            characterObj = Instantiate(playerPrefab, respownList[user.JoinOrder - 1].position, Quaternion.Euler(0, 180, 0));
            inputController = Instantiate(inputPrefab, Vector3.zero, Quaternion.identity);

            // �J�[�R���g���[���[�̎擾�E���f
            playerController = characterObj.transform.GetChild(0).gameObject;
            wheelAngle = characterObj.transform.Find("Visuals/WheelFrontLeft").transform;

            // �p�[�e�B�N���̎擾
            playerTurboParticle = characterObj.transform.Find("Visuals/ParticlesBoost").GetComponent<ParticleSystem>();
            playerDriftParticle = characterObj.transform.Find("Visuals/ParticlesDrifting").GetComponent<ParticleSystem>();

            // ���o�C���C���v�b�g�ɃJ�[�R���g���[���[��ݒ�
            tinyCarMobileInput.carController = playerController.GetComponent<TinyCarController>();

            // UI�ύX
            gameState = GameState.Join;
            Debug.Log("���@��������");

            // �ʒu���M�J�n
            InvokeRepeating("SendMoveData", 0.5f, internetSpeed);
        }
        else
        {
            // ���v���C���[�̐���
            characterObj = Instantiate(otherPrefab, respownList[user.JoinOrder - 1].position, Quaternion.Euler(0, 180, 0));
            characterObj.GetComponent<OtherPlayerManager>().ConnectionID = user.ConnectionId;   // �ڑ�ID�̕ۑ�
            characterObj.GetComponent<OtherPlayerManager>().UserName = user.UserData.Name;      // ���[�U�[���̕ۑ�
        }

        characterObj.transform.parent = parentObj.transform;    // �e��ݒ�
        characterList[user.ConnectionId] = characterObj;        // �t�B�[���h�ɕۑ�

        Debug.Log(user.JoinOrder + "P�Q��");
    }

    /// <summary>
    /// �ޏo�ʒm��M���̏���
    /// </summary>
    /// <param name="user"></param>
    private void OnExitedUser(JoinedUser user)
    {
        // �ʒu���̍X�V
        if (!characterList.ContainsKey(user.ConnectionId)) return;  // �ޏo�҃I�u�W�F�̑��݃`�F�b�N

        Destroy(characterList[user.ConnectionId]);   // �I�u�W�F�N�g�̔j��
        characterList.Remove(user.ConnectionId);     // ���X�g����폜
    }

    /// <summary>
    /// �v���C���[���ړ������Ƃ��̏���
    /// </summary>
    /// <param name="moveData"></param>
    private void OnMovedUser(MoveData moveData)
    {
        // �ʒu���̍X�V
        if (!characterList.ContainsKey(moveData.ConnectionId)) return;

        // �{�̈ʒu�̍X�V
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Position, internetSpeed).SetEase(Ease.Linear);
        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotation, internetSpeed).SetEase(Ease.Linear);

        // �^�C���p�̍X�V
        characterList[moveData.ConnectionId].transform.Find("wheels/wheel front right").transform.DORotate(new Vector3(0,moveData.WheelAngle,0),internetSpeed).SetEase(Ease.Linear);
        characterList[moveData.ConnectionId].transform.Find("wheels/wheel front left").transform.DORotate(new Vector3(0, moveData.WheelAngle, 0), internetSpeed).SetEase(Ease.Linear);

        // �p�[�e�B�N���̎擾�E�X�V
        characterList[moveData.ConnectionId].GetComponent<OtherPlayerManager>().playDrift(moveData.IsDrift);
        characterList[moveData.ConnectionId].GetComponent<OtherPlayerManager>().playTurbo(moveData.IsTurbo);
    }

    /// <summary>
    /// �C���Q�[���ʒm��M����
    /// </summary>
    private void OnInGameUser()
    {
        // �J�E���g�_�E���J�n
        Debug.Log("�J�E���g�_�E��");
        StartCoroutine("StartCount");
    }

    /// <summary>
    /// �Q�[���X�^�[�g�ʒm��M����
    /// </summary>
    private void OnStartGameUser()
    {
        SEManager.Instance.Play(SEPath.START);

        // �e�L�X�g�ύX
        countDownImage.sprite = countSprits[0];
        StartCoroutine("HiddenText");

        // �J�������g�b�v�_�E���ɕύX
        mainCamera.GetComponent<TinyCarCamera>().whatToFollow = playerController.transform;
        // ����\�ɂ���
        inputController.GetComponent<TinyCarStandardInput>().carController = playerController.GetComponent<TinyCarController>();
        mobileInputObj.SetActive(true);
        
    }

    /// <summary>
    /// �Q�[���I���ʒm��M����
    /// </summary>
    private void OnEndGameUser(List<ResultData> result)
    {
        // ����s�\�ɂ���
        playerController.GetComponent<Rigidbody>().isKinematic = true;

        //++ �I��SE�Đ�
        SEManager.Instance.Play(SEPath.GOAL);

        // �I���\��
        endImageObj.SetActive(true);

        // ���U���g�p�l���Ɍ��ʂ𔽉f (�����̏��ʁE���[�g�𔽉f)
        foreach(ResultData resultData in result)
        {
            if(resultData.UserId == roomModel.UserId)
            {
                rateText.text = resultData.Rate.ToString();                         // �擾�������[�g��\��
                changeRateText.text = Math.Abs(resultData.ChangeRate).ToString();   // �������[�g��\��
                if(resultData.ChangeRate < 0)
                {   // ���Z�̏ꍇ�� - ��\��
                    signText.text = "-";
                }

                rankImages[resultData.Rank - 1].SetActive(true);        // �Y�����ʂ̕\��
            }
        }

        // 1�b��Ƀ��U���g�\��
        StartCoroutine("DisplayResult");
    }

    /// <summary>
    /// �v���C���[���j�ʒm����
    /// </summary>
    /// <param name="attackName">���j�����l��PL��</param>
    /// <param name="cruchName"> ���j���ꂽ�l��PL��</param>
    /// <param name="crushID">   ���j���ꂽ�l�̐ڑ�ID</param>
    private void OnCrushingUser(string attackName, string cruchName, Guid crushID)
    {
        // ���j�ʒm�e�L�X�g�̓��e�ύX�E�\��
        crushText.GetComponent<Text>().text = attackName + " �� " + cruchName + "�����j�I";

        // �ʒm�\��Sequence���쐬
        var sequence = DOTween.Sequence();
        sequence.Append(crushText.transform.DOLocalMove(new Vector3(0f, 450f, 0f), 1.5f));
        sequence.Append(crushText.transform.DOLocalMove(new Vector3(0f, 625f, 0f), 0.5f));
        sequence.Play();

        // �����A�j���[�V��������
        if (roomModel.ConnectionId == crushID)
        {   // ���������j���ꂽ�Ƃ�
            SEManager.Instance.Play(SEPath.BOOM);
            Instantiate(explosionPrefab, playerController.transform.position, Quaternion.identity); // �����G�t�F�N�g
            mainCamera.GetComponent<TinyCarCamera>().whatToFollow = null;                           // �J��������ՂɕύX
            characterList[crushID].GetComponent<TinyCarExplosiveBody>().explode();                  // ��������
            characterList.Remove(crushID);                                                          // PL���X�g����폜
        }
        else
        {
            SEManager.Instance.Play(SEPath.BOOM);
            Instantiate(explosionPrefab, characterList[crushID].transform.position, Quaternion.identity);
            Destroy(characterList[crushID]);
            characterList.Remove(crushID);
        }
    }

    /// <summary>
    /// �Q�[���J�E���g
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCount()
    {
        for (int i = 3; i > 0; i--)
        {
            countDownImage.sprite = countSprits[i];

            SEManager.Instance.Play(SEPath.COUNT);

            // 1�b�҂��ăR���[�`�����f
            yield return new WaitForSeconds(1.0f);

            if (i == 1)
            {
                OnStart();
            }
        }
    }

    /// <summary>
    /// �^�C�}�[�e�L�X�g��\������
    /// </summary>
    /// <returns></returns>
    IEnumerator HiddenText()
    {
        // 1�b�҂��ăR���[�`�����f
        yield return new WaitForSeconds(0.8f);

        countDownImageObj.SetActive(false);
        timerText.text = "";
    }

    /// <summary>
    /// ���U���g�\��
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayResult()
    {
        // 1�b�҂��ăR���[�`�����f
        yield return new WaitForSeconds(1.0f);

        mobileInputObj.SetActive(false);
        endImageObj.SetActive(false);
        resultPanel.gameObject.SetActive(true); // ���U���g�p�l���\��
    }

    /// <summary>
    /// �^�C�g���{�^��������
    /// </summary>
    public async void OnTitleButton()
    {
        CancelInvoke();

        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // �ޏo
        await roomModel.ExitAsync();

        // �v���C���[�I�u�W�F�N�g�̍폜
        foreach (Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        gameState = GameState.None;
        Debug.Log("�ޏo");

        // ���[�����f���̔j��
        Destroy(GameObject.Find("RoomModel"));

        // �^�C�g���ɖ߂�
        SceneManager.LoadScene("02_MenuScene");
    }
}
