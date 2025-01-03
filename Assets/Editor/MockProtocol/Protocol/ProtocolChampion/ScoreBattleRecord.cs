using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 积分赛战斗记录
	/// </summary>
	[AdvancedInspector.Descriptor("积分赛战斗记录", "积分赛战斗记录")]
	public class ScoreBattleRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 roleIDA;

		public string nameA;

		public UInt64 roleIDB;

		public string nameB;
		/// <summary>
		/// 0 是胜 1是平
		/// </summary>
		[AdvancedInspector.Descriptor("0 是胜 1是平", "0 是胜 1是平")]
		public byte result;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleIDA);
			byte[] nameABytes = StringHelper.StringToUTF8Bytes(nameA);
			BaseDLL.encode_string(buffer, ref pos_, nameABytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint64(buffer, ref pos_, roleIDB);
			byte[] nameBBytes = StringHelper.StringToUTF8Bytes(nameB);
			BaseDLL.encode_string(buffer, ref pos_, nameBBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, result);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleIDA);
			UInt16 nameALen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameALen);
			byte[] nameABytes = new byte[nameALen];
			for(int i = 0; i < nameALen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameABytes[i]);
			}
			nameA = StringHelper.BytesToString(nameABytes);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleIDB);
			UInt16 nameBLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameBLen);
			byte[] nameBBytes = new byte[nameBLen];
			for(int i = 0; i < nameBLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBBytes[i]);
			}
			nameB = StringHelper.BytesToString(nameBBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref result);
		}


		#endregion

	}

}
