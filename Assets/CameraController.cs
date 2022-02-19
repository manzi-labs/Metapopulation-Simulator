using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector2 inputDir;

    public float cameraSpeed;
    // Start is called before the first frame update
    void Start()
    {
        // this.GetComponent<Camera>().orthographic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(cameraSpeed * Time.deltaTime,0,0));
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-cameraSpeed * Time.deltaTime,0,0));
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0,-cameraSpeed * Time.deltaTime,0));
        }
        if(Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0,cameraSpeed * Time.deltaTime,0));
        }
        
        float scroll = Input.GetAxis ("Mouse ScrollWheel");
        transform.Translate(0, 0, (scroll * 4) * cameraSpeed);
    }
}
