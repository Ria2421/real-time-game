//---------------------------------------------------------------
// �^�C�g���}�l�[�W���[ [ TitleManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/05
// Update:2025/01/30
//---------------------------------------------------------------
using DG.Tweening;
using KanKikuchi.AudioManager;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class TitleManager : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    /// <summary>
    /// �^�C�g���摜
    /// </summary>
    [SerializeField] private GameObject titleImage;

    /// <summary>
    /// �^�b�`�摜
    /// </summary>
    [SerializeField] private GameObject touchImage;

    /// <summary>
    /// ���[�U�[�o�^�p�l��
    /// </summary>
    [SerializeField] private GameObject registPanel;

    /// <summary>
    /// �o�^���[�U�[��
    /// </summary>
    [SerializeField] private Text nameText;

    /// <summary>
    /// �o�^�{�^��
    /// </summary>
    [SerializeField] private Button registButton;

    /// <summary>
    /// �G���[�{�^��
    /// </summary>
    [SerializeField] private GameObject errorButton;

    // �f�o�b�O�p *******************************

    /// <summary>
    /// �f�o�b�O�pID
    /// </summary>
    [SerializeField] private Text debugIDText;

    /// <summary>
    /// �f�o�b�O�p�{�^��
    /// </summary>
    [SerializeField] private Button debugButton;

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        Application.targetFrameRate = 60;

        // BGM�Đ�
        BGMManager.Instance.Play(BGMPath.MAIN_BGM,0.75f,0,1,true,true);

        // �^�C�g���摜�A�j���[�V����
        titleImage.transform.DOScale(0.9f, 1.3f).SetEase(Ease.InCubic).SetLoops(-1,LoopType.Yoyo);
        InvokeRepeating("BlinkingImage", 0, 0.8f);
    }

    /// <summary>
    /// �X�^�[�g�{�^��������
    /// </summary>
    public void OnStartButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // ���[�U�[�f�[�^�̓Ǎ������E���ʂ��擾
        bool isSuccess = UserModel.Instance.LoadUserData();

        if (!isSuccess)
        {
            // �o�^�p�p�l���\��
            Debug.Log("�f�[�^�Ȃ�");
            registPanel.SetActive(true);
        }
        else
        {   // �V�[���J�ڏ���
            Debug.Log("�f�[�^����");
            Initiate.DoneFading();
            Initiate.Fade("2_MenuScene", Color.white, 2.5f);
        }
    }

    /// <summary>
    /// �o�^�{�^��������
    /// </summary>
    public async void OnRegistUser()
    {
        if (nameText.text == "") return;

        // SE�Đ�
        SEManager.Instance.Play(SEPath.MENU_SELECT);

        // �{�^������
        registButton.interactable = false;

        // �o�^����
        UserModel.Status statusCode = await UserModel.Instance.RegistUserAsync(nameText.text);

        switch (statusCode)
        {
            case UserModel.Status.True:
                Debug.Log("�o�^����");
                Initiate.DoneFading();
                Initiate.Fade("2_MenuScene", Color.white, 2.5f);
                break;

            case UserModel.Status.False:
                Debug.Log("�ʐM���s");
                registButton.interactable = true;
                break;

            case UserModel.Status.SameName:
                Debug.Log("���O���");
                errorButton.SetActive(true);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// �摜�_�ŏ���
    /// </summary>
    private void BlinkingImage()
    {
        if(touchImage.activeSelf == true)
        {
            touchImage.SetActive(false);
        }
        else
        {
            touchImage.SetActive(true);
        }
    }

    // �f�o�b�O�p ********************

    /// <summary>
    /// ID�ۑ�����
    /// </summary>
    public void DebugOnSaveID()
    {
        if (debugIDText.text == "") return;

        debugButton.interactable = false;

        UserModel.Instance.UserId = int.Parse(debugIDText.text);

        Initiate.DoneFading();
        Initiate.Fade("2_MenuScene", Color.white, 2.5f);
    }

    /// <summary>
    /// �G���[�{�^��������
    /// </summary>
    public void OnErrorButton()
    {
        errorButton.SetActive(false);

        // �o�^�{�^���̗L����
        registButton.interactable = true;
    }
}