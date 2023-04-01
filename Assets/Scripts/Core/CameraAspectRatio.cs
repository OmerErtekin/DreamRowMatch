using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    [SerializeField] private float multiplierOnLandscape = 0.75f;
    private Camera orthoCamera;
    private float lastResWidth, lastResHeight;
    private bool isLandscape;
    private int targetCount;
    private void Awake()
    {
        orthoCamera = GetComponent<Camera>();
        lastResHeight = Screen.height;
        lastResWidth = Screen.width;
        isLandscape = lastResHeight < lastResWidth;
    }

    private void Start()
    {
        GridCreator.OnGridCreated += SetCameraVariables;
    }

    private void FixedUpdate()
    {
        IsResolutionChanged();
    }

    private void IsResolutionChanged()
    {
        if (lastResHeight != Screen.height || lastResWidth != Screen.width)
        {
            lastResHeight = Screen.height;
            lastResWidth = Screen.width;
            isLandscape = lastResHeight < lastResWidth;
            SetCamera();
        }
    }

    public void SetCameraVariables(LevelData data)
    {
        targetCount = Mathf.Max(data.gridHeight,data.gridWidth);
        SetCamera();
    }

    public void SetCamera()
    {
        orthoCamera.orthographicSize = !isLandscape ? (targetCount + 1) : (targetCount + 1) * multiplierOnLandscape;
    }

    private void OnDestroy()
    {
        GridCreator.OnGridCreated -= SetCameraVariables;
    }
}