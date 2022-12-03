using UnityEngine;

/* CLASS DOCUMENTATION *\
* [Variable Specifics]
* Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
* Dynamically changed: These variables are changed throughout the game.
* 
* [Class Flow]
* 1. This script is responsible for the player movement and player camera controls.
*/

public class PlayerController : MonoBehaviour
{
    [Header("Set in inspector - general settings")]
    [SerializeField] float characterHeight = 1.7f;
    [SerializeField] float mouseSensitivity = 500f;

    [Header("Gravity")]
    [SerializeField] float gravity = 10;

    [Header("Move Stats")]
    [SerializeField] float playerSpeed = 10f;
    [SerializeField] float sprintMultiplier;

    CharacterController charCtrl;
    Transform mainCam;
    Transform topViewCamera;

    #region PRIVATE_VARIABLES
    //Keyboard Input and Movement
    float inputVer = 0;
    float inputHor = 0;

    Vector3 direction;
    Vector3 movement;

    //Mouse Input
    float mouseInputX = 0;
    float mouseInputY = 0;
    float mouseXRotate = 0;

    float sprintBoost = 0;
    #endregion

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CacheComponents();
    }

    /// <summary>
    /// Call to cache the required gameObject components.
    /// </summary>
    void CacheComponents()
    {
        charCtrl = GetComponent<CharacterController>();
        topViewCamera = GameObject.FindGameObjectWithTag("TopViewCamera").transform;
        topViewCamera.gameObject.SetActive(false);
        mainCam = GameObject.FindGameObjectWithTag("PlayerCamera").transform;
    }

    void Start()
    {
        //Move the camera in player head height.
        mainCam.position = transform.position + Vector3.up * characterHeight;
        mainCam.parent = transform;
    }

    void Update()
    {
        //Getting Input on every frame
        inputHor = Input.GetAxis("Horizontal");
        inputVer = Input.GetAxis("Vertical");
        mouseInputY = Input.GetAxis("Mouse Y");
        mouseInputX = Input.GetAxis("Mouse X");

        //Rotation
        transform.Rotate(0, mouseInputX * mouseSensitivity * Time.deltaTime, 0);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sprintBoost = sprintMultiplier;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            sprintBoost = 0;
        }

        //Construct Direction Vectors
        direction = transform.forward * inputVer + transform.right * inputHor;
        direction *= playerSpeed + sprintBoost;

        direction.y = -gravity;

        //Moving the character
        movement = direction * Time.deltaTime;
        charCtrl.Move(movement);
    }

    private void LateUpdate()
    {
        //Control the camera FirstPerson rotation.
        mouseXRotate += -mouseInputY * mouseSensitivity * Time.deltaTime;
        mouseXRotate = Mathf.Clamp(mouseXRotate, -75, 85);
        mainCam.localRotation = Quaternion.Euler(mouseXRotate, 0, 0);
    }
}
