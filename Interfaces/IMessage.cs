using Singer.Domain;

namespace Singer.Interfaces;

public interface IMessage
{
    public void SendEmail(Email request);
}
