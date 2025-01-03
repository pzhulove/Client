using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 请求某押注项目中某选项已经压了多少
	/// </summary>
	[AdvancedInspector.Descriptor("请求某押注项目中某选项已经压了多少", "请求某押注项目中某选项已经压了多少")]
	public class SceneChampionGmableAlreadyBetReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509839;
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

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint64(buffer, ref pos_, option);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint64(buffer, ref pos_, ref option);
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
