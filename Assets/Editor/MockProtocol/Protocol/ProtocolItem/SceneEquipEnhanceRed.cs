using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client气息装备激活变成红字装备
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client气息装备激活变成红字装备", "scene->client气息装备激活变成红字装备")]
	public class SceneEquipEnhanceRed : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501064;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备uid", " 装备uid")]
		public UInt64 euqipUid;
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
			BaseDLL.encode_uint32(buffer, ref pos_, stuffID);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
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
