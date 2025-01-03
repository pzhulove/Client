using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  升级建筑
	/// </summary>
	[AdvancedInspector.Descriptor(" 升级建筑", " 升级建筑")]
	public class WorldGuildUpgradeBuilding : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601927;
		public UInt32 Sequence;
		/// <summary>
		///  建筑类型（对应枚举GuildBuildingType）
		/// </summary>
		[AdvancedInspector.Descriptor(" 建筑类型（对应枚举GuildBuildingType）", " 建筑类型（对应枚举GuildBuildingType）")]
		public byte type;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
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
