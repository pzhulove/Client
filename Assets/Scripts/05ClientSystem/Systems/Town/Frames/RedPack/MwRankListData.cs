using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    class MwRankListData: IComparable<MwRankListData>
    {
        /// <summary>
        /// 名次
        /// </summary>
        public int rank { get; set; }
        /// <summary>
        /// 服务器ID
        /// </summary>
        public int zone_id { get; set; }
        /// <summary>
        /// 账号ID
        /// </summary>
        public int acc_id { get; set; }
        /// <summary>
        /// 角色GUID
        /// </summary>
        public string role_id { get; set; }
        /// <summary>
        /// 总名望
        /// </summary>
        public int total_mw { get; set; }
        /// <summary>
        /// 角色姓名
        /// </summary>
        public string role_name { get; set; }
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string server_name { get; set; }

        // 更新时间
        public string time { get; set; }

        public int level { get; set; }

        public int occ { get; set; }

        public int CompareTo(MwRankListData other)
        {
            return rank - other.rank;
        }
    }
}
