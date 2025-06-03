using UnityEngine;
using Universal.Runtime.Systems.Audio;

namespace Universal.Runtime.Systems.AmbientWeather
{
    public class WeatherAudio : MonoBehaviour
    {
        [SerializeField, InLineEditor] AudioClip rainAudio;
        [SerializeField, InLineEditor] AudioClip windAudio;

        public void UpdateAudio(WeatherState state)
        {
            switch (state)
            {
                case WeatherState.Rainy:
                    MusicManager.Instance.AddToPlaylist(rainAudio);
                    break;
                case WeatherState.Stormy:
                    MusicManager.Instance.AddToPlaylist(windAudio);
                    break;
            }
        }
    }
}