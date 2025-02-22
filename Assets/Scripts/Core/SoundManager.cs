using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace com.hive.projectr
{
    /// @ingroup Core
    /// @class SoundPlayback
    /// @brief Represents a sound playback instance, including the associated AudioSource, play time, and playback configuration.
    ///
    /// The `SoundPlayback` class is used to manage individual instances of sound playback. It stores the `AudioSource` that plays the sound,
    /// the play time of the sound, and the configuration data that determines how the sound should behave during playback. It allows for
    /// tracking and controlling each sound instance, ensuring the proper management of audio resources and behavior during playback.
    public class SoundPlayback
    {
        /// <summary>
        /// The AudioSource component responsible for playing the sound.
        /// </summary>
        public AudioSource audioSource;

        /// <summary>
        /// The time at which the sound will start playing.
        /// </summary>
        public float playTime;

        /// <summary>
        /// The configuration data associated with the sound, including its type and other playback settings.
        /// </summary>
        public SoundConfigData configData;

        private Coroutine _routine;

        /// <summary>
        /// Initializes a new instance of the SoundPlayback class.
        /// </summary>
        /// <param name="audioSource">The AudioSource that will play the sound.</param>
        /// <param name="playTime">The time when the sound should start playing.</param>
        /// <param name="configData">The configuration data that defines the sound's properties.</param>
        public SoundPlayback(AudioSource audioSource, float playTime, SoundConfigData configData)
        {
            Update(audioSource, playTime, configData);
        }

        public void Update(AudioSource audioSource, float playTime, SoundConfigData configData)
        {
            this.audioSource = audioSource;
            this.playTime = playTime;
            this.configData = configData;

            audioSource.loop = configData.IsLoop;
        }

        /// <summary>
        /// Starts the playback of the sound by setting the AudioSource's properties and playing it.
        /// </summary>
        /// <param name="onFinishCallback">The callback to invoke when the playback finishes.</param>
        public void Play(Action onFinished)
        {
            if (audioSource != null)
            {
                audioSource.Play();

                ReleaseRoutine();

                _routine = MonoBehaviourUtil.Instance.StartCoroutine(PlayRoutine(onFinished));
            }
        }

        /// <summary>
        /// Pauses the playback of the sound.
        /// </summary>
        public void Pause()
        {
            if (audioSource != null)
            {
                audioSource.Pause();
            }

            ReleaseRoutine();
        }

        /// <summary>
        /// Stops the playback of the sound.
        /// </summary>
        public void Stop()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            ReleaseRoutine();
        }

        private void ReleaseRoutine()
        {
            if (_routine != null)
            {
                if (MonoBehaviourUtil.Instance != null)
                {
                    MonoBehaviourUtil.Instance.StopCoroutine(_routine);
                }
                _routine = null;
            }
        }

        private IEnumerator PlayRoutine(Action onFinished)
        {
            if (audioSource.clip != null)
            {
                yield return new WaitUntil(() => audioSource.clip.length - audioSource.time < .01f);
            }

            onFinished?.Invoke();
        }
    }

    /// @ingroup Core
    /// @class SoundManager
    /// @brief Manages the playback of sound effects and music in the game, including handling sound sources, sound types, and mixer groups.
    /// 
    /// The `SoundManager` class is responsible for playing sound effects and managing audio resources in the game. It allows for playing
    /// sounds with different configurations, including looping sounds, one-time sounds, and adjusting sound volumes based on the
    /// application’s focus. It also manages active and inactive sound sources, caches audio clips, and integrates with Unity’s audio mixer.
    public class SoundManager : SingletonBase<SoundManager>, ICoreManager
    {
        // Dictionary to manage cached sound clips
        private Dictionary<SoundType, AudioClip> _clipCacheDict;

        // Active audio sources for each sound type
        private Dictionary<SoundType, List<AudioSource>> _activeSourceDict;

        // Inactive audio sources for each sound type
        private Dictionary<SoundType, Queue<AudioSource>> _inactiveSourceDict;
        
        // Pending sound playbacks that are scheduled to play
        private List<SoundPlayback> _playbackRequests;

        // Active sound playbacks, organized by sound type
        private Dictionary<SoundType, List<SoundPlayback>> _activePlaybackDict;

        // Audio mixer groups for background and effects
        private Dictionary<AudioMixerGroupType, AudioMixerGroup> _mixerGroupDict;

        // Root transform for audio sources
        private Transform _sourceRoot;

        // Main audio mixer for managing sound groups
        private AudioMixer _mainMixer;

        // Path to the main mixer resource
        private static readonly string MixerGroupResourcePath = "AudioMixers/Main/MainMixer";

        #region Lifecycle
        /// <summary>
        /// Initializes the SoundManager, setting up audio sources, mixers, and playback requests.
        /// </summary>
        public void OnInit()
        {
            _sourceRoot = GameObject.FindGameObjectWithTag(TagNames.AudioSourceRoot).transform;
            _playbackRequests = new List<SoundPlayback>();
            _clipCacheDict = new Dictionary<SoundType, AudioClip>();
            _activeSourceDict = new Dictionary<SoundType, List<AudioSource>>();
            _inactiveSourceDict = new Dictionary<SoundType, Queue<AudioSource>>();
            _activePlaybackDict = new Dictionary<SoundType, List<SoundPlayback>>();
            _mixerGroupDict = new Dictionary<AudioMixerGroupType, AudioMixerGroup>();

            _mainMixer = Resources.Load<AudioMixer>(MixerGroupResourcePath);

            MonoBehaviourUtil.OnUpdate += Tick;
            MonoBehaviourUtil.OnApplicationFocusLost += OnFocusLost;
            MonoBehaviourUtil.OnApplicationFocusBack += OnFocusBack;
        }

        /// <summary>
        /// Disposes of the SoundManager, cleaning up audio sources and other resources.
        /// </summary>
        public void OnDispose()
        {
            _sourceRoot = null;

            foreach (var playback in _playbackRequests)
            {
                StopPlayback(playback);
            }
            _playbackRequests.Clear();
            _playbackRequests = null;

            foreach (var playbacks in _activePlaybackDict.Values)
            {
                foreach (var playback in playbacks)
                {
                    StopPlayback(playback);
                }
            }
            _activePlaybackDict.Clear();
            _activePlaybackDict = null;

            _mixerGroupDict.Clear();
            _mixerGroupDict = null;

            foreach (var clip in _clipCacheDict.Values)
            {
                Addressables.Release(clip);
            }
            _clipCacheDict.Clear();
            _clipCacheDict = null;

            _activeSourceDict.Clear();
            _activeSourceDict = null;

            _inactiveSourceDict.Clear();
            _inactiveSourceDict = null;

            MonoBehaviourUtil.OnUpdate -= Tick;
            MonoBehaviourUtil.OnApplicationFocusLost -= OnFocusLost;
            MonoBehaviourUtil.OnApplicationFocusBack -= OnFocusBack;
        }
        #endregion

        #region Event
        /// <summary>
        /// Handles focus regained event, restoring background volume.
        /// </summary>
        private void OnFocusBack()
        {
            var volume = 1f;
            SetAudioMixerGroupVolume(AudioMixerGroupType.Background, volume);
        }

        /// <summary>
        /// Handles focus lost event, reducing background volume.
        /// </summary>
        private void OnFocusLost()
        {
            var volume = GameGeneralConfig.GetData().SoundVolumePercentageWhenInBackground / 100f;
            SetAudioMixerGroupVolume(AudioMixerGroupType.Background, volume);
        }

        /// <summary>
        /// Main update method called each frame. It handles pending playback requests.
        /// </summary>
        private void Tick()
        {
            if (_playbackRequests.Count > 0)
            {
                for (var i = _playbackRequests.Count - 1; i >= 0; --i)
                {
                    var playback = _playbackRequests[i];
                    if (Time.time >= playback.playTime)
                    {
                        playback.Play(() => OnPlaybackFinished(playback));
                        _playbackRequests.RemoveAt(i);

                        // add to active list
                        var soundType = playback.configData.Type;
                        if (!_activePlaybackDict.TryGetValue(soundType, out var activePlaybacks))
                        {
                            activePlaybacks = new List<SoundPlayback>();
                            _activePlaybackDict[soundType] = activePlaybacks;
                        }
                        activePlaybacks.Add(playback);
                    }
                }
            }
        }

        private void OnPlaybackFinished(SoundPlayback playback)
        {
            if (!playback.audioSource.loop)
            {
                StopPlayback(playback);
            }
        }
        #endregion

        #region Sound Playback Management
        /// <summary>
        /// Retrieves an audio mixer group for a given type.
        /// </summary>
        private AudioMixerGroup GetAudioMixerGroup(AudioMixerGroupType type)
        {
            if (_mixerGroupDict.TryGetValue(type, out var group))
            {
                return group;
            }

            if (_mainMixer != null)
            {
                var groups = _mainMixer.FindMatchingGroups(type.ToString());
                if (groups != null && groups.Length > 0)
                {
                    group = groups[0];
                    _mixerGroupDict[type] = group;

                    return group;
                }
            }
            else
            {
                Logger.LogError($"_mainMixer is null");
            }

            Logger.LogError($"Failed to get AudioMixerGroup of type: {type}");

            return group;
        }

        /// <summary>
        /// Sets the volume of a specified audio mixer group.
        /// </summary>
        private void SetAudioMixerGroupVolume(AudioMixerGroupType groupType, float volume)
        {
            var group = GetAudioMixerGroup(groupType);
            if (group != null)
            {
                var db = volume > 0
                    ? 20 * Mathf.Log10(volume)
                    : -80;

                _mainMixer.SetFloat($"{group.name}Volume", db);
            }
        }

        private void StopPlayback(SoundPlayback playback)
        {
            playback.Stop();

            // put source back
            if (!_activeSourceDict.TryGetValue(playback.configData.Type, out var activeSources))
            {
                activeSources = new List<AudioSource>();
                _activeSourceDict[playback.configData.Type] = activeSources;
            }
            for (var i = 0; i < activeSources.Count; ++i)
            {
                if (activeSources[i] == playback.audioSource)
                {
                    activeSources.RemoveAt(i);
                    break;
                }
            }

            if (!_inactiveSourceDict.TryGetValue(playback.configData.Type, out var inactiveSources))
            {
                inactiveSources = new Queue<AudioSource>();
                _inactiveSourceDict[playback.configData.Type] = inactiveSources;
            }
            inactiveSources.Enqueue(playback.audioSource);

            if (!playback.configData.NeedCache)
            {
                var soundType = playback.configData.Type;
                if (_clipCacheDict.TryGetValue(soundType, out var clip))
                {
                    Addressables.Release(clip);
                    _clipCacheDict.Remove(soundType);
                }
            }
        }
        #endregion

        #region Sound Playback API
        /// <summary>
        /// Plays a sound of the specified type.
        /// </summary>
        public void PlaySound(SoundType soundType, float delay = 0)
        {
            PlaySoundAndGetSource(soundType, delay);
        }

        private void AssignAudioSourceToAudioMixerGroup(AudioSource source, AudioMixerGroupType groupType)
        {
            if (source == null)
                return;

            var mixerGroup = GetAudioMixerGroup(groupType);
            if (mixerGroup != null)
            {
                source.outputAudioMixerGroup = mixerGroup;
            }
        }

        /// <summary>
        /// Plays a sound and returns the AudioSource used to play it.
        /// </summary>
        public async Task<AudioSource> PlaySoundAndGetSource(SoundType soundType, float delay = 0)
        {
            var soundData = SoundConfig.GetData(soundType);
            if (soundData == null)
            {
                Logger.LogError($"Invalid sound type: {soundType}");
                return null;
            }

            if (!_clipCacheDict.TryGetValue(soundType, out var clip))
            {
                var handle = Addressables.LoadAssetAsync<AudioClip>(soundData.Clip);
                await handle.Task;
                clip = handle.Result;
                _clipCacheDict[soundType] = clip;
            }

            AudioSource source = null;

            if (soundData != null)
            {
                var wasActiveSource = false;
                var needNewPlayback = false;

                if (_inactiveSourceDict.TryGetValue(soundData.Type, out var inactiveSources) && inactiveSources.Count > 0)
                {
                    source = inactiveSources.Dequeue();
                    wasActiveSource = false;
                    needNewPlayback = true;
                }
                else
                {
                    if (!_activeSourceDict.TryGetValue(soundData.Type, out var activeSources))
                    {
                        source = CreateSource();

                        activeSources = new List<AudioSource>
                        {
                            source
                        };
                        _activeSourceDict[soundData.Type] = activeSources;

                        wasActiveSource = false;
                        needNewPlayback = true;
                    }
                    else
                    {
                        switch (soundData.PlayType)
                        {
                            case SoundPlayType.Multiple:
                                source = CreateSource();
                                wasActiveSource = false;
                                needNewPlayback = true;
                                break;
                            case SoundPlayType.SingleKeep:
                                source = activeSources[0];
                                wasActiveSource = true;
                                needNewPlayback = false;
                                break;
                            case SoundPlayType.SingleRestart:
                                source = activeSources[0];
                                wasActiveSource = true;
                                needNewPlayback = true;
                                break;
                            default:
                                source = CreateSource();
                                wasActiveSource = false;
                                needNewPlayback = true;
                                Logger.LogError($"Undefined play type: {soundData.PlayType}");
                                break;
                        }
                    }
                }

                if (source != null)
                {
                    AssignAudioSourceToAudioMixerGroup(source, soundData.MixerGroup);

                    source.clip = clip;
                    source.pitch = 1;

                    if (!wasActiveSource)
                    {
                        if (!_activeSourceDict.TryGetValue(soundData.Type, out var activeSources))
                        {
                            activeSources = new List<AudioSource>();
                            _activeSourceDict[soundData.Type] = activeSources;
                        }
                        activeSources.Add(source);
                    }

                    if (needNewPlayback)
                    {
                        AddPlayback(source, delay, soundData);
                    }
                }
                else
                {
                    Logger.LogError($"Failed to get a valid AudioSource with sound type: {soundData.Type}!");
                }
            }

            return source;
        }

        public void StopAllSound()
        {
            foreach (var list in _activePlaybackDict.Values)
            {
                foreach (var playback in list)
                {
                    StopPlayback(playback);
                }
            }

            _activePlaybackDict.Clear();
        }

        public void StopSound(SoundType soundType)
        {
            if (_activePlaybackDict.TryGetValue(soundType, out var activePlaybacks))
            {
                foreach (var playback in activePlaybacks)
                {
                    StopPlayback(playback);
                }

                _activePlaybackDict.Remove(soundType);
            }
        }

        private void AddPlayback(AudioSource source, float delay, SoundConfigData configData)
        {
            _playbackRequests.Add(new SoundPlayback(source, Time.time + delay, configData));
        }

        private AudioSource CreateSource()
        {
            var source = _sourceRoot.gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.enabled = true;
            return source;
        }
        #endregion
    }
}