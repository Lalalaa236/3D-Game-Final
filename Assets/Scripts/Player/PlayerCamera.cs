using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public PlayerManager playerManager;
    public Camera cameraObject;
    [SerializeField] private Transform cameraPivotTransform;

    private Vector3 cameraVelocity = Vector3.zero;
    private Vector3 cameraObjectPosition = Vector3.zero;
    private float cameraSmoothTime = 1f;
    // private Vector3 cameraOffset = new Vector3(0, 2, -7);

    [SerializeField] private float lrLookAngle;
    [SerializeField] private float udLookAngle;

    [SerializeField] private float lrRotationVelocity = 220f;
    [SerializeField] private float udRotationVelocity = 220f;

    [SerializeField] private float minimumLookAngle = -35f;
    [SerializeField] private float maximumLookAngle = 65f;

    // Collision
    [SerializeField] private float collisionRadius = 0.2f;
    [SerializeField] private LayerMask ignoreLayers;
    private float defaultPosition;
    private float targetPosition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        defaultPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleCameraAction()
    {
        // Handle camera movement and rotation based on player input
        if (playerManager != null)
        {
            FollowTarget();
            HandleRotation();
            HandleCollision();
        }
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, playerManager.transform.position, ref cameraVelocity, cameraSmoothTime * Time.deltaTime);
        transform.position = targetPosition;


        // Vector3 desiredPosition = playerManager.transform.position + cameraOffset;
        // Vector3 targetPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref cameraVelocity, cameraSmoothTime * Time.deltaTime);
        // transform.position = targetPosition;
    }

    private void HandleRotation()
    {
        lrLookAngle += InputManager.instance.horizontalCamera * lrRotationVelocity * Time.deltaTime;
        udLookAngle -= InputManager.instance.verticalCamera * udRotationVelocity * Time.deltaTime;
        udLookAngle = Mathf.Clamp(udLookAngle, minimumLookAngle, maximumLookAngle);

        Vector3 rotation = Vector3.zero;

        rotation.y = lrLookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = udLookAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    private void HandleCollision()
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, collisionRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
        {
            float distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(distance - collisionRadius);
        }

        if (Mathf.Abs(targetPosition) < collisionRadius)
        {
            targetPosition = -collisionRadius;
        }

        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }
}
