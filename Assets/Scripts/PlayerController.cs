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
    Vector3 cameraParentLocalPosOrg;
    Vector3 cameraParentPosOrg;
    Tween moveTween;
    Tween afkTween;
    bool isMoving = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        movementAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");

        xRot = cameraParent.localEulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        cameraParentLocalPosOrg = cameraParent.localPosition;
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
        LookWithSway();
        if(movementV2 != Vector2.zero)
        {
            if (!isMoving)
            {
                moveTween = cameraParent.DOPunchPosition(Vector3.up * Random.Range(0.15f, 0.25f), 1.25f, 2, default, false).SetLoops(-1).SetEase(Ease.OutSine).Play();
                Debug.Log("Started Moving Camera: " + moveTween.IsPlaying());
                isMoving = true;
            }
            NotAFK();
        }else
        {
            if (moveTween.IsActive())
            {
                moveTween.Kill();
                //cameraParent.localPosition = cameraParentLocalPosOrg;
                cameraParent.DOLocalMove(cameraParentLocalPosOrg, 0.25f).SetEase(Ease.OutSine);
                Debug.Log("Moved camera to org pos: " + cameraParentLocalPosOrg);
            }
            isMoving = false;
        }

    }
    void LookWithSway()
    {
        afkTimer -= Time.deltaTime;
        if (HandleLook() == Vector2.zero)
        {
            if (!afkTween.IsActive() && afkTimer < 0){
                Vector3 targetPosition = cameraParentPosOrg + Random.insideUnitSphere * sphereradius;
                afkTween = cameraParent.DOMove(new(targetPosition.x,targetPosition.y,cameraParent.position.z), 2f).SetEase(Ease.Linear);
            }
        }
        else
        {
            NotAFK();
        }

    }
    void NotAFK()
    {
        afkTween.Kill();
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
