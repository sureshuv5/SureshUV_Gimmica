using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RewardTweener : MonoBehaviour
{
    [SerializeField]
    GameObject _rewardPrefab;

    [SerializeField]
    Transform _target;

    [SerializeField]
    GameObject _rewardUIContainer;

    [SerializeField]
    int _maxRewardsCount;
    Queue<GameObject> _rewardsqueue = new Queue<GameObject>();

    [SerializeField]
    float _minAnimDuration = 1.0f;

    [SerializeField]
    float _maxAnimDuration = 2.0f;

    [SerializeField]
    float spawnradius = 1.5f;

    [SerializeField]
    float _spread = 10f;

    [SerializeField]
    Ease _rewardEaseInType;

    [SerializeField]
    Ease _rewardEaseOutType;

    [SerializeField]
    float _prefabScale = 0.4f;

    [SerializeField]
    float _spawnDelay = 1.0f;

    [SerializeField]
    float _particleDelay = 0.3f;

    [SerializeField]
    float _overshootvalue = 1.0f;

    [SerializeField]
    float _uiShrinkFactor = 0.75f;

    [SerializeField]
    float _uiscaleduration = 0.5f;

    [SerializeField]
    Ease _uiScaleEaseType;

    [SerializeField]
    GameObject _rewardiconparticleprefab;

    [SerializeField]
    ParticleSystem _rewardparticlesystem;

    [SerializeField]
    float _prefabScalemultiplier = 1.5f;

    [SerializeField]
    float _prefabScaleInDelay = 0.5f;

    [SerializeField]
    float _prefabScaleOutDelay = 0.5f;

    [SerializeField]
    float _prefabScaleinDuration = 0.25f;

    [SerializeField]
    float _prefabScaleOutDuration = 0.25f;

    [SerializeField]
    Ease _prefabScaleEaseinType;

    [SerializeField]
    Ease _prefabScaleEaseOutType;

    [SerializeField]
    float _formationDuration = 0.4f;

    [SerializeField]
    AudioClip[] _rewardAudioClips;

    [SerializeField]
    AudioSource _rewardAudio;

    Vector3 targetpos;
    Vector3 _uiShrinkScale;
    Vector3 _uiNormalScale;

    // Start is called before the first frame update
    private void Awake()
    {
        PrepareRewards(_maxRewardsCount);
    }

    private void OnEnable()
    {
        if (_maxRewardsCount > _rewardsqueue.Count)
        {
            int _remainingCount = _maxRewardsCount - _rewardsqueue.Count;
            PrepareRewards(_remainingCount);
            StartCoroutine(AnimateRewards());
        }

        StartCoroutine(AnimateRewards());
    }

    void PrepareRewards(int _maxCount)
    {
        _uiNormalScale = _target.localScale;
        _uiShrinkScale = _uiNormalScale * _uiShrinkFactor;

        _rewardiconparticleprefab.SetActive(true);

        GameObject _reward;
        Vector3 prefabscale = new Vector3(_prefabScale, _prefabScale, _prefabScale);
        for (int i = 0; i < _maxCount; i++)
        {
            _reward = Instantiate(_rewardPrefab);
            _reward.transform.SetParent(transform);
            _reward.transform.localPosition = Vector3.zero;
            _reward.transform.localScale = prefabscale;
            _reward.SetActive(false);
            _rewardsqueue.Enqueue(_reward);
        }
    }

    IEnumerator AnimateRewards()
    {
        targetpos = _target.transform.position;
        WaitForSeconds wait = new WaitForSeconds(_spawnDelay);

        _rewardAudio.PlayOneShot(_rewardAudioClips[0]);

        for (int i = 0; i < _maxRewardsCount; i++)
        {
            if (_rewardsqueue.Count > 0)
            {
                GameObject _reward = _rewardsqueue.Dequeue();
                _reward.transform.localPosition = Vector3.zero;

                Vector2 _spreadvalue = new Vector2(Random.Range(-_spread, _spread), 0);
                Vector2 spawnpos = (Random.insideUnitCircle * spawnradius) + _spreadvalue;

                _reward.SetActive(true);

                float duration = Random.Range(_minAnimDuration, _maxAnimDuration);

                _reward
                    .transform.DOLocalMove(spawnpos, _formationDuration)
                    .SetEase(_rewardEaseInType)
                    .OnPlay(() =>
                    {
                        _reward
                            .transform.DOScale(
                                _prefabScale * _prefabScalemultiplier,
                                _prefabScaleinDuration
                            )
                            .SetDelay(_prefabScaleInDelay)
                            .SetEase(_prefabScaleEaseinType);
                    })
                    .OnComplete(() =>
                    {
                        _reward
                            .transform.DOMove(targetpos, duration)
                            .SetEase(_rewardEaseOutType, _overshootvalue)
                            .OnPlay(() =>
                            {
                                _reward
                                    .transform.DOScale(_prefabScale, _prefabScaleOutDuration)
                                    .SetDelay(_prefabScaleOutDelay)
                                    .SetEase(_prefabScaleEaseOutType);
                            })
                            .OnComplete(() =>
                            {
                                _rewardparticlesystem.Stop();

                                _rewardparticlesystem.Play();
                                _rewardUIContainer
                                    .transform.DOScale(_uiShrinkScale, _uiscaleduration)
                                    .SetEase(_uiScaleEaseType)
                                    .OnComplete(() =>
                                    {
                                        _rewardUIContainer
                                            .transform.DOScale(_uiNormalScale, _uiscaleduration)
                                            .SetEase(_uiScaleEaseType)
                                            .OnComplete(() => { });
                                    });
                                _reward.SetActive(false);
                                _rewardAudio.PlayOneShot(_rewardAudioClips[0]);

                                _rewardsqueue.Enqueue(_reward);
                            });
                    });
            }
            yield return wait;
            this.enabled = false;
        }
    }
}
