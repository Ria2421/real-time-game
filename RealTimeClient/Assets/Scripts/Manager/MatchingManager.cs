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
    /// ���[�����f���i�[�p
    /// </summary>
    [SerializeField] private RoomModel roomModel;

    /// <summary>
    /// ���͂��ꂽ���[�U�[ID
    /// </summary>
    [SerializeField] private Text inputText;

    /// <summary>
    /// �}�b�`���O�{�^��
    /// </summary>
    [SerializeField] private Button matchingButton;

    //-------------------------------------------------------
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        roomModel.OnMatchingUser += OnMatchingUser;     // �}�b�`���O�����ʒm
    }

    /// <summary>
    /// �}�b�`���O�{�^��
    /// </summary>
    public async void OnMatchingButton()
    {
        matchingButton.interactable = false;

        // �ڑ�
        await roomModel.ConnectAsync();
        // �}�b�`���O
        await roomModel.JoinLobbyAsync(UserModel.Instance.UserId);
    }

    /// <summary>
    /// �}�b�`���O�����ʒm��M���̏���
    /// </summary>
    /// <param name="roomName"></param>
    private async void OnMatchingUser(string roomName)
    {
        roomModel.RoomName = roomName;  // ���s���ꂽ���[������ۑ�

        // �ޏo
        await roomModel.ExitAsync();

        StartCoroutine("TransGmaeScene");
    }

    // �Q�[���V�[���J��
    private IEnumerator TransGmaeScene()
    {
        // 1�b�҂��ăR���[�`�����f
        yield return new WaitForSeconds(1.0f);

        // �Q�[���V�[���ɑJ��
        SceneManager.LoadScene("05_OnlineScene");
    }
}
