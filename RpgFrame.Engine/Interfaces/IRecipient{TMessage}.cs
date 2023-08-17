namespace RpgFrame.Engine.Interfaces;

public interface IRecipient<in TMessage>
{
	void Receive(TMessage message);
}