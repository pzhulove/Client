/*
 * 
 * 语音 - 数据类
 * 
 */
using UnityEngine;
using System.Collections;
using System;

public class MessageData
{
	//接收到来自他人的消息数据 - 基础属性
	public UInt32 YunvaId{ get; set;}         //消息发送者id
	public string ChatRoomId{ get; set;}      //聊天队伍或房间id
	public UInt64 time {get;set;}             //消息发出时间 (格式 ？？ )
	//扩展属性
	public string ExpandString{ get; set;}    //附带的 第三方扩展字段，需要扩展的自定义数据（UTF-8）
}
	
public class RecordVoiceData : MessageData
{
	public string FilePath{ get;private set;}    //音频本地存储路径
	public string FileUrl{ get; private set;}    //音频在线下载路径
	public int Duration{ get; private set;}      //音频时长

	public RecordVoiceData(
		string filePath="",int duration=0,string fileUrl="")
	{
		this.FilePath = filePath;
		this.Duration = duration;

		this.FileUrl = fileUrl;
	}
}

public class TextMsgData : MessageData
{
	public string MsgString{ get; private set;}     //文本消息字符串

	public TextMsgData(string msg="")
	{
		this.MsgString = msg;
	}
}

public class RichMsgData : MessageData
{
	
	public string MsgString{ get; private set;}  //翻译音频得到的文本
	public string FilePath{ get;private set;}    //音频本地存储路径
	public string FileUrl{ get; private set;}    //音频在线下载路径
	public int Duration{ get; private set;}      //音频时长

	/*		
	//test
	public string MsgString{ get;  set;}  //翻译音频得到的文本
	public string FilePath{ get; set;}    //音频本地存储路径
	public string FileUrl{ get;  set;}    //音频在线下载路径
	public int Duration{ get;  set;}      //音频时长
	*/

	public RichMsgData(string msg="",
		string filePath="",
		int duration=0,
		string fileUrl="")
	{
		this.MsgString = msg;
		this.FilePath = filePath;
		this.Duration = duration;
		this.FileUrl = fileUrl;
	}
}