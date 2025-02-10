namespace Virtuality;
public struct DisposeAction : IDisposable
{
	public Action Action;

	public DisposeAction( Action action )
	{
		Action = action;
	}

	public void Dispose()
	{
		Action?.Invoke();
		Action = null;
	}

	public static IDisposable Create( Action action )
	{
		return new DisposeAction( action );
	}
}
