using ChatApp3.Models;

namespace ChatApp3.Hubs
{
    public interface IChatHub
    {
        void ReceiveMessage(Message message);
    }
}
