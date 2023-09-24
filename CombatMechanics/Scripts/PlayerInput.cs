using Godot;
using System;

public partial class PlayerInput : Node
{
	public override void _Ready()
	{
		Node3D rootNode = GetParent<Node3D>();
        MovementBase ship = rootNode.GetNode<MovementBase>("Ship");
		if (ship != null)
		{
			_controlledShip = ship;
            _controlledShip.SetPhysicsProcess(true);
            _controlledShip.CanSleep = false;
            _controlledShip.Sleeping = false;
        }
    }

	public override void _Input(InputEvent ev)
	{
		//GD.Print(ev.AsText());
		if (ev.IsAction("forward"))
		{
			GD.Print("Forward");
			if (_controlledShip != null)
                _controlledShip.Accelerate(ControlMovementDir.Forward);
		}
		else if (ev.IsAction("backward"))
		{
			if (_controlledShip != null)
				_controlledShip.Accelerate(ControlMovementDir.Reverse);
			GD.Print("Backward");
		}
		else if (ev.IsAction("brake"))
		{
			if (_controlledShip != null)
				_controlledShip.Brake();
			GD.Print("Brake");
		}
		else if (ev.IsAction("left"))
		{
			if (_controlledShip != null)
				_controlledShip.Turn(ControlTurningDir.Left);
			GD.Print("Left");
		}
		else if (ev.IsAction("right"))
		{
			if (_controlledShip != null)
				_controlledShip.Turn(ControlTurningDir.Right);
			GD.Print("Right");
		}
	}

	private MovementBase _controlledShip = null;
}
