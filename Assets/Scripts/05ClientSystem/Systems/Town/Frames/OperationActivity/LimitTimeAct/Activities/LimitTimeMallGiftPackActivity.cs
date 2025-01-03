using ProtoTable;

namespace GameClient
{
    /// <summary>
    /// 商城礼包，子分页
    /// </summary>
    public class LimitTimeMallGiftPackActivity : LimitTimeMallGiftPackActivityBase
    {
	    protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/MallGiftPackItem";
        }

        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/MallGiftPackActivity";
        }
    }
}