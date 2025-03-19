using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KinematicCharacterController.Examples;

namespace KinematicCharacterController.Examples
{
    public class Teleporter : MonoBehaviour
    {
        public Teleporter TeleportTo;
        public long requiredBrainCoins = 1000; // 🔹 Сколько BrainCoins нужно для входа
        public UnityAction<ExampleCharacterController> OnCharacterTeleport;

        public bool isBeingTeleportedTo { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (!isBeingTeleportedTo)
            {
                ExampleCharacterController cc = other.GetComponent<ExampleCharacterController>();
                if (cc)
                {
                    // 🔹 Проверяем, есть ли у игрока достаточно BrainCoins
                    if (BrainCurrency.Instance.brainCurrency >= requiredBrainCoins)
                    {

                        // 🔹 Телепортируем
                        cc.Motor.SetPositionAndRotation(TeleportTo.transform.position, TeleportTo.transform.rotation);

                        if (OnCharacterTeleport != null)
                        {
                            OnCharacterTeleport(cc);
                        }
                        TeleportTo.isBeingTeleportedTo = true;
                    }
                    else
                    {
                        Debug.Log("Недостаточно BrainCoins для телепортации!");
                    }
                }
            }

            isBeingTeleportedTo = false;
        }
    }
}
