using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  活动怪物信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 活动怪物信息", " 活动怪物信息")]
	public class ActivityMonsterInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public string name;

		public byte activity;

		public UInt32 id;
		/// <summary>
		///  刷怪点类型(DEntityType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 刷怪点类型(DEntityType)", " 刷怪点类型(DEntityType)")]
		public byte pointType;

		public UInt32 startTime;

		public UInt32 endTime;

		public UInt32 remainNum;

		public UInt32 nextRollStartTime;

		public DropItem[] drops = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, activity);
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, pointType);
			BaseDLL.encode_uint32(buffer, ref pos_, startTime);
			BaseDLL.encode_uint32(buffer, ref pos_, endTime);
			BaseDLL.encode_uint32(buffer, ref pos_, remainNum);
			BaseDLL.encode_uint32(buffer, ref pos_, nextRollStartTime);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)drops.Length);
			for(int i = 0; i < drops.Length; i++)
			{
				drops[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref activity);
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref pointType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref remainNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref nextRollStartTime);
			UInt16 dropsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dropsCnt);
			drops = new DropItem[dropsCnt];
			for(int i = 0; i < drops.Length; i++)
			{
				drops[i] = new DropItem();
				drops[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
