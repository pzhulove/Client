using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 锁定旋转值
/// </summary>
public class GeFixedRotate : MonoBehaviour
{
    [SerializeField] private Vector3 _rotateValue = Vector3.zero;

    private Quaternion _rotateQuaternion;

    void Awake()
    {
        _rotateQuaternion = Quaternion.Euler(_rotateValue);
        transform.localRotation = _rotateQuaternion;
    }
    
    void Update()
    {
        var curRotate = transform.localRotation;
        if (curRotate != _rotateQuaternion)
            transform.localRotation = _rotateQuaternion;
    }
}
