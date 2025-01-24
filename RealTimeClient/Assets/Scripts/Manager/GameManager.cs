//---------------------------------------------------------------
// �Q�[���}�l�[�W���[ [ GameManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2025/01/23
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
        InGame,
        Result
    }

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
    /// �ԑ̃I�u�W�F�̈ʒu���
    /// </summary>
    private Transform visualTransform;

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

    /// <summary>
    /// �Q����
    /// </summary>
    private int joinOrder = 0;

    [Header("���l�ݒ�")]

    /// <summary>
    /// �ʐM���x
    /// </summary>
    [SerializeField] private float internetSpeed = 0.1f;

    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField] private int timeLimit = 30;

    [Header("�e��Object���A�^�b�`")]

    /// <summary>
    /// ���o�C���C���v�b�g�X�N���v�g
    /// </summary>
    [SerializeField] private TinyCarMobileInput tinyCarMobileInput;

    /// <summary>
    /// ���o�C���C���v�b�gobj
    /// </summary>
    [SerializeField] private GameObject mobileInputObj;

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
    /// �����p�[�e�B�N��
    /// </summary>
    [SerializeField] private GameObject explosionPrefab;

    /// <summary>
    /// ���[�U�[���\���p�I�u�W�F
    /// </summary>
    [SerializeField] private GameObject[] nameObjs;

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
    /// ���[�g�\���p�I�u�W�F
    /// </summary>
    [SerializeField] private GameObject rateObjs;

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

    /// <summary>
    /// ���[�U�[���e�L�X�g
    /// </summary>
    [SerializeField] private Text[] nameTexts;

    [Space(10)]
    [Header("---- Panel ----")]

    /// <summary>
    /// ���U���g�p�l��
    /// </summary>
    [SerializeField] private GameObject resultPanel;

    /// <summary>
    /// �ؒf�\���p�l��
    /// </summary>
    [SerializeField] private GameObject disconnectPanel;

    /// <summary>
    /// �c�^�C���\���p�l��
    /// </summary>
    [SerializeField] private GameObject timerPanel;

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

    /// <summary>
    /// ���������\���p�I�u�W�F
    /// </summary>
    [SerializeField] private GameObject drawImageObj;

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
        roomModel.OnTimeCountUser += OnTimeCountUser;   // �^�C���J�E���g
        roomModel.OnTimeUpUser += OnTimeUpUser;         // �^�C���A�b�v

        // �������Ԃ̏�����
        timerText.text = timeLimit.ToString();

        // �ڑ�
        await roomModel.ConnectAsync();
        // ���� (���[�����ƃ��[�U�[ID��n���ē����B�ŏI�I�ɂ̓��[�J���f�[�^�̃��[�U�[ID���g�p����
        await roomModel.JoinAsync();

        Debug.Log("����");
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
            Rotation = visualTransform.eulerAngles,
            WheelAngle = wheelAngle.eulerAngles.y,
            IsTurbo = playerTurboParticle.isPlaying,
            IsDrift = playerDriftParticle.isPlaying
        };

        await roomModel.MoveAsync(moveData);
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

        // ���������v���C���[�����f
        if (user.ConnectionId == roomModel.ConnectionId)
        {
            // �Q�����̕ۑ�
            joinOrder = user.JoinOrder;

            // ���@�E����p�I�u�W�F�̐���
            characterObj = Instantiate(playerPrefab, respownList[user.JoinOrder - 1].position, Quaternion.Euler(0, 180, 0));
            inputController = Instantiate(inputPrefab, Vector3.zero, Quaternion.identity);

            // �J�[�R���g���[���[�̎擾�E���f
            playerController = characterObj.transform.GetChild(0).gameObject;
            wheelAngle = characterObj.transform.Find("Visuals/WheelFrontLeft").transform;

            // �v���C���[�̈ʒu�����擾
            visualTransform = characterObj.transform.GetChild(1).gameObject.GetComponent<Transform>();

            // �p�[�e�B�N���̎擾
            playerTurboParticle = characterObj.transform.Find("Visuals/ParticlesBoost").GetComponent<ParticleSystem>();
            playerDriftParticle = characterObj.transform.Find("Visuals/ParticlesDrifting").GetComponent<ParticleSystem>();

            // ���o�C���C���v�b�g�ɃJ�[�R���g���[���[��ݒ�
            tinyCarMobileInput.carController = playerController.GetComponent<TinyCarController>();

            // ���[�U�[��UI�̒Ǐ]�ݒ� & ���O���f
            nameObjs[user.JoinOrder - 1].GetComponent<NameTracker>().SetTarget(playerController.transform, 1);
            nameTexts[user.JoinOrder - 1].text = user.UserData.Name;
            nameObjs[user.JoinOrder - 1].SetActive(true);

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
            characterObj.GetComponent<OtherPlayerManager>().JoinOrder = user.JoinOrder;         // �Q�����̕ۑ�

            // ���[�U�[��UI�̒Ǐ]�ݒ� & ���O���f
            nameObjs[user.JoinOrder - 1].GetComponent<NameTracker>().SetTarget(characterObj.transform, 2);
            nameTexts[user.JoinOrder - 1].text = user.UserData.Name;
            nameObjs[user.JoinOrder - 1].SetActive(true);
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
        if (!characterList.ContainsKey(user.ConnectionId)) return;  // �ޏo�҃I�u�W�F�̑��݃`�F�b�N

        if(gameState == GameState.Result)
        {
            Destroy(characterList[user.ConnectionId]);   // �I�u�W�F�N�g�̔j��
            characterList.Remove(user.ConnectionId);     // ���X�g����폜
        }
        else
        {
            // ���v���C���[�̐ؒf�\���E�����Ń��j���[�ɖ߂�
            disconnectPanel.SetActive(true);
        }
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
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Position, internetSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);
        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotation, internetSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);

        // �^�C���p�̍X�V
        characterList[moveData.ConnectionId].transform.Find("wheels/wheel front right").transform.DORotate(new Vector3(0,moveData.WheelAngle,0),internetSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);
        characterList[moveData.ConnectionId].transform.Find("wheels/wheel front left").transform.DORotate(new Vector3(0, moveData.WheelAngle, 0), internetSpeed).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed, true);

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

        // �������ԕ\���E�z�X�g�̓J�E���g�J�n
        timerPanel.transform.DOLocalMove(new Vector3(820, 450, 0), 0.6f);
        if (joinOrder == 1)
        {
            InvokeRepeating("CountTime", 1, 1);
        }

        // �J�������g�b�v�_�E���ɕύX
        mainCamera.GetComponent<TinyCarCamera>().whatToFollow = playerController.transform;
        // ����\�ɂ���
        inputController.GetComponent<TinyCarStandardInput>().carController = playerController.GetComponent<TinyCarController>();
        mobileInputObj.SetActive(true);

        gameState = GameState.InGame;
    }

    /// <summary>
    /// �Q�[���I���ʒm��M����
    /// </summary>
    private void OnEndGameUser(List<ResultData> result)
    {
        gameState = GameState.Result;

        // ����s�\�ɂ���
        mobileInputObj.SetActive(false);

        // �I��SE�Đ�
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
    /// <param name="crushName"> ���j���ꂽ�l��PL��</param>
    /// <param name="crushID">   ���j���ꂽ�l�̐ڑ�ID</param>
    private void OnCrushingUser(string attackName, string crushName, Guid crushID, int deadNo)
    {
        // ���j�ʒm�e�L�X�g�̓��e�ύX�E�\������ ()
        if(deadNo == 1)
        {
            crushText.GetComponent<Text>().text = attackName + " �� " + crushName + "�����j�I";
        }
        else if(deadNo == 2)
        {
            crushText.GetComponent<Text>().text = crushName + "�������ɂ�蔚�j�I";
        }

        // �ʒm�\��Sequence���쐬
        var sequence = DOTween.Sequence();
        sequence.Append(crushText.transform.DOLocalMove(new Vector3(0f, 450f, 0f), 1.5f));
        sequence.Append(crushText.transform.DOLocalMove(new Vector3(0f, 625f, 0f), 0.5f));
        sequence.Play();
        
        // ���j���ꂽ�v���C���[�̖��O���\��
        foreach(Text name in nameTexts)
        {
            if(name.text == crushName)
            {
                name.text = "";
            }
        }

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
    /// �c�^�C���ʒm����
    /// </summary>
    /// <param name="time"></param>
    private void OnTimeCountUser(int time)
    {   // �c�^�C���̔��f����
        timerText.text = time.ToString();

        if (3 >= time)
        {
            timerText.color = Color.yellow;
        }
    }

    /// <summary>
    /// �^�C���A�b�v�ʒm
    /// </summary>
    private void OnTimeUpUser()
    {
        if (gameState == GameState.Result) return;

        // �I��SE�Đ�
        SEManager.Instance.Play(SEPath.GOAL);

        // ����s�\�ɂ���
        mobileInputObj.SetActive(false);

        // ���O��\��
        foreach (GameObject obj in nameObjs)
        {
            obj.SetActive(false);
        }

        drawImageObj.SetActive(true);           // ���������\��
        resultPanel.gameObject.SetActive(true); // ���U���g�p�l���\��

        gameState = GameState.Result;
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
    /// �c�^�C���J�E���g����
    /// </summary>
    private async void CountTime()
    {
        timeLimit--;

        await roomModel.TimeCountAsync(timeLimit);

        if(timeLimit <= 0)
        {   // 0�ȉ��̎��̓J�E���g�I��
            CancelInvoke("CountTime");
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
    }

    /// <summary>
    /// ���U���g�\��
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayResult()
    {
        CancelInvoke("CountTime");

        // ���O�\�������ׂ�OFF
        foreach (GameObject obj in nameObjs)
        {
            obj.SetActive(false);
        }

        // 1�b�҂��ăR���[�`�����f
        yield return new WaitForSeconds(1.0f);

        mobileInputObj.SetActive(false);
        endImageObj.SetActive(false);
        rateObjs.SetActive(true);               // ���[�g�\��
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
