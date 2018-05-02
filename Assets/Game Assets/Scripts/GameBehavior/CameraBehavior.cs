using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public float dragSpeed = 2;
    private Vector3 dragOrigin;

    public GameObject board;

    public Transform MenuPosition;
    public Transform GamePosition;

    private Transform defaultCameraPosition;
    private bool movingCamera = false;
    private bool shouldBeStatic = true;
    private bool staticCamera = true;

    public int movementSpeed = 2;

    void Start()
    {
        moveCameraTo(MenuPosition);
    }

    void FixedUpdate()
    {
        if (movingCamera)
        {
            transform.position = Vector3.Lerp(transform.position, defaultCameraPosition.position, movementSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, defaultCameraPosition.rotation, movementSpeed * Time.deltaTime);

            if (Vector3.Distance(defaultCameraPosition.transform.position, transform.position) < 0.001f)
            {
                movingCamera = false;
                if (defaultCameraPosition == GamePosition) staticCamera = shouldBeStatic;
            }
        }
        else if (!staticCamera) playerControl();
    }

    private void playerControl()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, 0, 0);

        if (!(transform.position.x + move.x >= -1 && transform.position.x + move.x < 0)) return;


        transform.Translate(move, Space.World);
        transform.LookAt(board.transform);
    }

    public void moveCameraTo(Transform target)
    {
        defaultCameraPosition = target;
        movingCamera = true;
    }

    public void disableControl()
    {
        shouldBeStatic = true;
        staticCamera = true;
    }

    public void enableControl()
    {
        shouldBeStatic = false;
        staticCamera = false;
    }
}
