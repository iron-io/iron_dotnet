﻿namespace IronSharp.Core
{
    public static class ExtensionsForIInspectable
    {
        public static string Inspect(this IInspectable value)
        {
            return JSON.Generate(value);
        }
    }
}