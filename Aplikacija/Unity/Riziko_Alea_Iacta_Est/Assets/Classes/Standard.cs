using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Classes
{
    public class Standard : User
    {
        public Standard() { }
        public Standard(string name, Settings sett,MenuUI ui):base(name,sett,ui)
        {

        }
        public override void JoinRanked()
        {
            PlayerPrefs.SetString("username", this.username);
            PlayerPrefs.SetString("gameType", "Ranked");
            PlayerPrefs.SetInt("isHost", 0);
            SceneManager.LoadScene("Lobby");
        }

        public override void HostCasual()
        {
            PlayerPrefs.SetString("username", this.username);
            PlayerPrefs.SetString("gameType", "Casual");
            PlayerPrefs.SetInt("isHost", 1);
            SceneManager.LoadScene("Lobby");
        }

        public override void HostFriendly()
        {
            PlayerPrefs.SetString("username", this.username);
            PlayerPrefs.SetString("gameType", "Friendly");
            PlayerPrefs.SetInt("isHost", 1);
            SceneManager.LoadScene("Lobby");
        }

        public override void HostRanked()
        {
            PlayerPrefs.SetString("username", this.username);
            PlayerPrefs.SetString("gameType", "Ranked");
            PlayerPrefs.SetInt("isHost", 1);
            SceneManager.LoadScene("Lobby");
        }
    }
}
