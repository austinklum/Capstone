using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;

public class EnvironmentLibrary : MonoBehaviour
{
    private String AllLocationsEndpoint = "https://localhost:44315/ImmersiveQuizAPI/AllLocations";
    private string ImagesFileLocation = "C:\\CapstoneQuestionAdder\\ImmersiveQuiz\\ImmersiveQuiz\\ImmersiveQuiz\\wwwroot";
    public List<Environment> Environments;
    public SkyBoxController skyBoxController;

    private Texture locationTexture;

    public IEnumerator GetLocations()
    {
        UnityEngine.Debug.Log("GetLocations() requested!"); 
        UnityWebRequest locationsRequest = UnityWebRequest.Get(AllLocationsEndpoint);
        locationsRequest.SetAuthHeader();
        yield return locationsRequest.SendWebRequest();

        string response = System.Text.Encoding.UTF8.GetString(locationsRequest.downloadHandler.data);
        if (locationsRequest.error != null)
        {
            UnityEngine.Debug.Log("There was an error getting the location: " + locationsRequest.error);
        }
        else
        {
            List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(response);
            StartCoroutine(ConvertToEnvironments(locations));
        }
    }

    public IEnumerator ConvertToEnvironments(List<Location> locations)
    {
        UnityEngine.Debug.Log("ConvertToEnvironments() requested!");
        foreach (Location location in locations)
        {
            Environment environment = new Environment()
            {
                LocationId = location.LocationId,
                WorldRotation = 0,
                Name = location.Name
            };
            StartCoroutine(LoadEnvironmentContent(location, environment));
            yield return null;
        }
    }

    private IEnumerator LoadEnvironmentContent(Location location, Environment environment)
    {
        StartCoroutine(LoadImageFromUrl(environment, location.ImagePath));
        yield return new WaitUntil(() => environment.Background != null);

        StartCoroutine(LoadQuestionsByLocation(environment, environment.LocationId));
        yield return new WaitUntil(() => environment.Questions?.Count > 0);
        Environments.Add(environment);
    }

    private IEnumerator LoadImageFromUrl(Environment environment, string url)
    {
        UnityEngine.Debug.Log("LoadImageFromUrl() requested!");
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(ImagesFileLocation + url);
        imageRequest.SetAuthHeader();
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
        string getQuestionsURL = "https://localhost:44315/ImmersiveQuizAPI/LocationsQuestions/" + locationId;
        UnityWebRequest questionsRequest = UnityWebRequest.Get(getQuestionsURL);
        questionsRequest.SetAuthHeader();
        yield return questionsRequest.SendWebRequest();

        string response = System.Text.Encoding.UTF8.GetString(questionsRequest.downloadHandler.data);
        if (questionsRequest.error != null)
        {
            UnityEngine.Debug.Log("There was an error getting the question: " + questionsRequest.error);
        }
        else
        {
            List<Question> questions = JsonConvert.DeserializeObject<List<Question>>(response);
            environment.Questions = questions;
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
    public List<Question> Questions;
}

public class Location
{
    public int LocationId;
    public string Name;
    public string ImagePath;
}

public class Question
{
    public int QuestionId;
    public string Content;
    public int LocationId;
    public List<Answer> Answers;
}
public class Answer
{
    public int AnswerId;
    public int QuestionId;
    public string Content;
    public bool IsCorrect;
}
