using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->server 装备回收积分提升请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->server 装备回收积分提升请求", "client->server 装备回收积分提升请求")]
	public class SceneEquipRecUpscoreReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501012;
		public UInt32 Sequence;
		/// <summary>
		/// 装备uid
		/// </summary>
		[AdvancedInspector.Descriptor("装备uid", "装备uid")]
		public UInt64 equid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, equid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref equid);
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
