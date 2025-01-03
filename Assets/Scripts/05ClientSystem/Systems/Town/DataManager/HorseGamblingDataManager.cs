using System;
using System.Collections.Generic;
using Network;
using Protocol;
using ProtoTable;

namespace GameClient
{

	public class HorseGamblingDataManager : DataManager<HorseGamblingDataManager>, IHorseGablingDataManager
	{
		//地图数据
		public List<IHorseGamblingMapZoneModel> MapZoneModels { get; private set; }

		public BetHorsePhaseType State { get; private set; }

		public int LeftSupply { get; private set; }

		public bool mIsOpen { get; private set; }
		public bool IsOpen {
			get { return mIsOpen && Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.HorseGambling); }
		}

		public uint TimeStamp { get; private set; }

		public BetHorseCfg ConfigData { get; private set; }

		public int UnknownShooterId { get; private set; }

		//天气
		public WeatherType Weather { get; private set; }
		//押注数据
		private readonly List<IHorseGamblingStakeModel> mStakeHistory = new List<IHorseGamblingStakeModel>();
		//比赛记录
		private readonly List<HorseGamblingHistoryModel> mGameHistoryList = new List<HorseGamblingHistoryModel>();
		//射手排行榜
		private readonly List<HorseGamblingRankModel> mShooterRankList = new List<HorseGamblingRankModel>();
		//射手信息
		private readonly Dictionary<int, HorseGamblingShooterModel> mShooterModels = new Dictionary<int, HorseGamblingShooterModel>();


		public override void Initialize()
		{
			mIsOpen = false;
			var configData = TableManager.GetInstance().GetTable<BetHorseCfg>();
			var data = configData.Values.GetEnumerator();
			data.MoveNext();
			ConfigData = (BetHorseCfg)data.Current;

			NetProcess.AddMsgHandler(SyncOpActivityDatas.MsgID, OnSyncActivities);
			NetProcess.AddMsgHandler(SyncOpActivityStateChange.MsgID, OnSyncActivityStateChange);
			NetProcess.AddMsgHandler(BetHorseRes.MsgID, OnDataResponse);
			NetProcess.AddMsgHandler(ShooterOddsRes.MsgID, OnShooterDataResponse);
			NetProcess.AddMsgHandler(ShooterHistoryRecordRes.MsgID, OnShooterHistoryResponse);
			NetProcess.AddMsgHandler(stakeRes.MsgID, OnShooterStakeResponse);
			NetProcess.AddMsgHandler(BetHorsePhaseSycn.MsgID, OnSyncStateChanged);
			NetProcess.AddMsgHandler(BattleRecordRes.MsgID, OnGameHistoryResponse);
			NetProcess.AddMsgHandler(shooterRankRes.MsgID, OnShooterRankListResponse);
			NetProcess.AddMsgHandler(StakeRecordRes.MsgID, OnStakeHistoryResponse);
			NetProcess.AddMsgHandler(WorldMallBuyRet.MsgID, OnBuyBulletRes);
		}

		public override void Clear()
		{
			mIsOpen = false;
			NetProcess.RemoveMsgHandler(BetHorseRes.MsgID, OnDataResponse);
			NetProcess.RemoveMsgHandler(ShooterOddsRes.MsgID, OnShooterDataResponse);
			NetProcess.RemoveMsgHandler(ShooterHistoryRecordRes.MsgID, OnShooterHistoryResponse);
			NetProcess.RemoveMsgHandler(stakeRes.MsgID, OnShooterStakeResponse);
			NetProcess.RemoveMsgHandler(BetHorsePhaseSycn.MsgID, OnSyncStateChanged);
			NetProcess.RemoveMsgHandler(BattleRecordRes.MsgID, OnGameHistoryResponse);
			NetProcess.RemoveMsgHandler(shooterRankRes.MsgID, OnShooterRankListResponse);
			NetProcess.RemoveMsgHandler(StakeRecordRes.MsgID, OnStakeHistoryResponse);
			NetProcess.RemoveMsgHandler(SyncOpActivityDatas.MsgID, OnSyncActivities);
			NetProcess.RemoveMsgHandler(SyncOpActivityStateChange.MsgID, OnSyncActivityStateChange);
			NetProcess.RemoveMsgHandler(WorldMallBuyRet.MsgID, OnBuyBulletRes);
			ClearDatas();

			if (mStakeHistory != null)
			{
				mStakeHistory.Clear();
			}

			if (mGameHistoryList != null)
			{
				mGameHistoryList.Clear();
			}

			if (mShooterRankList != null)
			{
				mShooterRankList.Clear();
			}
		}

		public void RequestData()
		{
			BetHorseReq req = new BetHorseReq();
			NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
		}

		public void RequestShooterOdds()
		{
			ShooterOddsReq req = new ShooterOddsReq();
			NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
		}

		public HorseGamblingShooterModel GetShooterModel(int shooterId)
		{
			if (mShooterModels.ContainsKey(shooterId))
			{
				return mShooterModels[shooterId];
			}

			return null;
		}

		public List<HorseGamblingRankModel> GetShooterRank()
		{
			if (mShooterRankList.Count <= 0)
			{
				RequestShooterRank();
			}
			return mShooterRankList;
		}

		public List<HorseGamblingHistoryModel> GetGameHistory()
		{
			return mGameHistoryList;
		}

		public ShooterRecord[] GetShooterHistory(int shooterId)
		{
			if (mShooterModels.ContainsKey(shooterId) && mShooterModels[shooterId].Records != null && mShooterModels[shooterId].Records.Length > 0)
			{
				return mShooterModels[shooterId].Records;
			}
			else
			{
				RequestShooterHistory((uint)shooterId);
			}

			return null;
		}

		public string GetShooterIconPath(int shooterId)
		{
			var shooterDatas = TableManager.GetInstance().GetTable<BetHorseShooter>();
			if (shooterDatas.ContainsKey(shooterId))
			{
				return ((BetHorseShooter) shooterDatas[shooterId]).IconPath;
			}

			return null;
		}

		public float GetShooterOdds(int shooterId)
		{
			if (mShooterModels.ContainsKey(shooterId))
			{
				return mShooterModels[shooterId].Odds;
			}
			else
			{
				RequestShooterOdds();
			}

			return 0;
		}

		public void ShooterStake(int shooterId, int num)
		{
			stakeReq req = new stakeReq();
			req.id = (uint)shooterId;
			req.num = (uint)num;
			NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
		}

		public void ExchangeBullet(int num)
		{
			if (ConfigData != null)
			{
				WorldMallBuy req = new WorldMallBuy();

				req.itemId = (uint)ConfigData.BulletMallItemId;
				req.num = (UInt16)num;

				NetManager netMgr = NetManager.Instance();
				netMgr.SendCommand(ServerType.GATE_SERVER, req);
			}
		}

		public List<IHorseGamblingStakeModel> GetStakeHistory()
		{
			if (mStakeHistory.Count <= 0)
			{
				RequestStakeHistory();
			}
			return mStakeHistory;
		}

		public void RequestStakeHistory()
		{
			StakeRecordReq req = new StakeRecordReq();
			NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
		}

		public void RequestGameHistory()
		{
			BattleRecordReq req = new BattleRecordReq();
			NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
		}

		void RequestShooterRank()
		{
			shooterRankReq req = new shooterRankReq();
			NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
		}

		void ClearDatas()
		{
			if (MapZoneModels != null)
			{
				MapZoneModels.Clear();
			}

			if (mShooterModels != null)
			{
				mShooterModels.Clear();
			}
		}

		void OnDataResponse(MsgDATA data)
		{
			var msg = new BetHorseRes();
			msg.decode(data.bytes);

			Weather = (WeatherType)msg.weather;
			State = (BetHorsePhaseType)msg.phase;
			TimeStamp = msg.stamp;
			UnknownShooterId = (int)msg.mysteryShooter;
			if (msg.shooterList == null)
			{
				Logger.LogError("赌马射手数据为空，检查服务端传过来的数据");
				return;
			}
			mShooterModels.Clear();
			for (int i = 0; i < msg.shooterList.Length; ++i)
			{
				var id =  msg.shooterList[i].id;
				mShooterModels.Add((int)id, new HorseGamblingShooterModel(msg.shooterList[i], id == msg.mysteryShooter));
				RequestShooterHistory((uint)id);
			}

			MapZoneModels = new List<IHorseGamblingMapZoneModel>(msg.mapList.Length);
			for (int i = 0; i < msg.mapList.Length; ++i)
			{
				MapZoneModels.Add(new HorseGamblingMapZoneModel(msg.mapList[i]));
			}

			//ShooterStakeModels = new Dictionary<int, IHorseGamblingStakeModel>();
			//if (msg.stakeList != null)
			//{
			//	for (int i = 0; i < msg.stakeList.Length; ++i)
			//	{
			//		var stakeModel = new HorseGamblingStakeModel((int)msg.stakeList[i].shooterId, (int)msg.stakeList[i].stakeNum);
			//		ShooterStakeModels.Add((int)msg.stakeList[i].shooterId, stakeModel);
			//	}
			//}
			UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HorseGamblingUpdate);
		}

		void RequestShooterHistory(uint shooterId)
		{
			ShooterHistoryRecordReq req = new ShooterHistoryRecordReq {id = shooterId };
			NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
		}

		void OnShooterDataResponse(MsgDATA data)
		{
			ShooterOddsRes msg = new ShooterOddsRes();
			msg.decode(data.bytes);
			if (msg.oddsList == null)
				return;

			for (int i = 0; i < msg.oddsList.Length; ++i)
			{
				if (mShooterModels.ContainsKey((int)msg.oddsList[i].id))
				{
					mShooterModels[(int)msg.oddsList[i].id].UpdateOdds((int)msg.oddsList[i].odds);
				}
			}

			UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HorseGamblingOddsUpdate);
		}
		void OnShooterHistoryResponse(MsgDATA data)
		{
			ShooterHistoryRecordRes msg = new ShooterHistoryRecordRes();
			msg.decode(data.bytes);

			if (mShooterModels.ContainsKey((int) msg.id))
			{
				mShooterModels[(int)msg.id].UpdateRecords(msg.recordList);
				UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HorseGamblingShooterHistoryUpdate, mShooterModels[(int)msg.id]);
			}
		}

		void OnShooterStakeResponse(MsgDATA data)
		{
			stakeRes msg = new stakeRes();
			msg.decode(data.bytes);
			if (msg.ret == 0)
				LeftSupply -= (int)msg.num;
			RequestStakeHistory();
			UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HorseGamblingShooterStakeResp, (int)msg.ret);
		}

		void OnSyncStateChanged(MsgDATA data)
		{
			BetHorsePhaseSycn msg = new BetHorsePhaseSycn();
			msg.decode(data.bytes);

			switch ((BetHorsePhaseType)msg.phaseSycn)
			{
				case BetHorsePhaseType.PHASE_TYPE_STAKE:
					ClearDatas();
					break;
				case BetHorsePhaseType.PHASE_TYPE_ADJUST:
					break;
				case BetHorsePhaseType.PHASE_TYPE_RESULT:
					RequestGameHistory();
					RequestStakeHistory();
					RequestShooterRank();
					break;
				case BetHorsePhaseType.PHASE_TYPE_DAY_END:
					break;
			}
			TimeStamp = msg.stamp;
			State = (BetHorsePhaseType) msg.phaseSycn;
			UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HorseGamblingGameStateUpdate, (BetHorsePhaseType)msg.phaseSycn);
		}

		void OnGameHistoryResponse(MsgDATA data)
		{
			BattleRecordRes msg = new BattleRecordRes();
			msg.decode(data.bytes);

			if (msg.BattleRecordList != null)
			{
				mGameHistoryList.Clear();
				for (int i = msg.BattleRecordList.Length - 1; i >= 0; --i)
				{
					mGameHistoryList.Add(new HorseGamblingHistoryModel(msg.BattleRecordList[i]));
				}
				UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HorseGamblingGameHistoryUpdate, mGameHistoryList);
			}
		}

		void OnShooterRankListResponse(MsgDATA data)
		{
			shooterRankRes msg = new shooterRankRes();
			msg.decode(data.bytes);

			if (msg.shooterRankList != null)
			{
				mShooterRankList.Clear();
				for (int i = msg.shooterRankList.Length - 1; i >= 0; --i)
				{
					var rankModel = new HorseGamblingRankModel(msg.shooterRankList[i]);
					int j = 0;
					for (j = 0; j < mShooterRankList.Count; ++j)
					{
						if (mShooterRankList[j].WinRate <= rankModel.WinRate)
						{
							mShooterRankList.Insert(j, rankModel);
							break;
						}
					}
					if (j == mShooterRankList.Count)
					{
						mShooterRankList.Add(rankModel);
					}
				}
				UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HorseGamblingRankListUpdate, mShooterRankList);
			}
		}

		void OnStakeHistoryResponse(MsgDATA data)
		{
			StakeRecordRes msg = new StakeRecordRes();
			msg.decode(data.bytes);
			var configData = TableManager.GetInstance().GetTable<BetHorseCfg>();
			var enumerator = configData.Values.GetEnumerator();
			enumerator.MoveNext();
			var firstData = (BetHorseCfg)enumerator.Current;
			LeftSupply = firstData.StakeMax;
			if (msg.StakeRecordList != null)
			{
				mStakeHistory.Clear();

				for (int i = 0; i < msg.StakeRecordList.Length; ++i)
				{
					mStakeHistory.Add(new HorseGamblingStakeModel(msg.StakeRecordList[i]));
					if (msg.StakeRecordList[i].profit == -1)
					{
						if (firstData != null)
						{
							LeftSupply -= (int)msg.StakeRecordList[i].stakeNum;
						}
					}
				}
			}


			UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HorseGamblingShooterStakeUpdate, mStakeHistory);
		}

		void OnSyncActivities(MsgDATA data)
		{
			var resp = new SyncOpActivityDatas();
			resp.decode(data.bytes);
			for (var i = 0; i < resp.datas.Length; ++i)
			{
				if (resp.datas[i].tmpType == (int)OpActivityTmpType.OAT_BET_HORSE)
				{
					OnStateChange((OpActivityState)resp.datas[i].state);
					break;
				}
			}
		}

		void OnSyncActivityStateChange(MsgDATA data)
		{
			var resp = new SyncOpActivityStateChange();
			resp.decode(data.bytes);
			if (resp.data.tmpType == (int)OpActivityTmpType.OAT_BET_HORSE)
			{
				OnStateChange((OpActivityState)resp.data.state);
			}
		}

		void OnStateChange(OpActivityState state)
		{
			if (state == OpActivityState.OAS_IN)
			{
				mIsOpen = true;
			}
			else
			{
				mIsOpen = false;
			}
			UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HorseGamblingStateUpdate);
		}

		void OnBuyBulletRes(MsgDATA data)
		{
			WorldMallBuyRet resp = new WorldMallBuyRet();
			resp.decode(data.bytes);
			var configData = TableManager.GetInstance().GetTable<BetHorseCfg>();
			var enumerator = configData.Values.GetEnumerator();
			enumerator.MoveNext();
			var firstData = (BetHorseCfg)enumerator.Current;
			if (firstData != null)
			{
				if (resp.mallitemid == firstData.BulletMallItemId)
				{
					UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HorseGamblingBuyBulletResponse, (int)resp.code);
				}
			}
		}
	}
}