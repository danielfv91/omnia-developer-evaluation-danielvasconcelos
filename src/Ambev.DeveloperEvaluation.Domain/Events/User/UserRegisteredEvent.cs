namespace Ambev.DeveloperEvaluation.Domain.Events.User
{
    public class UserRegisteredEvent
    {
        public Entities.User User { get; }

        public UserRegisteredEvent(Entities.User user)
        {
            User = user;
        }
    }
}
