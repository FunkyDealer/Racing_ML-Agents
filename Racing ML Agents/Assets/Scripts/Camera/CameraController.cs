using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    List<RacingTrainerManager> trainers;
    [SerializeField]
    KeyCode changePosKey;
    bool changePos;

    Transform currentTarget;
    int targetNr;

    Vector3 originalPos;
    Quaternion originalRot;

    Camera camera;
    [SerializeField]
    LayerMask visibleMask;
    [SerializeField]
    LayerMask invisibleMask;
    bool visible = true;

    private void Awake()
    {
        originalPos = transform.position;
        originalRot = transform.rotation;

        camera = GetComponent<Camera>();
        camera.cullingMask = visibleMask;
    }

    // Start is called before the first frame update
    void Start()
    {
        targetNr = 0;
        currentTarget = trainers[targetNr].GetCameraPos;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(changePosKey))
        {
            targetNr++;
            if (targetNr >= trainers.Count) targetNr = 0;

            currentTarget = (trainers[targetNr]).GetCameraPos;
            transform.rotation = currentTarget.rotation;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            transform.position = originalPos;
            transform.rotation = originalRot;
            currentTarget = transform;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            visible = !visible;

            if (visible) camera.cullingMask = visibleMask; 
            else camera.cullingMask = invisibleMask;
        }

    }

    private void FixedUpdate()
    {
        

        transform.localPosition = Vector3.Lerp(transform.position, currentTarget.position, 0.03f * 10);

        ResetInput();
    }

    void ResetInput()
    {
        changePos = false;
    }

}
