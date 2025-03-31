#import <UIKit/UIKit.h>
#import <AnyThinkSDK/AnyThinkSDK.h>
#import <AnyThinkInterstitial/AnyThinkInterstitial.h>
#import <TenjinUnityInterface.h>

// 插屏广告源信息键值（适配6.4.42）
static NSString *const kATInterstitialExtraAdSourceIDKey = @"adsource_id";
static NSString *const kATInterstitialExtraNetworkNameKey = @"network_name";
static NSString *const kATInterstitialExtraECPMKey = @"adsource_price";

@interface TopOnInterstitialAdBridge : NSObject <ATAdLoadingDelegate, ATInterstitialDelegate>
+ (instancetype)sharedInstance;
@end

@implementation TopOnInterstitialAdBridge

+ (instancetype)sharedInstance {
    static TopOnInterstitialAdBridge *instance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[TopOnInterstitialAdBridge alloc] init];
       
    });
    return instance;
}

#pragma mark - 广告加载方法
#ifdef __cplusplus
extern "C" {
#endif

void _LoadInterstitialAd(const char* placementId, const char* extraJson) {
    if (placementId == NULL) return;

    NSString *placementID = [NSString stringWithUTF8String:placementId];
    NSDictionary *customExtra = @{};

    if (extraJson != NULL) {
        NSError *error;
        NSData *jsonData = [[NSString stringWithUTF8String:extraJson] dataUsingEncoding:NSUTF8StringEncoding];
        customExtra = [NSJSONSerialization JSONObjectWithData:jsonData options:0 error:&error];

        if (error) {
            NSLog(@"[TopOn] 插屏广告JSON解析失败: %@", error);
            return;
        }
    }

    [[ATAdManager sharedManager] loadADWithPlacementID:placementID
                                                 extra:customExtra
                                             delegate:[TopOnInterstitialAdBridge sharedInstance]];
}

bool _IsInterstitialAdReady(const char* placementId) {
    if (placementId == NULL) return false;
    return [[ATAdManager sharedManager] interstitialReadyForPlacementID:
            [NSString stringWithUTF8String:placementId]];
}

void _ShowInterstitialAd(const char* placementId, const char* sceneId, const char* extraJson) {
    if (placementId == NULL || sceneId == NULL) return;

    NSString *placementID = [NSString stringWithUTF8String:placementId];
    NSString *sceneID = [NSString stringWithUTF8String:sceneId];
    NSString *customExt = extraJson ? [NSString stringWithUTF8String:extraJson] : @"";

    if ([[ATAdManager sharedManager] interstitialReadyForPlacementID:placementID]) {
        ATShowConfig *config = [[ATShowConfig alloc] initWithScene:sceneID
                                                     showCustomExt:customExt];

        [[ATAdManager sharedManager] showInterstitialWithPlacementID:placementID
                                                             showConfig:config
                                                   inViewController:UnityGetGLViewController()
                                                       delegate:[TopOnInterstitialAdBridge sharedInstance]
                                             nativeMixViewBlock:nil];
    }
}

#ifdef __cplusplus
}
#endif

#pragma mark - 广告加载回调
- (void)didFinishLoadingADWithPlacementID:(NSString *)placementID {
    NSLog(@"[插屏加载成功] %@", placementID);
    UnitySendMessage("Init", "OnInterstitialLoaded", [placementID UTF8String]);

  
}

- (void)didFailToLoadADWithPlacementID:(NSString *)placementID error:(NSError *)error {
    NSLog(@"[插屏加载失败] %@ - %@", placementID, error.localizedDescription);
    NSDictionary *dict = @{
        @"code": @(error.code),
        @"message": error.localizedDescription ?: @"" 
    };
    UnitySendMessage("Init", "OnInterstitialLoadFailed", [[self serializeJSON:dict] UTF8String]);

}

#pragma mark - 插屏广告展示回调
- (void)interstitialDidShowForPlacementID:(NSString *)placementID extra:(NSDictionary *)extra {
    // 提取基础信息
    NSString *adSourceID = extra[kATInterstitialExtraAdSourceIDKey] ?: @"未知";
    NSString *networkName = extra[kATInterstitialExtraNetworkNameKey] ?: @"未知";
    NSNumber *ecpm = extra[kATInterstitialExtraECPMKey] ?: @0;

    // 提取 adOfferInfo 中的详细信息
    NSDictionary *adOfferInfo = extra[@"adOfferInfo"] ?: @{};
    NSString *country = adOfferInfo[@"country"] ?: @"未知";
    NSNumber *publisherRevenue = adOfferInfo[@"publisher_revenue"] ?: @0;
    NSString *adunitID = adOfferInfo[@"adunit_id"] ?: @"未知";
    NSString *adunitFormat = adOfferInfo[@"ad_format"] ?: @"未知";
    NSString *adsourceID = adOfferInfo[@"adsource_id"] ?: @"未知";

    NSLog(@"\n━━━━ 插屏广告展示 ━━━━\n"
          @"PlacementID: %@\n"
          @"广告源ID: %@\n"
          @"广告平台: %@\n"
          @"eCPM: %.2f\n"
          @"国家: %@\n"
          @"发布者收益: %@\n"
          @"广告单元ID: %@\n"
          @"广告格式: %@\n"
          @"广告源ID: %@\n"
          @"完整Extra: %@",
          placementID, adSourceID, networkName, ecpm.doubleValue, country, publisherRevenue, adunitID, adunitFormat, adsourceID, extra);

    // 构建传递给 Unity 的数据
    NSDictionary *data = @{
        @"placementID": placementID,
        @"auid": adSourceID,
        @"network": networkName,
        @"ecpm": ecpm,
        @"country": country,
        @"publisher_revenue": publisherRevenue,
        @"adunit_id": adunitID,
        @"adunit_format": adunitFormat,
        @"adsource_id": adsourceID
    };

    // 发送给 Unity
    UnitySendMessage("Init", "OnInterstitialShow", [[self serializeJSON:data] UTF8String]);
    [TenjinSDK topOnImpressionFromDict:extra];
   
}

- (void)interstitialDidCloseForPlacementID:(NSString *)placementID extra:(NSDictionary *)extra {
    NSLog(@"[插屏关闭] %@", placementID);
    UnitySendMessage("Init", "OnInterstitialClosed", [placementID UTF8String]);

    
}

- (void)interstitialDidClickForPlacementID:(NSString *)placementID extra:(NSDictionary *)extra {
    NSLog(@"[插屏点击] %@", placementID);
    UnitySendMessage("Init", "OnInterstitialClicked", [placementID UTF8String]);

}

- (void)interstitialDidFailToPlayVideoForPlacementID:(NSString *)placementID error:(NSError *)error extra:(NSDictionary *)extra {
    NSLog(@"[插屏播放失败] %@ - %@", placementID, error.localizedDescription);
    NSDictionary *dict = @{
        @"code": @(error.code),
        @"message": error.localizedDescription ?: @"" 
    };
    UnitySendMessage("Init", "OnInterstitialFailed", [[self serializeJSON:dict] UTF8String]);

  
}

#pragma mark - 辅助方法
- (NSString*)serializeJSON:(NSDictionary*)dict {
    NSError *error;
    NSData *data = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];
    return error ? @"{}" : [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
}

@end