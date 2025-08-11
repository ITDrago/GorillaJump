using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Advertisement
{
    public class InterstitialAd : MonoBehaviour
    {
        private string _message = "";

        private InterstitialAdLoader _interstitialAdLoader;
        private Interstitial _interstitial;

        public void Awake()
        {
            _interstitialAdLoader = new InterstitialAdLoader();
            _interstitialAdLoader.OnAdLoaded += HandleAdLoaded;
            _interstitialAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
            
            RequestInterstitial();
        }

        private void RequestInterstitial()
        {
            MobileAds.SetAgeRestrictedUser(true);

            var adUnitId = "R-M-16382020-1";

            _interstitial?.Destroy();

            _interstitialAdLoader.LoadAd(CreateAdRequest(adUnitId));
            DisplayMessage("Interstitial is requested");
        }

        public void ShowInterstitial()
        {
            if (_interstitial == null)
            {
                DisplayMessage("Interstitial is not ready yet");
                return;
            }

            _interstitial.OnAdClicked += HandleAdClicked;
            _interstitial.OnAdShown += HandleAdShown;
            _interstitial.OnAdFailedToShow += HandleAdFailedToShow;
            _interstitial.OnAdImpression += HandleImpression;
            _interstitial.OnAdDismissed += HandleAdDismissed;

            _interstitial.Show();
        }

        private AdRequestConfiguration CreateAdRequest(string adUnitId) => new AdRequestConfiguration.Builder(adUnitId).Build();

        private void DisplayMessage(string message)
        {
            _message = message + (_message.Length == 0 ? "" : "\n--------\n" + _message);
            print(message);
        }

        #region Interstitial callback handlers
        public void HandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
        {
            DisplayMessage("HandleAdLoaded event received");

            _interstitial = args.Interstitial;
        }

        public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) => DisplayMessage($"HandleAdFailedToLoad event received with message: {args.Message}");

        public void HandleAdClicked(object sender, EventArgs args) => DisplayMessage("HandleAdClicked event received");

        public void HandleAdShown(object sender, EventArgs args) => DisplayMessage("HandleAdShown event received");

        public void HandleAdDismissed(object sender, EventArgs args)
        {
            DisplayMessage("HandleAdDismissed event received");

            _interstitial.Destroy();
            _interstitial = null;
            
            RequestInterstitial();
        }

        public void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            DisplayMessage($"HandleImpression event received with data: {data}");
        }

        public void HandleAdFailedToShow(object sender, AdFailureEventArgs args) => DisplayMessage($"HandleAdFailedToShow event received with message: {args.Message}");

        #endregion
    }
}