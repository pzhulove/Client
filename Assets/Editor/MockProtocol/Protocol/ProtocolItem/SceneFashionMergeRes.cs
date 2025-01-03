using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneFashionMergeRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500953;
		public UInt32 Sequence;

		public Int32 result;

		public byte resultType;

		public UInt32 itemA;

		public Int32 numA;

		public UInt32 itemB;

		public Int32 numB;

		public UInt32 itemC;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int32(buffer, ref pos_, result);
			BaseDLL.encode_int8(buffer, ref pos_, resultType);
			BaseDLL.encode_uint32(buffer, ref pos_, itemA);
			BaseDLL.encode_int32(buffer, ref pos_, numA);
			BaseDLL.encode_uint32(buffer, ref pos_, itemB);
			BaseDLL.encode_int32(buffer, ref pos_, numB);
			BaseDLL.encode_uint32(buffer, ref pos_, itemC);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int32(buffer, ref pos_, ref result);
			BaseDLL.decode_int8(buffer, ref pos_, ref resultType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemA);
			BaseDLL.decode_int32(buffer, ref pos_, ref numA);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemB);
			BaseDLL.decode_int32(buffer, ref pos_, ref numB);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemC);
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
