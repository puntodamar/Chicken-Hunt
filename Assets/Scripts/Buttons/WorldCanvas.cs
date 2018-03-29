using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvas : MonoBehaviour
{
    private Camera _cameraToLookAt;

    private void Start()
    {
        _cameraToLookAt = Camera.main;
    }

    private void Update()
    {
        Vector3 v = _cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt( _cameraToLookAt.transform.position - v ); 
        transform.Rotate(0,180,0);
    }
}
