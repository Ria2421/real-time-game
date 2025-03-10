//---------------------------------------------------------------
// 車用カメラ [ UserModel.cs ]
// Author:Kenta Nakamoto
//---------------------------------------------------------------
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Transform _cameraParent;
    private Transform _cameraChild;
    private Transform _camera;

    /// <summary>
    /// 注視対象
    /// 座標をCameraParentの座標に代入
    /// </summary>
    public Transform LookTarget;

    /// <summary>
    /// 注視点からの距離（CameraChildのローカルZ座標）
    /// </summary>
    public float Distance;

    /// <summary>
    /// 注視点への回り込み角度（CameraParentの角度）
    /// </summary>
    public Vector2 LookAngles;

    /// <summary>
    /// 視界オフセット座標（Main Cameraのローカル座標）
    /// </summary>
    public Vector2 OffsetPosition;

    /// <summary>
    /// カメラ座標補完係数
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
        _cameraChild.localPosition = new Vector3(0, 0, -Distance); // 負数にする
        _cameraParent.eulerAngles = new Vector2 (LookAngles.x,LookTarget.eulerAngles.y);
        _camera.localPosition = OffsetPosition;
    }
}