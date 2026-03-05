using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace fireAttackVFXNameSpace
{

    public class fireBallScript : MonoBehaviour
    {
        private bool GotHit = false;

        // reference to the VFX to play on collision
        public VisualEffect vfxPrefab;

        // reference to the rigidbody for movement
        private Rigidbody rb;

        public bool isLaunched = false;

        // fireball movement variables
        public float speed = 2f;             // starting speed
        public float maxSpeed = 3f;          // max speed
        public float acceleration = 2f;      // acceleration over time
        public float rotationSpeed = 100f;   // visual spin

        // damage related variables
        public int bonusDamage = 0; // extra damage passed from the mage class

        void Start()
        {
            rb = GetComponent<Rigidbody>(); // get the Rigidbody component
        }

        void Update()
        {
            if (!isLaunched) return;

            // gradually increase speed over time
            if (speed < maxSpeed)
            {
                speed += acceleration * Time.deltaTime;
                speed = Mathf.Min(speed, maxSpeed); // clamp to maxSpeed
            }

            // apply force forward to push the fireball
            rb.AddForce(transform.forward * speed);

            // rotates fireball visually
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        // called when fireball hits something
        private void OnCollisionEnter(Collision collision)
        {
            if (GotHit == false)
            {
                // Check if the object hit is tagged as Enemy
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    baseEnemy enemy = collision.gameObject.GetComponent<baseEnemy>();
                    if (enemy != null)
                    {
                        float baseDamage = enemy.enemyHealth * 0.5f; // 50% of current enemy health
                        float totalDamage = baseDamage + bonusDamage;

                        enemy.dealDamage((int)totalDamage); // Convert to int if needed
                        Debug.Log(" Mage ultimate hit enemy for " + totalDamage + " damage.");
                    }
                    
                    verdaliaBossEnemy boss = collision.gameObject.GetComponentInParent<verdaliaBossEnemy>();
                    if (boss != null)
                    {
                        float baseDamage = boss.enemyHealth * 0.5f;
                        float totalDamage = baseDamage + bonusDamage;

                        boss.takeDamage(totalDamage);
                        Debug.Log("Mage fireball hit boss enemy for " + totalDamage + " damage.");
                    }
                }

                // Play impact VFX
                if (vfxPrefab != null)
                {
                    VisualEffect vfxInstance = Instantiate(vfxPrefab, collision.contacts[0].point, Quaternion.identity);
                    vfxInstance.SendEvent("OnPlay");
                    Destroy(vfxInstance.gameObject, 1f);
                }

                Destroy(this.gameObject);
                GotHit = true;
            }
        }
    }
}
