using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Clinet->Scene 冠军赛押注
	/// </summary>
	[AdvancedInspector.Descriptor("Clinet->Scene 冠军赛押注", "Clinet->Scene 冠军赛押注")]
	public class SceneChampionGambleReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509811;
		public UInt32 Sequence;
		/// <summary>
		/// 角色id，客户端不用填
		/// </summary>
		[AdvancedInspector.Descriptor("角色id，客户端不用填", "角色id，客户端不用填")]
		public UInt64 roleID;
		/// <summary>
		/// 押注的id
		/// </summary>
		[AdvancedInspector.Descriptor("押注的id", "押注的id")]
		public UInt32 id;
		/// <summary>
		/// 押注的选项
		/// </summary>
		[AdvancedInspector.Descriptor("押注的选项", "押注的选项")]
		public UInt64 option;
		/// <summary>
		/// 押注的货币数量
		/// </summary>
		[AdvancedInspector.Descriptor("押注的货币数量", "押注的货币数量")]
		public UInt32 coin;
		/// <summary>
		/// 帐号id，客户端不用填
		/// </summary>
		[AdvancedInspector.Descriptor("帐号id，客户端不用填", "帐号id，客户端不用填")]
		public UInt32 accid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleID);
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint64(buffer, ref pos_, option);
			BaseDLL.encode_uint32(buffer, ref pos_, coin);
			BaseDLL.encode_uint32(buffer, ref pos_, accid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint64(buffer, ref pos_, ref option);
			BaseDLL.decode_uint32(buffer, ref pos_, ref coin);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
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
