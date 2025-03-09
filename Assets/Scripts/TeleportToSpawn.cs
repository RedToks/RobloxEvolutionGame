using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;

namespace KinematicCharacterController.Examples
{
    public class FallRespawn : MonoBehaviour
    {
        [Header("Настройки телепортации")]
        public Transform spawnPoint; // Точка спавна
        public float minYPosition = -10f; // Минимальная высота, после которой игрок телепортируется

        private ExampleCharacterController player;
        private KinematicCharacterMotor motor;

        public bool isBeingTeleportedTo { get; set; }

        private void Start()
        {
            player = FindObjectOfType<ExampleCharacterController>();
            if (player)
            {
                motor = player.Motor;
            }

            if (spawnPoint == null)
            {
                Debug.LogError("❌ Точка спавна не назначена! Укажите её в инспекторе.");
            }
        }

        private void Update()
        {
            if (!isBeingTeleportedTo && player != null && player.transform.position.y < minYPosition)
            {
                TeleportToSpawn();
            }
        }

        public void TeleportToSpawn()
        {
            isBeingTeleportedTo = true;

            if (motor != null)
            {
                motor.enabled = false; // Выключаем управление
            }


            player.Motor.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            Debug.Log("🏁 Игрок упал и был телепортирован на спавн!");



            if (motor != null)
            {
                motor.enabled = true; // Включаем управление обратно
            }

            isBeingTeleportedTo = false;
        }
    }
}
