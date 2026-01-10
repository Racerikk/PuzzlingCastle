using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    InputAction movementAction;
    InputAction lookAction;
    Rigidbody rb;

    [SerializeField] Transform cameraParent;
    [SerializeField] float speed = 5f;
    [SerializeField] float xSensitivity = 1f;
    [SerializeField] float ySensitivity = 1f;

    float xRot = 0f;
    float afkTimerValue = 2f;
    float afkTimer = 2f;
    float sphereradius = .05f;
    Vector3 cameraParentPosOrg;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        movementAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");

        xRot = cameraParent.localEulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        cameraParentPosOrg = cameraParent.position;
    }

    void Update()
    {
        if(GameManager.IsGamePaused) 
        {
            return;
        }
        Vector2 movementV2 = movementAction.ReadValue<Vector2>();
        Vector3 movement = new(movementV2.x, 0, movementV2.y);
        Vector3 dir = (transform.rotation * movement).normalized;
        movement.x = dir.x * speed;
        movement.z = dir.z * speed;
        rb.linearVelocity = movement;
        if(movementV2 != Vector2.zero)
        {
            NotAFK();
        }
        
        LookWithSway();
    }
    void LookWithSway()
    {
        afkTimer -= Time.deltaTime;
        if (HandleLook() == Vector2.zero)
        {
            if (!DOTween.IsTweening(cameraParent) && afkTimer < 0){
                Vector3 targetPosition = cameraParentPosOrg + Random.insideUnitSphere * sphereradius;
                cameraParent.DOMove(new(targetPosition.x,targetPosition.y,cameraParent.position.z), 2f).SetEase(Ease.Linear);
            }
        }
        else
        {
            NotAFK();
        }

    }
    void NotAFK()
    {
        if(afkTimer < 0)
            cameraParent.position = cameraParentPosOrg;
        cameraParent.DOKill();
        transform.DOKill();
        afkTimer = afkTimerValue;
        cameraParentPosOrg = cameraParent.position;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(cameraParentPosOrg, sphereradius); 
    }
    Vector2 HandleLook()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();
        transform.Rotate(Vector3.up * look.x * xSensitivity);

        float yRot = -look.y * ySensitivity;
        xRot += yRot;
        xRot = Mathf.Clamp(xRot, -70f, 70f);
        cameraParent.localEulerAngles = new Vector3(xRot, 0, 0);
        return look;
    }
}
