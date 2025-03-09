using System.Collections;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using SimpleInputNamespace;
using UnityEngine.UI;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        public ExampleCharacterController Character;
        public ExampleCharacterCamera CharacterCamera;

        [Header("Мобильное управление")]
        public GameObject joystick;  // Джойстик для передвижения
        public Touchpad touchpad;    // Тачпад для камеры
        public CanvasGroup mobileUI; // CanvasGroup для мобильных элементов
        public Button jumpButton;    // 🔹 Кнопка прыжка
        public Button zoomInButton;  // 🔹 Кнопка приближения камеры
        public Button zoomOutButton; // 🔹 Кнопка отдаления камеры

        private bool isMobile;
        private bool jumpPressed; // Флаг для прыжка

        private void Start()
        {
            // Определяем платформу
            isMobile = true; // 🔹 Здесь можно определить, мобильное ли устройство

            // Настраиваем CanvasGroup для мобильного управления
            if (mobileUI != null)
            {
                mobileUI.alpha = isMobile ? 1f : 0f;
                mobileUI.blocksRaycasts = isMobile;
            }

            // Подключаем кнопки
            if (isMobile)
            {
                jumpButton?.onClick.AddListener(OnJumpPressed);
                zoomInButton?.onClick.AddListener(() => AdjustZoom(-1f));
                zoomOutButton?.onClick.AddListener(() => AdjustZoom(1f));
            }

            // Камера следует за персонажем
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);
            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            Vector3 lookInputVector = Vector3.zero;

            if (isMobile)
            {
                lookInputVector = new Vector2(touchpad.Value.x, touchpad.Value.y);
                if (touchpad.xAxis.value == 0f && touchpad.yAxis.value == 0f)
                {
                    lookInputVector = Vector3.zero;
                }
            }
            else
            {
                if (SimpleInput.GetMouseButton(1))
                {
                    float mouseLookAxisUp = SimpleInput.GetAxisRaw("Mouse Y");
                    float mouseLookAxisRight = SimpleInput.GetAxisRaw("Mouse X");
                    lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);
                }
            }

            float scrollInput = isMobile ? 0f : -SimpleInput.GetAxis("Mouse ScrollWheel");
            CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            if (isMobile)
            {
                characterInputs.MoveAxisForward = SimpleInput.GetAxis("Vertical");
                characterInputs.MoveAxisRight = SimpleInput.GetAxis("Horizontal");
                characterInputs.JumpDown = jumpPressed; // 🔹 Прыжок по кнопке
                jumpPressed = false; // 🔹 Сбрасываем флаг после обработки
            }
            else
            {
                characterInputs.MoveAxisForward = SimpleInput.GetAxisRaw("Vertical");
                characterInputs.MoveAxisRight = SimpleInput.GetAxisRaw("Horizontal");
                characterInputs.JumpDown = SimpleInput.GetKeyDown(KeyCode.Space);
                characterInputs.CrouchDown = SimpleInput.GetKeyDown(KeyCode.C);
                characterInputs.CrouchUp = SimpleInput.GetKeyUp(KeyCode.C);
            }

            characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            Character.SetInputs(ref characterInputs);
        }

        private void OnJumpPressed()
        {
            jumpPressed = true;
        }

        private void AdjustZoom(float amount)
        {
            CharacterCamera.TargetDistance += amount;
        }
    }
}
