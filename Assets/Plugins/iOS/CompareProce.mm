#import "AnyThinkSDK/AnyThinkSDK.h"

extern "C"
const char* comparePrice(const char* priceInfo) {
    @autoreleasepool {
        // 1. 解析输入JSON
        NSString *jsonString = [NSString stringWithUTF8String:priceInfo];
        NSData *jsonData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
        NSError *error;
        NSArray *jsonArray = [NSJSONSerialization JSONObjectWithData:jsonData options:0 error:&error];
        if (error) {
            NSLog(@"JSON解析失败: %@", error.localizedDescription);
            return NULL;
        }

        // 2. 创建比价列表
        NSMutableArray<ATCustomContentInfo *> *adBids = [NSMutableArray array];
        for (NSDictionary *dict in jsonArray) {
            NSString *key = dict[@"key"];
            NSNumber *price = dict[@"price"];
            
            if ([price doubleValue] == -1) {
                // 内部广告：通过广告位ID初始化
                ATCustomContentInfo *info = [[ATCustomContentInfo alloc] initInfoWithContentString:key contentObject:nil];
                [adBids addObject:info];
                NSLog(@"[内部广告] 添加广告位: %@", key);
            } else {
                // 外部广告：通过价格初始化，key存入contentObject
                ATCustomContentInfo *info = [[ATCustomContentInfo alloc] initInfoWithContentDouble:[price doubleValue] contentObject:key];
                [adBids addObject:info];
                NSLog(@"[外部广告] 添加广告位: %@, 价格: %@", key, price);
            }
        }

        // 3. 获取比价结果
        ATCustomContentResult *sortedResult = [[ATSDKGlobalSetting sharedManager] customContentResultReviewByInfos:adBids];
        
        // 4. 构建返回数据
        NSMutableArray<NSDictionary *> *resultArray = [NSMutableArray array];
        for (ATCustomContentInfo *info in sortedResult.contentInfoArray) {
            NSString *finalKey = @"";
            double finalPrice = info.customContentDouble;
            NSString *adSourceID = @"";
            
            // 处理内部广告
            if (info.customContentDouble == -1) {
                finalKey = info.customContentString ?: @"";
                
                // 从adOfferInfo获取实际价格和广告源信息
                if (info.adOfferInfo) {
                    NSLog(@"[内部广告] 广告位 %@ 的adOfferInfo: %@", finalKey, info.adOfferInfo);
                    
                    // 尝试常见字段名（需根据日志调整）
                    NSNumber *offerPrice = info.adOfferInfo[@"price"] ?:
                    
                                          info.adOfferInfo[@"ecpm"] ?:
                                          info.adOfferInfo[@"price_value"];
                                          
                    if (offerPrice) {
                        finalPrice = [offerPrice doubleValue];
                        NSLog(@"[内部广告] 实际价格获取成功: %.2f", finalPrice);
                    } else {
                        NSLog(@"[错误] 未找到价格字段");
                    }
                    
                    // 获取广告源 ID（假设字段名为 "adSourceID" 或其他）
                    adSourceID = info.adOfferInfo[@"adSourceID"] ?: info.adOfferInfo[@"sourceID"] ?: @"";
                    if (!adSourceID.length) {
                        NSLog(@"[警告] 未找到广告源 ID");
                    }
                } else {
                    NSLog(@"[错误] 广告位 %@ 未加载成功", finalKey);
                }
            }
            // 处理外部广告
            else {
                finalKey = (NSString *)info.customContentObject ?: @"";
            }
            
            [resultArray addObject:@{
                @"key": finalKey,
                @"price": @(finalPrice),
                @"adSourceID": adSourceID
            }];
        }

        // 5. 返回JSON字符串
        NSError *jsonError;
        NSData *resultData = [NSJSONSerialization dataWithJSONObject:resultArray options:0 error:&jsonError];
        if (jsonError) {
            NSLog(@"JSON序列化失败: %@", jsonError.localizedDescription);
            return NULL;
        }
        NSString *resultString = [[NSString alloc] initWithData:resultData encoding:NSUTF8StringEncoding];
        return strdup([resultString UTF8String]);
    }
}