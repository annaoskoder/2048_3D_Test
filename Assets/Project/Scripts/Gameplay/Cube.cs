using System.Runtime.CompilerServices;
using Core;
using GamePlay;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

namespace GamePlay
{
    /// <summary>
    /// Represents a single interactable cube in the game that can be launched and merged with others of the same value.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class Cube : MonoBehaviour
    {
        [Header("Core Settings")]
        [Tooltip("The current numerical value of the cube (e.g., 2, 4, 8).")]
        [SerializeField]
        private int value = 2;

        [Space(10)]
        [Header("Merge Physics Settings")]
        [Tooltip("Minimum collision impulse magnitude required to trigger a merge.")]
        [Range(0.1f, 10f)]
        [SerializeField]
        private float mergeImpulseThreshold = 2f;

        [Tooltip("Threshold for the directional dot product to ensure cubes are moving towards each other during a collision.")]
        [Range(-1f, 1f)]
        [SerializeField]
        private float directionThreshold = 0.5f;

        [Space(10)]
        [Header("Visuals & References")]
        [Tooltip("Array of TextMeshPro elements displaying the cube's value on its faces.")]
        [SerializeField]
        private TextMeshProUGUI[] valueTexts;

        [Tooltip("Renderer used to dynamically change the cube's material color.")]
        [SerializeField]
        private Renderer cubeRenderer;

        [Tooltip("Array mapping specific numerical values to distinct colors.")]
        [SerializeField]
        private ValueColor[] valueColors;

        [Space(10)]
        [Header("Audio")]
        [Tooltip("Sound effect played when two cubes successfully merge.")]
        [SerializeField]
        private AudioClip mergeSound;

        // Private components and state variables
        private Rigidbody rb;
        private AudioSource audioSource;
        private bool isLaunched;
        private bool hasMerged;

        /// <summary>
        /// Event triggered when two cubes merge. Passes the newly created value.
        /// </summary>
        public static event System.Action<int> OnMerged;

        [System.Serializable]
        private struct ValueColor
        {
            public int value;
            public Color color;
        }

        

        public bool IsLaunched => this.isLaunched;
        public int Value => this.value;
        public Rigidbody Rigidbody => this.rb;


        private void Awake()
        {
            // Create a material instance so changing color doesn't affect all cubes
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


        private void OnCollisionEnter(Collision collision)
        {
            if (this.hasMerged) return;

            Cube other = collision.collider.GetComponent<Cube>();

            if (other != null)
            {
                // UNLOCK rotation for both cubes 
                // so they can bounce and spin naturally after collision
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

            // Directional check to ensure valid merge impact
            Vector3 directionToOther = (other.transform.position - this.transform.position).normalized;
            float directionalDot = Vector3.Dot(collision.impulse.normalized, directionToOther);

            if (directionalDot < this.directionThreshold) return;

            this.Merge(other, collision.impulse);
        }

        /// <summary>
        /// Handles the logic for merging two cubes, spawning a new one, and applying physics forces.
        /// </summary>
        /// <param name="other">The cube being merged with.</param>
        /// <param name="collisionImpulse">The impulse from the collision to calculate bounce.</param>
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

            // Inherit horizontal velocity
            Vector3 horizontalVelocity = (this.rb.linearVelocity + other.Rigidbody.linearVelocity) * 0.5f;

            horizontalVelocity.y = 0f;
            newRb.linearVelocity = horizontalVelocity;

            //Bounce depends on impact force
            float bounceForce = Mathf.Clamp(collisionImpulse.magnitude * 0.35f, 4f, 10f);

            newRb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);

            //Slight rotation
            newRb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.Impulse);

            AudioSource.PlayClipAtPoint(this.mergeSound, spawnPosition);

            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }



        /// <summary>
        /// Updates the cube's mathematical value and refreshes its visual representation.
        /// </summary>
        /// <param name="newvalue">The new numerical value to assign.</param>
        public void SetValue(int newvalue)
        {
            this.value = newvalue;
            this.UpdateVisual();
        }

        /// <summary>
        /// Refreshes the TextMeshPro texts and the material color based on the current value.
        /// </summary>
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



        /// <summary>
        /// Launches the cube forward with a specified physical force.
        /// </summary>
        /// <param name="force">The impulse force applied to the Z-axis.</param>
        public void Launch(float force)
        {
            this.isLaunched = true;
            this.rb.AddForce(Vector3.forward * force, ForceMode.Impulse);

        }

        /// <summary>
        /// Moves the cube horizontally to a specific X coordinate while maintaining its Y and Z positions.
        /// </summary>
        /// <param name="targetX">The target X-axis position.</param>
        public void MoveHorizontal(float targetX)
        {
            Vector3 newPosition = new Vector3(targetX,this.rb.position.y,this.rb.position.z);

            this.rb.MovePosition(newPosition);
        }

    }
}

