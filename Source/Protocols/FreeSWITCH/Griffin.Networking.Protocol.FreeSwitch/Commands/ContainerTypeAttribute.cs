using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Commands
{
    /// <summary>
    /// Kind of carrier used to send the command.
    /// </summary>
    public class ContainerTypeAttribute : Attribute
    {
        private readonly ContainerType _containerType;

        public ContainerTypeAttribute(ContainerType containerType)
        {
            _containerType = containerType;
        }

        public ContainerType ContainerType1
        {
            get { return _containerType; }
        }
    }

    public enum ContainerType
    {
        SendMsg,
        Api,
        Event
    }
}