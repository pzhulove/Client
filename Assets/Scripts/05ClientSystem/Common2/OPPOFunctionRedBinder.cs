using UnityEngine;
using System.Collections.Generic;
using Protocol;
using ProtoTable;

namespace GameClient
{
    class OPPOFunctionRedBinder : MonoBehaviour
    {
        public enum OPPOFunctionType
        {
            OFT_PRIVILRGR,//启动特权
            OFT_LUCKYGUY,//幸运转盘
            OFT_DAILYCHECK,//每日签到
            OFT_AMBERGIFTBAG,//琥珀礼包
            OFT_AMBERPRIVILEGE, //琥珀特权
            OFT_OPPOGROWTHHAOLI, //OPPO成长豪礼
        }

        public List<OPPOFunctionType> m_akFunctionTypes = new List<OPPOFunctionType>();
        public GameObject m_goTarget = null;
        public const int oppoPrivilegeID = 12000;
        public const int vivoPrivilegeID = 23000;
        public const int dailID = 15000;
        public const int luckyGuyID = 17000;
        public const int tableID = 10001;
        int IActivitytEmplateID =20000;

        private static OPPOFunctionRedBinder _instance;
        public static OPPOFunctionRedBinder instance
        {
            get
            {
                return _instance;
            }
        }
        void Start()
        {
            _instance = this;
            _Updata();
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;//让活动刷新的回调
        }
        void OnEnable()
        {
            _Updata();
        }

        void Update()
        {
            _Updata();
        }

        void OnDestroy()
        {
            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;//让活动刷新的回调
        }

        void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            _Updata();
        }
        void _Updata()
        {
            if (m_goTarget == null)
            {
                return;
            }

            bool bResultOk = false;
            for (int i = 0; !bResultOk&& i < m_akFunctionTypes.Count; ++i)
            {
                bResultOk = _CheckOK(m_akFunctionTypes[i]);
            }

            m_goTarget.CustomActive(bResultOk);
        }

        public void AddCheckFunction(OPPOFunctionType eOPPOFuctionType)
        {
            if (!m_akFunctionTypes.Contains(eOPPOFuctionType))
            {
                m_akFunctionTypes.Add(eOPPOFuctionType);
            }
            _Updata();
        }

        public void ClearCheckFunctions()
        {
            m_akFunctionTypes.Clear();
            _Updata();
        }

        bool _CheckOK(OPPOFunctionType eOPPOFunctionType)
        {
            if (eOPPOFunctionType == OPPOFunctionType.OFT_PRIVILRGR)
            {
                if (SDKInterface.Instance.IsOppoPlatform())
                {
                    return OPPOPrivilegeDataManager.GetInstance()._CheckPrivilrge(oppoPrivilegeID);
                }
                else if (SDKInterface.Instance.IsVivoPlatForm())
                {
                    return OPPOPrivilegeDataManager.GetInstance()._CheckPrivilrge(vivoPrivilegeID);
                }
            }
            else if (eOPPOFunctionType == OPPOFunctionType.OFT_LUCKYGUY)
            {
                return OPPOPrivilegeDataManager.GetInstance()._CheckLuckyGuy();
            }
            else if (eOPPOFunctionType == OPPOFunctionType.OFT_DAILYCHECK)
            {
                return OPPOPrivilegeDataManager.GetInstance()._CheckDail();
            }
            else if (eOPPOFunctionType == OPPOFunctionType.OFT_AMBERGIFTBAG)
            {
                return OPPOPrivilegeDataManager.GetInstance()._CheckAmberGiftBag();
            }
            else if (eOPPOFunctionType == OPPOFunctionType.OFT_AMBERPRIVILEGE)
            {
                return OPPOPrivilegeDataManager.GetInstance()._CheckAmberPrivilege();
            }
            else if (eOPPOFunctionType == OPPOFunctionType.OFT_OPPOGROWTHHAOLI)
            {
                return OPPOPrivilegeDataManager.GetInstance()._CheckOPPOGrowthHaoLi();
            }
            return false;
        }
        
    }
}