using System;
using Money;
using UnityEngine;
using UnityEngine.UI;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Advertisement
{
    public class RewardedAd : MonoBehaviour
    {
        [SerializeField] private Button _rewardedAdButton;
        
        private string _message = "";

        private RewardedAdLoader _rewardedAdLoader;
        private YandexMobileAds.RewardedAd _rewardedAd;
        
        private void OnEnable() => _rewardedAdButton.onClick.AddListener(ShowRewardedAd);
        private void OnDisable() => _rewardedAdButton.onClick.RemoveListener(ShowRewardedAd);
        
        public void Awake()
        {
            _rewardedAdLoader = new RewardedAdLoader();
            _rewardedAdLoader.OnAdLoaded += HandleAdLoaded;
            _rewardedAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;

            RequestRewardedAd();
        }

        private void RequestRewardedAd()
        {
            DisplayMessage("RewardedAd is not ready yet");
            MobileAds.SetAgeRestrictedUser(true);

            _rewardedAd?.Destroy();

            var adUnitId = "demo-rewarded-yandex";

            _rewardedAdLoader.LoadAd(CreateAdRequest(adUnitId));
            DisplayMessage("Rewarded Ad is requested");
        }

        private void ShowRewardedAd()
        {
            if (_rewardedAd == null)
            {
                DisplayMessage("RewardedAd is not ready yet");
                return;
            }

            _rewardedAd.OnAdClicked += HandleAdClicked;
            _rewardedAd.OnAdShown += HandleAdShown;
            _rewardedAd.OnAdFailedToShow += HandleAdFailedToShow;
            _rewardedAd.OnAdImpression += HandleImpression;
            _rewardedAd.OnAdDismissed += HandleAdDismissed;
            _rewardedAd.OnRewarded += HandleRewarded;

            _rewardedAd.Show();
        }

        private AdRequestConfiguration CreateAdRequest(string adUnitId) => new AdRequestConfiguration.Builder(adUnitId).Build();

        private void DisplayMessage(string message)
        {
            _message = message + (_message.Length == 0 ? "" : "\n--------\n" + _message);
            print(message);
        }

        #region Rewarded Ad callback handlers
        public void HandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
        {
            DisplayMessage("HandleAdLoaded event received");
            _rewardedAd = args.RewardedAd;
        }

        public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            DisplayMessage(
                $"HandleAdFailedToLoad event received with message: {args.Message}");
        }

        public void HandleAdClicked(object sender, EventArgs args) => DisplayMessage("HandleAdClicked event received");

        public void HandleAdShown(object sender, EventArgs args) => DisplayMessage("HandleAdShown event received");

        public void HandleAdDismissed(object sender, EventArgs args)
        {
            DisplayMessage("HandleAdDismissed event received");

            _rewardedAd.Destroy();
            _rewardedAd = null;
            
            RequestRewardedAd();
        }

        public void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            DisplayMessage($"HandleImpression event received with data: {data}");
        }

        public void HandleRewarded(object sender, Reward args)
        {
            DisplayMessage($"HandleRewarded event received: amout = {args.amount}, type = {args.type}");
            MoneySystem.Instance.AddMoney(args.amount);
        }

        public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            DisplayMessage(
                $"HandleAdFailedToShow event received with message: {args.Message}");
        }
        #endregion
    }
}