using System.Collections;
using RpgFrame.Engine.GameObjects;

namespace RpgFrame.Engine.Internal;

internal class GameObjectCollection : ICollection<GameObject>, IReadOnlyCollection<GameObject>
{
	private readonly Dictionary<Guid, GameObject> _objects = new();

	public int Count => _objects.Count;
	public bool IsReadOnly { get; } = false;

	public void Add(GameObject @object)
	{
		ArgumentNullException.ThrowIfNull(@object);

		if (_objects.ContainsKey(@object.Id))
		{
			throw new InvalidOperationException(); // TODO exception message
		}

		_objects.Add(@object.Id, @object);
	}

	public bool Contains(GameObject? item)
	{
		if (item is not null
		    && _objects.TryGetValue(item.Id, out var found))
		{
			return ReferenceEquals(item, found);
		}

		return false;
	}


	public bool Remove(GameObject item)
	{
		ArgumentNullException.ThrowIfNull(item);

		return _objects.Remove(item.Id);
	}

	public IEnumerator<GameObject> GetEnumerator()
	{
		return _objects.Values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	void ICollection<GameObject>.Clear()
	{
		_objects.Clear();
	}

	void ICollection<GameObject>.CopyTo(GameObject[] array, int arrayIndex)
	{
		_objects.Values.CopyTo(array, arrayIndex);
	}
}