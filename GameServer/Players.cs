using System;
using System.Linq;
using Common;
using Lidgren.Network;

namespace GameServer
{
    public class Players
    {
        public static Ownership OtherPlayer(Ownership player)
        {
            if(player == Ownership.Board) throw new ArgumentException("Can't OtherPlayer from Ownership.Board");
            return player == Ownership.FirstPlayer ? Ownership.SecondPlayer : Ownership.FirstPlayer;
        }
        
        private NetConnection[] _Players;
        public Players()
        {
            this._Players = new NetConnection[2];
            this._Players[0] = null;
            this._Players[1] = null;
            
        }

        // Should return a message
        public Ownership SignUp(NetConnection user)
        {
            Ownership ans = Ownership.Board;
            lock (_Players)
            {
                if (_Players.Contains(user)){
                    Console.Write("{0} AlreadyConnected and trying to SignUp again!", user.ToString());
                    ans = GetPlayerFromConnection(user);
                }
                else
                {
                    // Assign user with a Player ("FirstPlayer" etc)
                    int player_index = 0;
                    while (player_index < _Players.Length)
                    {
                        if (_Players[player_index] == null)
                        {
                            _Players[player_index] = user;
                            ans = (Ownership)(player_index+1);
                            Console.WriteLine(String.Format("Signed user {0} to Player {1}", user.ToString(), ans.ToString()));
                            break;
                        }
                        player_index++;
                    }

                    if (player_index <= _Players.Length)
                    {
                        Console.WriteLine("Too many players connected! attempt by {0}, IP: {1}. Connection will be refused.", user.ToString() ,user.RemoteEndPoint.ToString());
                        ans = Ownership.Board;
                    } 
                }
            }

            return ans;
        }

        public void SignOut(NetConnection user)
        {
            lock (_Players)
            {
                Ownership player = GetPlayerFromConnection(user);
                if (player == Ownership.Board)
                {
                    Console.WriteLine("Can't Signout {0}, IP: {1}, No such user!", user.ToString(),
                        user.RemoteEndPoint.ToString());
                }
                else
                {
                    _Players[(int)player - 1] = null;
                    Console.WriteLine(String.Format("Signed out user {0} to Player {1}", user.ToString(), player.ToString()));    
                }
            }
        }

        public NetConnection GetConnection(Ownership player)
        {
            lock (_Players)
            {
                return _Players[(int) player - 1];   
            }
        }

        public Ownership GetPlayerFromConnection(NetConnection user)
        {
            if(_Players[0] == user)
            {
                return Ownership.FirstPlayer;
            }
            else if (_Players[1] == user)
            {
                return Ownership.SecondPlayer;
            }
            else return Ownership.Board;
        }

        public NetConnection GetOtherUserConnection(NetConnection given_user)
        {
            var this_user = GetPlayerFromConnection(given_user);
            return this_user == Ownership.FirstPlayer
                ? GetConnection(Ownership.SecondPlayer)
                : GetConnection(Ownership.FirstPlayer);
        }
        public override string ToString()
        {
            String ans = "";
            NetConnection conn = GetConnection(Ownership.FirstPlayer);
            if (conn != null)
            {
                ans = String.Concat(ans, String.Format("FirstPlayer: {0} status: {1}  |", conn.ToString(), conn.Status));
            }
            else
            {
                ans = String.Concat(ans, "Awaiting FirstPlayer..  |");
            }
            
            conn = GetConnection(Ownership.SecondPlayer);

            if (conn != null)
            {    
                ans = String.Concat(ans, String.Format("SecondPlayer: {0} status: {1}", conn.ToString(), conn.Status));
            }
            else
            {
                ans = String.Concat(ans, "Awaiting SecondPlayer..");
            }
            
            return ans;
        }
    }
}