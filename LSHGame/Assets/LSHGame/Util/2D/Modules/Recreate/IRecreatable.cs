namespace LSHGame.Util
{
    interface IRecreatable
    {
        void Recreate();
    }

    interface IRecreateBlocker
    {
        bool DoesRecreate();
    }
}
