using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using RpgFrame.Engine.GameObjects;
using RpgFrame.Engine.Internal;

namespace RpgFrame.Engine.GameStates;

public abstract class GameState /*: IDisposable*/
{
	private readonly GameObjectCollection _gameObjects = new();
	public IReadOnlyCollection<GameObject> GameObjects => _gameObjects;

	internal Messenger Messenger { get; }

	protected GameState()
	{
		Messenger = new Messenger();
	}

	protected virtual void OnInitialize() { }
	protected virtual void OnUpdate(GameTime gameTime) { }

	protected void AddGameObject(GameObject gameObject)
	{
		_gameObjects.Add(gameObject);
		gameObject.GameState = this;
	}

	internal void Initialize()
	{
		// The game-state is initialized before game-objects
		OnInitialize();
		
		foreach (var gameObject in _gameObjects)
		{
			gameObject.Initialize();
		}
	}

	internal void Update(GameTime gameTime)
	{
		OnUpdate(gameTime);

		foreach (var gameObject in _gameObjects)
		{
			gameObject.Update(gameTime);
		}
	}
}