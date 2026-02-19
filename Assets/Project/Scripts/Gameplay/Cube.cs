using System.Runtime.CompilerServices;
using Core;
using GamePlay;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

namespace GamePlay
{
    [RequireComponent(typeof(Rigidbody))]
    public class Cube : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private int value = 2;
        [SerializeField]
        private TextMeshProUGUI[] valueTexts;

        private Rigidbody rb;

        private bool isLaunched;
        private bool hasMerged;
        [SerializeField]
        private float mergeImpulseThreshold = 2f;
        [SerializeField] private float directionThreshold = 0.5f;

        [SerializeField] private Renderer cubeRenderer;

        [SerializeField] private AudioClip mergeSound;

        private AudioSource audioSource;

        [SerializeField] private float moveSpeed = 15f;

        private float targetX;

        public static event System.Action<int> OnMerged;


        [System.Serializable]
        private struct ValueColor
        {
            public int value;
            public Color color;
        }

        [SerializeField] private ValueColor[] valueColors;

        public bool IsLaunched => this.isLaunched;
        public int Value => this.value;
        public Rigidbody Rigidbody => this.rb;


        private void Awake()
        {
            this.cubeRenderer.material = new Material(cubeRenderer.material);

            this.rb = GetComponent<Rigidbody>();

            this.audioSource = GetComponent<AudioSource>();

            if (this.audioSource == null)
            {
                this.audioSource = gameObject.AddComponent<AudioSource>();

            }

            if (this.valueTexts == null || this.valueTexts.Length == 0)
            {
                this.valueTexts = this.GetComponentsInChildren<TextMeshProUGUI>();
            }

            this.UpdateVisual();

        }

        private void Start()
        {

        }



        private void OnCollisionEnter(Collision collision)
        {
            if (this.hasMerged) return;

            Cube other = collision.collider.GetComponent<Cube>();

            if (other != null)
            {
                // РОЗБЛОКОВУЄМО обертання для обох кубів, 
                // щоб вони могли природно відскочити і покрутитися
                this.rb.constraints = RigidbodyConstraints.None;
                other.Rigidbody.constraints = RigidbodyConstraints.None;
            }

            if (other == null) return;
            if (other == this) return;
            if (other.hasMerged) return;

            if (this.value != other.value) return;
            if (!this.isLaunched || !other.isLaunched) return;

            float impulseMagnitude = collision.impulse.magnitude;

            if (impulseMagnitude < this.mergeImpulseThreshold)
                return;

            //  Перевірка напрямку
            Vector3 directionToOther = (other.transform.position - this.transform.position).normalized;

            float directionalDot = Vector3.Dot(collision.impulse.normalized, directionToOther);

            if (directionalDot < this.directionThreshold) return;

            this.Merge(other, collision.impulse);
        }


        private void Merge(Cube other, Vector3 collisionImpulse)
        {
            this.hasMerged = true;
            other.hasMerged = true;

            int newValue = this.value * 2;

            OnMerged?.Invoke(newValue);


            Vector3 spawnPosition = (this.transform.position + other.transform.position) / 2f;

            Cube newCube = Instantiate(this, spawnPosition, Quaternion.identity);
            newCube.SetValue(newValue);
            newCube.isLaunched = true;
            newCube.hasMerged = false;

            Rigidbody newRb = newCube.Rigidbody;

            // ---- 1. Успадковуємо горизонтальну швидкість ----
            Vector3 horizontalVelocity = (this.rb.linearVelocity + other.Rigidbody.linearVelocity) * 0.5f;

            horizontalVelocity.y = 0f;
            newRb.linearVelocity = horizontalVelocity;

            // ---- 2. Bounce залежить від сили удару ----
            float bounceForce = Mathf.Clamp(collisionImpulse.magnitude * 0.35f, 4f, 10f);

            newRb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);

            // ---- 3. Невелике обертання ----
            newRb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.Impulse);

            AudioSource.PlayClipAtPoint(this.mergeSound, spawnPosition);

            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }




        public void SetValue(int newvalue)
        {
            this.value = newvalue;
            this.UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (this.valueTexts != null)
            {
                foreach (var text in this.valueTexts)
                    text.text = this.value.ToString();
            }

            foreach (var vc in this.valueColors)
            {
                if (vc.value == this.value)
                {
                    this.cubeRenderer.material.color = vc.color;
                    break;
                }
            }
        }



        public void Launch(float force)
        {
            this.isLaunched = true;
            this.rb.AddForce(Vector3.forward * force, ForceMode.Impulse);

        }

        public void MoveHorizontal(float targetX)
        {
            Vector3 newPosition = new Vector3(targetX,this.rb.position.y,this.rb.position.z);

            this.rb.MovePosition(newPosition);
        }

    }
}

