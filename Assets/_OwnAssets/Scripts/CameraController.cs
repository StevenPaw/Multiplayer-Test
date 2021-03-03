using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField]
    private float offsetX = 0;
    [SerializeField]
    private float offsetY = 0;
    [SerializeField]
    private float offsetZ = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            Camera.main.transform.position =
                this.transform.position - new Vector3(offsetX,offsetY,offsetZ);
            Camera.main.transform.LookAt(this.transform.position);
            Camera.main.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
