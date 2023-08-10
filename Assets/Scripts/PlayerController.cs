using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    public Camera playerCam;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    public float walkSpeed = 5f;
    private bool isGrounded = false;

    public KeyCode crouchButton = KeyCode.LeftControl;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;
    public float fov = 70f;
    public KeyCode jumpButton = KeyCode.Space;
    public float jumpForce = 5f;
    private bool isWalking = false;
    public float maxVelocityChange = 10f;

    private bool isCrouched = false;
    private Vector3 originalScale;
    private GameObject[] myLight;



    private void Awake()
    {
        myLight = GameObject.FindGameObjectsWithTag("Light");
        rb = GetComponent<Rigidbody>();
        playerCam.fieldOfView = fov;
        originalScale = transform.localScale;

    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGrounded();

        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X");
        pitch -= Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -75f, 75f);
        transform.localEulerAngles = new Vector3(0, yaw, 0);
        playerCam.transform.localEulerAngles = new Vector3(pitch, 0, 0);

        if (Input.GetKeyDown(jumpButton) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(crouchButton))
        {
            Crouch();
        }
        else if(Input.GetKeyUp(crouchButton))
        {
            Crouch();
        }

        CheckForGrounded();
    }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (targetVelocity.x != 0 || targetVelocity.z != 0 && isGrounded)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void CheckForGrounded()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;
        

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            //Debug.DrawRay(origin, direction * distance, Color.red);
            foreach(GameObject lightObject in myLight)
            {
                lightObject.GetComponent<Light>().enabled = false;
            }
            isGrounded = true;
        }
        else
        {
            foreach (GameObject lightObject in myLight)
            {
                lightObject.GetComponent<Light>().enabled = true;
            }
            isGrounded = false;
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void Crouch()
    {
        if (isCrouched)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            walkSpeed /= speedReduction;

            isCrouched = false;
        }
        else
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            walkSpeed *= speedReduction;

            isCrouched = true;
        }
    }
}
