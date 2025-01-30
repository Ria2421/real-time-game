using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Transform _cameraParent;
    private Transform _cameraChild;
    private Transform _camera;

    /// <summary>
    /// 豕ｨ隕門ｯｾ雎｡
    /// 蠎ｧ讓吶ｒCameraParent縺ｮ蠎ｧ讓吶↓莉｣蜈･
    /// </summary>
    public Transform LookTarget;

    /// <summary>
    /// 豕ｨ隕也せ縺九ｉ縺ｮ霍晞屬（CameraChild縺ｮ繝ｭ繝ｼ繧ｫ繝ｫZ蠎ｧ讓呻ｼ�
    /// </summary>
    public float Distance;

    /// <summary>
    /// 豕ｨ隕也せ縺ｸ縺ｮ蝗槭ｊ霎ｼ縺ｿ隗貞ｺｦ（CameraParent縺ｮ隗貞ｺｦ）
    /// </summary>
    public Vector2 LookAngles;

    /// <summary>
    /// 隕也阜繧ｪ繝輔そ繝��ヨ蠎ｧ讓呻ｼ�Main Camera縺ｮ繝ｭ繝ｼ繧ｫ繝ｫ蠎ｧ讓呻ｼ�
    /// </summary>
    public Vector2 OffsetPosition;

    /// <summary>
    /// 繧ｫ繝｡繝ｩ蠎ｧ讓呵｣懷ｮ御ｿよ焚
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
        _cameraChild.localPosition = new Vector3(0, 0, -Distance); // 雋�謨ｰ縺ｫ縺吶ｋ
        _cameraParent.eulerAngles = new Vector2 (LookAngles.x,LookTarget.eulerAngles.y);
        _camera.localPosition = OffsetPosition;
    }
}