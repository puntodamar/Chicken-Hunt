using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float RotateSpeed = 100f;
    public bool PingPong = false;

    [Range(0,360)]
    public float PingPongAngle = 120f;

    private float _currentAngle = 0;
    
    private void Update()
    {
        if(!PingPong)
            transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime);
        
        else
            //transform.Rotate(Vector3.up,Mathf.PingPong(Time.time * RotateSpeed,PingPongAngle));
            transform.rotation = Quaternion.Euler(0f, Mathf.PingPong(Time.time * RotateSpeed, 90f), 0f);
    }
}
