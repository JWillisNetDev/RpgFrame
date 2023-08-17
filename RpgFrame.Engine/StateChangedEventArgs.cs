using System;
using RpgFrame.Engine.GameStates;

namespace RpgFrame.Engine;

public class StateChangedEventArgs : EventArgs
{
	public GameState LastState { get; }
	public GameState NewState { get; }

	public StateChangedEventArgs(GameState last, GameState @new)
	{
		LastState = last;
		NewState = @new;
	}
}