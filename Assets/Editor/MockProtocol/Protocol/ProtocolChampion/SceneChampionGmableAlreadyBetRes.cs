using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 返回某押注项目中某选项已经压了多少
	/// </summary>
	[AdvancedInspector.Descriptor("返回某押注项目中某选项已经压了多少", "返回某押注项目中某选项已经压了多少")]
	public class SceneChampionGmableAlreadyBetRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509840;
		public UInt32 Sequence;
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
		/// 已经压了多少
		/// </summary>
		[AdvancedInspector.Descriptor("已经压了多少", "已经压了多少")]
		public UInt64 num;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint64(buffer, ref pos_, option);
			BaseDLL.encode_uint64(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint64(buffer, ref pos_, ref option);
			BaseDLL.decode_uint64(buffer, ref pos_, ref num);
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
