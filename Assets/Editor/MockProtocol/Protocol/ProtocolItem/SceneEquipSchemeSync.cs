using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client	同步装备方案数据
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client	同步装备方案数据", "scene->client	同步装备方案数据")]
	public class SceneEquipSchemeSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501087;
		public UInt32 Sequence;

		public EquipSchemeInfo[] schemes = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)schemes.Length);
			for(int i = 0; i < schemes.Length; i++)
			{
				schemes[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 schemesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref schemesCnt);
			schemes = new EquipSchemeInfo[schemesCnt];
			for(int i = 0; i < schemes.Length; i++)
			{
				schemes[i] = new EquipSchemeInfo();
				schemes[i].decode(buffer, ref pos_);
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
