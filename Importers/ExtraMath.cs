using System;
using Godot;

namespace FullBroadside
{
    public static class Vector3Ext
    {
        public static Vector3 ProjectOnPlane(this Vector3 v, Vector3 n, bool assumeNormalized = true)
        {
            if (!assumeNormalized && !n.IsNormalized())
            {
                n = n.Normalized();
            }
            return v - (n * n.Dot(v));
        }
    }
}
