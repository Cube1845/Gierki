using Games.Application.TicTacToe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.Infrastructure;

public sealed class WaitingUsers
{
    private static readonly object lockObject = new object();

    private static WaitingUsers instance = null;

    private List<User> users;

    private WaitingUsers()
    {
        users = new List<User>();
    }

    public static WaitingUsers Instance
    {
        get
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = new WaitingUsers();
                }
                return instance;
            }
        }
    }

    public void AddUser(User user)
    {
        lock (lockObject)
        {
            users.Add(user);
        }
    }

    public List<User> GetUsers()
    {
        lock (lockObject)
        {
            return new List<User>(users);
        }
    }

    public void RemoveUser(User user)
    {
        lock (lockObject)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Username == user.Username)
                {
                    users.RemoveAt(i);
                }
            }
        }
    }

    public void ClearUsers()
    {
        lock (lockObject)
        {
            users = new List<User>();
        }
    }
}