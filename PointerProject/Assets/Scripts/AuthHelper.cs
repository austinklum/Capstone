using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class AuthHelper
{
  public static void SetAuthHeader(this UnityWebRequest request)
  {
        SetAuthHeader(request, "Username", "Password");
  }

    public static void SetAuthHeader(UnityWebRequest request, string username, string password)
    {
        byte[] textAsBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
        string encoded64 = Convert.ToBase64String(textAsBytes);
        request.SetRequestHeader("Authorization", $"Basic {encoded64}");
    }
}
