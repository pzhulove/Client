using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 通知客户端赛季信息
	/// </summary>
	[AdvancedInspector.Descriptor("通知客户端赛季信息", "通知客户端赛季信息")]
	public class SceneSyncSeasonInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506713;
		public UInt32 Sequence;

		public UInt32 seasonId;

		public UInt32 endTime;

		public UInt32 seasonAttrEndTime;

		public UInt32 seasonAttrLevel;

		public byte seasonAttr;

		public UInt32 seasonLevel;

		public UInt32 seasonStar;

		public UInt32 seasonExp;

		public byte seasonStatus;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, seasonId);
			BaseDLL.encode_uint32(buffer, ref pos_, endTime);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonAttrEndTime);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonAttrLevel);
			BaseDLL.encode_int8(buffer, ref pos_, seasonAttr);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonExp);
			BaseDLL.encode_int8(buffer, ref pos_, seasonStatus);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonAttrEndTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonAttrLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref seasonAttr);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonExp);
			BaseDLL.decode_int8(buffer, ref pos_, ref seasonStatus);
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
