//
//  XYBridge.hpp
//  XYPlatformDemo
//
//  Created by Yu Lekai on 03/11/2016.
//  Copyright © 2016 Eason. All rights reserved.
//

#ifndef IOSBridge_hpp
#define IOSBridge_hpp

# define _DLLExport __declspec (dllexport)

#ifdef __cplusplus
extern "C"{
#endif
		void _ResetBadge();
		void _SetNotification(int nid, const char *content, const char* title, int hour);
        void _SetNotificationWeekly(int nid, const char *content, const char* title, int weekday, int hour, int minute);
		void _RemoveNotification(int nid);
		void _RemoveAllNotification();
        bool HasNotification(int nid);
		void _SetNotificationWithTsp(int nid, const char *content, const char* title, long tsp);



        void _CommonInit();
    		
    		void _SetBrightness(float value);
    		float _GetBrightness();
        void ExitIOS();
    
        float _GetBatteryLevel();
        bool _RequestAudioAuthorization();
        void _SetAudioSessionActive();
        bool _GetAudioSessionActive();
		bool _IsIOSSystemVersionMoreThanNine();
        char* _GetTextFromClipboard();
        char* MakeStrCopy(const char* str);
        
        bool _HasNotch();
		int _GetNotchSize();
#ifdef __cplusplus
} // extern "C"
#endif

#endif
