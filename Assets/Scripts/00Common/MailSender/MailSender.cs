using UnityEngine;
using System.Collections;
using System.Net.Mail;
using System.Net;
using System;
using System.Text;
using System.Net.Mime;

public class MailSender
{
    static SmtpClient smtpClient = null;// 设置smtp协议
    static MailMessage mailMessage_mai = null; //设置邮件信息,要发送的内容
    /// <summary>
    /// 发邮件
    /// </summary>
    /// <param name="smtp">邮箱服务器名称</param>
    /// <param name="affix">附件路径</param>
    /// <param name="from">发件箱地址</param>
    /// <param name="pwd">发件箱密码</param>
    /// <param name="to">收件箱地址</param>
    /// <param name="title">邮件标题</param>
    /// <param name="body">邮件正文</param>
    /// <returns></returns>

    static bool _SendMail(string smtp, string affix,
        string from, string pwd, string to, string title, string body)
    { 
        smtpClient = new SmtpClient();
        smtpClient.Host = smtp;
        smtpClient.UseDefaultCredentials = false;

        //指定服务器认证
        NetworkCredential network = new NetworkCredential(from, pwd);
        //指定发件人信息,包括邮箱地址和密码
        NetworkCredential nc = new NetworkCredential(from, pwd);
        smtpClient.Credentials = nc as ICredentialsByHost; //这个在手机平台不成功
        //指定如何发送邮件
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

        //创建mailMessage对象
        mailMessage_mai = new MailMessage(from, to);
        mailMessage_mai.Subject = title;

        //设置正文默认格式为html
        mailMessage_mai.Body = body;
        mailMessage_mai.IsBodyHtml = true;
        mailMessage_mai.BodyEncoding = Encoding.UTF8;
        mailMessage_mai.IsBodyHtml = false;

        //添加附件
        if (!string.IsNullOrEmpty(affix))
        {
            Attachment data = new Attachment(affix, MediaTypeNames.Application.Octet);
            mailMessage_mai.Attachments.Add(data);
        }

        try
        {
            // smtpClient.Send(mailMessage_mai);
            //发送
            smtpClient.SendAsync(mailMessage_mai, "000000000");
            return true;//返回true则发送成功
        }
        catch (Exception)
        {
            return false;//返回false则发送失败
        }
    }

    public static bool SetEmail(string title, string content)
    {
        string prefixPlatform = "Unknown";
#if UNITY_ANDROID
        prefixPlatform = "Android";
#elif UNITY_IOS
        prefixPlatform = "iOS";
#elif UNITY_STANDALONE_WIN
        prefixPlatform = "Windows";
#else
        prefixPlatform = "Unknown";
#endif
        content = "平台：" + prefixPlatform + "，" + content;

        return _SendMail("smtp.exmail.qq.com", null, "jinxingmeng@herogo.net", "Q123wer", "simonking200@vip.qq.com", title, content);
    }

    public static bool SetEmail(string toUser, string title, string content)
    {
        string prefixPlatform = "Unknown";
#if UNITY_ANDROID
        prefixPlatform = "Android";
#elif UNITY_IOS
        prefixPlatform = "iOS";
#elif UNITY_STANDALONE_WIN
        prefixPlatform = "Windows";
#else
        prefixPlatform = "Unknown";
#endif
        content = "平台：" + prefixPlatform + "，" + content;

        return _SendMail("smtp.exmail.qq.com", null, "jinxingmeng@herogo.net", "Q123wer", toUser, title, content);
    }
}
