using UnityEngine;


namespace GamePlay
{
    public class InputController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CubeSpawner spawner;

        [Header("Settings")]
        [SerializeField] private float launchForce = 50f;

        [SerializeField] private float minX = -3f;
        [SerializeField] private float maxX = 3f;


        private Cube currentCube;
        private bool isDragging;
        private Vector2 lastInputPosition;

        private void Update()
        {
#if UNITY_EDITOR
            HandleMouseInput();
#else
            HandleTouchInput();
#endif
        }

        #region Mouse (Editor Testing)

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

        private void StartDragging(Vector2 inputPosition)
        {
            currentCube = spawner.ActiveCube;

            if (currentCube == null || currentCube.IsLaunched)
                return;

            isDragging = true;
            lastInputPosition = inputPosition;
        }

        private void Drag(Vector2 screenPosition)
        {
            if (currentCube == null) return;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(
                    screenPosition.x,
                    screenPosition.y,
                    Camera.main.WorldToScreenPoint(currentCube.transform.position).z
                )
            );

            float clampedX = Mathf.Clamp(worldPos.x, minX, maxX);

            currentCube.MoveHorizontal(clampedX);
        }



        private void Release()
        {
            isDragging = false;

            currentCube.Launch(this.launchForce);
            spawner.ClearActiveCube();

        }
    }
}
