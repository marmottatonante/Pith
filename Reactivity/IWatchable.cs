namespace Keystone.Reactivity;

/// <summary>
/// Represents an object capable of being watched for changes.
/// </summary>
public interface IWatchable
{
    /// <summary>
    /// Event that fires right before the object state changes.
    /// </summary>
    event Action? Changing;
    /// <summary>
    /// Event that fires right after the object state changes.
    /// </summary>
    event Action? Changed;
}