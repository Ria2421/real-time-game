//---------------------------------------------------------------
// �\���}�l�[�W���[ [ SoloManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2024/12/10
//---------------------------------------------------------------
using DavidJalbert;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoloManager : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    /// <summary>
    /// �����b�v��
    /// </summary>
    private int currentRapNum = 1;

    /// <summary>
    /// ���Ԍv���p
    /// </summary>
    private float timer;

    /// <summary>
    /// �v���t���O
    /// </summary>
    private bool isCount = false;

    /// <summary>
    /// ������̓I�u�W�F
    /// </summary>
    [SerializeField] private GameObject inputManager;

    /// <summary>
    /// �ԑ̔��j�}�l�[�W���[
    /// </summary>
    [SerializeField] private TinyCarExplosiveBody boomManager;

    /// <summary>
    /// �ő僉�b�v��
    /// </summary>
    [SerializeField] private int maxRapNum = 0;

    /// <summary>
    /// ���U���g�p�l��
    /// </summary>
    [SerializeField] private GameObject resultPanel;

    /// <summary>
    /// �v���^�C�}�[�p�l��
    /// </summary>
    [SerializeField] private GameObject timerPanel;

    /// <summary>
    /// �J�E���g�_�E���p�e�L�X�g
    /// </summary>
    [SerializeField] private Text countDownText;

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
        // �t���O������
        isCount = false;

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
    public void AddRapCnt()
    {
        currentRapNum++;    // ���b�v�����Z

        if(currentRapNum == maxRapNum + 1)
        {
            // �I������
            isCount = false;                // �^�C�}�[�X�g�b�v
            Invoke("BoomCar", 1);           // �ԑ̔��j
            resultPanel.SetActive(true);    // ���U���g�\��
            
            // �^�C�}�[�ړ�����
            var sequence = DOTween.Sequence();
            sequence.Append(timerPanel.transform.DOScale(1.7f, 0.7f));
            sequence.Append(timerPanel.transform.DOLocalMove(new Vector3(0,80,0), 0.5f));
            sequence.Play();

            Debug.Log("owari");
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
        countDownText.text = "";
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
        for (int i = 3; i > 0; i--)
        {
            countDownText.text = i.ToString();

            // 1�b�҂��ăR���[�`�����f
            yield return new WaitForSeconds(1.0f);

            if (i == 1)
            {
                // �v���J�n�E����\��
                countDownText.text = "GO!";
                inputManager.SetActive(true);
                isCount = true;

                // �J�E���g��\��
                Invoke("HiddenCount", 0.6f);
            }
        }
    }

    /// <summary>
    /// ���j���[�J�ڏ���
    /// </summary>
    public void OnBackMenu()
    {
        SceneManager.LoadScene("02_MenuScene");
    }
}
