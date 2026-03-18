using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private InputManager IM;
    private Animator _animator;
    private bool _hasAnimator;
    private int _xVelHash;
    private int _yVelHash;
    public float _walkSpeed = 2f;
    private Vector2 _currentVelocity;

    private void Start()
    {
        _hasAnimator = TryGetComponent<Animator>(out _animator);
        rb = GetComponent<Rigidbody>();
        IM = GetComponent<InputManager>();
        _xVelHash = Animator.StringToHash("X_Velocity");
        _yVelHash = Animator.StringToHash("Y_Velocity");
    }

    public void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if(!_hasAnimator) return;

        float targetSpeed = IM.Move.magnitude * _walkSpeed;
        if(IM.Move == Vector2.zero) targetSpeed = 0.1f;

        _currentVelocity.x = targetSpeed * IM.Move.x;
        _currentVelocity.y = targetSpeed * IM.Move.y;  

        var xVelDifference = _currentVelocity.x - rb.linearVelocity.x;
        var zVelDifference = _currentVelocity.y - rb.linearVelocity.z;

        rb.AddForce(transform.TransformVector(new Vector3(xVelDifference,0,zVelDifference)), ForceMode.VelocityChange);
        _animator.SetFloat(_xVelHash, _currentVelocity.x);
        _animator.SetFloat(_yVelHash, _currentVelocity.y);
    }
}
