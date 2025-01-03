using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client红字装备属性转化
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client红字装备属性转化", "scene->client红字装备属性转化")]
	public class SceneEquipEnhanceChg : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501066;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备uid", " 装备uid")]
		public UInt64 euqipUid;
		/// <summary>
		///  转化路线
		/// </summary>
		[AdvancedInspector.Descriptor(" 转化路线", " 转化路线")]
		public byte enhanceType;
		/// <summary>
		/// 使用的材料道具id
		/// </summary>
		[AdvancedInspector.Descriptor("使用的材料道具id", "使用的材料道具id")]
		public UInt32 stuffID;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, euqipUid);
			BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
			BaseDLL.encode_uint32(buffer, ref pos_, stuffID);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
			BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref stuffID);
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
