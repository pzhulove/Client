using System;
using System.Collections;
using System.Text;

namespace UnityEngine
{
    public static class ExtensionMethodsUnity
    {
        /*
                public static bool isFlagON(this LAYER_OPTION a, LAYER_OPTION b)
                {
                    return (a & b) == b;
                }*/
        public static bool FEQUAL(this float a, float b)
        {
            return a >= b - Mathf.Epsilon && a <= b + Mathf.Epsilon;
        }
        public static bool FUNEQUAL(this float a, float b)
        {
            return a < b - Mathf.Epsilon || a > b + Mathf.Epsilon;
        }
        public static bool ContainBounds(this Bounds bounds, Bounds target)
        {
            return bounds.Contains(target.min) && bounds.Contains(target.max);
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component != null)
                return component;

            return gameObject.AddComponent<T>();
        }

        public static T CopyFrom<T>(this T self, T src) where T : Component
        {
            System.Type type = self.GetType();

            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(self, field.GetValue(src));
            }
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanRead || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(self, prop.GetValue(src, null), null);
            }
            return self;
        }


        public static bool IsNull(this UnityEngine.Object self)
        {
            return self == null;
        }

        public static bool IsNotNull(this UnityEngine.Object self)
        {
            return self != null;
        }

        /// <summary>
        /// ��ȡhashtable��ֵ
        /// </summary>
        public static string GetValue(this Hashtable hashtable, string key)
        {
            if (hashtable.ContainsKey(key) && hashtable[key] != null)
            {
                return hashtable[key].ToString();
            }
            else
            {
                global::Logger.LogErrorFormat("HashTable Not Contain Key:{0}", key);
            }
            return "";
        }

        /// <summary>
        /// ת��String ��Uint
        /// </summary>
        public static uint ConvertStrToUInt(this String str, uint defaultValue = 0)
        {
            try
            {
                uint value = uint.Parse(str);
                return value;
            }
            catch (System.Exception ex)
            {
                global::Logger.LogErrorFormat("Convert Str To uint Failed:{0}", str);
            }
            return defaultValue;
        }


        /// <summary>
        /// ת��String ��Int
        /// </summary>
        public static int ConvertStrToInt(this String str, int defaultValue = 0)
        {
            try
            {
                int value = int.Parse(str);
                return value;
            }
            catch (System.Exception ex)
            {
                global::Logger.LogErrorFormat("Convert Str To int Failed:{0}", str);
            }
            return defaultValue;
        }

        /// <summary>
        /// ת��String ��Ushort
        /// </summary>
        public static ushort ConvertStrToUShort(this String str, ushort defaultValue = 0)
        {
            try
            {
                ushort value = ushort.Parse(str);
                return value;
            }
            catch (System.Exception ex)
            {
                global::Logger.LogErrorFormat("Convert Str To ushort Failed:{0}", str);
            }
            return defaultValue;
        }

        #region Transform

        /// <summary>
        /// ���þ���λ�õ� x ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="newValue">x ����ֵ��</param>
        public static void SetPositionX(this Transform transform, float newValue)
        {
            Vector3 v = transform.position;
            v.x = newValue;
            transform.position = v;
        }

        /// <summary>
        /// ���þ���λ�õ� y ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="newValue">y ����ֵ��</param>
        public static void SetPositionY(this Transform transform, float newValue)
        {
            Vector3 v = transform.position;
            v.y = newValue;
            transform.position = v;
        }

        /// <summary>
        /// ���þ���λ�õ� z ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="newValue">z ����ֵ��</param>
        public static void SetPositionZ(this Transform transform, float newValue)
        {
            Vector3 v = transform.position;
            v.z = newValue;
            transform.position = v;
        }

        /// <summary>
        /// ���Ӿ���λ�õ� x ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="deltaValue">x ����ֵ������</param>
        public static void AddPositionX(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.position;
            v.x += deltaValue;
            transform.position = v;
        }

        /// <summary>
        /// ���Ӿ���λ�õ� y ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="deltaValue">y ����ֵ������</param>
        public static void AddPositionY(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.position;
            v.y += deltaValue;
            transform.position = v;
        }

        /// <summary>
        /// ���Ӿ���λ�õ� z ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="deltaValue">z ����ֵ������</param>
        public static void AddPositionZ(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.position;
            v.z += deltaValue;
            transform.position = v;
        }

        /// <summary>
        /// �������λ�õ� x ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="newValue">x ����ֵ��</param>
        public static void SetLocalPositionX(this Transform transform, float newValue)
        {
            Vector3 v = transform.localPosition;
            v.x = newValue;
            transform.localPosition = v;
        }

        /// <summary>
        /// �������λ�õ� y ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="newValue">y ����ֵ��</param>
        public static void SetLocalPositionY(this Transform transform, float newValue)
        {
            Vector3 v = transform.localPosition;
            v.y = newValue;
            transform.localPosition = v;
        }
        /// <summary>
        /// �������λ�õ�anchorPos ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="newValue">y ����ֵ��</param>
        public static void SetLocalAnchoredPositionY(this RectTransform transform, float newValue)
        {
            Vector2 v = transform.anchoredPosition;
            v.y = newValue;
            transform.anchoredPosition = v;
        }
        /// <summary>
        /// �������λ�õ�anchorPos ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="newValue">x ����ֵ��</param>
        public static void SetLocalAnchoredPositionX(this RectTransform transform, float newValue)
        {
            Vector2 v = transform.anchoredPosition;
            v.x = newValue;
            transform.anchoredPosition = v;
        }
        /// <summary>
        /// �������λ�õ� z ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="newValue">z ����ֵ��</param>
        public static void SetLocalPositionZ(this Transform transform, float newValue)
        {
            Vector3 v = transform.localPosition;
            v.z = newValue;
            transform.localPosition = v;
        }

        /// <summary>
        /// �������λ�õ� x ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="deltaValue">x ����ֵ��</param>
        public static void AddLocalPositionX(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.localPosition;
            v.x += deltaValue;
            transform.localPosition = v;
        }

        /// <summary>
        /// �������λ�õ� y ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="deltaValue">y ����ֵ��</param>
        public static void AddLocalPositionY(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.localPosition;
            v.y += deltaValue;
            transform.localPosition = v;
        }

        /// <summary>
        /// �������λ�õ� z ���ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="deltaValue">z ����ֵ��</param>
        public static void AddLocalPositionZ(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.localPosition;
            v.z += deltaValue;
            transform.localPosition = v;
        }

        /// <summary>
        /// ������Գߴ�� x ������
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="newValue">x ����ֵ��</param>
        public static void SetLocalScaleX(this Transform transform, float newValue)
        {
            Vector3 v = transform.localScale;
            v.x = newValue;
            transform.localScale = v;
        }

        /// <summary>
        /// ������Գߴ�� y ������
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="newValue">y ����ֵ��</param>
        public static void SetLocalScaleY(this Transform transform, float newValue)
        {
            Vector3 v = transform.localScale;
            v.y = newValue;
            transform.localScale = v;
        }

        /// <summary>
        /// ������Գߴ�� z ������
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="newValue">z ����ֵ��</param>
        public static void SetLocalScaleZ(this Transform transform, float newValue)
        {
            Vector3 v = transform.localScale;
            v.z = newValue;
            transform.localScale = v;
        }

        /// <summary>
        /// ������Գߴ�� x ������
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="deltaValue">x ����������</param>
        public static void AddLocalScaleX(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.localScale;
            v.x += deltaValue;
            transform.localScale = v;
        }

        /// <summary>
        /// ������Գߴ�� y ������
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="deltaValue">y ����������</param>
        public static void AddLocalScaleY(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.localScale;
            v.y += deltaValue;
            transform.localScale = v;
        }

        /// <summary>
        /// ������Գߴ�� z ������
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="deltaValue">z ����������</param>
        public static void AddLocalScaleZ(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.localScale;
            v.z += deltaValue;
            transform.localScale = v;
        }

        /// <summary>
        /// ��ά�ռ���ʹ <see cref="UnityEngine.Transform" /> ָ��ָ��Ŀ�����㷨��ʹ���������ꡣ
        /// </summary>
        /// <param name="transform"><see cref="UnityEngine.Transform" /> ����</param>
        /// <param name="lookAtPoint2D">Ҫ����Ķ�ά����㡣</param>
        /// <remarks>�ٶ��� forward ����Ϊ <see cref="UnityEngine.Vector3.up" />��</remarks>
        public static void LookAt2D(this Transform transform, Vector2 lookAtPoint2D)
        {
            Vector3 vector = lookAtPoint2D.ToVector3() - transform.position;
            vector.y = 0f;

            if (vector.magnitude > 0f)
            {
                transform.rotation = Quaternion.LookRotation(vector.normalized, Vector3.up);
            }
        }

        #endregion Transform

        /// <summary>
        /// ȡ <see cref="UnityEngine.Vector3" /> �� (x, y, z) ת��Ϊ <see cref="UnityEngine.Vector2" /> �� (x, z)��
        /// </summary>
        /// <param name="vector3">Ҫת���� Vector3��</param>
        /// <returns>ת����� Vector2��</returns>
        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }

        /// <summary>
        /// ȡ <see cref="UnityEngine.Vector2" /> �� (x, y) ת��Ϊ <see cref="UnityEngine.Vector3" /> �� (x, 0, y)��
        /// </summary>
        /// <param name="vector2">Ҫת���� Vector2��</param>
        /// <returns>ת����� Vector3��</returns>
        public static Vector3 ToVector3(this Vector2 vector2)
        {
            return new Vector3(vector2.x, 0f, vector2.y);
        }

        /// <summary>
        /// ȡ <see cref="UnityEngine.Vector2" /> �� (x, y) �͸������� y ת��Ϊ <see cref="UnityEngine.Vector3" /> �� (x, ���� y, y)��
        /// </summary>
        /// <param name="vector2">Ҫת���� Vector2��</param>
        /// <param name="y">Vector3 �� y ֵ��</param>
        /// <returns>ת����� Vector3��</returns>
        public static Vector3 ToVector3(this Vector2 vector2, float y)
        {
            return new Vector3(vector2.x, y, vector2.y);
        }
    }
}

public static class ExtensionMethods
{
    public static void Clear(this StringBuilder stringBuilder)
    {
        stringBuilder.Length = 0;
    }
}
