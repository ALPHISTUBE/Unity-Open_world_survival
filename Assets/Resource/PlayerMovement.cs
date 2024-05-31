using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Component")]
    public Transform body;
    public CharacterController cc;

    [Header("Variable")]
    public float MouseSensitivity;
    public float moveSpeed;
    public float jumpHeight = 3f;
    public float gravity = 9.8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = .4f;


    float xRot;
    Vector3 velocity;
    bool isGrounded;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Shader.SetGlobalVector("_player", groundCheck.position);

        //check for ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //Camera Rotation
        Vector2 mosMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 100 * MouseSensitivity * Time.deltaTime;

        xRot -= mosMovement.y;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        body.Rotate(Vector3.up * mosMovement.x);

        //Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = body.right * x + body.forward * z;
        cc.Move(move * moveSpeed * Time.deltaTime);


        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);
    }
}
