using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->scene 设置时装武器显示请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->scene 设置时装武器显示请求", " client->scene 设置时装武器显示请求")]
	public class SceneSetFashionWeaponShowReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501027;
		public UInt32 Sequence;
		/// <summary>
		/// 是否显示时装武器(1:是,0:否)
		/// </summary>
		[AdvancedInspector.Descriptor("是否显示时装武器(1:是,0:否)", "是否显示时装武器(1:是,0:否)")]
		public byte isShow;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, isShow);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref isShow);
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
