using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 同步毒圈
	/// </summary>
	[AdvancedInspector.Descriptor("同步毒圈", "同步毒圈")]
	public class SceneBattleSyncPoisonRing : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508910;
		public UInt32 Sequence;

		public UInt32 battleID;

		public UInt32 x;

		public UInt32 y;

		public UInt32 radius;

		public UInt32 beginTimestamp;

		public UInt32 interval;

		public UInt32 x1;

		public UInt32 y1;

		public UInt32 radius1;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			BaseDLL.encode_uint32(buffer, ref pos_, x);
			BaseDLL.encode_uint32(buffer, ref pos_, y);
			BaseDLL.encode_uint32(buffer, ref pos_, radius);
			BaseDLL.encode_uint32(buffer, ref pos_, beginTimestamp);
			BaseDLL.encode_uint32(buffer, ref pos_, interval);
			BaseDLL.encode_uint32(buffer, ref pos_, x1);
			BaseDLL.encode_uint32(buffer, ref pos_, y1);
			BaseDLL.encode_uint32(buffer, ref pos_, radius1);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref x);
			BaseDLL.decode_uint32(buffer, ref pos_, ref y);
			BaseDLL.decode_uint32(buffer, ref pos_, ref radius);
			BaseDLL.decode_uint32(buffer, ref pos_, ref beginTimestamp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref interval);
			BaseDLL.decode_uint32(buffer, ref pos_, ref x1);
			BaseDLL.decode_uint32(buffer, ref pos_, ref y1);
			BaseDLL.decode_uint32(buffer, ref pos_, ref radius1);
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
