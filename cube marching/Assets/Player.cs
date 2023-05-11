using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //public GameObject cameraObject;
    Vector3 playerMoveDirection;
    public CharacterController characterController;
    public float playerSpeed = 1;
    public float sensitivity = 1;
    [SerializeField] private float clampAngle = 85f;


    private float verticalRotation;
    private float horizontalRotation;

    private void Start()
    {
        playerMoveDirection = new Vector3(0, 0, 0);
        characterController = gameObject.GetComponent<CharacterController>();
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = gameObject.transform.eulerAngles.y;
    }
    // Update is called once per frame
    void Update()
    {
        Look();
        playerMoveDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            playerMoveDirection.x += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerMoveDirection.x -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerMoveDirection.z += 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerMoveDirection.z -= 1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            playerMoveDirection.y += 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            playerMoveDirection.y -= 1;
        }

        characterController.Move(gameObject.transform.TransformDirection(playerMoveDirection) * Time.deltaTime * playerSpeed);
    }

    private void Look()
    {
        float mouseVertical = Input.GetAxis("Mouse Y");
        float mouseHorizontal = Input.GetAxis("Mouse X");

        verticalRotation += mouseVertical * sensitivity * Time.deltaTime;
        horizontalRotation += mouseHorizontal * sensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

        //transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f); //changed
        gameObject.transform.rotation = Quaternion.Euler(0f, horizontalRotation, verticalRotation);
    }
}
