using System;
using System.Text;

namespace Mock.Protocol
{

	public class EqRecScoreItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  装备uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备uid", " 装备uid")]
		public UInt64 uid;
		/// <summary>
		///  积分
		/// </summary>
		[AdvancedInspector.Descriptor(" 积分", " 积分")]
		public UInt32 score;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, uid);
			BaseDLL.encode_uint32(buffer, ref pos_, score);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref score);
		}


		#endregion

	}

}
