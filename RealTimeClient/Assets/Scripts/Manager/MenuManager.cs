//---------------------------------------------------------------
// ���j���[�}�l�[�W���[ [ MenuManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2025/01/30
//---------------------------------------------------------------
using KanKikuchi.AudioManager;
using RealTimeServer.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    private int imageNo = 0;

    [Header("---- Button ----")]

    // ���j���[�{�^��
    [SerializeField] private Button acountButton;           // �A�J�E���g
    [SerializeField] private Button shopButton;             // �V���b�v
    [SerializeField] private Button optionButton;           // �I�v�V����
    [SerializeField] private Button updateButton;           // �X�V�ҏW

    [Header("---- Slider ----")]

    // �T�E���h�X���C�_�[
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    [Header("---- Panel ----")]

    // ���j���[�p�l��
    [SerializeField] private GameObject accountPanel;
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private GameObject helpPanel;

    [Header("---- AccountPanel ----")]

    // �A�J�E���g�p�l���\��UI
    [SerializeField] private Text displayNameText;
    [SerializeField] private Text inputNameText;
    [SerializeField] private Text registText;
    [SerializeField] private Text rateText;
    [SerializeField] private GameObject errorButton;        // �G���[ (���O���)
    [SerializeField] private GameObject netErrorButton;     // �G���[ (�ʐM�G���[)
    [SerializeField] private GameObject nameUpdateButton;   // ���O�X�V����

    [Header("---- HelpPanel ----")]

    // �w���v�p�l���\��UI
    [SerializeField] private Text nowPageText;
    [SerializeField] private Text maxPageText;
    [SerializeField] private Image helpImage;
    [SerializeField] private Sprite[] helpSprites;

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        if (GameObject.Find("RoomModel"))
        {
            Destroy(GameObject.Find("RoomModel"));
        }

        //�Đ�����BGM�̖��O��S�Ď擾
        var currentBGMNames = BGMManager.Instance.GetCurrentAudioNames();

        maxPageText.text = helpSprites.Length.ToString();

        // �`���[�g���A���\�����f
        if (!UserModel.Instance.TutorialFlag)
        {
            helpPanel.SetActive(true);
            UserModel.Instance.TutorialFlag = true;
            UserModel.Instance.SaveUserData();
        }

        if (currentBGMNames[0] != "MainBGM")
        {   // MainBGM���ĊJ
            BGMManager.Instance.Stop(BGMPath.TIME_ATTACK);
            BGMManager.Instance.Stop(BGMPath.MULTI_PLAY);
            BGMManager.Instance.Play(BGMPath.MAIN_BGM, 0.75f, 0, 1, true, true);
        }
    }

    /// <summary>
    /// �G���[�{�^����\���E�X�V�{�^������
    /// </summary>
    public void OnErrorButton()
    {
        errorButton.SetActive(false);
        netErrorButton.SetActive(false);
        updateButton.interactable = true;
    }

    /// <summary>
    /// BGM���ʕύX����
    /// </summary>
    public void ChangeBgmVolume()
    {
        BGMManager.Instance.ChangeBaseVolume(bgmSlider.value);
    }

    /// <summary>
    /// SE���ʕύX����
    /// </summary>
    public void ChangeSeVolume()
    {
        SEManager.Instance.ChangeBaseVolume(seSlider.value);
    }

    /// <summary>
    /// �w��p�l���̕\������
    /// </summary>
    private void DisplayPanel(GameObject panel)
    {
        // �S�p�l�����\��
        accountPanel.SetActive(false);
        soundPanel.SetActive(false);
        helpPanel.SetActive(false);

        // �w��p�l����\��
        panel.SetActive(true);
    }

    //-----------------------------
    // �{�^����������

    /// <summary>
    /// �\���{�^��������
    /// </summary>
    public void OnSoloButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // �\���I�����[�h�J��
        Initiate.DoneFading();
        Initiate.Fade("3_SoloSelectScene", Color.white, 2.5f);
    }

    /// <summary>
    /// �I�����C���{�^��������
    /// </summary>
    public void OnOnlineButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // �I�����C�����[�h�J��
        Initiate.DoneFading();
        Initiate.Fade("4_MatchingScene", Color.white, 2.5f);
    }

    /// <summary>
    /// �^�C�g���{�^��������
    /// </summary>
    public void OnTitleButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // �^�C�g���J��
        Initiate.DoneFading();
        Initiate.Fade("1_TitleScene", Color.white, 2.5f);
    }

    /// <summary>
    /// �A�J�E���g�{�^��������
    /// </summary>
    public async void OnAcountButton()
    {
        if (accountPanel.activeSelf)
        {
            accountPanel.SetActive(false);
        }
        else
        {
            // SE�Đ�
            SEManager.Instance.Play(SEPath.TAP_BUTTON);

            // ���[�U�[�f�[�^�̎擾
            var userData = await UserModel.Instance.SearchUserID(UserModel.Instance.UserId);

            if (userData == null)
            {   // �G���[�\��
                errorButton.SetActive(true);
                return;
            }
            else
            {   // ���[�U�[�f�[�^���f�E�\��
                displayNameText.text = userData.Name;
                registText.text = userData.Created_at.ToString();
                rateText.text = userData.Rate.ToString();
                DisplayPanel(accountPanel);
            }
        }
    }

    /// <summary>
    /// ���[�U�[���ύX�{�^��
    /// </summary>
    public async void OnNameUpdateButton()
    {
        // �{�^��������
        updateButton.interactable = false;

        // �o�^����
        UserModel.Status statusCode = await UserModel.Instance.UpdateUserName(UserModel.Instance.UserId,inputNameText.text);

        switch (statusCode)
        {
            case UserModel.Status.True:
                Debug.Log("�o�^����");
                nameUpdateButton.SetActive(true);
                updateButton.interactable = true;
                break;

            case UserModel.Status.False:
                // �l�b�g�G���[�{�^���\��
                Debug.Log("�ʐM���s");
                netErrorButton.SetActive(true);
                break;

            case UserModel.Status.SameName:
                // �G���[�\��
                Debug.Log("���O���");
                errorButton.SetActive(true);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// �T�E���h�T�E���h�{�^��������
    /// </summary>
    public void OnSoundButton()
    {
        // ���ݕ\������Ă��邩
        if (soundPanel.activeSelf)
        {   // �\�����Ă��鎞
            soundPanel.SetActive(false);
        }
        else
        {
            // SE�Đ�
            SEManager.Instance.Play(SEPath.TAP_BUTTON);
            // �p�l���\��
            DisplayPanel(soundPanel);
        }
    }

    /// <summary>
    /// �w���v�{�^��������
    /// </summary>
    public void OnHelpButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // �p�l���\��
        DisplayPanel(helpPanel);
    }

    /// <summary>
    /// �w���v�l�N�X�g�{�^��������
    /// </summary>
    public void OnHelpNextButton()
    {
        imageNo++;

        // ���l�̏���ݒ�
        if(imageNo >= helpSprites.Length - 1) imageNo = helpSprites.Length - 1;

        // �摜�E�y�[�WNo�X�V
        nowPageText.text = (imageNo + 1).ToString();
        helpImage.sprite = helpSprites[imageNo];
    }

    /// <summary>
    /// �w���v�o�b�N�{�^��������
    /// </summary>
    public void OnHelpBackButton()
    {
        imageNo--;

        // ���l�̉����ݒ�
        if (imageNo <= 0) imageNo = 0;

        // �摜�E�y�[�WNo�X�V
        nowPageText.text = (imageNo + 1).ToString();
        helpImage.sprite = helpSprites[imageNo];
    }

    /// <summary>
    /// �p�l����\������
    /// </summary>
    public void OnCloseDisplay(GameObject gameObject)
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        gameObject.SetActive(false);
    }
}
