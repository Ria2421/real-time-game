using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Transform _cameraParent;
    private Transform _cameraChild;
    private Transform _camera;

    /// <summary>
    /// �����Ώ�
    /// ���W��CameraParent�̍��W�ɑ��
    /// </summary>
    public Transform LookTarget;

    /// <summary>
    /// �����_����̋����iCameraChild�̃��[�J��Z���W�j
    /// </summary>
    public float Distance;

    /// <summary>
    /// �����_�ւ̉�荞�݊p�x�iCameraParent�̊p�x�j
    /// </summary>
    public Vector2 LookAngles;

    /// <summary>
    /// ���E�I�t�Z�b�g���W�iMain Camera�̃��[�J�����W�j
    /// </summary>
    public Vector2 OffsetPosition;

    /// <summary>
    /// �J�������W�⊮�W��
    /// </summary>
    public float posComp;

    void Start()
    {
        Application.targetFrameRate = 60;
        _cameraParent = transform;
        _cameraChild = _cameraParent.GetChild(0);
        _camera = _cameraChild.GetChild(0);
    }

    void LateUpdate()
    {
        _cameraParent.position = Vector3.Lerp(_cameraParent.position, LookTarget.position, posComp * Time.deltaTime);
        _cameraChild.localPosition = new Vector3(0, 0, -Distance); // �����ɂ���
        _cameraParent.eulerAngles = new Vector2 (LookAngles.x,LookTarget.eulerAngles.y);
        _camera.localPosition = OffsetPosition;
    }
}