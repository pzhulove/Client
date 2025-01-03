using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  新手引导选择请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 新手引导选择请求", " 新手引导选择请求")]
	public class SceneNoviceGuideChooseReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 300205;
		public UInt32 Sequence;
		/// <summary>
		///  角色id
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色id", " 角色id")]
		public UInt64 roleId;
		/// <summary>
		///  选择标志(对应枚举NoviceGuideChooseFlag)
		/// </summary>
		[AdvancedInspector.Descriptor(" 选择标志(对应枚举NoviceGuideChooseFlag)", " 选择标志(对应枚举NoviceGuideChooseFlag)")]
		public byte chooseFlag;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_int8(buffer, ref pos_, chooseFlag);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_int8(buffer, ref pos_, ref chooseFlag);
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
