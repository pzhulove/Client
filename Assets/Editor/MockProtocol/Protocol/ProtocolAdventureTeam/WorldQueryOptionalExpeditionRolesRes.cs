using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 可选远征角色查询返回
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 可选远征角色查询返回", " world->client 可选远征角色查询返回")]
	public class WorldQueryOptionalExpeditionRolesRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608614;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误码", " 错误码")]
		public UInt32 resCode;
		/// <summary>
		///  可选远征角色列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 可选远征角色列表", " 可选远征角色列表")]
		public ExpeditionMemberInfo[] roles = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, resCode);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)roles.Length);
			for(int i = 0; i < roles.Length; i++)
			{
				roles[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
			UInt16 rolesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref rolesCnt);
			roles = new ExpeditionMemberInfo[rolesCnt];
			for(int i = 0; i < roles.Length; i++)
			{
				roles[i] = new ExpeditionMemberInfo();
				roles[i].decode(buffer, ref pos_);
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
