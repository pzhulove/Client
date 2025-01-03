using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    class RedPackRankListData: IComparable<RedPackRankListData>
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
        /// 总点券
        /// </summary>
        public int total_money { get; set; }
        /// <summary>
        /// 角色姓名
        /// </summary>
        public string role_name { get; set; }
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string server_name { get; set; }

        public int CompareTo(RedPackRankListData other)
        {
            return rank - other.rank;
        }
    }
}
