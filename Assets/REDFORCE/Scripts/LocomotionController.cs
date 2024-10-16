using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

namespace redforce01
{
    public class LocomotionController : MonoBehaviour
    {
        public bool IsForceAim = false;

        [SerializeField] private Animator characterAnimator;
        [SerializeField] private Rig AimingRig;

        private readonly int hashKeyOfSpeed = Animator.StringToHash("SpeedAnim");
        private readonly int hashKeyOfHorizontal = Animator.StringToHash("Horizontal");
        private readonly int hashKeyOfVertical = Animator.StringToHash("Vertical");
        private readonly int hashKeyOfAimed = Animator.StringToHash("Aimed");

        private float blendAiming = 0f;
        private float blendSpeed = 0f;
        private bool isCroching = false;

        private Vector2 lastInputDirection = Vector2.zero;

        public void Update()
        {
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");

            bool inputAim = Input.GetMouseButton(1) || IsForceAim;
            isCroching = Input.GetKey(KeyCode.LeftShift);

            Vector2 inputVec = new Vector2(inputX, inputY);
            
            if (inputVec.magnitude > 0f)
            {
                if (inputVec.x > 0 || inputVec.x < 0)
                {
                    lastInputDirection.x = Mathf.Lerp(lastInputDirection.x, inputVec.x > 0f? 1f:-1f, Time.deltaTime * 2f);
                }
                else
                {
                    //lastInputDirection.x = Mathf.Lerp(lastInputDirection.x, 0.0f, Time.deltaTime * 2f);
                    lastInputDirection.x = 0.0f;
                }

                if (inputVec.y > 0 || inputVec.y < 0)
                {
                    lastInputDirection.y = Mathf.Lerp(lastInputDirection.y, inputVec.y > 0f ? 1f : -1f, Time.deltaTime * 2f);
                }
                else
                {
                    //lastInputDirection.y = Mathf.Lerp(lastInputDirection.y, 0.0f, Time.deltaTime * 2f);
                    lastInputDirection.y = 0.0f;
                }
            }
            //blendAiming = Mathf.Lerp(blendAiming, inputAim ? 1 : 0, Time.deltaTime * 10);
            blendAiming = inputAim ? 1 : 0;
            AimingRig.weight = blendAiming;
            //blendSpeed = Mathf.Lerp(blendSpeed, inputVec.magnitude > 0f ? (isCroching ? 1f : 3f) : 0f, Time.deltaTime * 10f);
            blendSpeed = inputVec.magnitude > 0.1f ? (isCroching ? 1f : 3f) : 0f;

            characterAnimator.SetFloat(hashKeyOfSpeed, blendSpeed);
            characterAnimator.SetFloat(hashKeyOfHorizontal, lastInputDirection.x);
            characterAnimator.SetFloat(hashKeyOfVertical, lastInputDirection.y);
            characterAnimator.SetFloat(hashKeyOfAimed, blendAiming);
        }
    }
}

