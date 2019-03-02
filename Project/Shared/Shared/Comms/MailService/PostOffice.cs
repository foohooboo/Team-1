using System.Collections.Generic;
using System.Net;

namespace Shared.Comms.MailService
{
    public class PostOffice
    {
        public Dictionary<string,PostBox> PostBoxes
        {
            get; set;
        }

        public PostOffice()
        {
            PostBoxes = new Dictionary<string, PostBox>();
        }

        public bool HasPostBox()
        {
            return PostBoxes.Count > 0;
        }

        public void AddBox(PostBox postBox)
        {
            PostBoxes.Add(postBox.EndPoint.ToString(), postBox);
        }

        public PostBox GetBox(string address)
        {
            if (PostBoxes.TryGetValue(address, out PostBox postBox))
            {
                return postBox;
            }

            return null;
        }

        public void RemoveBox(string address)
        {
            PostBoxes.Remove(address);
        }
    }
}