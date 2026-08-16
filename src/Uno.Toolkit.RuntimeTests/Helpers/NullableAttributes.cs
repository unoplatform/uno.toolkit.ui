using System;

namespace System.Diagnostics.CodeAnalysis;

#if NETSTANDARD2_0
/// <summary>Specifies that null is allowed as an input even if the corresponding type disallows it.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
internal sealed class AllowNullAttribute : Attribute { }
#endif
