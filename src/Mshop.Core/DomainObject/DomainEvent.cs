namespace Mshop.Core.DomainObject
{
    public abstract class DomainEvent
    {
        public DateTime OccuredOn { get; set; }
        public string EventName { get; protected set; }
        public DomainEvent()
        {
            OccuredOn = DateTime.Now;
            EventName = GetType().Name;
        }

    }
}
