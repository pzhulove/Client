
public interface IBeTownActionPlay
{
    void Init(GeActorEx geActor, string actionPath);
    void Update(float timeElapsed);
    void DeInit();
    bool PlayAction(string actionName);

    float GetActionTime(string name);
}