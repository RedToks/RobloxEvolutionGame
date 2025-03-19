using System.Collections.Generic;
using YG;

namespace YG
{
    public partial class SavesYG
    {
        public float selectedMultiplier = 1;
        public List<float> unlockedMultipliers = new List<float>();

        public float bonusTimerRemaining = 0f;
        public string lastExitTime = "";

        public long brainCurrency = 0;
        public long coinCurrency = 0;

        public List<PetData> allPets = new List<PetData>();
        public List<PetData> selectedPets = new List<PetData>();

        public int selectedHairIndex = 0; // Индекс выбранной прически
        public string purchasedHairs = "1"; // Купленные прически (1 - куплена, 0 - не куплена)

        public int selectedSkin = 0;
        public string purchasedSkins = "1"; // Первый скин всегда куплен

        public int clickMultiplier = 1000; // 🔹 Храним множитель кликов (по умолчанию 1.0)
        public int petMultiplier = 1000;   // 🔹 Храним множитель питомцев (по умолчанию 1.0)

        public bool isObjectsDisabled = false;
        public bool isDoubleEarningsActive = false;
        public bool isDoubleNeuroEarningsActive = false;
        public bool isDoubleEarningsDisabled = false;
        public bool isDoubleNeuroEarningsDisabled = false;

        public int playedTime = 0;

        public string nextClaimTime = "";
        public int currentDay = 1;
        public Dictionary<int, bool> rewardClaimed = new Dictionary<int, bool>();
        public SavesYG()
        {
            for (int i = 1; i <= 7; i++)
            {
                rewardClaimed[i] = false;
            }
        }
        public string lastSpinTime = "";

        public Dictionary<int, int> questValues = new Dictionary<int, int>();
        public Dictionary<int, bool> questClaimed = new Dictionary<int, bool>();
    }
}

