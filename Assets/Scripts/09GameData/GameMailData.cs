using System;
using Protocol;
using System.Collections.Generic;

public class GameMailData
{
    public void ClearData()
    {
        mailList.Clear();
        mailDics.Clear();
        OneKeyDeleteNum = 0;
        OneKeyReceiveNum = 0;
        UnreadNum = 0;

        rewardMailList.Clear();
        rewardMailDics.Clear();
        OneKeyDeleteRewardNum = 0;
        OneKeyReceiveRewardNum = 0;
        UnreadRewardNum = 0;
    }

    public class MailData
    {
        public MailTitleInfo info;
        public string content;

		// 物品
        public List<ItemReward> items = new List<ItemReward>();
        public List<Item> detailItems = new List<Item>(); 
    }

    public List<MailTitleInfo> mailList = new List<MailTitleInfo>();
    public List<MailTitleInfo> rewardMailList = new List<MailTitleInfo>();
    public DictionaryView<UInt64,MailData> mailDics = new DictionaryView<UInt64,MailData>();
    public DictionaryView<UInt64, MailData> rewardMailDics = new DictionaryView<ulong, MailData>();

    public int OneKeyDeleteNum;
    public int OneKeyReceiveNum;
    public int UnreadNum;

    public int OneKeyDeleteRewardNum;
    public int OneKeyReceiveRewardNum;
    public int UnreadRewardNum;


    public MailTitleInfo FindMailTitleInfo(List<MailTitleInfo> mailList,UInt64 id)
    {
        return mailList.Find(x => x.id == id);
    }
 

    public MailData  FindMailData(DictionaryView<UInt64, MailData> mailDics,UInt64 id)
    {
        MailData data;
        if( mailDics.TryGetValue(id,out data) )
        {
            return data;
        }
        
        return null;
    }

    public void DeleteMailTitleInfo(UInt64 id)
    {
        for(int i = 0; i < mailList.Count; i++)
        {
            if(mailList[i].id == id)
            {
                mailList.RemoveAt(i);
                break;
            }
        }
    }

    public void DeleteRewardMailTitleInfo(UInt64 id)
    {
        for (int i = 0; i < rewardMailList.Count; i++)
        {
            if (rewardMailList[i].id == id)
            {
                rewardMailList.RemoveAt(i);
                break;
            }
        }
    }

    public void DeleteMailData(UInt64 id)
    {
        if(mailDics.ContainsKey(id))
        {
            mailDics.Remove(id);
        }
    }

    public void DeleteRewardMailData(UInt64 id)
    {
        if (rewardMailDics.ContainsKey(id))
        {
            rewardMailDics.Remove(id);
        }
    }

    public void SortMailList()
    {
        List<MailTitleInfo> UnReadList = new List<MailTitleInfo>();
        List<MailTitleInfo> ReadList = new List<MailTitleInfo>();

        for (int i = 0; i < mailList.Count; i++)
        {
            if (mailList[i].status == 0)
            {
                UnReadList.Add(mailList[i]);
            }
            else
            {
                ReadList.Add(mailList[i]);
            }
        }

        UnreadNum = UnReadList.Count;
        mailList = UnReadList;

        for (int i = 0; i < ReadList.Count; i++)
        {
            mailList.Add(ReadList[i]);
        }
    }

    public void SortRewardMailList()
    {
        List<MailTitleInfo> UnReadList = new List<MailTitleInfo>();
        List<MailTitleInfo> ReadList = new List<MailTitleInfo>();

        for (int i = 0; i < rewardMailList.Count; i++)
        {
            if (rewardMailList[i].status == 0)
            {
                UnReadList.Add(rewardMailList[i]);
            }
            else
            {
                ReadList.Add(rewardMailList[i]);
            }
        }

        UnreadRewardNum = UnReadList.Count;
        rewardMailList = UnReadList;

        for (int i = 0; i < ReadList.Count; i++)
        {
            rewardMailList.Add(ReadList[i]);
        }
    }

    public void UpdateOneKeyNum()
    {
        OneKeyDeleteNum = 0;
        OneKeyReceiveNum = 0;

        for (int i = 0; i < mailList.Count; i++)
        {
            if(mailList[i].hasItem == 0)
            {
                OneKeyDeleteNum++;
            }
            else
            {
                OneKeyReceiveNum++;
            }
        }
    }

    public void UpdateOneKeyRewardNum()
    {
        OneKeyDeleteRewardNum = 0;
        OneKeyReceiveRewardNum = 0;

        for (int i = 0; i < rewardMailList.Count; i++)
        {
            if (rewardMailList[i].hasItem == 0)
            {
                OneKeyDeleteRewardNum++;
            }
            else
            {
                OneKeyReceiveRewardNum++;
            }
        }
    }


    public List<UInt64> GetDeleteMailList(List<MailTitleInfo> mailList)
    {
        List<UInt64> DeleteMailList = new List<UInt64>();

        for (int i = 0; i < mailList.Count; i++)
        {
            if (mailList[i].hasItem == 0)
            {
                DeleteMailList.Add(mailList[i].id);
            }
        }

        return DeleteMailList;
    }

}