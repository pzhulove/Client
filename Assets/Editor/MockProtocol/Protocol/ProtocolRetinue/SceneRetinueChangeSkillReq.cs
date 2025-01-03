using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneRetinueChangeSkillReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507005;
		public UInt32 Sequence;

		public UInt64 id;

		public UInt32[] skillIds = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skillIds.Length);
			for(int i = 0; i < skillIds.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, skillIds[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			UInt16 skillIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref skillIdsCnt);
			skillIds = new UInt32[skillIdsCnt];
			for(int i = 0; i < skillIds.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillIds[i]);
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
