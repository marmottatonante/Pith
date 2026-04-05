namespace Keystone.Reactivity;

/// <summary>
/// Represents an object capable of being bound.
/// </summary>
/// <typeparam name="T">Type of the binded value.</typeparam>
public interface IBindable<T>
{
    void Bind(Func<T> compute, params IWatchable[] sources);
    void Unbind();
}