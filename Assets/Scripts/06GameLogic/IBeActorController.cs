
public interface IBeActorController
{
    void SetOwner(BeActor actor);
    void OnEnter();
    void OnTick(int delta);
    bool IsEnd();
    bool AutoRemove();
}