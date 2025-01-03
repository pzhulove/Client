using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  修改item状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 修改item状态", " 修改item状态")]
	public class SceneItemUpdate : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500628;
		public UInt32 Sequence;

		public UInt32 battleID;
		/// <summary>
		///  item最新信息
		/// </summary>
		[AdvancedInspector.Descriptor(" item最新信息", " item最新信息")]
		public SceneItemInfo data = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			data.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			data.decode(buffer, ref pos_);
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
