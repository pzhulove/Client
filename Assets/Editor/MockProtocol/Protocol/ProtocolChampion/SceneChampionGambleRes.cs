using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Scene->Client 冠军赛押注返回
	/// </summary>
	[AdvancedInspector.Descriptor("Scene->Client 冠军赛押注返回", "Scene->Client 冠军赛押注返回")]
	public class SceneChampionGambleRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509812;
		public UInt32 Sequence;

		public UInt32 id;

		public UInt64 option;

		public UInt32 errorCode;

		public UInt32 num;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint64(buffer, ref pos_, option);
			BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint64(buffer, ref pos_, ref option);
			BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
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
