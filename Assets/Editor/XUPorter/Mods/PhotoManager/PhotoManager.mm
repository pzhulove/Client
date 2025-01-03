#import "PhotoManager.h"
#import "Photos/PHAssetChangeRequest.h"
#import "Photos/PHAssetCollectionChangeRequest.h"

typedef enum {
    NORMAL,
    ERROR,
    FINISH,
}takePhotoState;

UIImage *img  = NULL;
PHFetchResult<PHAsset *>* Assets = nil;
PHAssetCollection * Collection = nil;
takePhotoState state = FINISH;
NSString *alartTitle = nil;
NSString *alartMessage = nil;
NSString *alartBtnText = nil;

void _InitAlartText(char *title,char *message,char *btnText){
    alartTitle = [[NSString alloc]initWithUTF8String:title];
    alartMessage = [[NSString alloc]initWithUTF8String:message];
    alartBtnText = [[NSString alloc]initWithUTF8String:btnText];
}

void _SavePhoto(char *readAddr)
{
    NSString *strReadAddr = [NSString stringWithUTF8String:readAddr];
    if(strReadAddr == nil){
        return;
    }
    img = [UIImage imageWithContentsOfFile:strReadAddr];
    if(img == NULL){
        return;
    }
    PHAuthorizationStatus status = [PHPhotoLibrary authorizationStatus];
    if (status == PHAuthorizationStatusRestricted || status == PHAuthorizationStatusDenied){
        showAlart();
        return;
    }
    else{
        state = NORMAL;
        createdAssets();
    }
}

void showAlart()
{
    UIAlertView * alart = [[UIAlertView alloc]initWithTitle:alartTitle message:alartMessage delegate:nil cancelButtonTitle:alartBtnText otherButtonTitles:nil, nil];
    [alart show];
}

void createdAssets()
{
    if(state != NORMAL){
        NSLog(@"Permission denied or Storage filled");
        return;
    }
    
    __block NSString *createdAssetId = nil;
    
    // 添加图片到【相机胶卷】
    [[PHPhotoLibrary sharedPhotoLibrary]performChanges:^{
        createdAssetId = [PHAssetChangeRequest creationRequestForAssetFromImage:img].placeholderForCreatedAsset.localIdentifier;
    } completionHandler:^(BOOL success,NSError * _Nullable error){
        if(nil != error || !success){
            state = ERROR;
        }
        
        dispatch_sync(dispatch_get_main_queue(), ^{
            if (createdAssetId == nil){
                Assets = nil;
            }
            else{
                // 在保存完毕后取出图片
                Assets = [PHAsset fetchAssetsWithLocalIdentifiers:@[createdAssetId] options:nil];
            }
            createdCollection();
        });
    }];
}


void createdCollection()
{
    if(state != NORMAL){
        NSLog(@"Permission denied or Storage filled");
        return;
    }
    // 获取软件的名字作为相册的标题
    NSString *title = [NSBundle mainBundle].infoDictionary[(NSString *)kCFBundleNameKey];
    
    // 获得所有的自定义相册
    PHFetchResult<PHAssetCollection *> *collections = [PHAssetCollection fetchAssetCollectionsWithType:PHAssetCollectionTypeAlbum subtype:PHAssetCollectionSubtypeAlbumRegular options:nil];
    for (PHAssetCollection *collection in collections) {
        if ([collection.localizedTitle isEqualToString:title]) {
            Collection = collection;
            saveImageIntoAlbum();
            return;
        }
    }
    
    // 代码执行到这里，说明还没有自定义相册
    
    __block NSString *createdCollectionId = nil;
    
    // 创建一个新的相册
    [[PHPhotoLibrary sharedPhotoLibrary]performChanges:^{
        createdCollectionId = [PHAssetCollectionChangeRequest creationRequestForAssetCollectionWithTitle:title].placeholderForCreatedAssetCollection.localIdentifier;
    } completionHandler:^(BOOL success, NSError * _Nullable error) {
        if(nil != error || !success){
            state = ERROR;
        }
        else{
            dispatch_sync(dispatch_get_main_queue(), ^{
                if(createdCollectionId == nil) Collection = nil;
                Collection = [PHAssetCollection fetchAssetCollectionsWithLocalIdentifiers:@[createdCollectionId] options:nil].firstObject;
                saveImageIntoAlbum();
            });
        }
    }];
}

void saveImageIntoAlbum()
{
    if(state != NORMAL){
        NSLog(@"Permission denied or Storage filled");
        return;
    }
    
    if (Assets == nil || Collection == nil) {
        NSLog(@"Assets is nil or Collection is nil");
        return;
    }
    [[PHPhotoLibrary sharedPhotoLibrary] performChanges:^{
        PHAssetCollectionChangeRequest *request = [PHAssetCollectionChangeRequest changeRequestForAssetCollection:Collection];
        [request insertAssets:Assets atIndexes:[NSIndexSet indexSetWithIndex:0]];
    } completionHandler:^(BOOL success, NSError * _Nullable error) {
        if(nil != error || !success){
            state = ERROR;
        }
        else{
            state = FINISH;
        }
        dispatch_sync(dispatch_get_main_queue(), ^{
            if(state == ERROR){
                showAlart();
            }
            if(state == FINISH){
                
            }
        });
    }];
}


