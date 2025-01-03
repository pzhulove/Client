
namespace GameClient
{

    public delegate void OnStorageSelectItemClick(int itemIndex);

    //仓库的类型：角色账号和账号仓库
    public enum StorageType
    {
        RoleStorage,    // 角色仓库
        AccountStorage, // 账号仓库
    }


    //解锁仓库消耗的数据模型
    public class StorageUnLockCostDataModel
    {
        public int CostItemId;              //消耗的道具Id
        public int CostItemNumber;          //消耗的道具数量
    }


    public class StorageItemDataModel
    {
        public ulong ItemGuid;          //道具的Guid
        public ItemData ItemData;       //道具数据
    }

}
