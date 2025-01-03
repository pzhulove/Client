using System.Collections.Generic;
using Protocol;
using ProtoTable;
using Scripts.UI;

namespace GameClient
{
	public class HorseGamblingRecordFrameParam
	{
		public EHorseGamblingRecord RecordType;
		public int Param = -1;
	}

	public class HorseGamblingRecordFrame : ClientFrame
	{
		private EHorseGamblingRecord mRecordType;
		private HorseGamblingRecordView mView;
		private int mParam;//在选手战绩界面是射手的id,在比赛历史和选手排名界面 0表示打开比赛历史，1表示打开选手排名
		private int mSelectId;
		public override string GetPrefabPath()
		{
			return "UIFlatten/Prefabs/HorseGambling/HorseGamblingRecordFrame";
		}

		protected override void _OnOpenFrame()
		{
			var param = (HorseGamblingRecordFrameParam) userData;
			if (param != null)
			{
				mRecordType = param.RecordType;
				mParam = param.Param;
			}
			mView = frame.GetComponent<HorseGamblingRecordView>();
			BindEvents();
			if (mView != null)
			{
				List<string> toggleNames = new List<string>();
				int defaultSelectId = 0;
				switch (mRecordType)
				{
					case EHorseGamblingRecord.Stake:
						toggleNames.Add(mView.StakeToggleName);
						break;
					case EHorseGamblingRecord.HistoryAndRank:
						defaultSelectId = mParam;
						toggleNames.Add(mView.GameRecordToggleName);
						toggleNames.Add(mView.ShooterRankToggleName);
						break;
					case EHorseGamblingRecord.ShooterHistory:
						toggleNames.Add(mView.ShooterRecordToggleName);
						break;
				}
				mView.Init(toggleNames.ToArray(), OnSelectTab, OnItemVisible, OnItemVisible, OnClose, defaultSelectId);
			}
		}

		protected override void _OnCloseFrame()
		{
			if (mView != null)
			{
				mView.Dispose();
				mView = null;
			}

			UnBindEvents();
		}

		void OnClose()
		{
			Close();
		}

		void OnItemVisible(ComUIListElementScript item)
		{
			switch (mRecordType)
			{
				case EHorseGamblingRecord.Stake:
					InitStakeItem(item);
					break;
				case EHorseGamblingRecord.HistoryAndRank:
					if (mSelectId == 0)
					{
						InitGameHistoryItem(item);
					}
					else
					{
						InitShooterRankItem(item);
					}
					break;
				case EHorseGamblingRecord.ShooterHistory:
					InitShooterHistoryItem(item);
					break;
			}
		}

		void InitStakeItem(ComUIListElementScript item)
		{
			var script = item.GetComponent<HorseGamblingStakeRecordItem>();
			var datas = HorseGamblingDataManager.GetInstance().GetStakeHistory();
			if (mView != null && script != null && item.m_index < datas.Count)
			{
				var data = datas[item.m_index];
				string name = data.ShooterName;
				string iconPath = HorseGamblingDataManager.GetInstance().GetShooterIconPath(data.ShooterId);
				//判断是否是当前场神秘射手 
				if (HorseGamblingDataManager.GetInstance().State == BetHorsePhaseType.PHASE_TYPE_STAKE && data.ShooterId == HorseGamblingDataManager.GetInstance().UnknownShooterId && data.Profit == -1)
				{
					var tableData = TableManager.GetInstance().GetTableItem<BetHorseShooter>(0);
					if (tableData != null)
					{
						name = tableData.Name;
						iconPath = tableData.IconPath;
					}
				}

				script.Init(data.GameId.ToString(), name, data.Odds.ToString(), data.Stake.ToString(), data.Profit, iconPath, item.m_index % 2 == 0 ? mView.ItemBg2 : mView.ItemBg1);
			}
		}

		void InitGameHistoryItem(ComUIListElementScript item)
		{
			var script = item.GetComponent<HorseGamblingGameRecordItem>();
			var datas = HorseGamblingDataManager.GetInstance().GetGameHistory();
			if (mView != null && script != null && item.m_index < datas.Count)
			{
				var data = datas[item.m_index];
				script.Init(data.GameId.ToString(), data.ChampionName, data.Odds.ToString(), data.MaxProfit.ToString(), HorseGamblingDataManager.GetInstance().GetShooterIconPath(data.ShooterId), item.m_index % 2 == 0 ? mView.ItemBg2 : mView.ItemBg1);
			}
		}
		void InitShooterRankItem(ComUIListElementScript item)
		{
			var script = item.GetComponent<HorseGamblingGameRecordItem>();
			var datas = HorseGamblingDataManager.GetInstance().GetShooterRank();
			if (mView != null && script != null && item.m_index < datas.Count)
			{
				var data = datas[item.m_index];
				script.Init((item.m_index + 1).ToString(), data.ShooterName, data.BattleNum.ToString(), string.Format(TR.Value("horse_gambling_shooter_win_rate"), data.WinRate), HorseGamblingDataManager.GetInstance().GetShooterIconPath(data.ShooterId), item.m_index % 2 == 0 ? mView.ItemBg2 : mView.ItemBg1);
			}
		}
		void InitShooterHistoryItem(ComUIListElementScript item)
		{
			var script = item.GetComponent<HorseGamblingShooterHistoryItem>();
			var datas = HorseGamblingDataManager.GetInstance().GetShooterHistory(mParam);
			if (mView != null && script != null && item.m_index < datas.Length)
			{
				var shooterData = TableManager.GetInstance().GetTableItem<BetHorseShooter>(mParam);
				if (shooterData == null)
					return;
				var index = datas.Length - 1 - item.m_index;
				if (index >= 0 && index < datas.Length)
				{
					var data = datas[index];
					script.Init(data.coutrId.ToString(), shooterData.Name, ((float)data.odds / 10000).ToString(), HorseGamblingDataManager.GetInstance().GetShooterIconPath(mParam), data.result == 1, item.m_index % 2 == 0 ? mView.ItemBg2 : mView.ItemBg1);
				}
			}
		}
		void OnSelectTab(bool value, int id)
		{
			if (value)
			{
				switch (mRecordType)
				{
					case EHorseGamblingRecord.Stake:
						if (mView != null)
						{
							mView.SetData(mView.StakeToggleName, mView.StakeTitleNames, HorseGamblingDataManager.GetInstance().GetStakeHistory().Count, mView.StakeItemPrefab);
						}
						break;
					case EHorseGamblingRecord.HistoryAndRank:
						if (mView != null)
						{
							mSelectId = id;
							int count = HorseGamblingDataManager.GetInstance().GetGameHistory().Count;
							string showName = mView.GameRecordToggleName;
							string itemPrefab = mView.GameRecordItemPrefab;
							List<string> titleNames = mView.GameRecordTitleNames;
							if (id == 1)
							{
								count = HorseGamblingDataManager.GetInstance().GetShooterRank().Count;
								showName = mView.ShooterRankToggleName;
								itemPrefab = mView.ShooterRankItemPrefab;
								titleNames = mView.ShooterRankTitleNames;
							}
							mView.SetData(showName, titleNames, count, itemPrefab);
						}
						break;
					case EHorseGamblingRecord.ShooterHistory:
						if (mView != null)
						{
							var shootData = HorseGamblingDataManager.GetInstance().GetShooterHistory(mParam);
							int count = 0;
							if (shootData != null)
							{
								count = shootData.Length;
							}
							mView.SetData(mView.ShooterRecordToggleName, mView.ShooterRecordTitleNames, count, mView.ShooterRecordItemPrefab);
						}
						break;
				}
			}
		}

		void BindEvents()
		{
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HorseGamblingShooterStakeUpdate, OnStakeHistoryUpdate);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HorseGamblingGameHistoryUpdate, OnGameHistoryUpdate);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HorseGamblingRankListUpdate, OnRankListUpdate);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HorseGamblingShooterHistoryUpdate, OnShooterHistoryUpdate);
		}

		void UnBindEvents()
		{
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HorseGamblingShooterStakeUpdate, OnStakeHistoryUpdate);
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HorseGamblingGameHistoryUpdate, OnGameHistoryUpdate);
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HorseGamblingRankListUpdate, OnRankListUpdate);
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HorseGamblingShooterHistoryUpdate, OnShooterHistoryUpdate);
		}
		void OnGameHistoryUpdate(UIEvent data)
		{
			if (mRecordType == EHorseGamblingRecord.HistoryAndRank && mSelectId == 0)
			{
				if (mView != null && data != null && data.Param1 != null)
				{
					List<HorseGamblingHistoryModel> list = (List<HorseGamblingHistoryModel>) data.Param1;
					if (list != null)
						mView.UpdateData(list.Count);
				}
			}
		}
		void OnRankListUpdate(UIEvent data)
		{
			if (mRecordType == EHorseGamblingRecord.HistoryAndRank && mSelectId == 1)
			{
				if (mView != null && data != null && data.Param1 != null)
				{
					List<HorseGamblingRankModel> list = (List<HorseGamblingRankModel>)data.Param1;
					if (list != null)
						mView.UpdateData(list.Count);
				}
			}
		}

		void OnStakeHistoryUpdate(UIEvent data)
		{
			if (mRecordType == EHorseGamblingRecord.Stake)
			{
				if (mView != null && data != null && data.Param1 != null)
				{
					List<IHorseGamblingStakeModel> list = (List<IHorseGamblingStakeModel>)data.Param1;
					if (list != null)
						mView.UpdateData(list.Count);
				}
			}
		}

		void OnShooterHistoryUpdate(UIEvent data)
		{
			if (mRecordType == EHorseGamblingRecord.ShooterHistory)
			{
				if (mView != null && data != null && data.Param1 != null)
				{
					IHorseGamblingShooterModel model = (IHorseGamblingShooterModel) data.Param1;
					if (model != null && model.Records != null)
					{
						mView.UpdateData(model.Records.Length);
					}
				}
			}
		}
	}
}