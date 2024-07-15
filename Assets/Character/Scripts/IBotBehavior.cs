public interface IBotBehavior
{
    public BotController Controller { get; set; }
    public void Start();
    public void Update();
    public void Exit();
}