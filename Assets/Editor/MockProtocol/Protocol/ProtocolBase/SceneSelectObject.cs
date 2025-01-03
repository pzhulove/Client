using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  选择角色，拉取信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 选择角色，拉取信息", " 选择角色，拉取信息")]
	public class SceneSelectObject : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500605;
		public UInt32 Sequence;
		/// <summary>
		///  目标ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标ID", " 目标ID")]
		public UInt64 target;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, target);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref target);
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
