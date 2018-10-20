using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Clement.Utilities.Vectors
{
    public static class Vectors
    {
        public static Vector2 Tov2(this float f)
        {
            return new Vector2(f, f);
        }

        public static Vector2 ToXY(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        public static Vector2 ToNegative(this Vector2 v2)
        {
            return new Vector2(-v2.x, -v2.y);
        }

        public static Vector2 ToNegativeX(this Vector2 v2)
        {
            return new Vector2(-v2.x, v2.y);
        }

        public static Vector2 ToNegativeY(this Vector2 v2)
        {
            return new Vector2(v2.x, -v2.y);
        }



    }
}