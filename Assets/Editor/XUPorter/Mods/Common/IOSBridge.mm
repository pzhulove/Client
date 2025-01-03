//
//  XYBridge.cpp
//  XYPlatformDemo
//
//  Created by Yu Lekai on 03/11/2016.
//  Copyright © 2016 Eason. All rights reserved.
//

#include "IOSBridge.h"

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import "UnityInterface.h"

void _CommonInit()
{
	[[UIApplication sharedApplication] setIdleTimerDisabled:YES];
    _RemoveAllNotification();
}

void _ResetBadge()
{
	[UIApplication sharedApplication].applicationIconBadgeNumber = 0;
}


NSDate* GetTargetDate(long tsp) //haomiao
{

    NSDate* now = [NSDate date];
    long nowtsp = [[NSNumber numberWithDouble:[now timeIntervalSince1970]] longValue];
    long curtsp = nowtsp + tsp / 1000; //sec
    
    
    NSDate* date = [NSDate dateWithTimeIntervalSince1970:curtsp]; //sec
    return date;
}

void _SetNotificationWithTsp(int nid, const char *content, const char* title, long tsp)
{
    if (HasNotification(nid))
        return;

    if ([[UIApplication sharedApplication] respondsToSelector:@selector(registerUserNotificationSettings:)]) {
        UIUserNotificationType type = UIUserNotificationTypeAlert | UIUserNotificationTypeBadge | UIUserNotificationTypeSound;
        UIUserNotificationSettings *settings = [UIUserNotificationSettings settingsForTypes:type categories:nil];
        [[UIApplication sharedApplication] registerUserNotificationSettings:settings];
    }


    UILocalNotification *notification = [[UILocalNotification alloc] init];

    NSDate *now1=[NSDate date];
    notification.timeZone=[NSTimeZone defaultTimeZone];
    notification.repeatInterval=NSDayCalendarUnit;
    notification.applicationIconBadgeNumber = 1;
    notification.alertAction = @"";
    notification.alertBody=[NSString stringWithUTF8String:content];
    notification.soundName = UILocalNotificationDefaultSoundName;

    NSDictionary * userDict =  [NSDictionary dictionaryWithObjectsAndKeys:@"1",[NSString stringWithFormat:@"%i", nid],nil];

    notification.userInfo = userDict;


    NSDate* tarDate = GetTargetDate(tsp);
    
    notification.fireDate = tarDate;
    
    [[UIApplication sharedApplication]   scheduleLocalNotification:notification];
    
    
}

UILocalNotification *GetNotification(int nid)
{
	NSArray *localNotifications = [UIApplication sharedApplication].scheduledLocalNotifications;  
	for (UILocalNotification *notification in localNotifications)  
	{
		NSDictionary *userInfo = notification.userInfo;  
		if (userInfo)  
		{
			NSString *key =  [NSString stringWithFormat:@"%i", nid];
			NSString *info = userInfo[key];  
			if (info != nil)  
			{
				return notification;
			}
		}
	}
	
	return nil;
}

bool HasNotification(int nid)
{
    return GetNotification(nid) != nil;
}

void _RemoveNotification(int nid)
{
	UILocalNotification *notification = GetNotification(nid);
	if (notification != nil)
	{
		 [[UIApplication sharedApplication] cancelLocalNotification:notification];  
	}
}

void _RemoveAllNotification()
{
		[UIApplication sharedApplication].applicationIconBadgeNumber = 0;
		[[UIApplication sharedApplication] cancelAllLocalNotifications];
}

void _SetNotification(int nid, const char *content, const char* title, int hour)
{
    if (HasNotification(nid))
			return;
        
   	if ([[UIApplication sharedApplication] respondsToSelector:@selector(registerUserNotificationSettings:)]) {
		  UIUserNotificationType type = UIUserNotificationTypeAlert | UIUserNotificationTypeBadge | UIUserNotificationTypeSound;
		  UIUserNotificationSettings *settings = [UIUserNotificationSettings settingsForTypes:type categories:nil];
		  [[UIApplication sharedApplication] registerUserNotificationSettings:settings];
		  }
    
    
    UILocalNotification *notification = [[UILocalNotification alloc] init];
    
    NSDate *now1=[NSDate date];
    notification.timeZone=[NSTimeZone defaultTimeZone];
    notification.repeatInterval=NSDayCalendarUnit;
    notification.applicationIconBadgeNumber = 1;
    notification.alertAction = @"";
    notification.alertBody=[NSString stringWithUTF8String:content];
    notification.soundName = UILocalNotificationDefaultSoundName;
    
    NSDictionary * userDict =  [NSDictionary dictionaryWithObjectsAndKeys:@"1",[NSString stringWithFormat:@"%i", nid],nil];  
    
    notification.userInfo = userDict;  
    
    
    NSDate *now = [NSDate date];

    NSCalendar *calendar = [NSCalendar currentCalendar];

    NSUInteger unitFlags = NSYearCalendarUnit | NSMonthCalendarUnit | NSDayCalendarUnit | NSHourCalendarUnit | NSMinuteCalendarUnit | NSSecondCalendarUnit;

    NSDateComponents *dateComponent = [calendar components:unitFlags fromDate:now];

    NSInteger year = [dateComponent year];
    NSInteger month = [dateComponent month];
    NSInteger day = [dateComponent day];
    
    NSDateComponents *components = [[NSDateComponents alloc] init];
    
    [components setYear:year];
    [components setMonth:month];
    [components setDay:day];
    [components setHour:hour];
    [components setMinute:0];
    [components setSecond:0];
    
    NSCalendar *gregorian = [[NSCalendar alloc] initWithCalendarIdentifier:NSGregorianCalendar];
    
    NSDate *fireDate = [gregorian dateFromComponents:components];
    
    notification.fireDate = fireDate;
    
    [[UIApplication sharedApplication]   scheduleLocalNotification:notification];
}


void _SetNotificationWeekly(int nid, const char *content, const char* title, int weekday, int hour, int minute)
{
    if (HasNotification(nid))
        return;
    
    if ([[UIApplication sharedApplication] respondsToSelector:@selector(registerUserNotificationSettings:)]) {
        UIUserNotificationType type = UIUserNotificationTypeAlert | UIUserNotificationTypeBadge | UIUserNotificationTypeSound;
        UIUserNotificationSettings *settings = [UIUserNotificationSettings settingsForTypes:type categories:nil];
        [[UIApplication sharedApplication] registerUserNotificationSettings:settings];
    }
    
    
    UILocalNotification *notificationWeekly = [[UILocalNotification alloc] init];

    notificationWeekly.timeZone=[NSTimeZone defaultTimeZone];
    notificationWeekly.repeatInterval=NSCalendarUnitWeekOfYear;
    notificationWeekly.applicationIconBadgeNumber = 1;
    notificationWeekly.alertAction = @"";
    notificationWeekly.alertBody=[NSString stringWithUTF8String:content];
    notificationWeekly.soundName = UILocalNotificationDefaultSoundName;

    NSDictionary * userDict =  [NSDictionary dictionaryWithObjectsAndKeys:@"1",[NSString stringWithFormat:@"%i", nid],nil];
    
    notificationWeekly.userInfo = userDict;
    
    NSDate *now = [NSDate date];
    
    NSCalendar *calendar = [NSCalendar currentCalendar];
    NSInteger monthDays = [[NSCalendar currentCalendar] rangeOfUnit:NSDayCalendarUnit inUnit:NSMonthCalendarUnit forDate:[NSDate date]].length;

    NSUInteger unitFlags = NSYearCalendarUnit | NSMonthCalendarUnit | NSDayCalendarUnit | NSWeekdayCalendarUnit | NSHourCalendarUnit | NSMinuteCalendarUnit | NSSecondCalendarUnit;
    
    NSDateComponents *dateComponent = [calendar components:unitFlags fromDate:now];
    
    NSInteger week = [dateComponent weekday];
    
    //计算最近的weekday日期
    NSInteger goalDay = (7 - week + weekday) % 7;
    NSDate *nextWeekday = [now dateByAddingTimeInterval:60*60*24*goalDay];
    
    NSDateComponents *nextComponents = [calendar components:NSYearCalendarUnit | NSMonthCalendarUnit | NSDayCalendarUnit | NSWeekdayCalendarUnit | NSHourCalendarUnit | NSMinuteCalendarUnit | NSSecondCalendarUnit fromDate:nextWeekday];
    
    NSDateComponents *components = [[NSDateComponents alloc] init];
    
    [components setYear:[nextComponents year]];
    [components setMonth:[nextComponents month]];
    [components setDay:[nextComponents day]];
    [components setHour:hour];
    [components setMinute:minute];
    [components setSecond:0];
    
    NSCalendar *gregorian = [[NSCalendar alloc] initWithCalendarIdentifier:NSGregorianCalendar];
    
    NSDate *fireDate = [gregorian dateFromComponents:components];
    
    NSString *dateString = [NSDateFormatter localizedStringFromDate:fireDate
                                                          dateStyle:NSDateFormatterShortStyle
                                                          timeStyle:NSDateFormatterFullStyle];
    
    notificationWeekly.fireDate = fireDate;
    
    [[UIApplication sharedApplication]   scheduleLocalNotification:notificationWeekly];
}

//设置屏幕亮度（范围 0 - 1）
void _SetBrightness(float value)
{
    [[UIScreen mainScreen] setBrightness:value];
}

//获取屏幕亮度（范围 0 - 1）
float _GetBrightness()
{
    return [[UIScreen mainScreen] brightness];
}

void ExitIOS()
{
    exit(0);
}

//获取系统电量 - 精度 5%    0.00 - 1.00    -1.00表示模拟器
float _GetBatteryLevel()
{
    [[UIDevice currentDevice]setBatteryMonitoringEnabled:YES];
    return [[UIDevice currentDevice]batteryLevel];
}

//请求麦克风权限
bool audioAuthAgree = false;
bool _RequestAudioAuthorization()
{
    AVAuthorizationStatus audioAuthStatus = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeAudio];
    if(audioAuthStatus == AVAuthorizationStatusNotDetermined)//未询问用户是否授权
    {
        [AVCaptureDevice requestAccessForMediaType:AVMediaTypeAudio completionHandler:^(BOOL granted) {
            //true//用户同意授权   false //用户拒绝授权
            audioAuthAgree = granted;
        }];
        return audioAuthAgree;
        
    }else if(audioAuthStatus == AVAuthorizationStatusDenied || audioAuthStatus == AVAuthorizationStatusRestricted)//未授权
    {
        [AVCaptureDevice requestAccessForMediaType:AVMediaTypeAudio completionHandler:^(BOOL granted) {
            //true//用户同意授权   false //用户拒绝授权
            audioAuthAgree = granted;
        }];
        return audioAuthAgree;
    }
    return true;//已授权
}

bool audioActive = false;
void _SetAudioSessionActive()
{
    //UnitySetAudioSessionActive(1);
    //AVAudioSession *audioSession = [AVAudioSession sharedInstance];
    //audioSession setCategory:<#(nonnull NSString *)#> withOptions:<#(AVAudioSessionCategoryOptions)#> error:<#(NSError * _Nullable __autoreleasing * _Nullable)#>
    //audioActive = [audioSession setActive:YES error:nil];
    
    //NSLog(@"ios Set Audio Session Caregory Start ............................");
    [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategorySoloAmbient error:nil];
    //NSLog(@"ios Set Audio Session Caregory End .............................");

}

bool _GetAudioSessionActive()
{
    AVAudioSession *audioSession = [AVAudioSession sharedInstance];
    bool audioActive = [audioSession isOtherAudioPlaying];
    return audioActive;
}

char* _GetTextFromClipboard()
{
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    NSString *text = pasteboard.string;
    return MakeStrCopy([text UTF8String]);
}

char* MakeStrCopy(const char* str)
{
    if(str == NULL)
        return NULL;
    char* res = (char*)malloc(strlen(str)+1);
    strcpy(res, str);
    return res;
}

bool _IsIOSSystemVersionMoreThanNine()
{
    NSString *version = [UIDevice currentDevice].systemVersion;
    //NSLog(@"IOS version str %@",version);
    //NSLog(@"IOS version is %f",version.doubleValue);
    if(version.doubleValue >= 9.0)
    {
        return true;
    }else
    {
        return false;
    }
}

bool _HasNotch()
{
	if (__IPHONE_OS_VERSION_MAX_ALLOWED < __IPHONE_11_0) {
        return false;
    }
	
	UIEdgeInsets safeAreaInsets = UIApplication.sharedApplication.windows.firstObject.safeAreaInsets;
    
    float bottomSpace = 0;
    
    switch (UIApplication.sharedApplication.statusBarOrientation) {
        case UIInterfaceOrientationPortrait:{
            bottomSpace = safeAreaInsets.bottom;
        }break;
        case UIInterfaceOrientationLandscapeLeft:{
            bottomSpace = safeAreaInsets.right;
        }break;
        case UIInterfaceOrientationLandscapeRight:{
            bottomSpace = safeAreaInsets.left;
        } break;
        case UIInterfaceOrientationPortraitUpsideDown:{
            bottomSpace = safeAreaInsets.top;
        }break;
        default:
            bottomSpace = safeAreaInsets.bottom;
            break;
    }
    
    if (bottomSpace > 0.0001f || bottomSpace < -0.0001f) {
        return true;
    }
    return false;
}

int _GetNotchSize()
{
    int i = 0;
    
    if (__IPHONE_OS_VERSION_MAX_ALLOWED < __IPHONE_11_0) {
        return i;
    }
    
    UIEdgeInsets safeAreaInsets = UIApplication.sharedApplication.windows.firstObject.safeAreaInsets;
    
    float bottomSpace = 0;
    
    switch (UIApplication.sharedApplication.statusBarOrientation) {
        case UIInterfaceOrientationPortrait:{
            bottomSpace = safeAreaInsets.bottom;
        }break;
        case UIInterfaceOrientationLandscapeLeft:{
            bottomSpace = safeAreaInsets.right;
        }break;
        case UIInterfaceOrientationLandscapeRight:{
            bottomSpace = safeAreaInsets.left;
        } break;
        case UIInterfaceOrientationPortraitUpsideDown:{
            bottomSpace = safeAreaInsets.top;
        }break;
        default:
            bottomSpace = safeAreaInsets.bottom;
            break;
    }
    

    float scale = [UIScreen mainScreen].scale;

    
    i = (int)(bottomSpace*scale);
    
    return i;
}
