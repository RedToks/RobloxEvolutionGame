using System.Collections.Generic;

[System.Serializable]
public class PetSaveData
{
    public List<PetData> allPets = new List<PetData>();
    public List<PetData> selectedPets = new List<PetData>();
}
