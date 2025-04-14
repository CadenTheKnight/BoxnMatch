using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DamageableObject : MonoBehaviour
{
    [Header("Damage")]
    public float damageModifier = 1f;
    public float currentDamage = 0f;

    [Header("Lives")]
    public int startingLives = 3;
    private int currLives;

    [Header("DamageFX")]
    public bool isPlayer;
    public float distortionAmt = 1f;
    public float distortionDuration = 1f;
    public GameObject shrapnelPS;

    [Header("DeathExplosion")]
    public GameObject explosionPrefab;

    [Header("DamageText")]
    [SerializeField] private TMP_Text damageText;

    [Header("LifeCounter")]
    [SerializeField] private TMP_Text LifeCounter;

    private Rigidbody2D rb;

    private Material playerMat;
    private readonly int distortionKey = Shader.PropertyToID("_DistortionAmt");
    private readonly int fillColorKey = Shader.PropertyToID("_BoxColor");
    private Coroutine distortionRoutine;

    //events
    public delegate void DeathEvent(DamageableObject damage);
    public event DeathEvent OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (isPlayer) playerMat = GetComponent<SpriteRenderer>().material;

        currLives = startingLives;
        LifeCounter.text = "" + currLives;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        handleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        handleCollision(collision.gameObject);
    }

    private void handleCollision(GameObject go)
    {
        if (go.CompareTag("DamageObject"))
        {
            // Try to access the damage object (extra check in case something got miss labeled)
            DamageObject d;
            try
            {
                d = go.GetComponent<DamageObject>();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("----------------------------\nError: No DamageObject Script is attatched\n----------------------------");
                return;
            }

            // get damage and knockback from script on object
            Vector2 damageKnockback = d.getDamageKnockback();
            float damage = damageKnockback[0];
            float knockback = damageKnockback[1];

            currentDamage += damage;
            Vector2 collisionDirection = transform.position - go.transform.position;
            HandleKnockback(knockback, collisionDirection);

            if(isPlayer)
            {
                //distortion
                if(distortionRoutine != null) StopCoroutine(distortionRoutine);
                distortionRoutine = StartCoroutine(DistortionRoutine());

                //shrapnel particles
                GameObject shrapnel = Instantiate(shrapnelPS, transform.position, Quaternion.identity);
                ParticleSystem shrapPs = shrapnel.GetComponent<ParticleSystem>();
                var main = shrapPs.main;
                main.startColor = playerMat.GetColor(fillColorKey);
                shrapPs.Play();
                Destroy(shrapnel, 3f);
            }
        }

        //update damage text
        if(damageText != null) damageText.text = currentDamage + "%";
    }

    private void HandleKnockback(float knockback, Vector2 dir)
    {
        Vector2 knockbackVelocity = dir * (knockback * currentDamage / 100);
        rb.AddForce(knockbackVelocity, ForceMode2D.Impulse);
    }

    private IEnumerator DistortionRoutine()
    {
        float timer = 0f;
        while (timer < distortionDuration)
        {
            timer += Time.deltaTime;
            playerMat.SetFloat(distortionKey, distortionAmt * (1f - (timer/distortionDuration)));
            yield return null;
        }
    }

    public void ExplodeDie()
    {
        GameObject explo = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explo.GetComponent<ParticleSystem>().Play();
        Destroy(explo, 5f);
    }

    public void Die()
    {
        // Trigger Explosive death VFX
        ExplodeDie();

        OnDeath?.Invoke(this);

        // Reduce number of lifes
        //LifeCounter.GetComponent<LifeCounter>().LoseLife();

        //i coded a tiny thing for tracking lives in this script, not realizing this was partially
        //done already. whoops. oh well
        UpdateLifeCount(-1);
    }

    public void PermaDie()
    {
        Destroy(damageText);
        Destroy(LifeCounter);
        Destroy(gameObject);
    }

    public void UpdateLifeCount(int lifeDelta)
    {
        currLives += lifeDelta;
        LifeCounter.text = "" + currLives;
        if (currLives <= 0) PermaDie();
    }
    
    // Makes this object invincible for the set amount of time (seconds)
    public void MakeInvincible(float time)
    {
        StartCoroutine(HandleMakeInvincible(time));
    }
    public IEnumerator HandleMakeInvincible(float time)
    {
        float temp = damageModifier; // saves current modifier
        damageModifier = 0; // sets modifier to 0 to prevent damage

        yield return new WaitForSeconds(time);

        damageModifier = temp; // sets modifier back to original
    }

}
