using Godot;
using System;

public enum ControlMovementDir { Forward, Reverse };
public enum ControlTurningDir { Left, Right };

public partial class MovementBase : RigidBody3D
{
	protected enum MovementDir { None, Forward, Reverse };
	protected enum TurningDir { None, Left, Right };
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

    public override void _Process(double delta)
    {
		if (_consumeNextAccBrk)
		{
			_nextAccelerate = false;
			_nextBrake = false;
			if (_speed < Mathf.Epsilon)
			{
				_movementDir = MovementDir.None;
			}
		}
		if (_consumeNextTurn)
		{
            _nextTurn = false;
        }
    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		Vector3 upDir = Vector3.Up;
		Transform3D tr = this.Transform;
		bool updateLinear = false;
		if (_nextTurn)
		{
			float rotationAngle = Mathf.DegToRad(_turningDir == TurningDir.Left ? MaxTurning : -MaxTurning);

			state.AngularVelocity = upDir * rotationAngle;
			updateLinear = true;
		}
		else if (_consumeNextTurn)
		{
            state.AngularVelocity = Vector3.Zero;
			updateLinear = true;
        }
		if (_nextAccelerate || _nextBrake)
		{
			if (_nextAccelerate)
			{
				_speed += MaxAcceleration * state.Step;
			}
			else if (_nextBrake)
			{
                _speed -= MaxBraking * state.Step;
            }
			_speed = Mathf.Clamp(_speed, .0f, MaxSpeed);
			updateLinear = true;
        }
		if (updateLinear)
            state.LinearVelocity = _movementDir == MovementDir.Forward ? tr.Basis.Z * _speed : -tr.Basis.Z * _speed;

        _consumeNextAccBrk = _consumeNextTurn = true;
    }


    public void Accelerate(ControlMovementDir dir)
	{
		if (CanAccelerate())
			AccelerateInner(dir);
	}

	protected virtual void AccelerateInner(ControlMovementDir dir)
	{
		MovementDir cnvrtDir = ConvertMovement(dir);

        if (_movementDir == MovementDir.None ||
			_movementDir == cnvrtDir)
		{
			_movementDir = cnvrtDir;
			_nextAccelerate = true;
		}
		else if (_movementDir != cnvrtDir)
		{
			_nextAccelerate = false;
			_nextBrake = true;
		}
        _consumeNextAccBrk = false;
    }

	public void Brake()
	{
        _nextBrake = true;
        _consumeNextAccBrk = false;
    }

	public void Turn(ControlTurningDir dir)
	{
        if (CanAccelerate())
            TurnInner(dir);
    }

	protected virtual void TurnInner(ControlTurningDir dir)
	{
		_turningDir = ConvertTurning(dir);
		_nextTurn = true;
		_consumeNextTurn = false;
    }

	public virtual float MaxSpeed {  get { return 1.0f; } }
    public virtual float MaxAcceleration { get { return 0.1f; } }
    public virtual float MaxBraking { get { return 1.0f; } }
    public virtual float MaxTurning { get { return 30.0f; } }
	public virtual bool CanAccelerate() {  return true; }
	protected bool _nextAccelerate = false, _nextBrake = false, _nextTurn = false,
				   _consumeNextAccBrk = false, _consumeNextTurn = false;

	protected float _speed;
	protected MovementDir _movementDir;
	protected TurningDir _turningDir;

	protected static MovementDir ConvertMovement(ControlMovementDir dir)
	{
		switch (dir)
		{
			case ControlMovementDir.Forward: return MovementDir.Forward;
			default: return MovementDir.Reverse;
		}
	}

    protected static TurningDir ConvertTurning(ControlTurningDir dir)
    {
        switch (dir)
        {
            case ControlTurningDir.Left: return TurningDir.Left;
            default: return TurningDir.Right;
        }
    }
}
