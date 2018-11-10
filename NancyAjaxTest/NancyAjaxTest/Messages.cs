using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace NancyAjaxTest
{
    public class Message
    {
        public string Text { get; private set; }
        public string Author { get; private set; }
        public DateTime Time { get; private set; }

        public Message(string text, string author)
        {
            Text = text;
            Author = author;
            Time = DateTime.Now;
        }
    }

    public class User
    {
        public string Name { get; private set; }
        private List<Message> New { get; set; }
        private List<Message> Old { get; set; }
        private object _lock = new object();
        private AutoResetEvent _event;

        public User(string name)
        {
            Name = name;
            New = new List<Message>();
            Old = new List<Message>();
            _event = new AutoResetEvent(false);
        }

        public void AddMessage(Message message)
        {
            lock (_lock)
            {
                New.Add(message);
                _event.Set();
            }
        }

        public IEnumerable<Message> ReadNew()
        {
            lock (_lock)
            {
                var l = New.ToList();
                New.Clear();
                Old.AddRange(l);
                return l;
            }
        }

        public IEnumerable<Message> ReadOld()
        {
            lock (_lock)
            {
                return Old.ToList();
            }
        }

        public bool WaitForNew()
        {
            lock (_lock)
            {
                if (New.Count > 0)
                {
                    return true;
                }
            }

            _event.WaitOne(1000);

            lock (_lock)
            {
                if (New.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class Network
    {
        private static Network _instance;

        private Dictionary<string, User> Users { get; set; }

        private object _lock = new object();

        public Network()
        {
            Users = new Dictionary<string, User>();
        }

        public User GetUser(string uid)
        {
            lock (_lock)
            {
                return Users[uid];
            }
        }

        public User AddUser(string uid)
        {
            lock (_lock)
            {
                if (!Users.ContainsKey(uid))
                {
                    var u = new User(uid);
                    Users.Add(u.Name, u);
                    return u;
                }
                else
                {
                    return GetUser(uid);
                }
            }
        }

        public void ForEachUser(Action<User> action)
        {
            lock (_lock)
            {
                foreach (var u in Users.Values)
                {
                    action(u);
                }
            }
        }

        public static Network Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Network();
                }

                return _instance;
            }
        }
    }
}
