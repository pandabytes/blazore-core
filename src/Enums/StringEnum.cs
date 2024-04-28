using System.Reflection;

namespace Blazor.Core.Enums;

/// <summary>
/// Represent a string enum.
/// </summary>
public abstract class StringEnum
{
  /// <summary>
  /// Key is a type that inherits <see cref="StringEnum"/>.
  /// Inner key is the string value and its value is the corresponding 
  /// <see cref="StringEnum"/> object.
  /// </summary>
  private static readonly IDictionary<Type, IDictionary<string, StringEnum>> StringEnumValuesMapping =
    new Dictionary<Type, IDictionary<string, StringEnum>>();

  /// <summary>
  /// The enum value.
  /// </summary>
  public string Value { get; }

  /// <summary>
  /// Constructor.
  /// </summary>
  /// <param name="value">Enum value.</param>
  protected StringEnum(string value)
  {
    Value = value;
  }

  /// <inheritdoc/>
  public override string ToString() => Value;

  /// <inheritdoc/>
  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(this, obj))
    {
      return true;
    }

    if (obj is null || obj.GetType() != GetType())
    {
      return false;
    }

    return ((StringEnum)obj).Value == Value;
  }

  /// <inheritdoc/>
  public override int GetHashCode() => Value.GetHashCode();

  /// <summary>
  /// Determines whether the <paramref name="value"/> is valid
  /// string enum value of <typeparamref name="TStringEnum"/>.
  /// </summary>
  /// <typeparam name="TStringEnum">
  /// Type that inherits <see cref="StringEnum"/>.
  /// </typeparam>
  /// <param name="value">String value to check for.</param>
  /// <returns>
  /// True if <paramref name="value"/> is a string enum value
  /// of <typeparamref name="TStringEnum"/>. False otherwise.
  /// </returns>
  public static bool Contains<TStringEnum>(string value) where TStringEnum : StringEnum
  {
    var type = typeof(TStringEnum);
    CacheStringEnum(type);
    return StringEnumValuesMapping[type].ContainsKey(value);
  }

  /// <summary>
  /// Get the corresponding <typeparamref name="TStringEnum"/> given
  /// <paramref name="value"/>.
  /// </summary>
  /// <typeparam name="TStringEnum">
  /// Type that inherits <see cref="StringEnum"/>.
  /// </typeparam>
  /// <param name="value">String value.</param>
  /// <returns>The corresponding <typeparamref name="TStringEnum"/> object.</returns>
  /// <exception cref="ArgumentException">
  /// Thrown when <paramref name="value"/> is not a string enum value
  /// of <typeparamref name="TStringEnum"/>.
  /// </exception>
  public static TStringEnum Get<TStringEnum>(string value) where TStringEnum : StringEnum
  {
    var type = typeof(TStringEnum);
    if (Contains<TStringEnum>(value))
    {
      return (TStringEnum)StringEnumValuesMapping[type][value];
    }

    throw new ArgumentException($"Value \"{value}\" not found.");
  }

  /// <summary>
  /// Implicitly convert <paramref name="stringEnum"/> to string.
  /// </summary>
  /// <param name="stringEnum">String enum object.</param>
  public static implicit operator string(StringEnum stringEnum) => stringEnum.Value;

  /// <summary>
  /// Compare 2 string enums.
  /// </summary>
  /// <param name="stringEnum1"></param>
  /// <param name="stringEnum2"></param>
  /// <returns>True if equal, false otherwise.</returns>
  public static bool operator ==(StringEnum stringEnum1, StringEnum stringEnum2)
    => stringEnum1.Equals(stringEnum2);

  /// <summary>
  /// The invert of "==".
  /// </summary>
  /// <param name="stringEnum1"></param>
  /// <param name="stringEnum2"></param>
  /// <returns>True if not equal, false otherwise.</returns>
  public static bool operator !=(StringEnum stringEnum1, StringEnum stringEnum2)
    => !(stringEnum1.Value == stringEnum2.Value);

  private static void CacheStringEnum(Type type)
  {
    if (StringEnumValuesMapping.ContainsKey(type))
    {
      return;
    }

    try
    {
      var stringEnumType = typeof(StringEnum);
      var values = type
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(field => field.IsInitOnly)
        .Where(field => field.FieldType == type)
        .Where(field => stringEnumType.IsAssignableFrom(field.FieldType))
        .Select(field => (StringEnum)field.GetValue(null)!)
        .ToDictionary(stringEnum => stringEnum.Value, stringEnum => stringEnum);

      if (values.Any())
      {
        StringEnumValuesMapping.Add(type, values);
      }
    }
    catch (ArgumentException)
    {
      throw new InvalidOperationException(
        $"String enum \"{type.FullName}\" has duplicate string values."
      );
    }
  }
}
