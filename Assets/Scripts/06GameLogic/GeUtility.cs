using UnityEngine;

namespace Battle
{
    public static class GeUtility
    {
        public static void AttachTo(GameObject go, GameObject parent, bool keepPos = false)
        {
			#if !LOGIC_SERVER			
            if(null == go || null == parent)
            {
                return ;
            }
            
            Transform goTransform    = go.transform;

            Vector3    lscale        = goTransform.transform.localScale;
            Vector3    lpos          = goTransform.transform.localPosition;
            Quaternion lrotation     = goTransform.transform.localRotation;

            go.transform.SetParent(parent.transform, true);

            goTransform.localScale    = lscale;
            goTransform.localRotation = lrotation;
            goTransform.localPosition = lpos;
			#endif
        }

        public static bool CheckArrayItems<T>(T[] array) where T : Object
        {
            if(array == null)
            {
                return true;
            }

            for(int i = 0; i < array.Length; ++i)
            {
                if( array[i] == null )
                {
                    return false;
                }
            }

            return true;
        }
    }
}
