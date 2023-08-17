using System.Runtime.CompilerServices;

namespace RpgFrame.Engine.Internal;

public abstract class MessageHandlerDispatcher
{
	public abstract void Invoke(object recipient, object message);

	public sealed class For<TRecipient, TMessage> : MessageHandlerDispatcher
		where TRecipient : class
		where TMessage : class
	{
		private readonly Messenger.MessageHandler<TRecipient, TMessage> _handler;

		public For(Messenger.MessageHandler<TRecipient, TMessage> handler)
		{
			_handler = handler;
		}

		public override void Invoke(object recipient, object message)
		{
			_handler(Unsafe.As<TRecipient>(recipient), Unsafe.As<TMessage>(message));
		}
	}
}