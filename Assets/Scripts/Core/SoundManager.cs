using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace com.hive.projectr
{
    public class SoundPlayback
    {
        public AudioSource audioSource;
        public float playTime;
        public SoundConfigData configData;

        private Coroutine _routine;

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

        public void Play(Action onFinished)
        {
            if (audioSource != null)
            {
                audioSource.Play();

                ReleaseRoutine();

                _routine = MonoBehaviourUtil.Instance.StartCoroutine(PlayRoutine(onFinished));
            }
        }

        public void Pause()
        {
            if (audioSource != null)
            {
                audioSource.Pause();
            }

            ReleaseRoutine();
        }

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

    public class SoundManager : SingletonBase<SoundManager>, ICoreManager
    {
        private Dictionary<SoundType, AudioClip> _clipCacheDict;
        private Dictionary<SoundType, List<AudioSource>> _activeSourceDict;
        private Dictionary<SoundType, Queue<AudioSource>> _inactiveSourceDict;

        private List<SoundPlayback> _playbackRequests;
        private Dictionary<SoundType, List<SoundPlayback>> _activePlaybackDict;

        private Transform _sourceRoot;

        #region Lifecycle
        public void OnInit()
        {
            _sourceRoot = GameObject.FindGameObjectWithTag(TagNames.AudioSourceRoot).transform;
            _playbackRequests = new List<SoundPlayback>();
            _clipCacheDict = new Dictionary<SoundType, AudioClip>();
            _activeSourceDict = new Dictionary<SoundType, List<AudioSource>>();
            _inactiveSourceDict = new Dictionary<SoundType, Queue<AudioSource>>();
            _activePlaybackDict = new Dictionary<SoundType, List<SoundPlayback>>();

            MonoBehaviourUtil.OnUpdate += Tick;
        }

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
        }
        #endregion

        #region Event
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

        public void PlaySound(SoundType soundType, float delay = 0)
        {
            PlaySoundAndGetSource(soundType, delay);
        }

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
            var source = _sourceRoot.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.enabled = true;
            return source;
        }
    }
}