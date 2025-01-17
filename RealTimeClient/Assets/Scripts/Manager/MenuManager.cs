//---------------------------------------------------------------
// ���j���[�}�l�[�W���[ [ MenuManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2024/12/10
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

    [Header("---- Button ----")]

    // ���j���[�{�^��
    [SerializeField] private Button acountButton;       // �A�J�E���g
    [SerializeField] private Button shopButton;         // �V���b�v
    [SerializeField] private Button optionButton;       // �I�v�V����
    [SerializeField] private Button updateButton;       // �X�V�ҏW
    [SerializeField] private GameObject errorButton;    // �G���[ (���O���)
    [SerializeField] private GameObject netErrorButton; // �G���[ (�ʐM�G���[)

    // �T�E���h�X���C�_�[
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    // ���j���[�p�l��
    [SerializeField] private GameObject acountPanel;
    [SerializeField] private GameObject soundPanel;

    // �A�J�E���g�p�l���\��UI
    [SerializeField] private Text displayNameText;
    [SerializeField] private Text inputNameText;
    [SerializeField] private Text registText;
    [SerializeField] private Text rateText;

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        // BGM�Đ�
        //�Đ�����BGM�̖��O��S�Ď擾
        var currentBGMNames = BGMManager.Instance.GetCurrentAudioNames();

        if (currentBGMNames[0] != "MainBGM")
        {   // MainBGM���ĊJ
            BGMManager.Instance.Stop(BGMPath.TIME_ATTACK);
            BGMManager.Instance.Stop(BGMPath.MULTI_PLAY);
            BGMManager.Instance.Play(BGMPath.MAIN_BGM, 0.75f, 0, 1, true, true);
        }

        //++ �v���t�B�[�����擾�EUI���f

        //++ �V���b�v���擾�EUI���f
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    void Update()
    {
        
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
        SceneManager.LoadScene("03_SoloSelectScene");
    }

    /// <summary>
    /// �I�����C���{�^��������
    /// </summary>
    public void OnOnlineButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // �I�����C�����[�h�J��
        SceneManager.LoadScene("05_MatchingScene");
    }

    /// <summary>
    /// �^�C�g���{�^��������
    /// </summary>
    public void OnTitleButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // �I�����C�����[�h�J��
        SceneManager.LoadScene("01_TitleScene");
    }

    /// <summary>
    /// �A�J�E���g�{�^��������
    /// </summary>
    public async void OnAcountButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // ���[�U�[�f�[�^�̎擾
        var userData =  await UserModel.Instance.SearchUserID(UserModel.Instance.UserId);

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
            acountPanel.SetActive(true);
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
                //++ �ύX�����E�B���h�E��\��
                Debug.Log("�o�^����");
                break;

            case UserModel.Status.False:
                // �l�b�g�G���[�{�^���\��
                Debug.Log("�ʐM���s");
                netErrorButton.SetActive(false);
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
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        soundPanel.SetActive(true);
    }

    /// <summary>
    /// �p�l����\������
    /// </summary>
    public void OnCloseDisplay(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �G���[�{�^����\���E�X�V�{�^������
    /// </summary>
    public void OnErrorButton()
    {
        errorButton.SetActive(false);
        netErrorButton.SetActive(false);
        updateButton.interactable=true;
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
}
