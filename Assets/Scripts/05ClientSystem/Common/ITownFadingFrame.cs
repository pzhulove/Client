
namespace GameClient
{
    internal interface ITownFadingFrame
    {
        int CurrentProgress
        {
            get;
        }

        void FadingOut(float fadeOutTime);
        void FadingIn(float fadeInTime);

        bool IsClosed();
        bool IsOpened();
    }
}