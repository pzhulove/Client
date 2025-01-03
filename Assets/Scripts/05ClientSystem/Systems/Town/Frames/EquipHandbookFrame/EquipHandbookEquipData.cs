using System.Collections.Generic;

namespace GameClient
{
    /// <summary>
    /// 查询返回格式
    /// </summary>
    class EquipHandbookEquipQueryData
    {
        /// <summary>
        /// 账号ID
        /// </summary>
        //public System.Int32 accountid { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        //public System.Int64 roleid { get; set; }
        /// <summary>
        /// 道具ID
        /// </summary>
        //public int itemid { get; set; }

        /// <summary>
        /// 返回码，是否成功
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 装备评论数据
        /// </summary>
        public List<EquipHandbookCommentData> itemcomments = new List<EquipHandbookCommentData>();
    }

    class EquipHandbookCommentData
    {
        /// <summary>
        /// EquipHandbookCommentTable.ID
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 喜欢人数目
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 自己是否喜欢
        /// </summary>
        public int selflike { get; set; }
    }
}
