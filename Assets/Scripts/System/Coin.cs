using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioSource audioSource;

    [Range(0.0f, 1f)] public float soundVolume;
    public int hitCount = 0;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (hitCount > 3)
        {
            return;
        }

        if (collision != null)
        {
            ContactPoint contact = collision.contacts[0];

            switch (collision.gameObject.layer)
            {
                case 0:
                    AudioSource.PlayClipAtPoint(audioSource.clip, contact.point, soundVolume * (10 - 3 * hitCount) * 0.1f);
                    hitCount++;
                    break;
            }
        }
    }
}
