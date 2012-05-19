﻿/*
Copyright 2011 MCForge
Dual-licensed under the Educational Community License, Version 2.0 and
the GNU General Public License, Version 3 (the "Licenses"); you may
not use this file except in compliance with the Licenses. You may
obtain a copy of the Licenses at
http://www.opensource.org/licenses/ecl2.php
http://www.gnu.org/licenses/gpl-3.0.html
Unless required by applicable law or agreed to in writing,
software distributed under the Licenses are distributed on an "AS IS"
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
or implied. See the Licenses for the specific language governing
permissions and limitations under the Licenses.
*/
using System.Threading;
using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Utils;
using MCForge.Utils.Settings;

namespace CommandDll
{
    public class CmdVoteKick : ICommand
    {
        public string Name { get { return "VoteKick"; } }
        public CommandTypes Type { get { return CommandTypes.Mod; } }
        public string Author { get { return "Arrem"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 50; } }


        public void Use(Player p, string[] args)
        {
            if (Server.voting) { p.SendMessage("A vote is already in progress!"); return; }
            Player who = null;
            if (args.Length == 0) { who = null; }
            else { who = Player.Find(args[0]); }
            if (who == null) { p.SendMessage("Cannot find that player!"); return; }
            if (Server.devs.Contains(who.Username)) { p.SendMessage("You can't votekick a MCForge Developer!"); return; }
            Server.kicker = who;
            ResetVotes();
            Server.voting = true;
            Server.kickvote = true;
            Player.UniversalChat("VOTE: Kick " + who.Username + "?");
            Player.UniversalChat("Use: %aYes " + ServerSettings.GetSetting("DefaultColor") + "or %cNo " + ServerSettings.GetSetting("DefaultColor") + "to vote!");
            Thread.Sleep(15000);
            Player.UniversalChat("The votes are in! %aYes: " + Server.YesVotes + " %cNo: " + Server.NoVotes + ServerSettings.GetSetting("DefaultColor") + "!");
            if (Server.YesVotes > Server.NoVotes) { who.Kick("Votekick'd"); return; }
            else if (Server.NoVotes > Server.YesVotes || Server.YesVotes == Server.NoVotes) { Player.UniversalChat("Looks like " + who.Username + " is staying!"); return; }
			Server.ForeachPlayer(delegate(Player pl)
			{
                pl.ExtraData.CreateIfNotExist("Voted", false);
                pl.ExtraData["Voted"] = false;
			});
            Server.voting = false;
            ResetVotes();
        }

        public void Help(Player p)
        {
            p.SendMessage("/votekick <player> - Starts a 15 second vote to kick <player>");
        }

        public void Initialize()
        {
            Command.AddReference(this, "votekick");
        }
        public void ResetVotes() { Server.YesVotes = 0; Server.NoVotes = 0; }
    }
}
