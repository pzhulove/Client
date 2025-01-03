using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ComCreateRoleScene : MonoBehaviour
{
    GameObject LightNode;
    GameObject CameraNode;
    static Camera sceneRoleCamera = null;
    static Camera curRoleCamera = null;
    static Animation curCamAnim = null;

    static readonly string sceneName = "Xuanjue3";

    static GameObject SceneLightRoot = null;
    static GameObject SceneRoot = null;
    static GameObject ActorRoot;
    static GameObject ActorLightRoot;
    static readonly int MAX_ACTOR_NUM = 3;
    static GameObject[] ActorSlot = new GameObject[MAX_ACTOR_NUM];
    static bool CreatePhase = false;

    protected class RoleDemoDesc
    {
        public GeDemoActor m_RoleActor = null;
        public GeAttach m_LeftWeapon = null;
        public GeAttach m_RightWeapon = null;
        public string m_JobString = null;
        public GameObject m_CameraNode = null;
        public Transform m_LightRefNode = null;
        public float m_InitHeight = 0.0f;
        public int m_PlayIdx = 0;
    }

    protected static List<RoleDemoDesc> m_RoleList = new List<RoleDemoDesc>();
    protected static int m_CurRoleIdx = 0;

    protected static RoleDemoDesc m_DemoActor = null;

    public static uint sCurRole = 0;

    public static bool sRoleSceneLoaded = false;

    public static GeDemoActor sRoleActor = null;
    static GameObject sEffect = null;

    //static protected readonly string[] m_ActionTable = new string[3] { "Anim_Show_Idle", "Anim_Show_Idle_special01", "Anim_Show_Idle_special02" };
    static protected readonly string[] m_ActionTable = new string[3] { "Show_Idle", "Show_Idle_special01", "Show_Idle_special02" };
    static protected readonly int[] m_PlayList = new int[] { 0, 0, 1, 0, 0 , 2, 0, 0 };
    static protected int m_PlayIdx = 0;

    static int jobIDHold = 0;
    static uint[] equipIDsHold = null;
    static int weaponStrengthenLvHold = 0;
    static string curJobString = "";

    static bool onSelectRot = false;
    static float targetAngle = 0;
    static float curAngle = 0;
    static float curAngularSpeed = 0.0f;
    static float curAngularAcc = 1.0f;
    static float curFactor = 1.0f;

    static float curForce = 0;
    static float curTarget = 0;
    static float damp = 0.8f;
    static int lastIdx = -1;
    static bool hasAlign = false;

    static float curRoleForce = 0;
    static bool canDragRot = false;

    static readonly string[] m_animTable = new string[]
    {
        "Anim_Show",
        "Anim_Show_Idle",
        "Anim_Show_Idle_special01",
        "Anim_Show_Idle_special02",
    };

    static readonly string[] m_stateTable = new string[]
    {
        "Show",
        "Show_Idle",
        "Show_Idle_special01",
        "Show_Idle_special02",
    };

    public static void SelectRole(int idx)
    {
        _LoadActorLight(false);

        if (CreatePhase)
            SetSelectRoleActive(true);
        SetDemoRoleActive(false);
        ResetCamera();

        onSelectRot = true;
        CreatePhase = false;
        curAngularSpeed = 120;

        if (null != ActorLightRoot)
            ActorLightRoot.transform.position = Vector3.zero;

        if (m_CurRoleIdx < m_RoleList.Count)
        {
            RoleDemoDesc lastRole = m_RoleList[m_CurRoleIdx];
            if (null != lastRole && null != lastRole.m_RoleActor)
                lastRole.m_RoleActor.ChangeLayer(0);
        }
        
        if (idx < m_RoleList.Count)
        {
            RoleDemoDesc roleDesc = m_RoleList[idx];
            if (null != roleDesc && null != roleDesc.m_RoleActor)
                roleDesc.m_RoleActor.ChangeLayer(9);
        }

        _Rotate(idx);
    }

    public static void FinishDragSelect()
    {
		if (null != ActorSlot && m_RoleList.Count > 0)
        {
            curForce = 0;
            float seg = 360.0f / (ActorSlot.Length);
            float seghalf = seg * 0.5f;
            int targetIdx = ((int)(curAngle + seg * 0.5f) / (int)seg) % ActorSlot.Length;
            targetIdx = targetIdx < m_RoleList.Count ? targetIdx : m_RoleList.Count - 1;

            curTarget = targetIdx * (360 / ActorSlot.Length);

            if (targetIdx < m_RoleList.Count)
            {
                RoleDemoDesc lastRole = m_RoleList[targetIdx];
                if (null != lastRole && null != lastRole.m_RoleActor)
                    lastRole.m_RoleActor.ChangeLayer(9);
            }

            GameClient.SelectRoleFrame curSelect = GameClient.ClientSystemManager.instance.GetFrame(typeof(GameClient.SelectRoleFrame)) as GameClient.SelectRoleFrame;
            if (null != curSelect)
                curSelect.SetSelectedID(targetIdx);

            m_CurRoleIdx = targetIdx;
            if (null != ActorRoot)
                ActorRoot.transform.eulerAngles = new Vector3(0, curAngle, 0);

            onSelectRot = true;
        }
    }

    public static void SetSelectRoleActive(bool bActive)
    {
        if (null != m_RoleList)
        {
            for (int i = 0, icnt = m_RoleList.Count; i < icnt; ++i)
            {
                if (null != m_RoleList[i])
                {
                    GeDemoActor curActor = m_RoleList[i].m_RoleActor;
                    if (null != curActor && null != curActor.avatarRoot)
                    {
                        curActor.avatarRoot.SetActive(bActive);
                        if (bActive)
                            curActor.ChangeState(m_RoleList[i].m_JobString + m_stateTable[1]);
                    }
                }
            }
        }

    }
    public static void SetDemoRoleActive(bool bActive)
    {
        if (null != m_DemoActor)
        {
            GeDemoActor curActor = m_DemoActor.m_RoleActor;
            if (null != curActor)
            {
                curActor.avatarRoot.SetActive(bActive);
                if (bActive)
                {
                    curActor.ChangeState(m_DemoActor.m_JobString + m_stateTable[1]);
                }
                else
                    curActor.ClearEffect();
            }
        }
        CreatePhase = bActive;
    }

    public static void LoadDemoActor(int jobID, uint[] equipIDs)
    {
        if(null == m_DemoActor)
            m_DemoActor = new RoleDemoDesc();
        
        if(null == m_DemoActor.m_RoleActor)
            m_DemoActor.m_RoleActor = new GeDemoActor();

        CreatePhase = true;
        /// 加载展示角色
        ProtoTable.JobTable jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>((jobID / 10) * 10);
        if (jobData == null)
        {
            Logger.LogError("职业ID找不到 ID = [" + jobID + "]\n");
            return;
        }

        _LoadActorLight(true);

        if (null != m_DemoActor.m_RoleActor)
            m_DemoActor.m_RoleActor.Deinit();

        Utility.LoadDemoActor(m_DemoActor.m_RoleActor, jobID, true);
        if (jobData.JobShowPath.Count > 0)
        {
            string jobShowPath = jobData.JobShowPath[0];
            for (int i = 0; i < m_animTable.Length; ++i)
            {
                curJobString = null;
                if (!string.IsNullOrEmpty(jobShowPath))
                {
                    string[] splits = jobShowPath.Split('/');
                    curJobString = splits[splits.Length - 1];
                    curJobString = curJobString.Replace("Show", null);
                }
                m_DemoActor.m_RoleActor.LoadState(jobShowPath + "/" + curJobString + m_stateTable[i]);
            }
        }

        /// 加载展示武器
        string weaponPath = null;
        string cameraPath = null;
        bool bLeftWeapon = false;
        bool bRightWeapon = false;
        if (5 == jobID / 10)
        {
            cameraPath = "Actor/Hero_Gungirl/Camera/Prefabs/p_Hero_Gungirl_Show_camera";
            weaponPath = "Actor/Hero_Gungirl/WeaponShow/p_Hero_GunGirl_weapon_Show";

            bLeftWeapon = true;
            bRightWeapon = true;
        }
        else if (3 == jobID / 10)
        {
            cameraPath = "Actor/Hero_Magegirl/Camera/Prefabs/p_Hero_Magegirl_Show_camera";
            weaponPath = "";

            bLeftWeapon = false;
            bRightWeapon = false;
        }
        else if(2 == jobID / 10)
        {
            cameraPath = "Actor/Hero_Gunman/Camera/Prefabs/p_Hero_Gunman_Show_camera";
            weaponPath = "Actor/Hero_Gungirl/WeaponShow/p_Hero_GunGirl_weapon_Show";/// 使用女枪武器模型

            bLeftWeapon = true;
            bRightWeapon = true;
        }
        else if (1 == jobID / 10)
        {
            cameraPath = "Actor/Hero_Swordsman/Camera/Prefabs/p_Hero_Swordsman_Show_camera";
            weaponPath = "Actor/Hero_Swordsman/WeaponShow/p_Hero_Swordman_weapon_Show";
            bRightWeapon = true;
        }

        ResetCamera();
        if (!string.IsNullOrEmpty(cameraPath))
        {
            m_DemoActor.m_CameraNode = AssetLoader.instance.LoadResAsGameObject(cameraPath);
            if (null != m_DemoActor.m_CameraNode)
            {
                if (null != Camera.current)
                    sceneRoleCamera = Camera.current;

                curRoleCamera = m_DemoActor.m_CameraNode.GetComponentInChildren<Camera>();
                curCamAnim = m_DemoActor.m_CameraNode.GetComponentInChildren<Animation>();
                if (null != curRoleCamera && null != curCamAnim)
                {
                    curCamAnim.Play();
                    Camera.SetupCurrent(curRoleCamera);
                }
            }

            m_DemoActor.m_CameraNode.SetActive(false);
        }

        _EquipDemoWeapon(m_DemoActor, weaponPath, bLeftWeapon, bRightWeapon, 0);
        m_DemoActor.m_RoleActor.ChangeState(curJobString + m_stateTable[0]);
        m_DemoActor.m_LightRefNode = m_DemoActor.m_RoleActor.GeAttachNode("[actor]Crotch").transform;
        m_DemoActor.m_InitHeight = m_DemoActor.m_LightRefNode.position.y;

        SkinnedMeshRenderer[] asmr = m_DemoActor.m_RoleActor.avatarRoot.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0, icnt = asmr.Length; i < icnt; ++i)
            if (null != asmr[i])
                asmr[i].updateWhenOffscreen = true;

        canDragRot = false;
    }

    public static void ResetCamera()
    {
        if (null != sceneRoleCamera)
            Camera.SetupCurrent(sceneRoleCamera);

        if (null != curCamAnim)
        {
            curCamAnim.Stop();
            curCamAnim = null;
            curRoleCamera = null;
            if (null != m_DemoActor.m_CameraNode)
            {
                m_DemoActor.m_CameraNode.SetActive(false);
                Destroy(m_DemoActor.m_CameraNode);
                m_DemoActor.m_CameraNode = null;
            }
        }
    }

    protected static void _PrepareSlots(Vector3 piovt,float radius)
    {
        if (null == ActorRoot)
            ActorRoot = new GameObject("CreateRoleRoot");

        if (null != ActorRoot)
        {
            if(null == ActorSlot)
                ActorSlot = new GameObject[MAX_ACTOR_NUM];

            Matrix4x4 mat = new Matrix4x4();
            Quaternion rot = new Quaternion();
            Vector3 localPos = new Vector3(radius, 0, 0);
            for (int i = 0, icnt = ActorSlot.Length; i < icnt; ++i)
            {
                if (null == ActorSlot[i])
                    ActorSlot[i] = new GameObject("RoleSlot" + i.ToString());

                rot.eulerAngles = new Vector3(0, -(360 / icnt) * i + 90, 0);
                mat.SetTRS(Vector3.zero, rot,Vector3.one);
                Vector3 slotPos = mat.MultiplyPoint(localPos);

                ActorSlot[i].transform.position = slotPos;
                ActorSlot[i].transform.localEulerAngles = new Vector3(0,- (360 / icnt) * i, 0);
                ActorSlot[i].transform.SetParent(ActorRoot.transform, true);
            }
        }
    }

    public static void _Rotate(int slotIdx)
    {
        if (slotIdx == m_CurRoleIdx)
            return;

        if(slotIdx < ActorSlot.Length)
        {
            if(slotIdx != m_CurRoleIdx)
            {
                targetAngle = slotIdx * (360 / ActorSlot.Length);

                while (targetAngle <= 0)
                    targetAngle += 360;
                while (targetAngle > 360)
                    targetAngle -= 360;

                //curAngle = m_PlayIdx * (360 / ActorSlot.Length);
                //
                while (curAngle < 0)
                    curAngle += 360;
                while (curAngle >= 360)
                    curAngle -= 360;

                //if (curAngle > targetAngle)
                //    curFactor = -1.0f;
                //else
                //    curFactor = 1.0f;
                m_CurRoleIdx = slotIdx;
            }
        }
    }

    public static void _DragRot(float delta)
    {
        if (m_RoleList.Count < 2)
            return;

        curForce = -delta;
        onSelectRot = false;
        curAngularSpeed = 120;
        hasAlign = false;

        if (m_CurRoleIdx < m_RoleList.Count)
        {
            RoleDemoDesc lastRole = m_RoleList[m_CurRoleIdx];
            if (null != lastRole && null != lastRole.m_RoleActor)
                lastRole.m_RoleActor.ChangeLayer(0);
        }
    }
    public static void _DragRotActor(float delta)
    {
        if(CreatePhase && canDragRot)
        {
            curRoleForce = -delta;
        }
    }

    public static void AddSelectActor(int jobID, uint[] equipIDs, int weaponStrengthenLv)
    {
        _PrepareSlots(Vector3.zero, 1.0f);

        /// 加载展示角色
        ProtoTable.JobTable jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>((jobID / 10) * 10);
        if (jobData == null)
        {
            Logger.LogError("职业ID找不到 ID = [" + jobID + "]\n");
            return;
        }

        RoleDemoDesc newDemoRole = new RoleDemoDesc();
        newDemoRole.m_RoleActor = new GeDemoActor();

        Utility.LoadDemoActor(newDemoRole.m_RoleActor, jobID,true);
        if (jobData.JobShowPath.Count > 0)
        {
            string jobShowPath = jobData.JobShowPath[0];
            for (int i = 0; i < m_animTable.Length; ++i)
            {
                newDemoRole.m_JobString = null;
                if (!string.IsNullOrEmpty(jobShowPath))
                {
                    string[] splits = jobShowPath.Split('/');
                    newDemoRole.m_JobString = splits[splits.Length - 1];
                    newDemoRole.m_JobString = newDemoRole.m_JobString.Replace("Show", null);
                }
                newDemoRole.m_RoleActor.LoadState(jobShowPath + "/" + newDemoRole.m_JobString + m_stateTable[i]);
            }
        }

        /// 加载展示武器
        List<uint> armorList = new List<uint>();
        string weaponPath = null;
        if (null != equipIDs)
        {
            for (int i = 0, icnt = equipIDs.Length; i < icnt; ++i)
            {
                string res = GameClient.PlayerBaseData.GetInstance().GetWeaponResFormID((int)equipIDs[i]);
                if (string.IsNullOrEmpty(res))
                    armorList.Add(equipIDs[i]);
                else
                    weaponPath = res;
            }
        }

        /// 其他装配强化
        if(armorList.Count > 0)
            GameClient.PlayerBaseData.GetInstance().AvatarEquipFromItems(newDemoRole.m_RoleActor, armorList.ToArray(), jobID, weaponStrengthenLv, null);

        if (string.IsNullOrEmpty(weaponPath))
        {
            ProtoTable.JobTable jobTbl = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(jobID);
            weaponPath = jobTbl.DefaultWeaponPath;
        }

        bool bLeftWeapon = false;
        bool bRightWeapon = false;
        if (5 == jobID / 10 || 2 == jobID / 10)
        {
            bLeftWeapon = true;
            bRightWeapon = true;
        }
        else if (1 == jobID / 10)
        {
            bRightWeapon = true;
        }
        _EquipDemoWeapon(newDemoRole, weaponPath, bLeftWeapon, bRightWeapon, weaponStrengthenLv);

        newDemoRole.m_RoleActor.ChangeState(newDemoRole.m_JobString + m_stateTable[1]);

        int roleSlotIdx = m_RoleList.Count < ActorSlot.Length ? m_RoleList.Count : ActorSlot.Length - 1;
        newDemoRole.m_RoleActor.avatarRoot.transform.SetParent(ActorSlot[roleSlotIdx].transform, false);
        newDemoRole.m_RoleActor.ChangeLayer(0);
        m_RoleList.Add(newDemoRole);
    }

    public static void ClearSelectRole()
    {
        if(null != m_RoleList)
        {
            for (int i = 0, icnt = m_RoleList.Count; i < icnt; ++i)
            {
                if (null != m_RoleList[i])
                {
                    GeDemoActor curActor = m_RoleList[i].m_RoleActor;
                    if (null != curActor)
                    {
                        if(null != curActor.avatarRoot)
                            curActor.avatarRoot.transform.SetParent(null, false);
                        curActor.Deinit();
                    }
                    curActor = null;
                }
            }
        }

        m_RoleList.Clear();

        if(null != ActorRoot)
            ActorRoot.transform.eulerAngles = Vector3.zero;

        if (null != ActorSlot)
        {
            for (int i = 0, icnt = ActorSlot.Length; i < icnt; ++i)
            {
                if (null != ActorSlot[i])
                {
                    ActorSlot[i].transform.position = Vector3.zero;
                    ActorSlot[i].transform.localEulerAngles = Vector3.zero;
                }
            }
        }

        curAngle = 0;
        targetAngle = 0;
        m_CurRoleIdx = 0;
        curFactor = 1;
    }

    public static void ClearDemoRole()
    {
        if(null != m_DemoActor && null != m_DemoActor.m_RoleActor)
        {
            m_DemoActor.m_RoleActor.Deinit();
            m_DemoActor.m_RoleActor = null;
        }
    }

    protected static void _EquipDemoWeapon(RoleDemoDesc actorDesc,string weaponRes,bool LHand,bool RHand,int strengthLv = 0,int weaponLayer = 18)
    {
        if(null != actorDesc && null != actorDesc.m_RoleActor && !string.IsNullOrEmpty(weaponRes))
        {
            if(LHand)
            {
                actorDesc.m_LeftWeapon = actorDesc.m_RoleActor.AttachAvatar("weaponL", weaponRes, "[actor]LWeapon");
                actorDesc.m_LeftWeapon.SetLayer(weaponLayer);
                actorDesc.m_LeftWeapon.ChangePhase(BeUtility.GetStrengthenEffectName(actorDesc.m_LeftWeapon.ResPath), strengthLv);
            }

            if(RHand)
            {
                actorDesc.m_RightWeapon = actorDesc.m_RoleActor.AttachAvatar("weaponR", weaponRes, "[actor]RWeapon");
                actorDesc.m_RightWeapon.SetLayer(weaponLayer);
                actorDesc.m_RightWeapon.ChangePhase(BeUtility.GetStrengthenEffectName(actorDesc.m_RightWeapon.ResPath), strengthLv);
            }
        }
    }

    public static void CheckLoadCreateRoleScene()
    {
         if(sRoleSceneLoaded == false)
         {
             //Application.LoadLevelAdditive(4);
             SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
         }
    }
 
    void Update()
    {
        if(CreatePhase)
        {
            if (null != m_DemoActor && null != m_DemoActor.m_RoleActor)
            {
                if (m_DemoActor.m_RoleActor.IsCurActionEnd())
                {
                    canDragRot = true;
                    m_DemoActor.m_RoleActor.ChangeState(curJobString + m_ActionTable[m_PlayList[m_DemoActor.m_PlayIdx]]);
                    ++m_DemoActor.m_PlayIdx;

                    if (m_DemoActor.m_PlayIdx >= m_PlayList.Length)
                        m_DemoActor.m_PlayIdx = UnityEngine.Random.Range(0, m_PlayList.Length);

                    m_DemoActor.m_PlayIdx = m_DemoActor.m_PlayIdx % m_PlayList.Length;
                }

                m_DemoActor.m_RoleActor.OnUpdate(1 / 30.0f);
                if(null != ActorLightRoot && null != m_DemoActor.m_LightRefNode)
                {
                    Vector3 pos = ActorLightRoot.transform.position;
                    pos.y = m_DemoActor.m_LightRefNode.position.y - m_DemoActor.m_InitHeight;
                    ActorLightRoot.transform.position = pos;
                }
                
                Vector3 curRoleAngle = m_DemoActor.m_RoleActor.avatarRoot.transform.eulerAngles;
                curRoleForce *= damp;
                curRoleAngle.y += curRoleForce;
                m_DemoActor.m_RoleActor.avatarRoot.transform.eulerAngles = curRoleAngle;
                
                if (null != m_DemoActor.m_CameraNode && false == m_DemoActor.m_CameraNode.activeSelf)
                    m_DemoActor.m_CameraNode.SetActive(true);
            }
        }
        else
        {
            for (int i = 0, icnt = m_RoleList.Count; i < icnt; ++i)
            {
                if (null != m_RoleList[i])
                {
                    GeDemoActor curActor = m_RoleList[i].m_RoleActor;
                    curActor.OnUpdate(0);

                    if (null != curActor && i == m_CurRoleIdx)
                    {
                        if (curActor.IsCurActionEnd())
                        {
                            curActor.ChangeState(m_RoleList[i].m_JobString + m_ActionTable[m_PlayList[m_RoleList[i].m_PlayIdx]]);
                            ++m_RoleList[i].m_PlayIdx;

                            if (m_RoleList[i].m_PlayIdx >= m_PlayList.Length)
                                m_RoleList[i].m_PlayIdx = UnityEngine.Random.Range(0, m_PlayList.Length);

                            m_RoleList[i].m_PlayIdx = m_RoleList[i].m_PlayIdx % m_PlayList.Length;
                        }

                        curActor.OnUpdate(1 / 30.0f);
                    }
                }
            }
        }

        _UpdateSelectRotation();
        _UpdateDragRotation();
    }

    void _UpdateSelectRotation()
    {
        if (!onSelectRot)
            return;

        if (null != ActorRoot)
        {
            if (Mathf.Abs(curAngle - targetAngle) >= 2.0f)
            {
                curAngularAcc = Mathf.Abs((curAngle - targetAngle) / 30);
                curAngle += (1 / 60.0f) * curAngularSpeed * curAngularAcc;

                while (curAngle < 0)
                    curAngle += 360;
                while (curAngle >= 360)
                    curAngle -= 360;

                ActorRoot.transform.eulerAngles = new Vector3(0, curAngle, 0);
            }
            else
                onSelectRot = false;
        }
    }

    static readonly float TIME_SLICE = 0.016666666667f;
    void _UpdateDragRotation()
    {
        if (onSelectRot)
            return;

        if (m_RoleList.Count < 2)
            return;

        if (null != ActorRoot)
        {
            if (Mathf.Abs(curForce) >= 0.1f)
            {
                curForce *= damp;
                curAngle += curForce;
            }
            else
            {
                float seg = 360.0f / (ActorSlot.Length);
                float seghalf = seg * 0.5f;
                int targetIdx = ((int)(curAngle + seg * 0.5f) / (int)seg) % ActorSlot.Length;
                targetIdx = targetIdx < m_RoleList.Count ? targetIdx : m_RoleList.Count - 1;

                curTarget = targetIdx * (360 / ActorSlot.Length);

                while (curTarget <= 0)
                    curTarget += 360;
                while (curTarget > 360)
                    curTarget -= 360;

                while (curAngle <= 0)
                    curAngle += 360;
                while (curAngle > 360)
                    curAngle -= 360;

                float shortest = 0;
                float factor = 0;
                if (curTarget - seghalf < curAngle && curAngle < curTarget)
                {
                    shortest = curTarget - curAngle;
                    factor = 1.0f;
                }
                else if(curTarget < curAngle && curAngle < curTarget + seghalf)
                {
                    shortest = curTarget + seghalf - curAngle;
                    factor =  - 1.0f;
                }
                else
                {
                    shortest = Mathf.Abs(curTarget - curAngle);
                    shortest = shortest < seghalf ? shortest : 360 - shortest;

                    factor = 1.0f;
                }

                float speedFactor = (shortest / 30);
                speedFactor = speedFactor > 1.0f ? 1.0f : speedFactor;

                if(speedFactor < 0.1f)
                {
                    if (targetIdx < m_RoleList.Count)
                    {
                        RoleDemoDesc lastRole = m_RoleList[targetIdx];
                        if (null != lastRole && null != lastRole.m_RoleActor)
                            lastRole.m_RoleActor.ChangeLayer(9);
                    }

                    if (lastIdx != targetIdx)
                    {
                        GameClient.SelectRoleFrame curSelect = GameClient.ClientSystemManager.instance.GetFrame(typeof(GameClient.SelectRoleFrame)) as GameClient.SelectRoleFrame;
                        if (null != curSelect)
                            curSelect.SetSelectedID(targetIdx);

                        lastIdx = targetIdx;
                    }
                    m_CurRoleIdx = targetIdx;
                }

                curAngle += TIME_SLICE * curAngularSpeed * speedFactor * factor;
                //Debug.LogFormat("curTarget:{0}  targetIdx{1} curAngle:{2} XX:{3}", curTarget, targetIdx, curAngle, (shortest / 30));
            }

            ActorRoot.transform.eulerAngles = new Vector3(0, curAngle, 0);
        }
    }

    void Start () 
	{
        GePhaseEffect.instance.UnInit();
        GePhaseEffect.instance.Init();

        var current = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(current);

        LightNode = GameObject.Find("Environment/Directional light");
        CameraNode = GameObject.Find("Environment/FollowPlayer/Main Camera");
	
        if(LightNode == null || CameraNode == null)
        {
            Logger.LogErrorFormat("创建角色脚本没有找到 Environment环境，请查询创建流程");
        }

        if(LightNode != null)
        LightNode.CustomActive(false);
        
        if(CameraNode != null)
        CameraNode.CustomActive(false);

        sRoleSceneLoaded = true;

        sceneRoleCamera = Camera.current;

        //ActorLightRoot = GameObject.Find("HGCreateRoleRoot/Light/Character Light");
        ActorLightRoot = null;
        SceneRoot = GameObject.Find("HGCreateRoleRoot");

        sEffect = AssetLoader.instance.LoadResAsGameObject("Effects/Scene_effects/Start/Prefab/Eff_Start");

        _PrepareActorRes();
    }

    void OnDisable()
    {
        if (LightNode != null)
            LightNode.CustomActive(true);

        if (CameraNode != null)
            CameraNode.CustomActive(true);

        sRoleSceneLoaded = false;

        if (null != m_DemoActor)
        {
            if (null != m_DemoActor.m_RoleActor)
                m_DemoActor.m_RoleActor.Deinit();

            if (null != m_RoleList)
            {
                for (int i = 0, icnt = m_RoleList.Count; i < icnt; ++i)
                {
                    if (null != m_RoleList[i])
                    {
                        GeDemoActor curActor = m_RoleList[i].m_RoleActor;
                        if (null != curActor)
                            curActor.Deinit();
                    }
                }
            }
        }

        if (null != ActorRoot)
        {
            GameObject.Destroy(ActorRoot);
            ActorRoot = null;
        }
    }
	void OnDestroy()
	{
        ActorLightRoot.transform.position = Vector3.zero;

        if (LightNode != null)
        LightNode.CustomActive(true);
        
        if(CameraNode != null)
        CameraNode.CustomActive(true);

        var current = SceneManager.GetSceneByName(sceneName);
        if(null != current)
            SceneManager.UnloadScene(current.buildIndex);

        if (null != sEffect)
        {
            GameObject.Destroy(sEffect);
            sEffect = null;
        }
        sRoleSceneLoaded = false;
	}

    protected void _PrepareActorRes()
    {
        GeDemoActor preload = new GeDemoActor();

        Utility.LoadDemoActor(preload, 10, true);
        preload.Deinit();

        Utility.LoadDemoActor(preload, 20, true);
        preload.Deinit();

        Utility.LoadDemoActor(preload, 30, true);
        preload.Deinit();

        Utility.LoadDemoActor(preload, 50, true);
        preload.Deinit();
    }


    static protected void _LoadActorLight(bool createRole)
    {
        if (null != SceneLightRoot)
            Destroy(SceneLightRoot);

        if(createRole)
        {/// 创角灯光
            SceneLightRoot = AssetLoader.instance.LoadResAsGameObject("Scene/Start/Perfab/Light_chuangjue");
        }
        else
        {/// 选角灯光
            SceneLightRoot = AssetLoader.instance.LoadResAsGameObject("Scene/Start/Perfab/Light_xuanjue");
        }

        if (null != SceneLightRoot)
        {
            SceneLightRoot.name = "Light";
            if (null != SceneRoot)
                SceneLightRoot.transform.SetParent(SceneRoot.transform, false);

            Transform rootLightTM = SceneLightRoot.transform.Find("Character Light");
            if (null != rootLightTM)
                ActorLightRoot = rootLightTM.gameObject;
        }
    }
}