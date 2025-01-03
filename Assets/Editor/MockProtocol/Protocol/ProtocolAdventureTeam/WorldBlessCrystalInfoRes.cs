using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 赐福水晶信息返回
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 赐福水晶信息返回", " world->client 赐福水晶信息返回")]
	public class WorldBlessCrystalInfoRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608604;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误码", " 错误码")]
		public UInt32 resCode;
		/// <summary>
		///  拥有的赐福水晶数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 拥有的赐福水晶数量", " 拥有的赐福水晶数量")]
		public UInt32 ownBlessCrystalNum;
		/// <summary>
		///  拥有的赐福水晶经验
		/// </summary>
		[AdvancedInspector.Descriptor(" 拥有的赐福水晶经验", " 拥有的赐福水晶经验")]
		public UInt64 ownBlessCrystalExp;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, resCode);
			BaseDLL.encode_uint32(buffer, ref pos_, ownBlessCrystalNum);
			BaseDLL.encode_uint64(buffer, ref pos_, ownBlessCrystalExp);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
			BaseDLL.decode_uint32(buffer, ref pos_, ref ownBlessCrystalNum);
			BaseDLL.decode_uint64(buffer, ref pos_, ref ownBlessCrystalExp);
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
