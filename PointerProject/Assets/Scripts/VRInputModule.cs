using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;

public class VRInputModule : BaseInputModule
{
    public Camera m_Camera;
    public SteamVR_Input_Sources m_TargetSource;
    public SteamVR_Action_Boolean m_ClickAction;

    private GameObject m_CurrentObject = null;
    private PointerEventData m_Data = null;

    public EnvironmentLibrary EnvironmentLibrary;

    public CanvasGroup QuestionCanvas;
    public SteamVR_Action_Boolean IsTouchpadPressed;

    private List<Question> questions = new List<Question>();
    private Question currentQuestion;
    public static GameObject currentObject;
    int currentID;

    [Serializable]
    public class NewEnvironment : UnityEvent<Environment> { }
    public NewEnvironment OnNewEnvironment;

    private int currentEnvironmentIndex = 0;

    public Timer timer;
    private int maxNumberOfButtons = 4;
    private float score = 0;
    private float scoreTime = 0;
    private int attempts = 0;

    protected override void Start()
    {
        base.Start();
        timer = gameObject.AddComponent<Timer>();
        StartCoroutine(LoadEnvironments());

        IsTouchpadPressed.AddOnStateDownListener(TouchpadDown, SteamVR_Input_Sources.Any);
        IsTouchpadPressed.AddOnStateUpListener(TouchpadUp, SteamVR_Input_Sources.Any);
    }

    protected override void Awake()
    {
        base.Awake();
        
        m_Data = new PointerEventData(eventSystem);
    }

    private void TouchpadDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        QuestionCanvas.alpha = 1;
    }

    private void TouchpadUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        QuestionCanvas.alpha = 0;
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

    private void Select()
    {
        scoreTime += timer.TimeAsScore();
        UnityEngine.Debug.Log("Select() called!");
        Environment env = EnvironmentLibrary.Environments[currentEnvironmentIndex];
        questions = env.Questions;
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
                currentEnvironmentIndex = 0;
                updateTxtQuestion($"Game over! \n Time Score: {scoreTime} \n Points Score: {score} \n\n Total Score: {scoreTime + score}");
            }
            else
            {
                Select();
            }
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
            GameObject btn = GameObject.Find("btnAnswer" + (i+1));
            btn.GetComponentInChildren<AnswerButton>().AnswerId = tempAnswers[i].AnswerId;
            btn.GetComponentInChildren<ButtonTransitioner>().ResetButtonColor();
            btn.GetComponentInChildren<Text>().text = tempAnswers[i].Content;
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
            score += temp;
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
