#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Utils;
using System.Text;
using System.Diagnostics;
using System.IO;
using UnityEditor.VersionControl;


public class PrefabTools
{
    public static void ApplyPrefab(GameObject gameObject)
    {
        GameObject gameObject2 = PrefabUtility.FindValidUploadPrefabInstanceRoot(gameObject);
        bool enabled = (gameObject2 != null && !AnimationMode.InAnimationMode());

        if (!enabled)
        {
            EditorUtility.DisplayDialog("警告", "请等待!", "确定");
            return;
        }


        UnityEngine.Object prefabParent = PrefabUtility.GetPrefabParent(gameObject2);
        string assetPath = AssetDatabase.GetAssetPath(prefabParent);

        System.Type t = typeof(Provider);

        var func = t.GetMethod("PromptAndCheckoutIfNeeded", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        bool flag2 = (bool)func.Invoke(null, new object[] { new string[]
        {
                                assetPath
        }, "The version control requires you to checkout the prefab before applying changes."});


        if (flag2)
        {
            PrefabUtility.ReplacePrefab(gameObject2, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
        }
    }
}


[ExecuteInEditMode]
public class DEntityInfoComponent : MonoBehaviour
{
    private ParticleSystem[]    pss;
    private Animation[] anis;
    private FrameMaterials[] frames;
    private Animator[] animators;
    private float duration;
    protected string _name = "effect";
    protected EffectPlayType _type = EffectPlayType.EffectTime;
    public    DEntityInfo info;
	private  ControlDoorState ctrlDoorState;


    public static void DrawRect(Quaternion rotation, Vector3 position, Vector2 size)
    {
        Vector3 vector = rotation * Vector3.right;
        Vector3 vector2 = rotation * Vector3.forward;
        float num = 0.5f * size.x;
        float num2 = 0.5f * size.y;
        Vector3 vector3 = position + vector * num + vector2 * num2;
        Vector3 vector4 = position - vector * num + vector2 * num2;
        Vector3 vector5 = position - vector * num - vector2 * num2;
        Vector3 vector6 = position + vector * num - vector2 * num2;
        Handles.DrawLine(vector3, vector4);
        Handles.DrawLine(vector4, vector5);
        Handles.DrawLine(vector5, vector6);
        Handles.DrawLine(vector6, vector3);
    }

    internal static void DrawRadius2DHandle(Vector3 position, float radius)
    {
        Vector3 b = Vector3.up;
        Handles.DrawWireDisc(position, b, radius);
    }

    void RemoveEmptyAnimator()
    {
        List<Animator> emptyAnimators = new List<Animator>();
        List<Animator> anim = new List<Animator>();

        for (int i = 0; i < animators.Length; ++i)
        {
            if (animators[i].GetCurrentAnimatorClipInfo(0).Length <= 0)
            {
                emptyAnimators.Add(animators[i]);
            }

            anim.Add(animators[i]);
        }

        for (int i = 0; i < emptyAnimators.Count; ++i)
        {
            anim.Remove(emptyAnimators[i]);
        }
        animators = anim.ToArray();
    }
    void Init()
    {
        if (gameObject != null)
        {
           
            pss = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            anis = gameObject.GetComponentsInChildren<Animation>(true);
            frames = gameObject.GetComponentsInChildren<FrameMaterials>(true);
            animators = gameObject.GetComponentsInChildren<Animator>(true);
			ctrlDoorState = gameObject.GetComponentInChildren<ControlDoorState> (true);
            //ctrlDoorState = gameObject.GetComponent<ControlDoorState> ();
            RemoveEmptyAnimator();
        }
        if (pss == null) pss = new ParticleSystem[0];
        if (anis == null) anis = new Animation[0];
        if (frames == null) frames = new FrameMaterials[0];

        duration = SampleDuration();
        
    }

	Vector3 GetDoorTransCenter()
	{
#if UNITY_EDITOR && !SERVER_LOGIC
        DTransportDoor doorinfo = info as DTransportDoor;
        if (doorinfo != null)
        {
            return doorinfo.GetDoorTransCenter();
        }
        else
        {
            DTownDoor townInfo = info as DTownDoor;
            if (townInfo != null)
                return townInfo.GetLocalBirthPos();
            else
                return Vector3.zero;
        }
#else
        return info.GetPosition();
#endif
    }

    void Awake()
    {
        Init();
        Sampler(0);
    }

    void OnDrawGizmos()
    {
        if(info == null)
        {
            return;
        }
        
#if UNITY_EDITOR && !SERVER_LOGIC
        var doorinfo = info as DTransportDoor;
        if(doorinfo != null)
        {
            doorinfo.UpdateData();
        }
#endif

        if(info.type == DEntityType.REGION || info.type == DEntityType.TRANSPORTDOOR || info.type == DEntityType.TOWNDOOR)
        {
            DRegionInfo rinfo = info as DRegionInfo;

            if(rinfo != null)
            {
                Matrix4x4 oldMat = Handles.matrix;
				Handles.matrix = Matrix4x4.identity;//gameObject.transform.parent.localToWorldMatrix;
                
				if (rinfo.regiontype == DRegionInfo.RegionType.Circle)
                {
                    Handles.color = Color.cyan;
					DrawRadius2DHandle(GetDoorTransCenter(), rinfo.radius);
                }
                else
                {
                    Handles.color = Color.cyan;
					DrawRect(Quaternion.identity, GetDoorTransCenter(), rinfo.rect);
                }
                
				Handles.matrix = gameObject.transform.parent.localToWorldMatrix;
                DTransportDoor dDoor = rinfo as DTransportDoor;
                Gizmos.color = Color.yellow;
				if (dDoor != null)
                	Gizmos.DrawCube(dDoor.birthposition,Vector3.one * 0.3f); 
                Handles.matrix = oldMat;
            }
        }
        else if (info.type == DEntityType.NPC)
        {
            // TODO 材质不是透明的
            DNPCInfo npcInfo = info as DNPCInfo;
            if (npcInfo != null)
            {
                Matrix4x4 oldMat = Handles.matrix;

                //                 Matrix4x4 mat = gameObject.transform.parent.localToWorldMatrix;
                //                 Handles.matrix = Matrix4x4.TRS(
                //                     mat.GetTranslation() + new Vector3(npcInfo.minFindRange.x*(-0.5f), 0.0f, npcInfo.minFindRange.y*(-0.5f)),
                //                     mat.GetRotation() * Quaternion.Euler(90.0f, 0.0f, 0.0f),
                //                     mat.GetScale()
                //                     );
                //                  Rect minRect = new Rect(new Vector2(npcInfo.position.x, npcInfo.position.z), npcInfo.minFindRange);
                //                  Handles.DrawSolidRectangleWithOutline(minRect, Color.red, Color.green);
                // 
                //                 Handles.matrix = gameObject.transform.parent.localToWorldMatrix;
                //                 Handles.color = Color.green;
                //                 DrawRect(Quaternion.identity, npcInfo.position, npcInfo.maxFindRange);

                Handles.matrix = gameObject.transform.parent.localToWorldMatrix;

                Handles.color = Color.black;
                DrawRect(Quaternion.identity, npcInfo.position, npcInfo.minFindRange);

                Handles.color = Color.green;
                DrawRect(Quaternion.identity, npcInfo.position, npcInfo.maxFindRange);

                Handles.matrix = oldMat;
            }
        }
		else if (info.type == DEntityType.ACTIVITY_BOSS_POS
				|| info.type == DEntityType.ACTIVITY_ELITE_POS
				|| info.type == DEntityType.ACTIVITY_MONSTER_POS)
		{
			if (!Application.isPlaying)
			{
				// Display the explosion radius when selected
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(gameObject.transform.position, 0.4f);
				Gizmos.DrawIcon(gameObject.transform.position + new Vector3(-0.5f, 0.5f, 0), "activity_flag.png", true);
				Gizmos.color = Color.white;
			}
		}
    }
    /*
    void Update()
    {
        double currentTime = EditorApplication.timeSinceStartup;
        Sampler((float)currentTime);
    }
    */


    public float SampleDuration()
    {
        float d = 0;

        for (int i = 0; i < pss.Length; ++i)
        {
            float tmp = pss[i].duration + pss[i].startDelay + pss[i].startLifetime;
            if (tmp > d)
                d = tmp;
        }

        for (int i = 0; i < anis.Length; ++i)
        {
            if (anis[i].clip != null && d < anis[i].clip.length)
            {
                d = anis[i].clip.length;
            }
        }

        for (int i = 0; i < frames.Length; ++i)
        {
            if (frames[i] != null && d < frames[i].Duration())
            {
                d = frames[i].Duration();
            }
        }

        if (animators != null)
        {
            for (int i = 0; i < animators.Length; ++i)
            {
                AnimatorClipInfo[] clipinfo = animators[i].GetCurrentAnimatorClipInfo(0);

                for (int j = 0; j < clipinfo.Length; ++j)
                {
                    if (clipinfo[j].clip != null && d < clipinfo[j].clip.length)
                    {
                        d = clipinfo[j].clip.length;
                    }
                }
            }
        }
        return d;
    }

    public void Sampler(float fTime)
    { 
        if (pss == null
            && anis == null && frames == null)
        {
            return;
        }

        for (int i = 0; i < pss.Length; ++i)
        {
            if (pss[i] != null)
                pss[i].Simulate(fTime);
        }

        for (int i = 0; i < anis.Length; ++i)
        {
            if (anis[i] != null && anis[i].clip)
                anis[i].clip.SampleAnimation(anis[i].gameObject, fTime);
        }

        for (int i = 0; i < frames.Length; ++i)
        {
            if (frames[i])
            {
                frames[i].Sampler(fTime);
            }
        }

        if (animators != null)
        {
            for (int i = 0; i < animators.Length; ++i)
            {
                if (animators[i])
                    animators[i].Update(fTime);
            }
        }

    }
}
#endif