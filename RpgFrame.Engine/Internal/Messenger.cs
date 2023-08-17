using System.Runtime.CompilerServices;
using RpgFrame.Engine.Interfaces;

namespace RpgFrame.Engine.Internal;

// Drawing massive inspiration from the MVVM dotnet Community Toolkit :)
// https://github.com/CommunityToolkit/dotnet/blob/main/src/CommunityToolkit.Mvvm/Messaging/WeakReferenceMessenger.cs

public class Messenger
{
	public delegate void MessageHandler<in TRecipient, in TMessage>(TRecipient recipient, TMessage message)
		where TRecipient : class
		where TMessage : class;

	// Contains mapping of message-type to recipients and corresponding handlers.
	private readonly Dictionary<Type, ConditionalWeakTable<object, object?>> _recipientsMap = new();

	public bool IsRegistered<TMessage>(object recipient)
	{
		ArgumentNullException.ThrowIfNull(recipient);

		return _recipientsMap.TryGetValue(typeof(TMessage), out var mapping)
		       && mapping.TryGetValue(recipient, out _);
	}

	public void Register<TMessage>(object recipient, MessageHandlerDispatcher? dispatcher = null)
		where TMessage : class
	{
		ArgumentNullException.ThrowIfNull(recipient);

		var type = typeof(TMessage);
		if (!_recipientsMap.TryGetValue(type, out ConditionalWeakTable<object, object?>? mapping))
		{
			mapping = new ConditionalWeakTable<object, object?>();
			_recipientsMap.Add(type, mapping);
		}

		if (!mapping.TryAdd(recipient, dispatcher))
		{
			throw new InvalidOperationException(); // TODO exception message duplicate message
		}
	}

	public TMessage Send<TMessage>(TMessage message)
		where TMessage : class
	{
		var type = typeof(TMessage);
		if (!_recipientsMap.TryGetValue(type, out var mapping))
		{
			return message;
		}

		// The CommunityToolkit devs use some low level memory marshaling to store pointers in a structured queue-like buffer
		// This is the monkey implementation, so we're using a queue.
		// TODO Optimize this? It would be easy. And fun :)
		Queue<(object Key, object? Value)> queue = new();
		foreach (var kvp in mapping)
		{
			queue.Enqueue((kvp.Key, kvp.Value));
		}

		SendAll(queue, message);

		return message;
	}

	private void SendAll<TMessage>(Queue<(object, object?)> queue, TMessage message)
		where TMessage : class
	{
		while (queue.TryDequeue(out var current))
		{
			var (recipient, handler) = current;
			if (handler is null)
			{
				Unsafe.As<IRecipient<TMessage>>(recipient).Receive(message);
			}
			else
			{
				Unsafe.As<MessageHandlerDispatcher>(handler).Invoke(recipient, message);
			}
		}
	}

	public void Unregister<TMessage>(object recipient)
		where TMessage : class
	{
		ArgumentNullException.ThrowIfNull(recipient);

		var type = typeof(TMessage);

		if (_recipientsMap.TryGetValue(type, out var mapping))
		{
			mapping.Remove(recipient);
		}
	}

	public void UnregisterAll(object recipient)
	{
		// The developers for CommunityToolkit did not use a foreach loop,
		// and instead broke into the enumerator to iterate over the collection manually
		// They know what they're doing so I'll look into this further
		foreach (var current in _recipientsMap)
		{
			current.Value.Remove(recipient);
		}
	}
}