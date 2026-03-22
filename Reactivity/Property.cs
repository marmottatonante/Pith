namespace Keystone.Reactivity;

public interface IReadOnlyProperty<T> : IObservable
{
    T Value { get; }
}

public interface IProperty<T> : IReadOnlyProperty<T>, IBindable<T>
{
    new T Value { get; set; }
}

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
    /// <param name="compute">Transform function to retrieve the value.</param>
    /// <param name="sources">Sources to subscribe to for the update events.</param>
    public Property(Func<T> compute, params IObservable[] sources)
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

    public void Bind(Func<T> compute, params IObservable[] sources)
    {
        Unbind();
        void handler() => Set(compute());
        foreach (var source in sources) source.Changed += handler;
        _unbind = () => { foreach (var source in sources) source.Changed -= handler; };
        handler();
    }

    public void Unbind()
    {
        _unbind?.Invoke();
        _unbind = null;
    }
}