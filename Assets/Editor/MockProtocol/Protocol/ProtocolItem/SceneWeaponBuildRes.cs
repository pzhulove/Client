using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求打造武器返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求打造武器返回", " 请求打造武器返回")]
	public class SceneWeaponBuildRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501101;
		public UInt32 Sequence;

		public UInt32 errorCode;

		public UInt64 guid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
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
