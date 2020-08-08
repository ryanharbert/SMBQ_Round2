[System.Serializable]
public class QuestData
{
    public string Type;
    public string Target;
    public int Progress;
    public int Complete;
    public int Reward;

    public bool Completed()
    {
        if(Progress >= Complete)
        {
            return true;
        }
        return false;
    }
}
