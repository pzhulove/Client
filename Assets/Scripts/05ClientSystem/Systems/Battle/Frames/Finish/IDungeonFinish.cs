namespace GameClient
{
    public interface IDungeonFinish
    {
        void SetName(string name);

        void SetBestTime(int time);

        void SetCurrentTime(int time);

        void SetDrops(ComItemList.Items[] items);

        void SetExp(ulong exp);

        void SetLevel(int lvl);

        void SetDiff(int diff);

        void SetFinish(bool isFinish);
    }
}
