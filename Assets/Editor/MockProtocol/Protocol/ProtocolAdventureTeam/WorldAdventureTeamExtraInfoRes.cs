using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 冒险队扩展信息返回
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 冒险队扩展信息返回", " world->client 冒险队扩展信息返回")]
	public class WorldAdventureTeamExtraInfoRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608610;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误码", " 错误码")]
		public UInt32 resCode;

		public AdventureTeamExtraInfo extraInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, resCode);
			extraInfo.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
			extraInfo.decode(buffer, ref pos_);
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
