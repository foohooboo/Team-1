using System.Collections.Generic;
using System.Net;

namespace Shared.Comms.MailService
{
    public class PostOffice
    {
        public Dictionary<EndPoint,PostBox> PostBoxes
        {
            get; set;
        }

        public PostOffice()
        {
            PostBoxes = new Dictionary<EndPoint, PostBox>();
        }

        public bool HasPostBox()
        {
            return PostBoxes.Count > 0;
        }

        public void AddBox(EndPoint address)
        {
            PostBoxes.Add(address, new PostBox(address));
        }

        public PostBox GetBox(EndPoint address)
        {
            if (PostBoxes.TryGetValue(address, out PostBox postBox))
            {
                return postBox;
            }

            return null;
        }

        public void RemoveBox(EndPoint address)
        {
            PostBoxes.Remove(address);
        }
    }
}