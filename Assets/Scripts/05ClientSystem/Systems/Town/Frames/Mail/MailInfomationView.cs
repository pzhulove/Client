using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class MailInfomationView : MonoBehaviour
    {
        [SerializeField]private Text mMailTitle;
        [SerializeField]private Text mMailSenderName;
        [SerializeField]private LinkParse mMailContent;
        [SerializeField]private List<RectTransform> mPostions;
        [SerializeField]private GameObject mItemsRoot;

        private MailDataModel mMailDataModel;
        private List<ComItem> mItems = new List<ComItem>();

        private void Awake()
        {
            InitPackageItem();
        }

        private void OnDestroy()
        {
            mMailDataModel = null;
            mItems.Clear();
        }

        private void InitPackageItem()
        {
            
            int index = 0;

            for (int i = 0; i < mPostions.Count; i++)
            {
                if (mPostions[i].name != string.Format("Pos{0}",index + 1))
                {
                    continue;
                }

                ComItem item = ComItemManager.Create(mPostions[i].gameObject);
                if (index < MailDataManager.MailItemNum)
                {
                    mItems.Add(item);
                }

                index++;
            }
        }

        private void ClearItemIcon()
        {
            for (int i = 0; i < mItems.Count; i++)
            {
                if (mItems[i] != null)
                {
                    mItems[i].Setup(null, null);
                }
            }
        }

        /// <summary>
        /// 更新邮件信息的展示信息
        /// </summary>
        /// <param name="mailDataModel"></param>
        public void UpdateMailInfoMationView(MailDataModel mailDataModel)
        {
            mMailDataModel = mailDataModel;
            UpdateMainInfo();
        }

        private void UpdateMainInfo()
        {
            if (mMailDataModel == null)
            {
                return;
            }

            if (mMailDataModel.info == null)
            {
                //Logger.LogErrorFormat("[MainInfomationView] UpdateMainInfo 邮件标题信息为空");
                return;
            }

            mMailTitle.text = mMailDataModel.info.title;

            mMailSenderName.text = mMailDataModel.info.sender;

            string mailContent = mMailDataModel.content.Replace("\\n", "\n");
            //mailContent += string.Format("{{U {0} {1} {2}}}", "前往", "http://ald.xy.com", "4");

            mMailContent.SetText(mailContent);

            ClearItemIcon();

            if (mMailDataModel.info.hasItem == 1)
            {
                int index = 0;

                for (int i = 0; i < mMailDataModel.items.Count && i < MailDataManager.MailItemNum; i++)
                {
                    var rewardData = mMailDataModel.items[i];
                    if (rewardData == null)
                    {
                        continue;
                    }

                    ItemData data = ItemDataManager.CreateItemDataFromTable((int)rewardData.id, rewardData.qualityLv, rewardData.strength);
                    if (data == null)
                    {
                        continue;
                    }

                    data.Count = (int)rewardData.num;
                    data.StrengthenLevel = rewardData.strength;
                    SetUpItem(data, i);

                    index++;
                }

                for (int i = 0; i < mMailDataModel.detailItems.Count && (index + i) < MailDataManager.MailItemNum; i++)
                {
                    ItemData data = ItemDataManager.GetInstance().CreateItemDataFromNet(mMailDataModel.detailItems[i]);
                    if (data == null)
                    {
                        continue;
                    }

                    data.Count = mMailDataModel.detailItems[i].num;

                    SetUpItem(data, index + i);
                }
            }

            mItemsRoot.CustomActive(mMailDataModel.info.hasItem == 1);
        }

        private void SetUpItem(ItemData data,int index)
        {
            if (mItems == null || index >= mItems.Count)
            {
                return;
            }

            int idx = index;

            mItems[idx].Setup(data, Utility.OnItemClicked);
        }
    }
}

