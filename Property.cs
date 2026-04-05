namespace Keystone;

public interface IReadOnlyProperty<T> : IWatchable
{
    T Value { get; }
}

public interface IProperty<T> : IReadOnlyProperty<T>, IBindable<T>
{
    new T Value { get; set; }
}

/// <summary>
/// Encapsulates a value and fires events on change.
/// </summary>
/// <typeparam name="T">Type of the encapsulated value.</typeparam>
public sealed class Property<T> : IProperty<T>
{
    private T _value;
    private Action? _unbind;

    public event Action? Changing;
    public event Action? Changed;

    public T Value
    {
        get => _value;
        set
        {
            if (_unbind is not null)
                throw new InvalidOperationException("Property is bound.");
            Set(value);
        }
    }

    /// <summary>
    /// Constructs a Property with an initial value.
    /// </summary>
    /// <param name="initial">Initial value.</param>
    public Property(T initial) => _value = initial;
    /// <summary>
    /// Constructs a Property that's already bound.
    /// </summary>
    /// <param name="compute">Function that computes the value.</param>
    /// <param name="sources">Sources for the update events.</param>
    public Property(Func<T> compute, params IWatchable[] sources)
    {
        // _value is set by Bind, default! is used to silence the compiler.
        _value = default!;
        Bind(compute, sources);
    }

    private void Set(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_value, value)) return;
        Changing?.Invoke();
        _value = value;
        Changed?.Invoke();
    }

    /// <summary>
    /// Bind this Property so that it updates automatically.
    /// </summary>
    /// <param name="compute">Function that computes the value.</param>
    /// <param name="sources">Sources for the update events.</param>
    public void Bind(Func<T> compute, params IWatchable[] sources)
    {
        Unbind();
        void handler() => Set(compute());
        foreach (var source in sources) source.Changed += handler;
        _unbind = () => { foreach (var source in sources) source.Changed -= handler; };
        handler();
    }

    /// <summary>
    /// Unbind this Property from the current binding.
    /// </summary>
    public void Unbind()
    {
        _unbind?.Invoke();
        _unbind = null;
    }
}