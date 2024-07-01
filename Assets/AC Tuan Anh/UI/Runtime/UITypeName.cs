using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.GameTool.UI
{
    public enum UITypeName
    {      
        Loading,
        HomeUI,
        MainUI,
        SettingUI,
        ResultUI,
        RankUI,
        NoInternetPopup,
        NoAdsPopup,
        RatePopup,
        Transition,
        BonusUI,
        BreakUI,
    }
    public enum TransitionType
    {
        None,
        Fade,
        Pop,
        MoveFromLeft,
        MoveFromRight,
        MoveFromTop,
        MoveFromBottom,
    }
}


