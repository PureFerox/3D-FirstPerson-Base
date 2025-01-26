using Godot;
using System.Collections.Generic;

public partial class StateMachineComponent : Node
{
	[Export] private Entity _body;
	[Export] private NodePath _initialState;
	private Dictionary<string, State> _states;
	private State _currentState;
	
	public override void _Ready()
	{
		_states = new Dictionary<string, State>();
		foreach (Node node in GetChildren()) {
			if (node is State state) {
				_states[node.Name] = state;
				state.Body = _body;
				state.StateMachineComponent = this;
				state.Exit();
			}
		}
		_currentState = GetNode<State>(_initialState);
		_currentState.Enter();
	}

	public override void _Process(double delta)
	{
		_currentState.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		_currentState.PhysicsUpdate(delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		_currentState.HandleInput(@event);
	}

	public void TransitionTo(string key) {
		if (!_states.ContainsKey(key) || _currentState == _states[key])
			return;

		_currentState.Exit();
		_currentState = _states[key];
		_currentState.Enter();
	}
}
