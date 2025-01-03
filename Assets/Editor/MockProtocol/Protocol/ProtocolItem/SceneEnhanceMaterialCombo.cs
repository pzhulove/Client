using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client装备增幅材料合成
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client装备增幅材料合成", "scene->client装备增幅材料合成")]
	public class SceneEnhanceMaterialCombo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501068;
		public UInt32 Sequence;
		/// <summary>
		///  目标ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标ID", " 目标ID")]
		public UInt32 goalId;
		/// <summary>
		///  目标数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标数量", " 目标数量")]
		public UInt32 goalNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, goalId);
			BaseDLL.encode_uint32(buffer, ref pos_, goalNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref goalId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref goalNum);
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
