using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  玩家在线状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 玩家在线状态", " 玩家在线状态")]
	public class PlayerOnline : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 uid;

		public byte online;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, uid);
			BaseDLL.encode_int8(buffer, ref pos_, online);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			BaseDLL.decode_int8(buffer, ref pos_, ref online);
		}


		#endregion

	}

}
