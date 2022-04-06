using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Classes
{
    public abstract class User
    {
        public string username;
        public Settings mySettings;
        public string elo;

        protected MenuUI menu;

        public User()
        {
        }
        public User(string name, Settings sett, MenuUI ui)
        {
            username = name;
            mySettings = sett;
            menu = ui;
        }
        public void JoinCasual()
        {
            PlayerPrefs.SetString("username", this.username);
            PlayerPrefs.SetString("gameType","Casual");
            PlayerPrefs.SetInt("isHost",0);
            SceneManager.LoadScene("Lobby");

        }
        public void JoinFriendly()
        {
            PlayerPrefs.SetString("username", this.username);
            PlayerPrefs.SetString("gameType", "Friendly");
            PlayerPrefs.SetInt("isHost", 0);
            SceneManager.LoadScene("Lobby");
        }
        public abstract void JoinRanked();
        public abstract void HostCasual ();
        public abstract void HostFriendly();
        public abstract void HostRanked();
        public void UpdateSettings(int avatar, float sound, string color, bool aConf)
        {
            mySettings.UpdateSettings(avatar, sound, color, aConf);
        }
    }
}
