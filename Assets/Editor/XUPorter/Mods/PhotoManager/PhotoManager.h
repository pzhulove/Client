#import <Foundation/Foundation.h>
#import <Photos/PHFetchResult.h>
#import <Photos/PHAsset.h>
#import <Photos/PHCollection.h>

#ifdef __cplusplus
extern "C"{
#endif
    void _InitAlartText(char *title,char *message,char *btnText);
    void _SavePhoto(char *readAddr);
    void showAlart();
    void createdAssets();
    void createdCollection();
    void saveImageIntoAlbum();
#ifdef __cplusplus
} // extern "C"
#endif
