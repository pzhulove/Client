
#ifndef XYBridge_hpp
#define XYBridge_hpp

# define _DLLExport __declspec (dllexport)

#ifdef __cplusplus
extern "C"{
#endif
    
    void _Init(const char *iosExtraInfoString , bool debug);
    void _OpenLogin();
    void _Pay(const char* payIofoString , const char* userInfoString);
    void _OpenMobileBind();
    void _CheckIsBindPhoneNum();
    void _ReportRoleInfo(const char *roleInfo ,int param);
    void _SetRoleLoginGame();
    void _SetRoleLogoutGame();
    void _GetNewVersionInAppstore();
    bool _IsLoginSmallGame();
    bool _IsSDKInited();
    
#ifdef __cplusplus
} // extern "C"
#endif

#endif 
