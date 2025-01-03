using UnityEngine;
using Tenmove.Runtime;

namespace Tenmove.Runtime.Unity
{
    public static class Extension
    {
        static public Vector2 ToVector2(this Math.Vec2 vec2)
        {
            return new Vector2(vec2.x, vec2.y);
        }

        static public Vector3 ToVector3(this Math.Vec3 vec3)
        {
            return new Vector3(vec3.x, vec3.y, vec3.z);
        }

        static public Vector4 ToVector4(this Math.Vec4 vec4)
        {
            return new Vector4(vec4.x, vec4.y, vec4.z, vec4.w);
        }

        static public Quaternion ToQuaternion(this Math.Quat quat)
        {
            return new Quaternion(quat.x, quat.y, quat.z,quat.w);
        }

        static public Color ToColor(this Graphic.RGBA rgba)
        {
            return new Color(rgba.r, rgba.g, rgba.b, rgba.a);
        }

        static public Rect ToRect(this Math.Rect rect)
        {
            return new Rect(rect.x, rect.y, rect.Width, rect.Height);
        }

        static public Math.Vec4 ToVec4(this Vector4 from)
        {
            return new Math.Vec4(from.x, from.y, from.z, from.w);
        }

        static public Math.Vec3 ToVec3(this Vector3 from)
        {
            return new Math.Vec3(from.x, from.y, from.z);
        }

        static public Math.Vec2 ToVec2(this Vector2 from)
        {
            return new Math.Vec2(from.x, from.y);
        }

        static public Math.Quat ToQuat(this Quaternion from)
        {
            return new Math.Quat(from.x, from.y, from.z, from.w);
        }

        static public Graphic.RGBA ToRGBA(this Color from)
        {
            return new Graphic.RGBA(from.r, from.g, from.b, from.a);
        }

        static public Math.Rect ToRect(this Rect rect)
        {
            return new Math.Rect(rect.x, rect.y, rect.width, rect.height);
        }
    }
}