using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;
using Object = UnityEngine.Object;

namespace GameClient
{
    //通用的助手类
    public static class CommonUtility
    {

        //加载通用的Item。CommonNewItem
        public static CommonNewItem CreateCommonNewItem(GameObject parent)
        {
            if (parent == null)
            {
                Logger.LogErrorFormat("CommonNewItem Create parent is null");
                return null;
            }

            GameObject item = AssetLoader.GetInstance()
                .LoadResAsGameObject("UIFlatten/Prefabs/Common/CommonItem/CommonNewItem");
            if (item == null)
                return null;

            CommonNewItem comNewItem = item.GetComponent<CommonNewItem>();
            if (comNewItem == null)
                return null;

            comNewItem.Reset();
            comNewItem.gameObject.transform.SetParent(parent.transform, false);
            return comNewItem;
        }

        //打开通用的键盘
        public static void OnOpenCommonKeyBoardFrame(Vector3 vector3,
            ulong currentValue = 0,
            ulong maxValue = 0)
        {
            OnCloseCommonKeyBoardFrame();

            var commonKeyBoardDataModel = new CommonKeyBoardDataModel
            {
                Position = vector3,
                CurrentValue = currentValue,
                MaxValue = maxValue,
            };

            ClientSystemManager.GetInstance().OpenFrame<CommonKeyBoardFrame>(FrameLayer.Middle,
                commonKeyBoardDataModel);
        }

        public static string GetItemColorName(ItemTable itemTable)
        {
            if (itemTable == null)
                return "";

            var qualityInfo = ItemData.GetQualityInfo(itemTable.Color, itemTable.Color2 == 1);
            if (qualityInfo == null)
                return itemTable.Name;

            return string.Format("<color={0}>{1}</color>", qualityInfo.ColStr, itemTable.Name);
        }

        //通过Color得到Name的字符串
        public static string GetItemColorNameByNameAndColor(string nameStr, ItemTable.eColor itemTableColor)
        {
            if (string.IsNullOrEmpty(nameStr) == true)
                return "";

            var qualityInfo = ItemData.GetQualityInfo(itemTableColor);
            if (qualityInfo == null)
                return nameStr;

            return string.Format("<color={0}>{1}</color>", qualityInfo.ColStr, nameStr);
        }

        public static string GetPetItemName(PetTable petTable)
        {
            if (petTable == null)
                return "";

            var petNameStr = petTable.Name;
            var petColor = (ItemTable.eColor) petTable.Quality;

            var itemNameStr = GetItemColorNameByNameAndColor(petNameStr, petColor);

            return itemNameStr;
        }

        //关闭通用的键盘
        public static void OnCloseCommonKeyBoardFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<CommonKeyBoardFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<CommonKeyBoardFrame>();
        }

        #region CommonButton

        //更新按钮的状态
        public static void UpdateButtonState(Button button, UIGray buttonGray, bool flag)
        {
            if (button != null)
                button.interactable = flag;

            if (buttonGray != null)
            {
                //uiGray赋值并刷新
                buttonGray.enabled = !flag;
                buttonGray.Refresh();
            }
        }

        //更新按钮的显示
        public static void UpdateButtonVisible(Button button, bool flag)
        {
            if (button != null)
                button.gameObject.SetActive(flag);
        }

        //设置CdButton的Visible，并进行重置
        public static void UpdateButtonWithCdVisibleAndReset(ComButtonWithCd buttonWithCd, bool flag)
        {
            if (buttonWithCd == null)
                return;

            buttonWithCd.Reset();
            UpdateGameObjectVisible(buttonWithCd.gameObject, flag);
        }

        //设置CdButton的Visible
        public static void UpdateButtonWithCdVisible(ComButtonWithCd buttonWithCd, bool flag)
        {
            if (buttonWithCd != null)
                UpdateGameObjectVisible(buttonWithCd.gameObject, flag);
        }

        public static void UpdateGameObjectVisible(GameObject go, bool flag)
        {
            if (go != null)
                go.SetActive(flag);
        }

        //设置为灰色
        public static void UpdateGameObjectUiGray(UIGray uiGray, bool flag)
        {
            if (uiGray != null)
                uiGray.SetEnable(flag);
        }

        public static void UpdateImageVisible(Image image, bool flag)
        {
            if (image != null)
                image.gameObject.CustomActive(flag);
        }

        public static void UpdateTextVisible(Text text, bool flag)
        {
            if (text != null)
                text.gameObject.CustomActive(flag);
        }

        public static void UpdateUIGrayVisible(UIGray uiGray, bool flag)
        {
            if (uiGray != null)
            {
                uiGray.enabled = flag;
                uiGray.Refresh();
            }
        }

        #endregion

        //得到中间的数值
        public static int GetMiddleValue(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        //得到道具超链接的字符串格式
        public static string GetItemNameLinkParseString(int itemId)
        {
            return string.Format(" {{I 0 {0} 0}}", itemId);
        }

        #region Load
        //通过路径加载资源
        public static GameObject LoadGameObjectWithPath(GameObject gameObjectRoot, string goPath)
        {
            if (gameObjectRoot == null)
                return null;

            var uiPrefabWrapper = gameObjectRoot.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper == null)
                return null;

            //设置路径
            uiPrefabWrapper.m_PrefabName = goPath;
            
            //加载Prefab
            var uiPrefabGameObject = uiPrefabWrapper.LoadUIPrefab();
            //设置位置
            if (uiPrefabGameObject != null)
                uiPrefabGameObject.transform.SetParent(gameObjectRoot.transform, false);

            return uiPrefabGameObject;
        }
        
        //加载资源
        public static GameObject LoadGameObject(GameObject gameObjectRoot)
        {
            if (gameObjectRoot == null)
                return null;

            var uiPrefabWrapper = gameObjectRoot.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper == null)
                return null;

            //加载UI
            var uiPrefabGameObject = uiPrefabWrapper.LoadUIPrefab();
            //设置UI的位置
            if (uiPrefabGameObject != null)
                uiPrefabGameObject.transform.SetParent(gameObjectRoot.transform, false);

            return uiPrefabGameObject;
        }

        //设置路径
        public static void SetGameObjectLoadPath(GameObject gameObjectRoot, string loadPath)
        {
            if (gameObjectRoot == null)
                return;

            var uiPrefabWrapper = gameObjectRoot.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper == null)
                return;

            uiPrefabWrapper.m_PrefabName = loadPath;
        }

        #endregion

        #region CommonMsgBox

        public static void OnShowCommonMsgBox(string tipContent, Action onRightAction, string rightButtonText)
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = tipContent,
                IsShowNotify = false,
                LeftButtonText = TR.Value("common_data_cancel"),
                RightButtonText = rightButtonText,
                OnRightButtonClickCallBack = onRightAction,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }

        #endregion

        #region IsSelfPlayer

        //判断playerGuid是否为自己的Guid
        public static bool IsSelfPlayerByPlayerGuid(ulong playerGuid)
        {
            if (playerGuid == 0)
                return false;

            if (playerGuid != PlayerBaseData.GetInstance().RoleID)
                return false;

            return true;
        }

        #endregion


        #region ChildGameObject

        //删除子物体
        public static void RemoveChildGameObject(GameObject root)
        {
            if (root == null || root.transform == null)
                return;

            var rootTf = root.transform;

            if (rootTf.childCount <= 0)
                return;

            for (var i = rootTf.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(rootTf.GetChild(i).gameObject);
            }            
        }

        #endregion

        #region StringParsing

        //将"1_2"这样格式的字符串解析成List数组
        public static List<int> GetNumberListBySplitStringWithLine(string splitStr)
        {
            if (string.IsNullOrEmpty(splitStr) == true)
                return null;

            var numberList = new List<int>();

            var numberStrArray = splitStr.Split('_');
            for (var i = 0; i < numberStrArray.Length; i++)
            {
                var numberStr = numberStrArray[i];
                if (string.IsNullOrEmpty(numberStr) == true)
                    continue;

                int outNumber = 0;
                if (int.TryParse(numberStr, out outNumber) == true)
                    numberList.Add(outNumber);
            }

            return numberList;
        }


        //将"1|2|3"这样格式的字符串解析成List数组
        public static List<int> GetNumberListBySplitString(string splitStr)
        {
            if (string.IsNullOrEmpty(splitStr) == true)
                return null;

            var numberList = new List<int>();

            var numberStrArray = splitStr.Split('|');
            for(var i = 0; i < numberStrArray.Length;i++)
            {
                var numberStr = numberStrArray[i];
                if(string.IsNullOrEmpty(numberStr) == true)
                    continue;

                int outNumber = 0;
                if (int.TryParse(numberStr, out outNumber) == true)
                    numberList.Add(outNumber);
            }

            return numberList;
        }

        //"1000,1,2|10000,2,3"字符串解析
        public static List<ReceiveItemDataModel> GetReceiveItemDataModelBySplitString(string splitStr)
        {
            if (string.IsNullOrEmpty(splitStr) == true)
                return null;

            var receiveItemDataModelList = new List<ReceiveItemDataModel>();

            var receiveItemDataModelArray = splitStr.Split('|');
            for (var i = 0; i < receiveItemDataModelArray.Length; i++)
            {
                var curReceiveItemDataModelStr = receiveItemDataModelArray[i];
                if(string.IsNullOrEmpty(curReceiveItemDataModelStr) == true)
                    continue;

                var curReceiveItemDataModelArray = curReceiveItemDataModelStr.Split(',');
                if(curReceiveItemDataModelArray.Length != 3)
                    continue;

                var receiveItemDataModel = new ReceiveItemDataModel();
                var itemIdStr = curReceiveItemDataModelArray[0];
                var minStr = curReceiveItemDataModelArray[1];
                var maxStr = curReceiveItemDataModelArray[2];

                var itemId = 0;
                if (int.TryParse(itemIdStr, out itemId) == true)
                    receiveItemDataModel.ItemId = itemId;

                int minNumber = 0;
                if (int.TryParse(minStr, out minNumber) == true)
                    receiveItemDataModel.MinNumber = minNumber;

                int maxNumber = 0;
                if (int.TryParse(maxStr, out maxNumber) == true)
                    receiveItemDataModel.MaxNumber = maxNumber;

                receiveItemDataModelList.Add(receiveItemDataModel);

            }

            return receiveItemDataModelList;
        }

        //分割成字符串List
        public static List<string> GetStrListBySplitString(string splitStr)
        {
            if (string.IsNullOrEmpty(splitStr) == true)
                return null;

            var strList = new List<string>();

            var curStrArray = splitStr.Split('|');
            for (var i = 0; i < curStrArray.Length; i++)
            {
                var numberStr = curStrArray[i];
                if (string.IsNullOrEmpty(numberStr) == true)
                    continue;
                strList.Add(numberStr);
            }

            return strList;
        }

        #endregion

        #region ProfessionTable

        //判断职业Id，是否为转职的职业（职业分为基础职业和专职职业)
        public static bool IsProfessionIdIsChangeProfessionId(int professionId)
        {
            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(professionId);
            if (jobTable == null)
                return false;

            //转职类型
            if (jobTable.JobType == 1)
                return true;

            return false;
        }

        //当前角色的基础职业
        public static int GetSelfBaseJobId()
        {
            var selfBaseId = PlayerBaseData.GetInstance().JobTableID;

            var data = TableManager.GetInstance().GetTableItem<JobTable>(selfBaseId);
            if (data == null)
                return selfBaseId;

            if (data.JobType == 1)
                selfBaseId = data.prejob;

            return selfBaseId;
        }

        //得到当前职业的名字，可能是小职业也可能是基础职业
        public static string GetPlayerProfessionName()
        {
            var selfJobId = PlayerBaseData.GetInstance().JobTableID;
            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(selfJobId);

            if (jobTable == null)
                return "";

            return jobTable.Name;
        }

        #endregion

        #region SearchWithBinary

        //二分查找
        public static bool FindInListByBinarySearch(List<ulong> numberList, ulong number)
        {
            if (numberList == null || numberList.Count <= 0)
                return false;

            var beginIndex = 0;
            var endIndex = numberList.Count - 1;
            while (beginIndex <= endIndex)
            {
                int middle = (endIndex - beginIndex) / 2 + beginIndex;
                if (numberList[middle] == number)
                {
                    return true;
                }
                else if (numberList[middle] > number)
                {
                    endIndex = middle - 1;
                }
                else
                {
                    beginIndex = middle + 1;
                }
            }

            return false;
        }

        #endregion

        #region ClientSystemScene

        //是否在吃鸡系统中
        public static bool IsInGameBattleScene()
        {
            var currentClientSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (currentClientSystem == null)
                return false;

            return true;
        }

        //是否在城镇系统中
        public static bool IsInTownScene()
        {
            var currentClientSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (currentClientSystem == null)
                return false;

            return true;
        }

        #endregion

        #region CommonSetContentFrame

        public static void OnOpenCommonSetContentFrame(CommonSetContentDataModel setContentDataModel)
        {
            OnCloseCommonSetContentFrame();

            ClientSystemManager.GetInstance().OpenFrame<CommonSetContentFrame>(FrameLayer.Middle,
                setContentDataModel);
        }

        public static void OnCloseCommonSetContentFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<CommonSetContentFrame>();
        }

        #endregion 


        //加载宠物的模型
        public static void LoadPetAvatarRenderEx(int petId, 
            GeAvatarRendererEx geAvatarRenderEx,
            bool isShowFootSite = true)
        {
            if (geAvatarRenderEx == null)
            {
                Logger.LogError("geAvatarRenderEx is null");
                return;
            }

            var petTable = TableManager.GetInstance().GetTableItem<PetTable>(petId);
            if (petTable == null)
            {
                Logger.LogErrorFormat("can not find PetTable with id:{0}", petId);
                return;
            }

            var resTable = TableManager.GetInstance().GetTableItem<ResTable>(petTable.ModeID);
            if (resTable == null)
            {
                Logger.LogErrorFormat("can not find ResTable with id:{0}", petTable.ModeID);
                return;
            }

            geAvatarRenderEx.ClearAvatar();
            geAvatarRenderEx.LoadAvatar(resTable.ModelPath);

            var backUp = geAvatarRenderEx.avatarPos;
            geAvatarRenderEx.ChangeAction("Anim_Idle01", 1f, true);

            //是否显示宠物下面的地盘，默认显示
            if (isShowFootSite == true)
            {
                geAvatarRenderEx.CreateEffect("Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS",
                    999999, backUp);
            }
        }

    }
}