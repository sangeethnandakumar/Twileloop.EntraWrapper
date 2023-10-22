namespace Twileloop.EntraWrapper
{
    public interface IEntraEventLogger
    {
        void OnInfo(string message);
        void OnSuccess(string message);
        void OnFailure(string message);
    }
}
