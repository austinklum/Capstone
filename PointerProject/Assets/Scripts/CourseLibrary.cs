using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Valve.Newtonsoft.Json;

public class CourseLibrary : MonoBehaviour
{
    private string AllCoursesEndpoint = "https://localhost:44315/ImmersiveQuizAPI/AllCourses";
    public Course[] Courses;

    public IEnumerator GetCourses()
    {
        List<Course> courses = new List<Course>();
        UnityWebRequest request = UnityWebRequest.Get(AllCoursesEndpoint);
        request.SetAuthHeader();
        yield return request.SendWebRequest();

        string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
        if (request.error != null)
        {
            UnityEngine.Debug.Log("There was an error getting the courses: " + request.error);
        }
        else
        {
            courses = JsonConvert.DeserializeObject<List<Course>>(response);
        }
        yield return new WaitUntil(() => courses?.Count > 0);
        Courses = courses.ToArray();

        UpdateTxtCourseName(courses[0].Name);
    }

    public void UpdateTxtCourseName(string courseName)
    {
        GameObject courseCanvas = GameObject.FindGameObjectWithTag("courseCanvasTag");
        Transform txtCourseName = courseCanvas.transform.Find("txtCourseName");
        txtCourseName.GetComponentInChildren<Text>().text = courseName;
    }
}


