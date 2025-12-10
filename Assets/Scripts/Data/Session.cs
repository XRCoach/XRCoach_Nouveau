using System; // ⭐ Pour DateTime

[System.Serializable]
public class Session
{
    public DateTime date;
    public int totalReps;
    public float avgScore;
    
    public Session()
    {
        date = DateTime.Now;
        totalReps = 0;
        avgScore = 0f;
    }
}