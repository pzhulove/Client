using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  红包领取记录
	/// </summary>
	[AdvancedInspector.Descriptor(" 红包领取记录", " 红包领取记录")]
	public class RedPacketRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一id
		/// </summary>
		[AdvancedInspector.Descriptor(" 唯一id", " 唯一id")]
		public UInt64 guid;
		/// <summary>
		///  时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 时间", " 时间")]
		public UInt32 time;
		/// <summary>
		///  红包发出者名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包发出者名字", " 红包发出者名字")]
		public string redPackOwnerName;
		/// <summary>
		///  货币id
		/// </summary>
		[AdvancedInspector.Descriptor(" 货币id", " 货币id")]
		public UInt32 moneyId;
		/// <summary>
		///  货币数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 货币数量", " 货币数量")]
		public UInt32 moneyNum;
		/// <summary>
		///  是否最佳红包
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否最佳红包", " 是否最佳红包")]
		public byte isBest;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_uint32(buffer, ref pos_, time);
			byte[] redPackOwnerNameBytes = StringHelper.StringToUTF8Bytes(redPackOwnerName);
			BaseDLL.encode_string(buffer, ref pos_, redPackOwnerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, moneyId);
			BaseDLL.encode_uint32(buffer, ref pos_, moneyNum);
			BaseDLL.encode_int8(buffer, ref pos_, isBest);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			UInt16 redPackOwnerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref redPackOwnerNameLen);
			byte[] redPackOwnerNameBytes = new byte[redPackOwnerNameLen];
			for(int i = 0; i < redPackOwnerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref redPackOwnerNameBytes[i]);
			}
			redPackOwnerName = StringHelper.BytesToString(redPackOwnerNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref moneyId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref moneyNum);
			BaseDLL.decode_int8(buffer, ref pos_, ref isBest);
		}


		#endregion

	}

}
