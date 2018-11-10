using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace NancyAjaxTest
{
    public class MessagesView
    {
        public List<MessageView> List { get; private set; }

        public MessagesView()
        {
            List = new List<MessageView>();
        }

        public MessagesView(IEnumerable<Message> ms)
        {
            List = new List<MessageView>();

            foreach (var m in ms)
            {
                List.Add(new MessageView(m));
            }
        }
    }

    public class MessageView
    {
        public string Author { get; private set; }
        public string Text { get; private set; }
        public string Time { get; private set; }

        public MessageView(Message m)
        {
            Author = m.Author;
            Text = m.Text;
            Time = m.Time.ToString("dd.MM.yyyy HH:mm");
        }
    }

    public class UpdateModule : NancyModule
    {
        public string ReadBody()
        {
            using (var reader = new System.IO.StreamReader(Context.Request.Body))
            {
                return reader.ReadToEnd();
            }
        }

        public string Sanatize(string input)
        {
            return input
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("\r\n", "<br/>")
                .Replace("\r", "<br/>")
                .Replace("\n", "<br/>");
        }

        public string SanatizeName(string input)
        {
            return input
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("\r\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty);
        }

        public UpdateModule()
        {
            Get["/"] = parameters =>
            {
                return Response.AsRedirect("/login");
            };
            Get["/login"] = parameters =>
            {
                return View["View/login.sshtml"];
            };
            Post["/login"] = parameters =>
            {
                return Response.AsRedirect("/update/" + SanatizeName(ReadBody()));
            };
            Get["/update/{uid}"] = parameters =>
            {
                return View["View/update.sshtml", parameters.uid];
            };
            Get["/old/{uid}"] = parameters =>
            {
                var uid = parameters.uid;
                var user = Network.Instance.AddUser(uid);
                var msgs = user.ReadOld();
                return View["View/msgs.sshtml", new MessagesView(msgs)];
            };
            Get["/new/{uid}"] = parameters =>
            {
                var uid = parameters.uid;
                var user = Network.Instance.GetUser(uid);
                if (user.WaitForNew())
                {
                    var msgs = user.ReadNew();
                    return View["View/msgs.sshtml", new MessagesView(msgs)];
                }
                else
                {
                    return View["View/msgs.sshtml", new MessagesView()];
                }
            };
            Post["/write/{uid}"] = parameters =>
            {
                var uid = parameters.uid;
                var user = Network.Instance.GetUser(uid);
                var msg = new Message(Sanatize(ReadBody()), user.Name);
                Network.Instance.ForEachUser(u => u.AddMessage(msg));
                return null;
            };
        }
    }
}
