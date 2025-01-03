using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  免战选项
	/// </summary>
	[AdvancedInspector.Descriptor(" 免战选项", " 免战选项")]
	public class SceneBattleNoWarOption : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508942;
		public UInt32 Sequence;

		public UInt64 enemyRoleId;

		public string enemyName;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, enemyRoleId);
			byte[] enemyNameBytes = StringHelper.StringToUTF8Bytes(enemyName);
			BaseDLL.encode_string(buffer, ref pos_, enemyNameBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref enemyRoleId);
			UInt16 enemyNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref enemyNameLen);
			byte[] enemyNameBytes = new byte[enemyNameLen];
			for(int i = 0; i < enemyNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref enemyNameBytes[i]);
			}
			enemyName = StringHelper.BytesToString(enemyNameBytes);
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
