#import <UIKit/UIKit.h>
#import <AnyThinkSDK/AnyThinkSDK.h>
#import <AnyThinkRewardedVideo/AnyThinkRewardedVideo.h>
#import <TenjinUnityInterface.h>
// 新增广告源信息键值（适配6.4.42）
static NSString *const kATRewardedVideoExtraAdSourceIDKey = @"adsource_id";
static NSString *const kATRewardedVideoExtraNetworkNameKey = @"network_name";
static NSString *const kATRewardedVideoExtraECPMKey = @"adsource_price";

@interface TopOnRewardedAdBridge : NSObject <ATAdLoadingDelegate, ATRewardedVideoDelegate>
+ (instancetype)sharedInstance;
@end

@implementation TopOnRewardedAdBridge

+ (instancetype)sharedInstance {
    static TopOnRewardedAdBridge *instance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[TopOnRewardedAdBridge alloc] init];
       
       
    });
    return instance;
}
#pragma mark - 广告加载方法
#ifdef __cplusplus
extern "C" {
#endif

void _LoadRewardedAd(const char* placementId, const char* extraJson) {
    if (placementId == NULL) return;
    
    NSString *placementID = [NSString stringWithUTF8String:placementId];
    NSDictionary *customExtra = @{};
    
    if (extraJson != NULL) {
        NSError *error;
        NSData *jsonData = [[NSString stringWithUTF8String:extraJson] dataUsingEncoding:NSUTF8StringEncoding];
        customExtra = [NSJSONSerialization JSONObjectWithData:jsonData options:0 error:&error];
        
        if (error) {
            NSLog(@"[TopOn] JSON解析失败: %@", error);
            return;
        }
    }
    
    [[ATAdManager sharedManager] loadADWithPlacementID:placementID
                                                 extra:customExtra
                                             delegate:[TopOnRewardedAdBridge sharedInstance]];
}

bool _IsRewardedAdReady(const char* placementId) {
    if (placementId == NULL) return false;
    return [[ATAdManager sharedManager] rewardedVideoReadyForPlacementID:
            [NSString stringWithUTF8String:placementId]];
}

void _ShowRewardedAd(const char* placementId, const char* sceneId, const char* extraJson) {
    if (placementId == NULL || sceneId == NULL) return;
    
    NSString *placementID = [NSString stringWithUTF8String:placementId];
    NSString *sceneID = [NSString stringWithUTF8String:sceneId];
    NSString *customExt = extraJson ? [NSString stringWithUTF8String:extraJson] : @"";
    
    if ([[ATAdManager sharedManager] rewardedVideoReadyForPlacementID:placementID]) {
        ATShowConfig *config = [[ATShowConfig alloc] initWithScene:sceneID
                                                     showCustomExt:customExt];
        
        [[ATAdManager sharedManager] showRewardedVideoWithPlacementID:placementID
                                                               config:config
                                                    inViewController:UnityGetGLViewController()
                                                            delegate:[TopOnRewardedAdBridge sharedInstance]];
    }
}

#ifdef __cplusplus
}
#endif

#pragma mark - 广告加载回调
- (void)didFinishLoadingADWithPlacementID:(NSString *)placementID {
    NSLog(@"[加载成功] %@", placementID);
    UnitySendMessage("Init", "OnAdLoaded", [placementID UTF8String]);
}

- (void)didFailToLoadADWithPlacementID:(NSString *)placementID error:(NSError *)error {
    NSLog(@"[加载失败] %@ - %@", placementID, error.localizedDescription);
    NSDictionary *dict = @{
        @"code": @(error.code),
        @"message": error.localizedDescription ?: @""
    };
    UnitySendMessage("Init", "OnAdLoadFailed", [[self serializeJSON:dict] UTF8String]);
}

#pragma mark - 广告播放回调
- (void)rewardedVideoDidStartPlayingForPlacementID:(NSString *)placementID extra:(NSDictionary *)extra {
    // 提取基础信息
    NSString *adSourceID = extra[kATRewardedVideoExtraAdSourceIDKey] ?: @"未知";
    NSString *networkName = extra[kATRewardedVideoExtraNetworkNameKey] ?: @"未知";
    NSNumber *ecpm = extra[kATRewardedVideoExtraECPMKey] ?: @0;

    // 提取 adOfferInfo 中的详细信息
    NSDictionary *adOfferInfo = extra[@"adOfferInfo"] ?: @{};
    NSString *country = adOfferInfo[@"country"] ?: @"未知";
    NSNumber *publisherRevenue = adOfferInfo[@"publisher_revenue"] ?: @0;
    NSString *adunitID = adOfferInfo[@"adunit_id"] ?: @"未知";
    NSString *adunitFormat = adOfferInfo[@"ad_format"] ?: @"未知";
    NSString *adsourceID = adOfferInfo[@"adsource_id"] ?: @"未知";

    NSLog(@"\n━━━━ 激励视频开始播放 ━━━━\n"
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
    UnitySendMessage("Init", "OnRewardedVideoStart", [[self serializeJSON:data] UTF8String]);
    [TenjinSDK topOnImpressionFromDict:extra];
}

-(void)rewardedVideoDidRewardSuccessForPlacementID:(NSString *)placementID extra:(NSDictionary *)extra {
    NSLog(@"[回调] 奖励发放成功 - PlacementID: %@", placementID);
    NSDictionary *dict = @{
        @"placementID": placementID,
        @"amount": @([extra[@"rewardAmount"] intValue])
    };
    UnitySendMessage("Init", "OnRewardedVideoClosed", [[self serializeJSON:dict] UTF8String]); // 补全发送逻辑
}

-(void)rewardedVideoDidCloseForPlacementID:(NSString *)placementID rewarded:(BOOL)rewarded extra:(NSDictionary *)extra {
    NSLog(@"[回调] 广告关闭 - PlacementID: %@，是否发放奖励: %@", placementID, rewarded ? @"是" : @"否");
    NSDictionary *dict = @{
        @"placementID": placementID,
        @"rewarded": @(rewarded)
    };
    UnitySendMessage("Init", "OnRewardedVideoClosed", [[self serializeJSON:dict] UTF8String]);
}

-(void)rewardedVideoDidFailToPlayForPlacementID:(NSString *)placementID error:(NSError *)error extra:(NSDictionary *)extra {
    NSLog(@"[回调] 播放失败 - PlacementID: %@，错误码: %ld", placementID, (long)error.code);
    NSDictionary *dict = @{
        @"code": @(error.code),
        @"message": error.localizedDescription ?: @""
    };
    UnitySendMessage("Init", "OnRewardedVideoFailed", [[self serializeJSON:dict] UTF8String]);
}

#pragma mark - 辅助方法
- (NSString*)serializeJSON:(NSDictionary*)dict {
    NSError *error;
    NSData *data = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];
    return error ? @"{}" : [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
}

@end
