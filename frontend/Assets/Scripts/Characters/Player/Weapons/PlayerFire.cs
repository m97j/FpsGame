using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    public GameObject firePosition;
    public GameObject bombFactory;
    public float throwPower = 15f;
    public GameObject bulletEffect;
    ParticleSystem ps;
    public int weaponPower = 5;
    Animator anim;

    enum WeaponMode
    {
        Normal,
        Sniper,
        Shotgun,
        Grenade,
        Rocket,
        Laser,
        Max
    }

    WeaponMode wMode;
    bool ZoomMode = false;

    public Text weaponModeText;
    public GameObject[] eff_Flash;

    void Start()
    {
        ps = bulletEffect.GetComponent<ParticleSystem>();
        anim = GetComponentInChildren<Animator>();
        wMode = WeaponMode.Normal;
    }

    void Update()
    {
        if (GameManager.gm.gState != GameManager.GameState.Run)
            return;

        // 우클릭 (보조 무기/줌)
        if (Input.GetMouseButtonDown(1))
        {
            switch (wMode)
            {
                case WeaponMode.Normal:
                    // Simplified 'new' expressions to address IDE0090 diagnostic code
                    var bomb = Instantiate(bombFactory);
                    bomb.transform.position = firePosition.transform.position;

                    var rb = bomb.GetComponent<Rigidbody>();
                    rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
                    break;

                case WeaponMode.Sniper:
                    if (ZoomMode)
                    {
                        Camera.main.fieldOfView = 60f;
                        ZoomMode = false;
                    }
                    else
                    {
                        Camera.main.fieldOfView = 15f;
                        ZoomMode = true;
                    }
                    break;
            }
        }

        // 좌클릭 (공격)
        if (Input.GetMouseButtonDown(0))
        {
            if (anim.GetFloat("MoveMotion") == 0)
                anim.SetTrigger("Attack");

            var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out var hitinfo))
            {
                // === B 방식 적용 부분 ===
                if (hitinfo.transform.TryGetComponent<IDamageable>(out var target))
                {
                    target.TakeDamage(weaponPower);
                }
                else
                {
                    // 피격 이펙트 처리
                    bulletEffect.transform.position = hitinfo.point;
                    bulletEffect.transform.forward = hitinfo.normal;
                    ps.Play();
                }
            }
            StartCoroutine(ShootEffectOn(0.05f));
        }

        // 무기 모드 전환
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            wMode = WeaponMode.Normal;
            Camera.main.fieldOfView = 60f;
            weaponModeText.text = "Normal Mode";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            wMode = WeaponMode.Sniper;
            weaponModeText.text = "Sniper Mode";
        }
        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    wMode = WeaponMode.Shotgun;
        //    Debug.Log("무기 모드: Shotgun");
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    wMode = WeaponMode.Grenade;
        //    Debug.Log("무기 모드: Grenade");
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    wMode = WeaponMode.Rocket;
        //    Debug.Log("무기 모드: Rocket");
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    wMode = WeaponMode.Laser;
        //    Debug.Log("무기 모드: Laser");
        //}
    }

    IEnumerator ShootEffectOn(float duration)
    {
        int num = Random.Range(0, eff_Flash.Length - 1);
        eff_Flash[num].SetActive(true);
        yield return new WaitForSeconds(duration);
        eff_Flash[num].SetActive(false);
    }
}
