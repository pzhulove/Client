using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneEquipEnhanceUpgradeRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501061;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回值", " 返回值")]
		public UInt32 code;
		/// <summary>
		///  返还材料
		/// </summary>
		[AdvancedInspector.Descriptor(" 返还材料", " 返还材料")]
		public ItemReward[] matNums = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)matNums.Length);
			for(int i = 0; i < matNums.Length; i++)
			{
				matNums[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			UInt16 matNumsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref matNumsCnt);
			matNums = new ItemReward[matNumsCnt];
			for(int i = 0; i < matNums.Length; i++)
			{
				matNums[i] = new ItemReward();
				matNums[i].decode(buffer, ref pos_);
			}
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
