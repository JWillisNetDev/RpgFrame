using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RpgFrame.Engine.GameStates;
using RpgFrame.Engine.Interfaces;
using RpgFrame.Engine.Internal;

namespace RpgFrame.Engine.GameObjects;

public abstract class GameObject
{
	public Guid Id { get; } = Guid.NewGuid();

    public bool Initialized { get; private set; } = false;

	public float Angle { get; set; } = 0f;
    public Color Color { get; set; } = Color.White;
    public bool Enabled { get; set; } = true;
    public Vector2 Position { get; set; } = Vector2.Zero;
    
    public Texture2D Sprite { get; set; }

    public GameState? GameState { get; internal set; }

    protected GameObject(Texture2D sprite)
    {
	    Sprite = sprite;
    }

    protected GameObject(Texture2D sprite, Vector2 position) : this(sprite)
    {
	    Position = position;
    }

    public virtual void OnInitialize()
    {
    }

    public virtual void OnUpdate(GameTime gameTime)
    {
    }

    internal void Initialize()
    {
	    OnInitialize();
	    Initialized = true;
    }

    internal void Update(GameTime gameTime)
    {
	    if (!Enabled) return;

	    OnUpdate(gameTime);
    }
 
    internal void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Sprite, Position, Color.White);
    }
}