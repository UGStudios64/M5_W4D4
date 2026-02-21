using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera mainCamera;
    private Collider col;

    [Header("// SPEED -----------------------------------------------------------------------------------------")]
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    private bool IsMoving;

    // DIRECTIONS -------------------------------------------------------------------------------------------------
    private float horizontal;
    private float vertical;
    private Vector3 direction;

    #region // GET -----------------------------------------------------------------------------------------------------------------------
    public Rigidbody GetRb() => rb;
    public Camera GetMainCamera() => mainCamera;
    public bool GetIsMoving() => IsMoving;
    public float GetHorizontal() => horizontal;
    public float GetVertical() => vertical;
    #endregion


    // GAME //-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-
    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!col) col = GetComponentInChildren<Collider>();
        if (!mainCamera) mainCamera = Camera.main;
    }
    
    void Update()
    {
        if (!col.CompareTag("Player")) return;

        // Take direction
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        IsMoving = horizontal != 0 || vertical != 0;
    }

    private void FixedUpdate()
    {
        if (!col.CompareTag("Player")) return;

        Move();
    }


    // FUNCTIONS //-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-
    private void Move()
    {
        // Move based on the Camera
        direction = mainCamera.transform.forward * vertical + mainCamera.transform.right * horizontal;
        direction.y = 0f;

        // Vector normalized
        if (direction.magnitude > 1f) direction.Normalize();

        // Apply Move and Speed
        Vector3 velocity = direction * speed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);


        // ROTATION //------- Roteate the player in the move direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            rb.MoveRotation(smoothRotation);
        }
    }
}