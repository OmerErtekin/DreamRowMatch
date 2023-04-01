using UnityEngine;

public class SwipeInputHandler : MonoBehaviour
{
    #region Variables
    [SerializeField] private LayerMask targetLayerMasks;
    [SerializeField] private float swipeThreshold = 2;
    private RaycastHit raycastHit;
    private Vector2 startSwipePosition;
    private Direction swipeDirection = Direction.None;
    private bool isMadeAMove = false;
    #endregion

    #region Components
    private GameUIController gameUIController;
    private Camera mainCamera;
    public GridObject currentSelected;
    private GridSwiper gridSwiper;
    #endregion
    private void Start()
    {
        gameUIController = GameUIController.Instance;
        gridSwiper = GetComponent<GridSwiper>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (gridSwiper.IsSwiping || gameUIController.IsGameFinished)
            return;

        HandleGridSelect();
        HandleSwipeMovement();
    }

    private void HandleSwipeMovement()
    {
        if (!currentSelected || isMadeAMove)
        {
            swipeDirection = Direction.None;
            return;
        }

        Vector2 currentSwipePosition = Input.mousePosition;
        Vector2 swipeVector = currentSwipePosition - startSwipePosition;
        SetSwipeDirection(swipeVector);
    }

    void SetSwipeDirection(Vector2 swipeVector)
    {
        if (Mathf.Abs(swipeVector.x) < swipeThreshold && Mathf.Abs(swipeVector.y) < swipeThreshold)
        {
            swipeDirection = Direction.None;
            return;
        }

        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            swipeDirection = swipeVector.x > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            swipeDirection = swipeVector.y > 0 ? Direction.Up : Direction.Down;
        }

        isMadeAMove = true;

        if (gridSwiper.CanSwipeTheGrid(currentSelected, swipeDirection))
        {
            gridSwiper.SwipeTheGrid(currentSelected, swipeDirection);
        }
        else
        {
            currentSelected.ShakeTheGrid();
        }
    }

    private void HandleGridSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, 500, targetLayerMasks))
            {
                startSwipePosition = Input.mousePosition;
                currentSelected = raycastHit.collider.gameObject.GetComponent<GridObject>();
                isMadeAMove = false;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isMadeAMove = false;
            currentSelected = null;
        }
    }
}

