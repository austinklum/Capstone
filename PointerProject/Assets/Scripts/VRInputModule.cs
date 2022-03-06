using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using Valve.Newtonsoft.Json;
using Valve.VR;

public class VRInputModule : BaseInputModule
{
    public Camera m_Camera;
    public SteamVR_Input_Sources m_TargetSource;
    public SteamVR_Action_Boolean m_ClickAction;

    private GameObject m_CurrentObject = null;
    private PointerEventData m_Data = null;

    public EnvironmentLibrary EnvironmentLibrary;
    public CourseLibrary CourseLibrary;

    public CanvasGroup QuestionCanvas;
    public SteamVR_Action_Boolean IsTouchpadPressed;

    private List<Question> questions = new List<Question>();
    private Question currentQuestion;
    public static GameObject currentObject;
    int currentID;

    private string username;
    public bool IsWorldStarted;

    [Serializable]
    public class NewEnvironment : UnityEvent<Environment> { }
    public NewEnvironment OnNewEnvironment;

    private int currentEnvironmentIndex = 0;

    public Timer timer;
    private static int maxNumberOfButtons = 6;
    private float maxScore = 0;
    private float pointScore = 0;
    private float timeScore = 0;
    private int attempts = 0;

    public CanvasGroup CourseCanvas;
    private int currentCourse;



    private GameObject[] buttons = new GameObject[maxNumberOfButtons + 1];

    protected override void Start()
    {
        base.Start();
        timer = gameObject.AddComponent<Timer>();

        IsTouchpadPressed.AddOnStateDownListener(TouchpadDown, SteamVR_Input_Sources.Any);
        IsTouchpadPressed.AddOnStateUpListener(TouchpadUp, SteamVR_Input_Sources.Any);

        for (int i = 0; i < maxNumberOfButtons; i++)
        {
            GameObject btn = GameObject.Find("btnAnswer" + (i + 1));
            buttons[i+1] = btn;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        m_Data = new PointerEventData(eventSystem);
    }

    private void TouchpadDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (IsWorldStarted)
        { 
            QuestionCanvas.alpha = 1;
        }
    }

    private void TouchpadUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (IsWorldStarted)
        {
            QuestionCanvas.alpha = 0;
        }
    }

    private void LoadStartScreen()
    {
        // Load Select List with Courses -- Maybe wait on this one...

        // Load Textbox with a Start Button

        // Register Start Button to Raycast/Pointer Event

        // Save Entered Name for grading later

        // Load Environments on the Start Event
    }

    public void StartWorld(string username)
    {
        this.username = username; // TODO: AK Make sure to sanitize this input

        
        IsWorldStarted = true;

        CourseCanvas.alpha = 1;
        // Get Courses
        StartCoroutine(CourseLibrary.GetCourses());
    }

    private IEnumerator LoadEnvironments()
    {
        UnityEngine.Debug.Log("LoadEnvironments() called!");
        StartCoroutine(EnvironmentLibrary.GetLocations());
        yield return new WaitUntil(() => IsEnvironment());
        Select();
    }

    bool IsEnvironment()
    {
        return EnvironmentLibrary.Environments.Count > 0;
    }
    bool IsCourses()
    {
        return CourseLibrary.Courses.Length > 0;
    }

    private void Select()
    {
        timeScore += timer.TimeAsScore();
        UnityEngine.Debug.Log("Select() called!");
        Environment env = EnvironmentLibrary.Environments[currentEnvironmentIndex];
        questions = env.Questions;
        maxScore += questions.Count() + 1;
        OnNewEnvironment.Invoke(env);
        getNextQuestion();
        timer.ResetTimer();
    }

    public void getNextQuestion()
    {
        if (questions.Count > 0)
        {
            currentQuestion = questions.First();
            updateTxtQuestion(currentQuestion.Content);
            updateBtnAnswers(currentQuestion.Answers);
            questions = questions.Where(q => q != currentQuestion).ToList();
        }
        else
        {
            currentEnvironmentIndex++;
            if (currentEnvironmentIndex >= EnvironmentLibrary.Environments.Count)
            {
                GameOver();
            }
            else
            {
                Select();
            }
        }
    }

    private void GameOver()
    {
         currentEnvironmentIndex = 0;
        updateTxtQuestion($"Congrats, {username} \n Time Score: {timeScore:0.00} \n Points Score: {pointScore:0.00} \n\n Total Score: {timeScore + pointScore:0.00} / {maxScore}");
        for (int i = 0; i < maxNumberOfButtons; i++)
        {
            GameObject btn = buttons[i + 1];
            btn.SetActive(false);
        }
        IsWorldStarted = false;
        QuestionCanvas.alpha = 1;
        StartCoroutine(SubmitScore(username, timeScore, pointScore));
     }

    private IEnumerator SubmitScore(string name, float timeScore, float pointScore)
    {
        var json = JsonConvert.SerializeObject(new { name, courseId = 1, timeScore, pointScore });
        var request = new UnityWebRequest("https://localhost:44315/ImmersiveQuizAPI/SubmitScore", "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        request.SetAuthHeader();

        yield return request.SendWebRequest();

        string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
        if (request.error != null)
        {
            UnityEngine.Debug.Log("There was an error getting the question: " + request.error);
        }
        else
        {
            //List<Question> questions = JsonConvert.DeserializeObject<List<Question>>(response);
            //environment.Questions = questions;
        }
    }

    private void updateTxtQuestion(string updatedText)
    {
        GameObject questionCanvas = GameObject.FindGameObjectWithTag("questionCanvasTag");
        Transform txtQuestion = questionCanvas.transform.Find("txtQuestion");
        txtQuestion.GetComponentInChildren<Text>().text = updatedText;
    }

    private void updateBtnAnswers(List<Answer> answers)
    {
        var tempAnswers = answers.ToArray();

        for (int i = 0; i < tempAnswers.Length; i++)
        {
            GameObject btn = buttons[i+1];
            btn.SetActive(true);
            btn.GetComponentInChildren<AnswerButton>().AnswerId = tempAnswers[i].AnswerId;
            btn.GetComponentInChildren<ButtonTransitioner>().ResetButtonColor();
            btn.GetComponentInChildren<Text>().text = tempAnswers[i].Content;
        }


        for (int i = tempAnswers.Length; i < maxNumberOfButtons; i++)
        {
            GameObject btn = buttons[i + 1];
            btn.SetActive(false);
        }

    }

    public bool ProcessClick(AnswerButton btnPressed)
    {
        if (IsCorrect(btnPressed.AnswerId))
        {
            UnityEngine.Debug.Log("Correct Answer");
            int possibleAnswers = currentQuestion.Answers.Count();
            var temp = (float)(possibleAnswers - attempts) / possibleAnswers;
            print("Score for Question:" + temp);
            pointScore += temp;
            getNextQuestion();
            attempts = 0;
            return true;
        }

        // Only count as an attempt if they haven't attempted the answer before
        if (btnPressed.image.color.r == .5 && btnPressed.image.color.g == .5 && btnPressed.image.color.b == .5)
        {
            attempts++;
        }
        return false;
    }

    public bool IsCorrect(int answerId)
    {
        Answer answer = currentQuestion.Answers.FirstOrDefault(ans => ans.AnswerId == answerId);

        if (answer != null)
        {
            return answer.IsCorrect;
        }

        return false;
    }


    public void ChangeCourse(int courseChange)
    {
        if (courseChange == -1 && currentCourse - 1 >= 0)
        {
            currentCourse--;
        }

        if (courseChange == -2 && currentCourse + 1 < CourseLibrary.Courses.Length)
        {
            currentCourse++;
        }

        if (courseChange == -3)
        {
            StartCoroutine(LoadEnvironments());
            CourseCanvas.alpha = 0;
            return;
        }

        CourseLibrary.UpdateTxtCourseName(CourseLibrary.Courses[currentCourse].Name);
    }


    // Pointer Project
    public override void Process()
    {
        m_Data.Reset();
        m_Data.position = new Vector2(m_Camera.pixelWidth / 2, m_Camera.pixelHeight / 2);

        eventSystem.RaycastAll(m_Data, m_RaycastResultCache);
        m_Data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        m_CurrentObject = m_Data.pointerCurrentRaycast.gameObject;

        m_RaycastResultCache.Clear();

        HandlePointerExitAndEnter(m_Data, m_CurrentObject);

        if (m_ClickAction.GetStateDown(m_TargetSource))
        {
            ProcessPress(m_Data);
        }

        if (m_ClickAction.GetStateUp(m_TargetSource))
        {
            ProcessRelease(m_Data);
        }

    }

    // Update is called once per frame
    public PointerEventData GetData()
    {
        return m_Data;
    }

    private void ProcessPress(PointerEventData data)
    {
        // Set Raycast
        data.pointerPressRaycast = data.pointerCurrentRaycast;

        // Check for Object Hit, get the down handler, call
        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(m_CurrentObject, data, ExecuteEvents.pointerDownHandler);

        // If no down handler, try and get click handler
        if (newPointerPress == null)
        {
            newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(m_CurrentObject);
        }

        // Set Data
        data.pressPosition = data.position;
        data.pointerPress = newPointerPress;
        data.rawPointerPress = m_CurrentObject;
    }

    private void ProcessRelease(PointerEventData data)
    {
        // Execute pointer up
        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);

        // Check for Click Handler
        GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(m_CurrentObject);

        // Check if actual
        if (data.pointerPress == pointerUpHandler)
        {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
        }

        // Clear selected GameObject
        eventSystem.SetSelectedGameObject(null);

        // Reset Data
        data.pressPosition = Vector2.zero;
        data.pointerPress = null;
        data.rawPointerPress = null;

    }
}
