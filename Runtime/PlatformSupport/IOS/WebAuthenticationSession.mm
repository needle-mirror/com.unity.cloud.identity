// WebAuthenticationSession.mm

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <AuthenticationServices/AuthenticationServices.h>
#import <SafariServices/SafariServices.h>
#import "UnityAppController.h"

// Strong reference storage
static id g_authenticationSession = nil;

// iOS 13+ Presentation context provider
@interface PKCEPresentationProvider : NSObject <ASWebAuthenticationPresentationContextProviding>
@end
@implementation PKCEPresentationProvider
- (ASPresentationAnchor)presentationAnchorForWebAuthenticationSession:(ASWebAuthenticationSession *)session {
    if (@available(iOS 15.0, *)) {
        for (UIWindowScene *scene in [UIApplication sharedApplication].connectedScenes) {
            if (scene.activationState == UISceneActivationStateForegroundActive) {
                return scene.windows.firstObject;
            }
        }
        return nil;
    } else if (@available(iOS 13.0, *)) {
        for (UIScene *scene in [UIApplication sharedApplication].connectedScenes) {
            if ([scene isKindOfClass:[UIWindowScene class]]) {
                UIWindowScene *windowScene = (UIWindowScene *)scene;
                if (windowScene.activationState == UISceneActivationStateForegroundActive) {
                    return windowScene.windows.firstObject;
                }
            }
        }
        return nil;
    } else {
        return [UIApplication sharedApplication].keyWindow;
    }
}
@end
static PKCEPresentationProvider *g_provider = nil;

void TriggerUnityApplicationOpenURL(const char* url) {
    NSString *urlStr = [NSString stringWithUTF8String:url];
    NSURL *callbackUrl = [NSURL URLWithString:urlStr];
    UnityAppController* controller = (UnityAppController*)[UIApplication sharedApplication].delegate;
    if ([controller respondsToSelector:@selector(application:openURL:options:)]) {
        [controller application:[UIApplication sharedApplication] openURL:callbackUrl options:@{}];
    }
}

extern "C" void LaunchWebAuthenticationServiceSession(const char* url, const char* redirectionScheme) {
    NSString *urlStr = [NSString stringWithUTF8String:url];
    NSString *redirectionSchemeStr = [NSString stringWithUTF8String:redirectionScheme];
    NSURL *authenticationURL = [NSURL URLWithString:urlStr];

    g_authenticationSession = nil;
    g_provider = nil;

    if (@available(iOS 13.0, *)) {
        // Use ASWebAuthenticationSession
        g_authenticationSession = [[ASWebAuthenticationSession alloc] initWithURL:authenticationURL
                                                     callbackURLScheme:redirectionSchemeStr
                                                     completionHandler:^(NSURL * _Nullable redirectionURL, NSError * _Nullable error) {
            if (redirectionURL) {
                NSLog(@"PKCE callback URL received: %@", redirectionURL.absoluteString);
                TriggerUnityApplicationOpenURL(redirectionURL.absoluteString.UTF8String);
            } else {
                NSLog(@"PKCE auth session error: %@", error);
            }
            g_authenticationSession = nil;
            g_provider = nil;
        }];
        g_provider = [PKCEPresentationProvider new];
        ((ASWebAuthenticationSession*)g_authenticationSession).presentationContextProvider = g_provider;
        [(ASWebAuthenticationSession*)g_authenticationSession start];
    }
    else if (@available(iOS 11.0, *)) {
        // Use SFAuthenticationSession
        g_authenticationSession = [[SFAuthenticationSession alloc] initWithURL:authenticationURL
                                                  callbackURLScheme:redirectionSchemeStr
                                                  completionHandler:^(NSURL * _Nullable redirectionURL, NSError * _Nullable error) {
            if (redirectionURL) {
                NSLog(@"PKCE callback URL received: %@", redirectionURL.absoluteString);
                TriggerUnityApplicationOpenURL(redirectionURL.absoluteString.UTF8String);
            } else {
                NSLog(@"PKCE auth session error: %@", error);
            }
            g_authenticationSession = nil;
        }];
        [(SFAuthenticationSession*)g_authenticationSession start];
    } else {
        NSLog(@"No supported authentication session (iOS < 11)");
    }
}
