using System.Collections.Generic;
///////删除linq
using Protocol;
using ProtoTable;

namespace GameClient
{
	public class HorseGamblingMapShooterModel : IHorseGamblingMapShooterModel
	{
		public int Id { get; private set; }
		public string Name { get; private set; }
		public bool IsShowOdds { get; private set; }

		//public string IconPath { get; private set; }
		public EHorseGamblingBattleResult BattleState { get; private set; }

		public HorseGamblingMapShooterModel(int id, string name, bool isShowOdds, EHorseGamblingBattleResult battleState)
		{
			Id = id;
			IsShowOdds = isShowOdds;
			BattleState = battleState;
			Name = name;
		}
	}

	public class HorseGamblingShooterModel : IHorseGamblingShooterModel
	{
		public int Id { get; private set; }
		public string IconPath { get; private set; }
		public string PortraitPath { get; private set; }
		public string Name { get; private set; }
		public string Occupation { get; private set; }
		public string OccupationIcon { get; private set; }
		public string Terrain { get; private set; }
		public string Weather { get; private set; }
		public ShooterStatusType Status { get; private set; }
		public float WinRate { get; private set; }
		public int ChampionCount { get; private set; }

		public float Odds { get; private set; }

		public bool IsUnknown { get; private set; }

		public ShooterRecord[] Records { get; private set; }

		private const int INT_TO_FLOAT_RATE = 10000;
		private const int WIN_RATE_TO_PERCENT = 100;
		public HorseGamblingShooterModel(ShooterInfo msg, bool isUnknown)
		{
			Id = (int)msg.id;
			var tableData = TableManager.GetInstance().GetTableItem<BetHorseShooter>((int)msg.dataid);

			if (tableData == null)
			{
				Logger.LogError(string.Format("赌马射手表中找不到id为{0}的数据", msg.dataid));
			}
			else
			{
				IconPath = tableData.IconPath;
				Name = tableData.Name;
				PortraitPath = tableData.PortraitPath;
				Occupation = tableData.Occupation;
				Terrain = tableData.Terrain;
				Weather = tableData.Weather;
				OccupationIcon = tableData.OccupationIcon;
			}

			IsUnknown = isUnknown;
			WinRate = (float)msg.winRate / WIN_RATE_TO_PERCENT;
			ChampionCount = (int) (msg.champion);
			Odds = (float) msg.odds / INT_TO_FLOAT_RATE;
			Status = (ShooterStatusType)msg.status;
		}

		public void UpdateOdds(int odds)
		{
			Odds = (float)odds / INT_TO_FLOAT_RATE;
		}

		public void UpdateRecords(ShooterRecord[] records)
		{
			Records = records;
		}
	}


	public class HorseGamblingMapZoneModel : IHorseGamblingMapZoneModel
	{
		public int Id { get; private set; }
		public string TerrainPath { get; private set; }
		public Dictionary<int, HorseGamblingMapShooterModel> Shooters { get; private set; }

		public int Phase { get; private set; }

		public HorseGamblingMapZoneModel(MapInfo msg)
		{
			Id = (int)msg.id;
			var tableData = TableManager.GetInstance().GetTableItem<BetHorseMap>(Id);
			if (tableData == null)
			{
				return;
			}
			var terrainPaths = tableData.TerrainPaths;
			if (msg.terrain <= terrainPaths.Count)
			{
				TerrainPath = terrainPaths[(int)msg.terrain - 1];
			}

			Phase = tableData.MapPhase;
			if (msg.shooter != null)
			{
				Shooters = new Dictionary<int, HorseGamblingMapShooterModel>(msg.shooter.Length);
				for (int i = 0; i < msg.shooter.Length; ++i)
				{
					var battleState = EHorseGamblingBattleResult.NotBattle;
					if (msg.winShooterId != 0)
					{
						battleState = msg.winShooterId == msg.shooter[i] ? EHorseGamblingBattleResult.Win : EHorseGamblingBattleResult.Lose;
					}
					var shooterData = HorseGamblingDataManager.GetInstance().GetShooterModel((int)msg.shooter[i]);
					if (shooterData != null)
					{
						Shooters.Add((int)msg.shooter[i], new HorseGamblingMapShooterModel((int)msg.shooter[i], shooterData.Name, tableData.MapPhase == 1, battleState));
					}
				}
			}
			else
			{
				Shooters = new Dictionary<int, HorseGamblingMapShooterModel>();
			}
		}

		public void Update(MapInfo msg)
		{
			if (msg.shooter != null)
			{
				for (int i = 0; i < msg.shooter.Length; ++i)
				{
					var battleState = EHorseGamblingBattleResult.NotBattle;
					if (msg.winShooterId != 0)
					{
						battleState = msg.winShooterId == msg.shooter[i] ? EHorseGamblingBattleResult.Win : EHorseGamblingBattleResult.Lose;
					}
					var shooterData = HorseGamblingDataManager.GetInstance().GetShooterModel((int)msg.shooter[i]);
					if (shooterData != null)
					{
						Shooters[(int)msg.shooter[i]] = new HorseGamblingMapShooterModel((int)msg.shooter[i], shooterData.Name, Phase == 1, battleState);
					}

				}
			}
		}
	}

	public class HorseGamblingStakeModel : IHorseGamblingStakeModel
	{
		public int ShooterId { get; private set; }
		public string ShooterName { get; private set; }
		public int Stake { get; private set; }
		public int GameId { get; private set; }
		public int Profit { get; private set; }
		public float Odds { get; private set; }

		public HorseGamblingStakeModel(StakeRecord msg)
		{
			ShooterId = (int)msg.stakeShooter;
			var tableData = TableManager.GetInstance().GetTableItem<BetHorseShooter>(ShooterId);
			if (tableData != null)
			{
				ShooterName = tableData.Name;
			}

			Stake = (int)msg.stakeNum;
			GameId = (int) msg.courtId;
			Profit = (int) msg.profit;
			Odds = (float) msg.odds / 10000;
		}

		public void UpdateStake(int num)
		{
			Stake = num;
		}
	}

	public struct HorseGamblingHistoryModel
	{
		public int GameId { get; private set; }

		public string ChampionName { get; private set; }
		public float Odds { get; private set; }
		public int MaxProfit { get; private set; }
		public int ShooterId { get; private set; }

		public HorseGamblingHistoryModel(BattleRecord msg) : this()
		{
			GameId = (int)msg.courtId;
			var shooterData = TableManager.GetInstance().GetTableItem<BetHorseShooter>((int)msg.champion);
			if (shooterData != null)
			{
				ChampionName = shooterData.Name;
			}

			ShooterId = (int) msg.champion;
			Odds = (float)msg.odds / 10000;
			MaxProfit = (int) msg.maxProfit;
		}
	}

	public struct HorseGamblingRankModel
	{
		public string ShooterName { get; private set; }

		public uint BattleNum { get; private set; }

		public float WinRate { get; private set; }
		public int ShooterId { get; private set; }

		private const int WIN_RATE_TO_PERCENT = 100;

		public HorseGamblingRankModel(shooterRankInfo msg) : this()
		{
			var shooterData = TableManager.GetInstance().GetTableItem<BetHorseShooter>((int)msg.shooterId);
			if (shooterData != null)
			{
				ShooterName = shooterData.Name;
			}
			ShooterId = (int)msg.shooterId;

			BattleNum = msg.battleNum;
			WinRate = (float)msg.winRate / WIN_RATE_TO_PERCENT;
		}
	}
}