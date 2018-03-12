using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Syng {

    [RequireComponent(typeof(Animator))]
    public class PoliceCarBehaviour : MonoBehaviour {

        public MeshRenderer     glowLight;
        public MeshRenderer     glowDark;
        public ParticleSystem   exhaust;
        public float            sireneSpeed = 0.1f;
        public float            speed = 2f;
        public float            parkingTime = 1f;

        public UnityEvent       onDrive;
        public UnityEvent       onHonk;

        private Animator        animator;
        private float[]         xPositions;
        private bool            sireneOn;
        private bool            isDriving;

        void Awake() {
            isDriving = false;
            sireneOn = false;
            glowLight.enabled = false;
            glowDark.enabled = false;
            animator = GetComponent<Animator>();
            xPositions = new float[] {-5.75f, 0f, 17f};
        }

        public void Drive() {
            if (isDriving) {
                onHonk.Invoke();
                return;
            }
            isDriving = true;
            animator.SetBool("Drive", true);
            exhaust.Play();
            onDrive.Invoke();
            StartCoroutine(DriveCar());
            StartCoroutine(Sirene());
        }

        IEnumerator Sirene() {
            glowLight.enabled = false;
            glowDark.enabled = true;
            sireneOn = true;
            while (sireneOn) {
                yield return new WaitForSeconds(sireneSpeed);
                glowLight.enabled = !glowLight.enabled;
                glowDark.enabled = !glowDark.enabled;
            }
            glowLight.enabled = false;
            glowDark.enabled = false;
        }

        IEnumerator DriveCar() {
            while (transform.localPosition.x < xPositions[2]) {
                transform.localPosition += Vector3.right * Time.deltaTime * speed;
                yield return new WaitForSeconds(0);
            }

            transform.localPosition = Vector3.right * xPositions[2];
            exhaust.Stop();
            yield return new WaitForSeconds(parkingTime);

            transform.localPosition = Vector3.right * xPositions[0];
            Vector3 localScale = Vector3.one * 0.85f;
            transform.localScale = localScale;
            exhaust.Play();

            while (transform.localPosition.x <= xPositions[1]) {
                transform.localPosition += Vector3.right * Time.deltaTime * speed;
                transform.localScale = localScale + Vector3.one * (1 - (transform.localPosition.x / xPositions[0])) * 0.15f;
                yield return new WaitForSeconds(0);
            }

            transform.localPosition = Vector3.zero;
            exhaust.Stop();
            animator.SetBool("Drive", false);
            sireneOn = false;
            isDriving = false;
        }

        public void OnStartParticleSystem() {
            exhaust.Play();
        }

        public void OnStopParticleSystem() {
            exhaust.Stop();
        }

        public void OnStopAnimation() {
            animator.SetBool("Drive", false);
            exhaust.Stop();
        }
    }
}
