//using AC.Core;
////using Facebook.Unity;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class FacebookManager : Singleton<FacebookManager>
//{
    
//    public void Init()
//    {
//        if(!FB.IsInitialized)
//        {
//            FB.Init(OnCompleted);
//        }
//        else
//        {
//            FB.ActivateApp();
//        }
//    }

//    private void OnCompleted()
//    {
//        if(FB.IsInitialized)
//        {
//            FB.ActivateApp();
//        }
//    }
//}
