using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// battleScene->client  敌人是否在地下城
	/// </summary>
	[AdvancedInspector.Descriptor("battleScene->client  敌人是否在地下城", "battleScene->client  敌人是否在地下城")]
	public class SceneLostDungeonEnemyInDungeon : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510032;
		public UInt32 Sequence;
		/// <summary>
		///  敌方id
		/// </summary>
		[AdvancedInspector.Descriptor(" 敌方id", " 敌方id")]
		public UInt64 enemyId;
		/// <summary>
		///  地下城id
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城id", " 地下城id")]
		public UInt32 dungeonId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, enemyId);
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref enemyId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
