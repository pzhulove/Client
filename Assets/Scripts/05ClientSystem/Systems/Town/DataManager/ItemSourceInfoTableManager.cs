using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class ItemSourceInfoTableManager : DataManager<ItemSourceInfoTableManager>
    {
        public static string kItemSourceInfoTablePath = "Data/ItemSourceInfoTable/ItemSourceInfoTable.asset";

        private ItemSourceInfoTable mItemSourceInfoTable = null;

        public override void Clear()
        {
            mItemSourceInfoTable = null;
        }

        public override void Initialize()
        {
            mItemSourceInfoTable = AssetLoader.instance.LoadRes(kItemSourceInfoTablePath, typeof(ItemSourceInfoTable)).obj as ItemSourceInfoTable;
            if (null == mItemSourceInfoTable)
            {
                Logger.LogErrorFormat("[itemsourceinfotable] 加载 {0} 失败", kItemSourceInfoTablePath);
                return;
            }
            //mItemSourceInfoTable.sources.Sort();
        }

        public string GetSourceInfoName(ISourceInfo info)
        {
            return ItemSourceInfoUtility.GetLinkName(mItemSourceInfoTable, info);
        }

        public string GetSourceInfoLink(ISourceInfo info)
        {
            return ItemSourceInfoUtility.GetLinkInfo(mItemSourceInfoTable, info);
        }

        public int GetItemBaseScore(int itemId)
        {
            if (null == mItemSourceInfoTable || null == mItemSourceInfoTable.scores)
            {
                return 0;
            }

            for (int i = 0; i < mItemSourceInfoTable.scores.Length; ++i)
            {
                if (mItemSourceInfoTable.scores[i].itemID == itemId)
                {
                    return mItemSourceInfoTable.scores[i].score;
                }
            }

            return 0;
        }

        /// <summary>
        /// 是否包含除了拍卖行的获取途径
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool IsContainSourceInfoExceptAuction(int itemId)
        {
            if (null == mItemSourceInfoTable)
            {
                return false;
            }

            SourceInfo[] allSourceInfos = null;// _binarySearch(itemId);

            // TODO binary search
            for (int i = 0; i < mItemSourceInfoTable.sources.Length; i++)
            {
                if (mItemSourceInfoTable.sources[i].itemID == itemId)
                {
                    allSourceInfos = mItemSourceInfoTable.sources[i].sources;
                    break;
                }
            }

            if (null == allSourceInfos)
            {
                return false;
            }

            if (0 == allSourceInfos.Length)
            {
                return false;
            }

            if (1 == allSourceInfos.Length && allSourceInfos[0].type == eItemSourceType.eAuction)
            {
                return false;
            }

            return true;
        }

        public IEnumerator GetSourceInfos(int itemId)
        {
            if (null == mItemSourceInfoTable)
            {
                yield break;
            }

            SourceInfo[] allSourceInfos = null;// _binarySearch(itemId);

            // TODO binary search
            for (int i = 0; i < mItemSourceInfoTable.sources.Length; i++)
            {
                if (mItemSourceInfoTable.sources[i].itemID == itemId)
                {
                    allSourceInfos = mItemSourceInfoTable.sources[i].sources;
                    break;
                }
            }

            if (null == allSourceInfos)
            {
                yield break;
            }

            bool hasGotDungeon = false;

            for (int i = 0; i < allSourceInfos.Length; ++i)
            {
                if (allSourceInfos[i].type == eItemSourceType.eDungeon)
                {
                    if (!hasGotDungeon)
                    {
                        hasGotDungeon = true;
                    }
                    else
                    {
                       continue;
                    }
                }


                yield return allSourceInfos[i];
            }
        }
        
        private SourceInfo[] _binarySearch(int itemId)
        {
            int startIdx = 0;
            int endIdx = mItemSourceInfoTable.sources.Length - 1;

            int midIdx = -1;
            int midItemId = -1;

            while (startIdx < endIdx)
            {
                midIdx = (startIdx + endIdx) / 2;

                midItemId = mItemSourceInfoTable.sources[midIdx].itemID;

                if (midItemId == itemId)
                {
                    break;
                }
                else if (midItemId > itemId)
                {
                    endIdx = midItemId - 1;
                }
                else
                {
                    startIdx = midItemId + 1;
                }
            }


            return null;
        }
    }
}
