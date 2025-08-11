using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Advertisement
{
    public class BannerAd : MonoBehaviour
    {
        private string _message = "";

        private Banner _banner;

        private void OnEnable() => RequestBanner();

        private void OnDestroy() => _banner?.Destroy();

        private void RequestBanner()
        {
            MobileAds.SetAgeRestrictedUser(true);

            var adUnitId = "R-M-16382020-3";

            _banner?.Destroy();
            // Set sticky banner width
            var bannerSize = BannerAdSize.StickySize(GetScreenWidthDp());
            // Or set inline banner maximum width and height
            // BannerAdSize bannerSize = BannerAdSize.InlineSize(GetScreenWidthDp(), 300);
            _banner = new Banner(adUnitId, bannerSize, AdPosition.BottomCenter);

            _banner.OnAdLoaded += HandleAdLoaded;
            _banner.OnAdFailedToLoad += HandleAdFailedToLoad;
            _banner.OnReturnedToApplication += HandleReturnedToApplication;
            _banner.OnLeftApplication += HandleLeftApplication;
            _banner.OnAdClicked += HandleAdClicked;
            _banner.OnImpression += HandleImpression;

            _banner.LoadAd(CreateAdRequest());
            DisplayMessage("Banner is requested");
        }

        private int GetScreenWidthDp()
        {
            var screenWidth = (int)Screen.safeArea.width;
            return ScreenUtils.ConvertPixelsToDp(screenWidth);
        }

        private AdRequest CreateAdRequest() => new AdRequest.Builder().Build();

        private void DisplayMessage(string message)
        {
            _message = message + (_message.Length == 0 ? "" : "\n--------\n" + _message);
            print(message);
        }

        #region Banner callback handlers
        public void HandleAdLoaded(object sender, EventArgs args)
        {
            DisplayMessage("HandleAdLoaded event received");
            _banner.Show();
        }

        public void HandleAdFailedToLoad(object sender, AdFailureEventArgs args) => DisplayMessage("HandleAdFailedToLoad event received with message: " + args.Message);

        public void HandleLeftApplication(object sender, EventArgs args) => DisplayMessage("HandleLeftApplication event received");

        public void HandleReturnedToApplication(object sender, EventArgs args) => DisplayMessage("HandleReturnedToApplication event received");

        public void HandleAdLeftApplication(object sender, EventArgs args) => DisplayMessage("HandleAdLeftApplication event received");

        public void HandleAdClicked(object sender, EventArgs args) => DisplayMessage("HandleAdClicked event received");

        public void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            DisplayMessage("HandleImpression event received with data: " + data);
        }
        #endregion
    }
}