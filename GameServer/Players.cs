using System;
using System.Linq;
using Common;
using Lidgren.Network;

namespace GameServer
{
    public class Players
    {
        private NetConnection[] _Players;
        public Players()
        {
            this._Players = new NetConnection[2];
            this._Players[0] = null;
            this._Players[1] = null;
            
        }

        // Should return a message
        public NetOutgoingMessage SignUp(NetConnection user, NetOutgoingMessage msg)
        {
            lock (_Players)
            {
                if (_Players.Contains(user)){
                    msg.Write("AlreadyConnected!");
                    return msg;
                }

                int player_index = 0;
                while (player_index < _Players.Length)
                {
                    if (_Players[player_index] == null)
                    {
                        _Players[player_index] = user;
                        msg.Write(((Ownership)(player_index+1)).ToString());
                        Console.WriteLine(String.Format("Signed user {0} to Player {1}", user.ToString(), ((Ownership)(player_index+1)).ToString()));
                        break;
                    }
                    player_index++;
                }
                if(player_index == _Players.Length)
                    msg.Write("Two Players already connected!");
            }

            return msg;
        }

        public NetOutgoingMessage SignOut(NetConnection user, NetOutgoingMessage msg)
        {
            lock (_Players)
            {
                int player_index = 0;
                while (player_index < _Players.Length)
                {
                    if (_Players[player_index] == user)
                    {
                        _Players[player_index] = null;
                        msg.Write("Sign out: "+((Ownership)(player_index+1)).ToString());
                        Console.WriteLine(String.Format("Signed out user {0} to Player {1}", user.ToString(), ((Ownership)(player_index+1)).ToString()));
                        break;
                    }
                    player_index++;
                }
                if(player_index == _Players.Length)
                    msg.Write("Can't Signout, No such user!");

                return msg;
            }
        }

        public NetConnection GetConnection(Ownership player)
        {
            lock (_Players)
            {
                return _Players[(int) player - 1];   
            }
        }

        public override string ToString()
        {
            String ans = "";
            NetConnection conn = GetConnection(Ownership.FirstPlayer);
            if (conn != null)
            {
                ans.Concat(String.Format("FirstPlayer: {0} status: {1}  |", conn.ToString(), conn.Status));
            }
            else
            {
                ans.Concat("Awaiting FirstPlayer..  |");
            }
            
            conn = GetConnection(Ownership.SecondPlayer);

            if (conn != null)
            {    
                ans.Concat(String.Format("SecondPlayer: {0} status: {1}", conn.ToString(), conn.Status));
            }
            else
            {
                ans.Concat("Awaiting SecondPlayer..");
            }
            
            return ans;
        }
    }
}