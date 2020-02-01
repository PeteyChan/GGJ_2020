﻿using UnityEngine;

[RequireComponent(typeof(CarryWalkState))]
[RequireComponent(typeof(PickupState))]
[RequireComponent(typeof(DropState))]
public class CarryIdleState : State
{
    private Player _player;
    private Rigidbody _rb;

    private void Awake()
    {
        _player = gameObject.Find<Player>();
        _rb = _player.GetComponent<Rigidbody>();
    }

    protected override IState OnUpdate(float deltaTime, float stateTime)
    {
        if (_player.GamePad.GetButton(GamePad.Buttons.face_left).wasPressed)
        {
            return GetComponent<DropState>();
        }
        
        if (_rb.velocity.magnitude > 0.1f)
        {
            return GetComponent<CarryWalkState>();
        }

        return this;
    }
}
