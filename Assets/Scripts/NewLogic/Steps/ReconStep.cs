    using System;
    using System.Collections;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ReconStep", menuName = "SceneSteps/Recon")]

    public class ReconStep : ScriptPartSO
    {
        public AudioClip audioClip;

        public override IEnumerator WaitForCompletion()
        {
            OnStart?.Invoke();
            
            var gm = SceneGameManager.Instance;
            var audioSource = gm.narrationAudioSource;

            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            
            yield return new WaitForSeconds(audioClip.length);
            
            if (gm.reconObjects.Length > 0)
            {
                GameObject currentReconObject = gm.reconObjects[gm.indexRecon];
                currentReconObject.SetActive(true);

                var notifier = currentReconObject.GetComponent<ReconTriggers>();
                bool triggered = false;

                if (notifier != null)
                {
                    Action handler = () => triggered = true;
                    notifier.OnTriggered += handler;

                    float timer = 0f;
                    float timeout = 10f;

                    while (!triggered && timer < timeout)
                    {
                        timer += Time.deltaTime;
                        yield return null;
                    }

                    notifier.OnTriggered -= handler;
                }
                else
                {
                    Debug.LogWarning("[RECON] El objeto no tiene ReconTriggerNotifier");
                }

                // Desactivar luego de usarse
                currentReconObject.SetActive(false);
                gm.indexRecon++;
            }

            if (extraDelay > 0)
                yield return new WaitForSeconds(extraDelay);
            
            OnEnd?.Invoke();
        }
    }
