using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步系统开关（登录时）
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步系统开关（登录时）", " 同步系统开关（登录时）")]
	public class SceneSyncServiceSwitch : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501214;
		public UInt32 Sequence;
		/// <summary>
		///  关掉的系统（对应枚举ServiceType）
		/// </summary>
		[AdvancedInspector.Descriptor(" 关掉的系统（对应枚举ServiceType）", " 关掉的系统（对应枚举ServiceType）")]
		public UInt16[] closedServices = new UInt16[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)closedServices.Length);
			for(int i = 0; i < closedServices.Length; i++)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, closedServices[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 closedServicesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref closedServicesCnt);
			closedServices = new UInt16[closedServicesCnt];
			for(int i = 0; i < closedServices.Length; i++)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref closedServices[i]);
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
