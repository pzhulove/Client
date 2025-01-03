using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  活动页签信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 活动页签信息", " 活动页签信息")]
	public class ActivityTabInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public string name;

		public UInt32[] actIds = new UInt32[0];

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)actIds.Length);
			for(int i = 0; i < actIds.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, actIds[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			UInt16 actIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref actIdsCnt);
			actIds = new UInt32[actIdsCnt];
			for(int i = 0; i < actIds.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref actIds[i]);
			}
		}


		#endregion

	}

}
