using UnityEngine;

public class GameInputHandler : MonoBehaviour
{
    #region Variables
    [SerializeField] private LayerMask targetLayerMasks;
    [SerializeField] private int swipeThreshold = 2;
    private RaycastHit raycastHit;
    private Vector2 startSwipePosition;
    private Direction swipeDirection = Direction.None;
    private bool isMadeAMove = false;
    #endregion

    #region Components
    private Camera mainCamera;
    public Grid currentSelected;
    private GridController gridController;
    #endregion
    private void Start()
    {
        gridController = GridController.Instance;
        mainCamera = GetComponent<Camera>();
        if(mainCamera == null )
        {
            Debug.LogWarning("Please add input handler to main camera!");
        }
    }

    private void Update()
    {
        if (gridController.IsSwiping)
            return;
        HandleGridSelect();
        HandleSwipeMovement();
    }


    private void HandleSwipeMovement()
    {
        if (!currentSelected || isMadeAMove)
        {
            this.swipeDirection = Direction.None;
            return;
        }

        Vector2 currentSwipePosition = Input.mousePosition;
        Vector2 swipeDirection = currentSwipePosition - startSwipePosition;
        SetSwipeDirection(swipeDirection);
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

        if(gridController.CanSwipeTheGrid(currentSelected, swipeDirection))
        {
            isMadeAMove = true;
            gridController.SwipeTheGrid(currentSelected, swipeDirection);
        }
        else
        {
            isMadeAMove = true;
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
                currentSelected = raycastHit.collider.gameObject.GetComponent<Grid>();

                isMadeAMove = false;

                if (currentSelected.ObjectType == GridObjectTypes.Matched)
                {
                    isMadeAMove = true;
                    currentSelected = null;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isMadeAMove = false;
            currentSelected = null;
        }
    }
}

public enum Direction { Right, Left, Up, Down, None }
