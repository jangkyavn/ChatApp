﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ChatApp3.Models;
using Microsoft.AspNet.SignalR;

namespace ChatApp3.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        public void SendMessage(string receiverName, string message)
        {
            using (var db = new DataContext())
            {
                var mes = new Message()
                {
                    MessageID = Guid.NewGuid(),
                    Content = message,
                    DateCreated = DateTime.Now,
                    SenderName = Context.User.Identity.Name,
                    ReceiverName = receiverName,
                    ContentType = "text"
                };
                db.Messages.Add(mes);
                db.SaveChanges();

                Clients.User(receiverName).ReceiveMessage(mes);
                Clients.Caller.ReceiveMessage(mes);
            }
        }

        public override Task OnConnected()
        {
            using (var db = new DataContext())
            {
                if (string.IsNullOrEmpty(Context.User.Identity.Name))
                {
                    return base.OnConnected();
                }

                var user = db.Users.FirstOrDefault(u => u.UserName == Context.User.Identity.Name);
                if (user == null)
                {
                    user = new User()
                    {
                        UserName = Context.User.Identity.Name,
                        Connected = true
                    };
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                else
                {
                    user.Connected = true;
                    db.SaveChanges();
                }
            }

            return base.OnConnected();
        }
    }
}