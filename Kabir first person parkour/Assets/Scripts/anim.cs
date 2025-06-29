using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;



    private void Start()
    {
        //lock cursor in middle screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        // mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);


    }
}
