using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 0.4f; // Adjust in Inspector (0.3-0.5 works well)
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;

    private GameObject slashAnim;

    private PlayerControls playerControls;
    private Animator myAnimator;
    private PlayerController playerController;
    private ActivateWeapon activateWeapon;

    private float lastAttackTime;
    private bool canAttack = true;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        activateWeapon = GetComponentInParent<ActivateWeapon>();
        myAnimator = GetComponent<Animator>();
        playerControls = new PlayerControls();
    }

    private void OnEnable() => playerControls.Enable();
    private void OnDisable() => playerControls.Disable();

    void Start()
    {
        playerControls.Combat.Attack.started += _ => TryAttack();
    }

    private void Update()
    {
        MouseFollowWithOffset();

        // Simple cooldown timer
        if (!canAttack && Time.time - lastAttackTime > attackCooldown)
        {
            canAttack = true;
        }
    }

    private void TryAttack()
    {
        if (canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        myAnimator.SetTrigger("Attack");

        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
        slashAnim.transform.parent = this.transform.parent;
        lastAttackTime = Time.time;
        canAttack = false;
    }

    public void SwingUpFlipAnim()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (playerController.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    public void SwingDownFlipAnim()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (playerController.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    private void MouseFollowWithOffset()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(playerController.transform.position);

        Vector2 direction = mousePos - playerScreenPoint;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x)
        {
            angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
            activateWeapon.transform.localScale = new Vector3(-1, 1, 1);
            activateWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            activateWeapon.transform.localScale = Vector3.one;
            activateWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}