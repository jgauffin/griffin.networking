using System;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// Extension in FreeSWITCH
    /// </summary>
    public class Extension
    {
    }

    public class PlainNumber : IPhoneNumber
    {
        public PlainNumber(string number)
        {
            if (number == null) throw new ArgumentNullException("number");
            Value = number;
        }

        public string Value { get; private set; }

        public override string ToString()
        {
            return Value;
        }
    }

    public class User
    {
        public User(string userName, string domainName)
        {
            UserName = userName;
            DomainName = domainName;
        }

        public string UserName { get; set; }
        public string DomainName { get; set; }
    }

    public class Channel
    {
        public Channel(UniqueId channelId)
        {
            Id = channelId;
            State = ChannelState.Unknown;
            CallerId = new CallerId("", new PlainNumber(""));
        }

        public UniqueId Id { get; set; }
        public ChannelDirection Direction { get; set; }
        public DateTime CreatedAt { get; set; }
        public ChannelState State { get; set; }
        public CallerId CallerId { get; set; }
        public string Destination { get; set; }
        public CallState CallState { get; set; }
        public Channel OtherChannel { get; set; }
        public User User { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1,-20} -> {2,-20} [{3,-10}/CS:{4}]", Id, CallerId, Destination, State, CallState);
        }
    }

    public class Application
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }

    public enum ChannelDirection
    {
        Inbound,
        Outbound
    }

    public enum CallState
    {
        Down = 0,
        Dialing = 1,
        Ringing = 2,
        Early,
        Active,
        Held,
        Hangup = 10
    }

    /*
     <uuid>cdefc67c-6865-4f02-b229-6c4066b2a376</uuid>
    <direction>inbound</direction>
    <created>2011-12-12 20:20:43</created>
    <created_epoch>1323717643</created_epoch>
    <name>sofia/internal/1003@192.168.1.79:5060</name>
    <state>CS_EXECUTE</state>
    <cid_name>1003</cid_name>
    <cid_num>1003</cid_num>
    <ip_addr>192.168.1.79</ip_addr>
    <dest>1002</dest>
    <application>bridge</application>
    <application_data>user/1002@192.168.1.79</application_data>
    <dialplan>XML</dialplan>
    <context>default</context>
    <read_codec>PCMU</read_codec>
    <read_rate>8000</read_rate>
    <read_bit_rate>64000</read_bit_rate>
    <write_codec>PCMU</write_codec>
    <write_rate>8000</write_rate>
    <write_bit_rate>64000</write_bit_rate>
    <secure></secure>
    <hostname>jobbpc</hostname>
    <presence_id>1003@192.168.1.79</presence_id>
    <presence_data></presence_data>
    <callstate>ACTIVE</callstate>
    <callee_name>Outbound Call</callee_name>
    <callee_num>1002</callee_num>
    <callee_direction>SEND</callee_direction>
    <call_uuid>cdefc67c-6865-4f02-b229-6c4066b2a376</call_uuid>
    <sent_callee_name>Outbound Call</sent_callee_name>
    <sent_callee_num>1002</sent_callee_num>*/
}