using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 通知段位信息
	/// </summary>
	[AdvancedInspector.Descriptor("通知段位信息", "通知段位信息")]
	public class SceneSyncSeasonLevel : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506711;
		public UInt32 Sequence;

		public UInt32 oldSeasonLevel;

		public UInt32 oldSeasonStar;

		public UInt32 seasonLevel;

		public UInt32 seasonStar;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonStar);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonStar);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
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
