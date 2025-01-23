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

        // �ڑ�
        await roomModel.ConnectAsync();
        // �}�b�`���O
        await roomModel.JoinLobbyAsync(UserModel.Instance.UserId);

        Debug.Log("�}�b�`���O�J�n");
    }

    /// <summary>
    /// ����X�V����
    /// </summary>
    private void FixedUpdate()
    {
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

        // �ؒf
        await roomModel.DisconnectionAsync();

        // ���j���[�V�[���ɑJ��
        SceneManager.LoadScene("02_MenuScene");

        Debug.Log("�}�b�`���O�L�����Z��");
    }

    /// <summary>
    /// �}�b�`���O�����ʒm��M���̏���
    /// </summary>
    /// <param name="roomName"></param>
    private async void OnMatchingUser(string roomName)
    {
        cancelButton.interactable = false;

        roomModel.RoomName = roomName;  // ���s���ꂽ���[������ۑ�

        // SE�Đ�
        SEManager.Instance.Play(SEPath.MATCHING_COMPLETE);
        // �\���ؑ�
        matchingTextObj.SetActive(false);
        completeTextObj.SetActive(true);

        // �ޏo
        await roomModel.ExitAsync();

        StartCoroutine("TransGmaeScene");

        Debug.Log("�}�b�`���O����");
    }

    // �Q�[���V�[���J��
    private IEnumerator TransGmaeScene()
    {
        // 1�b�҂��ăR���[�`�����f
        yield return new WaitForSeconds(1.2f);

        // �Q�[���V�[���ɑJ��
        SceneManager.LoadScene("06_OnlinePlayScene");
    }
}
