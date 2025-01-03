using UnityEngine;
using System.Collections;
namespace GameClient
{
    class WrappedTestFrame : WrappedClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UI/Prefabs/ActiveFrame/TestFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
        }

        public class TestData : WrappedData
        {

        }
        //class WrappedTestItem : WrappedCachedItemObject<WrappedTestFrame, TestData>
        //{
        //    TestFrame.ItemParent item = new TestFrame.ItemParent();

        //    public override void OnCreate(object[] param)
        //    {
        //        base.OnCreate(param);
        //        item.Create(goLocal);
        //        Enable();
        //        Update();
        //    }

        //    public override void OnDestroy()
        //    {
        //        item.Destroy(goLocal);
        //    }
        //}
        //ClientCachedObject<ulong, WrappedTestItem> m_akItemObjects = new ClientCachedObject<ulong, WrappedTestItem>();
    }
}