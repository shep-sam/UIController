namespace SpaceCore.UIController
{
    public interface IWindow
    {
        WindowTag WindowTag { get; }

        void Close();
    }
}