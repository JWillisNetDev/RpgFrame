using RpgFrame.Engine.Internal;
using RpgFrame.Engine.Interfaces;

namespace RpgFrame.Tests;

internal record TestMessage(string Payload);

internal class TestRecipient : TestFakeRecipient, IRecipient<TestMessage>
{
	void IRecipient<TestMessage>.Receive(TestMessage message)
	{
		Payload = message.Payload;
	}
}

internal class TestFakeRecipient
{
	public string Payload { get; set; }
}

public class UnitTest1
{
	[Fact]
	public void Test1()
	{
		const string expected = "Hello, world!!";
		TestMessage message = new TestMessage(expected);
		TestRecipient testRecipient = new();
		Messenger messenger = new();
		messenger.Register<TestMessage>(testRecipient);

		messenger.Send(message);

		Assert.NotNull(testRecipient.Payload);
		Assert.Equal(expected, testRecipient.Payload);
	}

	[Fact]
	public void Test2()
	{
		const string expected = "Hello, world!!";
		TestMessage message = new TestMessage(expected);
		TestFakeRecipient testRecipient = new();
		Messenger messenger = new();
		var handlerDispatcher = new MessageHandlerDispatcher.For<TestRecipient, TestMessage>((r, msg) =>
		{
			var recipient = Assert.IsType<TestFakeRecipient>(r);
			recipient.Payload = msg.Payload;
		});
		messenger.Register<TestMessage>(testRecipient, handlerDispatcher);

		messenger.Send(message);

		Assert.NotNull(testRecipient.Payload);
		Assert.Equal(expected, testRecipient.Payload);
	}
}