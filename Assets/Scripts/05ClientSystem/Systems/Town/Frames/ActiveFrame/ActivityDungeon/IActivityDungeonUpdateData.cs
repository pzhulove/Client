
namespace GameClient
{
    public interface IActivityDungeonUpdateData
    {
        bool IsChanged();
        
        void Update(float delta);

        void Reset();
    }
}
