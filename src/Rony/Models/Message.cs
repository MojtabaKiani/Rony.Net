namespace Rony.Models
{
    public class Message
    {
        public Message(byte[] body, object sender)
        {
            Body = body;
            Sender = sender;
        }

        public Message(string body, object sender) : this (body.GetBytes(),sender)
        {
        }

        public byte[] Body { get; set; }
        public object Sender { get; set; }
        public string BodyString => Body.GetString();
    }
}
