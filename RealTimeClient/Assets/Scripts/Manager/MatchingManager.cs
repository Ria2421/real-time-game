using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour
{
    //-------------------------------------------------------
    // �t�B�[���h

    [SerializeField] private RoomModel roomModel;

    [SerializeField] private Text inputText;

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
        // �ڑ�
        await roomModel.ConnectAsync();
        // �}�b�`���O
        await roomModel.JoinLobbyAsync(int.Parse(inputText.text));
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

        // �Q�[���V�[���Ɉړ�
        SceneManager.LoadScene("05_OnlineScene");
    }
}
