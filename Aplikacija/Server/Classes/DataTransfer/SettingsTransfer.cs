using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Classes;
using Server.Models;

namespace Server.Classes.DataTransfer
{
    public class SettingsTransfer
    {
        public int Id;
        public int Avatar;
        public float Sound;
        public string PrefColor;
        public bool ActionConfirm;

        public SettingsTransfer(Settings set)
        {
            Id = set.Id;
            Avatar = set.Avatar;
            Sound = set.Sound;
            PrefColor = set.PrefColor;
            ActionConfirm = set.ActionConfirm;
        }
    }
}
