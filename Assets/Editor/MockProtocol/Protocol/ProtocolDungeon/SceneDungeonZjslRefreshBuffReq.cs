using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  终极试炼地下城刷新BUFF请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 终极试炼地下城刷新BUFF请求", " 终极试炼地下城刷新BUFF请求")]
	public class SceneDungeonZjslRefreshBuffReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506835;
		public UInt32 Sequence;
		/// <summary>
		///  地下城ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城ID", " 地下城ID")]
		public UInt32 dungeonId;
		/// <summary>
		///  使用刷新票
		/// </summary>
		[AdvancedInspector.Descriptor(" 使用刷新票", " 使用刷新票")]
		public UInt32 useItem;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			BaseDLL.encode_uint32(buffer, ref pos_, useItem);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref useItem);
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
