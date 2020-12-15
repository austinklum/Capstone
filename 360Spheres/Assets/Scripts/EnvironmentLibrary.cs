using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
using Valve.VR.Extras;
using System.Diagnostics;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Linq;

public class EnvironmentLibrary : MonoBehaviour
{
    private String AllLocationsEndpoint = "http://localhost/QuestionAnswer/GetLocations.php";
    public List<Environment> Environments;
    public SkyBoxController skyBoxController;

    private Texture locationTexture;

    public IEnumerator GetLocations()
    {
        UnityEngine.Debug.Log("GetLocations() requested!"); 
        UnityWebRequest locationsRequest = UnityWebRequest.Get(AllLocationsEndpoint);
        yield return locationsRequest.SendWebRequest();

        string response = System.Text.Encoding.UTF8.GetString(locationsRequest.downloadHandler.data);
        if (locationsRequest.error != null)
        {
            UnityEngine.Debug.Log("There was an error getting the location: " + locationsRequest.error);
        }
        else
        {
            List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(response);
            ConvertToEnvironments(locations);
        }
    }

    public void ConvertToEnvironments(List<Location> locations)
    {
        UnityEngine.Debug.Log("ConvertToEnvironments() requested!");
        foreach (Location location in locations)
        {
            Environment environment = new Environment()
            {
                LocationId = location.locationId,
                WorldRotation = 0,
                Name = location.name
            };
            StartCoroutine(LoadImageFromUrl(environment, location.url));
            StartCoroutine(LoadQuestionsByLocation(environment, environment.LocationId));
            Environments.Add(environment);
        }
    }

    private IEnumerator LoadImageFromUrl(Environment environment, string url)
    {
        UnityEngine.Debug.Log("LoadImageFromUrl() requested!");
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(url);
        yield return imageRequest.SendWebRequest();

        if (imageRequest.error != null)
        {
            UnityEngine.Debug.Log("There was an error getting the image: " + imageRequest.error);
        }
        else
        {
            environment.Background = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
        }
    }

    private IEnumerator LoadQuestionsByLocation(Environment environment, int locationId)
    {
        string getQuestionsURL = "http://localhost/QuestionAnswer/GetQuestions.php";
        UnityWebRequest questionsRequest = UnityWebRequest.Get(getQuestionsURL);
        yield return questionsRequest.SendWebRequest();

        string response = System.Text.Encoding.UTF8.GetString(questionsRequest.downloadHandler.data);
        if (questionsRequest.error != null)
        {
            UnityEngine.Debug.Log("There was an error getting the question: " + questionsRequest.error);
        }
        else
        {
            List<Question> questions = JsonConvert.DeserializeObject<List<Question>>(response);
            environment.Questions = questions.Where(q => q.locationId == environment.LocationId);
        }
    }

    public void PrintList()
    {
        UnityEngine.Debug.Log("Printing List...");
       int count = 0;
        foreach(Environment environment in Environments)
        {
            UnityEngine.Debug.Log(count++ + " : " + environment.Name);
        }
    }

}

[Serializable]
public class Environment
{
    public int LocationId;
    public int WorldRotation;// { get => worldRotation; set => worldRotation = value; }
    public Texture Background;// { get => background; set => background = value; }
    public string Name;
    public IEnumerable<Question> Questions;
}

public class Location
{
    public int locationId;
    public string name;
    public string url;
}

