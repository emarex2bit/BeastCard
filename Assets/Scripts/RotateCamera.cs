using System.Collections;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 1.0f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        rotationY += sensitivity * Input.GetAxis("Mouse X");
        rotationX -= sensitivity * Input.GetAxis("Mouse Y");
        rotationX = Mathf.Clamp(rotationX, -90, 90);
        rotationY = Mathf.Clamp(rotationY, -90, 90);

       

        transform.eulerAngles = new Vector3(rotationX, rotationY, 0);
        
    }

    public void SetOnCursorLockState()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetOffCursoreLockState()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
