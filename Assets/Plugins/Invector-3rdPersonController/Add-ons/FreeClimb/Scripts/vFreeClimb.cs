using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Invector.vCharacterController.vActions
{
    [vClassHeader("FREE CLIMB ADD-ON", "Make sure the mesh you want to climb is assigned with the 'FreeClimb' Tag", iconName = "climbIcon")]
    public class vFreeClimb : vMonoBehaviour
    {
        #region Public variables

        public bool autoClimbEdge = true;
        public GenericInput climbEdgeInput = new GenericInput("E", "A", "A");
        public GenericInput enterExitInput = new GenericInput("Space", "X", "X");
        public GenericInput climbJumpInput = new GenericInput("Space", "X", "X");
        public string cameraState = "Default";
        public float climbStaminaCost = 0f;
        public float jumpClimbStaminaCost = 0f;
        public float staminaRecoveryDelay = 1.5f;

        [Range(0, 180)]
        public float minSurfaceAngle = 30, maxSurfaceAngle = 160;
        [Tooltip("Empty GameObject Child of Character\nPosition this object on the \"hand target position\"")]
        public Transform handTarget;
        [Tooltip("Tags of draggable walls")]
        public List<string> draggableTags = new List<string>() { "FreeClimb" };
        [Tooltip("Layer of draggable wall")]
        public LayerMask draggableWall;
        [Tooltip("Layer to check obstacles in movement direction ")]
        public LayerMask obstacle;
        public float climbEnterSpeed = 5f;
        public float climbEnterMaxDistance = 1f;
        [Tooltip("Use this to check if can go to horizontal position")]
        public float lastPointDistanceH = 0.4f;
        [Tooltip("Use this to check if can go to vertical position")]
        public float lastPointDistanceVUp = 0.2f;
        public float lastPointDistanceVDown = 1.25f;
        [Tooltip("Start Point of RayCast to check if Can GO")]
        public float offsetHandTarget = -0.2f;
        [Tooltip("Start Point of RayCast to check Base Rotation")]
        public float offsetBase = 0.35f;
        [Tooltip("Min Wall thickeness to climbUp")]
        public float climbUpMinThickness = 0.3f;
        [Tooltip("Min space  to climbUp with obstruction")]
        public float climbUpMinSpace = 0.5f;
        [Tooltip("Max Distance to ClimbJump")]
        public float climbJumpDistance = 2f;
        public float climbJumpDepth = 2f;
        public float climbUpHeight = 2f;
        [Tooltip("Offset to Hand IK")]
        public Vector3 offsetHandPositionL, offsetHandPositionR;
        [Tooltip("Root Animator state to call")]
        public string animatorStateHierarchy = "Base Layer.Actions.FreeClimb";
        public bool debugRays;
        [vHideInInspector("debugRays")]
        public bool debugClimbMovement = true;
        [vHideInInspector("debugRays")]
        public bool debugClimbUp;
        [vHideInInspector("debugRays")]
        public bool debugClimbJump;
        [vHideInInspector("debugRays")]
        public bool debugBaseRotation;
        [vHideInInspector("debugRays")]
        public bool debugHandIK;
        public UnityEngine.Events.UnityEvent onEnterClimb, onExitClimb;

        #endregion

        #region Protected variables

        protected vDragInfo dragInfo;
        protected vDragInfo jumDragInfo;
        protected vThirdPersonInput TP_Input;
        protected float horizontal, vertical;
        protected RaycastHit hit;
        protected bool canMoveClimb;
        protected bool inClimbUp;
        protected bool inClimbJump;
        protected bool inAlingClimb;
        protected bool inClimbEnter;
        protected bool climbEnterGrounded, climbEnterAir;
        protected Vector3 upPoint;
        protected Vector3 jumpPoint;
        protected float oldInput = 0.1f;
        protected Quaternion jumpRotation;
        protected Vector3 input;
        protected float ikWeight;
        protected float posTransition;
        protected float rotationTransition;

        Vector3 lHandPos;
        Vector3 rHandPos;
        Vector3 targetPositionL;
        Vector3 targetPositionR;
        Vector3 lastInput;
        Quaternion lastRotation;

        Vector3 handTargetPosition
        {
            get
            {
                return transform.TransformPoint(handTarget.localPosition.x, handTarget.localPosition.y, 0);
            }
        }

        #endregion

        #region UnityEngine Methods

        protected virtual void Start()
        {
            dragInfo = new vDragInfo();

            jumDragInfo = new vDragInfo();
            TP_Input = GetComponent<vThirdPersonInput>();
        }

        protected virtual void Update()
        {
            if (CheckClimbCondictions())
            {
                input = new Vector3(TP_Input.horizontalInput.GetAxis(),0, TP_Input.verticallInput.GetAxis());
                ClimbHandle();
                ClimbJumpHandle();
                ClimbUpHandle();
            }
            else
            {
                input = Vector2.zero;
                TP_Input.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, 0);
                TP_Input.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 0);
            }
        }

        protected virtual void LateUpdate()
        {
            if (dragInfo.inDrag && TP_Input.tpCamera != null)
            {
                TP_Input.CameraInput();
                TP_Input.tpCamera.ChangeState(cameraState, true);
                StaminaConsumption();
            }
        }

        #endregion

        #region Climb Behaviour

        [System.Serializable]
        public class vDragInfo
        {
            public bool canGo;
            public bool inDrag;
            public Vector3 position
            {
                get
                {
                    if (collider != null && collider.transform.parent) return collider.transform.parent.TransformPoint(localPosition);
                    return localPosition;
                }
                set
                {
                    if (collider != null && collider.transform.parent) localPosition = collider.transform.parent.InverseTransformPoint(value);
                    else localPosition = value;
                }
            }
            public Vector3 normal;
            private Vector3 localPosition;
            public Collider collider;
        }

        protected virtual bool CheckClimbCondictions()
        {
            if (TP_Input.cc.currentHealth <= 0 || !TP_Input || !TP_Input.cc)
            {
                if (TP_Input.enabled == false)
                {
                    dragInfo.inDrag = false;
                    dragInfo.canGo = false;
                    if (dragInfo.collider && dragInfo.collider.transform.parent == transform.parent)
                        transform.parent = null;
                    TP_Input.cc._rigidbody.isKinematic = false;
                    TP_Input.cc.enabled = true;
                    TP_Input.enabled = true;
                }
                return false;
            }
            return true;
        }

        protected virtual void ClimbHandle()
        {
            if (!TP_Input.cc.animator) return;

            if (!dragInfo.inDrag)
            {
                if (Physics.Raycast(handTargetPosition, transform.forward, out hit, climbEnterMaxDistance, draggableWall))
                {                   
                    if (IsValidPoint(hit.normal, hit.collider.transform.gameObject.tag))
                    {
                        if (debugRays) Debug.DrawRay(handTargetPosition, transform.forward * climbEnterMaxDistance, Color.green);
                        dragInfo.canGo = true;
                        dragInfo.normal = hit.normal;
                        dragInfo.collider = hit.collider;
                        dragInfo.position = hit.point;
                    }
                }
                else
                {
                    if (debugRays) Debug.DrawRay(handTargetPosition, transform.forward * climbEnterMaxDistance, Color.red);
                    dragInfo.canGo = false;
                }
            }
            if (dragInfo.canGo && !inClimbEnter && Physics.SphereCast(handTargetPosition + transform.forward * -TP_Input.cc._capsuleCollider.radius * 0.5f, TP_Input.cc._capsuleCollider.radius * 0.5f, transform.forward, out hit, climbEnterMaxDistance, draggableWall))
            {
                dragInfo.collider = hit.collider;
                var hitPointLocal = transform.InverseTransformPoint(hit.point);
                hitPointLocal.y = handTarget.localPosition.y;
                hitPointLocal.x = handTarget.localPosition.x;

                dragInfo.position = transform.TransformPoint(hitPointLocal);

                if (enterExitInput.GetButtonDown() && dragInfo.inDrag && input.magnitude == 0 && Time.time > (oldInput + 0.5f))
                    ExitClimb();
                else if (dragInfo.canGo && (enterExitInput.GetButton() || TP_Input.cc.input.z > 0.1f) && !dragInfo.inDrag && Time.time > (oldInput + 2f))
                    EnterClimb();
            }
            ClimbMovement();
        }

        protected virtual void ClimbMovement()
        {
            if (!dragInfo.inDrag) return;
            if (dragInfo.collider && dragInfo.collider.transform.parent && transform.parent != dragInfo.collider.transform.parent && !dragInfo.collider.transform.parent.gameObject.isStatic) transform.parent = dragInfo.collider.transform.parent;
            horizontal = input.x;
            vertical = input.z;
            canMoveClimb = CheckCanMoveClimb();
            dragInfo.canGo = canMoveClimb;

            if (canMoveClimb)
            {
                TP_Input.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, horizontal, 0.2f, Time.deltaTime);
                TP_Input.cc.animator.SetFloat(vAnimatorParameters.InputVertical, vertical, 0.2f, Time.deltaTime);
            }
            else if (!inAlingClimb && !inClimbJump)
            {
                TP_Input.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, 0, 0.2f, Time.deltaTime);
                TP_Input.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 0, 0.2f, Time.deltaTime);
            }

            if (input.z < 0 && Physics.Raycast(transform.position + transform.up * (TP_Input.cc._capsuleCollider.height * 0.5f), -transform.up, TP_Input.cc._capsuleCollider.height, TP_Input.cc.groundLayer))
            {
                ExitClimb();
            }
        }

        protected virtual bool CheckCanMoveClimb()
        {
            if (input.magnitude > 0.1f)
            {
                lastInput = input;
            }
            var h = lastInput.x > 0 ? 1 * lastPointDistanceH : lastInput.x < 0 ? -1 * lastPointDistanceH : 0;
            var v = lastInput.z > 0 ? 1 * lastPointDistanceVUp : lastInput.z < 0 ? -1 * lastPointDistanceVDown : 0;
            var centerCharacter = handTargetPosition + transform.up * offsetHandTarget;
            var targetPosNormalized = centerCharacter + (transform.right * h) + (transform.up * v);
            var targetPos = centerCharacter + (transform.right * lastInput.x) + (transform.up * lastInput.z);
            var castDir = (targetPosNormalized - handTargetPosition + (transform.forward * -0.5f)).normalized;
            var castDirCapsule = (targetPos - handTargetPosition + (transform.forward * -0.5f)).normalized;

            if (TP_Input.cc._capsuleCollider.CheckCapsule(castDirCapsule, out hit, obstacle) && !draggableTags.Contains(hit.collider.gameObject.tag))
            {
                return false;
            }

            if (inClimbJump || inClimbUp) return false;
            vLine climbLine = new vLine(centerCharacter, targetPosNormalized);
            climbLine.Draw(Color.green, draw: debugRays && debugClimbMovement);
            if (Physics.Linecast(climbLine.p1, climbLine.p2, out hit, draggableWall))
            {
                if (IsValidPoint(hit.normal, hit.collider.transform.gameObject.tag))
                {
                    dragInfo.collider = hit.collider;
                    dragInfo.normal = hit.normal;
                    return true;
                }
            }

            climbLine.p1 = climbLine.p2;
            climbLine.p2 = climbLine.p1 + transform.forward * TP_Input.cc._capsuleCollider.radius * 2f;
            climbLine.Draw(Color.yellow, draw: debugRays && debugClimbMovement);
            if (Physics.Linecast(climbLine.p1, climbLine.p2, out hit, draggableWall))
            {
                if (IsValidPoint(hit.normal, hit.collider.transform.gameObject.tag))
                {
                    dragInfo.collider = hit.collider;
                    dragInfo.normal = hit.normal;
                    return true;
                }
            }

            climbLine.p1 += transform.forward * TP_Input.cc._capsuleCollider.radius * 0.5f;
            climbLine.p2 += (transform.right * (TP_Input.cc._capsuleCollider.radius + lastPointDistanceH) * -input.x) + (transform.up * -v) + transform.forward * TP_Input.cc._capsuleCollider.radius;
            climbLine.Draw(Color.red, draw: debugRays && debugClimbMovement);
            if (Physics.Linecast(climbLine.p1, climbLine.p2, out hit, draggableWall))
            {
                if (IsValidPoint(hit.normal, hit.collider.transform.gameObject.tag))
                {
                    dragInfo.normal = hit.normal;
                    dragInfo.collider = hit.collider;
                    return true;
                }
            }
            return false;
        }

        protected virtual void ClimbJumpHandle()
        {
            if (TP_Input.enabled || !TP_Input.cc.animator || !dragInfo.inDrag || inClimbUp || inClimbEnter) return;
            if (climbJumpInput.GetButton() && !inClimbJump && input.magnitude > 0 && !TP_Input.cc.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStateHierarchy + ".ClimbJump"))
            {
                var angleBetweenCharacterAndCamera = Vector3.Angle(transform.right, Camera.main.transform.right);
                var rightDirection = angleBetweenCharacterAndCamera > 60 ? Camera.main.transform.right : transform.right;
                var pos360 = handTargetPosition + (transform.forward * -0.5f) + (rightDirection * climbJumpDistance * horizontal) + (Vector3.up * climbJumpDistance * vertical);
                if (debugRays && debugClimbJump)
                {
                    Debug.DrawLine(handTargetPosition + (transform.forward * -0.05f), pos360, Color.red, 1f);
                    Debug.DrawRay(pos360, transform.forward * climbJumpDepth, Color.red, 1f);
                }

                float casts = 0f;
                for (int i = 0; casts < 1f; i++)
                {
                    var radius = TP_Input.cc._capsuleCollider.radius / 0.45f;

                    var dir = (rightDirection * input.x + transform.up * input.z).normalized;
                    for (float a = 0; a < 1; a += 0.2f)
                    {
                        var p = transform.position + transform.up * TP_Input.cc._capsuleCollider.height * casts;
                        p = p + rightDirection * ((-TP_Input.cc._capsuleCollider.radius) + (radius * a));

                        if (Physics.Raycast(p, dir.normalized, out hit, climbJumpDistance, obstacle))
                        {
                            if (!(draggableWall == (draggableWall | (1 << hit.collider.gameObject.layer))) || !draggableTags.Contains(hit.collider.gameObject.tag))
                            {
                                if (debugRays && debugClimbJump) Debug.DrawRay(p, dir.normalized * climbJumpDistance, Color.red, 0.4f);
                                return;
                            }
                            else if (debugRays && debugClimbJump) Debug.DrawRay(p, dir.normalized * climbJumpDistance, Color.yellow, 0.4f);
                        }
                        else if (debugRays && debugClimbJump) Debug.DrawRay(p, dir.normalized * climbJumpDistance, Color.green, 0.4f);
                    }
                    casts += 0.1f;
                }

                if (Physics.Linecast(handTargetPosition + (transform.forward * -0.5f), pos360, out hit, draggableWall))
                {
                    if (IsValidPoint(hit.normal, hit.collider.transform.gameObject.tag))
                    {
                        var dir = pos360 - handTargetPosition;
                        var angle = Vector3.Angle(transform.up, dir);
                        angle = angle * (input.x > 0 ? 1 : input.x < 0 ? -1 : 1);
                        jumpRotation = Quaternion.LookRotation(-hit.normal);


                        jumDragInfo.collider = hit.collider;
                        jumDragInfo.normal = hit.normal;
                        jumDragInfo.position = hit.point;
                        dragInfo.collider = hit.collider;
                        dragInfo.position = hit.point;
                        ClimbJump();
                    }
                }
                else if (Physics.Raycast(pos360, transform.forward, out hit, climbJumpDepth, draggableWall))
                {
                    if (IsValidPoint(hit.normal, hit.collider.transform.gameObject.tag))
                    {
                        var dir = pos360 - handTargetPosition;
                        var angle = Vector3.Angle(transform.up, dir);
                        angle = angle * (input.x > 0 ? 1 : input.x < 0 ? -1 : 1);
                        jumpRotation = Quaternion.LookRotation(-hit.normal);
                        jumDragInfo.collider = hit.collider;
                        jumDragInfo.normal = hit.normal;
                        jumDragInfo.position = hit.point;
                        dragInfo.collider = hit.collider;
                        dragInfo.position = hit.point;
                        ClimbJump();
                    }
                }
            }
        }

        protected virtual void ClimbUpHandle()
        {
            if (inClimbJump || TP_Input.enabled || !TP_Input.cc.animator || !dragInfo.inDrag) return;

            if (inClimbUp && !inAlingClimb)
            {
                if (TP_Input.cc.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStateHierarchy + ".ClimbUpWall"))
                {
                    if (!TP_Input.cc.animator.IsInTransition(0))
                        TP_Input.cc.animator.MatchTarget(upPoint + Vector3.up * 0.1f, Quaternion.Euler(0, transform.eulerAngles.y, 0), AvatarTarget.RightHand, new MatchTargetWeightMask(new Vector3(0, 1, 1), 1), 0.1f, 0.4f);
                    if (TP_Input.cc.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .9f) ExitClimb();
                }
                return;
            }
            CheckClimbUp();
        }

        private void CheckClimbUp(bool ignoreInput = false)
        {
            var climbUpConditions = autoClimbEdge ? vertical > 0f : climbEdgeInput.GetButtonDown();

            if (!canMoveClimb && !inClimbUp && (climbUpConditions || ignoreInput))
            {
                var dir = transform.forward;

                var startPoint = dragInfo.position + transform.forward * -0.1f;
                var endPoint = startPoint + Vector3.up * (TP_Input.cc._capsuleCollider.height * 0.25f);
                var obstructionPoint = endPoint + dir.normalized * (climbUpMinSpace + 0.1f);
                var thicknessPoint = endPoint + dir.normalized * (climbUpMinThickness + 0.1f);
                var climbPoint = thicknessPoint + -transform.up * (TP_Input.cc._capsuleCollider.height * 0.5f);

                if (!Physics.Linecast(startPoint, endPoint, obstacle))
                {
                    if (debugRays && debugClimbUp) Debug.DrawLine(startPoint, endPoint, Color.green, 2f);
                    if (!Physics.Linecast(endPoint, obstructionPoint, obstacle))
                    {
                        if (debugRays && debugClimbUp) Debug.DrawLine(endPoint, obstructionPoint, Color.green, 2f);
                        if (Physics.Linecast(thicknessPoint, climbPoint, out hit, TP_Input.cc.groundLayer))
                        {
                            if (debugRays && debugClimbUp) Debug.DrawLine(thicknessPoint, climbPoint, Color.green, 2f);
                            var angle = Vector3.Angle(Vector3.up, hit.normal);
                            var localUpPoint = transform.InverseTransformPoint(hit.point + (angle > 25 ? Vector3.up * TP_Input.cc._capsuleCollider.radius : Vector3.zero) + dir * -(climbUpMinThickness * 0.5f));
                            localUpPoint.z = TP_Input.cc._capsuleCollider.radius;
                            upPoint = transform.TransformPoint(localUpPoint);
                            if (Physics.Raycast(hit.point + Vector3.up * -0.05f, Vector3.up, out hit, TP_Input.cc._capsuleCollider.height, obstacle))
                            {
                                if (hit.distance > TP_Input.cc._capsuleCollider.height * 0.5f)
                                {
                                    if (hit.distance < TP_Input.cc._capsuleCollider.height)
                                    {
                                        TP_Input.cc.isCrouching = true;
                                        TP_Input.cc.animator.SetBool(vAnimatorParameters.IsCrouching, true);
                                    }
                                    ClimbUp();
                                }
                                else
                                {
                                    if (debugRays && debugClimbUp) Debug.DrawLine(upPoint, hit.point, Color.red, 2f);
                                }
                            }
                            else ClimbUp();
                        }
                        else if (debugRays && debugClimbUp) Debug.DrawLine(thicknessPoint, climbPoint, Color.red, 2f);
                    }
                    else if (debugRays && debugClimbUp) Debug.DrawLine(endPoint, obstructionPoint, Color.red, 2f);
                }
                else if (debugRays && debugClimbUp) Debug.DrawLine(startPoint, endPoint, Color.red, 2f);
            }
        }

        IEnumerator AlignClimb()
        {
            inAlingClimb = true;
            var transition = 0f;
            var dir = transform.forward;
            dir.y = 0;
            var angle = Vector3.Angle(Vector3.up, transform.forward);

            var targetRotation = Quaternion.LookRotation(-dragInfo.normal);
            var targetPosition = ((dragInfo.position + dir * -TP_Input.cc._capsuleCollider.radius + Vector3.up * 0.1f) - transform.rotation * handTarget.localPosition);

            TP_Input.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 1f);
            while (transition < 1 && Vector3.Distance(targetRotation.eulerAngles, transform.rotation.eulerAngles) > 0.2f && angle < 60)
            {
                TP_Input.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 1f);
                transition += Time.deltaTime * 0.5f;
                targetPosition = ((dragInfo.position + dir * -TP_Input.cc._capsuleCollider.radius) - transform.rotation * handTarget.localPosition);
                transform.position = Vector3.Slerp(transform.position, targetPosition, transition);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, transition);
                yield return null;
            }
            TP_Input.cc.animator.CrossFadeInFixedTime("ClimbUpWall", 0.1f);
            inAlingClimb = false;
        }

        protected virtual bool IsValidPoint(Vector3 normal, string tag)
        {
            if (!draggableTags.Contains(tag)) return false;

            var angle = Vector3.Angle(Vector3.up, normal);

            if (angle >= minSurfaceAngle && angle <= maxSurfaceAngle)
                return true;
            return false;
        }

        protected virtual bool IsInLayerMask(int layer, LayerMask layermask)
        {
            return layermask == (layermask | (1 << layer));
        }

        protected void StaminaConsumption()
        {
            TP_Input.cc.StaminaRecovery();          // run stamina methods
            TP_Input.UpdateHUD();                   // update hud graphics       

            if (climbStaminaCost == 0) return;

            if (TP_Input.cc.currentStamina <= 0)
            {
                ExitClimb();
            }
            else
            {
                TP_Input.cc.ReduceStamina(climbStaminaCost, true);                  // call the ReduceStamina method from the player                
                TP_Input.cc.currentStaminaRecoveryDelay = staminaRecoveryDelay;     // delay to start recovery stamina           
            }
        }

        #endregion

        #region Trigger Animations

        protected virtual void ClimbJump()
        {
            inClimbJump = true;
            TP_Input.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, input.x);
            TP_Input.cc.animator.SetFloat(vAnimatorParameters.InputVertical, input.z);
            TP_Input.cc.animator.CrossFadeInFixedTime("ClimbJump", 0.2f);
            if (jumpClimbStaminaCost > 0) TP_Input.cc.ReduceStamina(jumpClimbStaminaCost, false);
        }

        protected virtual void ClimbUp()
        {
            StartCoroutine(AlignClimb());
            inClimbUp = true;
        }

        protected virtual void EnterClimb()
        {            
            oldInput = Time.time;
            TP_Input.cc.enabled = false;
            TP_Input.enabled = false;
            TP_Input.cc._rigidbody.isKinematic = true;
            RaycastHit hit;
            var dragPosition = new Vector3(dragInfo.position.x, transform.position.y, dragInfo.position.z) + transform.forward * -TP_Input.cc._capsuleCollider.radius;
            var castObstacleUp = Physics.Raycast(dragPosition + transform.up * TP_Input.cc._capsuleCollider.height, transform.up, TP_Input.cc._capsuleCollider.height * 0.5f, obstacle);
            var castDragableWallForward = Physics.Raycast(dragPosition + transform.up * (TP_Input.cc._capsuleCollider.height * climbUpHeight), transform.forward, out hit, 1f, draggableWall) && draggableTags.Contains(hit.collider.gameObject.tag);
            var climbUpConditions = TP_Input.cc.isGrounded && !castObstacleUp && castDragableWallForward;

            TP_Input.cc.animator.SetBool(vAnimatorParameters.IsGrounded, true);
            TP_Input.cc.animator.CrossFadeInFixedTime(climbUpConditions ? "EnterClimbGrounded" : "EnterClimbAir", 0.2f);

            if (dragInfo.collider && dragInfo.collider.transform.parent && transform.parent != dragInfo.collider.transform.parent && !dragInfo.collider.transform.parent.gameObject.isStatic)
                transform.parent = dragInfo.collider.transform.parent;

            if (!climbUpConditions)
            {
                StartCoroutine(EnterClimbAlignment());
            }
            else
            {
                transform.position = (dragInfo.position - transform.rotation * handTarget.localPosition);
                dragInfo.inDrag = true;
            }

            onEnterClimb.Invoke();
            TP_Input.cc.onActionStay.AddListener(OnTriggerStayEvent);
        }

        IEnumerator EnterClimbAlignment()
        {
            inClimbEnter = true;
            dragInfo.inDrag = true;
            var _position = transform.position;
            var _rotation = transform.rotation;
            var _targetRotation = Quaternion.LookRotation(-dragInfo.normal);
            var _targetPosition = (dragInfo.position - transform.rotation * handTarget.localPosition);

            var _transition = 0f;
            Debug.DrawLine(handTargetPosition, dragInfo.position, Color.red, 10f);
            Debug.DrawLine(transform.position, dragInfo.position, Color.red, 10f);
            while (_transition < 1)
            {
                _transition += Time.deltaTime * climbEnterSpeed;

                transform.rotation = Quaternion.Lerp(_rotation, _targetRotation, _transition);
                _targetPosition = (dragInfo.position - transform.rotation * handTarget.localPosition);
                transform.position = _targetPosition;      
                yield return null;
            }
            _targetPosition = (dragInfo.position - transform.rotation * handTarget.localPosition);
            transform.position = _targetPosition;
            inClimbEnter = false;
        }

        protected virtual void ExitClimb()
        {
            TP_Input.cc.onActionStay.RemoveListener(OnTriggerStayEvent);
            oldInput = Time.time;
            dragInfo.inDrag = false;
            dragInfo.canGo = false;

            inClimbJump = false;
            TP_Input.cc._rigidbody.isKinematic = false;
            TP_Input.cc.isJumping = false;
            if (!inClimbUp)
            {
                bool nextGround = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.5f, TP_Input.cc.groundLayer);
                var charAngle = Vector3.Angle(transform.forward, Vector3.up);
                if (charAngle < 80)
                {
                    var dir = transform.forward;
                    dir.y = 0;

                    var postion = dragInfo.position + dir.normalized * -TP_Input.cc._capsuleCollider.radius + Vector3.down * TP_Input.cc._capsuleCollider.height;
                    transform.position = postion;
                }

                TP_Input.cc.animator.CrossFadeInFixedTime(nextGround ? "ExitGrounded" : "ExitAir", 0.2f);
            }
            else
            {
                TP_Input.cc.verticalVelocity = 0;
                TP_Input.cc.animator.SetFloat(vAnimatorParameters.GroundDistance, 0);
            }

            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            TP_Input.cc.enabled = true;
            TP_Input.enabled = true;
            inClimbUp = false;
            if (transform.parent != null && dragInfo.collider && dragInfo.collider.transform.parent && transform.parent == dragInfo.collider.transform.parent) transform.parent = null;
            onExitClimb.Invoke();
        }

        #endregion

        #region RootMotion and AnimatorIK

        protected virtual void OnAnimatorMove()
        {
            if (TP_Input.enabled) return;

            climbEnterGrounded = (TP_Input.cc.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStateHierarchy + ".EnterClimbGrounded"));
            climbEnterAir = (TP_Input.cc.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStateHierarchy + ".EnterClimbAir"));

            if (dragInfo.inDrag && (canMoveClimb) && !inClimbUp && !inClimbJump && !climbEnterGrounded)
            {
                ApplyClimbMovement();
            }
            else if (inClimbJump)
            {
                ApplyClimbJump();
            }
            else if (inClimbUp || climbEnterGrounded || climbEnterAir)
            {
                if (!inClimbUp)
                    CheckClimbUp(true);

                ApplyRootMotion();
            }
        }

        protected virtual void OnAnimatorIK()
        {
            if (TP_Input.enabled || inClimbJump || inClimbUp || !dragInfo.inDrag) { ikWeight = 0; return; }
            ikWeight = Mathf.Lerp(ikWeight, 1f, 2f * Time.deltaTime);
            if (ikWeight > 0)
            {
                var lRoot = transform.InverseTransformPoint(TP_Input.cc.animator.GetBoneTransform(HumanBodyBones.LeftHand).position);
                var rRoot = transform.InverseTransformPoint(TP_Input.cc.animator.GetBoneTransform(HumanBodyBones.RightHand).position);
                RaycastHit hit2;

                if (Physics.Raycast(TP_Input.cc.animator.GetBoneTransform(HumanBodyBones.LeftHand).position + transform.forward * -0.5f + transform.up * -0.2f, transform.forward, out hit2, 1f, draggableWall))
                {
                    targetPositionL = transform.InverseTransformPoint(hit2.point);
                    if (debugRays && debugHandIK) Debug.DrawLine(TP_Input.cc.animator.GetBoneTransform(HumanBodyBones.LeftHand).position + transform.forward * -0.5f + transform.up * -0.2f, hit2.point, Color.green);
                }
                else
                {
                    var center = transform.TransformPoint(0, lRoot.y, 0);
                    var target = rRoot;
                    if (Physics.Raycast(center, transform.forward, out hit2, 1f, draggableWall))
                    {
                        target = transform.InverseTransformPoint(hit2.point);
                    }
                    target.x = 0;
                    targetPositionL = Vector3.Lerp(targetPositionL, target, 5f * Time.deltaTime);
                    if (debugRays && debugHandIK) Debug.DrawRay(TP_Input.cc.animator.GetBoneTransform(HumanBodyBones.LeftHand).position + transform.forward * -0.5f + transform.up * -0.2f, transform.forward, Color.red);
                }

                if (Physics.Raycast(TP_Input.cc.animator.GetBoneTransform(HumanBodyBones.RightHand).position + transform.forward * -0.5f + transform.up * -0.2f, transform.forward, out hit2, 1f, draggableWall))
                {
                    targetPositionR = transform.InverseTransformPoint(hit2.point);
                    if (debugRays && debugHandIK) Debug.DrawLine(TP_Input.cc.animator.GetBoneTransform(HumanBodyBones.RightHand).position + transform.forward * -0.5f + transform.up * -0.2f, hit2.point, Color.green);
                }
                else
                {
                    var center = transform.TransformPoint(0, rRoot.y, 0);
                    var target = lRoot;
                    if (Physics.Raycast(center, transform.forward, out hit2, 1f, draggableWall))
                        target = transform.InverseTransformPoint(hit2.point);

                    target.x = 0;
                    targetPositionR = Vector3.Lerp(targetPositionR, target, 5f * Time.deltaTime);
                    if (debugRays && debugHandIK) Debug.DrawRay(TP_Input.cc.animator.GetBoneTransform(HumanBodyBones.RightHand).position + transform.forward * -0.5f + transform.up * -0.2f, transform.forward, Color.red);
                }
                var leftHandPosition = transform.position + transform.right * targetPositionL.x + transform.up * lRoot.y + transform.forward * targetPositionL.z;
                var rightHandPosition = transform.position + transform.right * targetPositionR.x + transform.up * rRoot.y + transform.forward * targetPositionR.z;
                lHandPos = transform.forward * offsetHandPositionL.z + transform.right * offsetHandPositionL.x + transform.up * offsetHandPositionL.y;// Vector3.Lerp(lHandPos, transform.forward * offsetHandPositionL.z + transform.right * offsetHandPositionL.x + transform.up * offsetHandPositionL.y, 2 * Time.deltaTime);
                rHandPos = transform.forward * offsetHandPositionR.z + transform.right * offsetHandPositionR.x + transform.up * offsetHandPositionR.y;// Vector3.Lerp(rHandPos, transform.forward * offsetHandPositionR.z + transform.right * offsetHandPositionR.x + transform.up * offsetHandPositionR.y, 2 * Time.deltaTime);
                leftHandPosition += lHandPos;
                rightHandPosition += rHandPos;

                TP_Input.cc.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
                TP_Input.cc.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);

                TP_Input.cc.animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPosition);
                TP_Input.cc.animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPosition);
            }
            else
            {
                TP_Input.cc.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                TP_Input.cc.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            }
        }

        void ApplyClimbMovement()
        {
            ///Apply Rotation
            CalculateMovementRotation();
            ///Apply Position
            posTransition = Mathf.Lerp(posTransition, 1f, 5 * Time.deltaTime);
            var root = transform.InverseTransformPoint(TP_Input.cc.animator.rootPosition);

            var position = (dragInfo.position - transform.rotation * handTarget.localPosition) + (transform.right * root.x + transform.up * root.y);
            Debug.DrawLine(transform.position, dragInfo.position);
            if (input.magnitude > 0.1f)
                transform.position = Vector3.Lerp(transform.position, position, posTransition);
        }

        void CalculateMovementRotation(bool ignoreLerp = false)
        {
            var h = lastInput.x;
            var v = lastInput.z;
            var characterBase = transform.position + transform.up * (TP_Input.cc._capsuleCollider.radius + offsetBase);
            var directionPoint = characterBase + transform.right * (h * lastPointDistanceH) + transform.up * (v * lastPointDistanceVUp);

            RaycastHit rotationHit;
            vLine centerLine = new vLine(characterBase, directionPoint);
            centerLine.Draw(Color.cyan, draw: debugRays && debugBaseRotation);
            var hasBasePoint = CheckBasePoint(out rotationHit);

            var basePoint = rotationHit.point;
            if (Physics.Linecast(centerLine.p1, centerLine.p2, out rotationHit, draggableWall) && draggableTags.Contains(rotationHit.collider.gameObject.tag))
            {
                RotateTo(-rotationHit.normal, hasBasePoint ? basePoint : rotationHit.point, ignoreLerp);
                return;
            }

            centerLine.p1 = centerLine.p2;
            centerLine.p2 += transform.forward * (climbEnterMaxDistance);
            centerLine.Draw(Color.yellow, draw: debugRays && debugBaseRotation);

            if (Physics.Linecast(centerLine.p1, centerLine.p2, out rotationHit, draggableWall) && draggableTags.Contains(rotationHit.collider.gameObject.tag))
            {
                RotateTo(-rotationHit.normal, hasBasePoint ? basePoint : rotationHit.point, ignoreLerp);
                return;
            }
            centerLine.p1 += transform.forward * TP_Input.cc._capsuleCollider.radius * 0.5f;
            centerLine.p2 += (transform.right * ((TP_Input.cc._capsuleCollider.radius + lastPointDistanceH) * -input.x)) + (transform.up * lastPointDistanceVUp * -v) + transform.forward * TP_Input.cc._capsuleCollider.radius;
            centerLine.Draw(Color.red, draw: debugRays && debugBaseRotation);

            if (Physics.Linecast(centerLine.p1, centerLine.p2, out rotationHit, draggableWall) && draggableTags.Contains(rotationHit.collider.gameObject.tag))
            {
                RotateTo(-rotationHit.normal, hasBasePoint ? basePoint : rotationHit.point, ignoreLerp);
                return;
            }
        }

        bool CheckBasePoint(out RaycastHit baseHit)
        {
            var forward = new Vector3(transform.forward.x, 0, transform.forward.z);
            var characterBase = transform.position + transform.up * (TP_Input.cc._capsuleCollider.radius + offsetBase) - forward * (TP_Input.cc._capsuleCollider.radius * 2);

            var targetPoint = transform.position + forward * (1 + TP_Input.cc._capsuleCollider.radius);
            vLine baseLine = new vLine(characterBase, targetPoint);

            if (Physics.Linecast(baseLine.p1, baseLine.p2, out baseHit, draggableWall) && draggableTags.Contains(baseHit.collider.gameObject.tag))
            {
                baseLine.Draw(Color.blue, draw: debugRays && debugBaseRotation);
                return true;
            }
            baseLine.Draw(Color.magenta, draw: debugRays);
            baseLine.p1 = baseLine.p2;
            baseLine.p2 = baseLine.p1 + forward + Vector3.up;

            if (Physics.Linecast(baseLine.p1, baseLine.p2, out baseHit, draggableWall) && draggableTags.Contains(baseHit.collider.gameObject.tag))
            {
                baseLine.Draw(Color.blue, draw: debugRays && debugBaseRotation);
                return true;
            }
            baseLine.Draw(Color.magenta, draw: debugRays);
            baseLine.p2 = baseLine.p1 + forward + Vector3.down;

            if (Physics.Linecast(baseLine.p1, baseLine.p2, out baseHit, draggableWall) && draggableTags.Contains(baseHit.collider.gameObject.tag))
            {
                baseLine.Draw(Color.blue, draw: debugRays && debugBaseRotation);
                return true;
            }
            baseLine.Draw(Color.magenta, draw: debugRays && debugBaseRotation);
            return false;
        }

        void RotateTo(Vector3 direction, Vector3 point, bool ignoreLerp = false)
        {
            if (input.magnitude < 0.1f) return;
            var referenceDirection = point - dragInfo.position;
            if (debugRays && debugBaseRotation) Debug.DrawLine(point, dragInfo.position, Color.blue, .1f);
            var resultDirection = Quaternion.AngleAxis(-90, transform.right) * referenceDirection;
            var eulerX = Quaternion.LookRotation(resultDirection).eulerAngles.x;
            var baseRotation = Quaternion.LookRotation(direction);
            var resultRotation = Quaternion.Euler(eulerX, baseRotation.eulerAngles.y, transform.eulerAngles.z);
            //var eulerResult = resultRotation.eulerAngles - transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Lerp(transform.rotation, resultRotation, (TP_Input.cc.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) * 0.2f);
        }

        void ApplyClimbJump()
        {
            if (TP_Input.cc.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStateHierarchy + ".ClimbJump"))
            {
                var pos = (jumDragInfo.position - transform.rotation * handTarget.localPosition);
                if (!TP_Input.cc.animator.IsInTransition(0) && TP_Input.cc.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.25f)
                {
                    var percentage = (((TP_Input.cc.animator.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.25f) / 0.8f) * 100f) * 0.01f;
                    transform.position = Vector3.Lerp(transform.position, pos, percentage);
                    transform.rotation = Quaternion.Lerp(transform.rotation, jumpRotation, percentage);
                }

                if (TP_Input.cc.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
                {
                    inClimbJump = false;
                    transform.position = pos;
                    transform.rotation = jumpRotation;
                }
            }
            if (Physics.Raycast(handTargetPosition + (transform.forward * -0.5f), transform.forward, out hit, 1, draggableWall))
            {
                if (IsValidPoint(hit.normal, hit.collider.transform.gameObject.tag))
                {
                    dragInfo.canGo = true;
                    dragInfo.collider = hit.collider;
                    dragInfo.position = hit.point;
                    dragInfo.normal = hit.normal;
                }
            }
        }

        void ApplyRootMotion()
        {
            transform.position = TP_Input.cc.animator.rootPosition;
            transform.rotation = TP_Input.cc.animator.rootRotation;
            posTransition = 0;
        }

        void OnTriggerStayEvent(Collider other)
        {
            if (other.gameObject.CompareTag("ClimbDirection") && !inClimbJump && dragInfo.inDrag)
            {
                var euler = transform.eulerAngles;
                if (input.magnitude > 0.1f)
                {
                    euler.z = Mathf.LerpAngle(euler.z, other.transform.eulerAngles.z, 2 * Time.fixedDeltaTime);
                    transform.eulerAngles = euler;
                }
            }
        }        
        #endregion
    }
}