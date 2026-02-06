using System;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting;

public class DynamicContextProperty<T>(string propertyName) {
    private bool _HasBeenSet;
    private T Value { get; set; }

    public T Get() {
        return !_HasBeenSet
            ? throw new Exception($"Property {propertyName} has not been set")
            : Value;
    }

    public void Set(T value) {
        _HasBeenSet = true;
        Value = value;
    }
}
