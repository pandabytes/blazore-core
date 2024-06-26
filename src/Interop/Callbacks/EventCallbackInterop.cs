using Microsoft.AspNetCore.Components;

namespace Blazor.Core.Interop.Callbacks;

/// <summary>
/// This class represents an EventCallback callback for JS interop.
/// Note when using this class, we copy 
/// <see cref="EventCallback"/> because
/// <see cref="EventCallback"/> is a struct.
/// </summary>
public sealed class EventCallbackInterop : BaseCallbackInterop
{
  private class JSInteropWrapper
  {
    private readonly EventCallback _callback;

    public JSInteropWrapper(EventCallback callback) => _callback = callback;

    // Keep name as Invoke (instead of usually InvokeAsync)
    // so that JS only needs to know to call "Invoke"
    [JSInvokable]
    public async Task Invoke() => await _callback.InvokeAsync();
  }

  /// <inheritdoc />
  public override bool IsAsync => true;

  /// <summary>
  /// Construct with the given <paramref name="callback"/>.
  /// </summary>
  public EventCallbackInterop(EventCallback callback)
    => DotNetRef = DotNetObjectReference.Create(new JSInteropWrapper(callback));

  /// <summary>
  /// Easy way to convert to a callback interop object.
  /// Just make sure to dispose it afterwards.
  /// </summary>
  /// <param name="callback">Callback.</param>
  public static implicit operator EventCallbackInterop(EventCallback callback) => new(callback);
}

/// <summary>
/// This class represents an EventCallback callback for JS interop.
/// Note when using this class, we copy 
/// <see cref="EventCallback"/> because
/// <see cref="EventCallback"/> is a struct.
/// </summary>
/// <typeparam name="T">Parameter type of the callback.</typeparam>
public sealed class EventCallbackInterop<T> : BaseCallbackInterop
{
  private class JSInteropWrapper
  {
    private readonly EventCallback<T> _callback;

    public JSInteropWrapper(EventCallback<T> callback) => _callback = callback;

    // Keep name as Invoke (instead of usually InvokeAsync)
    // so that JS only needs to know to call "Invoke"
    [JSInvokable]
    public async Task Invoke(T arg) => await _callback.InvokeAsync(arg);
  }

  /// <inheritdoc />
  public override bool IsAsync => true;

  /// <summary>
  /// Construct with the given <paramref name="callback"/>.
  /// </summary>
  public EventCallbackInterop(EventCallback<T> callback)
    => DotNetRef = DotNetObjectReference.Create(new JSInteropWrapper(callback));

  /// <summary>
  /// Easy way to convert to a callback interop object.
  /// Just make sure to dispose it afterwards.
  /// </summary>
  /// <param name="callback">Callback.</param>
  public static implicit operator EventCallbackInterop<T>(EventCallback<T> callback) => new(callback);
}
