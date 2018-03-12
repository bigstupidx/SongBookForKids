using UnityEngine;
using UnityEngine.Analytics;
using Mandarin;
using Mandarin.PluginSystem;
using System.Collections.Generic;
using PaperPlaneTools;

namespace Syng {

	public class MShowRatingPopup : IMessage {}

	[RegisterPlugin]
	public class RatingPlugin : MonoBehaviour, IPlugin {

		private int languageId;

		public void Init(Messenger msg) {
			msg
				.AddListener<MShowRatingPopup>(OnShowRatingPopup)
				.AddListener<MLanguageLoaded>(OnLanguageLoaded);
		}

		public void Ready(Messenger msg) {
			RateBox.Instance.Init (
				RateBox.GetStoreUrl ("1151798674", Application.identifier),
				new RateBoxConditions {
					MinSessionCount = 0,
					MinCustomEventsCount = 5,
					DelayAfterInstallInSeconds = 0,
					DelayAfterLaunchInSeconds = 0,
					PostponeCooldownInSeconds = 24 * 3600,
					RequireInternetConnection = true,
				},
				null, 
				new RateBoxSettings () {
					UseIOSReview = true
				}
			);

            OnApplicationPause(false);
		}

		private void OnShowRatingPopup(MShowRatingPopup showRatingPopup) {
			switch (languageId) {
			case 0:
				RateBox.Instance.ForceShow(
					"Liker du SYNG?",
					"Vi setter pris på en hyggelig vurdering!",
					"Vurder",
					"Senere"
				);
				break;
			default:
				RateBox.Instance.ForceShow(
					"Enjoying SING?",
					"Please take a moment to rate it!",
					"Rate",
					"Later"
				);
				break;
			}
		}

        public void OnApplicationPause(bool pauseStatus) {
            if (!pauseStatus) {
                RateBox.Instance.IncrementCustomCounter();

                int ratingPopupCount = PlayerPrefs.GetInt("ratingPopupCount", 0);

                if (RateBox.Instance.CheckConditionsAreMet()) {
                    ratingPopupCount++;
                    PlayerPrefs.SetInt("ratingPopupCount", ratingPopupCount);
                    Debug.Log(string.Format("Rating popups: {0}", ratingPopupCount));
                }

                switch (languageId) {
                case 0:
                    RateBox.Instance.Show(
                        "Liker du SYNG?",
                        "Vi setter pris på en hyggelig vurdering!",
                        "Vurder",
                        "Senere",
                        ratingPopupCount > 3 ? "Ikke spør igjen" : null
                    );
                    break;
                default:
                    RateBox.Instance.Show(
                        "Enjoying SING?",
                        "Please take a moment to rate it!",
                        "Rate",
                        "Later",
                        ratingPopupCount > 3 ? "Stop asking" : null
                    );
                    break;
                }
            }
        }

		private void OnLanguageLoaded(MLanguageLoaded languageLoaded) {
			languageId = languageLoaded.language;
		}
	}
}
