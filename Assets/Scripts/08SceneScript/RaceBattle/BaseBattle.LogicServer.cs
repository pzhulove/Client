using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameClient
{
    public partial class BaseBattle
    {
        public void StartLogicServer(LogicServer logicServer)
        {
            mLogicServer = logicServer;

            //所有加载同步随机数种子
            FrameRandom.ResetSeed((uint)ClientApplication.playerinfo.session);
            OnAfterSeedInited();
            mDungeonManager.GetDungeonDataManager().FirstArea();
            _onStart();
            _createBase();
            _createEntitys();
            _onSceneStart();
            _bindEvents();

            mDungeonManager.StartFight();

#if LOGIC_SERVER
            PostStart();
#endif
        }

        
    }
}
