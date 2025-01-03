using System;
using System.Text;

namespace Mock.Protocol
{

	public class GateClientLoginRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 300204;
		public UInt32 Sequence;

		public UInt32 result;

		public byte hasrole;
		/// <summary>
		///  需要等待的玩家数
		/// </summary>
		[AdvancedInspector.Descriptor(" 需要等待的玩家数", " 需要等待的玩家数")]
		public UInt32 waitPlayerNum;
		/// <summary>
		///  服务器开服时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 服务器开服时间", " 服务器开服时间")]
		public UInt32 serverStartTime;
		/// <summary>
		/// 通知老兵回归
		/// </summary>
		[AdvancedInspector.Descriptor("通知老兵回归", "通知老兵回归")]
		public byte notifyVeteranReturn;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_int8(buffer, ref pos_, hasrole);
			BaseDLL.encode_uint32(buffer, ref pos_, waitPlayerNum);
			BaseDLL.encode_uint32(buffer, ref pos_, serverStartTime);
			BaseDLL.encode_int8(buffer, ref pos_, notifyVeteranReturn);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_int8(buffer, ref pos_, ref hasrole);
			BaseDLL.decode_uint32(buffer, ref pos_, ref waitPlayerNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref serverStartTime);
			BaseDLL.decode_int8(buffer, ref pos_, ref notifyVeteranReturn);
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
