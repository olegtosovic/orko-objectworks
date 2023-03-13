using Microsoft.CodeAnalysis;
using Orko.ObjectWorks.Abstractions;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Orko.ObjectWorks.Core;

/// <summary>
/// Represents object metadata context.
/// </summary>
/// <typeparam name="TObject"></typeparam>
public sealed class ObjectContext<TObject> : IObjectContext<TObject> where TObject : class
{
    /// <summary>
    /// Single instance per object type.
    /// </summary>
    private static readonly ObjectContext<TObject> _instance = new();

    /// <summary>
    /// Dictionary holding all object properties.
    /// </summary>
    private readonly Dictionary<string, IPropertyContext<TObject>> _propertyDictionary = new();

    /// <summary>
    /// Read only collection holding all properties.
    /// </summary>
    private ReadOnlyCollection<IPropertyContext<TObject>> _propertyCollection = null!;

    #region Constructors
    /// <summary>
    /// Only called inside.
    /// </summary>
    private ObjectContext()
    {
        CacheObjectProperties();
    }
    #endregion

    #region Get property
    /// <summary>
    /// Gets cached object property.
    /// </summary>
    public bool TryGetProperty(string propertyName, out IPropertyContext<TObject>? propertyContext)
    {
        return _propertyDictionary.TryGetValue(propertyName, out propertyContext);
    }

    /// <summary>
    /// Gets cached object property.
    /// </summary>
    public IPropertyContext<TObject> GetProperty(string propertyName)
    {
        if (_propertyDictionary.TryGetValue(propertyName, out IPropertyContext<TObject>? propertyContext))
            return propertyContext;
        throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName);
    }

    /// <summary>
    /// Gets cached object properties.
    /// </summary>
    public IReadOnlyCollection<IPropertyContext<TObject>> GetProperties()
    {
        return _propertyCollection;
    }
    #endregion

    #region Get instance
    /// <summary>
    /// Gets object context instance.
    /// </summary>
    public static IObjectContext<TObject> GetInstance()
    {
        return _instance;
    }
    #endregion

    #region Indexers
    #endregion

    #region Cache methods
    /// <summary>
    /// Creates and caches all object properties.
    /// </summary>
    private void CacheObjectProperties()
    {
        // Cache properties to dictionary.
        typeof(TObject)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(pinfo => new PropertyContext<TObject>(pinfo)).ToList()
            .ForEach(pctx => _propertyDictionary.Add(pctx.Name, pctx));

        // Cache properties as collection.
        _propertyCollection = new ReadOnlyCollection<IPropertyContext<TObject>>(
            _propertyDictionary
                .Select(x => x.Value)
                .ToList());
    }
    #endregion
}
