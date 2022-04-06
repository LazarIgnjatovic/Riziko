using Assets.Classes.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Classes
{
    public class Settings
    {
        public int Id;
        public int Avatar;
        public float Sound;
        public string PrefColor;
        public bool ActionConfirm;

        public Settings() { }
        public Settings(SettingsTransfer set)
        {
            Id = set.Id;
            Avatar = set.Avatar;
            Sound = set.Sound;
            PrefColor = set.PrefColor;
            ActionConfirm = set.ActionConfirm;
        }
        public Settings(int av, float s, string pc, bool ac)
        {
            Avatar = av;
            Sound = s;
            PrefColor = pc;
            ActionConfirm = ac;
        }

        public void UpdateSettings(int av, float s, string pc, bool ac)
        {
            Avatar = av;
            Sound = s;
            PrefColor = pc;
            ActionConfirm = ac;
        }

    }
}
