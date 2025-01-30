//---------------------------------------------------------------
// �^�C�g���}�l�[�W���[ [ MatchingManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2025/01/30
//---------------------------------------------------------------
using KanKikuchi.AudioManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour
{
    //-------------------------------------------------------
    // �t�B�[���h

    /// <summary>
    /// �v���C�X�e�[�WID
    /// </summary>
    private int playStageID = 0;

    /// <summary>
    /// �}�b�`���O�e�L�X�g�I�u�W�F
    /// </summary>
    [SerializeField] private GameObject matchingTextObj;

    /// <summary>
    /// �}�b�`���O�����e�L�X�g�I�u�W�F
    /// </summary>
    [SerializeField] private GameObject completeTextObj;

    /// <summary>
    /// ���[�����f���i�[�p
    /// </summary>
    [SerializeField] private RoomModel roomModel;

    /// <summary>
    /// �L�����Z���{�^���I�u�W�F
    /// </summary>
    [SerializeField] private GameObject cancelObj;

    /// <summary>
    /// �L�����Z���{�^��
    /// </summary>
    [SerializeField] private Button cancelButton;

    /// <summary>
    /// �Ԕw�i
    /// </summary>
    [SerializeField] private Transform carBG;

    //-------------------------------------------------------
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    async void Start()
    {
        roomModel.OnMatchingUser += OnMatchingUser;     // �}�b�`���O�����ʒm

        Invoke("StartMatching",2.0f);
    }

    /// <summary>
    /// ����X�V����
    /// </summary>
    private void FixedUpdate()
    {
        // �ԉ摜����
        carBG.localEulerAngles += new Vector3(0,0,1.0f);
    }

    /// <summary>
    /// �L�����Z���{�^��
    /// </summary>
    public async void OnCancelButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        cancelButton.interactable = false;

        // �ؒf (�I����A���Ă�����V�[���ړ��ɂ���)
        await roomModel.DisconnectionAsync();

        // ���j���[�V�[���ɑJ��
        Initiate.DoneFading();
        Initiate.Fade("2_MenuScene", Color.white, 2.5f);

        Debug.Log("�}�b�`���O���~");
    }

    /// <summary>
    /// �}�b�`���O�����ʒm��M���̏���
    /// </summary>
    /// <param name="roomName"></param>
    private async void OnMatchingUser(string roomName,int stageID)
    {
        roomModel.RoomName = roomName;  // ���s���ꂽ���[������ۑ�
        playStageID = stageID;          // �X�e�[�WID��ۑ�

        // SE�Đ�
        SEManager.Instance.Play(SEPath.MATCHING_COMPLETE);
        // �\���ؑ�
        cancelObj.SetActive(false);
        matchingTextObj.SetActive(false);
        completeTextObj.SetActive(true);

        // �ޏo
        await roomModel.ExitAsync();

        StartCoroutine("TransGmaeScene");

        Debug.Log("�}�b�`���O����");
    }

    /// <summary>
    /// �Q�[���V�[���J��
    /// </summary>
    /// <returns></returns>
    private IEnumerator TransGmaeScene()
    {
        // 1�b�҂��ăR���[�`�����f
        yield return new WaitForSeconds(1.2f);

        // playStageID�ɉ����đΉ�����Q�[���V�[���ɑJ��
        Initiate.DoneFading();
        Initiate.Fade(playStageID.ToString() + "_OnlinePlayScene", Color.white, 2.5f);
    }

    /// <summary>
    /// �}�b�`���O�J�n����
    /// </summary>
    private async void StartMatching()
    {
        // �ڑ�
        await roomModel.ConnectAsync();
        // �}�b�`���O
        await roomModel.JoinLobbyAsync(UserModel.Instance.UserId);
        // �L�����Z���{�^���L����
        cancelButton.interactable = true;

        Debug.Log("�}�b�`���O�J�n");
    }
}
