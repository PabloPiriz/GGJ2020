using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGyroscopeTesting : MonoBehaviour
{
    // Faces for 6 sides of the cube
    private GameObject[] quads;

    public Camera _camera;

    public float _speedX = 4;
    public float _speedY = 4;
    private float yaw = 0;
    private float pitch = 0;
    private Vector3 mCurrentFacing = new Vector3();


    void Start()
    {
        // make camera solid colour and based at the origin
        _camera.backgroundColor = new Color(49.0f / 255.0f, 77.0f / 255.0f, 121.0f / 255.0f);
        _camera.transform.position = new Vector3(0, 0, 0);
        _camera.clearFlags = CameraClearFlags.SolidColor;

        mCurrentFacing = _camera.transform.forward;

        quads = new GameObject[6];

        // create the six quads forming the sides of a cube
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);

        quads[0] = createQuad(quad, new Vector3(1, 0, 0), new Vector3(0, 90, 0), "plus x",
            Color.blue);
        quads[1] = createQuad(quad, new Vector3(0, 1, 0), new Vector3(-90, 0, 0), "plus y",
            Color.red);
        quads[2] = createQuad(quad, new Vector3(0, 0, 1), new Vector3(0, 0, 0), "plus z",
            Color.white);
        quads[3] = createQuad(quad, new Vector3(-1, 0, 0), new Vector3(0, -90, 0), "neg x",
            Color.cyan);
        quads[4] = createQuad(quad, new Vector3(0, -1, 0), new Vector3(90, 0, 0), "neg y",
            Color.green);
        quads[5] = createQuad(quad, new Vector3(0, 0, -1), new Vector3(0, 180, 0), "neg z",
            Color.yellow);

        GameObject.Destroy(quad);

        foreach (var item in quads)
        {
            item.SetActive(false);
        }
    }

    // make a quad for one side of the cube
    GameObject createQuad(GameObject quad, Vector3 pos, Vector3 rot, string name, Color col)
    {
        Quaternion quat = Quaternion.Euler(rot);
        GameObject GO = Instantiate(quad, pos, quat);
        GO.name = name;
        GO.GetComponent<Renderer>().material.color = col;
        //GO.GetComponent<Renderer>().material.mainTexture = t;
        GO.transform.localScale += new Vector3(0.25f, 0.25f, 0.25f);
        return GO;
    }

    protected void FixedUpdate()
    {
#if UNITY_EDITOR
        mouseRotateCamera();
#elif UNITY_IOS
        gyroModifyCamera();
#endif
        mCurrentFacing = _camera.transform.forward;
    }

    protected void OnGUI()
    {
        GUI.skin.label.fontSize = Screen.width / 15;
        if (!CTransitionManager.Inst.IsScreenCovered())
        {
#if UNITY_IOS
            GUILayout.Label("Orientation: " + Screen.orientation);
            GUILayout.Label("input.gyro.attitude: " + Input.gyro.attitude);
            GUILayout.Label("iphone width/font: " + Screen.width + " : " + GUI.skin.label.fontSize);
#endif
            GUILayout.Label("CameraQuaternion: " + _camera.transform.rotation);
        }
    }

    /********************************************/

    // The Gyroscope is right-handed.  Unity is left handed.
    // Make the necessary change to the camera.
    private void gyroModifyCamera()
    {
        transform.rotation = gyroToUnity(Input.gyro.attitude);


    }

    private static Quaternion gyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    private void mouseRotateCamera()
    {
        if (Input.GetMouseButton(0))
        {
            //Debug.Log("button down!");
            yaw += _speedX * Input.GetAxis("Mouse X");
            pitch -= _speedY * Input.GetAxis("Mouse Y");

            _camera.transform.eulerAngles = new Vector3(pitch, yaw, 0);
        }
        /*
        var c = _camera.transform;
        c.Rotate(0, Input.GetAxis("Mouse X") * _sensitivity, 0);
        c.Rotate(-Input.GetAxis("Mouse Y") * _sensitivity, 0, 0);
        c.Rotate(0, 0, -Input.GetAxis("QandE") * 90 * Time.deltaTime);*/
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_camera.transform.position, _camera.transform.forward);
    }

    public Vector3 getCurrentFacing()
    {
        return mCurrentFacing;
    }
}
