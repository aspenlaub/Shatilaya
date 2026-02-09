using System;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya;

public class DynamicContextProperty<T>(string propertyName) {
    public bool HasBeenSet { get; private set; }
    private T Value { get; set; }

    public T Get() {
        return !HasBeenSet
            ? throw new Exception($"Property {propertyName} has not been set")
            : Value;
    }

    public void Set(T value) {
        HasBeenSet = true;
        Value = value;
    }
}
