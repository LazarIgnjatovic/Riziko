using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Classes
{
    public class Guest : User
    {
        public Guest() { }
        public Guest(string name, Settings sett, MenuUI ui) : base(name, sett, ui)
        {

        }

        public override void HostCasual()
        {
            menu.ClosePannels();
            menu.Notify(false, "Sign in or create account to access the full game!");
        }

        public override void HostFriendly()
        {
            menu.ClosePannels();
            menu.Notify(false, "Sign in or create account to access the full game!");
        }

        public override void HostRanked()
        {
            menu.ClosePannels();
            menu.Notify(false, "Sign in or create account to access the full game!");
        }

        public override void JoinRanked()
        {
            menu.ClosePannels();
            menu.Notify(false, "Sign in or create account to access the full game!");
        }
    }
}
