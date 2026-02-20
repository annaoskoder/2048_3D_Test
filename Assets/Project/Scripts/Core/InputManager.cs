using GamePlay;
using UnityEngine;


namespace Core
{

    /// <summary>
    /// Handles player input for dragging and launching cubes. 
    /// Supports both mouse (PC/Editor) and touch (Mobile) controls.
    /// </summary>
    [DisallowMultipleComponent]
    public class InputManager : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Reference to the Spawner that provides the current active cube.")]
        [SerializeField] 
        private Spawner spawner;

        [Header("Launch Settings")]
        [Tooltip("The impulse force applied to the cube when released.")]
        [Range(10f, 150f)]
        [SerializeField] 
        private float launchForce = 50f;

        [Space(10)]
        [Header("Boundary Settings")]
        [Tooltip("The minimum X-axis world position the cube can be dragged to.")]
        [SerializeField] 
        private float minX = -3f;

        [Tooltip("The maximum X-axis world position the cube can be dragged to.")]
        [SerializeField] 
        private float maxX = 3f;


        private Cube currentCube;
        private bool isDragging;
        private Vector2 lastInputPosition;

        // Cached reference to the main camera to optimize performance in the Update loop
        private Camera mainCamera;

        private void Awake()
        {
            this.mainCamera = Camera.main;

            if (this.mainCamera == null)
            {
                Debug.LogError("[InputManager] Main Camera is missing in the scene!");
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            HandleMouseInput();
#else
            HandleTouchInput();
#endif
        }

        #region Mouse (Editor Testing)

        /// <summary>
        /// Processes standard mouse inputs for dragging operations.
        /// </summary>
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
                StartDragging(Input.mousePosition);

            if (Input.GetMouseButton(0) && isDragging)
                Drag(Input.mousePosition);

            if (Input.GetMouseButtonUp(0) && isDragging)
                Release();
        }

        #endregion

        #region Touch (Android)

        /// <summary>
        /// Processes screen touch inputs for dragging operations.
        /// </summary>
        private void HandleTouchInput()
        {
            if (Input.touchCount == 0)
                return;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartDragging(touch.position);
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                        Drag(touch.position);
                    break;

                case TouchPhase.Ended:
                    if (isDragging)
                        Release();
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the drag state if a valid cube is ready to be launched.
        /// </summary>
        /// <param name="inputPosition">The screen coordinates where the input started.</param>
        private void StartDragging(Vector2 inputPosition)
        {
            currentCube = spawner.ActiveCube;

            if (currentCube == null || currentCube.IsLaunched)
                return;

            isDragging = true;
            lastInputPosition = inputPosition;
        }

        /// <summary>
        /// Calculates the required world position from screen coordinates and moves the cube.
        /// </summary>
        /// <param name="screenPosition">The current screen coordinates of the mouse or touch.</param>
        private void Drag(Vector2 screenPosition)
        {
            if (currentCube == null) return;

            // Convert screen position to world space using the cached camera
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(
                    screenPosition.x,
                    screenPosition.y,
                    Camera.main.WorldToScreenPoint(currentCube.transform.position).z
                )
            );

            // Clamp the horizontal movement within the allowed boundaries
            float clampedX = Mathf.Clamp(worldPos.x, minX, maxX);

            currentCube.MoveHorizontal(clampedX);
        }


        /// <summary>
        /// Ends the drag state, launches the active cube, and signals the spawner.
        /// </summary>
        private void Release()
        {
            isDragging = false;

            currentCube.Launch(this.launchForce);
            spawner.ClearActiveCube();

        }
    }
}
