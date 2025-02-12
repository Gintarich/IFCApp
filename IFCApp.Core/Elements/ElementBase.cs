using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IFCApp.Core.Elements
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(Wall), typeDiscriminator: "Wall")]
    [JsonDerivedType(typeof(SandwichPanel), typeDiscriminator: "SandwichPanel")]
    [JsonDerivedType(typeof(WallPanel), typeDiscriminator: "WallPanel")]
    [JsonDerivedType(typeof(Door), typeDiscriminator: "Door")]
    [JsonDerivedType(typeof(Window), typeDiscriminator: "Window")]
    [JsonDerivedType(typeof(Opening), typeDiscriminator: "Opening")]
    [JsonDerivedType(typeof(Beam), typeDiscriminator: "Beam")]
    public class ElementBase
    {
        public Dictionary<string,string> UserData { get; set; }
        public ElementBase() { }
    }
}
