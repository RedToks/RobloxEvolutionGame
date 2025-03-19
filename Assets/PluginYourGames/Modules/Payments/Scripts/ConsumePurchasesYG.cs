using UnityEngine;

namespace YG
{
    public class ConsumePurchasesYG : MonoBehaviour
    {
        private static bool consumed;

        private void Start()
        {
            YG2.GameReadyAPI();
            if (!consumed)
            {
                consumed = true;
                YG2.ConsumePurchases();
            }
        }
    }
}