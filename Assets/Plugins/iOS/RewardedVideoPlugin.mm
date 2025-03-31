#import <AnyThinkSDK/AnyThinkSDK.h>
#import <AnyThinkRewardedVideo/AnyThinkRewardedVideo.h>
#import <UIKit/UIKit.h>

@interface TopOnBridge : NSObject
+ (UIViewController *)topViewController;
@end

@implementation TopOnBridge

+ (UIViewController *)topViewController {
    // 获取当前最顶层的视图控制器
    UIViewController *topVC = UIApplication.sharedApplication.keyWindow.rootViewController;
    while (topVC.presentedViewController) {
        topVC = topVC.presentedViewController;
    }
    return topVC;
}

// 使用 extern "C" 包裹方法，确保 C++ 编译器不会对方法名进行修饰
extern "C" {
    // 检查激励视频是否加载完成
    bool isRewardedVideoAdReady(const char* placementID) {
        @autoreleasepool {
           NSString *placementIDString = [NSString stringWithUTF8String:placementID];
           return [[ATAdManager sharedManager] getRewardedVideoValidAdsForPlacementID:placementIDString];
        }
    }

    // 展示激励视频
    void showad(const char* placementID, const char* sceneID, const char* showCustomExt) {
        @autoreleasepool {
            // 将 C 字符串转换为 NSString
            NSString *placementIDString = [NSString stringWithUTF8String:placementID];
            NSString *sceneIDString = [NSString stringWithUTF8String:sceneID];
            NSString *showCustomExtString = [NSString stringWithUTF8String:showCustomExt];

            // 获取当前视图控制器
            UIViewController *viewController = [TopOnBridge topViewController];

            // 创建配置对象
            ATShowConfig *config = [[ATShowConfig alloc] initWithScene:sceneIDString
                                                               showCustomExt:showCustomExtString
                                                    customContentResult:nil]; // 不需要 customContentResult

            // 调用 TopOn SDK 展示激励视频
            [[ATAdManager sharedManager] showRewardedVideoWithPlacementID:placementIDString
                                                                   config:config
                                                     inViewController:viewController
                                                          delegate:nil];
        }
    }
}

@end