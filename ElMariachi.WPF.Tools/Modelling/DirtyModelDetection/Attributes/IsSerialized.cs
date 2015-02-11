using System;

namespace ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.Attributes
{
    /// <summary>
    /// Attribute used to indicate that the property is serialized, meaning that the property value have some impact on serialized data.
    /// </summary>
    public class IsSerialized : Attribute
    {
    }
}