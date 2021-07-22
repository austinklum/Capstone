using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using Valve.VR;
using Valve.VR.Extras;
using UnityEngine.UI;
using System.Linq;

public class PlayerInput : MonoBehaviour
{
    public EnvironmentLibrary EnvironmentLibrary;

    public SteamVR_Action_Boolean IsTouchpadPressed;

    public CanvasGroup QuestionCanvas;

    private List<Question> questions = new List<Question>();
    private Question currentQuestion;
    public static GameObject currentObject;
    int currentID;

    [Serializable]
    public class NewEnvironment : UnityEvent<Environment> { }
    public NewEnvironment OnNewEnvironment;

    private int currentEnvironmentIndex = 0;

    private void Start()
    {

        StartCoroutine(LoadEnvironments());

        IsTouchpadPressed.AddOnStateDownListener(TouchpadDown, SteamVR_Input_Sources.Any);
        IsTouchpadPressed.AddOnStateUpListener(TouchpadUp, SteamVR_Input_Sources.Any);

        SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerPressed, SteamVR_Input_Sources.Any);
        currentObject = null;
        currentID = 0;
        // SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerPressed, SteamVR_Input_Sources.Any);
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
    private void Update()
    {
        
    }

    private void TriggerPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);
        Debug.Log("Hits length: " + hits.Length);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            int id = hit.collider.gameObject.GetInstanceID();

            if (currentID != id)
            {
                currentID = id;
                currentObject = hit.collider.gameObject;

                Environment env = EnvironmentLibrary.Environments[currentEnvironmentIndex];
                
                AnswerButton btnPressed = currentObject.GetComponentInChildren<AnswerButton>();
                if (IsCorrect(btnPressed.AnswerId))
                {
                    UnityEngine.Debug.Log("Correct Answer");
                    currentObject.GetComponentInChildren<Text>().text = "Correct!!";
                    getNextQuestion();
                }
            }
        }
    }
    private bool IsCorrect(int answerId)
    {
        Answer answer = currentQuestion.Answers.FirstOrDefault(ans => ans.AnswerId == answerId);

        if (answer != null)
        {
            return answer.IsCorrect;
        }

        return false;
    }
    //private void TriggerPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    //{
    //    currentEnvironmentIndex++;
    //    UnityEngine.Debug.Log("NextEnv = " + currentEnvironmentIndex);

    //    if (currentEnvironmentIndex >= EnvironmentLibrary.Environments.Count)
    //    {
    //        currentEnvironmentIndex = 0;
    //    }

    //    Select();
    //}

    private void getNextQuestion()
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
                updateTxtQuestion("Game over!");
            }
            Select();
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
        //GameObject pnlAnswers = GameObject.FindGameObjectWithTag("pnlAnswers");
        //Transform btnAnswer1 = pnlAnswers.transform.Find("btnAnswer1");
        //Transform btnAnswer2 = pnlAnswers.transform.Find("btnAnswer2");
        //Transform btnAnswer3 = pnlAnswers.transform.Find("btnAnswer3");
        //Transform btnAnswer4 = pnlAnswers.transform.Find("btnAnswer4");


        //btnAnswer1.GetComponentInChildren<Text>().text = answers.First().Content;
        //answers.Remove(answers.First());

        //btnAnswer2.GetComponentInChildren<Text>().text = answers.First().Content;
        //answers.Remove(answers.First());

        //btnAnswer3.GetComponentInChildren<Text>().text = answers.First().Content;
        //answers.Remove(answers.First());

        //btnAnswer4.GetComponentInChildren<Text>().text = answers.First().Content;
        //answers.Remove(answers.First());

        var tempAnswers = answers.ToList();
        int count = tempAnswers.Count;
        for (int i = 1; i <= count; i++)
        {
            GameObject btn = GameObject.Find("btnAnswer" + i);
            btn.GetComponentInChildren<AnswerButton>().AnswerId = tempAnswers.First().AnswerId;
            btn.GetComponentInChildren<Text>().text = tempAnswers.First().Content;
            tempAnswers.Remove(tempAnswers.First());
        }
         
    }

    private void TouchpadDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        QuestionCanvas.alpha = 1;
    }

    private void TouchpadUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        QuestionCanvas.alpha = 0;
    }

    private void Select()
    {
        UnityEngine.Debug.Log("Select() called!");
        Environment env = EnvironmentLibrary.Environments[currentEnvironmentIndex];
        questions = env.Questions;
        OnNewEnvironment.Invoke(env);
        getNextQuestion();
    }
}
